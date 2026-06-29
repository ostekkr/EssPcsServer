using EssPcsService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;

namespace EssPcsServer
{
    public class clsGatewayServer
    {
        #region Public Attributes

        /*
         * 서버 소켓 객체
         * */
        public System.Net.Sockets.TcpListener Socket
        {
            get { return _Listener; }
            set { _Listener = value; }
        }

        public List<clsGatewayClient> m_ConnectedClients = new List<clsGatewayClient>();
        public UInt64 m_nTcpTxCount = 0;
        public UInt64 m_nTcpRxCount = 0;
        IEssPcsService m_ServiceProxy = null;
        clsSqlServer g_clsSqlServer;
        int m_nIndex = 0;

        #endregion

        #region Private Attributes

        private System.Net.Sockets.TcpListener _Listener = null;
        int _BufferLength = 4096;
        System.Net.Sockets.NetworkStream _Stream;
        System.Threading.Thread _ListenThread = null;
        System.Threading.Thread _BindThread = null;

        #endregion

        #region Constructor

        /*
         * int Port : 서버 통신 포트
         * System.Net.IPAddress LocalEndPoint : 서버 IP 주소
         * Encoding EncodingInstance : 문자열 인코딩
         * clsMySQL mysql : MySQL 클래스
         * */
        public clsGatewayServer(int Port, System.Net.IPAddress LocalEndPont, int nIndex, ref clsSqlServer sqlServer, ref IEssPcsService serviceProxy)
        {
//            LocalEndPont = IPAddress.Parse("127.0.0.1");
            _Listener = new System.Net.Sockets.TcpListener(LocalEndPont, Port);
            g_clsSqlServer = sqlServer;
            m_ServiceProxy = serviceProxy;

            m_nIndex = nIndex;
#if DEBUG
            m_nIndex = 0;
#endif
        }

        #endregion

        #region Private Methods
        
        /*
         * Client를 추가한다.
         * object ClientObject : 추가할 Client 객체
         * */
        private void _AddClient(object ClientObject)
        {
            System.Net.Sockets.TcpClient tcpClient = (System.Net.Sockets.TcpClient)ClientObject;
            IPEndPoint ipEndPoint = (IPEndPoint)(tcpClient.Client.LocalEndPoint);
            Byte[] bIp = ipEndPoint.Address.GetAddressBytes();
            Int32 nPort = ipEndPoint.Port;
            Int32 nIndex = -1;
            int i;

            if ((nPort >= 8001) && (nPort <= 8032))
            {
                nIndex = nPort - 8001;
            }
            /*
            for (i = 0; i < g_clsSqlServer.m_clsSettingInfo.Count; i++)
            {
                String strIp = m_ServiceProxy.GetSetting(i).strGatewayIP;
                IPAddress localEndPoint = IPAddress.Parse(strIp);
                var ipAddress = IPAddress.Parse(strIp);
                var ipBytes = ipAddress.GetAddressBytes();

                if ((ipBytes[0] == bIp[0]) && (ipBytes[1] == bIp[1]) && (ipBytes[2] == bIp[2]) && (ipBytes[3] == bIp[3]))
                {
                    nIndex = i;
                    break;
                }
            }
            */

            /*
            if (nIndex != -1)
            {
                foreach (clsGatewayClient client in m_ConnectedClients)
                {
                    if (client.m_nIndex == nIndex)
                    {                        
                        client.m_Socket.Close();
                    }
                }                
            }
            */
            if (nIndex != -1)
            {
                /*
                 * 새로운 Client를 생성한다. 생성에 필요한 데이터는 서버 데이터를 그대로 사용한다.
                 * */

                clsGatewayClient Client = new clsGatewayClient(nIndex, tcpClient, ref g_clsSqlServer, ref m_ServiceProxy);
                /*
                 * 수신 데이터 처리 Delegate 선언
                 * */
                Client.MessageReceived += new clsGatewayClient.MessageReceivedDelegate(_Client_MessageReceived);
                /*
                 * 송신 데이터 처리 Delegate 선언
                 * */
                Client.MessageSent += new clsGatewayClient.MessageSentDelegate(_Client_MessageSent);
                /*
                 * 클라이언트를 리스트에 추가
                 * */
                
                m_ConnectedClients.Add(Client);

                ClientConnected(this, m_ConnectedClients[m_ConnectedClients.Count - 1]);
            }
            else
            {
                Debug.WriteLine("Refuse Connection ...\r\n");
                tcpClient.Close();   
            }
        }

        /*
         * Client 수신 Delegate 처리
         * object sender : Client 객체
         * string Message : Client 수신 문자열
         * */
        protected virtual void _Client_MessageReceived(object sender, string Message)
        {
            /*
             * 메인 프로그램의 DisplayLog delegate 호출
             * */
            DisplayLog(Message);
            /*
             * 수신 회수 증가
             * */
            m_nTcpRxCount++;
            /*
             * 수신 회수 갱신 delegate 호출
             * */
            UpdateRxCount(m_nTcpRxCount);
        }

