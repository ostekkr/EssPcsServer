using EssPcsService;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EssPcsServer
{
    public partial class frmMain
    {
        Stopwatch m_swSQLite = new Stopwatch();
        public List<String> m_JsonList = new List<String>();
        public UInt32 m_nDbSaveIndex = 0;

        String makeJsonString(int i, clsEssPcsTrendInfo clsTrend)
        {
            var json = new JObject();
            double fPs = 0;
            double fAcos = 0;
            int nStatus = 0;

            //  pcs_TIME: 계측 시각 / timestamp
            String strMeasureTime = String.Format("{0:0000}{1:00}{2:00}{3:00}{4:00}{5:00}",
                clsTrend.dtTime.Year,
                clsTrend.dtTime.Month,
                clsTrend.dtTime.Day,
                clsTrend.dtTime.Hour,
                clsTrend.dtTime.Minute,
                clsTrend.dtTime.Second);

            json.Add("REC_TIME", strMeasureTime);
            TimeSpan ts = clsTrend.dtTime - new DateTime(1970, 1, 1, 0, 0, 0);
            json.Add("pcs_TIME", ts.TotalMilliseconds);

            //  pcs_VDC: 입력(DC) 전압 / V
            json.Add("pcs_VDC", clsTrend.fBatteryVoltage);

            //  pcs_ADC: 입력(DC) 전류 / A
            json.Add("pcs_ADC", clsTrend.fBatteryCurrent);

            //  pcs_PDC: 입력(DC) 전력 / kW
            json.Add("pcs_PDC", clsTrend.fBatteryPower);

            //  pcs_V1: AC 전압(R) / V
            json.Add("pcs_V1", clsTrend.fGRID_Volt);

            //  pcs_V2: AC 전압(S) / V
            json.Add("pcs_V2", clsTrend.fGRID_Volt);

            //  pcs_V3: AC 전압(T) / V
            json.Add("pcs_V3", clsTrend.fGRID_Volt);

            //  pcs_A1: AC 전류(R) / A
            json.Add("pcs_A1", clsTrend.fGRID_Current);

            //  pcs_A2: AC 전류(S) / A
            json.Add("pcs_A2", clsTrend.fGRID_Current);

            //  pcs_A3: AC 전류(T) / A
            json.Add("pcs_A3", clsTrend.fGRID_Current);

            /*
            //  pcs_A1: AC 전류(R) / A
            json.Add("pcs_A1", clsTrend.fGRID_Current / 3.0f);

            //  pcs_A2: AC 전류(S) / A
            json.Add("pcs_A2", clsTrend.fGRID_Current / 3.0f);

            //  pcs_A3: AC 전류(T) / A
            json.Add("pcs_A3", clsTrend.fGRID_Current / 3.0f);
            */

            //  pcs_PAC: 유효전력 / kW
            json.Add("pcs_PAC", clsTrend.fGRID_Power);

            //  pcs_QAC: 무효전력 / kVAR
            fPs = 0;
            if (clsTrend.nInvStatus == 1)
            {
                if (Math.Abs(clsTrend.fTotalPowerFactor) > 0.001)
                {
                    fAcos = Math.Acos(clsTrend.fTotalPowerFactor);
                    fPs = clsTrend.fGRID_Power / clsTrend.fTotalPowerFactor;
                    fPs *= Math.Sin(fAcos);
                }
            }
            json.Add("pcs_QAC", (float)fPs);

            //  pcs_SET_ECHO: 설정 충방전 전력 / kW
            json.Add("pcs_SET_ECHO", clsTrend.fReferenceP);

            //  pcs_PF: 역률 / %
            json.Add("pcs_PF", clsTrend.fTotalPowerFactor * 100);

            //  pcs_FREQ: 주파수 / Hz
            json.Add("pcs_FREQ", clsTrend.fGRID_Frequency);

            //  pcs_ACC_C: 누적 충전량 / MWh
            json.Add("pcs_ACC_C", clsTrend.fAccCharge * -0.001f);

            //  pcs_ACC_D: 누적 방전량 / MWh
            json.Add("pcs_ACC_D", clsTrend.fAccDischarge * 0.001f);

            //  pcs_TEMP: 온도 / °C
            json.Add("pcs_TEMP", clsTrend.fTemp_InternalPcs);

            //  pcs_STATUS: 동작상태    0:stop, 1:run, 2:unkown
            //  INV-Status: 1[Run] 2[Ready] 3[Fault]
            if (clsTrend.nInvStatus == 1)
            {
                json.Add("pcs_STATUS", 1);
            }
            else if (clsTrend.nInvStatus == 2)
            {
                json.Add("pcs_STATUS", 0);
            }
            else
            {
                json.Add("pcs_STATUS", 2);
            }

            //  pcs_CHGSTA: 충방전 상태  0: Idle 1: Charge 2:Discharge
            if (clsTrend.fGRID_Current > 0)
            {
                json.Add("pcs_CHGSTA", 2);
            }
            else if (clsTrend.fGRID_Current < 0)
            {
                json.Add("pcs_CHGSTA", 1);
            }
            else
            {
                json.Add("pcs_CHGSTA", 0);
            }

            //  pcs_COMMSTA: 통신 상태   0: Normal 1: Error
            if (clsTrend.m_flagFaultStatus.bit06_BatteryCommLost)
            {
                json.Add("pcs_COMMSTA", 1);
            }
            else
            {
                json.Add("pcs_COMMSTA", 0);
            }

            //  pcs_WARNSTA: 경고 유무
            //  bit 0 : 직류 부족전압 경고 여부 (0 : Normal, 1 : Warning)
            //  bit 1 : 교류 부족전압 경고 여부(0 : Normal, 1 : Warning)
            //  bit 2 : 주파수 상승 경고 여부(0 : Normal, 1 : Warning)
            //  bit 3 : 주파수 저하 경고 여부(0 : Normal, 1 : Warning)
            //  bit 4 : 과 온도 경고 여부(0 : Normal, 1 : Warning)
            //  bit 5 : 직류 과전압 경고 여부(0 : Normal, 1 : Warning)
            //  bit 6 : 교류 과전압 경고 여부(0 : Normal, 1 : Warning)
            //  bit 7 : 교류 과전류 경고 여부(0 : Normal, 1 : Warning)
            //  bit 8 : 직류 과전류 경고 여부(0 : Normal, 1 : Warning)
            //  bit 15 : 기타 경고 여부(0 : Normal, 1 : Warning)
            json.Add("pcs_WARNSTA", nStatus);

            //  pcs_FAULTSTA: 고장 유무
            //  bit 0 : 직류 부족전압 고장 여부 (0 : Normal, 1 : Fault)
            //  bit 1 : 교류 부족전압 고장 여부(0 : Normal, 1 : Fault)
            //  bit 2 : 주파수 상승 고장 여부(0 : Normal, 1 : Fault)
            //  bit 3 : 주파수 저하 고장 여부(0 : Normal, 1 : Fault)
            //  bit 4 : 과 온도 고장 여부(0 : Normal, 1 : Fault)
            //  bit 5 : 직류 과전압 고장 여부(0 : Normal, 1 : Fault)
            //  bit 6 : 교류 과전압 고장 여부(0 : Normal, 1 : Fault)
            //  bit 7 : 교류 과전류 고장 여부(0 : Normal, 1 : Fault)
            //  bit 8 : 직류 과전류 고장 여부(0 : Normal, 1 : Fault)
            //  bit 9 : 지락 전류 고장 여부(0 : Normal, 1 : Fault)
            //  bit 15 : 기타 고장 여부(0 : Normal, 1 : Fault)

            //  인버터고장
            if (clsTrend.m_flagWarningStatus.bit00_InverterFault) nStatus |= 0x8000;
            //  계통과전압
            if (clsTrend.m_flagWarningStatus.bit01_GridOverVoltage) nStatus |= 0x0040;
            //  계통과주파수
            if (clsTrend.m_flagWarningStatus.bit02_GridOverFrequency) nStatus |= 0x0004;
            //  변압기과열
            if (clsTrend.m_flagWarningStatus.bit04_TransformerOverHeat) nStatus |= 0x0010;
            //  필터인덕터과열
            if (clsTrend.m_flagWarningStatus.bit05_FilterInductorOverHeat) nStatus |= 0x0010;
            //  필터커패시터과열
            if (clsTrend.m_flagWarningStatus.bit06_FilterCapacitorOverHeat) nStatus |= 0x0010;
            //  함체내부과열
            if (clsTrend.m_flagWarningStatus.bit07_EnclosureInternalOverHeat) nStatus |= 0x0010;
            //  주손실
            if (clsTrend.m_flagWarningStatus.bit08_FaultMainLoss) nStatus |= 0x8000;
            //  절연파괴
            if (clsTrend.m_flagWarningStatus.bit09_DcImd1) nStatus |= 0x8000;
            //  SPD1개방
            if (clsTrend.m_flagWarningStatus.bit10_ControlSpd1Open) nStatus |= 0x8000;
            //  배터리 과열
            if (clsTrend.m_flagWarningStatus.bit11_BatteryOverHeat) nStatus |= 0x0010;
            //   DC트립
            if (clsTrend.m_flagWarningStatus.bit12_DcCb1Trip) nStatus |= 0x8000;
            //  AC트립
            if (clsTrend.m_flagWarningStatus.bit13_AcOutCb2Trip) nStatus |= 0x8000;
            //  비상정지
            if (clsTrend.m_flagWarningStatus.bit15_EstopPushedOrDoorOpen) nStatus |= 0x8000;

            //  계통저전압
            if (clsTrend.m_flagFaultStatus.bit01_GridUnderVoltage) nStatus |= 0x0002;
            //  계통저주파수
            if (clsTrend.m_flagFaultStatus.bit02_GridUnderFrequency) nStatus |= 0x0008;
            //  DC과전압
            if (clsTrend.m_flagFaultStatus.bit03_DcOverVoltage) nStatus |= 0x0020;
            //  DC저전압
            if (clsTrend.m_flagFaultStatus.bit04_DcUnderVoltage) nStatus |= 0x0001;
            //  DC과전류
            if (clsTrend.m_flagFaultStatus.bit05_DcOverCurrent) nStatus |= 0x0080;
            //  배터리통신손실
            if (clsTrend.m_flagFaultStatus.bit06_BatteryCommLost) nStatus |= 0x8000;
            //  CB1개방
            if (clsTrend.m_flagFaultStatus.bit07_Cb1Open) nStatus |= 0x8000;
            //  CB2개방
            if (clsTrend.m_flagFaultStatus.bit08_Cb2Open) nStatus |= 0x8000;
            //  교류비대칭
            if (clsTrend.m_flagFaultStatus.bit09_AcCurrentUnbalance) nStatus |= 0x8000;
            //  MC3개방
            if (clsTrend.m_flagFaultStatus3.bit00_Mc3OpenTimeout) nStatus |= 0x8000;
            //  정지타임아웃
            if (clsTrend.m_flagFaultStatus3.bit01_InverterRunOffTimeout) nStatus |= 0x8000;
            //  동기OFF타임아웃
            if (clsTrend.m_flagFaultStatus3.bit02_SyncDoneOffTimeout) nStatus |= 0x8000;
            //  MC개방타임아웃
            if (clsTrend.m_flagFaultStatus3.bit03_NetMcOpenTimeout) nStatus |= 0x8000;
            //  실행타임아웃
            if (clsTrend.m_flagFaultStatus3.bit04_InverterRunOnTimeout) nStatus |= 0x8000;
            //  동기ON타임아웃
            if (clsTrend.m_flagFaultStatus3.bit05_SyncDoneOnTimeout) nStatus |= 0x8000;
            //  접속ON타임아웃
            if (clsTrend.m_flagFaultStatus3.bit06_NetCloseOnTimeout) nStatus |= 0x8000;
            //  MC접속타임아웃
            if (clsTrend.m_flagFaultStatus3.bit07_NetMcCloseTimeout) nStatus |= 0x8000;
            //  HMI통신타임아웃
            if (clsTrend.m_flagFaultStatus3.bit14_CommWithHmiTimeout) nStatus |= 0x8000;
            //  인버터통신타임아웃
            if (clsTrend.m_flagFaultStatus3.bit15_CommWithInverterTimeout) nStatus |= 0x8000;
            //  필드버스오류
            if (clsTrend.m_flagFaultStatus4.bit01_FieldbusSlotDFault) nStatus |= 0x8000;
            //  시스템버스오류
            if (clsTrend.m_flagFaultStatus4.bit02_SystemBusFault) nStatus |= 0x8000;
            //  슬롯통신오류
            if (clsTrend.m_flagFaultStatus4.bit03_SlotCommunicationFault) nStatus |= 0x8000;
            //  출력위상손실
            if (clsTrend.m_flagFaultStatus4.bit04_OutputPhaseLost) nStatus |= 0x8000;
            //  외부오류
            if (clsTrend.m_flagFaultStatus4.bit05_ExternalFault) nStatus |= 0x8000;
            //  메인스위치상태오류
            if (clsTrend.m_flagFaultStatus4.bit06_MainSwitchStateFault) nStatus |= 0x8000;
            //  IGBT과열
            if (clsTrend.m_flagFaultStatus4.bit07_IgbtOverHeat) nStatus |= 0x0010;
            //  인버터과열
            if (clsTrend.m_flagFaultStatus5.bit00_InverterOverHeat) nStatus |= 0x0010;
            //  인버터과냉
            if (clsTrend.m_flagFaultStatus5.bit01_InverterOverCool) nStatus |= 0x8000;
            //  예비충전실패
            if (clsTrend.m_flagFaultStatus5.bit02_PrechargeFailed) nStatus |= 0x8000;
            //  계통전압이상
            if (clsTrend.m_flagFaultStatus5.bit03_GridVoltageAbnormal) nStatus |= 0x8000;
            //  계통주파수이상
            if (clsTrend.m_flagFaultStatus5.bit04_GridFreqAbnormal) nStatus |= 0x8000;
            //  OPT손실
            if (clsTrend.m_flagFaultStatus5.bit05_OptD7Missing) nStatus |= 0x8000;
            //  계통단락
            if (clsTrend.m_flagFaultStatus5.bit06_GridShortCircuit) nStatus |= 0x8000;
            //  계통고조파이상
            if (clsTrend.m_flagFaultStatus5.bit07_GridOverThdv) nStatus |= 0x8000;
            //  교류과전류
            if (clsTrend.m_flagFaultStatus5.bit08_AcOverCurrent) nStatus |= 0x0100;
            //  과부하
            if (clsTrend.m_flagFaultStatus5.bit09_AcOverLoad) nStatus |= 0x8000;
            //  지락
            if (clsTrend.m_flagFaultStatus5.bit10_EarthFault) nStatus |= 0x0200;
            //  인버터저전압
            if (clsTrend.m_flagFaultStatus5.bit11_InverterUnderVoltage) nStatus |= 0x0002;
            //  인버터과전압
            if (clsTrend.m_flagFaultStatus5.bit12_InverterOverVoltage) nStatus |= 0x0040;
            //  인버터온도센서이상
            if (clsTrend.m_flagFaultStatus5.bit13_InverterTemperatureSensorFault) nStatus |= 0x8000;
            //  계통동기실패
            if (clsTrend.m_flagFaultStatus5.bit14_LineSyncFault) nStatus |= 0x8000;
            //  통신이상
            if (clsTrend.m_flagFaultStatus5.bit15_PcCommunicationFault) nStatus |= 0x8000;
            //  랙과전압
            if (clsTrend.m_flagFaultStatus8.bit00_Rbms1OverRackVoltage) nStatus |= 0x0020;
            //  랙저전압
            if (clsTrend.m_flagFaultStatus8.bit01_Rbms1UnderRackVoltage) nStatus |= 0x0001;
            //  과충전
            if (clsTrend.m_flagFaultStatus8.bit02_Rbms1OverSoc) nStatus |= 0x8000;
            //  과방전
            if (clsTrend.m_flagFaultStatus8.bit03_Rbms1UnderSoc) nStatus |= 0x8000;
            //  MC타임아웃
            if (clsTrend.m_flagFaultStatus8.bit04_Rbms1BatteryMcOnIdleTimeout) nStatus |= 0x8000;
            //  이상
            if (clsTrend.m_flagFaultStatus8.bit06_Rbms1AnyWarningOrFault) nStatus |= 0x8000;
            //  대기타임아웃
            if (clsTrend.m_flagFaultStatus8.bit07_Rbms1StandbyTimeout) nStatus |= 0x8000;  
            json.Add("pcs_FAULTSTA", nStatus);

            return json.ToString();
        }

        private void runSQLite()
        {
            if (m_swSQLite.ElapsedMilliseconds > 1000)
            {
                try
                {
                    var con = new SQLiteConnection("Data Source=" + clsConstant.APP_DB_NAME);

                    con.Open();

                    var cmd = new SQLiteCommand(con);

                    while (m_JsonList.Count > 0)
                    {
                        if (Properties.Settings.Default.runSQLite)
                        {
                            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0);

                            cmd.CommandText = string.Format("INSERT INTO tblJson(JSTR,JDT) VALUES('{0}',{1})", m_JsonList[0], ts.TotalSeconds);
                            cmd.ExecuteNonQuery();
                        }

                        m_JsonList.RemoveAt(0);
                    }

                    con.Close();
                }
                catch (Exception ex)
                {
                    toolStripStatusDisplayLog.Text = String.Format("{0} > {1}", DateTime.Now, ex.Message);
                }
                finally
                {
                    m_swSQLite.Restart();
                }
            }
        }

        private bool sendJsonString(Int32 nIndex, String strSite, Int32 nPort, String strJson)
        {
            bool bResult = false;

            try
            {
                if (nPort == 0)
                {
                    bResult = true;
                }
                else
                { 
                    String strUrl = strSite + ":" + nPort.ToString() + "/spot_1";

                    var httpWebRequest = (WebRequest)WebRequest.Create(strUrl);

                    httpWebRequest.ContentType = "application/json";
                    httpWebRequest.Timeout = 1000;
                    httpWebRequest.Method = "POST";

                    using (var jsonStreamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        jsonStreamWriter.Write(strJson);
                        jsonStreamWriter.Flush();
                        jsonStreamWriter.Close();

                        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                        try
                        {
                            if (httpResponse.StatusCode == HttpStatusCode.NoContent)
                            {
                                displayTopicLink(nIndex, DateTime.Now, String.Format("Send To : {0}", strUrl));
                                bResult = true;
                            }
                            else
                            {
                                displayTopicLink(nIndex, DateTime.Now, String.Format("Error : {0} / {1}", httpResponse.StatusCode, httpResponse.GetResponseStream()));
                            }
                        }
                        catch (Exception ex)
                        {
                            clsLog.addExceptionLog(ex.Message);

                            displayTopicLink(nIndex, DateTime.Now, ex.Message);

                            toolStripStatusDisplayLog.Text = String.Format("{0} > {1}", DateTime.Now.ToString(), ex.Message);
                        }
                        finally
                        {
                            if (httpResponse != null)
                            {
                                httpResponse.Close();
                            }

                            httpResponse = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsLog.addExceptionLog(ex.Message);

                toolStripStatusDisplayLog.Text = String.Format("{0} > {1}", DateTime.Now.ToString(), ex.Message);
            }
            finally
            {

            }

            return bResult;
        }
    }
}
