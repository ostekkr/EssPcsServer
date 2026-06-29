using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using EssPcsService;
using System.IO;
using System.Text.RegularExpressions;

namespace EssPcsServer
{
    public partial class clsSqlServer
    {
        public List<clsEssPcsSettingInfo> m_clsSettingInfo = new List<clsEssPcsSettingInfo>();
        public List<clsEssPcsTrendInfo> m_clsTrendInfo = new List<clsEssPcsTrendInfo>();
        public String m_strLocalIp = "192.168.1.21";

        public void parseSetting(clsEssPcsSettingInfo settingInfo)
        {
            String strVal = "";
            UInt16 uData;
            int i, j;

            //  001: AI보드 수
            settingInfo.nAiBoard = settingInfo.bSetupData[0];

            //  002: DI보드 수
            settingInfo.nDiBoard = settingInfo.bSetupData[1];

            //  003: SPARE
            settingInfo.nSpare = settingInfo.bSetupData[2];

            //  004: 피더 수
            settingInfo.nFeeder = settingInfo.bSetupData[3];

            //  005 - 006
            uData = (UInt16)(settingInfo.bSetupData[4] + settingInfo.bSetupData[5] * 256);
            //  BIT 00: 주파수 0:60, 1: 50
            settingInfo.nFreq = (uData & 0x0001) == 0x0001 ? 1 : 0;
            //  BIT 01: 고장기록 순시파형
            settingInfo.bInsSave = (uData & 0x0002) == 0x0002 ? true : false;
            //  BIT 02: 고장기록 페이저
            settingInfo.bPhaSave = (uData & 0x0004) == 0x0004 ? true : false;
            //  BIT 03 - 04: 시각 동기화 (0:내부, 1: GPS, 2: IRIG B000, 3: IRIG B120)
            settingInfo.nTimeSync = (uData >> 3) & 0x0003;
            //  BIT 05: PMU Module
            settingInfo.bPmu = (uData & 0x0020) == 0x0020 ? true : false;
            //  BIT 06: 1PPS
            settingInfo.b1PPS = (uData & 0x0040) == 0x0040 ? true : false;
            //  BIT 07: TCP/UDP (0:TCP)
            settingInfo.nTcp = (uData >> 7) & 0x0001;
            //  BIT 08: Trigger Manual
            settingInfo.bTriggerManual = (uData & 0x0100) == 0x0100 ? true : false;
            //  BIT 09: Trigger Host
            settingInfo.bTriggerHost = (uData & 0x0200) == 0x0200 ? true : false;
            //  BIT 10: Trigger Ext
            settingInfo.bTriggerExt = (uData & 0x0400) == 0x0400 ? true : false;
            //  BIT 11 - 14: Relay Enable/Disable
            settingInfo.bRelay[0] = (uData & 0x0800) == 0x0800 ? true : false;
            settingInfo.bRelay[1] = (uData & 0x1000) == 0x1000 ? true : false;
            settingInfo.bRelay[2] = (uData & 0x2000) == 0x2000 ? true : false;
            settingInfo.bRelay[3] = (uData & 0x4000) == 0x4000 ? true : false;

            //  #007 - #010: IP Address
            settingInfo.bIpAddress[0] = settingInfo.bSetupData[6];
            settingInfo.bIpAddress[1] = settingInfo.bSetupData[7];
            settingInfo.bIpAddress[2] = settingInfo.bSetupData[8];
            settingInfo.bIpAddress[3] = settingInfo.bSetupData[9];

            //  #011 - #014: Gateway Address
            settingInfo.bGatewayAddress[0] = settingInfo.bSetupData[10];
            settingInfo.bGatewayAddress[1] = settingInfo.bSetupData[11];
            settingInfo.bGatewayAddress[2] = settingInfo.bSetupData[12];
            settingInfo.bGatewayAddress[3] = settingInfo.bSetupData[13];

            //  #015 - #016: PMU ID
            settingInfo.uPmuID = (UInt16)(settingInfo.bSetupData[14] + settingInfo.bSetupData[15] * 256);

            //  #017: 순시 Pre
            settingInfo.nInsPre = (UInt16)(settingInfo.bSetupData[16] * 100);

            //  #018: 순시 Post
            settingInfo.nInsPost = (UInt16)(settingInfo.bSetupData[17] * 100);

            //  #019: PMU Data Rate [Hz]
            settingInfo.nPmuDataRate = settingInfo.bSetupData[18];

            //  #020: 페이저 Pre
            settingInfo.nPhaPre = (UInt16)(settingInfo.bSetupData[19] * 100);

            //  #021 - #022: 페이저 Post
            uData = (UInt16)(settingInfo.bSetupData[20] + settingInfo.bSetupData[21] * 256);
            settingInfo.nPhaPost = (UInt16)(uData * 100);

            //  #023 - #026: Subnet Mask
            settingInfo.bSubnetMask[0] = settingInfo.bSetupData[22];
            settingInfo.bSubnetMask[1] = settingInfo.bSetupData[23];
            settingInfo.bSubnetMask[2] = settingInfo.bSetupData[24];
            settingInfo.bSubnetMask[3] = settingInfo.bSetupData[25];

            //  #027 - #028: 주파수 상한 기동 주파수
            uData = (UInt16)(settingInfo.bSetupData[26] + settingInfo.bSetupData[27] * 256);
            settingInfo.fFreqTrigUp = (float)(uData / 10.0f);

            //  #029 - #030: 주파수 하한 기동 주파수
            uData = (UInt16)(settingInfo.bSetupData[28] + settingInfo.bSetupData[29] * 256);
            settingInfo.fFreqTrigDn = (float)(uData / 10.0f);

            //  #031: 주파수 ROC 기준 주파수
            settingInfo.fFreqTrigRocRef = (float)(settingInfo.bSetupData[30] / 10.0f);

            //  #032: BIT 0 - 주파수 상한 Enable
            settingInfo.bFreqTrigUp = (settingInfo.bSetupData[31] & 0x01) == 0x01 ? true : false;

            //  #032: BIT 1 - 주파수 하한 Enable
            settingInfo.bFreqTrigDn = (settingInfo.bSetupData[31] & 0x02) == 0x02 ? true : false;

            //  #032: BIT 2 - 7 - 주파수 변동 기동 주기 설정 (0: Disable, 1 ~ 15 Cycle)
            settingInfo.bFreqTrigRoc = (Byte)((settingInfo.bSetupData[31] >> 4) & 0x0F);

            //  #033 - #036: WI-FI IP Address
            settingInfo.bWifiIpAddress[0] = settingInfo.bSetupData[32];
            settingInfo.bWifiIpAddress[1] = settingInfo.bSetupData[33];
            settingInfo.bWifiIpAddress[2] = settingInfo.bSetupData[34];
            settingInfo.bWifiIpAddress[3] = settingInfo.bSetupData[35];

            //  #037 - #040: WI-FI Gateway
            settingInfo.bWifiGateway[0] = settingInfo.bSetupData[36];
            settingInfo.bWifiGateway[1] = settingInfo.bSetupData[37];
            settingInfo.bWifiGateway[2] = settingInfo.bSetupData[38];
            settingInfo.bWifiGateway[3] = settingInfo.bSetupData[39];

            //  #041 - #044: WI-FI Subnet Mask
            settingInfo.bWifiSubnetMask[0] = settingInfo.bSetupData[40];
            settingInfo.bWifiSubnetMask[1] = settingInfo.bSetupData[41];
            settingInfo.bWifiSubnetMask[2] = settingInfo.bSetupData[42];
            settingInfo.bWifiSubnetMask[3] = settingInfo.bSetupData[43];

            //  #045 - #046: WI-FI Port
            settingInfo.nWifiPort = (UInt16)(settingInfo.bSetupData[44] + settingInfo.bSetupData[45] * 256);

            //  #047: WI-FI Encryption (0:None, 1:WPA, 2:WEP)
            settingInfo.bWifiEncryption = (Byte)(settingInfo.bSetupData[46] & 0x03);

            //  #048: WI-FI Use
            settingInfo.bWifiUse = settingInfo.bSetupData[47] == 0 ? false : true;

            //  #049 - #060: WI-FI SSID
            {
                Byte[] bData = new Byte[12];

                Array.Copy(settingInfo.bSetupData, 48, bData, 0, 12);
                strVal = Encoding.Default.GetString(bData);
                //...settingInfo.strWifiSSID = strVal.Replace("\0", String.Empty);
            }

            //  #061: SYNC Hour
            settingInfo.bSyncHour = settingInfo.bSetupData[60];

            //  #062: SYNC Minute
            settingInfo.bSyncMinute = settingInfo.bSetupData[61];

            //  #063: SYNC Second
            settingInfo.bSyncSecond = settingInfo.bSetupData[62];

            //  #064: Reserved
            settingInfo.bReserved1 = settingInfo.bSetupData[63];

            //  #065 - #076: WI-FI Password
            {
                Byte[] bData = new Byte[12];

                Array.Copy(settingInfo.bSetupData, 64, bData, 0, 12);
                strVal = Encoding.Default.GetString(bData);
                settingInfo.strWifiPassword = strVal.Replace("\0", String.Empty);
            }

            //  #077 - #080: Reserved
            settingInfo.bReserved2[0] = settingInfo.bSetupData[76];
            settingInfo.bReserved2[1] = settingInfo.bSetupData[77];
            settingInfo.bReserved2[2] = settingInfo.bSetupData[78];
            settingInfo.bReserved2[3] = settingInfo.bSetupData[79];

            //  #081 - #096: PMU Station Name
            {
                Byte[] bData = new Byte[16];

                Array.Copy(settingInfo.bSetupData, 80, bData, 0, 16);
                strVal = Encoding.Default.GetString(bData);
                settingInfo.strPmuStationName = strVal.Replace("\0", String.Empty);
            }

            for (i = 0, j = 96; i < 16; i++, j += 12)
            {
                Byte[] bData = new Byte[12];

                Array.Copy(settingInfo.bSetupData, j, bData, 0, 12);

                //  #108: BIT 0 - 1 채널 종류 (0:PT, 1:CT, 2:4-20mA, 3:ZeroPT
                //  #108: BIT 2 상한 Enable
                //  #108: BIT 3 하한 Enable
                //  #108: BIT 4 - 7 변동 기동 주기 설정
            }

            for (i = 0, j = 316; i < 16; i++, j += 16)
            {
                Byte[] bData = new Byte[16];

                Array.Copy(settingInfo.bSetupData, j, bData, 0, 16);
                strVal = Encoding.Default.GetString(bData);
            }

            uData = (UInt16)(settingInfo.bSetupData[288] + settingInfo.bSetupData[289] * 256);

            uData = (UInt16)(settingInfo.bSetupData[290] + settingInfo.bSetupData[291] * 256);

            for (i = 0, j = 572; i < 16; i++, j += 16)
            {
                Byte[] bData = new Byte[16];

                Array.Copy(settingInfo.bSetupData, j, bData, 0, 16);
                strVal = Encoding.Default.GetString(bData);
            }

            //  #293 - #296: 정격 유효전력 [kW]
            settingInfo.nRateP = (UInt32)(settingInfo.bSetupData[292] + settingInfo.bSetupData[293] * 0x100 + settingInfo.bSetupData[294] * 0x10000 + settingInfo.bSetupData[295] * 0x1000000);

            //  #297 - #300: 정격 무효전력 [kVAR]
            settingInfo.nRateQ = (UInt32)(settingInfo.bSetupData[296] + settingInfo.bSetupData[297] * 0x100 + settingInfo.bSetupData[298] * 0x10000 + settingInfo.bSetupData[299] * 0x1000000);

            //  #301 - #302: 유효 상한
            uData = (UInt16)(settingInfo.bSetupData[300] + settingInfo.bSetupData[301] * 0x100);
            settingInfo.fTrigUpP = (float)(uData / 10.0f);

            //  #303 - #304: 무효 상한
            uData = (UInt16)(settingInfo.bSetupData[302] + settingInfo.bSetupData[303] * 0x100);
            settingInfo.fTrigUpQ = (float)(uData / 10.0f);

            //  #305 - #306: 유효 하한
            uData = (UInt16)(settingInfo.bSetupData[304] + settingInfo.bSetupData[305] * 0x100);
            settingInfo.fTrigDnP = (float)(uData / 10.0f);

            //  #307 - #308: 무효 하한
            uData = (UInt16)(settingInfo.bSetupData[306] + settingInfo.bSetupData[307] * 0x100);
            settingInfo.fTrigDnQ = (float)(uData / 10.0f);

            //  #309: P 변동
            settingInfo.fRocP = (float)(settingInfo.bSetupData[308] / 10.0f);

            //  #310: Q 변동
            settingInfo.fRocQ = (float)(settingInfo.bSetupData[309] / 10.0f);

            //  #311: P Swing
            settingInfo.fSwingP = (float)(settingInfo.bSetupData[310] / 10.0f);

            //  #312: Q Swing
            settingInfo.fSwingQ = (float)(settingInfo.bSetupData[311] / 10.0f);

            uData = (UInt16)(settingInfo.bSetupData[312] + settingInfo.bSetupData[313] * 0x100);

            //  #313 - #314: BIT 0 P 상한 Enable / Disable
            settingInfo.bTrigUpP = (uData & 0x0001) == 0x0001 ? true : false;

            //  #313 - #314: BIT 1 P 하한 Enable / Disable
            settingInfo.bTrigDnP = (uData & 0x0002) == 0x0002 ? true : false;

            //  #313 - #314: BIT 2 Q 상한 Enable / Disable
            settingInfo.bTrigUpQ = (uData & 0x0004) == 0x0004 ? true : false;

            //  #313 - #314: BIT 3 Q 하한 Enable / Disable
            settingInfo.bTrigDnQ = (uData & 0x0008) == 0x0008 ? true : false;

            //  #313 - #314: BIT 4 P Swing Enable / Disable
            settingInfo.bTrigSwingP = (uData & 0x0010) == 0x0010 ? true : false;

            //  #313 - #314: BIT 5 Q Swing Enable / Disable
            settingInfo.bTrigSwingQ = (uData & 0x0020) == 0x0020 ? true : false;

            //  #313 - #314: BIT 8 - 11 P 변동 기동 주기 설정
            settingInfo.bTrigRocP = (Byte)((uData >> 8) & 0x000F);

            //  #313 - #314: BIT 12 - 15 Q 변동 기동 주기 설정
            settingInfo.bTrigRocQ = (Byte)((uData >> 12) & 0x000F);

            //  #315 - #316: 아날로그 채널 결선 상태
            settingInfo.bInputMode[0] = settingInfo.bSetupData[314];
            settingInfo.bInputMode[1] = settingInfo.bSetupData[315];

            //  #829 - #830: PMU TCP Port
            settingInfo.nPmuTcpPort = (UInt16)(settingInfo.bSetupData[828] + settingInfo.bSetupData[829] * 256);

            //  #831 - #832: Device Address
            settingInfo.nDeviceAddr = (UInt16)(settingInfo.bSetupData[830] + settingInfo.bSetupData[831] * 256);

            //  #833 - #834: Ethernet Port
            settingInfo.nEthernetPort = (UInt16)(settingInfo.bSetupData[832] + settingInfo.bSetupData[833] * 256);

            //  ANALOG GAIN
            for (i = 0; i < 4; i++)
            {
            }

            for (; i < 16; i++)
            {
            }
        }

