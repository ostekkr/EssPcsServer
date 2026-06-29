using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using EssPcsService;

namespace EssPcsServer
{
    public class clsWaveInfo
    {
        public String m_strDeviceName = "";
        public String m_strLoc = "";
        public String m_strMsg = "";
        public Int32 m_nIndex = 0;
        public Int32 m_nDeviceNo = 0;
        public Int32 m_nFilePos = 0;
        public Int32 m_nFileType = 0;
        public String m_strFileType = "";

        public Byte[] m_bData = new Byte[16];
        public Int32 m_nEventCh = 0;
        public DateTime m_dtEvent = DateTime.Now;
        public Int32 m_nMs = 0;
        public UInt32 m_nStartRec = 0;
        public UInt32 m_nEndRec = 0;

        public Int32 m_nTrigger = 0;
        public Int32 m_nEvent = 0;

        public clsWaveInfo(int nIndex, clsEssPcsSettingInfo s, Byte[] bBuf, int nFilePos, int nFileType)
        {
            m_nIndex = nIndex;
            m_nDeviceNo = s.nDeviceNo;
            m_strDeviceName = s.strDevice;
            m_strLoc = s.strLOC;
            m_nFilePos = nFilePos;
            m_nFileType = nFileType;
            if (m_nFileType == 0)
            {
                m_strFileType = "INSTANT";
            }
            else
            {
                m_strFileType = "PHASOR";
            }
            Array.Copy(bBuf, m_bData, 16);

            parseBuf();
        }

        private void parseBuf()
        {
            UInt32 ulData = 0;
            UInt16 uData = 0;
            
            uData = (UInt16)(m_bData[0] + m_bData[1] * 256);
            m_nEventCh = ((uData >> 0) & 0x00FF);
            m_nEvent = ((uData >> 8) & 0x000F);
            m_nTrigger = ((uData >> 12) & 0x000F);
            m_nMs = (UInt16)(m_bData[2] + m_bData[3] * 256);
            ulData = (UInt32)(m_bData[4] + m_bData[5] * 0x100 + m_bData[6] * 0x10000 + m_bData[7] * 0x1000000);
            m_nStartRec = (UInt32)(m_bData[8] + m_bData[9] * 0x100 + m_bData[10] * 0x10000 + m_bData[11] * 0x1000000);
            m_nEndRec = (UInt32)(m_bData[12] + m_bData[13] * 0x100 + m_bData[14] * 0x10000 + m_bData[15] * 0x1000000);

            //  INS:4-1307398
            //  PHA:4-1826996

            /*
            if (m_nStartRec > 65535)
            {
                m_nStartRec = 65535;
            }

            if (m_nEndRec > 65535)
            {
                m_nEndRec = 65535;
            }
            */

            m_dtEvent = clsConstant.REF_DATETIME.AddSeconds(ulData);

            if (m_nFileType == 0)
            {
                m_strMsg = "순시사고> ";
            }
            else
            {
                m_strMsg = "페이저사고> ";
            }

            if (m_nTrigger == 1)
            {
                if (m_nEvent == 0)
                {
                    m_strMsg += String.Format("채널 {0} 상한 트리거", m_nEventCh);
                }
                else if (m_nEvent == 1)
                {
                    m_strMsg += String.Format("채널 {0} 하한 트리거", m_nEventCh);
                }
                else
                {
                    switch (m_nEvent)
                    {
                        case 2: 
                            m_strMsg += "유효전력 상한 트리거";  
                            break;

                        case 3: 
                            m_strMsg += "유효전력 하한 트리거"; 
                            break;

                        case 4: 
                            m_strMsg += "무효전력 상한 트리거"; 
                            break;

                        case 5: 
                            m_strMsg += "무효전력 하한 트리거"; 
                            break;

                        case 6: 
                            m_strMsg += "주파수 상한 트리거"; 
                            break;

                        case 7: 
                            m_strMsg += "주파수 하한 트리거"; 
                            break;

                        default: 
                            m_strMsg += String.Format("알 수 없는 레벨 트리거 [#{0}]", m_nEvent); 
                            break;
                    }
                }
            }
            else if (m_nTrigger == 2)
            {
                if (m_nEvent == 0)
                {
                    m_strMsg += String.Format("채널 {0} 변동 트리거", m_nEventCh);
                }
                else
                {
                    switch (m_nEvent)
                    {
                        case 2: 
                            m_strMsg += "유효전력 변동 트리거"; 
                            break;

                        case 4: 
                            m_strMsg += "무효전력 변동 트리거"; 
                            break;

                        case 6: 
                            m_strMsg += "주파수 변동 트리거"; 
                            break;

                        default: 
                            m_strMsg += String.Format("알 수 없는 변동 트리거 [#{0}]", m_nEvent); 
                            break;
                    }
                }
            }
            else if (m_nTrigger == 3)
            {
                switch (m_nEvent)
                {
                    case 2: 
                        m_strMsg += "유효전력 스윙 트리거"; 
                        break;

                    case 4: 
                        m_strMsg += "무효전력 스윙 트리거"; 
                        break;

                    default: 
                        m_strMsg += String.Format("알 수 없는 스윙 트리거 [#{0}]", m_nEvent); 
                        break;
                }
            }
            else if (m_nTrigger == 4)
            {
                m_strMsg += String.Format("채널 {0} 디지털 트리거", m_nEventCh);
            }
            else if (m_nTrigger == 5)
            {
                switch (m_nEvent)
                {
                    case 1: 
                        m_strMsg += "수동 트리거"; 
                        break;

                    case 2: 
                        m_strMsg += "자가 트리거"; 
                        break;

                    case 3: 
                        m_strMsg += "원격 트리거"; 
                        break;

                    case 4: 
                        m_strMsg += "외부 기동 트리거"; 
                        break;

                    default: 
                        m_strMsg += String.Format("알 수 없는 수동 트리거 [#{0}]", m_nEvent); 
                        break;
                }
            }
            else 
            {
                m_strMsg += String.Format("알 수 없는 트리거 [Event #{0} : Ch #{1}]", m_nEvent, m_nEventCh);
            }
        }
    }

