using EssPcsService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

namespace EssPcsServer
{
    public class clsGatewayClient
    {
        #region Public Attributes
        /*
         * 통신 TCP 소켓
         * */
        public System.Net.Sockets.TcpClient Socket
        {
            get { return m_Socket; }
            set { m_Socket = value; }
        }

        public bool m_bConnected;
        IEssPcsService m_ServiceProxy = null;

        #endregion

        #region Private Attributes

        public System.Net.Sockets.TcpClient m_Socket = null;
        System.Threading.Thread _ClientThread = null;
        private string _IPAdress = string.Empty;
        private int _Port = 0;
        private bool _Connected = false;
        int _BufferLength = 4096;
        clsGateway m_clsGateway;
        clsSqlServer m_clsSqlServer;
        public Int32 m_nIndex = -1;
        public DateTime m_dtCom = DateTime.Now;

        System.Net.Sockets.NetworkStream _Stream;

        #endregion

        #region Constructor

        public clsGatewayClient(Int32 nIndex, System.Net.Sockets.TcpClient Socket, ref clsSqlServer sqlServer, ref IEssPcsService serviceProxy)
        {
            m_Socket = Socket;
            _Stream = m_Socket.GetStream();
            m_nIndex = nIndex;

            m_clsSqlServer = sqlServer;
            m_ServiceProxy = serviceProxy;

            String strIp = "";// m_ServiceProxy.GetSetting(nIndex).strGatewayIP;
            IPAddress localEndPoint = IPAddress.Parse(strIp);
            var ipAddress = IPAddress.Parse(strIp);
            var ipBytes = ipAddress.GetAddressBytes();
            
            m_clsGateway = new clsGateway(nIndex, 
                ref m_clsSqlServer,
                ipBytes,
                m_clsSqlServer.m_clsSettingInfo[m_nIndex].bSubnetMask,
                m_clsSqlServer.m_clsSettingInfo[m_nIndex].bGatewayAddress);

            //  쓰레드를 생성하고
            _ClientThread = new System.Threading.Thread(new System.Threading.ThreadStart(_ClientThradFunc));

            //  실행한다.
            _ClientThread.Start();

            m_bConnected = true;
        }

        #endregion

        #region Private Methods

        private void _ClientThradFunc()
        {
            int nLen;

            System.Threading.Thread.Sleep(1);
            _Stream.ReadTimeout = -1;
            //  연결되어 있으면
            while (m_bConnected)
            {
                m_dtCom = DateTime.Now;
                //  Sleep 1ms
                System.Threading.Thread.Sleep(1);

                String responseData = String.Empty;
                Byte[] data = new Byte[_BufferLength];

                try
                {
                    if (_Stream.CanRead)
                    {
                        //  stream에서 데이터를 읽어온다.
                        Int32 bytes = _Stream.Read(data, 0, data.Length);

                        //  정상적으로 데이터를 읽으면
                        if (bytes > 0)
                        {
                            if ((nLen = m_clsGateway.parsePacket(ref data, bytes, ref m_ServiceProxy)) != 0)
                            {
                                _Stream.Write(m_clsGateway.g_bSendBuffer, 0, nLen);
                            }
                        }
                        else
                        {
                            m_bConnected = false;
                            Debug.WriteLine("Receive 0");
                            break;
                        }
                    }
                    //  그렇지 않은 경우 쓰레드를 종료한다.
                    else
                    {
                        m_bConnected = false;
                        break;
                    }
                }
                catch (IOException iex)
                {
                    m_bConnected = false;
                    break;
                }
                catch (Exception ex)
                {
                    m_bConnected = false;
                    break;
                }
            }
        }

        #endregion

        #region Public Methods

        /*
         * 쓰레드 중지
         * */
        public void StopThread()
        {
            m_bConnected = false;

            if (_Stream != null)
            {
                //  스트림을 닫고
                _Stream.Close();
                _Stream = null;
            }

            //  쓰레드가 존재하고
            if (_ClientThread != null)
            {
                //  쓰레드를 종료하고
                _ClientThread.Abort();
                _ClientThread = null;
            }

            if (m_Socket != null)
            {
                //  소켓을 닫는다
                m_Socket.Close();
                m_Socket = null;
            }
        }

        public void setProxy(ref IEssPcsService serviceProxy)
        {
            m_ServiceProxy = serviceProxy;
        }
        #endregion

        #region Events

        public delegate void MessageReceivedDelegate(object sender, string Message);
        public event MessageReceivedDelegate MessageReceived;
        public delegate void MessageSentDelegate(object sender, string Message);
        public event MessageSentDelegate MessageSent;

        #endregion
    }
}