        public string Setting_Make()
        {
            m_clsSettingInfo.Clear();
            m_clsTrendInfo.Clear();
            
            return Setting_ReadAll();
        }

        public string Setting_ReadAll()
        {
            string strResult = "";
            int nIndex;

            try
            {
                for (nIndex = 0; nIndex < clsConstant.ESSPCS_MAX_NUM; nIndex++)
                {
                    clsEssPcsSettingInfo settingInfo = new clsEssPcsSettingInfo();
                    clsEssPcsTrendInfo trendInfo = new clsEssPcsTrendInfo();

                    settingInfo.nDeviceNo = nIndex;

                    m_clsSettingInfo.Add(settingInfo);

                    strResult = Setting_Read(nIndex);

                    if (m_clsSettingInfo[nIndex].strDevice == "")
                    {
                        m_clsSettingInfo[nIndex].strDevice = String.Format("{0:00}. 설정 정보 없음", nIndex + 1);
                    }

                    if (strResult != "")
                    {

                    }

                    trendInfo.nDeviceNo = settingInfo.nDeviceNo;
                    trendInfo.strDeviceName = settingInfo.strDevice;
                    trendInfo.strLoc = settingInfo.strLOC;
                    trendInfo.strComment = settingInfo.strCOMMENT;

                    m_clsTrendInfo.Add(trendInfo);
                }
            }
            catch (Exception e)
            {
                strResult = "Setting_ReadAll : " + e.Message;
            }
            finally
            {
            }

            return strResult;
        }

