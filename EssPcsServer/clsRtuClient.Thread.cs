using EssPcsService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace EssPcsServer
{
    public partial class clsRtuClient
    {
        const int TCPIP_TIMEOUT = 3000;
        const int TREND_TIMEOUT = 100;
        const int TCPIP_RETRY_COUNT = 3;

        int m_nTimeSyncTick = Environment.TickCount;
        int m_nManualTriggerTick = Environment.TickCount;
        public bool m_bWaitInsFile = false;
        public bool m_bWaitPhaFile = false;
        public int m_nExpectedLength = 0;
        public int m_nExpectedCount = 0;
        public byte[] m_bExpectedBuffer = new byte[clsConstant.CDFR_ETHERNET_DATA_LENGTH];

        public Boolean m_bTopicPageUpdate;
        public String m_strTopicPage;
        public Int32 m_nTopicPageImage;
        public Boolean m_bPauseThread = true;
        public Boolean m_bThreadRun = false;
        public Boolean m_bReqTrendStop = false;

        private void setMsgFinishMark(Int32 nMsgType, DateTime dt)
        {
            CommandQueue.SetFinishMark(nMsgType, dt);
        }

        private void setMsgFinishMark(Int32 nMsgType, DateTime dt, Byte[] bMsgBuf, Int32 nMsgLen)
        {
            CommandQueue.SetFinishMark(nMsgType, dt, bMsgBuf, nMsgLen);
        }

        private void setMsgFail(Int32 nCommandType, DateTime dt)
        {
            /*
            Int32 nMsgType = -1;

            switch (nCommandType)
            {
                //  장치 메모리 초기화
                case clsConstant.CDFR_DEVICE_MEMORY_INIT: 
                    nMsgType = clsConstant.CDFR_CMD_MEMORY_INIT; 
                    break;

                //  장치 재부팅
                case clsConstant.CDFR_DEVICE_REBOOT: 
                    nMsgType = clsConstant.CDFR_CMD_REBOOT; 
                    break;

                //  이전 트리거로 이동
                case clsConstant.CDFR_DEVICE_PREV_TRIGGER: 
                    nMsgType = clsConstant.CDFR_CMD_PREV_TRIGGER; 
                    break;

                //  수동 트리거 발생
                case clsConstant.CDFR_DEVICE_MANUAL_TRIGGER: 
                    nMsgType = clsConstant.CDFR_CMD_MANUAL_TRIGGER; 
                    break;

                //  칼리브레이션 시작
                case clsConstant.CDFR_DEVICE_CAL_START: 
                    nMsgType = clsConstant.CDFR_CMD_CAL_START; 
                    break;

                //  칼리브레이션 종료
                case clsConstant.CDFR_DEVICE_CAL_STOP: 
                    nMsgType = clsConstant.CDFR_CMD_CAL_STOP; 
                    break;
                    
                //  장치 릴레이 설정 ON
                case clsConstant.CDFR_DEVICE_RELAY_CONTROL_ON: 
                    nMsgType = clsConstant.CDFR_CMD_RELAY_CONTROL_ON; 
                    break;

                //  장치 릴레이 설정 OFF
                case clsConstant.CDFR_DEVICE_RELAY_CONTROL_OFF: 
                    nMsgType = clsConstant.CDFR_CMD_RELAY_CONTROL_OFF; 
                    break;

                //  장치 LOCK ON
                case clsConstant.CDFR_DEVICE_TRIGGER_LOCK_ON: 
                    nMsgType = clsConstant.CDFR_CMD_TRIGGER_LOCK_ON; 
                    break;

                //  장치 LOCK OFF
                case clsConstant.CDFR_DEVICE_TRIGGER_LOCK_OFF: 
                    nMsgType = clsConstant.CDFR_CMD_TRIGGER_LOCK_OFF; 
                    break;

                //  장치 릴레이 설정
                case clsConstant.CDFR_DEVICE_RELAY: 
                    nMsgType = clsConstant.CDFR_CMD_RELAY; 
                    break;

                //  장치 시각 설정
                case clsConstant.CDFR_DEVICE_TIME_SET: 
                    nMsgType = clsConstant.CDFR_CMD_TIME_SET; 
                    break;

                //  장치 시각 읽어오기
                case clsConstant.CDFR_DEVICE_TIME_GET: 
                    nMsgType = clsConstant.CDFR_CMD_TIME_GET; 
                    break;
            
                //  장치 설정 정보 전송
                case clsConstant.CDFR_DEVICE_SEND_SETUP_DATA: 
                    nMsgType = clsConstant.CDFR_CMD_INFO_SET; 
                    break;

                //  장치 설정 정보 수신
                case clsConstant.CDFR_DEVICE_RECEIVE_SETUP_DATA: 
                    nMsgType = clsConstant.CDFR_CMD_INFO_GET; 
                    break;

                //  순시 사고 정보 읽어오기
                case clsConstant.CDFR_DEVICE_INS_INFO: 
                    nMsgType = clsConstant.CDFR_CMD_INS_INFO; 
                    break;

                //  페이저 사고 정보 읽어오기
                case clsConstant.CDFR_DEVICE_PHA_INFO: 
                    nMsgType = clsConstant.CDFR_CMD_PHA_INFO; 
                    break;

                //  순시 사고 파형 읽어오기
                case clsConstant.CDFR_DEVICE_INS_WAVE: 
                    nMsgType = clsConstant.CDFR_CMD_INS_WAVE; 
                    break;

                //  페이저 사고 파형 읽어오기
                case clsConstant.CDFR_DEVICE_PHA_WAVE: 
                    nMsgType = clsConstant.CDFR_CMD_PHA_WAVE; 
                    break;

                //  고장 거리 정보
                case clsConstant.CDFR_DEVICE_FAULT_LOC: 
                    nMsgType = clsConstant.CDFR_CMD_FAULT_LOC; 
                    break;

                //  장치 시각 읽어오기
                case clsConstant.CDFR_DEVICE_TREND_DATA_RECEIVE: 
                    nMsgType = clsConstant.CDFR_CMD_TIME_GET; 
                    break;
            }

            if (nMsgType > -1)
            {
                Int32 i;

                for (i = 0; i < m_EssPcsMsgList.Count; i++)
                {
                    if (m_EssPcsMsgList[i].msgType == nMsgType)
                    {
                        m_EssPcsMsgList[i].msgResult = -1;
                        m_EssPcsMsgList[i].msgDateTime = dt;
                        m_EssPcsMsgList[i].msgFinish = 1;
                    }
                }
            }
            */

            CommandQueue.SetFailByCommandType(nCommandType, dt);
        }

        public void removeMsgFromCommandType(Int32 nCommandType)
        {
            CommandQueue.MarkDoneByCommandType(nCommandType);
        }

        public void setSetupData(Byte[] bSetupBuf)
        {
            m_nReceiveByteCount = clsConstant.PCS_SETUP_DATA_NUM;
            Array.Copy(bSetupBuf, m_byteReceive, clsConstant.PCS_SETUP_DATA_NUM);
            parseSetupDataPacket();
            m_bUpdateSetting = true;
        }

        private void clearRequestMsg()
        {
            CommandQueue.MarkPendingAsDone();
        }

        private bool checkTimeSync()
        {
            if (Properties.Settings.Default.sitePort1 > 0)
            {
                if ((Math.Abs(m_nTimeSyncTick - Environment.TickCount) / 1000) > (Properties.Settings.Default.sitePort1 * 60))
                {
                    /*
                    clsEssPcsMsg msg = new clsEssPcsMsg();

                    msg.msgDevice = m_clsSettingInfo.nDeviceNo;
                    msg.msgType = clsConstant.ESSPCS_CMD_TIME_SET;
                    msg.msgIndex = 0;
                    msg.msgDateTime = DateTime.Now;
                    msg.msgSession = "";
                    msg.msgResult = 0;
                    msg.msgDeviceName = m_clsSettingInfo.strDevice;
                    m_EssPcsMsgList.Add(msg);
                    */
                    return true;
                }
            }
            /*
            if (Properties.Settings.Default.manualTriggerTime > 0)
            {
                if ((Math.Abs(m_nManualTriggerTick - Environment.TickCount) / 1000) > (Properties.Settings.Default.manualTriggerTime * 60))
                {
                    clsEssPcsMsg msg = new clsEssPcsMsg();

                    msg.msgDevice = m_clsSettingInfo.nDeviceNo;
                    msg.msgType = clsConstant.CDFR_CMD_MANUAL_TRIGGER;
                    msg.msgIndex = 0;
                    msg.msgDateTime = DateTime.Now;
                    msg.msgSession = "";
                    msg.msgResult = 0;
                    msg.msgDeviceName = m_clsSettingInfo.strDevice;
                    m_EssPcsMsgList.Add(msg);

                    m_nManualTriggerTick = Environment.TickCount;

                    return true;
                }
            }
            */
            return false;
        }

        public void setThreadStop()
        {

        }

        private void _ClientThreadFunc()
        {
            int nIndex = m_nIndex;
            int nWaitCnt = 0;

            m_bRun = true;
            m_bThreadRun = true;
            m_nMode = clsConstant.MODBUS_MODE_NONE;

            Debug.WriteLine(String.Format("{0} > _ClientThreadFunc : {1}", DateTime.Now.ToString(), m_nIndex));

            while (m_bRun)
            {
                System.Threading.Thread.Sleep(100);

                if (m_rtuClient != null)
                {
                    if (m_bDaqRun && m_rtuClient.IsOpen)
                    {
                        //...if (m_nMode == clsConstant.MODBUS_MODE_NONE)
                        {
                            if (CommandQueue.Count > 0)
                            {
                                sendMessage();

                                if (m_nSendLength > 0)
                                {
                                    Send(m_byteSend, m_nSendLength);
                                    m_swTrendData.Restart();
                                    CommandQueue.DequeueFirst();
                                    nWaitCnt = 0;
                                }
                            }

                            if (m_swTrendData.ElapsedMilliseconds > TREND_TIMEOUT)
                            {
                                //...if (m_nMode == clsConstant.MODBUS_MODE_NONE)
                                {
                                    receiveTrendData();

                                    if (!m_bReqTrendStop)
                                    {
                                        Send(m_clsModbus.g_bSendBuffer, m_nSendLength);
                                    }

                                    m_timeStart = Environment.TickCount;

                                    m_swTrendData.Restart();
                                }
                            }
                        }
                        /*
                        else
                        {
                            nWaitCnt++;

                            if (nWaitCnt > 5)
                            {
                                m_nMode = clsConstant.MODBUS_MODE_NONE;
                            }
                        }
                        */
                    }
                }
#if false
                if (m_bPauseThread) continue;

                if (!m_bProcessReceive && (m_socketClient != null))
                {
                    if ((m_nMode == clsConstant.CDFR_MODE_NORMAL) || (m_nMode == clsConstant.PQMS_MODE_SBO) || (m_nMode == clsConstant.PQMS_MODE_SBO_RELAY))
                    {
                        /*
                         *  Resend command to CDFR Terminal 
                         */
                        if (m_nStep == 0)
                        {
                            if (m_bSent)
                            {
                                m_nStep++;
                            }
                            else
                            {
                                m_bSent = true;
                            }
                        }
                        else if (m_nStep == 1)
                        {
                            if (Math.Abs(m_timeStart - Environment.TickCount) > TCPIP_TIMEOUT)
                            {
                                m_nErrorCount++;

                                if (m_nErrorCount > TCPIP_RETRY_COUNT)
                                {
                                    clearRequestMsg();

                                    if (m_bWaitInsFile || m_bWaitPhaFile)
                                    {
                                        m_bWaitInsFile = false;
                                        m_bWaitPhaFile = false;

                                        m_nMode = clsConstant.CDFR_MODE_RUN_TREND;

                                        m_swTrendData.Start();
                                    }
                                    else if (m_nMode != clsConstant.CDFR_MODE_RUN_TREND)
                                    {
                                        m_nMode = clsConstant.CDFR_MODE_RUN_TREND;
                                        m_swTrendData.Start();
                                    }
                                    else 
                                    {
                                        m_nMode = clsConstant.CDFR_MODE_NONE;
                                        m_nComErrorCount++;
                                        m_nErrorCount = 0;
                                        m_statusEventList.Add(new clsStatusEventInfo(m_nIndex, clsConstant.PCS_TCP_COM_ERROR));
                                        makeStatus("장치 통신 대기 시간 초과", true);

                                        m_socketClient.Disconnect(false);
                                        stopSocket();

                                        m_bSent = true;
                                    }

                                    //TcpConnect(m_strIp, m_nPort);

                                    //m_nMode = clsConstant.PQMS_MODE_NONE;
                                }
                                else
                                {
                                    m_nStep = 0;
                                }
                            }
                        }
                    }
                    else if (m_nMode == clsConstant.CDFR_MODE_RUN_TREND)
                    {
                        if (m_EssPcsMsgList.Count > 0)
                        {
                            if (m_EssPcsMsgList[0].msgFinish == 0)
                            {
                                sendMessage();
                            }
                            else if (m_EssPcsMsgList[0].msgFinish == 2)
                            {
                                m_EssPcsMsgList.RemoveAt(0);
                            }
                        }
                        else
                        {
                        }
                    }
                }
#endif
            }

            m_bThreadRun = false;
        }

        void ReceiveCallback(object sender, SerialDataReceivedEventArgs e)
        {
            Int32 m_nStep = -1;

#if USE_EXCEPTION
            try
            {
#endif
                if (m_rtuClient != null)
                {
                    String strMsg = "";
                    UInt32 nSize = 0;
                    int recv = 0;

                    if (m_rtuClient.IsOpen)
                    {
                        try
                        {
                            //...System.Threading.Thread.Sleep(100);

                            recv = m_rtuClient.Read(m_byteReceive, 0, m_byteReceive.Length);
                        }
                        catch (Exception ex)
                        {
                            pauseThread();
                            stopRtu();
                            makeStatus(String.Format("Serial Port Error: {0}", ex.Message), true);
                            return;
                        }
                    }

                    /*
                    rxLog(recv);
                    */
                    
                    if (recv > 0)
                    {
                        if (m_nExpectedLength > 0)
                        {
                            Array.Copy(m_byteReceive, 0, m_bExpectedBuffer, m_nExpectedCount, recv);

                            m_nExpectedCount += recv;

                            if (m_nExpectedCount < m_nExpectedLength)
                            {
                                return;
                            }

                            recv = m_nExpectedLength;
                            m_nExpectedLength = 0;
                            m_nExpectedCount = 0;
                            Array.Copy(m_bExpectedBuffer, 0, m_byteReceive, 0, recv);
                        }

                        m_bProcessReceive = true;

                        if (modbusParsePacket(recv))
                        {
                            if (m_nMode == clsConstant.MODBUS_MODE_NORMAL)
                            {
                                m_nStep = 2;
                                m_nMode = clsConstant.MODBUS_MODE_NONE;

                                if (m_nCommandType == clsConstant.PQMS_COMMAND_CONNECTION_CHECK)
                                {
                                    makeStatus("장치 연결 확인 성공", true);
                                }
                                else if (m_nCommandType == clsConstant.ESSPCS_DEVICE_INV_STOP)
                                {
                                    makeStatus("PCS 정지 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_DEVICE_INV_STOP, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.ESSPCS_DEVICE_INV_RUN)
                                {
                                    makeStatus("PCS 동작 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_DEVICE_INV_RUN, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.ESSPCS_DEVICE_INV_PARAMETER)
                                {
                                    makeStatus("PCS 파라미터 데이터 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_DEVICE_INV_PARAMETER, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_REBOOT)
                                {
                                    makeStatus("장치 리부팅 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_REBOOT, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_MANUAL_TRIGGER)
                                {
                                    makeStatus("장치 매뉴얼 트리거 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_MANUAL_TRIGGER, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_PREV_TRIGGER)
                                {
                                    makeStatus("장치 이전 트리거 수신 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_PREV_TRIGGER, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_CAL_START)
                                {
                                    makeStatus("장치 캘리브레이션 시작 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_CAL_START, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_CAL_STOP)
                                {
                                    makeStatus("장치 캘리브레이션 결과 저장 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_CAL_STOP, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_RELAY_CONTROL_ON)
                                {
                                    makeStatus("장치 릴레이 CONTROL ON 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_RELAY_CONTROL_ON, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_RELAY_CONTROL_OFF)
                                {
                                    makeStatus("장치 릴레이 CONTROL OFF 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_RELAY_CONTROL_OFF, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_TRIGGER_LOCK_ON)
                                {
                                    makeStatus("장치 TRIGGER LOCK ON 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_TRIGGER_LOCK_ON, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_TRIGGER_LOCK_OFF)
                                {
                                    makeStatus("장치 TRIGGER LOCK OFF 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_TRIGGER_LOCK_OFF, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_RELAY)
                                {
                                    makeStatus("장치 RELAY 제어 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_RELAY, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.ESSPCS_CMD_OUTPUT_SET)
                                {
                                    makeStatus("PCS 출력 제어 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_OUTPUT_SET, DateTime.Now);
                                    clsLog.addSystemLog("PCS 출력 명령 전송 완료");
                                }
                                else if (m_nCommandType == clsConstant.ESSPCS_CMD_FAULT_CLEAR)
                                {
                                    makeStatus("PCS FAULT CLEAR 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_FAULT_CLEAR, DateTime.Now);
                                    clsLog.addSystemLog("PCS FAULT CLEAR 명령 전송 완료");
                                }
                                else if (m_nCommandType == clsConstant.ESSPCS_CMD_WARN_CLEAR)
                                {
                                    makeStatus("PCS WARN CLEAR 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_WARN_CLEAR, DateTime.Now);
                                    clsLog.addSystemLog("PCS WARN CLEAR 명령 전송 완료");
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_INS_WAVE)
                                {
                                    /*
                                    m_nInsWaveIndex += 8;
                                    */
                                    m_nInsWaveIndex += clsConstant.CDFR_ONE_TBLOCK_RECS;
                                    m_nInsWaveIndex %= 782082;

                                    if (m_nInsWaveIndex >= m_nInsWaveEnd)
                                    {
                                        nSize = m_waveFormInfo.m_nWaveSize - m_nInsWaveOffset;
                                        Array.Copy(m_byteReceive, 0, m_waveFormInfo.m_bWaveData, m_nInsWaveOffset, nSize);
                                        Array.Copy(m_clsSettingInfo.bSetupData, m_waveFormInfo.m_bSetupData, m_waveFormInfo.m_nSetupSize);
                                        makeStatus("순시 사고 파형 데이터 수신 완료", true);

                                        String strLog = String.Format("{0} 순시 사고 파형 데이터 수신 완료", m_strDeviceName);

                                        Debug.WriteLine(strLog);
                                        clsLog.addRtuLog(strLog);

                                        //  m_EssPcsMsgList[0].msgDateTime = DateTime.Now;
                                        /*
                                        m_EssPcsMsgList[0].msgBuf = new Byte[clsConstant.CDFR_INS_REC_SIZE];
                                        Array.Copy(m_byteReceive, m_EssPcsMsgList[0].msgBuf, clsConstant.CDFR_INS_REC_SIZE);
                                        */
                                        
                                        //  tcpClientEssPcsMsg(m_EssPcsMsgList[0]);
                                        
                                        setMsgFinishMark(clsConstant.ESSPCS_CMD_INS_WAVE, DateTime.Now);

                                        m_bWaitInsFile = false;

                                        receiveTrendData();
                                    }
                                    else
                                    {
                                        /*
                                        Array.Copy(m_byteReceive, 0, m_waveFormInfo.m_bWaveData, m_nInsWaveOffset, 4096 - (nSize - 1) * 512);
                                        */
                                        Array.Copy(m_byteReceive, 0, m_waveFormInfo.m_bWaveData, m_nInsWaveOffset, clsConstant.CDFR_TBLOCK_SIZE);
                                        makeStatus(String.Format("순시 사고 파형 데이터 수신 [{0} / {1}]", m_nInsWaveIndex, m_nInsWaveEnd), false);
                                        /*
                                        m_nInsWaveOffset += 4096;
                                        */
                                        m_nInsWaveOffset += clsConstant.CDFR_TBLOCK_SIZE;
                                        receiveInsWave();
                                    }
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_PHA_WAVE)
                                {
                                    /*
                                    m_nPhaWaveIndex += 8;
                                    */
                                    m_nPhaWaveIndex += clsConstant.CDFR_ONE_TBLOCK_RECS;
                                    m_nPhaWaveIndex %= 913498;

                                    if (m_nPhaWaveIndex >= m_nPhaWaveEnd)
                                    {
                                        nSize = m_nPhaWaveIndex - m_nPhaWaveEnd;
                                        /*
                                        Array.Copy(m_byteReceive, 0, m_waveFormInfo.m_bWaveData, m_nPhaWaveOffset, 4096 - (nSize - 1) * 512);
                                        */
                                        Array.Copy(m_byteReceive, 0, m_waveFormInfo.m_bWaveData, m_nPhaWaveOffset, clsConstant.CDFR_TBLOCK_SIZE - (nSize - 1) * clsConstant.CDFR_ONE_REC_SIZE);
                                        Array.Copy(m_clsSettingInfo.bSetupData, m_waveFormInfo.m_bSetupData, m_waveFormInfo.m_nSetupSize);
                                        //  tcpClientStatusWaveForm(m_waveFormInfo);
                                        //  m_WaveFormList.Add(m_waveFormInfo);

                                        makeStatus("페이저 사고 파형 데이터 수신 완료", true);

                                        String strLog = String.Format("{0} 페이저 사고 파형 데이터 수신 완료", m_strDeviceName);
                                        
                                        Debug.WriteLine(strLog);
                                        clsLog.addRtuLog(strLog);

                                        setMsgFinishMark(clsConstant.ESSPCS_CMD_PHA_WAVE, DateTime.Now);

                                        m_bWaitPhaFile = false;

                                        receiveTrendData();
                                    }
                                    else
                                    {
                                        /*
                                        Array.Copy(m_byteReceive, 0, m_waveFormInfo.m_bWaveData, m_nPhaWaveOffset, 4096);
                                        */
                                        Array.Copy(m_byteReceive, 0, m_waveFormInfo.m_bWaveData, m_nPhaWaveOffset, clsConstant.CDFR_TBLOCK_SIZE);
                                        makeStatus(String.Format("페이저 사고 파형 데이터 수신 [{0} / {1}]", m_nPhaWaveIndex, m_nPhaWaveEnd), false);
                                        /*
                                        m_nInsWaveOffset += 4096;
                                        */
                                        m_nInsWaveOffset += clsConstant.CDFR_TBLOCK_SIZE;
                                        receivePhaWave();
                                    }
                                }
                                else if (m_nCommandType == clsConstant.PCS_DEVICE_TREND_DATA_RECEIVE)
                                {
#if DEBUG_RT
                                    Debug.WriteLine("Receive Real {0}", m_nReceiveByteCount);
#endif
                                    //...if (m_nReceiveByteCount == clsConstant.PCS_TREND_DATA_NUM)
                                    {
                                        Array.Copy(m_byteReceive, 0, m_bTrendDataBuffer, 0, m_nReceiveByteCount);

                                        m_bReceiveTrendData = true;
                                        tcpClientUpdateEventInfo(clsConstant.PCS_TREND_DATA);
                                        strMsg = String.Format("ESS-PCS 실시간 상태 정보 데이터 수신 [장치 시각: {0}]", m_TrendInfo.dtTime);
                                        makeStatus(strMsg, false);
                                        m_statusEventList.Add(new clsStatusEventInfo(m_nIndex, clsConstant.PCS_TREND_DATA));
                                    }

                                    resetTrendStopWatch();
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_RECEIVE_SETUP_DATA)
                                {
                                    if (parseSetupDataPacket())
                                    {
                                        m_bReceiveSetupData = true;
                                        m_clsSettingInfo.bRequestSetupData = false;
                                        makeStatus("SETUP 데이터 수신 완료", true);

                                        setMsgFinishMark(clsConstant.ESSPCS_CMD_INFO_GET, DateTime.Now, m_clsSettingInfo.bSetupData, clsConstant.PCS_SETUP_DATA_NUM);
                                    }
                                    else
                                    {
                                        makeStatus("SETUP 데이터 수신 오류", true);
                                    }
                                }
                                /*
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_SEND_SETUP_DATA)
                                {
                                    m_bSendSetupData = true;
                                    m_clsSettingInfo.bRequestSetupData = false;
                                    makeStatus("SETUP 데이터 송신 완료", true);

                                    setMsgFinishMark(clsConstant.ESSPCS_CMD_INFO_SET, DateTime.Now);
                                }
                                */
                                else if (m_nCommandType == clsConstant.ESSPCS_DEVICE_OUTPUT_SET)
                                {
                                    makeStatus("PCS 출력 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_DEVICE_OUTPUT_SET, DateTime.Now);
                                }
                                else if (m_nCommandType == clsConstant.ESSPCS_DEVICE_INV_PARAMETER)
                                {
                                    makeStatus("PCS 파라미터 명령 전송 완료", true);
                                    setMsgFinishMark(clsConstant.ESSPCS_DEVICE_INV_PARAMETER, DateTime.Now);
                                }
                                else
                                {
                                    m_bWaitInsFile = false;
                                    m_bWaitPhaFile = false;
                                    //<-- 2015.11.22 START
                                    makeStatus(String.Format("알 수 없는 명령어 수신: {0}", m_nCommandType), true);
                                    removeMsgFromCommandType(m_nCommandType);

                                    receiveTrendData();
                                    //--> 2015.11.22 END
                                }
                            }
                        }
                        else
                        {
                            if ((m_nReceiveFunction & 0x80) == 0x80)
                            {
                                makeStatus("통신 예외 메시지 수신", true);

                                String strLog = String.Format("{0} 통신 예외 메시지 수신 ({1:X2})", m_strDeviceName, m_nReceiveFunction);

                                Debug.WriteLine(strLog);
                                clsLog.addRtuLog(strLog);

                                if (m_nCommandType == clsConstant.CDFR_DEVICE_INS_WAVE)
                                {
                                    strLog = String.Format("{0} 순시 파형 받기 재시작", m_strDeviceName);

                                    Debug.WriteLine(strLog);
                                    clsLog.addRtuLog(strLog);
                                
                                    receiveInsWave();
                                }
                                else if (m_nCommandType == clsConstant.CDFR_DEVICE_PHA_WAVE)
                                {
                                    strLog = String.Format("{0} 페이저 파형 받기 재시작", m_strDeviceName);

                                    Debug.WriteLine(strLog);
                                    clsLog.addRtuLog(strLog);
                                    
                                    receivePhaWave();
                                }
                                else
                                {
                                    m_nStep = 2;
                                    m_nMode = clsConstant.MODBUS_MODE_NONE;

                                    setMsgFail(m_nCommandType, DateTime.Now);

                                    m_bWaitPhaFile = false;
                                    m_bWaitInsFile = false;

                                    receiveTrendData();
                                }
                                /*
                                m_nModBusException = (int)m_byteReceive[2];
                                m_bModBusException = true;
                                displayExceptionCode();
                                */
                            }
                        }

                        m_bProcessReceive = false;
                    }
                    else
                    {
                        m_nDisConnectStep = 1;
                        m_bDestroyClient = false;
                        //  TcpDisconnect(true);
                    }
                }
#if USE_EXCEPTION
            }
            //  접속 종료
            catch (Exception ex)
            {
                pauseThread();

                stopRtu();

                makeStatus(String.Format("Shutdown Socket [{0}] : {1}", m_nCommandType, ex.Message), true);
            }
#endif
        }
    }
}
