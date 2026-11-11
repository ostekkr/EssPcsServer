using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using System.Runtime.Serialization;
using EssPcsService;
using System.Diagnostics;
using System.IO;

namespace EssPcsServer
{
    public partial class frmMain
    {
        private bool Setting_Write(int nIndex)
        {
            bool bResult = false;

            try
            {
                clsEssPcsSettingInfo settingInfo = g_clsSqlServer.m_clsSettingInfo[nIndex];
                String strFile;

                strFile = String.Format(clsConstant.APP_PATH + "setup\\setup_{0}.txt", nIndex + 1);

                using (StreamWriter sw = File.CreateText(strFile))
                {
                    sw.WriteLine(String.Format("DEVICE={0}", settingInfo.strDevice));
                    sw.WriteLine(String.Format("LOC={0}", settingInfo.strLOC));
                    sw.WriteLine(String.Format("USED={0}", settingInfo.bUSED ? 1 : 0));
                    sw.WriteLine(String.Format("COMMENT={0}", settingInfo.strCOMMENT));
                    sw.WriteLine(String.Format("SITE={0}", settingInfo.nTagSite));
                    sw.WriteLine(String.Format("SPOT={0}", settingInfo.nTagSpot));
                    sw.WriteLine(String.Format("TYPE={0}", settingInfo.nTagDeviceType));
                    sw.WriteLine(String.Format("ID={0}", settingInfo.nTagDeviceId));
                }

                strFile = String.Format(clsConstant.APP_PATH + "setup\\setup_{0}.dat", nIndex + 1);

                using (FileStream writeStream = new FileStream(strFile, FileMode.Create))
                {
                    BinaryWriter writeBinay = new BinaryWriter(writeStream);
                    writeBinay.Write(settingInfo.bSetupData);
                    writeBinay.Close();
                }

                bResult = true;
            }
            catch (Exception e)
            {
            }
            finally
            {
            }

            return bResult;
        }

        private void checkGatewayClient()
        {
            Int32 nIndex;
            Int32 i;

            for (nIndex = 0; nIndex < 32; nIndex++ )
            {
                for (i = 0; i < m_clsGateway[nIndex].m_ConnectedClients.Count; i++)
                {
                    if (!m_clsGateway[nIndex].m_ConnectedClients[i].Socket.Connected)
                    {
                        m_clsGateway[nIndex].m_ConnectedClients[i] = null;
                        m_clsGateway[nIndex].m_ConnectedClients.RemoveAt(i);
                        break;
                    }
                }
            }
        }

