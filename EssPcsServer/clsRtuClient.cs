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
using System.IO.Ports;

namespace EssPcsServer
{
    public partial class clsRtuClient
    {
        public List<clsModbusMapAddrInfo> m_AddrInfoList = new List<clsModbusMapAddrInfo>();
        public List<clsEventReq> m_EventReqList = new List<clsEventReq>();
        public List<clsEssPcsMsg> m_EssPcsMsgList = new List<clsEssPcsMsg>();
        public clsEssPcsTrendInfo m_TrendInfo;
        public clsModBus m_clsModbus = new clsModBus();

        public int m_nConnectionStatus = clsConstant.PCS_RTU_SHUTDOWN;
        public bool m_bConnectionStatus = false;
        public DateTime m_dtConnectionStatus = DateTime.Now;

        public bool m_bRun = false;
        public string m_strStatus;
        public DateTime m_dateStatus;
        public bool m_bUpdateStatus = false;
        public SerialPort m_rtuClient = null;
        public clsEvent m_clsEvent = new clsEvent();
        public byte m_nModBusAddress = 1;

        private byte[] m_byteReceive = new byte[clsConstant.CDFR_ETHERNET_DATA_LENGTH];
        private byte[] m_byteSend = new byte[clsConstant.CDFR_ETHERNET_DATA_LENGTH];
        private int m_nSendLength = 0;
        private int m_nReceiveLength = 0;
        private int m_nReceiveFunction = clsConstant.MODBUS_FUNC_READ_INPUT;
        private int m_nReceiveByteCount = 0;
        private int _BufferLength = clsConstant.CDFR_ETHERNET_DATA_LENGTH;
        private string m_strIp;
        private int m_nPort;
        private bool m_bSent = true;

        public bool m_bStepResult = false;
        System.Threading.Thread _ClientThread = null;
        private int m_timeStart = Environment.TickCount;
        private int m_nMode = -1;
        public int m_nStep = -1;
        public int m_nErrorCount = 0;
        public int m_nComErrorCount = 0;
        public int m_nIndex = -1;
        private StreamWriter m_txtFileWriter = null;

        private int m_nCommandType = -1;
        private int m_nModBusFunction = 0;
        private int m_nModBusException = 0;
        private bool m_bModBusException = false;
        private int m_nModBusRegister = 100;
        private int m_nModBusRelayRegister = 0;
        private bool m_bModBusSingleCoil = false;
        private int m_nCommandTypeSBO = -1;
        private int m_nCommandTypeRelay = -1;
        private clsEssPcsSettingInfo m_clsSettingInfo;
        private bool m_bProcessReceive = false;

        public bool m_bDaqRun = false;
        public bool m_bReceiveTime = false;
        public bool m_bReceiveSetting = false;
        public bool m_bReceiveSystemEvent = false;
        public bool m_bReceivePqEvent = false;
        public bool m_bReceiveLoadProfile = false;
        public bool m_bReceiveWaveData = false;
        public bool m_bReceiveTrendData = false;
        public bool m_bReceiveSetupData = false;
        public bool m_bSendSetupData = false;
        public int m_nSystemEventCount = 0;
        public int m_nPqEventCount = 0;
        public int m_nLoadProfileCount = 0;

        public int m_nWaveIndex = -1;//clsConstant.PQMS_WAVE_START_INDEX;
        public DateTime m_dtWaveTime = DateTime.Now;
        public int m_nFaultPoint = 0;
        
        public byte[] m_bTrendDataBuffer = new byte[clsConstant.PCS_TREND_DATA_NUM * 2];
        public Stopwatch m_swTrendData = new Stopwatch();

        public DateTime m_timeDevice = DateTime.Now;

        //  public tcpClientStatusWaveFormDelegate tcpClientStatusWaveForm;  

        public Int32 m_nModbusMapIndex = 0;
        public UInt32 m_nInsWaveEnd = 0;
        public UInt32 m_nInsWaveIndex = 0;
        public UInt32 m_nInsWaveOffset = 0;

        public UInt32 m_nPhaWaveStart = 0;
        public UInt32 m_nPhaWaveEnd = 0;
        public UInt32 m_nPhaWaveIndex = 0;
        public UInt32 m_nPhaWaveOffset = 0;

        int m_nDeviceNo = 0;
        String m_strDeviceName = "";
        String m_strLogError = "";

        public Int32 m_nMsgImageIndex = -1;

        public List<clsStatusEventInfo> m_statusEventList = new List<clsStatusEventInfo>();
        public List<clsEssPcsTrendInfo> m_trendInfoList = new List<clsEssPcsTrendInfo>();
        public List<clsStatus> m_statusList = new List<clsStatus>();
        public bool m_bReqStopSocket = false;
        public Int32 m_nConnectStep = 0;
        public Int32 m_nDisConnectStep = 0;
        public DateTime m_dtConnectStep = DateTime.Now;
        public DateTime m_dtDisConnectStep = DateTime.Now;
        public Boolean m_bDestroyClient = false;

        public clsRtuClient(int index, clsEssPcsSettingInfo info, clsEssPcsTrendInfo trend, Boolean bDaqRun)
        {
            m_nIndex = index;
            m_clsSettingInfo = info;
            m_TrendInfo = trend;
            m_bDaqRun = bDaqRun;
            m_nDeviceNo = info.nDeviceNo;
            m_strDeviceName = info.strDevice;

            pauseThread();
            startThread();

            m_AddrInfoList.Add(new clsModbusMapAddrInfo(4, 0, 21));
            m_AddrInfoList.Add(new clsModbusMapAddrInfo(3, 0, 65));

            //  m_swTrendData.Start();
        }

        public void setConnectionStatus(int nStatus)
        {
            m_bConnectionStatus = false;
            m_nConnectionStatus = nStatus;
            m_dtConnectionStatus = DateTime.Now;
            m_TrendInfo.nConnectionStatus = nStatus;
            m_bConnectionStatus = true;
        }

        public void setModBusAddress(byte address)
        {
            m_nModBusAddress = address;
        }

        public void resumeThread()
        {
            m_bPauseThread = false;
        }

        /*
        public bool openLog()
        {
            bool bResult = false;
            string strFile;
            DateTime t = DateTime.Now;

            strFile = string.Format("log_{0:000}_{1}.txt", m_nIndex + 1, t.ToString("yyyyMMdd_HHmmss"));

#if USE_EXCEPTION
            try
            {
#endif 
                m_txtFileWriter = new StreamWriter(strFile);
                bResult = true;
#if USE_EXCEPTION
            }
            catch (Exception e)
            {
                makeStatus("openLog : " + e.Message, false, true);

                if (m_txtFileWriter != null)
                {
                    m_txtFileWriter.Close();
                    m_txtFileWriter = null;
                }
            }
#endif
            return bResult;
        }

        public void closeLog()
        {
            if (m_txtFileWriter != null)
            {
                m_txtFileWriter.Close();
                m_txtFileWriter = null;
            }
        }

        private void txLog(int nLen)
        {
            if (m_txtFileWriter != null)
            {
                string strLog;
                int i;

                strLog = string.Format("{0} TX[{1}] :", DateTime.Now, nLen);
                for (i = 0; i < nLen; i++)
                {
                    strLog += string.Format(" {0:X2}", m_byteSend[i]);
                }
                m_txtFileWriter.WriteLine(strLog);
            }
        }

        private void rxLog(int nLen)
        {
            if (m_txtFileWriter != null)
            {
                string strLog;
                int i;

                strLog = string.Format("{0} RX[{1}] :", DateTime.Now, nLen);
                for (i = 0; i < nLen; i++)
                {
                    strLog += string.Format(" {0:X2}", m_byteReceive[i]);
                }
                m_txtFileWriter.WriteLine(strLog);
            }
        }
        */


