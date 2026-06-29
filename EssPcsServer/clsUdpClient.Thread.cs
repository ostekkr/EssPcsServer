using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Diagnostics;
using System.ServiceModel;
using System.Runtime.Serialization;
using EssPcsService;

namespace EssPcsServer
{
    public partial class clsUdpClient
    {
        UInt16 m_nUdpTxCount = 0;
        public UInt32 m_nUdpRxCount = 0;
        bool m_bUdpStartReceive = false;
        Int32 i;

        private void _ClientThreadFunc()
        {
            m_bUdpStartReceive = false;
            m_bRun = true;

            while (m_bRun)
            {
                try
                {
                    System.Threading.Thread.Sleep(1);
                    
                    if (m_udpClient != null)
                    {
                        if (!m_bUdpStartReceive)
                        {
                            //...clsLog.addTcpSocketLog("m_bUdpStartReceive");
                            m_bUdpStartReceive = true;

                            m_udpClient.BeginReceive(new AsyncCallback(receiveCallback), null);
                        }
                    }

                    for (i = 0; i < 3; i++)
                    {
                        if (m_nPcsPeriod[i] > 0)
                        {
                            if (m_swPcs[i].ElapsedMilliseconds >= (m_nPcsPeriod[i] * 1000))
                            {
                                String strIp = m_strRtuIp[i];
                                Byte[] bFloat = BitConverter.GetBytes((float)m_nPcsOut[i]);
                                Byte[] bTx = new Byte[6] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                                bTx[0] = (Byte)m_nPcsId[i];
                                bTx[1] = 2;
                                bTx[2] = bFloat[0];
                                bTx[3] = bFloat[1];
                                bTx[4] = bFloat[2];
                                bTx[5] = bFloat[3];
                                
                                m_udpClient.Send(bTx, bTx.Length, strIp, 50001);

                                m_swPcs[i].Restart();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {

                }
            }

            try
            {
                if (m_udpClient != null)
                {
                }
                else
                {
                    processSocketException();
                }
            }
            catch (Exception ex)
            {

            }
        }

        void parsePacket(Byte[] rxBytes)
        {
        }
    }
}
