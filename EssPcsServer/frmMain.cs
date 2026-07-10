using C1.Win.Command;
using EssBmsService;
using EssPcsService;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using System.Text;
using System.Windows.Forms;

namespace EssPcsServer
{
    public partial class frmMain : C1.Win.Ribbon.C1RibbonForm
    {
        public clsSqlServer g_clsSqlServer;
        public int m_nDeviceNormal = 0;
        public int m_nDeviceFault = 0;
        public bool m_bLogin = false;
        string m_strDisplayLog = "";
        String m_strLogError = "";
        bool m_bRun = false;
        public List<clsRtuClient> m_rtuClient = new List<clsRtuClient>();
        clsUdpClient m_UdpClient = new clsUdpClient();

        int m_nSelectedMeter = 0;
        DateTime m_DeviceSyncTime = DateTime.Now;
        DateTime m_ManualTriggerTime = DateTime.Now;
        ServiceHost m_ServiceHost = null;

        bool m_bServerRun = false;
        frmWaitThread m_frmWait;
        public bool m_bDaqRun = false;
        public bool m_bIsEmsRun = false;

        /*
        StreamWriter m_swSystemEvent;
        StreamWriter m_swPqEvent;
        StreamWriter m_swLoadProfile;
        StreamWriter m_swWaveform;
        StreamWriter m_swTrend;
        */

        System.Threading.Thread m_threadRun = null;

        const int m_nDeviceMax = clsConstant.ESSPCS_MAX_NUM;
#if DEBUG_RT
        public int m_nDeviceNum = 0;
#else
        public int m_nDeviceNum = m_nDeviceMax;
#endif

        public Stopwatch m_swEms = new Stopwatch();

        private readonly System.Collections.Concurrent.ConcurrentQueue<Action> _wcfActionQueue =
            new System.Collections.Concurrent.ConcurrentQueue<Action>();
        private readonly object _jsonListLock = new object();

        private void EnqueueWcf(Action action)
        {
            if (action != null) _wcfActionQueue.Enqueue(action);
        }

        clsGatewayServer[] m_clsGateway = new clsGatewayServer[32];

        void processStatusEventList()
        {
            String strMsg;
            int i;

            for (i = 0; i < m_rtuClient.Count; i++)
            {
                try
                {
                    if (m_rtuClient[i] != null)
                    {
                        if (m_rtuClient[i].m_bConnectionStatus)
                        {
                            m_rtuClient[i].m_bConnectionStatus = false;
                            processStatusEventInfo(i, m_rtuClient[i].m_nConnectionStatus);
                        }
                    }
                }
                catch (Exception ex)
                {

                }
            }

            for (i = 0; i < m_rtuClient.Count; i++)
            {
                try
                {
                    if (m_rtuClient[i] != null)
                    {
                        if (m_rtuClient[i].m_statusEventList.Count > 0)
                        {
                            clsStatusEventInfo evt = m_rtuClient[i].m_statusEventList[0];

                            processStatusEventInfo(evt.nDevice, evt.nEvent);
                            m_rtuClient[i].m_statusEventList.RemoveAt(0);
                        }

                        if (m_rtuClient[i].m_statusList.Count > 0)
                        {
                            foreach (clsStatus statusInfo in m_rtuClient[i].m_statusList)
                            {
                                displayTcpClientLog(i, statusInfo.m_dtMsg, statusInfo.m_strMsg);

                                if (statusInfo.m_bMsgLog)
                                {
                                    strMsg = String.Format(" {0} > {1}", g_clsSqlServer.m_clsSettingInfo[i].strDevice, statusInfo.m_strMsg);
                                    m_SystemLogList.Add(new clsSystemLog(i, strMsg, statusInfo.m_dtMsg));
                                    statusInfo.m_bMsgLog = false;
                                }
                            }

                            m_rtuClient[i].m_statusList.Clear();
                        }

                        if (m_rtuClient[i].m_nMsgImageIndex != -1)
                        {
                            c1TopicBarDevice.Pages[i].ImageIndex = m_rtuClient[i].m_nMsgImageIndex;
                            m_rtuClient[i].m_nMsgImageIndex = -1;
                        }
                    }
                }
                catch (Exception ex)
                {
                    clsLog.addExceptionLog(ex.Message);
                }
            }
        }

        void processEssPcsMsgList()
        {
            if (m_EssPcsMsgList.Count > 0)
            {
                clsEssPcsMsg msg;// = new clsEssPcsMsg(m_EssPcsMsgList[0]);

                msg = m_EssPcsMsgList[0];

                processEssPcsMsg(msg);

                m_EssPcsMsgList.RemoveAt(0);
            }
        }

        void processSystemLogList()
        {
            while (m_SystemLogList.Count > 0)
            {
                try
                {
                    if (g_clsSqlServer.SysLog_Add(m_SystemLogList[0]))
                    {
                        var logItem = m_SystemLogList[0];
                        EnqueueWcf(() =>
                        {
                            if (m_EssPcsServiceProxy != null)
                                m_EssPcsServiceProxy.SetSysLog(logItem.m_dtLog, logItem.m_strLog);
                        });

                        if ((logItem.m_nDevice >= 0) && (logItem.m_nDevice <= 31))
                        {
                            displayTopicLink(logItem.m_nDevice, logItem.m_dtLog, logItem.m_strLog);
                        }

                        m_SystemLogList.RemoveAt(0);
                    }
                    else
                    {
                        displayLog(g_clsSqlServer.g_strSysLogError);
                        m_SystemLogList.Clear();
                    }
                }
                catch (Exception ex)
                {
                    m_strLogError = "processSystemLogList : " + ex.Message;
                    clsLog.addSqlLog(m_strLogError);
                }
                finally
                {
                }
            }
        }

        void processWaveInfoList()
        {
            if (m_WaveInfoList.Count > 0)
            {
                try
                {
                    if (g_clsSqlServer.g_strWaveInfoLogError != "")
                    {
                        clsLog.addSqlLog(g_clsSqlServer.g_strWaveInfoLogError);
                        g_clsSqlServer.g_strWaveInfoLogError = "";
                    }

                    if (g_clsSqlServer.WaveInfo_Add(m_WaveInfoList[0]))
                    {
                        int nIndex = m_WaveInfoList[0].m_nIndex;

                        if (m_rtuClient[nIndex] != null)
                        {
                            if (m_WaveInfoList[0].m_nFileType == 0)
                            {
                                m_rtuClient[nIndex].m_statusList.Add(new clsStatus("순시 파형 정보 저장", false, m_WaveInfoList[0].m_dtEvent));
                            }
                            else
                            {
                                m_rtuClient[nIndex].m_statusList.Add(new clsStatus("페이저 파형 정보 저장", false, m_WaveInfoList[0].m_dtEvent));
                            }
                        }

                        m_WaveInfoList.RemoveAt(0);
                    }
                    else
                    {
                        clsLog.addSqlLog(g_clsSqlServer.g_strWaveInfoLogError);
                        m_WaveInfoList.Clear();
                    }
                }
                catch (Exception ex)
                {
                    m_strLogError = "processWaveInfoList : " + ex.Message;
                    clsLog.addSqlLog(m_strLogError);
                }
                finally
                {
                }
            }
        }

        void processTrendList()
        {
            int i, j;

            for (i = 0; i < m_rtuClient.Count; i++)
            {
                if (m_rtuClient[i] != null)
                {
                    while (m_rtuClient[i].m_trendInfoList.Count > 0)
                    {
                        String strJson = makeJsonString(i, m_rtuClient[i].m_trendInfoList[0]);
                        m_rtuClient[i].m_trendInfoList.RemoveAt(0);

                        if (strJson != null)
                        {
                            string url1 = txtDbServerSite1.Text;
                            int port1 = Int32.Parse(numericDbServerPort1.Text);
                            string url2 = txtDbServerSite2.Text;
                            int port2 = Int32.Parse(numericDbServerPort2.Text);
                            int idx = i;
                            string jsonCopy = strJson;

                            System.Threading.Tasks.Task.Run(() =>
                            {
                                if (!sendJsonString(idx, url1, port1, jsonCopy) ||
                                    !sendJsonString(idx, url2, port2, jsonCopy))
                                {
                                    lock (_jsonListLock)
                                    {
                                        m_JsonList.Add(jsonCopy);
                                        if (m_JsonList.Count > 10000)
                                            m_JsonList.RemoveAt(0);
                                    }
                                }
                            });
                        }
                    }
                }
            }
        }

        int checkListCount()
        {
            int nCount = 0;

            nCount += m_EssPcsMsgList.Count;

            return nCount;
        }

        void processEmptyList()
        {
            while (m_EssPcsMsgList.Count > 0)
            {
                processEssPcsMsgList();
            }

            while (m_WaveInfoList.Count > 0)
            {
                processWaveInfoList();
            }
        }

        void endProgram()
        {
            Int32 i;

            m_SystemLogList.Add(new clsSystemLog(-1, "ESS-PCS 데이터 서버 프로그램 종료", DateTime.Now));

            processEmptyList();

            g_clsSqlServer.disconnectSqlServer();

            Properties.Settings.Default.Save();

            /*
            m_swSystemEvent.Close();
            m_swPqEvent.Close();
            m_swLoadProfile.Close();
            m_swWaveform.Close();
            m_swTrend.Close();
            */

            if (m_UdpClient != null)
            {
                m_UdpClient.UdpDisconnect();
            }

            for (i = 0; i < 32; i++ )
            {
                if (m_clsGateway[i] != null)
                {
                    m_clsGateway[i].DisconnectAll();
                    m_clsGateway[i].StopListening();
                }
            }

            if (m_EssPcsServiceProxy != null)
            {
                ((IClientChannel)m_EssPcsServiceProxy).Close();
            }

            if (m_ServiceHost != null)
            {
                m_ServiceHost = null;
//                m_ServiceHost.Close();
            }

            /*
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => destroyFrmWait()));
            }
            else
            {
                destroyFrmWait();
            }
            */
            //  if (m_frmWait.Visible)
            //  {
            //      m_frmWait.stopWait();             
            //  }
        }

        void destroyFrmWait()
        {
            if (m_frmWait != null)
            {
                m_frmWait.Dispose();
                m_frmWait = null;
            }
        }

