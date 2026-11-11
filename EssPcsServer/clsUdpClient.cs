using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using EssPcsService;

namespace EssPcsServer
{
    public partial class clsUdpClient
    {
        public bool m_bRun = false;
        public UdpClient m_udpClient = null;

        private byte[] m_byteSend = new byte[1024];

        private byte[] m_byteReceive = new byte[1024];

        public string m_strIp;
        public int m_nPort;
        public int m_nLocalPort = 50001;

        public string[] m_strRtuIp = { "192.168.1.101", "192.168.1.103", "192.168.1.103" };
        public int[] m_nPcsId = { 1, 1, 2 };
        public float[] m_nPcsOut = { 0, 0, 0 };

        //  주기를 기다리지 않고 다음 루프에서 한 번 보낸다(시나리오 제한값 변경·해제용).
        public volatile bool[] m_bPcsSendNow = { false, false, false };

        //  보낸 뒤 주기 전송을 멈춘다(제한 해제값을 한 번만 보낼 때).
        public volatile bool[] m_bPcsSendOnce = { false, false, false };
        public int[] m_nPcsPeriod = { 30, 30, 30 };

        public Stopwatch[] m_swPcs = { new Stopwatch(), new Stopwatch(), new Stopwatch() };

        System.Threading.Thread _ClientThread = null;
        public int m_nComErrorCount = 0;

        String m_strDeviceName = "";
        String m_strLogError = "";
        IPEndPoint m_RecvPoint = null;

        public clsUdpClient()
        {
            int i;

            for (i = 0; i < 256; i++)
            {
                clsGenInfo clsInfo = new clsGenInfo();

                m_GenInfoList.Add(clsInfo);
            }
        }

        public byte CalcCRCByte(byte[] buf, int nStart, int nLen)
        {
            byte u8CRC = 0xFF;
            int i, j;

            for (i = 0; i < nLen; i++)
            {
                u8CRC = (byte)(u8CRC ^ buf[nStart++]);

                for (j = 0; j < 8; j++)
                {
                    if ((u8CRC & 0x01) != 0)
                    {
                        u8CRC = (byte)((u8CRC >> 1) ^ 0xD5);
                    }
                    else
                    {
                        u8CRC >>= 1;
                    }
                }
            }

            return u8CRC;
        }

        public void runPcsTimer(int nId)
        {
            if ((nId >= 1) && (nId <= 3))
            {
                m_swPcs[nId - 1].Restart();
            }
        }

        public void stopPcsTimer(int nId)
        {
            if ((nId >= 1) && (nId <= 3))
            {
                m_swPcs[nId - 1].Stop();
            }
        }

        /// <summary>출력 제한값을 즉시 보내고, bOnce 가 false 면 그 뒤로 주기 전송을 이어 간다.</summary>
        public void sendPcsOutNow(int nId, bool bOnce)
        {
            if ((nId >= 1) && (nId <= 3))
            {
                m_bPcsSendOnce[nId - 1] = bOnce;
                m_bPcsSendNow[nId - 1] = true;
            }
        }

        public bool IsPcsTimerRunning(int nId)
        {
            if ((nId >= 1) && (nId <= 3))
            {
                return m_swPcs[nId - 1].IsRunning;
            }

            return false;
        }

        public void UdpConnect(String strLocalIp,int nLocalPort, int nPort = -1, string strIp = "")
        {
            if (m_udpClient != null)
            {
                //m_udpClient.Client.Shutdown(SocketShutdown.Both);
                //m_udpClient.Client.Close();
                //m_udpClient = null;
                return;
            }

            try
            {
                m_strIp = strIp;
                m_nPort = nPort;
                m_nLocalPort = nLocalPort;

                m_udpClient = new UdpClient();
                IPEndPoint localEP = new IPEndPoint(IPAddress.Parse(strLocalIp), nLocalPort);
                m_udpClient.Client.Bind(localEP); 
                m_udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                if (nPort > 0)
                {
                    m_udpClient.Connect(IPAddress.Parse(m_strIp), m_nPort);
                }
                m_RecvPoint = new IPEndPoint(IPAddress.Any, 0);

                m_strIp = strIp;
                m_nPort = nPort;

                startThread();
            }
            catch (Exception e)
            {
            }
        }

        public void cancelConnect()
        {
            if (m_udpClient != null)
            {
                m_udpClient.Client.Shutdown(SocketShutdown.Both);
                m_udpClient.Client.Close();
                m_udpClient = null;
            }
        }

        void processSocketException()
        {
            if (m_udpClient != null)
            {
                m_udpClient.Client.Shutdown(SocketShutdown.Both);
                m_udpClient.Client.Close();
                m_udpClient = null;
            }

            if (m_bRun)
            {
                m_nComErrorCount++;
            }
            else
            {
            }
        }

        void startThread()
        {
            try
            {
                _ClientThread = new System.Threading.Thread(new System.Threading.ThreadStart(_ClientThreadFunc));
                _ClientThread.Start();
            }
            catch (SocketException ex)
            {
                processSocketException();
            }
        }

        public void Receive()
        {
            try
            {
                if (m_udpClient != null)
                {
                }
            }
            catch (SocketException e)
            {
            }
        }