    public class clsWaveFormInfo
    {
        public int m_nDeviceNo = 0;
        public string m_strDeviceName = "";
        public DateTime m_dtWaveForm = DateTime.Now;
        public int m_nMs = 0;
        public int m_nEvent = 0;
        public int m_nTrigger = 0;
        public int m_nEventCh = 0;
        public int m_nFilePos = 0;
        public int m_nFileType = 0;
        public string m_strFileType = "";
        public string m_strMsg = "";
        public int m_nSetupSize = 0;
        public uint m_nWaveSize = 0;
        public byte[] m_bWaveData;
        public byte[] m_bSetupData;

        public clsWaveFormInfo(clsWaveInfo info)
        {
            m_nDeviceNo = info.m_nDeviceNo;
            m_strDeviceName = info.m_strDeviceName;
            m_dtWaveForm = info.m_dtEvent;
            m_nMs = info.m_nMs;
            m_nEvent = info.m_nEvent;
            m_nTrigger = info.m_nTrigger;
            m_nEventCh = info.m_nEventCh;
            m_nFilePos = info.m_nFilePos;
            m_nFileType = info.m_nFileType;
            m_strFileType = info.m_strFileType;
            m_strMsg = info.m_strMsg;
            m_nSetupSize = clsConstant.PCS_SETUP_DATA_NUM;

            m_nWaveSize = (uint)(info.m_nEndRec - info.m_nStartRec + 1) * 512;

            m_bWaveData = new byte[m_nWaveSize];
            m_bSetupData = new byte[m_nSetupSize];
        }
    }

    public class clsSystemLog
    {
        public string m_strLog;
        public DateTime m_dtLog = DateTime.Now;
        public int m_nDevice;

        public clsSystemLog(int nDevice, string strLog, DateTime dtLog)
        {
            m_nDevice = nDevice;
            m_strLog = strLog;
            m_dtLog = dtLog;
        }
    }

    public class clsEventReq
    {
        public int m_nEventType = clsConstant.PQMS_SYSTEM_EVENT;
        public int m_nIndex = 0;
        public int m_nWaveEndBlock = 0;
        public int m_nFaultPoint = 0;
        public DateTime m_dtEvent = DateTime.Now;