        /*
         * Client 송신 Delegate 처리
         * object sender : Client 객체
         * string Message : Client 송신 문자열
         * */
        protected virtual void _Client_MessageSent(object sender, string Message)
        {
            /*
             * 메인 프로그램의 DisplayLog delegate 호출
             * */
            DisplayLog(Message);
            /*
             * 송신 회수 증가
             * */
            m_nTcpTxCount++;
            /*
             * 송신 회수 갱신 delegate 호출
             * */
            UpdateTxCount(m_nTcpTxCount);
        }

        /*
         * Tcp Listen Thread
         * */
        private void _ListenFunc()
        {
            //  Sleep 1ms
            System.Threading.Thread.Sleep(1);
            //  Server Socket start
            try
            {
                _Listener.Start();
            }
            catch (Exception ex)
            {
                clsLog.addExceptionLog(String.Format("GatewayServer._ListenFunc Start: {0}", ex.Message));
            }

            Byte[] bytes = new Byte[256];

            while (true)
            {
                //  Sleep 1ms
                System.Threading.Thread.Sleep(100);

                //  클라이언트에서 접속 요청이 있으면 해당 함수 호출
                try
                {
                    new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(_AddClient)).Start(_Listener.AcceptTcpClient());
                }
                catch (Exception ex)
                {
                    clsLog.addExceptionLog(String.Format("GatewayServer._ListenFunc AcceptTcpClient: {0}", ex.Message));
                }
            }
        }

        #endregion

        #region Public Methods

        /*
         * Server Listener 실행
         * */
        public void StartListening()
        {
            //  기존의 쓰레드가 존재하고
            if (_ListenThread != null)
            {
                //  동작 중이면
                if (_ListenThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    //  중지 시키고
                    _Listener.Stop();
                    //  쓰레드를 중지 시킨다.
                    _ListenThread.Abort();
                }
            }

            //  쓰레드를 생성하고
            _ListenThread = new System.Threading.Thread(new System.Threading.ThreadStart(_ListenFunc));
            //  동작 시킨다.
            _ListenThread.Start();
        }

        /*
         * Server Listener 중지
         * */
        public void StopListening()
        {
            //  쓰레드가 존재하고
            if (_ListenThread != null)
            {
                //  동작 중이면
                if (_ListenThread.ThreadState == System.Threading.ThreadState.Running)
                {
                    //  중지 시키고
                    _Listener.Stop();
                    //  쓰레드를 중지 시킨다.
                    _ListenThread.Abort();
                    _ListenThread = null;
                }
            }
        }

        /*
         * 모든 Client의 접속 해제
         * */
        public bool DisconnectAll()
        {
            bool bResult = false;

            /*
             * Client 쓰레드 정지
             * */
            foreach (clsGatewayClient cs in m_ConnectedClients)
            {
                /*
                 * 접속 확인
                 * */
                if (cs.m_bConnected)
                {
                    bResult = true;
                }
                cs.StopThread();
            }

            /*
             * 리스트에서 Client 객체 제거
             * */
            while (m_ConnectedClients.Count > 0)
            {
                m_ConnectedClients.RemoveAt(0);
            }

            return bResult;
        }

        public void checkConnections()
        {
            Int32 i;

            /*
             * 접속이 끊어진 Client가 있는지 확인
             */
            for (i = 0; i < m_ConnectedClients.Count; i++)
            {
                if (!m_ConnectedClients[i].m_bConnected || !m_ConnectedClients[i].m_Socket.Connected)
                {
                    m_ConnectedClients[i] = null;
                    m_ConnectedClients.RemoveAt(i);
                    continue;
                }

                TimeSpan ts = DateTime.Now - m_ConnectedClients[i].m_dtCom;

                if (Math.Abs(ts.Seconds) > 30)
                {
                    m_ConnectedClients[i].m_Socket.Close();
                }
            }
        }

        public void setProxy(ref IEssPcsService serviceProxy)
        {
            Int32 i;

            m_ServiceProxy = serviceProxy;

            for (i = 0; i < m_ConnectedClients.Count; i++)
            {
                m_ConnectedClients[i].setProxy(ref m_ServiceProxy);
            }
        }
        #endregion

        #region Events

        public delegate void ClientConnectedDelegate(object sender, clsGatewayClient Client);
        public event ClientConnectedDelegate ClientConnected;
        public delegate void DisplayLogDelegate(string strLog);
        public event DisplayLogDelegate DisplayLog;
        public delegate void UpdateTxCountDelegate(UInt64 nCount);
        public event UpdateTxCountDelegate UpdateTxCount;
        public delegate void UpdateRxCountDelegate(UInt64 nCount);
        public event UpdateRxCountDelegate UpdateRxCount;

        #endregion
    }
}