        void uiThreadRunFunc()
        {
            clsRtuClient tcpClient;
            int nClient = 0;
            int nStep = 0;

            m_bRun = true;

            while (m_bRun)
            {
                System.Threading.Thread.Sleep(1);

                // WCF 큐 드레인 - 모든 WCF 호출을 이 스레드에서 직렬 처리
                Action wcfAction;
                while (_wcfActionQueue.TryDequeue(out wcfAction))
                {
                    try { wcfAction(); }
                    catch (Exception ex)
                    {
                        m_strLogError = "WcfAction: " + ex.Message;
                    }
                }

                try
                {

                    if (m_frmWait != null)
                    {
                        if (m_frmWait.Visible)
                        {
                            for (nClient = 0; nClient < m_rtuClient.Count; nClient++)
                            {
                                if (m_rtuClient[nClient] != null)
                                {
                                    if ((m_rtuClient[nClient].m_TrendInfo.nConnectionStatus != clsConstant.PCS_RTU_SHUTDOWN) && (m_rtuClient[nClient].m_TrendInfo.nConnectionStatus == clsConstant.PCS_RTU_DISCONNECTED))
                                    {
                                        break;
                                    }
                                }
                            }

                            if (nClient == m_rtuClient.Count)
                            {
                                if (checkListCount() == 0)
                                {
                                    m_frmWait.stopWait();
                                }
                            }

                            nClient = 0;

                        }
                        else
                        {
                            if (this.InvokeRequired)
                            {
                                this.BeginInvoke(new Action(() => destroyFrmWait()));
                            }
                            else
                            {
                                destroyFrmWait();
                            }
                        }
                    }
                    else
                    {
                        if (nClient >= m_rtuClient.Count)
                        {
                            nClient = 0;
                        }

                        tcpClient = m_rtuClient[nClient];

                        if (g_clsSqlServer.m_clsSettingInfo[nClient].bUSED && (tcpClient != null) && (m_EssPcsServiceProxy != null))
                        {
                            int devNo = g_clsSqlServer.m_clsSettingInfo[nClient].nDeviceNo;
                            int errCount = (int)tcpClient.m_nComErrorCount;
                            EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.SetErrorCount(devNo, errCount); });
                        }

                        nClient++;
                    }
                }
                catch (Exception ex)
                {
                    m_strLogError = "uiThreadRunFunc : " + ex.Message;
                    m_strLogError = String.Format("uiThreadRunFunc : {0} > {1}\n{2}", ex.Message, nStep, ex.StackTrace.ToString());
                }
            }

