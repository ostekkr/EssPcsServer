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

        private void runEms()
        {
            DateTime dtNow = DateTime.Now;

            clsEssFrequencySupportCommand freqSupport = m_scenarioCommandSet.EssFrequencySupport;
            clsPvCurtailmentCommand pvCurtailment = m_scenarioCommandSet.PvCurtailment;
            clsPvEssControlCommand pvEssControl = m_scenarioCommandSet.PvEssControl;

            dtNow = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, dtNow.Hour, dtNow.Minute, 0);

            if ((freqSupport != null) && (dtNow >= freqSupport.StartTime) && (dtNow <= freqSupport.EndTime))
            {
                runEms_FrequencySupport(freqSupport);
                
                if (comboBoxOperationMode.SelectedIndex != 3)
                {
                    comboBoxOperationMode.SelectedIndex = 3;
                }
            }
            else if ((pvCurtailment != null) && (dtNow >= pvCurtailment.StartTime) && (dtNow <= pvCurtailment.EndTime))
            {
                runEms_PvCurtailment(pvCurtailment);
                
                if (comboBoxOperationMode.SelectedIndex != 1)
                {
                    comboBoxOperationMode.SelectedIndex = 1;
                }
            }
            else if ((pvEssControl != null) && (dtNow >= pvEssControl.StartTime) && (dtNow <= pvEssControl.EndTime))
            {
                runEms_PvEssControl(pvEssControl);
                
                if (comboBoxOperationMode.SelectedIndex != 2)
                {
                    comboBoxOperationMode.SelectedIndex = 2;
                }
            }
            else
            {
                runEms_General();

                if (comboBoxOperationMode.SelectedIndex != 0)
                {
                    comboBoxOperationMode.SelectedIndex = 0;
                }
            }
        }
    }
}