        public clsEventReq(int nEventTpe, int nIndex, DateTime dtEvent, int nArg1, int nArg2)
        {
            m_nEventType = nEventTpe;
            m_nIndex = nIndex;
            m_dtEvent = dtEvent;

            if (m_nEventType == clsConstant.PQMS_WAVE_DATA)
            {
                m_nWaveEndBlock = nArg1;
                m_nFaultPoint = nArg2;
            }
        }
    }

    public partial class frmMain
    {
        private List<clsSystemLog> m_SystemLogList = new List<clsSystemLog>();
        private List<clsWaveInfo> m_WaveInfoList = new List<clsWaveInfo>();

        private bool saveWaveInfo(clsWaveInfo info)
        {
            if ((info.m_nStartRec == 0xFFFFFFFF) || (info.m_nEndRec == 0xFFFFFFFF))
            {
                return false;
            }
            else 
            {
                m_WaveInfoList.Add(info);
                return true;
            }
        }

        private void startInsFileReceive(int nIndex, clsWaveInfo info)
        {
            clsEssPcsMsg msg = new clsEssPcsMsg();
            int nDevice = info.m_nDeviceNo;

            msg.msgDevice = info.m_nDeviceNo;
            msg.msgType = clsConstant.ESSPCS_CMD_INS_WAVE;
            msg.msgIndex = 0;
            msg.msgDateTime = DateTime.Now;
            msg.msgSession = "";
            msg.msgResult = 0;
            msg.msgDeviceName = info.m_strDeviceName;
            msg.msgBuf = new Byte[8];
            msg.msgBuf[0] = (Byte)((info.m_nStartRec >> 0) & 0xFF);
            msg.msgBuf[1] = (Byte)((info.m_nStartRec >> 8) & 0xFF);
            msg.msgBuf[2] = (Byte)((info.m_nStartRec >> 16) & 0xFF);
            msg.msgBuf[3] = (Byte)((info.m_nStartRec >> 24) & 0xFF);
            msg.msgBuf[4] = (Byte)((info.m_nEndRec >> 0) & 0xFF);
            msg.msgBuf[5] = (Byte)((info.m_nEndRec >> 8) & 0xFF);
            msg.msgBuf[6] = (Byte)((info.m_nEndRec >> 16) & 0xFF);
            msg.msgBuf[7] = (Byte)((info.m_nEndRec >> 24) & 0xFF);

            m_rtuClient[nIndex].m_waveFormInfo = new clsWaveFormInfo(info);
            m_rtuClient[nIndex].CommandQueue.Enqueue(msg);
        }

        private void startPhaFileReceive(int nIndex, clsWaveInfo info)
        {
            clsEssPcsMsg msg = new clsEssPcsMsg();
            int nDevice = info.m_nDeviceNo;

            msg.msgDevice = info.m_nDeviceNo;
            msg.msgType = clsConstant.ESSPCS_CMD_PHA_WAVE;
            msg.msgIndex = 0;
            msg.msgDateTime = DateTime.Now;
            msg.msgSession = "";
            msg.msgResult = 0;
            msg.msgDeviceName = info.m_strDeviceName;
            msg.msgBuf = new Byte[8];
            msg.msgBuf[0] = (Byte)((info.m_nStartRec >> 0) & 0xFF);
            msg.msgBuf[1] = (Byte)((info.m_nStartRec >> 8) & 0xFF);
            msg.msgBuf[2] = (Byte)((info.m_nStartRec >> 16) & 0xFF);
            msg.msgBuf[3] = (Byte)((info.m_nStartRec >> 24) & 0xFF);
            msg.msgBuf[4] = (Byte)((info.m_nEndRec >> 0) & 0xFF);
            msg.msgBuf[5] = (Byte)((info.m_nEndRec >> 8) & 0xFF);
            msg.msgBuf[6] = (Byte)((info.m_nEndRec >> 16) & 0xFF);
            msg.msgBuf[7] = (Byte)((info.m_nEndRec >> 24) & 0xFF);

            m_rtuClient[nIndex].m_waveFormInfo = new clsWaveFormInfo(info);
            m_rtuClient[nIndex].CommandQueue.Enqueue(msg);
        }
    }
}