            endProgram();
        }

        void openSocket(int nIndex)
        {
            //  closeSocket(nIndex);

            //  m_tcpClient[nIndex] = new clsTcpClient(nIndex, g_clsSqlServer.m_clsSettingInfo[nIndex], g_clsSqlServer.m_clsTrendInfo[nIndex], false);
            //  m_tcpClient[nIndex].tcpClientDisplaySetting = new tcpClientDisplaySettingDelegate(displayTcpClientSetting);
            //  m_tcpClient[nIndex].tcpClientStatusWaveForm = new tcpClientStatusWaveFormDelegate(statusTcpClientWaveForm);
        }

        void closeSocket(int nIndex, bool bClose = true)
        {
            try
            {
                if (m_rtuClient[nIndex] != null)
                {
                    m_rtuClient[nIndex].stopThread();

                    m_rtuClient[nIndex].m_nDisConnectStep = 1;
                    m_rtuClient[nIndex].m_bDestroyClient = true;
                }
            }
            catch (Exception ex)
            {

            }
        }

        void pauseSocket(int nIndex, bool bPause)
        {
            clsRtuClient tcpClient = m_rtuClient[nIndex];

            if (m_rtuClient[nIndex] != null)
            {
                m_rtuClient[nIndex].setPause(bPause);

                /*
                if (bPause)
                {
                    m_tcpClient[nIndex].m_nStep = 1;
                    m_tcpClient[nIndex].m_bDaqRun = true;
                    m_tcpClient[nIndex].m_bStepResult = true;

                    m_tcpClient[nIndex].Receive();
                }
                * */
            }
        }

        public frmMain()
        {
            InitializeComponent();

            frmService frm = new frmService();

            frm.ShowDialog();

            g_clsSqlServer = new clsSqlServer();

            try
            {
                m_ServiceHost = new ServiceHost(typeof(EssPcsWcfService));

                m_ServiceHost.Open();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "서비스 열기 오류");
                Application.ExitThread();
                Environment.Exit(0);
                return;
            }

            openEssPcsWcf();
            openEssBmsWcf();
            /*
            m_swSystemEvent = new StreamWriter(@"systemevent.txt", true);
            m_swPqEvent = new StreamWriter(@"pqevnet.txt", true);
            m_swLoadProfile = new StreamWriter(@"loadprofile.txt", true);
            m_swWaveform = new StreamWriter(@"waveform.txt", true);
            m_swTrend = new StreamWriter(@"trend.txt", true);
            */
        }

        private void runClose()
        {
            try
            {
                int i;

                m_EssPcsServiceProxy = null;

                timerMain.Enabled = false;
                timerEssPcs.Enabled = false;

                for (i = 0; i < m_rtuClient.Count; i++)
                {
                    m_rtuClient[i].pauseThread();
                }
               
                for (i = 0; i < m_rtuClient.Count; i++)
                {
                    m_rtuClient[i].stopThread();
                }

                for (i = 0; i < m_rtuClient.Count; i++ )
                {
                    m_rtuClient[i] = null;
                }

                m_rtuClient.Clear();

                m_bRun = false;
            }
            catch (Exception ex)
            {

            }
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_bServerRun)
            {
                MessageBox.Show("장치와 접속 중입니다.\n접속 해제를 먼저 수행하세요.", "ESS-PCS Data Server");
                e.Cancel = true;
            }
            else 
            {
                if (MessageBox.Show("프로그램을 종료하시겠습니까?", "ESS-PCS Data Server", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    e.Cancel = true;
                }
                else
                {
                    runClose();
                }

                Properties.Settings.Default.sitePort1 = UInt32.Parse(numericDbServerPort1.Text);
                Properties.Settings.Default.siteUrl1 = txtDbServerSite1.Text;

                Properties.Settings.Default.sitePort2 = UInt32.Parse(numericDbServerPort2.Text);
                Properties.Settings.Default.siteUrl2 = txtDbServerSite2.Text;


                Properties.Settings.Default.pcsOutputP = Single.Parse(numInvSetP.Text);
                Properties.Settings.Default.pcsOutputQ = Single.Parse(numInvSetQ.Text);

                Properties.Settings.Default.essChargeStartHour = Int32.Parse(numEssChargeStartTimeHour.Text);
                Properties.Settings.Default.essChargeStartMin = Int32.Parse(numEssChargeStartTimeMin.Text);
                Properties.Settings.Default.essChargeEndHour = Int32.Parse(numEssChargeEndTimeHour.Text);
                Properties.Settings.Default.essChargeEndMin = Int32.Parse(numEssChargeEndTimeMin.Text);
                Properties.Settings.Default.essChargeSocLimit = Single.Parse(numEssChargeSocLimit.Text);
                Properties.Settings.Default.essChargeOutputLimit = Single.Parse(numEssChargeOutputLimit.Text);

                Properties.Settings.Default.essDischargeStartHour = Int32.Parse(numEssDischargeStartTimeHour.Text);
                Properties.Settings.Default.essDischargeStartMin = Int32.Parse(numEssDischargeStartTimeMin.Text);
                Properties.Settings.Default.essDischargeEndHour = Int32.Parse(numEssDischargeEndTimeHour.Text);
                Properties.Settings.Default.essDischargeEndMin = Int32.Parse(numEssDischargeEndTimeMin.Text);
                Properties.Settings.Default.essDischargeSocLimit = Single.Parse(numEssDischargeSocLimit.Text);
                Properties.Settings.Default.essDischargeOutputLimit = Single.Parse(numEssDischargeOutputLimit.Text);

                Properties.Settings.Default.essGenUse = chkEssGen.Checked;
                Properties.Settings.Default.essGenOutPower = Single.Parse(numEssGenOutPower.Text);
                Properties.Settings.Default.essGenOutPercent = Single.Parse(numEssGenOutPercent.Text);

                Properties.Settings.Default.essControlPeriod = Int32.Parse(numEssControlPeriod.Text);

                Properties.Settings.Default.rtuIp1 = txtRtuIpAddress1.Text;
                Properties.Settings.Default.rtuIp2 = txtRtuIpAddress2.Text;
                Properties.Settings.Default.rtuIp3 = txtRtuIpAddress3.Text;

                Properties.Settings.Default.pcsId1 = Int32.Parse(txtPcsId1.Text);
                Properties.Settings.Default.pcsId2 = Int32.Parse(txtPcsId2.Text);
                Properties.Settings.Default.pcsId3 = Int32.Parse(txtPcsId3.Text);
                Properties.Settings.Default.pcsOut1 = (Int32)numPcsOut1.Value;
                Properties.Settings.Default.pcsOut2 = (Int32)numPcsOut2.Value;
                Properties.Settings.Default.pcsOut3 = (Int32)numPcsOut3.Value;

                Properties.Settings.Default.pcsPeriod1 = (Int32)numPcsPeriod1.Value;
                Properties.Settings.Default.pcsPeriod2 = (Int32)numPcsPeriod2.Value;
                Properties.Settings.Default.pcsPeriod3 = (Int32)numPcsPeriod3.Value;

                Properties.Settings.Default.Save();

                clsLog.addRtuLog("STOP");
                clsLog.closeRtuLog();
                clsLog.addSqlLog("STOP");
                clsLog.closeSqlLog();
                clsLog.addExceptionLog("STOP");
                clsLog.closeExceptionLog();
                clsLog.addSystemLog("STOP");
                clsLog.closeSystemLog();
            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            Byte[] bIpAddress = { 0, 0, 0, 0 };
            Byte[] bSubnetMask = { 255, 255, 255, 0 };
            Byte[] bGatewayIp = { 192, 168, 1, 1 };
            int i, j;
            string strLog;

            clsLog.openRtuLog();
            clsLog.addRtuLog("START");
            clsLog.openSqlLog();
            clsLog.addSqlLog("START");
            clsLog.openExceptiontLog();
            clsLog.addExceptionLog("START");
            //  clsLog.openSystemLog();
            clsLog.addSystemLog("START");

            //  toolStripStatusDbUser.Text = String.Format("[{0}]", Properties.Settings.Default.sqlServerUser);
            //  toolStripStatusDbIp.Text = String.Format("[{0}]", Properties.Settings.Default.sqlServerIp);
            menuStripMainSettingTray.Checked = Properties.Settings.Default.trayMove;
            chkHmiMode.Checked = Properties.Settings.Default.hmiMode;

            numericDbServerPort1.Value = Properties.Settings.Default.sitePort1;
            txtDbServerSite1.Text = Properties.Settings.Default.siteUrl1;
            numericDbServerPort2.Value = Properties.Settings.Default.sitePort2;
            txtDbServerSite2.Text = Properties.Settings.Default.siteUrl2;

            numInvSetP.Text = String.Format("{0:F1}", Properties.Settings.Default.pcsOutputP);
            numInvSetQ.Text = String.Format("{0:F1}", Properties.Settings.Default.pcsOutputQ);

            numEssChargeStartTimeHour.Value = Properties.Settings.Default.essChargeStartHour;
            numEssChargeStartTimeMin.Value = Properties.Settings.Default.essChargeStartMin;
            numEssChargeEndTimeHour.Value = Properties.Settings.Default.essChargeEndHour;
            numEssChargeEndTimeMin.Value = Properties.Settings.Default.essChargeEndMin;
            numEssChargeSocLimit.Text = String.Format("{0:F1}", Properties.Settings.Default.essChargeSocLimit);
            numEssChargeOutputLimit.Text = String.Format("{0:F1}", Properties.Settings.Default.essChargeOutputLimit);
            
            numEssDischargeStartTimeHour.Value = Properties.Settings.Default.essDischargeStartHour;
            numEssDischargeStartTimeMin.Value = Properties.Settings.Default.essDischargeStartMin;
            numEssDischargeEndTimeHour.Value = Properties.Settings.Default.essDischargeEndHour;
            numEssDischargeEndTimeMin.Value = Properties.Settings.Default.essDischargeEndMin;
            numEssDischargeSocLimit.Text = String.Format("{0:F1}", Properties.Settings.Default.essDischargeSocLimit);
            numEssDischargeOutputLimit.Text = String.Format("{0:F1}", Properties.Settings.Default.essDischargeOutputLimit);

            chkEssGen.Checked = Properties.Settings.Default.essGenUse;
            numEssGenOutPower.Text = String.Format("{0:F1}", Properties.Settings.Default.essGenOutPower);
            numEssGenOutPercent.Text = String.Format("{0:F1}", Properties.Settings.Default.essGenOutPercent);

            numEssControlPeriod.Text = String.Format("{0}", Properties.Settings.Default.essControlPeriod);

            txtRtuIpAddress1.Text = Properties.Settings.Default.rtuIp1;
            txtRtuIpAddress2.Text = Properties.Settings.Default.rtuIp2;
            txtRtuIpAddress3.Text = Properties.Settings.Default.rtuIp3;

            txtPcsId1.Text = Properties.Settings.Default.pcsId1.ToString();
            txtPcsId2.Text = Properties.Settings.Default.pcsId2.ToString();
            txtPcsId3.Text = Properties.Settings.Default.pcsId3.ToString();

            numPcsOut1.Value = (decimal)Properties.Settings.Default.pcsOut1;
            numPcsOut2.Value = (decimal)Properties.Settings.Default.pcsOut2;
            numPcsOut3.Value = (decimal)Properties.Settings.Default.pcsOut3;

            numPcsPeriod1.Value = (decimal)Properties.Settings.Default.pcsPeriod1;
            numPcsPeriod2.Value = (decimal)Properties.Settings.Default.pcsPeriod2;
            numPcsPeriod3.Value = (decimal)Properties.Settings.Default.pcsPeriod3;

            comboBoxOperationMode.SelectedIndex = 0;

            setEssControlInfo();

            g_clsSqlServer.DisplayLog += displayLog;

            m_SystemLogList.Add(new clsSystemLog(-1, "ESS-PCS 데이터 서버 프로그램 시작", DateTime.Now));

            strLog = g_clsSqlServer.Setting_Make();

            if (strLog != "")
            {
                m_SystemLogList.Add(new clsSystemLog(-1, strLog, DateTime.Now));
                MessageBox.Show(strLog);
            }

            g_clsSqlServer.WaveInfo_Make();
            g_clsSqlServer.WaveForm_Make();
            g_clsSqlServer.SysLog_Make();
            g_clsSqlServer.Trend_Make();
            //<--|
            /*
            for (i = 0; i < m_tcpClient.Count; i++)
            {
                if (m_tcpClient[i].m_socketClient.Connected)
                {
                    m_tcpClient[i].m_socketClient.Disconnect(false);
                }
            }
            */
            //|-->

            m_rtuClient.Clear();

            for (i = 0; i < g_clsSqlServer.m_clsSettingInfo.Count; i++)
            {
                clsRtuClient tcpClient = new clsRtuClient(i, g_clsSqlServer.m_clsSettingInfo[i], g_clsSqlServer.m_clsTrendInfo[i], false);

                m_rtuClient.Add(tcpClient);
            }

            for (i = 0; i < g_clsSqlServer.m_clsSettingInfo.Count; i++)
            {
                openSocket(i);
            }

            while (true)
            {
                try
                {
                    m_EssPcsServiceProxy.StartService(true);
                    break;
                }
                catch (Exception exception)
                {
                    //MessageBox.Show(exception.Message, "서비스 시작 오류");
                    //Application.ExitThread();
                    //Environment.Exit(0);
                    //return;
                }
            }

            m_EssPcsServiceProxy.ClearSetting();

            System.Diagnostics.Debug.WriteLine("Server Session ID [" + m_EssPcsServiceProxy.SessionId() + "]");

            for (i = 0; i < g_clsSqlServer.m_clsSettingInfo.Count; i++)
            {
                m_EssPcsServiceProxy.AddSetting(g_clsSqlServer.m_clsSettingInfo[i], g_clsSqlServer.m_clsTrendInfo[i]);
            }

            displayMeterList(false);

            m_threadRun = new System.Threading.Thread(new System.Threading.ThreadStart(uiThreadRunFunc));
            m_threadRun.Start();

            for (i = 0; i < 32; i++)
            {
                m_clsGateway[i] = new clsGatewayServer(8001 + i, System.Net.IPAddress.Any, 0, ref g_clsSqlServer, ref m_EssPcsServiceProxy);
                m_clsGateway[i].ClientConnected += new clsGatewayServer.ClientConnectedDelegate(serverClientConnected);
                m_clsGateway[i].StartListening();
            }

            timerMain.Enabled = true;

            if (Properties.Settings.Default.hmiMode)
            {
                m_bServerRun = true;
                runConnect();
                runStart();
                moveTray();
            }
            else
            {
                this.ShowInTaskbar = true;
                this.WindowState = FormWindowState.Maximized;//.Normal;
            }

            timerEssPcs.Enabled = true;
            m_swEms.Start();
        }

        protected void serverClientConnected(object sender, clsGatewayClient Client)
        {
            System.Net.Sockets.Socket s = Client.Socket.Client;
            clsGatewayServer serverObj = (clsGatewayServer)sender;
            string strLog;

            strLog = string.Format("Connected to {0} : {1} {2}",
                System.Net.IPAddress.Parse(((System.Net.IPEndPoint)(s.RemoteEndPoint)).Address.ToString()),
                ((System.Net.IPEndPoint)(s.RemoteEndPoint)).Port,
                serverObj.m_ConnectedClients.Count);

            //  displayLog(strLog);
            //  MessageBox.Show(strLog);
        }


        public void displayLog(string log)
        {
            DateTime t = DateTime.Now;
            displayLog(t, log);
        }

        public void displayLog(DateTime t, string log)
        {
            if (!toolStripStatusDisplayLog.IsDisposed)
            {
                m_strDisplayLog = string.Format("{0} : {1}", t, log);
            }
        }

        public void displayTcpClientSetting(clsEssPcsMsg msg)
        {
            /*
            m_ServiceProxy.SendToClient(msg.msgSession, msg);
            if (m_tcpClient[nDevice].m_bReceiveSetting)
            {
                m_tcpClient[nDevice].m_bReceiveSetting = false;
                m_bSettingUpdate = true;
            }
            */
        }

        /*
        private void checkEssPcsClient(int nDevice)
        {
            if (!m_tcpClient[nDevice].m_bReqStopSocket)
            {
                String strIp;

                strIp = String.Format("{0}.{1}.{2}.{3}",
                    g_clsSqlServer.m_clsSettingInfo[nDevice].bIpAddress[0],
                    g_clsSqlServer.m_clsSettingInfo[nDevice].bIpAddress[1],
                    g_clsSqlServer.m_clsSettingInfo[nDevice].bIpAddress[2],
                    g_clsSqlServer.m_clsSettingInfo[nDevice].bIpAddress[3]);

                m_tcpClient[nDevice].TcpConnect(strIp, clsConstant.ETHERNET_PORT);
            }
        }
        */

        private void processStatusEventInfo(int nDevice, int nEvent)
        {
            if (m_rtuClient[nDevice] != null)
            {
                if (m_rtuClient[nDevice].m_TrendInfo.nConnectionStatus != clsConstant.PCS_RTU_SHUTDOWN)
                {
                    if (nEvent == clsConstant.PCS_TREND_DATA)
                    {
                        var trendInfo = m_rtuClient[nDevice].m_TrendInfo;
                        EnqueueWcf(() =>
                        {
                            if (m_EssPcsServiceProxy != null)
                                m_EssPcsServiceProxy.SetTrendInfo(trendInfo);
                        });

                        if ((m_rtuClient[nDevice].m_bWaitInsFile == false) && m_rtuClient[nDevice].m_bDaqRun)
                        {
                            /*
                            if (m_rtuClient[nDevice].m_TrendInfo.nInsIndex != m_rtuClient[nDevice].m_TrendInfo.nInsNum)
                            {
                                int nIndex = m_rtuClient[nDevice].m_TrendInfo.nInsIndex;

                                if ((nIndex >= 0) && (nIndex < clsConstant.CDFR_INS_REC_NUM))
                                {
                                    if (m_tcpClient[nDevice].m_EssPcsMsgList.Count == 0)
                                    {
                                        clsEssPcsMsg msg = new clsEssPcsMsg();

                                        msg.msgDevice = g_clsSqlServer.m_clsSettingInfo[nDevice].nDeviceNo;
                                        msg.msgType = clsConstant.CDFR_CMD_INS_INFO;
                                        msg.msgIndex = m_tcpClient[nDevice].m_TrendInfo.nInsIndex + 1;
                                        msg.msgDateTime = DateTime.Now;
                                        msg.msgSession = "";
                                        msg.msgResult = 0;
                                        msg.msgDeviceName = g_clsSqlServer.m_clsSettingInfo[nDevice].strDevice;

                                        m_tcpClient[nDevice].m_EssPcsMsgList.Add(msg);

                                        m_tcpClient[nDevice].m_bWaitInsFile = true;

                                        return;
                                    }
                                }
                            }
                            */
                        }

                        /*
                        if ((m_tcpClient[nDevice].m_bWaitPhaFile == false) && m_tcpClient[nDevice].m_bDaqRun)
                        {
                            if (m_tcpClient[nDevice].m_TrendInfo.nPhaIndex != m_tcpClient[nDevice].m_TrendInfo.nPhaNum)
                            {
                                int nIndex = m_tcpClient[nDevice].m_TrendInfo.nPhaIndex;

                                if ((nIndex >= 0) && (nIndex < clsConstant.CDFR_PHA_REC_NUM))
                                {
                                    if (m_tcpClient[nDevice].m_EssPcsMsgList.Count == 0)
                                    {
                                        clsEssPcsMsg msg = new clsEssPcsMsg();

                                        msg.msgDevice = g_clsSqlServer.m_clsSettingInfo[nDevice].nDeviceNo;
                                        msg.msgType = clsConstant.CDFR_CMD_PHA_INFO;
                                        msg.msgIndex = m_tcpClient[nDevice].m_TrendInfo.nPhaIndex + 1;
                                        msg.msgDateTime = DateTime.Now;
                                        msg.msgSession = "";
                                        msg.msgResult = 0;
                                        msg.msgDeviceName = g_clsSqlServer.m_clsSettingInfo[nDevice].strDevice;

                                        m_tcpClient[nDevice].m_EssPcsMsgList.Add(msg);

                                        m_tcpClient[nDevice].m_bWaitPhaFile = true;

                                        return;
                                    }
                                }
                            }
                        }
                        */

                        return;
                    }
                }
            }

            if (nEvent == clsConstant.PCS_RTU_CONNECTED)
            {
                updateMeterListImage(nDevice, clsConstant.PCS_RTU_CONNECTED);
                m_rtuClient[nDevice].m_TrendInfo.nConnectionStatus = clsConstant.PCS_RTU_CONNECTED;
                m_EssPcsServiceProxy.SetTrendInfo(m_rtuClient[nDevice].m_TrendInfo);
                return;
            }
            else if (nEvent == clsConstant.PCS_RTU_DISCONNECTED)
            {
                updateMeterListImage(nDevice, clsConstant.PCS_RTU_DISCONNECTED);
                m_rtuClient[nDevice].m_TrendInfo.nConnectionStatus = clsConstant.PCS_RTU_DISCONNECTED;
                m_EssPcsServiceProxy.SetTrendInfo(m_rtuClient[nDevice].m_TrendInfo);
                
                if ((m_rtuClient[nDevice] != null) && (m_rtuClient[nDevice].m_TrendInfo.nConnectionStatus != clsConstant.PCS_RTU_SHUTDOWN) && m_bServerRun)
                {
                    if (m_rtuClient[nDevice].m_bRun)
                    {
                        m_rtuClient[nDevice].pauseThread();
                    }
                    else
                    {

                    }
                    //  checkEssPcsClient(nDevice);   //<--|
                }

                return;
            }
            else if (nEvent == clsConstant.PCS_RTU_CONNECTION_FAILED)
            {
                updateMeterListImage(nDevice, clsConstant.PCS_RTU_CONNECTION_FAILED);

                if ((m_rtuClient[nDevice] != null) && (m_rtuClient[nDevice].m_TrendInfo.nConnectionStatus != clsConstant.PCS_RTU_SHUTDOWN) && m_bServerRun)
                {
                    if (m_rtuClient[nDevice].m_bRun)
                    {
                        m_rtuClient[nDevice].pauseThread();
                    }
                    else
                    {

                    }
                    //  checkEssPcsClient(nDevice);   //<--|
                }
                
                return;
            }
            else if (nEvent == clsConstant.PCS_RTU_CONNECTING)
            {
                if ((m_rtuClient[nDevice] != null) && (m_rtuClient[nDevice].m_TrendInfo.nConnectionStatus != clsConstant.PCS_RTU_SHUTDOWN) && m_bServerRun)
                {
                    updateMeterListImage(nDevice, clsConstant.PCS_RTU_CONNECTING);
                    m_rtuClient[nDevice].m_TrendInfo.nConnectionStatus = clsConstant.PCS_RTU_CONNECTING;
                    m_EssPcsServiceProxy.SetTrendInfo(m_rtuClient[nDevice].m_TrendInfo);
                }

                return;
            }
            else if (nEvent == clsConstant.PCS_RTU_COM_ERROR)
            {
                updateMeterListImage(nDevice, clsConstant.PCS_RTU_COM_ERROR);
                m_rtuClient[nDevice].m_TrendInfo.nConnectionStatus = clsConstant.PCS_RTU_COM_ERROR;
                m_EssPcsServiceProxy.SetTrendInfo(m_rtuClient[nDevice].m_TrendInfo);

                return;
            }
            else if (nEvent == clsConstant.PCS_RTU_SHUTDOWN)
            {
                updateMeterListImage(nDevice, clsConstant.PCS_RTU_SHUTDOWN);
                m_rtuClient[nDevice].m_TrendInfo.nConnectionStatus = clsConstant.PCS_RTU_SHUTDOWN;
                m_EssPcsServiceProxy.SetTrendInfo(m_rtuClient[nDevice].m_TrendInfo);

                if (m_bServerRun)
                {
                    //checkEssPcsClient(nDevice);
                }
                else
                {
                }

                return;
            }
        }

        public void processTcpClientEssPcsMsg(clsEssPcsMsg msg)
        {
            String strLog;
            int nIndex;

            //  장치 시각 설정
            /*
            if ((msg.msgType == clsConstant.ESSPCS_CMD_TIME_SET) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "시각 설정 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            */
            //  장치 시각 읽어오기
            if ((msg.msgType == clsConstant.ESSPCS_CMD_TIME_GET) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "시각 읽어오기 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  장치 설정 정보 읽어오기
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_INFO_GET) && (msg.msgResult == 0))
            {
                nIndex = g_clsSqlServer.GetDeviceIndex(msg.msgDevice);
                g_clsSqlServer.parseSetting(g_clsSqlServer.m_clsSettingInfo[nIndex]);
                var capturedSetting = g_clsSqlServer.m_clsSettingInfo[nIndex];
                EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.SetSetting(capturedSetting); });

                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "장치 설정 읽기");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            /*
            //  장치 설정 정보 기록하기
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_INFO_SET) && (msg.msgResult == 0))
            {
                nIndex = g_clsSqlServer.GetDeviceIndex(msg.msgDevice);
                Array.Copy(msg.msgBuf, g_clsSqlServer.m_clsSettingInfo[nIndex].bSetupData, clsConstant.PCS_SETUP_DATA_NUM);
                g_clsSqlServer.parseSetting(g_clsSqlServer.m_clsSettingInfo[nIndex]);
                m_EssPcsServiceProxy.SetSetting(g_clsSqlServer.m_clsSettingInfo[nIndex]);

                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "장치 설정 쓰기");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            */
            //  Fault Clear
            else if (msg.msgType == clsConstant.ESSPCS_CMD_FAULT_CLEAR)
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "Fault Clear 제어 명령 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  Warn Clear
            else if (msg.msgType == clsConstant.ESSPCS_CMD_WARN_CLEAR)
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "Warn Clear 제어 명령 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  장치 메모리 초기화
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_MEMORY_INIT) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "메모리 초기화 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  장치 재부팅
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_REBOOT) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "재부팅 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  장치 매뉴얼 트리거
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_MANUAL_TRIGGER) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "매뉴얼 트리거 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  장치 이전 트리거 정보 수신
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_PREV_TRIGGER) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "이전 트리거 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  장치 칼리브레이션 시작
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_CAL_START) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "캘리브레이션 시작 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  장치 칼리브레이션 중지
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_CAL_STOP) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "캘리브레이션 결과 저장 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  장치 릴레이 컨트롤 활성화
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_RELAY_CONTROL_ON) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "RELAY CONTROL ON 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  장치 릴레이 컨트롤 비활성화
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_RELAY_CONTROL_OFF) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "RELAY CONTROL OFF 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  장치 트리거 락 활성화
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_TRIGGER_LOCK_ON) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "TRIGGER LOCK ON 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  장치 트리거 락 비활성화
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_TRIGGER_LOCK_OFF) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "TRIGGER LOCK OFF 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  장치 릴레이 제어 명령
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_RELAY) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "RELAY ON/OFF 명령 응답 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  PCS 출력 제어
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_OUTPUT_SET) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "PCS 출력 제어 명령 수신");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }
            //  고장거리 정보
            else if ((msg.msgType == clsConstant.ESSPCS_CMD_FAULT_LOC) && (msg.msgResult == 0))
            {
                strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "고장 위치 정보 전송");
                m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
            }

            if (msg.msgSession != "")
            {
                m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
            }
        }

        private void updateMeterListImage(int nDevice, int nStatus)
        {
            if (c1TopicBarDevice.ImageList != null)
            {
                clsEssPcsSettingInfo clsInfo = g_clsSqlServer.m_clsSettingInfo[nDevice];
                String strPage = "";
                int nImageIndex = -1;

                if (nStatus == clsConstant.PCS_RTU_CONNECTED)
                {
                    strPage = "Connected";
                    nImageIndex = 12;
                }
                else if (nStatus == clsConstant.PCS_RTU_CONNECTION_FAILED)
                {
                    strPage = "Connection Failed";
                    nImageIndex = 13;
                }
                else if (nStatus == clsConstant.PCS_RTU_CONNECTING)
                {
                    strPage = "Connecting";
                    nImageIndex = 14;
                }
                else if (nStatus == clsConstant.PCS_RTU_COM_ERROR)
                {
                    if (m_rtuClient[nDevice] != null)
                    {
                        if (m_rtuClient[nDevice].m_nComErrorCount > 0)
                        {
                            strPage = string.Format("Error (Count: {0})", m_rtuClient[nDevice].m_nComErrorCount);
                        }
                        else
                        {
                            strPage = "Waiting for connection";
                        }
                    }
                    else
                    {
                        strPage = "";
                    }
                }
                else
                {
                    strPage = "Disconnected";
                    nImageIndex = 10;
                }

                if (m_rtuClient[nDevice] != null)
                {
                    m_rtuClient[nDevice].m_statusList.Add(new clsStatus(strPage, false));
                    m_rtuClient[nDevice].m_nMsgImageIndex = nImageIndex;
                }
            }
        }

        private void displayTopicPage(int nIndex, String strPage)
        {
            c1TopicBarDevice.Pages[nIndex].Text = strPage;
        }

        private void displayTopicLink(int nIndex, DateTime t, String strLog)
        {
            C1TopicPage topicPage = c1TopicBarDevice.Pages[nIndex];
            C1TopicLink topicLink = new C1TopicLink();

            if (topicPage.Links.Count > 9)
            {
                topicPage.Links.RemoveAt(0);
            }

            topicLink.Text = String.Format(" {0}: {1}",t, strLog);

            topicPage.Links.Add(topicLink);
        }

        public void displayTcpClientLog(int nIndex, DateTime t, string strLog)
        {
            clsEssPcsSettingInfo clsInfo = g_clsSqlServer.m_clsSettingInfo[nIndex];
            String strPage;

            strPage = String.Format(" {0} > {1}: {2}", clsInfo.strDevice, t, strLog);

            displayTopicPage(nIndex, strPage);
        }

        private void menuStripMainFileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void runConnect()
        {
            String strIp;
            int i;

            for (i = 0; i < m_rtuClient.Count; i++)
            {
                m_rtuClient[i].setPause(false);

                if (g_clsSqlServer.m_clsSettingInfo[i].bUSED)
                {
                    m_rtuClient[i].m_bReqStopSocket = false;

                    strIp = String.Format("{0}.{1}.{2}.{3}",
                        g_clsSqlServer.m_clsSettingInfo[i].bIpAddress[0],
                        g_clsSqlServer.m_clsSettingInfo[i].bIpAddress[1],
                        g_clsSqlServer.m_clsSettingInfo[i].bIpAddress[2],
                        g_clsSqlServer.m_clsSettingInfo[i].bIpAddress[3]);

                    m_rtuClient[i].setTcpConnectInfo(strIp, g_clsSqlServer.m_clsSettingInfo[i].nEthernetPort);

                    m_rtuClient[i].m_nConnectStep = 1;
                }
                else
                {
                    m_rtuClient[i].m_bReqStopSocket = true;
                }
            }

            m_UdpClient.UdpConnect(g_clsSqlServer.m_strLocalIp, 60001);

            menuStripMainFileRefresh.Enabled = false;
            menuStripMainConnect.Enabled = false;
            menuStripMainRunStart.Enabled = true;
            menuStripMainRunStop.Enabled = false;
            btnInvFaultClear.Enabled = true;
            menuStripMainDisconnect.Enabled = true;

            txtStatusRun.Text = " [접속] ";

            try
            {
                m_EssPcsServiceProxy.SetServerStatus(m_bServerRun);
            }
            catch (CommunicationException exception)
            {
                System.Diagnostics.Debug.WriteLine("runConnect: " + exception.Message);
            }
            catch (TimeoutException exception)
            {
                System.Diagnostics.Debug.WriteLine("runConnect: " + exception.Message);
            }
        }

        private void runStart()
        {
            m_bDaqRun = true;

            menuStripMainFileRefresh.Enabled = false;
            menuStripMainConnect.Enabled = false;
            menuStripMainRunStart.Enabled = false;
            menuStripMainRunStop.Enabled = true;
            menuStripMainDisconnect.Enabled = false;

            btnInvSet.Enabled = true;
            btnInvRun.Enabled = true;
            btnInvStop.Enabled = true;

            btnEssControlStart.Enabled = true;
            btnEssControlStop.Enabled = true;

            txtStatusRun.Text = " [실행] ";

            m_swSQLite.Start();

            m_bIsEmsRun = false;

            try
            {
                m_EssPcsServiceProxy.SetServerStatus(m_bServerRun);
            }
            catch (CommunicationException exception)
            {
                System.Diagnostics.Debug.WriteLine("runStart: " + exception.Message);
            }
            catch (TimeoutException exception)
            {
                System.Diagnostics.Debug.WriteLine("runStart: " + exception.Message);
            }
        }

        private void runStop()
        {
            m_EssPcsServiceProxy.SetServerStatus(m_bServerRun);

            m_bDaqRun = false;

            menuStripMainFileRefresh.Enabled = true;
            menuStripMainConnect.Enabled = false;
            menuStripMainRunStart.Enabled = true;
            menuStripMainRunStop.Enabled = false;
            menuStripMainDisconnect.Enabled = true;

            btnInvSet.Enabled = false;
            btnInvRun.Enabled = false;
            btnInvStop.Enabled = false;

            btnEssControlStart.Enabled = false;
            btnEssControlStop.Enabled = false;

            txtStatusRun.Text = " [정지] ";

            m_swSQLite.Stop();

            m_bIsEmsRun = false;

            //m_frmWait = new frmWaitThread();
            //m_frmWait.ShowDialog();
            //<--|
            //|-->
            /*            
                        foreach (clsTcpClient tcpClient in m_tcpClient)
                        {
                            tcpClient.dis
                        }
             */
            /*           for (i = 0; i < g_clsSqlServer.m_clsSettingInfo.Count; i++)
                       {
                           clsTcpClient tcpClient = new clsTcpClient(i, g_clsSqlServer.m_clsSettingInfo[i], g_clsSqlServer.m_clsTrendInfo[i], false);

                           m_tcpClient.Add(tcpClient);
                       }
                     */
            //m_tcpClient.Clear();
        }

        private void runDisconnect()
        {
            int i;

            m_EssPcsServiceProxy.SetServerStatus(m_bServerRun);

            for (i = 0; i < m_rtuClient.Count; i++)
            {
                try
                {
                    if (m_rtuClient[i] != null)
                    {
                        if (g_clsSqlServer.m_clsSettingInfo[i].bUSED)
                        {
                            m_rtuClient[i].m_bReqStopSocket = true;
                            m_rtuClient[i].m_bDaqRun = false;
                            m_rtuClient[i].m_nDisConnectStep = 1;
                            m_rtuClient[i].m_bDestroyClient = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    m_SystemLogList.Add(new clsSystemLog(m_rtuClient[i].m_nIndex, ex.Message, DateTime.Now));
                }
            }

            menuStripMainFileRefresh.Enabled = true;
            menuStripMainConnect.Enabled = true;
            menuStripMainRunStart.Enabled = false;
            menuStripMainRunStop.Enabled = false;
            btnInvFaultClear.Enabled = false;
            menuStripMainDisconnect.Enabled = false;

            txtStatusRun.Text = " [해제] ";

            //m_frmWait = new frmWaitThread();
            //m_frmWait.ShowDialog();
            //<--|
            //|-->
        }

        private void menuStripMainRunStart_Click(object sender, EventArgs e)
        {
            runStart();
        }

        private void menuStripMainRunStop_Click(object sender, EventArgs e)
        {
            runStop();
        }

        private void menuStripMainHelpAbout_Click(object sender, EventArgs e)
        {            
            frmAbout frm = new frmAbout();

            frm.ShowDialog();
        }

        private void toolStripSettingConnectionCheck_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].ConnectionCheck();
        }

        private void toolStripSettingV0Reset_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].MaxV0Reset();
        }

        private void toolStripSettingDemandReset_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].DemandReset();
        }

        private void toolStripSettingSysClear_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].SystemEventClear();
        }

        private void toolStripSettingPQClear_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].PqEventClear();
        }

        private void toolStripSettingSysEvent_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].SystemEventReceive();
        }

        private void toolStripSettingPQEvent_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].PqEventReceive();
        }

        private void toolStripSettingLoadProfile_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].LoadProfileReceive();
        }

        private void toolStripSettingManualTrigger_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].ManualTrigger();
        }

        private void toolStripSettingCalibration0_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].Calibration_0();
        }

        private void toolStripSettingCalibration60_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].Calibration_60();
        }

        private void menuStripMainRunRelay1On_Click(object sender, EventArgs e)
        {
            setRelay(1, true);
        }

        private void menuStripMainRunRelay1Off_Click(object sender, EventArgs e)
        {
            setRelay(1, false);
        }

        private void menuStripMainRunRelay2On_Click(object sender, EventArgs e)
        {
            setRelay(2, true);
        }

        private void menuStripMainRunRelay2Off_Click(object sender, EventArgs e)
        {
            setRelay(2, false);
        }

        private void toolStripSettingReceiveInfo_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].GetSetting();
        }

        private void btnRelay1On_Click(object sender, EventArgs e)
        {
            setRelay(1, true);
        }

        private void btnRelay1Off_Click(object sender, EventArgs e)
        {
            setRelay(1, false);
        }

        private void btnRelay2On_Click(object sender, EventArgs e)
        {
            setRelay(2, true);
        }

        private void btnRelay2Off_Click(object sender, EventArgs e)
        {
            setRelay(2, false);
        }

        private void toolStripSettingClock_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].SetTime();
        }

        private void toolStripSettingSendInfo_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //Setting_Get();
            //m_tcpClient[nNO].SetSetting();
        }

        private void menuStripMainRunSysEvent_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].m_EventReqList.Add(new clsEventReq(clsConstant.PQMS_SYSTEM_EVENT, -1, DateTime.Now, 0, 0));
        }

        private void menuStripMainRunPqEvent_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].m_EventReqList.Add(new clsEventReq(clsConstant.PQMS_PQ_EVENT, -1, DateTime.Now, 0, 0));
        }

        private void menuStripMainRunLoadProfile_Click(object sender, EventArgs e)
        {
            //int nNO = toolStripComboSettingMeterId.SelectedIndex;

            //m_tcpClient[nNO].m_EventReqList.Add(new clsEventReq(clsConstant.PQMS_LOAD_PROFILE, -1, DateTime.Now, 0, 0));
        }

        private void menuStripMainRunWave_Click(object sender, EventArgs e)
        {
            /*
            DateTime dtWave = DateTime.Now;
            string strLog = "";
            int nRowIndex = 0;
            int nWaveIndex = 0;
            int nWaveEndBlock = 0;
            int nFaultPoint = 0;
            int nMeter = 0;
            int nDevice = 0;

            nRowIndex = navPqEvent.BindingSource.Position;
            g_clsSqlServer.PqEvent_GetWaveInfo(nRowIndex, ref nMeter, ref nWaveIndex, ref nWaveEndBlock, ref dtWave, ref nFaultPoint, ref strLog);
            nDevice = getMeterIndex(nMeter);
            m_tcpClient[nDevice].m_nWaveBlock = clsConstant.PQMS_WAVE_START_BLOCK;
            m_tcpClient[nDevice].receiveWaveDataStart(nWaveIndex, nWaveEndBlock, dtWave, nFaultPoint);
            */
        }

        private void menuStripMainRunRealInfo_Click(object sender, EventArgs e)
        {
            /*
            int nNO = toolStripComboSettingMeterId.SelectedIndex;

            setStatusProgressBarRange(clsConstant.CDFR_TREND_DATA_START, clsConstant.CDFR_TREND_DATA_NUM);

            m_tcpClient[nNO].m_bReceiveTrendData    = false;
            m_tcpClient[nNO].receiveTrendData();
            */
        }

        private void timerMain_Tick(object sender, EventArgs e)
        {
            processEssPcsMsgList();
            processWaveInfoList();
            processStatusEventList();
            processTrendList();
            processSystemLogList();
            timerMainLog();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            /*
            DateTime dtSearchStart = Convert.ToDateTime(dateSearchStart.Text);
            DateTime dtSearchEnd = Convert.ToDateTime(dateSearchEnd.Value);
            int nSearchType = 0;
            int nMeter = selSearchDevice.SelectedIndex - 1;

            if (selSearchDay.Checked) nSearchType = 1;
            if (selSearchWeek.Checked) nSearchType = 2;
            if (selSearchMonth.Checked) nSearchType = 3;
            if (selSearchCustom.Checked) nSearchType = 4;

            g_clsSqlServer.SysLog_Search(nSearchType, dtSearchStart, dtSearchEnd, nMeter);
            g_clsSqlServer.SystemEvent_Search(nSearchType, dtSearchStart, dtSearchEnd, nMeter);
            g_clsSqlServer.PqEvent_Search(nSearchType, dtSearchStart, dtSearchEnd, nMeter);
            g_clsSqlServer.LoadProfile_Search(nSearchType, dtSearchStart, dtSearchEnd, nMeter);
            g_clsSqlServer.WaveForm_Search(nSearchType, dtSearchStart, dtSearchEnd, nMeter);
            */
        }

        private void toolStripSettingResetError_Click(object sender, EventArgs e)
        {
            /*
            int nNO = toolStripComboSettingMeterId.SelectedIndex;

            if (nNO != -1)
            {
                if (m_tcpClient[nNO] != null)
                {
                    m_tcpClient[nNO].m_nComErrorCount = 0;
                    updateMeterListImage(nNO, clsConstant.PQMS_COM_ERROR);
                }
            }
            */
        }

        private void menuStripMainFileRefresh_Click(object sender, EventArgs e)
        {
            displayMeterList(true);
        }

        private void Tray_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            Tray.Visible = false;
            this.WindowState = FormWindowState.Maximized;
        }

        private void frmMain_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                if (Properties.Settings.Default.trayMove)
                {
                    if (!moveTray())
                    {

                    }
                }
            }
        }

        private bool moveTray()
        {
            //  if (MessageBox.Show("트레이로 이동하시겠습니까?", "ESS-PCS Data Server", MessageBoxButtons.YesNo) == DialogResult.Yes)
            //  {
                this.Visible = false;
                Tray.Visible = true;

                return true;
            //  }
            //  else
            //  {
            //    return false;
            //  }
        }

        private void setDeviceListState()
        {
            /*
            if(Properties.Settings.Default.showDeviceList)
            {
                splitContainer1.Panel1MinSize       = 25;
                splitContainer1.SplitterDistance    = 200;
                splitContainer1.SplitterWidth       = 4;
                splitContainer1.Panel1.Show();
            }
            else
            {
                splitContainer1.Panel1.Hide();
                splitContainer1.Panel1MinSize       = 0;
                splitContainer1.SplitterDistance    = 0;
                splitContainer1.SplitterWidth       = 1;
            }

            menuStripMainSettingDeviceList.Checked = Properties.Settings.Default.showDeviceList;
            */
            Properties.Settings.Default.Save();
        }

        private void menuStripMainSettingTray_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.trayMove = !Properties.Settings.Default.trayMove;
            menuStripMainSettingTray.Checked = Properties.Settings.Default.trayMove;
        }

        private void ribbonHide_Click(object sender, EventArgs e)
        {
            c1RibbonMenu.Minimized = true;
        }

        private void ribbonShow_Click(object sender, EventArgs e)
        {
            c1RibbonMenu.Minimized = false;
        }

        private void menuStripMainSettingWaveSave_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.saveData = !Properties.Settings.Default.saveData;
        }

        private void menuStripMainConnect_Click(object sender, EventArgs e)
        {
            /*
            int i;

            m_tcpClient.Clear();

            for (i = 0; i < g_clsSqlServer.m_clsSettingInfo.Count; i++)
            {
                clsTcpClient tcpClient = new clsTcpClient(i, g_clsSqlServer.m_clsSettingInfo[i], g_clsSqlServer.m_clsTrendInfo[i], false);

                m_tcpClient.Add(tcpClient);
            }
            */
            m_bServerRun = true;

            runConnect();
        }

        private void menuStripMainDisconnect_Click(object sender, EventArgs e)
        {
            m_bServerRun = false;

            runDisconnect();
            m_UdpClient.UdpDisconnect();
        }

        private void btnPeriodSet_Click(object sender, EventArgs e)
        {
            UInt32 nDeviceSyncTime = 0;
            UInt32 nManualTriggerTime = 0;

            nDeviceSyncTime = Decimal.ToUInt32(numericDbServerPort1.Value);
            Properties.Settings.Default.sitePort1 = nDeviceSyncTime;
            Properties.Settings.Default.Save();

            m_EssPcsServiceProxy.SetPeriod(nDeviceSyncTime, nManualTriggerTime, false);
        }

        private void chkHmiMode_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.hmiMode = !Properties.Settings.Default.hmiMode;
            chkHmiMode.Checked = Properties.Settings.Default.hmiMode;
            Properties.Settings.Default.Save();
        }

        private void timerEssPcs_Tick(object sender, EventArgs e)
        {
            String strData = m_bIsEmsRun ? "[EMS RUN]" : "[EMS STOP]";
            Int16 sData = 0;

            if (txtEmsRun.Text != strData)
            {
                txtEmsRun.Text = strData;
            }

            if (m_UdpClient != null)
            {
                float fGenOut = 0;

                m_UdpClient.checkUpdate();

                if (m_UdpClient.getGenOut(ref fGenOut))
                {
                    DateTime dtNow = DateTime.Now;
                    float genOutCopy = fGenOut;
                    EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setEssGenOutput(genOutCopy, true); });
                    txtGenOut.Text = String.Format("[{0:00}:{1:00}:{2:00} {3:F1}kW]", dtNow.Hour, dtNow.Minute, dtNow.Second, fGenOut);
                }
            }

            if (m_rtuClient[0] != null)
            {
                String strFault = "";
                bool bStop = false;
                bool bRun = false;
                bool bSet = false;
                bool bParameter = false;
                bool bFaultClear = false;
                float fSetP = 0;
                float fSetQ = 0;

                m_rtuClient[0].m_bReqTrendStop = chkStopTrend.Checked;

                txtTemp.Text = String.Format("[TEMP: {0:F1}℃]", m_rtuClient[0].m_TrendInfo.fTemp_InternalPcs);
                txtReferencePSet.Text = String.Format("[SETP: {0:F1}kW]", m_rtuClient[0].m_TrendInfo.fReferenceP);
                txtReferenceQSet.Text = String.Format("[SETQ: {0:F1}kVar]", m_rtuClient[0].m_TrendInfo.fReferenceQ);
                txtBmsSoc.Text = String.Format("[SOC: {0:F1}%]", m_rtuClient[0].m_TrendInfo.fBatSoc);
                txtPcsOutput.Text = String.Format("[OUT: {0:F1}kW]", m_rtuClient[0].m_TrendInfo.fBatteryPower);

                switch(m_rtuClient[0].m_TrendInfo.nInvStatus)
                {
                    case 1:
                        txtInvStatus.Text = String.Format("[인버터동작]");
                        break;

                    case 2:
                        txtInvStatus.Text = String.Format("[인버터정지]");
                        break;

                    case 3:
                        txtInvStatus.Text = String.Format("[인버터고장]");
                        break;

                    case 0:
                        txtInvStatus.Text = String.Format("[인버터 {0}]", m_rtuClient[0].m_TrendInfo.nInvStatus);
                        break;
                }

                strFault = getEssPcsFault(m_rtuClient[0].m_TrendInfo);

                // WCF 상태조회 + 명령 처리를 백그라운드 스레드에서 수행
                var rtuClient0 = m_rtuClient[0];
                EnqueueWcf(() =>
                {
                    if (m_EssPcsServiceProxy == null) return;

                    bool bStop2 = false, bRun2 = false, bSet2 = false, bParameter2 = false, bFaultClear2 = false;
                    float fSetP2 = 0, fSetQ2 = 0;
                    m_EssPcsServiceProxy.getInvStatus(0, ref bStop2, ref bRun2, ref bSet2, ref fSetP2, ref fSetQ2, ref bFaultClear2, ref bParameter2);

                    if (bStop2)
                    {
                        clsEssPcsMsg msg = new clsEssPcsMsg();
                        msg.msgType = clsConstant.ESSPCS_CMD_INV_STOP;
                        msg.msgFinish = 0; msg.msgDevice = 0; msg.msgIndex = 2;
                        rtuClient0.CommandQueue.Enqueue(msg);
                        m_EssPcsServiceProxy.setInvStop(0, false);
                    }

                    if (bRun2)
                    {
                        clsEssPcsMsg msg = new clsEssPcsMsg();
                        msg.msgType = clsConstant.ESSPCS_CMD_INV_RUN;
                        msg.msgFinish = 0; msg.msgDevice = 0; msg.msgIndex = 1;
                        rtuClient0.CommandQueue.Enqueue(msg);
                        m_EssPcsServiceProxy.setInvRun(0, false);
                    }

                    if (bSet2)
                    {
                        clsEssPcsMsg msg = new clsEssPcsMsg();
                        msg.msgType = clsConstant.ESSPCS_CMD_OUTPUT_SET;
                        msg.msgFinish = 0; msg.msgDevice = 0;
                        msg.msgBuf = new Byte[4];
                        Int16 sData2 = (Int16)(fSetP2 * 10);
                        msg.msgBuf[0] = (Byte)(sData2 >> 8); msg.msgBuf[1] = (Byte)(sData2 >> 0);
                        sData2 = (Int16)(fSetQ2 * 10);
                        msg.msgBuf[2] = (Byte)(sData2 >> 8); msg.msgBuf[3] = (Byte)(sData2 >> 0);
                        rtuClient0.CommandQueue.Enqueue(msg);
                        m_EssPcsServiceProxy.setInvSet(0, false, 0.0f, 0.0f);
                    }

                    if (bFaultClear2)
                    {
                        clsEssPcsMsg msg = new clsEssPcsMsg();
                        msg.msgType = clsConstant.ESSPCS_CMD_FAULT_CLEAR;
                        msg.msgFinish = 0; msg.msgDevice = 0; msg.msgIndex = 0x0003;
                        rtuClient0.CommandQueue.Enqueue(msg);
                        m_EssPcsServiceProxy.setInvFaultClear(0, false);
                    }

                    if (bParameter2)
                    {
                        clsEssPcsParameterInfo clsParamInfo = m_EssPcsServiceProxy.getInvParameter();
                        clsEssPcsMsg msg = new clsEssPcsMsg();
                        msg.msgType = clsConstant.ESSPCS_CMD_INV_PARAMETER;
                        msg.msgFinish = 0; msg.msgDevice = 0;
                        msg.msgBuf = new Byte[104];
                        Array.Copy(clsParamInfo.bParameterData, msg.msgBuf, 104);
                        rtuClient0.CommandQueue.Enqueue(msg);
                        m_EssPcsServiceProxy.clearInvParameter();
                    }
                });

                checkEssControlInfo();
            }

            EnqueueWcf(() =>
            {
                if (m_EssBmsServiceProxy != null && m_rtuClient.Count > 0 && m_rtuClient[0] != null)
                {
                    if (m_EssBmsServiceProxy.GetServerStatus())
                    {
                        clsEssBmsInfo bmsInfo = m_EssBmsServiceProxy.GetBmsInfo(0);
                        m_rtuClient[0].m_TrendInfo.fBatSoc = bmsInfo.fSoc;
                    }
                }
            });

            if (m_bIsEmsRun && m_bServerRun)
            {
                runEms();
            }
        }

        private void btnInvSet_Click(object sender, EventArgs e)
        {
            if ((m_rtuClient[0] != null) && m_bServerRun)
            {
                m_EssPcsServiceProxy.setInvSet(0, true, Single.Parse(numInvSetP.Text), Single.Parse(numInvSetQ.Text));
            }
            else 
            {
                MessageBox.Show("PCS와의 통신이 연결되어 있지 않습니다.", "시험 조건 오류");
            }
        }

        private String getEssPcsFault(clsEssPcsTrendInfo trendInfo)
        {
            String strFault = "";
            /*
            if (trendInfo.dcStatus.bit00_OverVoltageRelayError) strFault += "(DC Over Voltage Relay Error)";
            if (trendInfo.dcStatus.bit01_UnderVoltageRelayError) strFault += "(DC Under Voltage Relay Error)";
            if (trendInfo.dcStatus.bit02_CanFlt) strFault += "(DC CAN Fault)";
            if (trendInfo.dcStatus.bit03_DcPreChargeFlt) strFault += "(DC Pre-Charge Fault)";
            if (trendInfo.dcStatus.bit04_SpdError) strFault += "(DC SPD Error)";
            if (trendInfo.dcStatus.bit05_MccbAError) strFault += "(DC MCCB A Error)";
            if (trendInfo.dcStatus.bit06_GfrError) strFault += "(DC GFR Error)";
            if (trendInfo.dcStatus.bit07_BatteryOverVoltageError) strFault += "(DC Battery Over Voltage Error)";
            if (trendInfo.dcStatus.bit08_BatteryUnderVoltageError) strFault += "(DC Battery Under Voltage Error)";

            if (trendInfo.unitStatus.bit00_OverVoltage) strFault += "(Over Voltage)";
            if (trendInfo.unitStatus.bit01_IgbtFlt) strFault += "(IGBT Fault)";
            if (trendInfo.unitStatus.bit02_OverCurrentTrip) strFault += "(Over Current Trip)";
            if (trendInfo.unitStatus.bit03_OverCurrentRelay) strFault += "(Over Current Relay)";
            if (trendInfo.unitStatus.bit04_OverLoad) strFault += "(Over Load)";
            if (trendInfo.unitStatus.bit05_OverCurrentPeak) strFault += "(Over Current Peak)";
            if (trendInfo.unitStatus.bit06_SwOverCurrentTrip) strFault += "(SW Over Current Trip)";
            if (trendInfo.unitStatus.bit07_TemperatureFault) strFault += "(Temperature Fault)";
            if (trendInfo.unitStatus.bit08_McFault) strFault += "(MC Fault)";
            if (trendInfo.unitStatus.bit09_SyncFault) strFault += "(Sync Fault)";
            if (trendInfo.unitStatus.bit10_PwmFault) strFault += "(PWM Fault)";
            if (trendInfo.unitStatus.bit11_CurrentInbalance) strFault += "(Current InBanace)";
            if (trendInfo.unitStatus.bit15_Flt) strFault += "(Unit Fault)";

            if (trendInfo.gridStatus.bit00_OverVoltageRelay) strFault += "(GRID Over Voltage Relay)";
            if (trendInfo.gridStatus.bit01_UnderVoltageRelay) strFault += "(GRID Under Voltage Relay)";
            if (trendInfo.gridStatus.bit02_OverFreqRelay) strFault += "(GRID Over Frequency Relay)";
            if (trendInfo.gridStatus.bit03_UnderFreqRelay) strFault += "(GRID Under Frequency Relay)";
            if (trendInfo.gridStatus.bit04_Fail) strFault += "(GRID Fail)";
            if (trendInfo.gridStatus.bit05_EarthError) strFault += "(GRID Earth Error)";
            if (trendInfo.gridStatus.bit06_OverCurrentRelay) strFault += "(GRID Over Current Relay)";
            if (trendInfo.gridStatus.bit07_EmergencyStop) strFault += "(GRID Emergency Stop)";
            if (trendInfo.gridStatus.bit08_FuseOpen) strFault += "(GRID Fuse Open)";
            if (trendInfo.gridStatus.bit09_EmsCommunicationError) strFault += "(GRID Ems Communication Error)";
            if (trendInfo.gridStatus.bit10_DcMccbFault) strFault += "(GRID DC MCCB Fault)";
            if (trendInfo.gridStatus.bit11_MainMccbFault) strFault += "(GRID Main MCCB Fault)";
            if (trendInfo.gridStatus.bit14_BmsCommunicationError) strFault += "(GRID BMS Communication Error)";
            if (trendInfo.gridStatus.bit15_Flt) strFault += "(GRID Fault)";

            if (trendInfo.canError.bit00_Stack1CanError) strFault += "(Stack1 CAN Error)";
            if (trendInfo.canError.bit01_Stack2CanError) strFault += "(Stack2 CAN Error)";
            if (trendInfo.canError.bit02_Stack3CanError) strFault += "(Stack3 CAN Error)";
            if (trendInfo.canError.bit03_Stack4CanError) strFault += "(Stack4 CAN Error)";
            if (trendInfo.canError.bit04_Stack5CanError) strFault += "(Stack5 CAN Error)";
            if (trendInfo.canError.bit05_Stack6CanError) strFault += "(Stack6 CAN Error)";
            if (trendInfo.canError.bit06_Stack7CanError) strFault += "(Stack7 CAN Error)";
            if (trendInfo.canError.bit07_Stack8CanError) strFault += "(Stack8 CAN Error)";
            */

            return strFault;
        }

        private void btnInvRun_Click(object sender, EventArgs e)
        {
            if ((m_rtuClient[0] != null) && m_bServerRun)
            {
                m_EssPcsServiceProxy.setInvRun(0, true);
            }
            else
            {
                MessageBox.Show("PCS와의 통신이 연결되어 있지 않습니다.", "인버터 제어 오류");
            }
        }

        private void btnInvStop_Click(object sender, EventArgs e)
        {
            if ((m_rtuClient[0] != null) && m_bServerRun)
            {
                m_EssPcsServiceProxy.setInvStop(0, true);
            }
            else
            {
                MessageBox.Show("PCS와의 통신이 연결되어 있지 않습니다.", "인버터 제어 오류");
            }
        }
        
        private bool checkTimeRange()
        {
            Int32 nChargeStart = Int32.Parse(numEssChargeStartTimeHour.Text) * 60 + Int32.Parse(numEssChargeStartTimeMin.Text);
            Int32 nChargeEnd = Int32.Parse(numEssChargeEndTimeHour.Text) * 60 + Int32.Parse(numEssChargeEndTimeMin.Text);
            Int32 nDischargeStart = Int32.Parse(numEssDischargeStartTimeHour.Text) * 60 + Int32.Parse(numEssDischargeStartTimeMin.Text);
            Int32 nDischargeEnd = Int32.Parse(numEssDischargeEndTimeHour.Text) * 60 + Int32.Parse(numEssDischargeEndTimeMin.Text);
            bool bResult = true;

            if (nChargeStart == nChargeEnd)
            {
                bResult = false;
            }
            else if (nDischargeStart == nDischargeEnd)
            {
                bResult = false;
            }
            else if (nChargeStart < nChargeEnd)
            {
                if ((nDischargeStart >= nChargeStart) && (nDischargeStart <= nChargeEnd))
                {
                    bResult = false;
                }
                else if ((nDischargeEnd >= nChargeStart) && (nDischargeEnd <= nChargeEnd))
                {
                    bResult = false;
                }
            }
            else
            {
                if ((nDischargeStart >= nChargeStart) && (nDischargeEnd >= nChargeStart))
                {
                    bResult = false;
                }
                else if ((nDischargeStart <= nChargeEnd) && (nDischargeEnd <= nChargeEnd))
                {
                    bResult = false;
                }
            }

            return bResult;
        }

        private void btnEssControlStart_Click(object sender, EventArgs e)
        {
            if (checkTimeRange())
            {
                numEssChargeStartTimeHour.Enabled = false;
                numEssChargeStartTimeMin.Enabled = false;
                numEssChargeEndTimeHour.Enabled = false;
                numEssChargeEndTimeMin.Enabled = false;

                numEssDischargeStartTimeHour.Enabled = false;
                numEssDischargeStartTimeMin.Enabled = false;
                numEssDischargeEndTimeHour.Enabled = false;
                numEssDischargeEndTimeMin.Enabled = false;

                m_bIsEmsRun = true;
            }
            else
            {
                MessageBox.Show("충전시간과 방전시간 설정에 문제가 있습니다.", "시간 설정 오류");
            }
        }

        private void btnEssControlStop_Click(object sender, EventArgs e)
        {
            m_bIsEmsRun = false;

            if (m_rtuClient[0] != null)
            {
                m_EssPcsServiceProxy.setInvStop(0, true);
            }

            numEssChargeStartTimeHour.Enabled = true;
            numEssChargeStartTimeMin.Enabled = true;
            numEssChargeEndTimeHour.Enabled = true;
            numEssChargeEndTimeMin.Enabled = true;

            numEssDischargeStartTimeHour.Enabled = true;
            numEssDischargeStartTimeMin.Enabled = true;
            numEssDischargeEndTimeHour.Enabled = true;
            numEssDischargeEndTimeMin.Enabled = true;
        }

        private void btnInvFaultClear_Click(object sender, EventArgs e)
        {
            if ((m_rtuClient[0] != null) && m_bServerRun)
            {
                m_EssPcsServiceProxy.setInvFaultClear(0, true);
            }
            else
            {
                MessageBox.Show("PCS와의 통신이 연결되어 있지 않습니다.", "인버터 Fault Clear 오류");
            }
        }

        private void btnPcsOutput_Click(object sender, EventArgs e)
        {
            frmPcsControl frm = new frmPcsControl();

            frm.ShowDialog();
        }

        private void btnPanelAxis_Click(object sender, EventArgs e)
        {
            frmAxisControl frm = new frmAxisControl();

            frm.ShowDialog();
        }

        private void btnDcLinkOn_Click(object sender, EventArgs e)
        {
            if (m_EssBmsServiceProxy != null)
            {
                if (m_EssBmsServiceProxy.GetServerStatus())
                {
                    clsEssBmsMsg msg = new clsEssBmsMsg();
                    int nIndex = m_nSelectedMeter;

                    msg.msgType = clsConstant.ESSBMS_CMD_DCLINK_ON;
                    msg.msgIndex = 0;
                    msg.msgDateTime = DateTime.Now;
                    msg.msgSession = "";
                    msg.msgResult = 0;
                    msg.msgDevice = m_EssBmsServiceProxy.GetSetting(nIndex).nDeviceNo;
                    msg.msgDeviceName = m_EssBmsServiceProxy.GetSetting(nIndex).strDevice;

                    m_EssBmsServiceProxy.SetCmd(msg);
                }
            }
        }

        private void btnDcLinkOff_Click(object sender, EventArgs e)
        {
            if (m_EssBmsServiceProxy != null)
            {
                if (m_EssBmsServiceProxy.GetServerStatus())
                {
                    clsEssBmsMsg msg = new clsEssBmsMsg();
                    int nIndex = m_nSelectedMeter;

                    msg.msgType = clsConstant.ESSBMS_CMD_DCLINK_OFF;
                    msg.msgIndex = 0;
                    msg.msgDateTime = DateTime.Now;
                    msg.msgSession = "";
                    msg.msgResult = 0;
                    msg.msgDevice = m_EssBmsServiceProxy.GetSetting(nIndex).nDeviceNo;
                    msg.msgDeviceName = m_EssBmsServiceProxy.GetSetting(nIndex).strDevice;

                    m_EssBmsServiceProxy.SetCmd(msg);
                }
            }
        }

        private void btnBmsFaultReset_Click(object sender, EventArgs e)
        {
            if (m_EssBmsServiceProxy != null)
            {
                if (m_EssBmsServiceProxy.GetServerStatus())
                {
                    clsEssBmsMsg msg = new clsEssBmsMsg();
                    int nIndex = m_nSelectedMeter;

                    msg.msgType = clsConstant.ESSBMS_CMD_RESET;
                    msg.msgIndex = 0;
                    msg.msgDateTime = DateTime.Now;
                    msg.msgSession = "";
                    msg.msgResult = 0;
                    msg.msgDevice = m_EssBmsServiceProxy.GetSetting(nIndex).nDeviceNo;
                    msg.msgDeviceName = m_EssBmsServiceProxy.GetSetting(nIndex).strDevice;

                    m_EssBmsServiceProxy.SetCmd(msg);
                }
            }
        }

        private void btnBmsWatchdog_Click(object sender, EventArgs e)
        {
            if (m_EssBmsServiceProxy != null)
            {
                if (m_EssBmsServiceProxy.GetServerStatus())
                {
                    clsEssBmsMsg msg = new clsEssBmsMsg();
                    int nIndex = m_nSelectedMeter;

                    msg.msgType = clsConstant.ESSBMS_CMD_WATCHDOG;
                    msg.msgIndex = 0;
                    msg.msgDateTime = DateTime.Now;
                    msg.msgSession = "";
                    msg.msgResult = 0;
                    msg.msgDevice = m_EssBmsServiceProxy.GetSetting(nIndex).nDeviceNo;
                    msg.msgDeviceName = m_EssBmsServiceProxy.GetSetting(nIndex).strDevice;

                    m_EssBmsServiceProxy.SetCmd(msg);
                }
            }
        }

        private void btnPcsSet_Click(object sender, EventArgs e)
        {
            if ((C1.Win.Ribbon.RibbonButton)sender == btnPcsSet1)
            {
                m_UdpClient.m_strRtuIp[0] = txtRtuIpAddress1.Text;
                m_UdpClient.m_nPcsId[0] = Int32.Parse(txtPcsId1.Text);
                m_UdpClient.m_nPcsPeriod[0] = (Int32)numPcsPeriod1.Value;

                m_UdpClient.runPcsTimer(1);
            }
            else if ((C1.Win.Ribbon.RibbonButton)sender == btnPcsSet2)
            {
                m_UdpClient.m_strRtuIp[1] = txtRtuIpAddress2.Text;
                m_UdpClient.m_nPcsId[1] = Int32.Parse(txtPcsId2.Text);
                m_UdpClient.m_nPcsPeriod[1] = (Int32)numPcsPeriod2.Value;

                m_UdpClient.runPcsTimer(2);
            }
            else if ((C1.Win.Ribbon.RibbonButton)sender == btnPcsSet3)
            {
                m_UdpClient.m_strRtuIp[2] = txtRtuIpAddress3.Text;
                m_UdpClient.m_nPcsId[2] = Int32.Parse(txtPcsId3.Text);
                m_UdpClient.m_nPcsPeriod[2] = (Int32)numPcsPeriod3.Value;

                m_UdpClient.runPcsTimer(3);
            }
        }

        private void btnPcsOut_Click(object sender, EventArgs e)
        {
            if ((C1.Win.Ribbon.RibbonButton)sender == btnPcsOut1)
            {
                m_UdpClient.m_nPcsOut[0] = (Int32)numPcsOut1.Value;

                m_UdpClient.runPcsTimer(1);
            }
            else if ((C1.Win.Ribbon.RibbonButton)sender == btnPcsOut2)
            {
                m_UdpClient.m_nPcsOut[1] = (Int32)numPcsOut2.Value;

                m_UdpClient.runPcsTimer(2);
            }
            else if ((C1.Win.Ribbon.RibbonButton)sender == btnPcsOut3)
            {
                m_UdpClient.m_nPcsOut[2] = (Int32)numPcsOut3.Value;

                m_UdpClient.runPcsTimer(3);
            }
        }

        private void btnPcsRun_Click(object sender, EventArgs e)
        {
            if ((C1.Win.Ribbon.RibbonButton)sender == btnPcsRun1)
            {
                m_UdpClient.runPcs(1, (Int32)numWaitPcs1.Value);
            }
            else if ((C1.Win.Ribbon.RibbonButton)sender == btnPcsRun2)
            {
                m_UdpClient.runPcs(2, (Int32)numWaitPcs2.Value);
            }
            else if ((C1.Win.Ribbon.RibbonButton)sender == btnPcsRun3)
            {
                m_UdpClient.runPcs(3, (Int32)numWaitPcs3.Value);
            }
        }

        private void btnPcsStop_Click(object sender, EventArgs e)
        {
            if ((C1.Win.Ribbon.RibbonButton)sender == btnPcsStop1)
            {
                m_UdpClient.stopPcs(1, (Int32)numWaitPcs1.Value);
            }
            else if ((C1.Win.Ribbon.RibbonButton)sender == btnPcsStop2)
            {
                m_UdpClient.stopPcs(2, (Int32)numWaitPcs2.Value);
            }
            else if ((C1.Win.Ribbon.RibbonButton)sender == btnPcsStop3)
            {
                m_UdpClient.stopPcs(3, (Int32)numWaitPcs3.Value);
            }
        }
    }
}
