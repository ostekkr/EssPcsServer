using C1.Win.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EssPcsService;

namespace EssPcsServer
{
    public partial class frmMain
    {
        public List<clsGroupInfo> m_GroupListInfo = new List<clsGroupInfo>();
        public List<TreeNode> m_GroupListNode = new List<TreeNode>();
        public List<TreeNode> m_DeviceListNode = new List<TreeNode>();
        public List<TreeNode> m_meterListNode = new List<TreeNode>();
        public TreeNode[] m_meterStatusNode = new TreeNode[3];

        private void setEssControlInfo()
        {
            DateTime dt;
            DateTime dtNow = DateTime.Now;
            int nHour, nMin;
            
            //  충전 시작 시각
            nHour = Int32.Parse(numEssChargeStartTimeHour.Value.ToString());
            nMin = Int32.Parse(numEssChargeStartTimeMin.Value.ToString());
            dt = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, nHour, nMin, 0);
            m_EssPcsServiceProxy.setDtEssChargeStart(dt, false);
            //  충전 종료 시각
            nHour = Int32.Parse(numEssChargeEndTimeHour.Value.ToString());
            nMin = Int32.Parse(numEssChargeEndTimeMin.Value.ToString());
            dt = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, nHour, nMin, 0);
            m_EssPcsServiceProxy.setDtEssChargeEnd(dt, false);
            //  충전 SOC 제한 값
            m_EssPcsServiceProxy.setEssChargeSocLimit(Single.Parse(numEssChargeSocLimit.Text.ToString()), false);
            //  충전 출력 제한
            m_EssPcsServiceProxy.setEssChargeOutput(Single.Parse(numEssChargeOutputLimit.Text.ToString()), false);

