using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EssPcsService;
using C1.Win.Command;

namespace EssPcsServer
{
    public partial class frmMain
    {
        private void esspcsCmdPeriodSet(clsEssPcsMsg msg)
        {
            UInt32 nDeviceSyncTime = 0;
            UInt32 nManualTriggerTime = 0;

            m_EssPcsServiceProxy.GetPeriod(ref nDeviceSyncTime, ref nManualTriggerTime);

            Properties.Settings.Default.sitePort1 = nDeviceSyncTime;
            numericDbServerPort1.Value = nDeviceSyncTime;
            Properties.Settings.Default.Save();

            msg.msgResult = 0;

            m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
        }

        private void esspcsCmdErrorInit(clsEssPcsMsg msg)
        {
            int i;

            i = g_clsSqlServer.GetDeviceIndex(msg.msgDevice);

            if (i > -1)
            {
                msg.msgResult = 0;
                msg.msgDeviceName = g_clsSqlServer.m_clsSettingInfo[i].strDevice;

                if (m_rtuClient[i] != null)
                {
                    m_rtuClient[i].m_nErrorCount = 0;
                    m_EssPcsServiceProxy.SetErrorCount(msg.msgDevice, 0);
                    msg.msgDateTime = DateTime.Now;
                }
                else
                {
                    //  장치 접속 오류
                    msg.msgResult = -2;
                }
            }
            else
            {
                //  장치 정보 없음
                msg.msgResult = -1;
            }

            m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
        }

        private void esspcsCmdTimeGet(clsEssPcsMsg msg)
        {
            int i;

            i = g_clsSqlServer.GetDeviceIndex(msg.msgDevice);

            if (i > -1)
            {
                msg.msgResult = 0;
                msg.msgDeviceName = g_clsSqlServer.m_clsSettingInfo[i].strDevice;

                if (m_rtuClient[i] != null)
                {
                    lock (m_rtuClient[i].MsgListLock) { m_rtuClient[i].m_EssPcsMsgList.Add(msg); }
                }
                else
                {
                    //  장치 접속 오류
                    msg.msgResult = -2;

                    m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
                }
            }
            else
            {
                //  장치 정보 없음
                msg.msgResult = -1;

                m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
            }
        }

        private void esspcsCmdTimeSync(clsEssPcsMsg msg)
        {
            int i;

            i = g_clsSqlServer.GetDeviceIndex(msg.msgDevice);

            if (i > -1)
            {
                msg.msgResult = 0;
                msg.msgDeviceName = g_clsSqlServer.m_clsSettingInfo[i].strDevice;

                if (m_rtuClient[i] != null)
                {
                    lock (m_rtuClient[i].MsgListLock) { m_rtuClient[i].m_EssPcsMsgList.Add(msg); }
                }
                else
                {
                    //  장치 접속 오류
                    msg.msgResult = -2;
                    m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
                }
            }
            else
            {
                //  장치 정보 없음
                msg.msgResult = -1;
                m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
            }
        }

        private void esspcsCmdInfoGet(clsEssPcsMsg msg)
        {
            int i;

            i = g_clsSqlServer.GetDeviceIndex(msg.msgDevice);

            if (i > -1)
            {
                msg.msgResult = 0;
                msg.msgDeviceName = g_clsSqlServer.m_clsSettingInfo[i].strDevice;

                if (m_rtuClient[i] != null)
                {
                    lock (m_rtuClient[i].MsgListLock) { m_rtuClient[i].m_EssPcsMsgList.Add(msg); }
                }
                else
                {
                    //  장치 접속 오류
                    msg.msgResult = -2;
                    m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
                }
            }
            else
            {
                //  장치 정보 없음
                msg.msgResult = -1;
                m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
            }
        }

        private void esspcsCmdInfoSet(clsEssPcsMsg msg)
        {
            int i;

            i = g_clsSqlServer.GetDeviceIndex(msg.msgDevice);

            if (i > -1)
            {
                msg.msgResult = 0;
                msg.msgDeviceName = g_clsSqlServer.m_clsSettingInfo[i].strDevice;

                if (m_rtuClient[i] != null)
                {
                    lock (m_rtuClient[i].MsgListLock) { m_rtuClient[i].m_EssPcsMsgList.Add(msg); }
                }
                else
                {
                    //  장치 접속 오류
                    msg.msgResult = -2;
                    m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
                }
            }
            else
            {
                //  장치 정보 없음
                msg.msgResult = -1;
                m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
            }
        }

