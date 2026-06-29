using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Numerics;
using EssPcsService;

namespace EssPcsServer
{
    public class clsGateway
    {
        public const Int32 HOST_CMD_DEVICE_RACK_READ = 0x000003E8;
        public const Int32 HOST_CMD_DEVICE_GAIN_READ = 0x0000044C;
        public const Int32 HOST_CMD_GENERAL_SETUP_READ = 0x000004B0;
        public const Int32 HOST_CMD_ANALOG_SETUP_READ = 0x00000514;
        public const Int32 HOST_CMD_DIGITAL_SETUP_READ = 0x00000578;
        public const Int32 HOST_CMD_BUSINFO_SETUP_READ = 0x000005DC;
        public const Int32 HOST_CMD_FEEDER_SETUP_READ = 0x00000640;
        public const Int32 HOST_CMD_PMUINFO_SETUP_READ = 0x000006A4;
        public const Int32 HOST_CMD_GENERAL_SETUP_WRITE = 0x00000898;
        public const Int32 HOST_CMD_ANALOG_SETUP_WRITE = 0x000008FC;
        public const Int32 HOST_CMD_DIGITAL_SETUP_WRITE = 0x00000960;
        public const Int32 HOST_CMD_BUSINFO_SETUP_WRITE = 0x000009C4;
        public const Int32 HOST_CMD_FEEDER_SETUP_WRITE = 0x00000A28;
        public const Int32 HOST_CMD_PMUINFO_SETUP_WRITE = 0x00000A8C;
        public const Int32 COMMAND_RMS_DATA_STATUS_DATA_READ = 0x000005DC;

        public Byte[] g_bSendBuffer = new Byte[MAX_SEND_PACKET];

        private readonly static Int32 MAX_SEND_PACKET = 4096;
        private UInt32 m_nHostCommandStr = 0;
        private UInt32 m_nReceiveLength = 0;
        private UInt32 m_nChannelKindInfo = 0;
        private UInt32 m_nChannelNumber = 0;
        private UInt32 m_nCheckSum = 0;
        private Int32 m_nIndex = 0;
        private Byte[] m_bIpAddress = { 127, 0, 0, 1 };
        private Byte[] m_bSubnetMask = { 255, 255, 255, 0 };
        private Byte[] m_bGatewayAddress = { 0, 0, 0, 0 };
        private Int32[] m_nPhasor = new Int32[16];
        private clsEssPcsSettingInfo m_clsSettingInfo;
        private clsSqlServer m_clsSqlServer;

        public clsGateway(int nIndex, ref clsSqlServer sqlServer, Byte[] bIpAddress, Byte[] bSubnetMask, Byte[] bGatewayAddress)
        {
            m_nIndex = nIndex;
            m_clsSqlServer = sqlServer;

            Array.Copy(bIpAddress, m_bIpAddress, 4);
            Array.Copy(bSubnetMask, m_bSubnetMask, 4);
            Array.Copy(bGatewayAddress, m_bGatewayAddress, 4);
        }

        private int makeHeader()
        {
            int tx = 0;

            g_bSendBuffer[tx++] = 0;
            g_bSendBuffer[tx++] = 0;
            g_bSendBuffer[tx++] = 0;
            g_bSendBuffer[tx++] = 0;
            g_bSendBuffer[tx++] = 0;
            g_bSendBuffer[tx++] = 1;
 
            return tx;
        }

        public int gatewayReadPacket(int nFunction, int nAddress, int nReqLength)
        {
            int len, crc16;

            len = 0;

            /*  GATEWAY ETHERNET HEADER */
            g_bSendBuffer[len++] = 0;
            g_bSendBuffer[len++] = 0;
            g_bSendBuffer[len++] = 0;
            g_bSendBuffer[len++] = 0;
            g_bSendBuffer[len++] = 0;
            g_bSendBuffer[len++] = 1;

            g_bSendBuffer[len++] = 0x01;
            g_bSendBuffer[len++] = (byte)nFunction;
            g_bSendBuffer[len++] = (byte)((nAddress >> 8) & 0xFF);
            g_bSendBuffer[len++] = (byte)((nAddress >> 0) & 0xFF);
            g_bSendBuffer[len++] = (byte)((nReqLength >> 8) & 0xFF);
            g_bSendBuffer[len++] = (byte)((nReqLength >> 0) & 0xFF);

            crc16 = clsCrc16.crc16(g_bSendBuffer, len);

            g_bSendBuffer[len++] = (byte)((crc16 >> 0) & 0xFF);
            g_bSendBuffer[len++] = (byte)((crc16 >> 8) & 0xFF);

            return len;
        }

        public int parsePacket(ref Byte[] bBuf, int nLen, ref IEssPcsService serviceProxy)
        {
            UInt32 nChk = 0;
            int i;

            if (nLen > 8)
            {

                m_nHostCommandStr = BitConverter.ToUInt32(bBuf, 0);

                if (m_nHostCommandStr == 0x00005248)
                {
                    m_nReceiveLength = BitConverter.ToUInt32(bBuf, 4);
                    m_nChannelKindInfo = BitConverter.ToUInt32(bBuf, 8);
                    m_nChannelNumber = BitConverter.ToUInt32(bBuf, 12);
                    m_nCheckSum = BitConverter.ToUInt32(bBuf, 20);
#if DEBUG_GATEWAY
                    Debug.WriteLine("LEN = {0}> RxLength[{1}] ChKind[{2}] ChNum[{3}] COUNT[{4}]",
                        nLen, m_nReceiveLength, m_nChannelKindInfo, m_nChannelNumber, m_nDevceSetupCount);
#endif
                    nChk = 0;
                    nChk += m_nHostCommandStr;
                    nChk += m_nReceiveLength;
                    nChk += m_nChannelKindInfo;
                    nChk += m_nChannelNumber;

                    if (nChk == m_nCheckSum)
                    {
                        return processReadData(ref serviceProxy);
                    }
                }
                else if (m_nHostCommandStr == 0x00005151)
                {
                    m_nReceiveLength = BitConverter.ToUInt32(bBuf, 4);
                    m_nChannelKindInfo = BitConverter.ToUInt32(bBuf, 8);
                    m_nChannelNumber = BitConverter.ToUInt32(bBuf, 12);
                    m_nCheckSum = BitConverter.ToUInt32(bBuf, 20);

#if DEBUG_GATEWAY
                    Debug.WriteLine("LEN = {0}> RxLength[{1}] ChKind[{2}] ChNum[{3}] COUNT[{4}]",
                        nLen, m_nReceiveLength, m_nChannelKindInfo, m_nChannelNumber, m_nDevceSetupCount);
#endif
                    nChk = 0;
                    nChk += m_nHostCommandStr;
                    nChk += m_nReceiveLength;
                    nChk += m_nChannelKindInfo;
                    nChk += m_nChannelNumber;

                    if (nChk == m_nCheckSum)
                    {
                        return processRtData(ref serviceProxy);
                    }
                }
                else if (m_nHostCommandStr == 0x00005748)
                {
                    m_nReceiveLength = BitConverter.ToUInt32(bBuf, 4);
                    m_nChannelKindInfo = BitConverter.ToUInt32(bBuf, 8);
                    m_nChannelNumber = BitConverter.ToUInt32(bBuf, 12);
                    m_nCheckSum = BitConverter.ToUInt32(bBuf, (int)(m_nReceiveLength - 4));

#if DEBUG_GATEWAY
                    Debug.WriteLine("LEN = {0}> RxLength[{1}] ChKind[{2}] ChNum[{3}] COUNT[{4}]",
                        nLen, m_nReceiveLength, m_nChannelKindInfo, m_nChannelNumber, m_nDevceSetupCount);
#endif
                    Array.Copy(bBuf, g_bSendBuffer, m_nReceiveLength - 4);
                    nChk = makeCheckSum((int)m_nReceiveLength - 4);

                    if (nChk == m_nCheckSum)
                    {
                        return processWriteData(ref bBuf, ref serviceProxy);
                    }
                }
                else if (m_nHostCommandStr == 0x00005448)
                {
                    int nTx = 0;

                    m_nReceiveLength = BitConverter.ToUInt32(bBuf, 4);
                    m_nChannelKindInfo = BitConverter.ToUInt32(bBuf, 8);
                    m_nChannelNumber = BitConverter.ToUInt32(bBuf, 12);
                    m_nCheckSum = BitConverter.ToUInt32(bBuf, (int)(m_nReceiveLength - 4));

#if DEBUG_GATEWAY
                    Debug.WriteLine("LEN = {0}> RxLength[{1}] ChKind[{2}] ChNum[{3}] COUNT[{4}]",
                        nLen, m_nReceiveLength, m_nChannelKindInfo, m_nChannelNumber, m_nDevceSetupCount);
#endif
                    Array.Copy(bBuf, g_bSendBuffer, m_nReceiveLength - 4);
                    nChk = makeCheckSum((int)m_nReceiveLength - 4);

                    if (nChk == m_nCheckSum)
                    {
                        if (serviceProxy.GetServerStatus())
                        {
                            clsEssPcsMsg msg = new clsEssPcsMsg();
                            int nIndex = m_nIndex;

                            msg.msgType = clsConstant.ESSPCS_CMD_MANUAL_TRIGGER;
                            msg.msgIndex = 0;
                            msg.msgDateTime = DateTime.Now;
                            msg.msgSession = "";
                            msg.msgResult = 0;
                            msg.msgDevice = serviceProxy.GetSetting(nIndex).nDeviceNo;
                            msg.msgDeviceName = serviceProxy.GetSetting(nIndex).strDevice;

                            serviceProxy.SetCmd(msg);
                        }
                    }

                    nTx = makeHeaderInfo(0);
                    Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
                    nTx += 4;

                    return nTx;
                }
                else
                {
#if DEBUG_GATEWAY
                    Debug.WriteLine("LEN = {0}> HOSTCMD[{1}]",
                        nLen, m_nHostCommandStr);
#endif
                }
            }

            return 0;
        }

