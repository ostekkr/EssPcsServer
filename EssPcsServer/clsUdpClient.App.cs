using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EssPcsService;

namespace EssPcsServer
{
    public partial class clsUdpClient
    {
        public List<clsGenInfo> m_GenInfoList = new List<clsGenInfo>();

        public void updateGenInfo(int nGen, int nInv, float fOut)
        {
            if ((nGen > 0) && (nGen <= m_GenInfoList.Count))
            {
                nGen--;

                m_GenInfoList[nGen].m_fInvOut = fOut;
                m_GenInfoList[nGen].m_nGenIndex = nGen + 1;
                m_GenInfoList[nGen].m_nInvIndex = nInv;
                m_GenInfoList[nGen].m_dtUpdated = DateTime.Now;
                m_GenInfoList[nGen].m_bUpdated = true;

                //...clsLog.addTcpSocketLog(String.Format("updateGenInfo : Gen[{0}] Inv[{1}] fOut[{2}]", nGen, nInv, fOut));
            }
        }

        public void checkUpdate()
        {
            int i;
            
            for (i = 0; i < m_GenInfoList.Count; i++)
            {
                if (m_GenInfoList[i].m_bUpdated)
                {
                    TimeSpan ts = DateTime.Now - m_GenInfoList[i].m_dtUpdated;

                    if (Math.Abs(ts.TotalSeconds) > 3)
                    {
                        m_GenInfoList[i].m_fInvOut = 0;
                        m_GenInfoList[i].m_bUpdated = false;
                    }
                }
            }
        }

        public bool getGenOut(ref float fRet)
        {
            bool bRet = false;
            int i;

            for (i = 0; i < m_GenInfoList.Count; i++)
            {
                if (m_GenInfoList[i].m_bUpdated)
                {
                    fRet += m_GenInfoList[i].m_fInvOut;
                    bRet = true;
                }
            }

            return bRet;
        }

        public void runPcs(Int32 nId, Int32 nSleep)
        {
            if ((nId >= 1) && (nId <= 3))
            {
                Int32 i = nId - 1;

                String strIp = m_strRtuIp[i];
                Byte[] bFloat = BitConverter.GetBytes((float)nSleep);
                Byte[] bTx = new Byte[6] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                bTx[0] = (Byte)m_nPcsId[i];
                bTx[1] = 1;
                bTx[2] = bFloat[0];
                bTx[3] = bFloat[1];
                bTx[4] = bFloat[2];
                bTx[5] = bFloat[3];

                m_udpClient.Send(bTx, bTx.Length, strIp, 50001);

                m_swPcs[i].Restart();
            }
        }

        public void stopPcs(Int32 nId, Int32 nSleep)
        {
            if ((nId >= 1) && (nId <= 3))
            {
                Int32 i = nId - 1;

                String strIp = m_strRtuIp[i];
                Byte[] bFloat = BitConverter.GetBytes((float)nSleep);
                Byte[] bTx = new Byte[6] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

                bTx[0] = (Byte)m_nPcsId[i];
                bTx[1] = 0;
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