        public bool modbusParsePacket(int nLen)
        {
            bool bResult = false;

            if (nLen > 4)   /*  MIN LENGTH : Exception Code Length = 5   */
            {
                UInt16 calcCrc16 = clsCrc16.crc16_Bigendian(m_byteReceive, nLen - 2);
                UInt16 crc16 = (UInt16)(m_byteReceive[nLen - 2] * 0x100 + m_byteReceive[nLen - 1]);

                if (calcCrc16 == crc16)
                {
                    if (m_byteReceive[0] == (byte)m_clsSettingInfo.nDeviceAddr)
                    {
                        m_nReceiveFunction = m_byteReceive[1];

                        if (m_nReceiveFunction == m_clsModbus.g_bSendBuffer[1])
                        {
                            m_nReceiveLength = (int)m_byteReceive[2];

                            try
                            {
                                Array.Copy(m_byteReceive, 3, m_byteReceive, 0, m_nReceiveLength);

                                if (m_nReceiveFunction == clsConstant.ESSPCS_COMMAND_SINGLE_WRITE)
                                {
                                    m_nReceiveByteCount = m_nReceiveLength;
                                }
                                else if (m_nReceiveFunction == clsConstant.MODBUS_COMMAND_READ_HOLD_REGISTER)
                                {
                                    m_nReceiveByteCount = m_nReceiveLength;
                                }
                                else if (m_nReceiveFunction == clsConstant.MODBUS_FUNC_READ_INPUT)
                                {
                                    m_nReceiveByteCount = m_nReceiveLength;
                                }
                                else if (m_nReceiveFunction == clsConstant.CDFR_COMMAND_WRITE_AFE)
                                {
                                    m_nReceiveByteCount = (int)(m_byteReceive[4] * 0x100 + m_byteReceive[5]);
                                }
                                else
                                {
                                    m_nReceiveByteCount = 0;
                                }

                                bResult = true;
                            }
                            catch (Exception ex)
                            {
                                m_nReceiveByteCount = 0;
                            }
                        }
                        else
                        {
                            m_nReceiveByteCount = 0;
                        }
                    }
                }
                else
                {
                    m_nReceiveFunction = 0x00;
                }
            }

            return bResult;
        }

        public void resetTrendStopWatch()
        {
  //          if (m_bDaqRun)
 //           {
                m_nMode = clsConstant.MODBUS_MODE_RUN_TREND;
                m_swTrendData.Restart();
                m_bReceiveTrendData = false;
//            }
        }

        void makeStatus(string strMsg, bool bLog)
        {
            m_statusList.Add(new clsStatus(strMsg, bLog));

            resetTrendStopWatch();
        }
        
        public void setTcpConnectInfo(string strIp, int nPort)
        {
            m_strIp = strIp;
            m_nPort = nPort;
        }

        public void RtuConnect()
        {
            if (m_rtuClient != null)
            {
                return;
            }

            m_nMode = clsConstant.MODBUS_MODE_NONE;

#if USE_EXCEPTION
            try
            {
#endif
                m_rtuClient = new SerialPort();

                m_rtuClient.PortName = m_clsSettingInfo.strSerial;          // 포트 번호 (환경에 맞게 수정)
                m_rtuClient.BaudRate = 19200;            // 보드레이트
                m_rtuClient.Parity = Parity.None;       // 패리티 비트
                m_rtuClient.DataBits = 8;               // 데이터 비트
                m_rtuClient.StopBits = StopBits.One;    // 스톱 비트

                m_rtuClient.DataReceived += new SerialDataReceivedEventHandler(ReceiveCallback);

                m_rtuClient.Open();
                m_nConnectStep = 3;
                m_dtConnectStep = DateTime.Now;

                setConnectionStatus(clsConstant.PCS_RTU_CONNECTING);

                m_nStep = 0;
                m_bSent = true;
                m_bStepResult = false;

                Connected();

#if USE_EXCEPTION
            }
            catch (Exception e)
            {
                makeStatus("RtuConnect : " + e.Message, true);
                m_nConnectStep = -1;
            }
#endif
        }

        void Connected()
        {
#if USE_EXCEPTION
            try
            {
#endif

                if (m_rtuClient.IsOpen)
                {
                    m_bStepResult = true;
                    m_clsSettingInfo.bRequestSetupData = true;
                    m_nStep = 1;
                    m_bWaitInsFile = false;
                    m_bWaitPhaFile = false;
                    m_EssPcsMsgList.Clear();
                    m_bProcessReceive = false;
                    m_nConnectStep = 3;

                    makeStatus("Connected", true);

                    setConnectionStatus(clsConstant.PCS_RTU_CONNECTED);

                    resumeThread();

                    m_swTrendData.Start();
                }
                else
                {
                    makeStatus("Failed to connect", true);
                    
                    stopRtu();

                    m_nConnectStep = -1;
                }
#if USE_EXCEPTION
            }
            catch (SocketException ex)
            {
                makeStatus(string.Format("Failed to connect : {0}", ex.Message), true);

                stopRtu();
                
                m_nConnectStep = -1;
            }
#endif
        }

        public void processRtuException()
        {
            if (m_rtuClient != null)
            {
                if (m_rtuClient.IsOpen)
                {
                    m_rtuClient.Close();
                }

                m_rtuClient.Dispose();
                m_rtuClient = null;
            }

            m_nMode = clsConstant.MODBUS_MODE_NONE;
            m_bStepResult = false;
            m_nStep = -1;

            if (m_bRun)
            {
                m_nComErrorCount++;

                setConnectionStatus(clsConstant.PCS_RTU_CONNECTION_FAILED);
            }
            else
            {
                setConnectionStatus(clsConstant.PCS_RTU_SHUTDOWN);
            }
        }