        public string Setting_Read(int nIndex)
        {
            String strResult = "";
            String strLine;

            try
            {
                String strFile;

                strFile = String.Format(clsConstant.APP_PATH + "setup\\setup_{0}.txt", nIndex + 1);

                using (StreamReader sr = new StreamReader(strFile))
                {
                    while ((strLine = sr.ReadLine()) != null)
                    {
                        String[] strSplit = strLine.Split('=');
                        String strVal = "";

                        if (strSplit[0] == "DEVICE")
                        {
                            strVal = strSplit[1].Replace("\0", string.Empty);
                            m_clsSettingInfo[nIndex].strDevice = Regex.Replace(strVal, @"[^\u0000-\u007F]+", String.Empty);
                        }
                        else if (strSplit[0] == "LOC")
                        {
                            strVal = strSplit[1].Replace("\0", string.Empty);
                            m_clsSettingInfo[nIndex].strLOC = Regex.Replace(strVal, @"[^\u0000-\u007F]+", String.Empty);
                        }
                        else if (strSplit[0] == "USED")
                        {
                            m_clsSettingInfo[nIndex].bUSED = Int32.Parse(strSplit[1]) == 1 ? true : false;
                        }
                        else if (strSplit[0] == "COMMENT")
                        {
                            strVal = strSplit[1].Replace("\0", string.Empty);
                            m_clsSettingInfo[nIndex].strCOMMENT = Regex.Replace(strVal, @"[^\u0000-\u007F]+", String.Empty);
                        }
                        else if (strSplit[0] == "SITE")
                        {
                            m_clsSettingInfo[nIndex].nTagSite = Int32.Parse(strSplit[1]);
                        }
                        else if (strSplit[0] == "SPOT")
                        {
                            m_clsSettingInfo[nIndex].nTagSpot = Int32.Parse(strSplit[1]);
                        }
                        else if (strSplit[0] == "TYPE")
                        {
                            m_clsSettingInfo[nIndex].nTagDeviceType = Int32.Parse(strSplit[1]);
                        }
                        else if (strSplit[0] == "ID")
                        {
                            m_clsSettingInfo[nIndex].nTagDeviceId = Int32.Parse(strSplit[1]);
                        }
                        else if (strSplit[0] == "SERIAL")
                        {
                            m_clsSettingInfo[nIndex].strSerial = strSplit[1];
                        }
                        else if (strSplit[0] == "IP")
                        {
                            m_strLocalIp = strSplit[1];
                        }
                    }
                }

                strFile = String.Format(clsConstant.APP_PATH + "setup\\setup_{0}.dat", nIndex + 1);

                using (FileStream fs = File.Open(strFile, FileMode.Open))
                {
                    m_clsSettingInfo[nIndex].bSetupData = new BinaryReader(fs).ReadBytes((int)fs.Length);

                    parseSetting(m_clsSettingInfo[nIndex]);
                }
            }
            catch (Exception e)
            {
                strResult = "Setting_Read : " + e.Message;
            }

            return strResult;
        }