        private int BcdToInt(byte b)
        {
            int i, n;

            i = (int)((b >> 4) & 0x0F);
            if (i > 9) i = 0;
            n = i * 10;

            i = (int)((b >> 0) & 0x0F);
            if (i > 9) i = 0;
            n += i;

            return n;
        }

        private byte IntToBcd(int i)
        {
            byte b1, b2;

            i %= 100;

            b1 = (byte)(i / 10);
            b1 <<= 4;
            b1 &= 0xF0;

            b2 = (byte)(i % 10);
            b2 &= 0x0F;

            return (byte)(b1 + b2);
        }

        UInt32 BytesToUInt32(byte[] buf, int offset)
        {
            UInt32 i;

            i = (UInt32)(buf[offset + 3] * 0x1000000 + buf[offset + 2] * 0x10000 + buf[offset + 1] * 0x100 + buf[offset + 0]);

            return i;
        }

        UInt16 BytesToUInt16(byte[] buf, int offset)
        {
            UInt16 i;

            i = (UInt16)(buf[offset + 1] * 0x100 + buf[offset + 0]);

            return i;
        }

        private void stopSocket()
        {
            if (m_udpClient != null)
            {
                stopThread();
                m_udpClient.Client.Shutdown(SocketShutdown.Both);
                m_udpClient.Client.Close();
                m_udpClient = null;
            }
        
            m_bRun = false;
        }

        public Byte[] m_bTxBuf;

        public void Send(byte[] byteSend, int nSend)
        {
            try
            {
                if (m_udpClient != null)
                {
                    /*
                    m_bTxBuf = new byte[nSend + 2];

                    m_nUdpTxCount++;
                    m_nSendLength = nSend;
                    m_bTxBuf[0] = (Byte)((m_nUdpTxCount >> 8) & 0xFF);
                    m_bTxBuf[1] = (Byte)((m_nUdpTxCount >> 0) & 0xFF);
                    Array.Copy(byteSend, 0, m_bTxBuf, 2, m_nSendLength);

                    m_udpClient.BeginSend(m_bTxBuf, m_nSendLength + 2, new AsyncCallback(SendCallback), m_udpClient);
                    */
                }
            }
            catch (Exception ex)
            {
                UdpDisconnect();
            }
            finally
            {
                //...m_bReqSend = false;
            }
        }

        void receiveCallback(IAsyncResult ar)
        {
            if (m_udpClient != null)
            {
                try
                {
                    Byte[] rxBytes = m_udpClient.EndReceive(ar, ref m_RecvPoint);
                    int nRxLen = rxBytes.Length;

                    //...clsLog.addTcpSocketLog("receiveCallback > " + nRxLen.ToString());

                    if (nRxLen == 6)
                    {
                        int nGen = rxBytes[0];
                        int nInv = rxBytes[1];
                        float fP = BitConverter.ToSingle(rxBytes, 2);

                        updateGenInfo(nGen, nInv, fP);
                        /*
                        if (m_bReqSend)
                        {
                            if (m_bTxBuf != null)
                            {
                                if (nRxLen == m_bTxBuf.Length)
                                {
                                    for (i = 0; i < nRxLen; i++)
                                    {
                                        if (m_bTxBuf[i] != rxBytes[i])
                                        {
                                            break;
                                        }
                                    }

                                    if (i == nRxLen)
                                    {
                                        m_bReqSend = false;
                                    }

                                    bChkSend = true;
                                }
                            }
                        }

                        if (!bChkSend)
                        {
                            if (rxBytes.Length > 3)
                            {
                                UInt32 nUdpRxCount = (UInt32)(rxBytes[0] + rxBytes[1] * 0x100 + rxBytes[2] * 0x10000 + rxBytes[3] * 0x1000000);

                                if (nUdpRxCount != m_nUdpRxCount)
                                {
                                    m_nUdpRxCount = nUdpRxCount;

                                    parsePacket(rxBytes);
                                }
                            }
                        }
                        */
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    if (m_udpClient != null)
                    {
                        m_udpClient.BeginReceive(new AsyncCallback(receiveCallback), null);
                    }
                }
            }
            else
            {
                m_bUdpStartReceive = false;
            }
        }

        void SendCallback(IAsyncResult ar)
        {
            try
            {
                UdpClient u = (UdpClient)ar.AsyncState;

                if (u != null)
                {
                    int bytesSent = u.EndSend(ar);
                }
            }
            catch (Exception e)
            {
                UdpDisconnect();
            }
        }

        private void DisconnectCallback(IAsyncResult ar)
        {
            //  Complete the disconnect request.
            if (m_udpClient != null)
            {
                try
                {
                    Socket client = (Socket)ar.AsyncState;

                    if (client != null)
                    {
                        client.EndDisconnect(ar);
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    
                }
            }
        }

        public void stopThread()
        {
            try
            {
                m_bRun = false;

                if (_ClientThread != null)
                {
                    _ClientThread.Interrupt();
                    _ClientThread.Join();
                    _ClientThread = null;
                }
            }
            catch (Exception ex)
            { 
            }
        }

        public void UdpDisconnect()
        {
            stopSocket();
        }
    }
}