        public void runFirstStep()
        {
            if (m_clsSettingInfo.bRequestSetupData)
            {
                receiveSetupData();
            }
            else
            {
                receiveTrendData();
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

        private bool parseTimePacket()
        {
            bool result = false;
            int year, month, day, hour, minute, second;

            if (m_nReceiveByteCount == 6)
            {
                year = 2000 + BcdToInt(m_byteReceive[3]);
                month = BcdToInt(m_byteReceive[4]);
                day = BcdToInt(m_byteReceive[5]);
                hour = BcdToInt(m_byteReceive[6]);
                minute = BcdToInt(m_byteReceive[7]);
                second = BcdToInt(m_byteReceive[8]);

                m_timeDevice    = new DateTime(year, month, day, hour, minute, second);
                result = true;
            }

            return result;
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

        private int makeSettingPacket()
        {
            byte[] byteArray;
            byte bVal;
            int len = 0, i;
            int address = 0, length = 29;

            /*  MODBUS ETHERNET HEADER */
            /*
            m_byteSend[len++] = 0;
            m_byteSend[len++] = 0;
            m_byteSend[len++] = 0;
            m_byteSend[len++] = 0;
            m_byteSend[len++] = 0;
            m_byteSend[len++] = (byte)(length * 2 + 7);

            m_byteSend[len++] = m_nModBusAddress;
            m_byteSend[len++] = 0x10;
            m_byteSend[len++] = (byte)((address >> 8) & 0xff);
            m_byteSend[len++] = (byte)((address >> 0) & 0xff);
            m_byteSend[len++] = (byte)((length >> 8) & 0xff);
            m_byteSend[len++] = (byte)((length >> 0) & 0xff);
            m_byteSend[len++] = (byte)(length * 2);

            //  [0000] PT비 : Len 4
            byteArray = BitConverter.GetBytes(m_clsSettingInfo.fPTRATIO);
            Array.Copy(byteArray, 0, m_byteSend, len, 4);
            len += 4;
            //  [0004] CT비 : Len 2
            m_byteSend[len++] = (byte)(m_clsSettingInfo.nCTRATIO >> 0);
            m_byteSend[len++] = (byte)(m_clsSettingInfo.nCTRATIO >> 8);
            //  [0006] DI Config : Len 1
            bVal = (byte)(m_clsSettingInfo.nDICONFIG1 & 0x03);
            bVal |= (byte)((m_clsSettingInfo.nDICONFIG2 << 2) & 0x0C);
            bVal |= (byte)((m_clsSettingInfo.nDICONFIG3 << 4) & 0x30);
            bVal |= (byte)((m_clsSettingInfo.nDICONFIG4 << 6) & 0xC0);
            m_byteSend[len++] = bVal;
            //  [0007] Relay Config : Len 1
            bVal = (byte)(m_clsSettingInfo.nRYCONFIG1 & 0x03);
            bVal |= (byte)((m_clsSettingInfo.nRYCONFIG2 << 2) & 0x0C);
            m_byteSend[len++] = bVal;
            //  [0008] PQ En : Bit 0
            bVal = 0;
            if (m_clsSettingInfo.bPQEN)
            {
                bVal = 1;
            }
            //  [0008] FREQ : Bit 0
            if(m_clsSettingInfo.nFREQ == 1)
            {
                bVal |= 0x02;
            }
            //  [0008] Sample Rate : Bit 2 - 3
            bVal |= (byte)((m_clsSettingInfo.nSAMPLERATE << 2) & 0x0C);
            //  [0008] Wire Mode : Bit 4 - 5
            bVal |= (byte)((m_clsSettingInfo.nWIREMODE << 4) & 0x30);
            //  [0008] Demand 주기 : Bit 6 - 7
            bVal |= (byte)((m_clsSettingInfo.nDEMAND << 6) & 0xC0);
            m_byteSend[len++] = bVal;
            //  [0009] Baudrate : Bit 0 -1
            bVal = (byte)(m_clsSettingInfo.nBAUDRATE & 0x03);
            //  [0009] 무선 통신 : Bit 2 - 3
            bVal |= (byte)((m_clsSettingInfo.nWIRELESS << 2) & 0x0C);
            //  [0009] RS485 : Bit 4
            bVal |= (byte)((m_clsSettingInfo.nRS485 << 4) & 0x10);
            //  [0009] Wavelet : Bit 5
            bVal |= (byte)((m_clsSettingInfo.nWAVELET << 5) & 0x20);
            //  [0009] Wavelet Level : Bit 6 - 7
            bVal |= (byte)((m_clsSettingInfo.nWAVELETLEVEL << 6) & 0xC0);
            m_byteSend[len++] = bVal;
            //  [0010] PT비 : Bit 0 - 1
            bVal = (byte)(m_clsSettingInfo.nPTRATE & 0x03);
            //  [0010] CT비 : Bit 2 - 3
            bVal |= (byte)((m_clsSettingInfo.nCTRATE << 2) & 0x0C);
            //  [0010] HOST 요청 주기 : Bit 4 - 5
            bVal |= (byte)((m_clsSettingInfo.nHOST << 4) & 0x30);
            //  [0010] Wave 기록 시간 : Bit 6 - 7
            bVal |= (byte)((m_clsSettingInfo.nSECTOR << 6) & 0xC0);
            m_byteSend[len++] = bVal;
            //  [0011] MODBUS Address : Len 1
            m_byteSend[len++] = (byte)m_clsSettingInfo.nMODBUS;
            // [0012] IP Address : Len 4
            var ipAddress = IPAddress.Parse(m_clsSettingInfo.strIPADDRESS);
            var ipBytes=ipAddress.GetAddressBytes();
            m_byteSend[len++] = (byte)ipBytes[0];
            m_byteSend[len++] = (byte)ipBytes[1];
            m_byteSend[len++] = (byte)ipBytes[2];
            m_byteSend[len++] = (byte)ipBytes[3];
            //  [0016] Subnet mask : Len 4
            var subnetMask = IPAddress.Parse(m_clsSettingInfo.strSUBNETMASK);
            var subnetMaskBytes = subnetMask.GetAddressBytes();
            m_byteSend[len++] = (byte)subnetMaskBytes[0];
            m_byteSend[len++] = (byte)subnetMaskBytes[1];
            m_byteSend[len++] = (byte)subnetMaskBytes[2];
            m_byteSend[len++] = (byte)subnetMaskBytes[3];
            //  [0020] Gateway Address : Len 4
            var gwAddress = IPAddress.Parse(m_clsSettingInfo.strGWADDRESS);
            var gwBytes = gwAddress.GetAddressBytes();
            m_byteSend[len++] = (byte)gwBytes[0];
            m_byteSend[len++] = (byte)gwBytes[1];
            m_byteSend[len++] = (byte)gwBytes[2];
            m_byteSend[len++] = (byte)gwBytes[3];
            //  [0024] Password : Len 4
            byte[] password = new System.Text.UTF8Encoding().GetBytes(m_clsSettingInfo.strPASSWORD);
            m_byteSend[len++] = password[0];
            m_byteSend[len++] = password[1];
            m_byteSend[len++] = password[2];
            m_byteSend[len++] = password[3];
            //  [0028] 요율 Rate #1 : Bit 0 - 1
            m_byteSend[len] = (byte)(m_clsSettingInfo.nRATE1 & 0x03);
            //  [0028] 요율 Rate 시간 #1 : Bit 2 - 7
            m_byteSend[len++] += (byte)((m_clsSettingInfo.nRATETIME1 << 2) & 0xFC);
            //  [0029] 요율 Rate #2 : Bit 0 - 1
            m_byteSend[len] = (byte)(m_clsSettingInfo.nRATE2 & 0x03);
            //  [0029] 요율 Rate 시간 #2 : Bit 2 - 7
            m_byteSend[len++] += (byte)((m_clsSettingInfo.nRATETIME2 << 2) & 0xFC);
            //  [0030] 요율 Rate #3 : Bit 0 - 1
            m_byteSend[len] = (byte)(m_clsSettingInfo.nRATE3 & 0x03);
            //  [0030] 요율 Rate 시간 #3 : Bit 2 - 7
            m_byteSend[len++] += (byte)((m_clsSettingInfo.nRATETIME3 << 2) & 0xFC);
            //  [0031] 요율 Rate #4 : Bit 0 - 1
            m_byteSend[len] = (byte)(m_clsSettingInfo.nRATE4 & 0x03);
            //  [0031] 요율 Rate 시간 #4 : Bit 2 - 7
            m_byteSend[len++] += (byte)((m_clsSettingInfo.nRATETIME4 << 2) & 0xFC);

            //  [0032] Pre Trigger Cycle : Bit 0 - 1
            m_byteSend[len] = (byte)(m_clsSettingInfo.nPTCycle & 0x03);
            //  [0032] Fault Pickup Delay (ms) : Bit 2 - 7
            m_byteSend[len++] += (byte)((m_clsSettingInfo.nFPDelay << 2) & 0xFC);
            //  [0033] Voltage Low Limit (%) : Bit 0 - 3
            m_byteSend[len] = (byte)(m_clsSettingInfo.nVLLimit & 0x0F);
            //  [0033] Voltage High Limit (%) : Bit 4 - 7
            m_byteSend[len++] += (byte)((m_clsSettingInfo.nVHLimit << 4) & 0xF0);
            //  [0034] Pulse Time Sync (분) : Len 1
            m_byteSend[len++] = (byte)(m_clsSettingInfo.nPTSMinute);
            //  [0035] Pulse Time Sync (시) : Len 1
            m_byteSend[len++] = (byte)(m_clsSettingInfo.nPTSHour);
            //  [0036] 장치 Site 이름 : Len 16
            byte[] strDevice = Encoding.UTF8.GetBytes(m_clsSettingInfo.strDevice);
            for (i = 0; (i < 16) && (i < strDevice.Length); i++)
            {
                m_byteSend[len++] = (byte)strDevice[i];
            }
            for (; i < 16; i++)
            {
                m_byteSend[len++] = 0;
            }
            //  [0052] 예약 : Len 6
            for (i = 0; i < 6; i++)
            {
                m_byteSend[len++] = 0;
            }
            */
            return len;
        }

        private bool parseSetupDataPacket()
        {
            String strVal = "";
            byte[] strDevice = new byte[17];
            bool result = false;
            int i, j;

            if (m_nReceiveByteCount == clsConstant.PCS_SETUP_DATA_NUM)
            {
                UInt16 uData;

                Array.Copy(m_byteReceive, m_clsSettingInfo.bSetupData, clsConstant.PCS_SETUP_DATA_NUM);

                //  001: AI보드 수
                m_clsSettingInfo.nAiBoard = m_byteReceive[0];

                //  002: DI보드 수
                m_clsSettingInfo.nDiBoard = m_byteReceive[1];

                //  003: SPARE
                m_clsSettingInfo.nSpare = m_byteReceive[2];

                //  004: 피더 수
                m_clsSettingInfo.nFeeder = m_byteReceive[3];

                //  005 - 006
                uData = (UInt16)(m_byteReceive[4] + m_byteReceive[5] * 256);
                //  BIT 00: 주파수 0:60, 1: 50
                m_clsSettingInfo.nFreq = (uData & 0x0001) == 0x0001 ? 1 : 0;
                //  BIT 01: 고장기록 순시파형
                m_clsSettingInfo.bInsSave = (uData & 0x0002) == 0x0002 ? true : false;
                //  BIT 02: 고장기록 페이저
                m_clsSettingInfo.bPhaSave = (uData & 0x0004) == 0x0004 ? true : false;
                //  BIT 03 - 04: 시각 동기화 (0:내부, 1: GPS, 2: IRIG B000, 3: IRIG B120)
                m_clsSettingInfo.nTimeSync = (uData >> 3) & 0x0003;
                //  BIT 05: PMU Module
                m_clsSettingInfo.bPmu = (uData & 0x0020) == 0x0020 ? true : false;
                //  BIT 06: 1PPS
                m_clsSettingInfo.b1PPS = (uData & 0x0040) == 0x0040 ? true : false;
                //  BIT 07: TCP/UDP (0:TCP)
                m_clsSettingInfo.nTcp = (uData >> 7) & 0x0001;
                //  BIT 08: Trigger Manual
                m_clsSettingInfo.bTriggerManual = (uData & 0x0100) == 0x0100 ? true : false;
                //  BIT 09: Trigger Host
                m_clsSettingInfo.bTriggerHost = (uData & 0x0200) == 0x0200 ? true : false;
                //  BIT 10: Trigger Ext
                m_clsSettingInfo.bTriggerExt = (uData & 0x0400) == 0x0400 ? true : false;
                //  BIT 11 - 14: Relay Enable/Disable
                m_clsSettingInfo.bRelay[0] = (uData & 0x0800) == 0x0800 ? true : false;
                m_clsSettingInfo.bRelay[1] = (uData & 0x1000) == 0x1000 ? true : false;
                m_clsSettingInfo.bRelay[2] = (uData & 0x2000) == 0x2000 ? true : false;
                m_clsSettingInfo.bRelay[3] = (uData & 0x4000) == 0x4000 ? true : false;

                //  #007 - #010: IP Address
                m_clsSettingInfo.bIpAddress[0] = m_byteReceive[6];
                m_clsSettingInfo.bIpAddress[1] = m_byteReceive[7];
                m_clsSettingInfo.bIpAddress[2] = m_byteReceive[8];
                m_clsSettingInfo.bIpAddress[3] = m_byteReceive[9];

                //  #011 - #014: Gateway Address
                m_clsSettingInfo.bGatewayAddress[0] = m_byteReceive[10];
                m_clsSettingInfo.bGatewayAddress[1] = m_byteReceive[11];
                m_clsSettingInfo.bGatewayAddress[2] = m_byteReceive[12];
                m_clsSettingInfo.bGatewayAddress[3] = m_byteReceive[13];

                //  #015 - #016: PMU ID
                m_clsSettingInfo.uPmuID = (UInt16)(m_byteReceive[14] + m_byteReceive[15] * 256);

                //  #017: 순시 Pre
                m_clsSettingInfo.nInsPre = (UInt16)(m_byteReceive[16] * 100);

                //  #018: 순시 Post
                m_clsSettingInfo.nInsPost = (UInt16)(m_byteReceive[17] * 100);

                //  #019: PMU Data Rate [Hz]
                m_clsSettingInfo.nPmuDataRate = m_byteReceive[18];

                //  #020: 페이저 Pre
                m_clsSettingInfo.nPhaPre = (UInt16)(m_byteReceive[19] * 100);

                //  #021 - #022: 페이저 Post
                uData = (UInt16)(m_byteReceive[20] + m_byteReceive[21] * 256);
                m_clsSettingInfo.nPhaPost = (UInt16)(uData * 100);

                //  #023 - #026: Subnet Mask
                m_clsSettingInfo.bSubnetMask[0] = m_byteReceive[22];
                m_clsSettingInfo.bSubnetMask[1] = m_byteReceive[23];
                m_clsSettingInfo.bSubnetMask[2] = m_byteReceive[24];
                m_clsSettingInfo.bSubnetMask[3] = m_byteReceive[25];

                //  #027 - #028: 주파수 상한 기동 주파수
                uData = (UInt16)(m_byteReceive[26] + m_byteReceive[27] * 256);
                m_clsSettingInfo.fFreqTrigUp = (float)(uData / 10.0f);

                //  #029 - #030: 주파수 하한 기동 주파수
                uData = (UInt16)(m_byteReceive[28] + m_byteReceive[29] * 256);
                m_clsSettingInfo.fFreqTrigDn = (float)(uData / 10.0f);

                //  #031: 주파수 ROC 기준 주파수
                m_clsSettingInfo.fFreqTrigRocRef = (float)(m_byteReceive[30] / 10.0f);

                //  #032: BIT 0 - 주파수 상한 Enable
                m_clsSettingInfo.bFreqTrigUp = (m_byteReceive[31] & 0x01) == 0x01 ? true :false;

                //  #032: BIT 1 - 주파수 하한 Enable
                m_clsSettingInfo.bFreqTrigDn = (m_byteReceive[31] & 0x02) == 0x02 ? true : false;

                //  #032: BIT 2 - 7 - 주파수 변동 기동 주기 설정 (0: Disable, 1 ~ 15 Cycle)
                m_clsSettingInfo.bFreqTrigRoc = (Byte)((m_byteReceive[31] >> 4) & 0x0F);

                //  #033 - #036: WI-FI IP Address
                m_clsSettingInfo.bWifiIpAddress[0] = m_byteReceive[32];
                m_clsSettingInfo.bWifiIpAddress[1] = m_byteReceive[33];
                m_clsSettingInfo.bWifiIpAddress[2] = m_byteReceive[34];
                m_clsSettingInfo.bWifiIpAddress[3] = m_byteReceive[35];

                //  #037 - #040: WI-FI Gateway
                m_clsSettingInfo.bWifiGateway[0] = m_byteReceive[36];
                m_clsSettingInfo.bWifiGateway[1] = m_byteReceive[37];
                m_clsSettingInfo.bWifiGateway[2] = m_byteReceive[38];
                m_clsSettingInfo.bWifiGateway[3] = m_byteReceive[39];

                //  #041 - #044: WI-FI Subnet Mask
                m_clsSettingInfo.bWifiSubnetMask[0] = m_byteReceive[40];
                m_clsSettingInfo.bWifiSubnetMask[1] = m_byteReceive[41];
                m_clsSettingInfo.bWifiSubnetMask[2] = m_byteReceive[42];
                m_clsSettingInfo.bWifiSubnetMask[3] = m_byteReceive[43];

                //  #045 - #046: WI-FI Port
                m_clsSettingInfo.nWifiPort = (UInt16)(m_byteReceive[44] + m_byteReceive[45] * 256);

                //  #047: WI-FI Encryption (0:None, 1:WPA, 2:WEP)
                m_clsSettingInfo.bWifiEncryption = (Byte)(m_byteReceive[46] & 0x03);

                //  #048: WI-FI Use
                m_clsSettingInfo.bWifiUse = m_byteReceive[47] == 0 ? false : true;

                //  #049 - #060: WI-FI SSID
                {
                    Byte[] bData = new Byte[12];

                    Array.Copy(m_byteReceive, 48, bData, 0, 12);
                    strVal = Encoding.Default.GetString(bData);
                    //...m_clsSettingInfo.strWifiSSID = strVal.Replace("\0", String.Empty);
                }

                //  #061: SYNC Hour
                m_clsSettingInfo.bSyncHour = m_byteReceive[60];

                //  #062: SYNC Minute
                m_clsSettingInfo.bSyncMinute = m_byteReceive[61];

                //  #063: SYNC Second
                m_clsSettingInfo.bSyncSecond = m_byteReceive[62];

                //  #064: Reserved
                m_clsSettingInfo.bReserved1 = m_byteReceive[63];

                //  #065 - #076: WI-FI Password
                {
                    Byte[] bData = new Byte[12];

                    Array.Copy(m_byteReceive, 64, bData, 0, 12);
                    strVal = Encoding.Default.GetString(bData);
                    m_clsSettingInfo.strWifiPassword = strVal.Replace("\0", String.Empty);
                }

                //  #077 - #080: Reserved
                m_clsSettingInfo.bReserved2[0] = m_byteReceive[76];
                m_clsSettingInfo.bReserved2[1] = m_byteReceive[77];
                m_clsSettingInfo.bReserved2[2] = m_byteReceive[78];
                m_clsSettingInfo.bReserved2[3] = m_byteReceive[79];

                //  #081 - #096: PMU Station Name
                {
                    Byte[] bData = new Byte[16];

                    Array.Copy(m_byteReceive, 80, bData, 0, 16);
                    strVal = Encoding.Default.GetString(bData);
                    m_clsSettingInfo.strPmuStationName = strVal.Replace("\0", String.Empty);
                }

                for (i = 0, j = 96; i < 16; i++, j += 12)
                {
                    Byte[] bData = new Byte[12];

                    Array.Copy(m_byteReceive, j, bData, 0, 12);

                    //  #108: BIT 0 - 1 채널 종류 (0:PT, 1:CT, 2:4-20mA, 3:ZeroPT
                    //  #108: BIT 2 상한 Enable
                    //  #108: BIT 3 하한 Enable
                    //  #108: BIT 4 - 7 변동 기동 주기 설정
                }

                for (i = 0, j = 316; i < 16; i++, j += 16)
                {
                    Byte[] bData = new Byte[16];

                    Array.Copy(m_byteReceive, j, bData, 0, 16);
                    strVal = Encoding.Default.GetString(bData);
                }

                uData = (UInt16)(m_byteReceive[288] + m_byteReceive[289] * 256);

                uData = (UInt16)(m_byteReceive[290] + m_byteReceive[291] * 256);

                for (i = 0, j = 572; i < 16; i++, j += 16)
                {
                    Byte[] bData = new Byte[16];

                    Array.Copy(m_byteReceive, j, bData, 0, 16);
                    strVal = Encoding.Default.GetString(bData);
                }

                //  #293 - #296 정격 유효전력 [kW]
                m_clsSettingInfo.nRateP = (UInt32)(m_byteReceive[292] + m_byteReceive[293] * 0x100 + m_byteReceive[294] * 0x10000 + m_byteReceive[295] * 0x1000000);

                //  #297 - #300: 정격 무효전력 [kVAR]
                m_clsSettingInfo.nRateQ = (UInt32)(m_byteReceive[296] + m_byteReceive[297] * 0x100 + m_byteReceive[298] * 0x10000 + m_byteReceive[299] * 0x1000000);

                //  #301 - #302: 유효 상한
                uData = (UInt16)(m_byteReceive[300] + m_byteReceive[301] * 0x100);
                m_clsSettingInfo.fTrigUpP = (float)(uData / 10.0f);

                //  #303 - #304: 무효 상한
                uData = (UInt16)(m_byteReceive[302] + m_byteReceive[303] * 0x100);
                m_clsSettingInfo.fTrigUpQ = (float)(uData / 10.0f);

                //  #305 - #306: 유효 하한
                uData = (UInt16)(m_byteReceive[304] + m_byteReceive[305] * 0x100);
                m_clsSettingInfo.fTrigDnP = (float)(uData / 10.0f);

                //  #307 - #308: 무효 하한
                uData = (UInt16)(m_byteReceive[306] + m_byteReceive[307] * 0x100);
                m_clsSettingInfo.fTrigDnQ = (float)(uData / 10.0f);

                //  #309: P 변동
                m_clsSettingInfo.fRocP = (float)(m_byteReceive[308] / 10.0f);

                //  #310: Q 변동
                m_clsSettingInfo.fRocQ = (float)(m_byteReceive[309] / 10.0f);

                //  #311: P Swing
                m_clsSettingInfo.fSwingP = (float)(m_byteReceive[310] / 10.0f);

                //  #312: Q Swing
                m_clsSettingInfo.fSwingQ = (float)(m_byteReceive[311] / 10.0f);

                uData = (UInt16)(m_byteReceive[312] + m_byteReceive[313] * 0x100);

                //  #313 - #314: BIT 0 P 상한 Enable / Disable
                m_clsSettingInfo.bTrigUpP = (uData & 0x0001) == 0x0001 ? true : false;

                //  #313 - #314: BIT 1 P 하한 Enable / Disable
                m_clsSettingInfo.bTrigDnP = (uData & 0x0002) == 0x0002 ? true : false;

                //  #313 - #314: BIT 2 Q 상한 Enable / Disable
                m_clsSettingInfo.bTrigUpQ = (uData & 0x0004) == 0x0004 ? true : false;

                //  #313 - #314: BIT 3 Q 하한 Enable / Disable
                m_clsSettingInfo.bTrigDnQ = (uData & 0x0008) == 0x0008 ? true : false;

                //  #313 - #314: BIT 4 P Swing Enable / Disable
                m_clsSettingInfo.bTrigSwingP = (uData & 0x0010) == 0x0010 ? true : false;

                //  #313 - #314: BIT 5 Q Swing Enable / Disable
                m_clsSettingInfo.bTrigSwingQ = (uData & 0x0020) == 0x0020 ? true : false;

                //  #313 - #314: BIT 8 - 11 P 변동 기동 주기 설정
                m_clsSettingInfo.bTrigRocP = (Byte)((uData >> 8) & 0x000F);

                //  #313 - #314: BIT 12 - 15 Q 변동 기동 주기 설정
                m_clsSettingInfo.bTrigRocQ = (Byte)((uData >> 12) & 0x000F);

                //  #315 - #316: 아날로그 채널 결선 상태
                m_clsSettingInfo.bInputMode[0] = m_byteReceive[314];
                m_clsSettingInfo.bInputMode[1] = m_byteReceive[315];

                //  #829 - #830: PMU TCP Port
                m_clsSettingInfo.nPmuTcpPort = (UInt16)(m_byteReceive[828] + m_byteReceive[829] * 256);

                //  #831 - #832: Device Address
                m_clsSettingInfo.nDeviceAddr = (UInt16)(m_byteReceive[830] + m_byteReceive[831] * 256);
                
                //  #833 - #834: Ethernet Port
                m_clsSettingInfo.nEthernetPort = (UInt16)(m_byteReceive[832] + m_byteReceive[833] * 256);
                
                for (i = 0; i < 4; i++)
                {
                }

                for (; i < 16; i++ )
                {
                }

                result = true;
            }

            return result;
        }

        private void stopRtu()
        {
            m_nConnectStep = 0;

            //  m_bDaqRun = false;

            if (m_rtuClient != null)
            {
                if (m_rtuClient.IsOpen)
                {
                    m_rtuClient.Close();
                }

                m_rtuClient.Dispose();
                m_rtuClient = null;
            }

            setConnectionStatus(clsConstant.PCS_RTU_SHUTDOWN);

            String strLog = String.Format("{0} > stopRtu ({1})", DateTime.Now.ToString(), m_nIndex);
            Debug.WriteLine(strLog);
            clsLog.addRtuLog(strLog);

            if (!m_bReqStopSocket)
            {
                m_nConnectStep = 1;
            }
        }

        public void Send(byte[] byteSend, int nSend)
        {
            try
            {
                m_bSent = false;

                if (m_rtuClient.IsOpen)
                {
                    m_nSendLength = nSend;
                    Array.Copy(byteSend, m_byteSend, m_nSendLength);
                    m_rtuClient.Write(m_byteSend, 0, m_nSendLength);
                    SendCallback();
                }
            }
            catch (Exception ex)
            {
                String strLog = "";

                strLog = String.Format("{0} > {1} : {2}", DateTime.Now.ToString(), m_strDeviceName, ex.Message);
            }
        }

        void SendCallback()
        {
#if USE_EXCEPTION
            try
            {
#endif
                if (m_rtuClient != null)
                {
                    m_bSent = true;
                }
#if USE_EXCEPTION
            }
            catch (Exception e)
            {
                m_bSent = true;

                makeStatus("SendCallback : " + e.Message, true);
            }
#endif
        }

        private void DisconnectCallback()
        {
            // Complete the disconnect request.
            if (m_rtuClient != null)
            {
                try
                {
                    /*
                    if (m_rtuClient.IsOpen)
                    {
                        m_rtuClient.Close();
                    }
                    */
                    m_rtuClient.Dispose();
                    m_rtuClient = null;
                    m_nDisConnectStep = 3;
                }
                catch (Exception ex)
                {
                    clsLog.addRtuLog("DisconnectCallback: " + ex.Message);
                    m_nDisConnectStep = -1;
                }
                finally
                {
                    
                }
            }

            setConnectionStatus(clsConstant.PCS_RTU_SHUTDOWN);
        }

        public void pauseThread()
        {
            m_bPauseThread = true;
        }

        public void startThread()
        {
            _ClientThread = new System.Threading.Thread(new System.Threading.ThreadStart(_ClientThreadFunc));
            _ClientThread.Start();
        }

        public void stopThread()
        {
            if (m_bThreadRun)
            {
                pauseThread();
                m_bRun = false;
            }

            System.Threading.Thread.Sleep(1);

            if (_ClientThread != null)
            {
                _ClientThread.Join();
            }

            _ClientThread = null;

            String strLog = String.Format("{0} > stopThread ({1})", DateTime.Now.ToString(), m_nIndex);
            
            Debug.WriteLine(strLog);
            clsLog.addRtuLog(strLog);
        }

        public void RtuDisconnect()
        {
            m_nStep = -1;
            m_nMode = clsConstant.MODBUS_MODE_NONE;
            //  m_bDaqRun = false;

#if USE_EXCEPTION
            try
            {
#endif
                pauseThread();

                if (m_rtuClient != null)
                {
                    if (m_rtuClient.IsOpen)
                    {
                        makeStatus("Disconnect", false);

                        DisconnectCallback();
                        m_nDisConnectStep = 2;
                        m_dtDisConnectStep = DateTime.Now;
                    }
                    else
                    {
                        m_nDisConnectStep = 3;
                        stopRtu();
                    }
                }
                else
                {
                    m_nDisConnectStep = 3;
                    stopRtu();
                }
#if USE_EXCEPTION
            }
            catch (Exception ex)
            {
                m_strLogError = "RtuDisconnect : " + ex.Message;
                m_nDisConnectStep = 3;
                stopRtu();
            }
#endif
        }

        public void ConnectionCheck()
        {
            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength =  EssPcsReadPacket(0, 8);
            m_nCommandType = clsConstant.PQMS_COMMAND_CONNECTION_CHECK;
            m_nModBusFunction = clsConstant.MODBUS_FUNC_READ_INPUT;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void GetTime()
        {
            /*
            m_nMode = clsConstant.CDFR_MODE_NONE;

            m_nSendLength = EssPcsReadPacket(6000, 3);
            m_nCommandType = clsConstant.CDFR_DEVICE_TIMESYNC_GET;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_READ_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;
            m_bReceiveTime = false;

            m_nMode = clsConstant.CDFR_MODE_NORMAL;
            */
        }

        public void SetTime()
        {
            DateTime dt = DateTime.Now;
            byte[] buf = new byte[6];

            m_nMode = clsConstant.MODBUS_MODE_NONE;

            buf[0] = (byte)IntToBcd(dt.Year - 2000);
            buf[1] = (byte)IntToBcd(dt.Month);
            buf[2] = (byte)IntToBcd(dt.Day);
            buf[3] = (byte)IntToBcd(dt.Hour);
            buf[4] = (byte)IntToBcd(dt.Minute);
            buf[5] = (byte)IntToBcd(dt.Second);

            //...m_nSendLength = EssPcsWritePacket(2000, 6, buf);
            //m_nCommandType = clsConstant.CDFR_DEVICE_TIME_SET;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;

            m_nTimeSyncTick = Environment.TickCount;
        }

        public void sendMemoryInit()
        {
            DateTime dt = DateTime.Now;
            byte[] buf = { 0xFF, 0x01 };

            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsWritePacket(1100, 2, buf);
            m_nCommandType = clsConstant.CDFR_DEVICE_MEMORY_INIT;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendReboot()
        {
            DateTime dt = DateTime.Now;
            byte[] buf = { 0xFF, 0x02 };

            m_nMode = clsConstant.MODBUS_MODE_NONE;

            m_nSendLength = //...EssPcsWritePacket(1100, 2, buf);
            m_nCommandType = clsConstant.CDFR_DEVICE_REBOOT;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendManualTrigger()
        {
            DateTime dt = DateTime.Now;
            byte[] buf = { 0xFF, 0x03 };

            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsWritePacket(1100, 2, buf);
            m_nCommandType = clsConstant.CDFR_DEVICE_MANUAL_TRIGGER;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendPrevTrigger()
        {
            DateTime dt = DateTime.Now;
            byte[] buf = { 0xFF, 0x04 };

            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsWritePacket(1100, 2, buf);
            m_nCommandType = clsConstant.CDFR_DEVICE_PREV_TRIGGER;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendCalStart()
        {
            DateTime dt = DateTime.Now;
            byte[] buf = { 0xFF, 0x05 };

            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsWritePacket(1100, 2, buf);
            m_nCommandType = clsConstant.CDFR_DEVICE_CAL_START;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendCalStop()
        {
            DateTime dt = DateTime.Now;
            byte[] buf = { 0xFF, 0x06 };

            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsWritePacket(1100, 2, buf);
            m_nCommandType = clsConstant.CDFR_DEVICE_CAL_STOP;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendRelayControlOn()
        {
            DateTime dt = DateTime.Now;
            byte[] buf = { 0xFF, 0x07 };

            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsWritePacket(1100, 2, buf);
            m_nCommandType = clsConstant.CDFR_DEVICE_RELAY_CONTROL_ON;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendRelayControlOff()
        {
            DateTime dt = DateTime.Now;
            byte[] buf = { 0xFF, 0x08 };

            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsWritePacket(1100, 2, buf);
            m_nCommandType = clsConstant.CDFR_DEVICE_RELAY_CONTROL_OFF;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendTriggerLockOn()
        {
            DateTime dt = DateTime.Now;
            byte[] buf = { 0xFF, 0x09 };

            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsWritePacket(1100, 2, buf);
            m_nCommandType = clsConstant.CDFR_DEVICE_TRIGGER_LOCK_ON;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendTriggerLockOff()
        {
            DateTime dt = DateTime.Now;
            byte[] buf = { 0xFF, 0x0A };

            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsWritePacket(1100, 2, buf);
            m_nCommandType = clsConstant.CDFR_DEVICE_TRIGGER_LOCK_OFF;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendRelay(byte bRelayBit)
        {
            DateTime dt = DateTime.Now;
            UInt16 uBuf = 0xFF80;

            m_nMode = clsConstant.MODBUS_MODE_NONE;
            uBuf += bRelayBit;
            m_nSendLength = m_clsModbus.modbusWriteSingle((int)m_clsSettingInfo.nDeviceAddr, 1100, uBuf);
            m_nCommandType = clsConstant.CDFR_DEVICE_RELAY;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendEssPcsSetParameter(clsEssPcsMsg msg)
        {
            DateTime dt = DateTime.Now;

            m_nMode = clsConstant.MODBUS_MODE_NONE;
            m_nSendLength = m_clsModbus.modbusWriteMultiple((int)m_clsSettingInfo.nDeviceAddr, 2, msg.msgBuf, 104);
            Array.Copy(m_clsModbus.g_bSendBuffer, m_byteSend, m_nSendLength);
            m_nCommandType = clsConstant.ESSPCS_DEVICE_INV_PARAMETER;
            m_nModBusFunction = clsConstant.ESSPCS_COMMAND_MULTIPLE_WRITE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
            clsLog.addSystemLog("setInverterParameter");
        }

        public void sendEssPcsRun(clsEssPcsMsg msg)
        {
            DateTime dt = DateTime.Now;

            m_nMode = clsConstant.MODBUS_MODE_NONE;
            m_nSendLength = m_clsModbus.modbusWriteSingle((int)m_clsSettingInfo.nDeviceAddr, 0, msg.msgIndex);
            Array.Copy(m_clsModbus.g_bSendBuffer, m_byteSend, m_nSendLength);
            m_nCommandType = clsConstant.ESSPCS_DEVICE_INV_RUN;
            m_nModBusFunction = clsConstant.ESSPCS_COMMAND_SINGLE_WRITE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
            clsLog.addSystemLog("sendEssPcsRun");
        }

        public void sendEssPcsStop(clsEssPcsMsg msg)
        {
            DateTime dt = DateTime.Now;

            m_nMode = clsConstant.MODBUS_MODE_NONE;
            m_nSendLength = m_clsModbus.modbusWriteSingle((int)m_clsSettingInfo.nDeviceAddr, 0, msg.msgIndex);
            Array.Copy(m_clsModbus.g_bSendBuffer, m_byteSend, m_nSendLength);
            m_nCommandType = clsConstant.ESSPCS_DEVICE_INV_STOP;
            m_nModBusFunction = clsConstant.ESSPCS_COMMAND_SINGLE_WRITE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
            clsLog.addSystemLog("sendEssPcsStop");
        }

        public void sendEssPcsOutput(clsEssPcsMsg msg)
        {
            DateTime dt = DateTime.Now;

            m_nMode = clsConstant.MODBUS_MODE_NONE;
            m_nSendLength = m_clsModbus.modbusWriteMultiple((int)m_clsSettingInfo.nDeviceAddr, 0, msg.msgBuf, 4);
            Array.Copy(m_clsModbus.g_bSendBuffer, m_byteSend, m_nSendLength);
            m_nCommandType = clsConstant.ESSPCS_DEVICE_OUTPUT_SET;
            m_nModBusFunction = clsConstant.ESSPCS_COMMAND_MULTIPLE_WRITE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
            clsLog.addSystemLog("sendEssPcsOutput");
        }

        public void sendEssPcsFaultClear(clsEssPcsMsg msg)
        {
            DateTime dt = DateTime.Now;

            m_nMode = clsConstant.MODBUS_MODE_NONE;
            m_nSendLength = m_clsModbus.modbusWriteSingle((int)m_clsSettingInfo.nDeviceAddr, 0, msg.msgIndex);
            Array.Copy(m_clsModbus.g_bSendBuffer, m_byteSend, m_nSendLength);
            m_nCommandType = clsConstant.ESSPCS_DEVICE_FAULT_CLEAR;
            m_nModBusFunction = clsConstant.ESSPCS_COMMAND_SINGLE_WRITE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
            clsLog.addSystemLog("sendEssPcsFaultClear");
        }

        public void sendEssPcsWarnClear(clsEssPcsMsg msg)
        {
            DateTime dt = DateTime.Now;

            m_nMode = clsConstant.MODBUS_MODE_NONE;
            m_nSendLength = m_clsModbus.modbusWriteSingle((int)m_clsSettingInfo.nDeviceAddr, 0, msg.msgIndex);
            Array.Copy(m_clsModbus.g_bSendBuffer, m_byteSend, m_nSendLength);
            m_nCommandType = clsConstant.ESSPCS_DEVICE_WARN_CLEAR;
            m_nModBusFunction = clsConstant.ESSPCS_COMMAND_SINGLE_WRITE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
            clsLog.addSystemLog("sendEssPcsWarnClear");
        }

        public void receiveTrendData()
        {
            m_nMode = clsConstant.MODBUS_MODE_NONE;

            m_nSendLength = m_clsModbus.modbusReadPacket((int)m_clsSettingInfo.nDeviceAddr, m_AddrInfoList[m_nModbusMapIndex].m_nFuctionCode, m_AddrInfoList[m_nModbusMapIndex].m_nStartAddr, m_AddrInfoList[m_nModbusMapIndex].m_nLength);
            m_nExpectedLength = (m_AddrInfoList[m_nModbusMapIndex].m_nLength * 2) + 5;
            m_nExpectedCount = 0;
            m_nCommandType = clsConstant.PCS_DEVICE_TREND_DATA_RECEIVE;
            m_nModBusFunction = m_AddrInfoList[m_nModbusMapIndex].m_nFuctionCode;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            //...m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        /*
         * Request Setup Data from CDFR Terminal
         */
        public void receiveSetupData()
        {
            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = m_clsModbus.modbusReadPacket(0x03, m_clsSettingInfo.nDeviceAddr, clsConstant.PCS_SETUP_DATA_START, clsConstant.PCS_SETUP_DATA_NUM);
            m_nCommandType = clsConstant.CDFR_DEVICE_RECEIVE_SETUP_DATA;
            m_nModBusFunction = clsConstant.MODBUS_FUNC_READ_INPUT;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;
            m_bReceiveSetupData = false;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendSetupData(clsEssPcsMsg msg)
        {
            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsWritePacket(clsConstant.CDFR_SETUP_DATA_START, clsConstant.CDFR_SETUP_DATA_NUM, msg.msgBuf);
            //...m_nCommandType = clsConstant.CDFR_DEVICE_SEND_SETUP_DATA;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;
            m_bSendSetupData = false;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void sendFaultLoc(clsEssPcsMsg msg)
        {
            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsWritePacket(clsConstant.CDFR_FAULT_LOC_START, clsConstant.CDFR_FAULT_LOC_NUM, msg.msgBuf);
            m_nCommandType = clsConstant.CDFR_DEVICE_FAULT_LOC;
            m_nModBusFunction = clsConstant.CDFR_COMMAND_WRITE_AFE;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;
            m_bSendSetupData = false;

            m_nExpectedLength = 0;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void receiveInsWave()
        {
            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsReadInsWavePacket((UInt32)m_nInsWaveIndex);
            m_nCommandType = clsConstant.CDFR_DEVICE_INS_WAVE;
            m_nModBusFunction = clsConstant.ESSPCS_COMMAND_SINGLE_COIL;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = clsConstant.CDFR_EXPECTED_WAVE_LENGTH;
            
            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void cancelWave()
        {
            if (m_bWaitInsFile || m_bWaitPhaFile)
            {
                m_bWaitInsFile = false;
                m_bWaitPhaFile = false;
                clearRequestMsg();
                receiveTrendData();
            }
        }

        public void receivePhaWave()
        {
            m_nMode = clsConstant.MODBUS_MODE_NONE;

            //...m_nSendLength = EssPcsReadPhaWavePacket((UInt32)m_nPhaWaveIndex);
            m_nCommandType = clsConstant.CDFR_DEVICE_PHA_WAVE;
            m_nModBusFunction = clsConstant.ESSPCS_COMMAND_SINGLE_COIL;
            m_bModBusException = false;
            m_nStep = 0;
            m_nErrorCount = 0;

            m_nExpectedLength = clsConstant.CDFR_EXPECTED_WAVE_LENGTH;

            m_nMode = clsConstant.MODBUS_MODE_NORMAL;
        }

        public void setPause(bool bPause)
        {
            m_bDaqRun = bPause;
            m_nTimeSyncTick = Environment.TickCount;
            m_nManualTriggerTick = Environment.TickCount;
        }
    }

    public class clsStatus
    {
        public bool m_bMsgLog;
        public String m_strMsg;
        public DateTime m_dtMsg;

        public clsStatus(string strMsg, bool bLog)
        {
            m_dtMsg = DateTime.Now;
            m_strMsg = strMsg;
            m_bMsgLog = bLog;
        }

        public clsStatus(string strMsg, bool bLog, DateTime dt)
        {
            m_dtMsg = dt;
            m_strMsg = strMsg;
            m_bMsgLog = bLog;
        }
    }
}