        private int processReadData(ref IEssPcsService serviceProxy)
        {
            int nTx = 0;

            if (m_nReceiveLength == 24)
            {
                switch (m_nChannelKindInfo)
                {
                    case HOST_CMD_DEVICE_RACK_READ:
                        nTx = makeRackInfo(ref serviceProxy);
                        Debug.WriteLine("HOST_CMD_DEVICE_RACK_READ\r\n");
                        break;

                    case HOST_CMD_DEVICE_GAIN_READ:
                        nTx = makeGainInfo(ref serviceProxy);
                        Debug.WriteLine("HOST_CMD_DEVICE_GAIN_READ\r\n");
                        break;

                    case HOST_CMD_GENERAL_SETUP_READ:
                        nTx = makeGeneralInfo(ref serviceProxy);
                        Debug.WriteLine("HOST_CMD_GENERAL_SETUP_READ\r\n");
                        break;

                    case HOST_CMD_ANALOG_SETUP_READ:
                        nTx = makeAnalogInfo(ref serviceProxy);
                        Debug.WriteLine("HOST_CMD_ANALOG_SETUP_READ\r\n");
                        break;

                    case HOST_CMD_DIGITAL_SETUP_READ:
                        nTx = makeDigitalInfo(ref serviceProxy);
                        Debug.WriteLine("HOST_CMD_DIGITAL_SETUP_READ\r\n");
                        break;

                    case HOST_CMD_BUSINFO_SETUP_READ:
                        nTx = makeBusInfo(ref serviceProxy);
                        Debug.WriteLine("HOST_CMD_BUSINFO_SETUP_READ\r\n");
                        break;
        
                    case HOST_CMD_FEEDER_SETUP_READ:
                        nTx = makeFeederInfo(ref serviceProxy);
                        Debug.WriteLine("HOST_CMD_FEEDER_SETUP_READ\r\n");
                        break;
        
                    case HOST_CMD_PMUINFO_SETUP_READ:
                        nTx = makePmuInfo(ref serviceProxy);
                        Debug.WriteLine("HOST_CMD_PMUINFO_SETUP_READ\r\n");
                        break;
                }
            }

            return nTx;
        }

        private Byte getBcd(int nVal)
        {
            int nDiv = nVal / 10;
            int nMod = nVal % 10;
            Byte bRet = 0;

            bRet = (Byte)((nDiv * 0x10) + nMod);

            return bRet;
        }