        public string Setting_Remove(int nIndex)
        {
            SqlCommand Com;
            string Sql;
            string strResult = "";

#if USE_EXCEPTION
            try
            {
#endif
                g_bIsUse = true;

                Sql = "DELETE FROM tblSetting WHERE NO=@NO";
                Com = new SqlCommand(Sql, m_conSql);

                Com.Parameters.Add("@NO", SqlDbType.Int);

                Com.Parameters["@NO"].Value = m_clsSettingInfo[nIndex].nDeviceNo;

                Com.ExecuteNonQuery();
#if USE_EXCEPTION
            }
            catch (Exception ex)
            {
                strResult = "Setting_Remove : " + nIndex.ToString() + " > " + ex.Message;
                clsLog.addSqlLog(strResult);
            }
            finally
            {
                g_bIsUse = false;
            }
#endif
            return strResult;
        }

        public string getPtRatio(int index)
        {
            switch (index)
            {
                case 0: return "63.5V";
                case 1: return "110V";
                case 2: return "115V";
                case 3: return "220V";
            }

            return "";
        }

        public string getCtRatio(int index)
        {
            switch (index)
            {
                case 0: return "5A";
                case 1: return "1A";
                case 2: return "120A";
                case 3: return "40A";
            }

            return "";
        }

        public string getWireMode(int index)
        {
            switch (index)
            {
                case 0: return "3상 4선식";
                case 1: return "3상 3선식";
                case 2: return "단상 3선식";
                case 3: return "단상 2선식";
            }

            return "";
        }

        public string getSamples(int index)
        {
            switch (index)
            {
                case 0: return "4096 샘플";
                case 1: return "8192 샘플";
                case 2: return "12288 샘플";
                case 3: return "16384 샘플";
            }

            return "";
        }
    }
}