        private void esspcsCmdFaultLoc(clsEssPcsMsg msg)
        {
            int i;

            i = g_clsSqlServer.GetDeviceIndex(msg.msgDevice);

            if (i > -1)
            {
                msg.msgResult = 0;
                msg.msgDeviceName = g_clsSqlServer.m_clsSettingInfo[i].strDevice;

                if (m_rtuClient[i] != null)
                {
                    lock (m_rtuClient[i].MsgListLock) { m_rtuClient[i].m_EssPcsMsgList.Add(msg); }
                }
                else
                {
                    //  장치 접속 오류
                    msg.msgResult = -2;
                    m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
                }
            }
            else
            {
                //  장치 정보 없음
                msg.msgResult = -1;
                m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
            }
        }

        private void esspcsCmdInfoUpdate(clsEssPcsMsg msg)
        {
            g_clsSqlServer.m_clsSettingInfo[msg.msgDevice] = m_EssPcsServiceProxy.GetSetting(msg.msgDevice);

            m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, "설정 데이터 업데이트", DateTime.Now));
        }

        private void esspcsCmdControl(clsEssPcsMsg msg)
        {
            int i;

            i = g_clsSqlServer.GetDeviceIndex(msg.msgDevice);

            if (i > -1)
            {
                msg.msgResult = 0;
                msg.msgDeviceName = g_clsSqlServer.m_clsSettingInfo[i].strDevice;

                if (m_rtuClient[i] != null)
                {
                    String strLog = "";

                    m_rtuClient[i].cancelWave();

                    lock (m_rtuClient[i].MsgListLock) { m_rtuClient[i].m_EssPcsMsgList.Add(msg); }

                    if (msg.msgType == clsConstant.ESSPCS_CMD_MEMORY_INIT)
                    {
                        strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "메모리 초기화 명령 송신");
                        m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
                    }
                    else if (msg.msgType == clsConstant.ESSPCS_CMD_REBOOT)
                    {
                        strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "재부팅 명령 송신");
                        m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
                    }
                    else if (msg.msgType == clsConstant.ESSPCS_CMD_MANUAL_TRIGGER)
                    {
                        strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "매뉴얼 트리거 명령 송신");
                        m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
                    }
                    else if (msg.msgType == clsConstant.ESSPCS_CMD_PREV_TRIGGER)
                    {
                        strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "이전 트리거 명령 송신");
                        m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
                    }
                    else if (msg.msgType == clsConstant.ESSPCS_CMD_CAL_START)
                    {
                        strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "캘리브레이션 시작 명령 송신");
                        m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
                    }
                    else if (msg.msgType == clsConstant.ESSPCS_CMD_CAL_STOP)
                    {
                        strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "캘리브레이션 결과 저장 명령 송신");
                        m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
                    }
                    else if (msg.msgType == clsConstant.ESSPCS_CMD_RELAY_CONTROL_ON)
                    {
                        strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "RELAY CONTROL ON 명령 송신");
                        m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
                    }
                    else if (msg.msgType == clsConstant.ESSPCS_CMD_RELAY_CONTROL_OFF)
                    {
                        strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "RELAY CONTROL OFF 명령 송신");
                        m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
                    }
                    else if (msg.msgType == clsConstant.ESSPCS_CMD_TRIGGER_LOCK_ON)
                    {
                        strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "TRIGGER LOCK ON 명령 송신");
                        m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
                    }
                    else if (msg.msgType == clsConstant.ESSPCS_CMD_TRIGGER_LOCK_OFF)
                    {
                        strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "TRIGGER LOCK OFF 명령 송신");
                        m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
                    }
                    else if (msg.msgType == clsConstant.ESSPCS_CMD_RELAY)
                    {
                        strLog = String.Format(" {0} > {1}", msg.msgDeviceName, "RELAY ON/OFF 명령 송신");
                        m_SystemLogList.Add(new clsSystemLog(msg.msgDevice, strLog, DateTime.Now));
                    }
                }
                else
                {
                    //  장치 접속 오류
                    msg.msgResult = -2;
                    m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
                }
            }
            else
            {
                //  장치 정보 없음
                msg.msgResult = -1;
                m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
            }
        }

        private void esspcsCmdWave(clsEssPcsMsg msg)
        {
            /*
            int i;

            i = g_clsSqlServer.GetDeviceIndex(msg.msgDevice);

            if (i > -1)
            {
                if (msg.msgType == clsConstant.ESSPCS_CMD_INS_INFO)
                {
                    //  순시 파형 데이터 인덱스 범위 오류
                    if ((msg.msgIndex < 1) || (msg.msgIndex > clsConstant.CDFR_INS_REC_SIZE))
                    {
                        msg.msgResult = 1;
                        m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
                        return;
                    }
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_PHA_INFO)
                {
                    //  페이저 파형 데이터 인덱스 범위 오류
                    if ((msg.msgIndex < 1) || (msg.msgIndex > clsConstant.CDFR_PHA_REC_SIZE))
                    {
                        msg.msgResult = 1;
                        m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
                        return;
                    }
                }

                msg.msgResult = 0;
                msg.msgDeviceName = g_clsSqlServer.m_clsSettingInfo[i].strDevice;

                if (m_tcpClient[i] != null)
                {
                    lock (m_tcpClient[i].MsgListLock) { m_tcpClient[i].m_EssPcsMsgList.Add(msg); }
                }
                else
                {
                    //  장치 접속 오류
                    msg.msgResult = -2;
                    m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
                }
            }
            else
            {
                //  장치 정보 없음
                msg.msgResult = -1;
                m_EssPcsServiceProxy.SendToClient(msg.msgSession, msg);
            }
            */
        }

        private void processEssPcsMsg(clsEssPcsMsg msg)
        {
            if (msg.msgType == clsConstant.ESSPCS_CMD_OUTPUT_SET)
            {
                //...esspcsCmdPeriodSet(msg);
            }
            /*
            else if (msg.msgType == clsConstant.ESSPCS_CMD_ERROR_INIT)
            {
                esspcsCmdErrorInit(msg);
            }
            */
            else if (msg.msgType == clsConstant.ESSPCS_CMD_TIME_GET)
            {
                esspcsCmdTimeGet(msg);
            }
            /*
            else if (msg.msgType == clsConstant.ESSPCS_CMD_TIME_SET)
            {
                esspcsCmdTimeSync(msg);
            }
            */
            else if (msg.msgType == clsConstant.ESSPCS_CMD_INFO_GET)
            {
                esspcsCmdInfoGet(msg);
            }
            /*
            else if (msg.msgType == clsConstant.ESSPCS_CMD_INFO_SET)
            {
                esspcsCmdInfoSet(msg);
            }
            */
            else if (msg.msgType == clsConstant.ESSPCS_CMD_INFO_UPDATE)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() => esspcsCmdInfoUpdate(msg)));
                }
                else
                {
                    esspcsCmdInfoUpdate(msg);
                }
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_MEMORY_INIT)
            {
                esspcsCmdControl(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_REBOOT)
            {
                esspcsCmdControl(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_MANUAL_TRIGGER)
            {
                esspcsCmdControl(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_PREV_TRIGGER)
            {
                esspcsCmdControl(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_CAL_START)
            {
                esspcsCmdControl(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_CAL_STOP)
            {
                esspcsCmdControl(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_RELAY_CONTROL_ON)
            {
                esspcsCmdControl(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_RELAY_CONTROL_OFF)
            {
                esspcsCmdControl(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_INS_WAVE)
            {
                esspcsCmdWave(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_PHA_WAVE)
            {
                esspcsCmdWave(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_TRIGGER_LOCK_ON)
            {
                esspcsCmdControl(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_TRIGGER_LOCK_OFF)
            {
                esspcsCmdControl(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_RELAY)
            {
                esspcsCmdControl(msg);
            }
            else if (msg.msgType == clsConstant.ESSPCS_CMD_FAULT_LOC)
            {
                esspcsCmdFaultLoc(msg);
            }
        }

        private void processInsWaveInfo(clsEssPcsMsg msg)
        {
            if (msg.msgResult == 0)
            {
                clsWaveInfo info = new clsWaveInfo(msg.msgDevice, g_clsSqlServer.m_clsSettingInfo[msg.msgDevice], msg.msgBuf, msg.msgIndex, 0);

                msg.msgString = info.m_strMsg;

                if (msg.msgSession == "")
                {
                    if (saveWaveInfo(info))
                    {
                        if (Properties.Settings.Default.saveData)
                        {
                            startInsFileReceive(msg.msgDevice, info);
                        }
                    }
                }
                else
                {
                    msg.msgString = "순시 사고 정보 요청 오류 발생";
                }
            }
        }

        private void processPhaWaveInfo(clsEssPcsMsg msg)
        {
            if (msg.msgResult == 0)
            {
                clsWaveInfo info = new clsWaveInfo(msg.msgDevice, g_clsSqlServer.m_clsSettingInfo[msg.msgDevice], msg.msgBuf, msg.msgIndex, 1);

                msg.msgString = info.m_strMsg;

                if (msg.msgSession == "")
                {
                    if (saveWaveInfo(info))
                    {
                        if (Properties.Settings.Default.saveData)
                        {
                            startPhaFileReceive(msg.msgDevice, info);
                        }
                    }
                }
                else
                {
                    msg.msgString = "페이저 사고 정보 요청 오류 발생";
                }
            }
        }
    }
}