        private int processRtData(ref IEssPcsService serviceProxy)
        {
            int nStartIndex = 0;
            int nTx = 0;
            int i;

            if ((m_nReceiveLength == 24) && (m_nChannelKindInfo == 1500))
            {
                clsEssPcsTrendInfo trendInfo = serviceProxy.GetTrendInfo(m_nIndex);

                nTx = makeHeaderInfo(164 + 4 + 4);
                
                for (i = 0; i < (164 + 4 + 4); i++)
                {
                    g_bSendBuffer[nTx + i] = 0;
                }


                //  Padding Data (??QQ)
                g_bSendBuffer[nTx + 0] = (Byte)'?';
                g_bSendBuffer[nTx + 1] = (Byte)'?';
                g_bSendBuffer[nTx + 2] = (Byte)'Q';
                g_bSendBuffer[nTx + 3] = (Byte)'Q';
                nTx += 4;

                nStartIndex = nTx;

                //  실시간 선택된 장비의 개수
                Array.Copy(BitConverter.GetBytes((int)1), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  Data Read Sec
                //Array.Copy(BitConverter.GetBytes((int)0), 0, g_bSendBuffer, nTx, 4);
                //nTx += 4;

                //  Device Number (등록되어 있는 첫번째 장비)
                //Array.Copy(BitConverter.GetBytes((int)0), 0, g_bSendBuffer, nTx, 4);
                //nTx += 4;

                //  Total Device Count
                //Array.Copy(BitConverter.GetBytes((int)0), 0, g_bSendBuffer, nTx, 4);
                //nTx += 4;

                //  Real Date Time Info
#if DEBUG
                trendInfo.dtTime = DateTime.Now;
#endif
                //  시
                g_bSendBuffer[nTx++] = getBcd(trendInfo.dtTime.Hour);
                //  일
                g_bSendBuffer[nTx++] = getBcd(trendInfo.dtTime.Day);
                //  월
                g_bSendBuffer[nTx++] = getBcd(trendInfo.dtTime.Month);
                //  년
                g_bSendBuffer[nTx++] = getBcd(16/*trendInfo.dtTime.Year - 2000*/);
                //  ms
                g_bSendBuffer[nTx++] = 0x00;
                g_bSendBuffer[nTx++] = 0x00;
                //  초
                g_bSendBuffer[nTx++] = getBcd(trendInfo.dtTime.Second);
                //  분
                g_bSendBuffer[nTx++] = getBcd(trendInfo.dtTime.Minute);

                //  Digital Data
                Int32 nDigital = 0;
                /*
                if (trendInfo.bDI[0]) nDigital += 0x0001;
                if (trendInfo.bDI[1]) nDigital += 0x0002;
                if (trendInfo.bDI[2]) nDigital += 0x0004;
                if (trendInfo.bDI[3]) nDigital += 0x0008;
                if (trendInfo.bDI[4]) nDigital += 0x0010;
                if (trendInfo.bDI[5]) nDigital += 0x0020;
                if (trendInfo.bDI[6]) nDigital += 0x0040;
                if (trendInfo.bDI[7]) nDigital += 0x0080;
                if (trendInfo.bDI[8]) nDigital += 0x0100;
                if (trendInfo.bDI[9]) nDigital += 0x0200;
                if (trendInfo.bDI[10]) nDigital += 0x0400;
                if (trendInfo.bDI[11]) nDigital += 0x0800;
                if (trendInfo.bDI[12]) nDigital += 0x1000;
                if (trendInfo.bDI[13]) nDigital += 0x2000;
                if (trendInfo.bDI[14]) nDigital += 0x4000;
                if (trendInfo.bDI[15]) nDigital += 0x8000;
                */
#if DEBUG
                nDigital = 0x00000f0f;
#endif
                Array.Copy(BitConverter.GetBytes(nDigital), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  Analog Data
#if DEBUG
//                for (i = 0; i < 16; i++)
//                {
//                    trendInfo.fValue[i] = 10.1f + i; 
//                    trendInfo.fPhasor[i] = 0.1f + i * 0.1f;
//                }
#endif
                /*
                for (i = 0; i < 16; i++)
                {
                    m_nPhasor[i] = (Int32)((trendInfo.fPhasor[i] * 180.0f * 100.0f) / Math.PI);
                    m_nPhasor[i] %= 36000;

                    if (m_nPhasor[i] > 18000)
                    {
                        m_nPhasor[i] -= 36000;
                    }

                    Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[i] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                    Array.Copy(BitConverter.GetBytes(m_nPhasor[i]), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                }
                */
                /*
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[1] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes(m_nPhasor[0]), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[2] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes(m_nPhasor[0]), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[3] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes(m_nPhasor[0]), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[4] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes(m_nPhasor[0]), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[5] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes(m_nPhasor[0]), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[6] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes(m_nPhasor[0]), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[7] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes(m_nPhasor[0]), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[8] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fPhasor[8] * 100)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[9] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fPhasor[9] * 100)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[10] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fPhasor[10] * 100)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[11] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fPhasor[11] * 100)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[12] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fPhasor[12] * 100)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[13] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fPhasor[13] * 100)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[14] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fPhasor[14] * 100)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fValue[15] * 10)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                Array.Copy(BitConverter.GetBytes((int)(trendInfo.fPhasor[15] * 100)), 0, g_bSendBuffer, nTx, 4); nTx += 4;
                */
                //  Feeder Data
                Int32 nPosVmag = 0;
                Int32 nPosVang = 0;

                //  Calculation Positive Voltage and Angle
                {
                    Complex UnbalanceMap01 = new Complex(Math.Cos(2 * Math.PI / 3.0), Math.Sin(2 * Math.PI / 3.0));
                    Complex UnbalanceMap02 = new Complex(Math.Cos(-2 * Math.PI / 3.0), Math.Sin(-2 * Math.PI / 3.0));
                    Complex cPosValue = new Complex(0, 0);
                    Complex[] cValue = new Complex[3];
                    double[] fAng = { 0, 0, 0 };
                    /*
                    fAng[0] = (trendInfo.fPhasor[0] * 2 * Math.PI) / 360.0;
                    fAng[1] = (trendInfo.fPhasor[1] * 2 * Math.PI) / 360.0;
                    fAng[2] = (trendInfo.fPhasor[2] * 2 * Math.PI) / 360.0;

                    cValue[0] = new Complex(trendInfo.fValue[0] * Math.Cos(fAng[0]), trendInfo.fValue[0] * Math.Sin(fAng[0]));
                    cValue[1] = new Complex(trendInfo.fValue[1] * Math.Cos(fAng[1]), trendInfo.fValue[1] * Math.Sin(fAng[1]));
                    cValue[2] = new Complex(trendInfo.fValue[2] * Math.Cos(fAng[2]), trendInfo.fValue[2] * Math.Sin(fAng[2]));
                    */
                    cPosValue = (cValue[0] + cValue[1] * UnbalanceMap01 + cValue[2] * UnbalanceMap02) / (double)3.0;

                    nPosVmag = (Int32)(cPosValue.Magnitude * 10);
                    nPosVang = (Int32)((double)((cPosValue.Phase * 360.0 * 100.0) / (Math.PI * 2.0)));
                    nPosVang %= 36000;
                    if (nPosVang > 18000)
                    {
                        nPosVang -= 36000;
                    }
                }

#if DEBUG
                //trendInfo.fP = 100;
                //trendInfo.fQ = 5;
                //trendInfo.fFreq = 60;
                //  nPosVmag = (Int32)(10.0f * 10);
                //  nPosVang = (Int32)(10.0f * 100);
#endif
                //  PValue
                //Array.Copy(BitConverter.GetBytes((int)(trendInfo.fP * 10)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
                //  QValue
                //Array.Copy(BitConverter.GetBytes((int)(trendInfo.fQ * 10)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
                //  VValue (정상분 크기)
                Array.Copy(BitConverter.GetBytes(nPosVmag), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
                //  FValue (정상분 위상)
                Array.Copy(BitConverter.GetBytes(nPosVang), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  Bus Data
                //Array.Copy(BitConverter.GetBytes((int)(trendInfo.fFreq * 1000)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx, nStartIndex) + 0x5151), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                return nTx;
            }

            return nTx;
        }

        private int processWriteData(ref Byte[] bBuf, ref IEssPcsService serviceProxy)
        {
            int nTx = 0;

                switch (m_nChannelKindInfo)
                {
                    case HOST_CMD_GENERAL_SETUP_WRITE:
                        nTx = parseGeneralInfo(ref bBuf, ref serviceProxy);
                        break;

                    case HOST_CMD_ANALOG_SETUP_WRITE:
                        nTx = parseAnalogInfo(ref bBuf, ref serviceProxy);
                        break;

                    case HOST_CMD_DIGITAL_SETUP_WRITE:
                        nTx = parseDigitalInfo(ref bBuf, ref serviceProxy);
                        break;

                    case HOST_CMD_BUSINFO_SETUP_WRITE:
                        nTx = parseBusInfo(ref bBuf, ref serviceProxy);
                        break;

                    case HOST_CMD_FEEDER_SETUP_WRITE:
                        nTx = parseFeederInfo(ref bBuf, ref serviceProxy);
                        break;

                    case HOST_CMD_PMUINFO_SETUP_WRITE:
                        nTx = parsePmuInfo(ref bBuf, ref serviceProxy);
                        break;
                }

            return nTx;
        }

        private UInt32 makeCheckSum(int nSize, int nStartIndex = 0)
        {
            UInt32 nChk = 0;
            int i;

            for (i = nStartIndex; i < nSize; i += 4)
            {
                nChk += BitConverter.ToUInt32(g_bSendBuffer, i);
            }

            return nChk;
        }

        private int makeHeaderInfo(int nLen)
        {
            int nTx = 0;

            Array.Copy(BitConverter.GetBytes(m_nHostCommandStr), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            Array.Copy(BitConverter.GetBytes(nLen + 24), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            Array.Copy(BitConverter.GetBytes(m_nChannelKindInfo), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            Array.Copy(BitConverter.GetBytes(m_nChannelNumber), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            return nTx;
        }

        private int makeRackInfo(ref IEssPcsService serviceProxy)
        {
            UInt32 nAI = 0x000000AD;
            UInt32 nDI = 0x000000D1;
            int nTx = 0;
            int i;

            nTx = makeHeaderInfo(32);

            for (i = 0; i < 4; i++)
            {
                Array.Copy(BitConverter.GetBytes(nAI), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            for (i = 0; i < 4; i++)
            {
                Array.Copy(BitConverter.GetBytes(nDI), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            return nTx;
        }

        private int makeGainInfo(ref IEssPcsService serviceProxy)
        {
            UInt32 nMMIVersion = 0x00000001;
            UInt32 nIOVersion = 0x00000001;
            int nTx = 0;
            int i;

            nTx = makeHeaderInfo(3592);

            Array.Copy(BitConverter.GetBytes(nMMIVersion), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            Array.Copy(BitConverter.GetBytes(nIOVersion), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  Ref Gain
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  CalSlop[0]
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  CalSlop[1]
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  CalSlop[2]
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  CalSlop[3]
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  CalStart[0]
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  CalStart[1]
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  CalStart[2]
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  CalStart[3]
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  DcSlop
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  DcOffset
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  Binary1
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  Binary2
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            //  Binary3
            for (i = 0; i < 64; i++)
            {
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }

            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);

            nTx += 4;

            return nTx;
        }

        private int makeGeneralInfo(ref IEssPcsService serviceProxy)
        {
            int nTx = 0;
            int i;

            nTx = makeHeaderInfo(192);

            clsEssPcsSettingInfo settingInfo = serviceProxy.GetSettingByIndex(m_nIndex);
            Byte[] bBuf = Encoding.Default.GetBytes(settingInfo.strDevice);

            //  Site Name
            for (i = 0; i < bBuf.Length; i++)
            {
                g_bSendBuffer[nTx + i] = bBuf[i];
            }

            for (; i < 24; i++ )
            {
                g_bSendBuffer[nTx + i] = 0;
            }

            nTx += 24;

            //  Site Number
            Array.Copy(BitConverter.GetBytes(m_nIndex + 1), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
  
            //  PMU IP, SUBNET MASK, GATEWAY, MAC HIGH, MAC LOW
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  장치 전력 주파수 [0:50, 1:60]
            if (settingInfo.nFreq == 1)
            {
                Array.Copy(BitConverter.GetBytes(50), 0, g_bSendBuffer, nTx, 4);
            }
            else
            {
                Array.Copy(BitConverter.GetBytes(60), 0, g_bSendBuffer, nTx, 4);
            }
            nTx += 4;

            //  PSDR-XU VERSION
            Array.Copy(BitConverter.GetBytes(1), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  환경 설정 인쇄 페이지 수
            Array.Copy(BitConverter.GetBytes(1), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  장치 시간 설정
            //  GPS
            if (settingInfo.nTimeSync == 1)
            {
                Array.Copy(BitConverter.GetBytes(1), 0, g_bSendBuffer, nTx, 4);
            }
            //  IRIG B120
            else if ((settingInfo.nTimeSync == 2) || (settingInfo.nTimeSync == 3))
            {
                Array.Copy(BitConverter.GetBytes(2), 0, g_bSendBuffer, nTx, 4);
            }
            //  RTC
            else
            {
                Array.Copy(BitConverter.GetBytes(4), 0, g_bSendBuffer, nTx, 4);
            }
            nTx += 4;

            //  1PPS CHECK ENABLE
            Array.Copy(BitConverter.GetBytes((int)(settingInfo.b1PPS ? 1 : 0)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  BAUDRATE
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            
            //  DATA BIT
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            
            //  PARITY BIT
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  장치 IP 주소
            Array.Copy(m_bIpAddress, 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  장치 NETAMSK
            Array.Copy(m_bSubnetMask, 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            
            //  장치 GATEWAY
            Array.Copy(m_bGatewayAddress, 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  BUS 개수
            Array.Copy(BitConverter.GetBytes(1), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            
            //  FEEDER 개수
            Array.Copy(BitConverter.GetBytes(1), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  RMS TRIGGER ENABLE
            Array.Copy(BitConverter.GetBytes(settingInfo.bInsSave ? 1 : 0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  MANUAL TRIGGER ENABLE
            Array.Copy(BitConverter.GetBytes(settingInfo.bTriggerManual ? 1 : 0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  SYNC TRIGGER ENABLE
            Array.Copy(BitConverter.GetBytes(settingInfo.bTriggerExt ? 1 : 0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  REMOTE TRIGGER ENABLE
            Array.Copy(BitConverter.GetBytes(settingInfo.bTriggerHost ? 1 : 0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  AUTO TRIGGER ENABLE
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  AUTO TRIGGER 주기
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  PRE TRG TIME
            Array.Copy(BitConverter.GetBytes((int)settingInfo.nInsPre), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  POST TRG TIME
            Array.Copy(BitConverter.GetBytes((int)settingInfo.nInsPost), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  장비 타입
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  PRE PHA TIME
            Array.Copy(BitConverter.GetBytes((int)settingInfo.nPhaPre), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  POST PHA TIME
            Array.Copy(BitConverter.GetBytes((int)settingInfo.nPhaPost), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  PQVF, FR 타입
            Array.Copy(BitConverter.GetBytes(1), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  사고 파일 자동 인쇄
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  DO 설정
            g_bSendBuffer[nTx + 0] = 0;
            g_bSendBuffer[nTx + 1] = 0;
            g_bSendBuffer[nTx + 2] = 0;
            g_bSendBuffer[nTx + 3] = 0;
            if (settingInfo.bRelay[0]) g_bSendBuffer[nTx + 2] += 0x01;
            if (settingInfo.bRelay[1]) g_bSendBuffer[nTx + 2] += 0x10;
            if (settingInfo.bRelay[2]) g_bSendBuffer[nTx + 3] += 0x01;
            if (settingInfo.bRelay[3]) g_bSendBuffer[nTx + 3] += 0x10;
            nTx += 4;

            //  TRG FIFO 개수
            Array.Copy(BitConverter.GetBytes(128), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  PHA FIFO 개수
            Array.Copy(BitConverter.GetBytes(128), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  B3DMapped
            Array.Copy(BitConverter.GetBytes((Int32)(8001 + m_nIndex)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  POS X
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            
            //  POS Y
            Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  AI 개수
            Array.Copy(BitConverter.GetBytes(16), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            //  DI 개수
            Array.Copy(BitConverter.GetBytes(16), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;
            
            //  Cut Off
            Array.Copy(BitConverter.GetBytes(100), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            return nTx;
        }

        private int parseGeneralInfo(ref Byte[] bBuf, ref IEssPcsService serviceProxy)
        {
            String strVal = "";
            Byte[] tBuf = new Byte[24];
            Int32 nVal = 0;
            int nTx = 0;
            int nRx = 0;

            nTx = makeHeaderInfo(0);
            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            m_clsSettingInfo = serviceProxy.GetSettingByIndex(m_nIndex);

            nRx = 20;

            //  Site Name
            Array.Copy(bBuf, nRx, tBuf, 0, 24);
            m_clsSettingInfo.strDevice = BytesToString(tBuf, 24, 16);
            
            nRx += 24;

            //  Site Number
            nRx += 4;

            //  PMU IP, SUBNET MASK, GATEWAY, MAC HIGH, MAC LOW
            nRx += 4;
            nRx += 4;
            nRx += 4;
            nRx += 4;
            nRx += 4;

            //  장치 전력 주파수 [50 -> 1, else 0]
            nVal = BitConverter.ToInt32(bBuf, nRx);
            if (nVal == 50)
            {
                m_clsSettingInfo.nFreq = 1;
            }
            else
            {
                m_clsSettingInfo.nFreq = 0;
            }
            nRx += 4;

            //  PSDR-XU VERSION
            nRx += 4;

            //  환경 설정 인쇄 페이지 수
            nRx += 4;

            //  장치 시간 설정
            nVal = BitConverter.ToInt32(bBuf, nRx);
            //  GPS
            if (nVal == 1)
            {
                m_clsSettingInfo.nTimeSync = 1;
            }
            //  IRIG B120
            else if (nVal == 2)
            {
                m_clsSettingInfo.nTimeSync = 3;
            }
            //  SNTP : 3
            //  RTC : 4
            else
            {
                m_clsSettingInfo.nTimeSync = 0;
            }
            nRx += 4;

            //  1PPS CHECK ENABLE
            m_clsSettingInfo.b1PPS = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
            nRx += 4;

            //  BAUDRATE
            nRx += 4;

            //  DATA BIT
            nRx += 4;

            //  PARITY BIT
            nRx += 4;

            //  LOCAL SETUP COUNT
            nRx += 4;

            //  장치 IP 주소
            //  Array.Copy(bBuf, nRx, settingInfo.bIpAddress, 0, 4);
            nRx += 4;

            //  장치 NETAMSK
            //  Array.Copy(bBuf, nRx, settingInfo.bSubnetMask, 0, 4);
            nRx += 4;

            //  장치 GATEWAY
            //  Array.Copy(bBuf, nRx, settingInfo.bGatewayAddress, 0, 4);
            nRx += 4;

            //  BUS 개수
            nRx += 4;

            //  FEEDER 개수
            nRx += 4;

            //  RMS TRIGGER ENABLE
            m_clsSettingInfo.bInsSave = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
            nRx += 4;

            //  MANUAL TRIGGER ENABLE
            m_clsSettingInfo.bTriggerManual = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
            nRx += 4;

            //  SYNC TRIGGER ENABLE
            m_clsSettingInfo.bTriggerExt = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
            nRx += 4;

            //  REMOTE TRIGGER ENABLE
            m_clsSettingInfo.bTriggerHost = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
            nRx += 4;

            //  AUTO TRIGGER ENABLE
            nRx += 4;

            //  AUTO TRIGGER 주기
            nRx += 4;

            //  PRE TRG TIME
            m_clsSettingInfo.nInsPre = (ushort)BitConverter.ToInt32(bBuf, nRx);
            nRx += 4;

            //  POST TRG TIME
            m_clsSettingInfo.nInsPost = (ushort)BitConverter.ToInt32(bBuf, nRx);
            nRx += 4;

            //  장비 타입
            nRx += 4;

            //  PRE PHA TIME
            m_clsSettingInfo.nPhaPre = (ushort)BitConverter.ToInt32(bBuf, nRx);
            nRx += 4;

            //  POST PHA TIME
            m_clsSettingInfo.nPhaPost = (ushort)BitConverter.ToInt32(bBuf, nRx);
            nRx += 4;

            //  PQVF, FR 타입
            nRx += 4;

            //  사고 파일 자동 인쇄
            nRx += 4;

            //  DO 설정
            m_clsSettingInfo.bRelay[0] = false;
            m_clsSettingInfo.bRelay[1] = false;
            m_clsSettingInfo.bRelay[2] = false;
            m_clsSettingInfo.bRelay[3] = false;
            if ((g_bSendBuffer[nRx + 2] & 0x01) == 0x01) m_clsSettingInfo.bRelay[0] = true;
            if ((g_bSendBuffer[nRx + 2] & 0x10) == 0x10) m_clsSettingInfo.bRelay[1] = true;
            if ((g_bSendBuffer[nRx + 3] & 0x01) == 0x01) m_clsSettingInfo.bRelay[2] = true;
            if ((g_bSendBuffer[nRx + 3] & 0x10) == 0x10) m_clsSettingInfo.bRelay[3] = true;
            nRx += 4;

            //  TRG FIFO 개수
            nRx += 4;

            //  PHA FIFO 개수
            nRx += 4;

            //  B3DMapped
            nRx += 4;

            //  POS X
            nRx += 4;

            //  POS Y
            nRx += 4;

            //  AI 개수
            nRx += 4;

            //  DI 개수
            nRx += 4;

            //  Cut Off
            nRx += 4;

            return nTx;
        }

        private int makeAnalogInfo(ref IEssPcsService serviceProxy)
        {
            int nTx = 0;
            int i;

            nTx = makeHeaderInfo(88);

            for (i = 0; i < 88; i++ )
            {
                g_bSendBuffer[nTx + i] = 0;
            }

            if (m_nChannelNumber < 16)
            {
                clsEssPcsSettingInfo settingInfo = serviceProxy.GetSettingByIndex(m_nIndex);

                nTx += 24;

                nTx += 4;

                nTx += 4;

                nTx += 4;

                //  Reference DC
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  Input DC
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  Primary Ref Unit
                if (m_nChannelNumber < 4)
                {
                    Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                }
                else
                {
                    Array.Copy(BitConverter.GetBytes(2), 0, g_bSendBuffer, nTx, 4);
                }
                nTx += 4;

                //  Primary Unit
                for (i = 0; i < 8; i++)
                {
                    g_bSendBuffer[nTx + i] = 0;
                }
                nTx += 8;

                nTx += 4;

                nTx += 4;

                nTx += 4;

                nTx += 4;

                nTx += 4;

                nTx += 4;

                nTx += 4;

                nTx += 4;
            }
            else
            {
                nTx += 88;
            }

            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            return nTx;
        }

        private String BytesToString(Byte[] tBuf, int nLength, int tLength)
        {
            String strVal = "";
            int i;

            for (i = 0; i < nLength; i++)
            {
                if (tBuf[i] == 0)
                {
                    break;
                }
            }

            for (; i < nLength; i++)
            {
                tBuf[i] = 0;
            }

            strVal = Encoding.Default.GetString(tBuf, 0, tLength);

            return strVal.Replace("\0", String.Empty);
        }

        private int parseAnalogInfo(ref Byte[] bBuf, ref IEssPcsService serviceProxy)
        {
            String strVal = "";
            int nTx = 0;
            int nRx = 0;
            int i;
            bool bRoc = false;

            nTx = makeHeaderInfo(0);
            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            if (m_nChannelNumber < 16)
            {
                nRx = 20;

                //  Analog Channel Name
                Byte[] tBuf = new Byte[24];
                
                Array.Copy(bBuf, nRx, tBuf, 0, 24);
                strVal = Encoding.Default.GetString(tBuf, 0, 16);
                nRx += 24;

                //  Aanlog Type
                nRx += 4;

                //  Reference
                nRx += 4;

                //  Input
                nRx += 4;

                //  Reference DC
                nRx += 4;

                //  Input DC
                nRx += 4;

                //  Primary Ref
                nRx += 4;

                //  Primary Unit
                nRx += 8;

                //  Input Mode
                nRx += 4;

                //  Level Over Trigger Enabled
                nRx += 4;

                //  Level Under Trigger Enabled
                nRx += 4;

                //  ROC Trigger Enabled
                bRoc = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
                nRx += 4;

                //  Level Over Trigger Value
                nRx += 4;

                //  Level Under Trigger Value
                nRx += 4;

                //  ROC Trigger Value
                nRx += 4;

                //  ROC Trigger Count
                nRx += 4;
            }

            return nTx;
        }

        private int makeDigitalInfo(ref IEssPcsService serviceProxy)
        {
            int nCh = 16;
            int nTx = 0;
            int i, j;

            nTx = makeHeaderInfo(nCh * 24 + 8);

            for (i = 0; i < (nCh * 24 + 8); i++)
            {
                g_bSendBuffer[nTx + i] = 0;
            }

            clsEssPcsSettingInfo settingInfo = serviceProxy.GetSettingByIndex(m_nIndex);

            if (m_nChannelNumber == 0)
            {
                nTx += 4;

                nTx += 4;
            }
            else
            {
                nTx += (nCh * 24 + 8);
            }

            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            return nTx;
        }

        private int parseDigitalInfo(ref Byte[] bBuf, ref IEssPcsService serviceProxy)
        {
            String strVal = "";
            Byte[] tBuf = new Byte[24];
            int nCh = 16;
            int nTx = 0;
            int nRx = 0;
            int i;

            nTx = makeHeaderInfo(0);
            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;


            if (m_nChannelNumber == 0)
            {
                nRx = 20;

                //  DIGITAL CHANNEL NAME
                for (i = 0; i < nCh; i++)
                {
                    Array.Copy(bBuf, nRx, tBuf, 0, 24);
                    nRx += 24;
                }

                UInt32 nContact = 0;
                UInt32 nTrigger = 0;

                nContact = BitConverter.ToUInt32(bBuf, nRx);
                nRx += 4;

                nTrigger = BitConverter.ToUInt32(bBuf, nRx);
                nRx += 4;

                for (i = 0; i < 16; i++)
                {
                    nContact >>= 1;
                    nTrigger >>= 1;
                }
            }

            return nTx;
        }

        private int makeBusInfo(ref IEssPcsService serviceProxy)
        {
            int nTx = 0;
            int i;

            nTx = makeHeaderInfo(68);

            for (i = 0; i < 68; i++)
            {
                g_bSendBuffer[nTx + i] = 0;
            }

            if (m_nChannelNumber == 0)
            {
                clsEssPcsSettingInfo settingInfo = serviceProxy.GetSettingByIndex(m_nIndex);
                Byte[] bBuf = Encoding.Default.GetBytes(settingInfo.strDevice);

                //  Bus Name : OFFSET 0
                for (i = 0; i < bBuf.Length; i++)
                {
                    g_bSendBuffer[nTx + i] = bBuf[i];
                }
                for (; i < 24; i++)
                {
                    g_bSendBuffer[nTx + i] = 0;
                }
                nTx += 24;

                //  Bus Mode : OFFSET 24
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  Va : OFFSET 28
                Array.Copy(BitConverter.GetBytes(1), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  Vb : OFFSET 32
                Array.Copy(BitConverter.GetBytes(2), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  Vc : OFFSET 36
                Array.Copy(BitConverter.GetBytes(3), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  BUS Over Trigger Enabled
                if (settingInfo.bFreqTrigUp)
                {
                    Array.Copy(BitConverter.GetBytes(1), 0, g_bSendBuffer, nTx, 4);
                }
                else
                {
                    Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                }
                nTx += 4;   //  OFFSET 40
                
                //  BUS Under Trigger Enabled
                if (settingInfo.bFreqTrigDn)
                {
                    Array.Copy(BitConverter.GetBytes(1), 0, g_bSendBuffer, nTx, 4);
                }
                else
                {
                    Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                }
                nTx += 4;   //  OFFSET 44

                //  BUS ROC Trigger Enabled
                if (settingInfo.bFreqTrigRoc > 0)
                {
                    Array.Copy(BitConverter.GetBytes(1), 0, g_bSendBuffer, nTx, 4);
                }
                else
                {
                    Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                }
                nTx += 4;   //  OFFSET 48

                //  BUS Over Trigger Value
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.fFreqTrigUp * 100)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;   //  OFFSET 52

                //  BUS Under Trigger Value
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.fFreqTrigDn * 100)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;   //  OFFSET 56

                //  BUS ROC Trigger Value
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.fFreqTrigRocRef * 100)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;   //  OFFSET 60

                //  BUS ROC Trigger Cycle
                Array.Copy(BitConverter.GetBytes((int)settingInfo.bFreqTrigRoc), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;   //  OFFSET 64
            }
            else
            {
                nTx += 68;
            }

            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            return nTx;
        }

        private int parseBusInfo(ref Byte[] bBuf, ref IEssPcsService serviceProxy)
        {
            int nTx = 0;
            int nRx = 0;

            nTx = makeHeaderInfo(0);
            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            if (m_nChannelNumber == 0)
            {
                nRx = 20;

                //  Bus Channel Name
                nRx += 24;

                //  Bus Mode
                nRx += 4;

                //  Bus Va
                nRx += 4;

                //  Bus Vb
                nRx += 4;

                //  Bus Vc
                nRx += 4;

                //  Bus Over Trigger Enabled
                m_clsSettingInfo.bFreqTrigUp = BitConverter.ToUInt32(bBuf, nRx) == 0 ? false : true;
                nRx += 4;

                //  Bus Under Trigger Enabled
                m_clsSettingInfo.bFreqTrigDn = BitConverter.ToUInt32(bBuf, nRx) == 0 ? false : true;
                nRx += 4;

                //  Bus ROC Trigger Enabled
                bool bRoc = BitConverter.ToUInt32(bBuf, nRx) == 0 ? false : true;
                nRx += 4;

                //  Bus Over Trigger Value
                m_clsSettingInfo.fFreqTrigUp = (float)(BitConverter.ToUInt32(bBuf, nRx) / 100.0f); 
                nRx += 4;

                //  Bus Under Trigger Value
                m_clsSettingInfo.fFreqTrigDn = (float)(BitConverter.ToUInt32(bBuf, nRx) / 100.0f);
                nRx += 4;

                //  Bus ROC Trigger Value
                m_clsSettingInfo.fFreqTrigRocRef = (float)(BitConverter.ToUInt32(bBuf, nRx) / 100.0f);
                nRx += 4;

                //  Bus ROC Trigger Cycle
                Byte nRoc = (Byte)(BitConverter.ToUInt32(bBuf, nRx) & 0x0F);
                nRx += 4;

                if (bRoc)
                {
                    m_clsSettingInfo.bFreqTrigRoc = nRoc;
                }
                else
                {
                    m_clsSettingInfo.bFreqTrigRoc = 0;
                }
            }

            return nTx;
        }

        private int makeFeederInfo(ref IEssPcsService serviceProxy)
        {
            int nTx = 0;
            int i;

            nTx = makeHeaderInfo(152);

            for (i = 0; i < 152; i++)
            {
                g_bSendBuffer[nTx + i] = 0;
            }
            
            if (m_nChannelNumber == 0)
            {
                clsEssPcsSettingInfo settingInfo = serviceProxy.GetSettingByIndex(m_nIndex);
                Byte[] bBuf = Encoding.Default.GetBytes(settingInfo.strDevice);

                //  Feeder Name
                for (i = 0; i < bBuf.Length; i++)
                {
                    g_bSendBuffer[nTx + i] = bBuf[i];
                }
                for (; i < 24; i++)
                {
                    g_bSendBuffer[nTx + i] = 0;
                }
                nTx += 24;

                //  설정Bus Number
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  Ia
                Array.Copy(BitConverter.GetBytes(5), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  Ib
                Array.Copy(BitConverter.GetBytes(6), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  Ic
                Array.Copy(BitConverter.GetBytes(7), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  유효전력 Reference
                Array.Copy(BitConverter.GetBytes(settingInfo.nRateP), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  유효전력 단위
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  무효전력 Reference
                Array.Copy(BitConverter.GetBytes(settingInfo.nRateQ), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  무효전력 단위
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  유효전력 Over Trigger Enabled
                Array.Copy(BitConverter.GetBytes(settingInfo.bTrigUpP ? 1 : 0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  유효전력 Under Trigger Enabled
                Array.Copy(BitConverter.GetBytes(settingInfo.bTrigDnP ? 1 : 0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  유효전력 ROC Trigger Enabled
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.bTrigRocP == 0 ? 0 : 1)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  유효전력 Swing Trigger Enabled
                Array.Copy(BitConverter.GetBytes(settingInfo.bTrigSwingP ? 1 : 0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  무효전력 Over Trigger Enabled
                Array.Copy(BitConverter.GetBytes(settingInfo.bTrigUpQ ? 1 : 0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  무효전력 Under Trigger Enabled
                Array.Copy(BitConverter.GetBytes(settingInfo.bTrigDnQ ? 1 : 0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  무효전력 ROC Trigger Enabled
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.bTrigRocQ == 0 ? 0 : 1)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  무효전력 Swing Trigger Enabled
                Array.Copy(BitConverter.GetBytes(settingInfo.bTrigSwingQ ? 1 : 0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  유효전력 Over Trigger Value
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.fTrigUpP * 10)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  유효전력 Under Trigger Value
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.fTrigDnP * 10)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  유효전력 ROC Value
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.fRocP * 10)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  유효전력 ROC Cycle
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.bTrigRocP)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  유효전력 Swing Value
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.fSwingP * 10)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  유효전력 Swing Cycle
                Array.Copy(BitConverter.GetBytes(60), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  무효전력 Over Trigger Value
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.fTrigUpQ * 10)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  무효전력 Under Trigger Value
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.fTrigDnQ * 10)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  무효전력 ROC Value
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.fRocQ * 10)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  무효전력 ROC Cycle
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.bTrigRocQ)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  무효전력 Swing Value
                Array.Copy(BitConverter.GetBytes((int)(settingInfo.fSwingQ * 10)), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  무효전력 Swing Cycle
                Array.Copy(BitConverter.GetBytes(60), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  임피던스 Xo
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;

                //  임피던스 Ro
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
                
                //  임피던스 Xp
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
                
                //  임피던스 Rp
                Array.Copy(BitConverter.GetBytes(0), 0, g_bSendBuffer, nTx, 4);
                nTx += 4;
            }
            else
            {
                nTx += 152;
            }

            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            return nTx;
        }

        private int parseFeederInfo(ref Byte[] bBuf, ref IEssPcsService serviceProxy)
        {
            bool bRocP = false;
            bool bRocQ = false;
            bool bSwingP = false;
            bool bSwingQ = false;
            int nTx = 0;
            int nRx = 0;

            nTx = makeHeaderInfo(0);
            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            if (m_nChannelNumber == 0)
            {
                nRx = 20;

                //  Feeder Name
                nRx += 24;

                //  설정Bus Number
                nRx += 4;

                //  Ia
                nRx += 4;

                //  Ib
                nRx += 4;

                //  Ic
                nRx += 4;

                //  유효전력 Reference
                m_clsSettingInfo.nRateP = (uint)(BitConverter.ToInt32(bBuf, nRx));
                nRx += 4;

                //  유효전력 단위
                nRx += 4;

                //  무효전력 Reference
                m_clsSettingInfo.nRateQ = (uint)(BitConverter.ToInt32(bBuf, nRx));
                nRx += 4;

                //  무효전력 단위
                nRx += 4;

                //  유효전력 Over Trigger Enabled
                m_clsSettingInfo.bTrigUpP = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
                nRx += 4;

                //  유효전력 Under Trigger Enabled
                m_clsSettingInfo.bTrigDnP = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
                nRx += 4;

                //  유효전력 ROC Trigger Enabled
                bRocP = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
                nRx += 4;

                //  유효전력 Swing Trigger Enabled
                m_clsSettingInfo.bTrigSwingP = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
                nRx += 4;

                //  무효전력 Over Trigger Enabled
                m_clsSettingInfo.bTrigUpQ = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
                nRx += 4;

                //  무효전력 Under Trigger Enabled
                m_clsSettingInfo.bTrigDnQ = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
                nRx += 4;

                //  무효전력 ROC Trigger Enabled
                bRocQ = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
                nRx += 4;

                //  무효전력 Swing Trigger Enabled
                m_clsSettingInfo.bTrigSwingQ = BitConverter.ToInt32(bBuf, nRx) == 1 ? true : false;
                nRx += 4;

                //  유효전력 Over Trigger Value
                m_clsSettingInfo.fTrigUpP = (float)BitConverter.ToInt32(bBuf, nRx) / 10.0f;
                nRx += 4;

                //  유효전력 Under Trigger Value
                m_clsSettingInfo.fTrigDnP = (float)BitConverter.ToInt32(bBuf, nRx) / 10.0f;
                nRx += 4;

                //  유효전력 ROC Value
                m_clsSettingInfo.fRocP = (float)BitConverter.ToInt32(bBuf, nRx) / 10.0f;
                nRx += 4;

                //  유효전력 ROC Cycle
                if (bRocP)
                {
                    m_clsSettingInfo.bTrigRocP = (Byte)BitConverter.ToInt32(bBuf, nRx);
                }
                else
                {
                    m_clsSettingInfo.bTrigRocP = 0;
                }
                nRx += 4;

                //  유효전력 Swing Value
                m_clsSettingInfo.fSwingP = (float)BitConverter.ToInt32(bBuf, nRx) / 10.0f;
                nRx += 4;

                //  유효전력 Swing Cycle
                nRx += 4;

                //  무효전력 Over Trigger Value
                m_clsSettingInfo.fTrigUpQ = (float)BitConverter.ToInt32(bBuf, nRx) / 10.0f;
                nRx += 4;

                //  무효전력 Under Trigger Value
                m_clsSettingInfo.fTrigDnQ = (float)BitConverter.ToInt32(bBuf, nRx) / 10.0f;
                nRx += 4;

                //  무효전력 ROC Value
                m_clsSettingInfo.fRocQ = (float)BitConverter.ToInt32(bBuf, nRx) / 10.0f;
                nRx += 4;

                //  무효전력 ROC Cycle
                if (bRocQ)
                {
                    m_clsSettingInfo.bTrigRocQ = (Byte)BitConverter.ToInt32(bBuf, nRx);
                }
                else
                {
                    m_clsSettingInfo.bTrigRocQ = 0;
                }
                nRx += 4;

                //  무효전력 Swing Value
                m_clsSettingInfo.fSwingQ = (float)BitConverter.ToInt32(bBuf, nRx) / 10.0f;
                nRx += 4;

                //  무효전력 Swing Cycle
                nRx += 4;

                //  임피던스 Xo
                nRx += 4;

                //  임피던스 Ro
                nRx += 4;

                //  임피던스 Xp
                nRx += 4;

                //  임피던스 Rp
                nRx += 4;
            }


            return nTx;
        }

        private int makePmuInfo(ref IEssPcsService serviceProxy)
        {
            int nTx = 0;
            int i;

            nTx = makeHeaderInfo(72);

            for (i = 0; i < 72; i++)
            {
                g_bSendBuffer[nTx + i] = 0;
            }

            clsEssPcsSettingInfo settingInfo = serviceProxy.GetSettingByIndex(m_nIndex);
            Byte[] bBuf = Encoding.Default.GetBytes(settingInfo.strDevice);

            //  PMU Name
            for (i = 0; i < bBuf.Length; i++)
            {
                g_bSendBuffer[nTx + i] = bBuf[i];
            }

            for (; i < 24; i++)
            {
                g_bSendBuffer[nTx + i] = 0;
            }

            nTx += 72;

            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);

            nTx += 4;

            return nTx;
        }

        private int parsePmuInfo(ref Byte[] bBuf, ref IEssPcsService serviceProxy)
        {
            int nTx = 0;

            nTx = makeHeaderInfo(0);           
            Array.Copy(BitConverter.GetBytes(makeCheckSum(nTx)), 0, g_bSendBuffer, nTx, 4);
            nTx += 4;

            if (serviceProxy.GetServerStatus())
            {
                clsEssPcsMsg msg = new clsEssPcsMsg();
                int nIndex = m_nIndex;

                //...msg.msgType = clsConstant.ESSPCS_CMD_INFO_SET;
                msg.msgIndex = 0;
                msg.msgDevice = serviceProxy.GetSetting(nIndex).nDeviceNo;
                msg.msgBuf = new Byte[clsConstant.PCS_SETUP_DATA_NUM];

                makeSetupData(ref m_clsSettingInfo);
                Array.Copy(m_clsSettingInfo.bSetupData, msg.msgBuf, clsConstant.PCS_SETUP_DATA_NUM);
                serviceProxy.SetCmd(msg);

                m_clsSqlServer.m_clsSettingInfo[m_nIndex] = m_clsSettingInfo;
            }

            return nTx;
        }

        private void makeSetupData(ref clsEssPcsSettingInfo settingInfo)
        {
            UInt16 uData;
            UInt32 uiData;
            int nLen;
            int i, j;

            //  001: AI보드 수
            settingInfo.bSetupData[0] = (Byte)settingInfo.nAiBoard;

            //  002: DI보드 수
            settingInfo.bSetupData[1] = (Byte)settingInfo.nDiBoard;

            //  003: SPARE
            settingInfo.bSetupData[2] = (Byte)settingInfo.nSpare;

            //  004: 피더 수
            settingInfo.bSetupData[3] = (Byte)settingInfo.nFeeder;

            //  005 - 006
            uData = 0;
            //  BIT 00: 주파수 0:60, 1: 50
            if (settingInfo.nFreq == 1) uData |= 0x0001;
            //  BIT 01: 고장기록 순시파형
            if (settingInfo.bInsSave) uData |= 0x0002;
            //  BIT 02: 고장기록 페이저
            if (settingInfo.bPhaSave) uData |= 0x0004;
            //  BIT 03 - 04: 시각 동기화 (0:내부, 1: GPS, 2: IRIG B000, 3: IRIG B120)
            uData |= (UInt16)(settingInfo.nTimeSync << 3);
            //  BIT 05: PMU Module
            if (settingInfo.bPmu) uData |= 0x0020;
            //  BIT 06: 1PPS
            if (settingInfo.b1PPS) uData |= 0x0040;
            //  BIT 07: TCP/UDP (0:TCP)
            if (settingInfo.nTcp == 1) uData |= 0x0080;
            //  BIT 08: Trigger Manual
            if (settingInfo.bTriggerManual) uData |= 0x0100;
            //  BIT 09: Trigger Host
            if (settingInfo.bTriggerHost) uData |= 0x0200;
            //  BIT 10: Trigger Ext
            if (settingInfo.bTriggerExt) uData |= 0x0400;
            //  BIT 11 - 14: Relay Enable/Disable
            if (settingInfo.bRelay[0]) uData |= 0x0800;
            if (settingInfo.bRelay[1]) uData |= 0x1000;
            if (settingInfo.bRelay[2]) uData |= 0x2000;
            if (settingInfo.bRelay[3]) uData |= 0x4000;
            settingInfo.bSetupData[4] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[5] = (Byte)((uData >> 8) & 0x00FF);

            //  #007 - #010: IP Address
            settingInfo.bSetupData[6] = settingInfo.bIpAddress[0];
            settingInfo.bSetupData[7] = settingInfo.bIpAddress[1];
            settingInfo.bSetupData[8] = settingInfo.bIpAddress[2];
            settingInfo.bSetupData[9] = settingInfo.bIpAddress[3];

            //  #011 - #014: Gateway Address
            settingInfo.bSetupData[10] = settingInfo.bGatewayAddress[0];
            settingInfo.bSetupData[11] = settingInfo.bGatewayAddress[1];
            settingInfo.bSetupData[12] = settingInfo.bGatewayAddress[2];
            settingInfo.bSetupData[13] = settingInfo.bGatewayAddress[3];

            //  #015 - #016: PMU ID
            settingInfo.bSetupData[14] = (Byte)((settingInfo.uPmuID >> 0) & 0x00FF);
            settingInfo.bSetupData[15] = (Byte)((settingInfo.uPmuID >> 8) & 0x00FF);

            //  #017: 순시 Pre
            settingInfo.bSetupData[16] = (Byte)(settingInfo.nInsPre / 100);

            //  #018: 순시 Post
            settingInfo.bSetupData[17] = (Byte)(settingInfo.nInsPost / 100);

            //  #019: PMU Data Rate [Hz]
            settingInfo.bSetupData[18] = (Byte)settingInfo.nPmuDataRate;

            //  #020: 페이저 Pre
            settingInfo.bSetupData[19] = (Byte)(settingInfo.nPhaPre / 100);

            //  #021 - #022: 페이저 Post
            uData = (UInt16)(settingInfo.nPhaPost / 100);
            settingInfo.bSetupData[20] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[21] = (Byte)((uData >> 8) & 0x00FF);

            //  #023 - #026: Subnet Mask
            settingInfo.bSetupData[22] = settingInfo.bSubnetMask[0];
            settingInfo.bSetupData[23] = settingInfo.bSubnetMask[1];
            settingInfo.bSetupData[24] = settingInfo.bSubnetMask[2];
            settingInfo.bSetupData[25] = settingInfo.bSubnetMask[3];

            //  #027 - #028: 주파수 상한 기동 주파수
            uData = (UInt16)(settingInfo.fFreqTrigUp * 10);
            settingInfo.bSetupData[26] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[27] = (Byte)((uData >> 8) & 0x00FF);

            //  #029 - #030: 주파수 하한 기동 주파수
            uData = (UInt16)(settingInfo.fFreqTrigDn * 10);
            settingInfo.bSetupData[28] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[29] = (Byte)((uData >> 8) & 0x00FF);

            //  #031: 주파수 ROC 기준 주파수
            settingInfo.bSetupData[30] = (Byte)(settingInfo.fFreqTrigRocRef * 10);

            //  #032: BIT 0 - 주파수 상한 Enable
            settingInfo.bSetupData[31] = 0;
            if (settingInfo.bFreqTrigUp) settingInfo.bSetupData[31] |= 0x01;

            //  #032: BIT 1 - 주파수 하한 Enable
            if (settingInfo.bFreqTrigDn) settingInfo.bSetupData[31] |= 0x02;

            //  #032: BIT 2 - 7 - 주파수 변동 기동 주기 설정 (0: Disable, 1 ~ 15 Cycle)
            settingInfo.bSetupData[31] |= (Byte)((settingInfo.bFreqTrigRoc << 4) & 0xF0);

            //  #033 - #036: WI-FI IP Address
            settingInfo.bSetupData[32] = settingInfo.bWifiIpAddress[0];
            settingInfo.bSetupData[33] = settingInfo.bWifiIpAddress[1];
            settingInfo.bSetupData[34] = settingInfo.bWifiIpAddress[2];
            settingInfo.bSetupData[35] = settingInfo.bWifiIpAddress[3];

            //  #037 - #040: WI-FI Gateway
            settingInfo.bSetupData[36] = settingInfo.bWifiGateway[0];
            settingInfo.bSetupData[37] = settingInfo.bWifiGateway[1];
            settingInfo.bSetupData[38] = settingInfo.bWifiGateway[2];
            settingInfo.bSetupData[39] = settingInfo.bWifiGateway[3];

            //  #041 - #044: WI-FI Subnet Mask
            settingInfo.bSetupData[40] = settingInfo.bWifiSubnetMask[0];
            settingInfo.bSetupData[41] = settingInfo.bWifiSubnetMask[1];
            settingInfo.bSetupData[42] = settingInfo.bWifiSubnetMask[2];
            settingInfo.bSetupData[43] = settingInfo.bWifiSubnetMask[3];

            //  #045 - #046: WI-FI Port
            uData = (UInt16)settingInfo.nWifiPort;
            settingInfo.bSetupData[44] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[45] = (Byte)((uData >> 8) & 0x00FF);

            //  #047: WI-FI Encryption (0:None, 1:WPA, 2:WEP)
            settingInfo.bSetupData[46] = settingInfo.bWifiEncryption;

            //  #048: WI-FI Use
            settingInfo.bSetupData[47] = 0;
            if (settingInfo.bWifiUse) settingInfo.bSetupData[47] = 1;

            //  #049 - #060: WI-FI SSID
            {
                /*
                Byte[] bData = Encoding.UTF8.GetBytes(settingInfo.strWifiSSID);

                nLen = bData.Length;
                if (nLen > 12) nLen = 12;
                Array.Copy(bData, 0, settingInfo.bSetupData, 48, nLen);
                */
            }

            //  #061: SYNC Hour
            settingInfo.bSetupData[60] = settingInfo.bSyncHour;

            //  #062: SYNC Minute
            settingInfo.bSetupData[61] = settingInfo.bSyncMinute;

            //  #063: SYNC Second
            settingInfo.bSetupData[62] = settingInfo.bSyncSecond;

            //  #064: Reserved
            settingInfo.bSetupData[63] = settingInfo.bReserved1;

            //  #065 - #076: WI-FI Password
            {
                Byte[] bData = Encoding.UTF8.GetBytes(settingInfo.strWifiPassword);

                nLen = bData.Length;
                if (nLen > 12) nLen = 12;
                Array.Copy(bData, 0, settingInfo.bSetupData, 64, nLen);
            }

            //  #077 - #080: Reserved
            settingInfo.bSetupData[76] = settingInfo.bReserved2[0];
            settingInfo.bSetupData[77] = settingInfo.bReserved2[1];
            settingInfo.bSetupData[78] = settingInfo.bReserved2[2];
            settingInfo.bSetupData[79] = settingInfo.bReserved2[3];

            //  #081 - #096: PMU Station Name
            {
                Byte[] bData = Encoding.UTF8.GetBytes(settingInfo.strPmuStationName);

                nLen = bData.Length;
                if (nLen > 16) nLen = 16;
                Array.Copy(bData, 0, settingInfo.bSetupData, 80, nLen);
            }

            for (i = 0, j = 96; i < 16; i++, j += 12)
            {
            }

            for (i = 0, j = 316; i < 16; i++, j += 16)
            {
            }

            uData = 0;

            settingInfo.bSetupData[288] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[289] = (Byte)((uData >> 8) & 0x00FF);

            uData = 0;

            settingInfo.bSetupData[290] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[291] = (Byte)((uData >> 8) & 0x00FF);

            for (i = 0, j = 572; i < 16; i++, j += 16)
            {
            }

            //  #293 - #296: 정격 유효전력 [kW]
            uiData = (UInt32)(settingInfo.nRateP);
            settingInfo.bSetupData[292] = (Byte)((uiData >> 0) & 0x000000FF);
            settingInfo.bSetupData[293] = (Byte)((uiData >> 8) & 0x000000FF);
            settingInfo.bSetupData[294] = (Byte)((uiData >> 16) & 0x000000FF);
            settingInfo.bSetupData[295] = (Byte)((uiData >> 24) & 0x000000FF);

            //  #297 - #300: 정격 무효전력 [kVAR]
            uiData = (UInt32)(settingInfo.nRateQ);
            settingInfo.bSetupData[296] = (Byte)((uiData >> 0) & 0x000000FF);
            settingInfo.bSetupData[297] = (Byte)((uiData >> 8) & 0x000000FF);
            settingInfo.bSetupData[298] = (Byte)((uiData >> 16) & 0x000000FF);
            settingInfo.bSetupData[299] = (Byte)((uiData >> 24) & 0x000000FF);

            //  #301 - #302: 유효 상한
            uData = (UInt16)(settingInfo.fTrigUpP * 10);
            settingInfo.bSetupData[300] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[301] = (Byte)((uData >> 8) & 0x00FF);

            //  #303 - #304: 무효 상한
            uData = (UInt16)(settingInfo.fTrigUpQ * 10);
            settingInfo.bSetupData[302] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[303] = (Byte)((uData >> 8) & 0x00FF);

            //  #305 - #306: 유효 하한
            uData = (UInt16)(settingInfo.fTrigDnP * 10);
            settingInfo.bSetupData[304] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[305] = (Byte)((uData >> 8) & 0x00FF);

            //  #307 - #308: 무효 하한
            uData = (UInt16)(settingInfo.fTrigDnQ * 10);
            settingInfo.bSetupData[306] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[307] = (Byte)((uData >> 8) & 0x00FF);

            //  #309: P 변동
            settingInfo.bSetupData[308] = (Byte)(settingInfo.fRocP * 10);

            //  #310: Q 변동
            settingInfo.bSetupData[309] = (Byte)(settingInfo.fRocQ * 10);

            //  #311: P Swing
            settingInfo.bSetupData[310] = (Byte)(settingInfo.fSwingP * 10);

            //  #312: Q Swing
            settingInfo.bSetupData[311] = (Byte)(settingInfo.fSwingQ * 10);

            //  #313 - #314: BIT 0 P 상한 Enable / Disable
            uData = 0;
            if (settingInfo.bTrigUpP) uData |= 0x0001;

            //  #313 - #314: BIT 1 P 하한 Enable / Disable
            if (settingInfo.bTrigDnP) uData |= 0x0002;

            //  #313 - #314: BIT 2 Q 상한 Enable / Disable
            if (settingInfo.bTrigUpQ) uData |= 0x0004;

            //  #313 - #314: BIT 3 Q 하한 Enable / Disable
            if (settingInfo.bTrigDnQ) uData |= 0x0008;

            //  #313 - #314: BIT 4 P Swing Enable / Disable
            if (settingInfo.bTrigSwingP) uData |= 0x0010;

            //  #313 - #314: BIT 5 Q Swing Enable / Disable
            if (settingInfo.bTrigSwingQ) uData |= 0x0020;

            //  #313 - #314: BIT 8 - 11 P 변동 기동 주기 설정
            uData |= (UInt16)((settingInfo.bTrigRocP << 8) & 0x0F00);

            //  #313 - #314: BIT 12 - 15 Q 변동 기동 주기 설정
            uData |= (UInt16)((settingInfo.bTrigRocQ << 12) & 0xF000);

            settingInfo.bSetupData[312] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[313] = (Byte)((uData >> 8) & 0x00FF);

            //  #315 - #316: Input Mode
            settingInfo.bInputMode[0] = 0;
            settingInfo.bInputMode[1] = 0;
            settingInfo.bSetupData[314] = settingInfo.bInputMode[0];
            settingInfo.bSetupData[315] = settingInfo.bInputMode[1];

            //  #829 - #830: PMU TCP Port
            uData = (UInt16)settingInfo.nPmuTcpPort;
            settingInfo.bSetupData[828] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[829] = (Byte)((uData >> 8) & 0x00FF);

            //  #831 - #832: PMU UDP Port
            uData = (UInt16)settingInfo.nDeviceAddr;
            settingInfo.bSetupData[830] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[831] = (Byte)((uData >> 8) & 0x00FF);

            //  #833 - #834: ETHERNET Port
            uData = (UInt16)settingInfo.nEthernetPort;
            settingInfo.bSetupData[832] = (Byte)((uData >> 0) & 0x00FF);
            settingInfo.bSetupData[833] = (Byte)((uData >> 8) & 0x00FF);
        }
    }
}