        private void runDeviceLog()
        {
            try
            {
                if (m_strDisplayLog != "")
                {
                    toolStripStatusDisplayLog.Text = m_strDisplayLog;
                    m_strDisplayLog = "";
                }

                if (clsLog.m_RtuLogList.Count > 0)
                {
                    clsLog.writeRtuLog();
                    clsLog.m_RtuLogList.Clear();
                }

                if (clsLog.m_SqlLogList.Count > 0)
                {
                    clsLog.writeSqlLog();
                    clsLog.m_SqlLogList.Clear();
                }

                if (clsLog.m_ExceptionLogList.Count > 0)
                {
                    clsLog.writeExceptionLog();
                    clsLog.m_ExceptionLogList.Clear();
                }

                if (clsLog.m_SystemLogList.Count > 0)
                {
                    clsLog.writeSystemLog();
                    clsLog.m_SystemLogList.Clear();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void checkDeviceSetting(Int32 nIndex)
        {
            Int32 i;
            Int32 nLen;

            try
            {
                if (g_clsSqlServer.m_clsSettingInfo[nIndex].bUSED)
                {
                    if (m_rtuClient[nIndex] != null)
                    {
                        if (m_rtuClient[nIndex].m_bUpdateSetting)
                        {
                            m_rtuClient[nIndex].m_bUpdateSetting = false;

                            Debug.WriteLine(String.Format("Device Name Changed to " + g_clsSqlServer.m_clsSettingInfo[nIndex].strDevice + "\r\n"));

                            for (i = 0; i < 3; i++)
                            {
                                if (Setting_Write(nIndex))
                                {
                                    break;
                                }
                            }
                        }

                        {
                            clsEssPcsMsg firstMsg = m_rtuClient[nIndex].CommandQueue.PeekFirst();
                            if (firstMsg != null && firstMsg.msgFinish == 1)
                            {
                                processTcpClientEssPcsMsg(firstMsg);
                                firstMsg.msgFinish = 2;
                            }
                        }

                        m_rtuClient[nIndex].CommandQueue.RemoveAll(m => m.msgFinish == 2);
                    }

                    if (m_rtuClient[nIndex].m_nConnectionStatus == clsConstant.PCS_RTU_CONNECTED)
                    {
                        m_rtuClient[nIndex].m_bDaqRun = m_bDaqRun;
                    }
                    else
                    {
                        m_rtuClient[nIndex].m_bDaqRun = false;
                    }
                }
                else
                {
                    m_rtuClient[nIndex].m_bDaqRun = false;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void runDeviceNetwork(Int32 nIndex)
        {
            try
            {
                if (m_rtuClient[nIndex] != null)
                {
                    if (m_rtuClient[nIndex].m_nDisConnectStep > 0)
                    {
                        if (m_rtuClient[nIndex].m_nDisConnectStep == 1)
                        {
                            m_rtuClient[nIndex].RtuDisconnect();
                        }
                        else if (m_rtuClient[nIndex].m_nDisConnectStep == 2)
                        {
                            TimeSpan ts = DateTime.Now - m_rtuClient[nIndex].m_dtDisConnectStep;

                            if (Math.Abs(ts.TotalSeconds) > 5)
                            {
                                m_rtuClient[nIndex].m_nDisConnectStep = 0;
                            }
                        }
                        else if (m_rtuClient[nIndex].m_nDisConnectStep == 3)
                        {
                            if (m_rtuClient[nIndex].m_bDestroyClient)
                            {
                                m_rtuClient[nIndex].stopThread();
                                m_rtuClient[nIndex].m_nDisConnectStep = 4;
                                m_rtuClient[nIndex].m_dtDisConnectStep = DateTime.Now;
                            }
                            else
                            {
                                m_rtuClient[nIndex].m_nDisConnectStep = 0;
                            }
                        }
                        else if (m_rtuClient[nIndex].m_nDisConnectStep == 4)
                        {
                            if (m_rtuClient[nIndex].m_bThreadRun)
                            {
                                TimeSpan ts = DateTime.Now - m_rtuClient[nIndex].m_dtDisConnectStep;

                                if (Math.Abs(ts.TotalSeconds) > 5)
                                {
                                    m_rtuClient[nIndex] = null;
                                    m_rtuClient[nIndex].m_nDisConnectStep = 0;
                                }
                            }
                            else
                            {
                                m_rtuClient[nIndex] = null;
                                m_rtuClient[nIndex].m_nDisConnectStep = 0;
                            }
                        }
                        else if (m_rtuClient[nIndex].m_nDisConnectStep == -1)
                        {
                            //                                m_tcpClient[nIndex].processSocketException();
                            m_rtuClient[nIndex].m_nConnectStep = 3;
                        }
                    }
                    else
                    {
                        if (m_rtuClient[nIndex].m_nConnectStep == 0)
                        {
                            if (m_bServerRun && !m_rtuClient[nIndex].m_bConnectionStatus && m_rtuClient[nIndex].m_bPauseThread)
                            {
                                if (g_clsSqlServer.m_clsSettingInfo[nIndex].bUSED)
                                {
                                    //  m_tcpClient[nIndex].setPause(false);

                                    m_rtuClient[nIndex].RtuConnect();
                                }
                            }
                        }
                        else if (m_rtuClient[nIndex].m_nConnectStep == 1)
                        {
                            m_rtuClient[nIndex].RtuConnect();
                        }
                        else if (m_rtuClient[nIndex].m_nConnectStep == 2)
                        {
                            TimeSpan ts = DateTime.Now - m_rtuClient[nIndex].m_dtConnectStep;

                            if (Math.Abs(ts.TotalSeconds) > 30)
                            {
                                m_rtuClient[nIndex].processRtuException();
                                m_rtuClient[nIndex].m_nConnectStep = 1;
                            }
                        }
                        else if (m_rtuClient[nIndex].m_nConnectStep == 3)
                        {
                            m_rtuClient[nIndex].runFirstStep();
                            m_rtuClient[nIndex].m_nConnectStep = 0;
                        }
                        else if (m_rtuClient[nIndex].m_nConnectStep == -1)
                        {
                            m_rtuClient[nIndex].processRtuException();
                            m_rtuClient[nIndex].m_nConnectStep = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void runGatewayStatus()
        {
            try
            {
                Int32 i;

                for (i = 0; i < 32; i++ )
                {
                    m_clsGateway[i].checkConnections();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void timerMainLog()
        {
            Int32 nIndex;

            if (m_bDaqRun)
            {
                if (m_JsonList.Count > 0)
                {
                    runSQLite();
                }
            }

            runDeviceLog();

            if (m_rtuClient.Count > 0)
            {
                for (nIndex = 0; nIndex < m_nDeviceNum; nIndex++)
                {
                    checkDeviceSetting(nIndex);
                    runDeviceNetwork(nIndex);
                }
            }

            runGatewayStatus();

            bool bServerRun = m_bServerRun;
            EnqueueWcf(() =>
            {
                try
                {
                    if (m_EssPcsServiceProxy == null) return;

                    m_EssPcsServiceProxy.SetServerStatus(bServerRun);

                    clsProgramInfo clsInfo = m_EssPcsServiceProxy.GetProgramInfo();

                    if (clsInfo.bReqShow)
                    {
                        clsInfo.bReqShow = false;
                        this.BeginInvoke(new Action(runShow));
                    }

                    if (clsInfo.bReqHide)
                    {
                        clsInfo.bReqHide = false;
                        this.BeginInvoke(new Action(runHide));
                    }

                    m_EssPcsServiceProxy.SetProgramInfo(clsInfo);
                }
                catch { }
            });
        }

        private void runShow()
        {
            if (!this.Visible)
            {
                this.Visible = true;
                Tray.Visible = false;
                this.WindowState = FormWindowState.Maximized;
                this.ShowInTaskbar = true;
            }
        }

        private void runHide()
        {
            if (this.Visible)
            {
                this.Visible = false;
                Tray.Visible = true;
                this.ShowInTaskbar = false;
            }
        }

        /// <summary>
        /// EMS 1초 주기 실행.
        /// - PV 출력제한은 ESS 운영 선택과 따로 적용한다(PV 만 다루므로).
        /// - ESS 는 부하감축 > 주파수 지원 > PV-ESS 연계 > 일반운영 순으로 하나만 실행한다.
        ///   버전 2(구간 지령) 시나리오는 runEms_EssSegments, 버전 1 은 기존 함수를 그대로 쓴다.
        /// </summary>
        private void runEms()
        {
            DateTime dtNow = clsClock.NowMinute();

            float?[] pvLimits = clsEmsPlanner.PvLimits(m_scenarioCommandSet.PvCurtailment, dtNow);
            applyPvLimits(pvLimits);

            clsScenarioCommandBase selected;
            enEmsMode mode = clsEmsPlanner.SelectEssMode(m_scenarioCommandSet, dtNow, out selected);

            switch (mode)
            {
                case enEmsMode.LoadReduction:
                    runEms_EssSegments(selected, dtNow);
                    break;

                case enEmsMode.FrequencySupport:
                    if (selected.IsVersion2) runEms_EssSegments(selected, dtNow);
                    else runEms_FrequencySupport((clsEssFrequencySupportCommand)selected);
                    break;

                case enEmsMode.PvEssControl:
                    if (selected.IsVersion2) runEms_EssSegments(selected, dtNow);
                    else runEms_PvEssControl((clsPvEssControlCommand)selected);
                    break;

                default:
                    runEms_General();
                    break;
            }

            //  운영모드 콤보 표시: ESS 운영 종류. ESS 가 일반운영이고 PV 제한 중이면 PVCurtailment 로 표시한다.
            int nIndex = (int)mode;

            if (mode == enEmsMode.General && Array.Exists(pvLimits, x => x.HasValue))
            {
                nIndex = (int)enEmsMode.PvCurtailment;
            }

            if (nIndex < comboBoxOperationMode.Items.Count && comboBoxOperationMode.SelectedIndex != nIndex)
            {
                comboBoxOperationMode.SelectedIndex = nIndex;
            }
        }
    }
}