            //  충전 시작 시각
            nHour = Int32.Parse(numEssDischargeStartTimeHour.Value.ToString());
            nMin = Int32.Parse(numEssDischargeStartTimeMin.Value.ToString());
            dt = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, nHour, nMin, 0);
            m_EssPcsServiceProxy.setDtEssDischargeStart(dt, false);
            //  충전 종료 시각
            nHour = Int32.Parse(numEssDischargeEndTimeHour.Value.ToString());
            nMin = Int32.Parse(numEssDischargeEndTimeMin.Value.ToString());
            dt = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, nHour, nMin, 0);
            m_EssPcsServiceProxy.setDtEssDischargeEnd(dt, false);
            //  충전 SOC 제한 값
            m_EssPcsServiceProxy.setEssDischargeSocLimit(Single.Parse(numEssDischargeSocLimit.Text.ToString()), false);
            //  충전 출력 제한
            m_EssPcsServiceProxy.setEssDischargeOutput(Single.Parse(numEssDischargeOutputLimit.Text.ToString()), false);

            //  발전량 연계
            m_EssPcsServiceProxy.setEssGen(chkEssGen.Checked, false);
            //  발전 전력
            m_EssPcsServiceProxy.setEssGenOutput(Single.Parse(numEssGenOutPower.Text.ToString()), false);
            //  충전율
            m_EssPcsServiceProxy.setEssGenPercent(Single.Parse(numEssGenOutPercent.Text.ToString()), false);

            //  제어 주기
            m_EssPcsServiceProxy.setEssControlPeriod(Int32.Parse(numEssControlPeriod.Text.ToString()), false);

            //  EMS RUN/STOP
            m_EssPcsServiceProxy.setEmsRun(m_bIsEmsRun ? 1 : 0, false);
        }

        private void checkEssControlInfo()
        {
            DateTime dt;
            float fVal;
            bool bSet = false;
            bool bVal = false;
            int nHour, nMin, nVal;

            //  충전 시작 시각
            dt = m_EssPcsServiceProxy.getDtEssChargeStart(ref bSet);
            if (bSet)
            {
                numEssChargeStartTimeHour.Value = dt.Hour;
                numEssChargeStartTimeMin.Value = dt.Minute;
                m_EssPcsServiceProxy.clearDtEssChargeStart();
            }
            //  충전 종료 시각
            dt = m_EssPcsServiceProxy.getDtEssChargeEnd(ref bSet);
            if (bSet)
            {
                numEssChargeEndTimeHour.Value = dt.Hour;
                numEssChargeEndTimeMin.Value = dt.Minute;
                m_EssPcsServiceProxy.clearDtEssChargeEnd();
            }
            //  충전 SOC 제한 값
            fVal = m_EssPcsServiceProxy.getEssChargeSocLimit(ref bSet);
            if (bSet)
            {
                numEssChargeSocLimit.Text = String.Format("{0:F1}", fVal);
                m_EssPcsServiceProxy.clearEssChargeSocLimit();
            }
            //  충전 출력 제한
            fVal = m_EssPcsServiceProxy.getEssChargeOutput(ref bSet);
            if (bSet)
            {
                numEssChargeOutputLimit.Text = String.Format("{0:F1}", fVal);
                m_EssPcsServiceProxy.clearEssChargeOutput();
            }

            //  방전 시작 시각
            dt = m_EssPcsServiceProxy.getDtEssDischargeStart(ref bSet);
            if (bSet)
            {
                numEssDischargeStartTimeHour.Value = dt.Hour;
                numEssDischargeStartTimeMin.Value = dt.Minute;
                m_EssPcsServiceProxy.clearDtEssDischargeStart();
            }
            //  방전 종료 시각
            dt = m_EssPcsServiceProxy.getDtEssDischargeEnd(ref bSet);
            if (bSet)
            {
                numEssDischargeEndTimeHour.Value = dt.Hour;
                numEssDischargeEndTimeMin.Value = dt.Minute;
                m_EssPcsServiceProxy.clearDtEssDischargeEnd();
            }
            //  방전 SOC 제한 값
            fVal = m_EssPcsServiceProxy.getEssDischargeSocLimit(ref bSet);
            if (bSet)
            {
                numEssDischargeSocLimit.Text = String.Format("{0:F1}", fVal);
                m_EssPcsServiceProxy.clearEssDischargeSocLimit();
            }
            //  방전 출력 제한
            fVal = m_EssPcsServiceProxy.getEssDischargeOutput(ref bSet);
            if (bSet)
            {
                numEssDischargeOutputLimit.Text = String.Format("{0:F1}", fVal);
                m_EssPcsServiceProxy.clearEssChargeOutput();
            }

            //  발전량 연계
            bVal = m_EssPcsServiceProxy.getEssGen(ref bSet);
            if (bSet)
            {
                chkEssGen.Checked = bVal;
                m_EssPcsServiceProxy.clearEssGen();
            }

            //  발전 전력
            fVal = m_EssPcsServiceProxy.getEssGenOutput(ref bSet);
            if (bSet)
            {
                numEssGenOutPower.Text = String.Format("{0:F1}", fVal);
                m_EssPcsServiceProxy.clearEssGenOutput();
            }
            //  충전율
            fVal = m_EssPcsServiceProxy.getEssGenPercent(ref bSet);
            if (bSet)
            {
                numEssGenOutPercent.Text = String.Format("{0:F1}", fVal);
                m_EssPcsServiceProxy.clearEssGenPercent();
            }

            //  제어 주기
            nVal = m_EssPcsServiceProxy.getEssControlPeriod(ref bSet);
            if (bSet)
            {
                numEssControlPeriod.Text = nVal.ToString();
                m_EssPcsServiceProxy.clearEssControlPeriod();
            }

            //  EMS RUN/STOP
            nVal = m_EssPcsServiceProxy.getEmsRun(ref bSet);
            if (bSet)
            {
                if (m_bServerRun)
                {
                    if (nVal == 1)
                    {
                        m_bIsEmsRun = true;
                    }
                    else
                    {
                        m_bIsEmsRun = false;

                        if (m_rtuClient[0] != null)
                        {
                            m_EssPcsServiceProxy.setInvStop(0, true);
                        }
                    }
                }

                m_EssPcsServiceProxy.clearEmsRun();
            }
        }

        public void Setting_Get()
        {
            /*
            int nNO = toolStripComboSettingMeterId.SelectedIndex;

            g_clsSqlServer.m_clsSettingInfo[nNO].strLOC = textSettingLoc.Text;
            g_clsSqlServer.m_clsSettingInfo[nNO].strCOMMENT = textSettingComment.Text;
            g_clsSqlServer.m_clsSettingInfo[nNO].bUSED = checkSettingUsed.Checked;

//            g_clsSqlServer.m_clsSettingInfo[nNO].nPTRATE = comboSettingPtRate.SelectedIndex;
//            g_clsSqlServer.m_clsSettingInfo[nNO].nCTRATE = comboSettingCtRate.SelectedIndex;
            g_clsSqlServer.m_clsSettingInfo[nNO].fPTRATIO = Convert.ToSingle(textSettingAiChName.Text.ToString());
//            g_clsSqlServer.m_clsSettingInfo[nNO].nCTRATIO = Convert.ToInt32(textSettingCtRatio.Text.ToString());
//            g_clsSqlServer.m_clsSettingInfo[nNO].nWIREMODE = comboSettingWireMode.SelectedIndex;

//            g_clsSqlServer.m_clsSettingInfo[nNO].bPQEN = comboSettingPqEnable.SelectedIndex == 1 ? true : false;
//            g_clsSqlServer.m_clsSettingInfo[nNO].nDEMAND = comboSettingDemand.SelectedIndex;

            comboSettingFreq.SelectedIndex = g_clsSqlServer.m_clsSettingInfo[nNO].nFREQ;
//            comboSettingSampleRate.SelectedIndex = g_clsSqlServer.m_clsSettingInfo[nNO].nSAMPLERATE;

            g_clsSqlServer.m_clsSettingInfo[nNO].nIPPORT = Int32.Parse(textSettingIpPort.Text);
            g_clsSqlServer.m_clsSettingInfo[nNO].nMODBUS = Int32.Parse(textSettingPmuTcp.Text);
            g_clsSqlServer.m_clsSettingInfo[nNO].strPASSWORD = textSettingPmuUdp.Text;

            g_clsSqlServer.m_clsSettingInfo[nNO].strIPADDRESS = textSettingIpAddress.Text;
            g_clsSqlServer.m_clsSettingInfo[nNO].strGWADDRESS = textSettingGatewayAddress.Text;
            g_clsSqlServer.m_clsSettingInfo[nNO].strSUBNETMASK = textSettingSubnetMask.Text;

//            g_clsSqlServer.m_clsSettingInfo[nNO].nWAVELET = comboSettingWaveletEnable.SelectedIndex;
            g_clsSqlServer.m_clsSettingInfo[nNO].nWAVELETLEVEL = comboSettingWifiEncryption.SelectedIndex;

//            g_clsSqlServer.m_clsSettingInfo[nNO].nDICONFIG1 = comboSettingDiConfig1.SelectedIndex;
//            g_clsSqlServer.m_clsSettingInfo[nNO].nDICONFIG2 = comboSettingDiConfig2.SelectedIndex;
//            g_clsSqlServer.m_clsSettingInfo[nNO].nDICONFIG3 = comboSettingDiConfig3.SelectedIndex;
//            g_clsSqlServer.m_clsSettingInfo[nNO].nDICONFIG4 = comboSettingDiConfig4.SelectedIndex;
//            g_clsSqlServer.m_clsSettingInfo[nNO].nRYCONFIG1 = comboSettingRyConfig1.SelectedIndex;
//            g_clsSqlServer.m_clsSettingInfo[nNO].nRYCONFIG2 = comboSettingRyConfig2.SelectedIndex;

            g_clsSqlServer.m_clsSettingInfo[nNO].strDevice = textSettingDevice.Text;
            g_clsSqlServer.m_clsSettingInfo[nNO].nGroup = getGroupNo(comboSettingGroup.SelectedIndex);
//            g_clsSqlServer.m_clsSettingInfo[nNO].nSECTOR = comboSettingSamples.SelectedIndex;
//            g_clsSqlServer.m_clsSettingInfo[nNO].nPTCycle = comboSettingPreTirrgerCycle.SelectedIndex;
//            g_clsSqlServer.m_clsSettingInfo[nNO].nFPDelay = comboSettingFaultPickupDelay.SelectedIndex;
//            g_clsSqlServer.m_clsSettingInfo[nNO].nVLLimit = comboSettingVoltageLowLimit.SelectedIndex;
//            g_clsSqlServer.m_clsSettingInfo[nNO].nVHLimit = comboSettingVoltageHighLimit.SelectedIndex;
//            g_clsSqlServer.m_clsSettingInfo[nNO].nPTSHour = int.Parse(numericSettingPulseTimeSyncHour.Value.ToString());
//            g_clsSqlServer.m_clsSettingInfo[nNO].nPTSMinute = int.Parse(numericSettingPulseTimeSyncMinute.Value.ToString());
            */ 
        }

        private void readGroupList()
        {
            clsTblGroup m_tblGroup = new clsTblGroup();

            m_GroupListInfo.Clear();
            m_EssPcsServiceProxy.ClearGroup();

            if (m_tblGroup.Open())
            {
                if (m_tblGroup.Read())
                {
                    int i;

                    for (i = 0; i < m_tblGroup.m_GroupList.Count; i++)
                    {
                        clsGroupInfo groupInfo = new clsGroupInfo();

                        groupInfo = m_tblGroup.m_GroupList[i];
                        m_GroupListInfo.Add(groupInfo);
                        m_EssPcsServiceProxy.AddGroup(groupInfo);
                    }
                }

                m_tblGroup.Close();
            }
        }

        private void displayMeterList(bool bRefreshALL = true)
        {
            C1TopicPage topicPage;
            C1TopicLink topicLink;
            String strMeter;
            String strLog;
            int i, index;

            c1TopicBarDevice.Pages.Clear();

            strLog = g_clsSqlServer.Setting_Make();

            if (strLog != "")
            {
                m_SystemLogList.Add(new clsSystemLog(-1, strLog, DateTime.Now));
                MessageBox.Show(strLog);
            }

            if (bRefreshALL)
            {
                m_rtuClient.Clear();

                for (i = 0; i < g_clsSqlServer.m_clsSettingInfo.Count; i++)
                {
                    clsRtuClient rtuClient = new clsRtuClient(i, g_clsSqlServer.m_clsSettingInfo[i], g_clsSqlServer.m_clsTrendInfo[i], false);

                    m_rtuClient.Add(rtuClient);
                }
            }

            for (i = 0; i < g_clsSqlServer.m_clsSettingInfo.Count; i++)
            {
                clsEssPcsSettingInfo info = g_clsSqlServer.m_clsSettingInfo[i];

                strMeter = info.strDevice;

                index = 10;

                if (m_rtuClient.Count > 0)
                {
                    if (m_rtuClient[i] != null)
                    {
                        if (m_rtuClient[i].m_TrendInfo.nConnectionStatus == clsConstant.PCS_RTU_CONNECTED)
                        {
                            index = 12;
                        }
                        else if (m_rtuClient[i].m_TrendInfo.nConnectionStatus == clsConstant.PCS_RTU_CONNECTION_FAILED)
                        {
                            index = 13;
                        }
                        else if (m_rtuClient[i].m_TrendInfo.nConnectionStatus == clsConstant.PCS_RTU_CONNECTING)
                        {
                            index = 14;
                        }
                    }
                }

                topicPage = new C1TopicPage(" " + info.strDevice, info.strCOMMENT, index);
                topicPage.Tag = info.nDeviceNo.ToString();
                topicPage.Collapse();
                c1TopicBarDevice.Pages.Add(topicPage);
            }
        }

        private void addToGroupList(int nGroup, C1TopicLink topicLink)
        {
            C1TopicPage topicPage;
            String strTag;

            strTag = "G" + nGroup.ToString();

            topicPage = c1TopicBarDevice.FindPageByTag(strTag);

            if(topicPage != null)
            {
                topicPage.Links.Add(topicLink);
            }
            else
            {
                c1TopicBarDevice.Pages[c1TopicBarDevice.Pages.Count - 1].Links.Add(topicLink);
            }
        }

        private int getGroupNo(int index)
        {
            int no = -1;

            if(m_GroupListInfo.Count > index)
            {
                if(index >= 0)
                {
                    no = m_GroupListInfo[index].nGroup;
                }
            }

            return no;
        }

        private int getGroupIndex(int no)
        {
            int index = 0;

            foreach (clsGroupInfo groupInfo in m_GroupListInfo)
            {
                if (groupInfo.nGroup == no)
                {
                    break;
                }
                index++;
            }

            if (index == m_GroupListInfo.Count)
            {
                index = -1;
            }

            return index;
        }

        private String getGroupName(int no)
        {
            String strGroup = "그룹 없음";

            foreach (clsGroupInfo groupInfo in m_GroupListInfo)
            {
                if (groupInfo.nGroup == no)
                {
                    strGroup = groupInfo.strGroup;
                    break;
                }
            }

            return strGroup;
        }

        private int getMeterNo(int index)
        {
            int nDeviceNo = -1;

            if (g_clsSqlServer.m_clsSettingInfo.Count > index)
            {
                if (index >= 0)
                {
                    nDeviceNo = g_clsSqlServer.m_clsSettingInfo[index].nDeviceNo;
                }
            }

            return nDeviceNo;
        }

        private int getMeterIndex(int no)
        {
            int index = 0;

            foreach (clsEssPcsSettingInfo settingInfo in g_clsSqlServer.m_clsSettingInfo)
            {
                if (settingInfo.nDeviceNo == no)
                {
                    break;
                }
                index++;
            }

            if (index == g_clsSqlServer.m_clsSettingInfo.Count)
            {
                index = -1;
            }

            return index;
        }

        private void setRelay(int nRelay, bool bOp)
        {
            /*
            int nNO = toolStripComboSettingMeterId.SelectedIndex;

            if (nRelay == 1)
            {
                if (bOp)
                {
                    m_tcpClient[nNO].Relay1On();
                }
                else
                {
                    m_tcpClient[nNO].Relay1Off();
                }
            }
            else if (nRelay == 2)
            {
                if (bOp)
                {
                    m_tcpClient[nNO].Relay2On();
                }
                else
                {
                    m_tcpClient[nNO].Relay2Off();
                }
            }
            */
        }

        private void Setting_Default()
        {
            /*
            if (comboSettingGroup.Items.Count > 0)
            {
                comboSettingGroup.SelectedIndex = 0;
            }
            else
            {
                comboSettingGroup.SelectedIndex = -1;
            }

            textSettingDevice.Text = "";
            textSettingLoc.Text = "";
            textSettingComment.Text = "";

            checkSettingUsed.Enabled = true;

//            comboSettingPtRate.SelectedIndex = 0;
//            comboSettingCtRate.SelectedIndex = 0;
//            comboSettingWireMode.SelectedIndex = 0;

            textSettingAiChName.Text = "1.0";
//            textSettingCtRatio.Text = "1";

//            comboSettingPqEnable.SelectedIndex = 0;
//            comboSettingDemand.SelectedIndex = 0;

            comboSettingFreq.SelectedIndex = 0;
//            comboSettingSampleRate.SelectedIndex = 0;

            textSettingIpPort.Text = "0.0.0.0";
            textSettingGatewayAddress.Text = "0.0.0.0";
            textSettingSubnetMask.Text = "255.255.255.0";

            textSettingIpPort.Text = "502";
            textSettingPmuTcp.Text = "1";
            textSettingPmuUdp.Text = "1234";

//            comboSettingWaveletEnable.SelectedIndex = 0;
            comboSettingWifiEncryption.SelectedIndex = 0;

//            comboSettingDiConfig1.SelectedIndex = 0;
//            comboSettingDiConfig2.SelectedIndex = 0;
//            comboSettingDiConfig3.SelectedIndex = 0;
//            comboSettingDiConfig4.SelectedIndex = 0;
//            comboSettingRyConfig1.SelectedIndex = 0;
//            comboSettingRyConfig2.SelectedIndex = 0;

//            comboSettingSamples.SelectedIndex = 0;
//            comboSettingPreTirrgerCycle.SelectedIndex = 0;
//            comboSettingFaultPickupDelay.SelectedIndex = 0;
//            comboSettingVoltageLowLimit.SelectedIndex = 0;
//            comboSettingVoltageHighLimit.SelectedIndex = 0;
//            numericSettingPulseTimeSyncHour.Value = 0;
//            numericSettingPulseTimeSyncMinute.Value = 0;
            */
        }

        void removeFromGroupNode(TreeNode treeNode)
        {
            foreach (TreeNode tNode in m_GroupListNode)
            {
                if (tNode.Nodes.Contains(treeNode))
                {
                    tNode.Nodes.Remove(treeNode);
                }
            }
        }

        void addToGroupNode(int nGroup, TreeNode treeNode)
        {
            foreach (TreeNode nodeGroup in m_GroupListNode)
            {
                String strTag = nodeGroup.Tag.ToString();
                int n = int.Parse(strTag.Remove(0, 1));

                if (n == nGroup)
                {
                    nodeGroup.Nodes.Add(treeNode);
                    return;
                }
            }

            ((TreeNode)m_GroupListNode[m_GroupListNode.Count - 1]).Nodes.Add(treeNode);
        }

        void updateMeterList(int index)
        {
            /*
            toolStripComboSettingMeterId.Items[index].Ribbon.Text = g_clsSqlServer.m_clsSettingInfo[index].strDevice + " [" + getGroupName(g_clsSqlServer.m_clsSettingInfo[index].nGroup) + "]";
            selSearchDevice.Items[index + 1] = g_clsSqlServer.m_clsSettingInfo[index].strDevice + " [" + getGroupName(g_clsSqlServer.m_clsSettingInfo[index].nGroup) + "]";
            m_meterListNode[index].Text = g_clsSqlServer.m_clsSettingInfo[index].strDevice;
            removeFromGroupNode(m_meterListNode[index]);
            addToGroupNode(g_clsSqlServer.m_clsSettingInfo[index].nGroup, m_meterListNode[index]);
            */
        }

        bool removeMeterList(int index)
        {
            //String strReult = "";
            bool bResult = true;
            /*
            strReult = g_clsSqlServer.Setting_Remove(index);

            if (strReult == "")
            {
                closeSocket(index);
                m_tcpClient.RemoveAt(index);
                toolStripComboSettingMeterId.Items.RemoveAt(index);
                selSearchDevice.Items.RemoveAt(index + 1);
                m_meterListNode[index].Remove();

                if (toolStripComboSettingMeterId.Items.Count > 0)
                {
                    toolStripComboSettingMeterId.SelectedIndex = 0;
                }
                else
                {
                    Setting_Default();
                }
            }
            else
            {
                bResult = false;
            }
            */
            return bResult;
        }
    }
}
