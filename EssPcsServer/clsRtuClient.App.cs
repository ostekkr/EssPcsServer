using EssPcsService;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace EssPcsServer
{
    public partial class clsRtuClient
    {
        public int m_nMinProgressBar = 0;
        public int m_nMaxProgressBar = 0;
        public bool m_bUpdateSetting = false;
        public int m_nInsIndex = -1;
        public int m_nInsNum = -1;
        public clsWaveFormInfo m_waveFormInfo;
        public clsTrend m_clsTrend = new clsTrend();

        void removeEventMessage()
        {
            m_EventReqList.Clear();
        }

        void removeEventReq()
        {
            if (m_EventReqList.Count > 0)
            {
                m_EventReqList.RemoveAt(0);
            }
        }

        private void sendMessage()
        {
            clsEssPcsMsg msg = CommandQueue.PeekFirst();
            if (msg == null) return;

            if (msg.msgFinish == 0)
            {
                if (msg.msgType == clsConstant.ESSPCS_CMD_TIME_GET)
                {
                    //...receiveTrendData();
                    //...msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_INV_PARAMETER)
                {
                    sendEssPcsSetParameter(msg);
                    msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_INFO_GET)
                {
                    //...receiveSetupData();
                    //...msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_INV_STOP)
                {
                    sendEssPcsStop(msg);
                    msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_INV_RUN)
                {
                    sendEssPcsRun(msg);
                    msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_REBOOT)
                {
                    //...sendReboot();
                    //...msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_MANUAL_TRIGGER)
                {
                    //...sendManualTrigger();
                    //...msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_PREV_TRIGGER)
                {
                    //...sendPrevTrigger();
                    //...msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_CAL_START)
                {
                    //...sendCalStart();
                    //...msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_CAL_STOP)
                {
                    //...sendCalStop();
                    //...msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_RELAY_CONTROL_ON)
                {
                    //...sendRelayControlOn();
                    //...msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_RELAY_CONTROL_OFF)
                {
                    //...sendRelayControlOff();
                    //...msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_TRIGGER_LOCK_ON)
                {
                    //...sendTriggerLockOn();
                    //...msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_TRIGGER_LOCK_OFF)
                {
                    //...sendTriggerLockOff();
                    //...msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_RELAY)
                {
                    //...sendRelay(msg.msgBuf[0]);
                    //...msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_FAULT_CLEAR)
                {
                    sendEssPcsFaultClear(msg);
                    msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_WARN_CLEAR)
                {
                    sendEssPcsWarnClear(msg);
                    msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_OUTPUT_SET)
                {
                    sendEssPcsOutput(msg);
                    msg.msgCommand = m_nCommandType;
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_INS_WAVE)
                {
                    /*
                    m_bWaitInsFile = true;

                    //...m_nInsWaveStart = (UInt32)(msg.msgBuf[0] + msg.msgBuf[1] * 0x100 + msg.msgBuf[2] * 0x10000 + msg.msgBuf[3] * 0x1000000);
                    m_nInsWaveEnd = (UInt32)(msg.msgBuf[4] + msg.msgBuf[5] * 0x100 + msg.msgBuf[6] * 0x10000 + msg.msgBuf[7] * 0x1000000);
                    //...Debug.WriteLine("InsWave [{0} - {1}] [{2} - {3}]", m_nInsWaveStart, m_nInsWaveEnd, m_nInsWaveStart % 782082, m_nInsWaveEnd % 782082);
                    //...m_nInsWaveIndex = m_nInsWaveStart;
                    m_nInsWaveOffset = 0;

                    receiveInsWave();

                    msg.msgCommand = m_nCommandType;
                    */
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_PHA_WAVE)
                {
                    /*
                    m_bWaitPhaFile = true;

                    m_nPhaWaveStart = (UInt32)(msg.msgBuf[0] + msg.msgBuf[1] * 0x100 + msg.msgBuf[2] * 0x10000 + msg.msgBuf[3] * 0x1000000);
                    m_nPhaWaveEnd = (UInt32)(msg.msgBuf[4] + msg.msgBuf[5] * 0x100 + msg.msgBuf[6] * 0x10000 + msg.msgBuf[7] * 0x1000000);
                    m_nPhaWaveIndex = m_nPhaWaveStart;
                    m_nPhaWaveOffset = 0;

                    receivePhaWave();

                    msg.msgCommand = m_nCommandType;
                    */
                }
                else if (msg.msgType == clsConstant.ESSPCS_CMD_FAULT_LOC)
                {
                    /*
                    sendFaultLoc(msg);

                    msg.msgCommand = m_nCommandType;
                    */
                }
                else
                {
                    msg.msgFinish = 2;
                }
            }
        }

        //  Process Event Information
        public void tcpClientUpdateEventInfo(int nEvent)
        {
            if (nEvent == clsConstant.PQMS_WAVE_DATA)
            {
                if (m_bReceiveWaveData)
                {
                    /*
                    DateTime dtWave = m_dtWaveTime;
                    float fPtRatio = m_clsSettingInfo.fPTRATIO;
                    float fCtRatio = Convert.ToSingle(m_clsSettingInfo.nCTRATIO);
                    float fPtRating = clsConstant.PQMS_PT_RATING[m_clsSettingInfo.nPTRATE];
                    float fCtRating = clsConstant.PQMS_CT_RATING[m_clsSettingInfo.nCTRATE];
                    int nSamplingRate = clsConstant.PQMS_SAMPLING_RATE[m_clsSettingInfo.nSAMPLERATE];
                    float fFrequency = clsConstant.PQMS_FREQUENCY[m_clsSettingInfo.nFREQ];
                    int nSamplings = clsConstant.PQMS_SAMPLES[m_clsSettingInfo.nSECTOR] * clsConstant.PQMS_BYTES_PER_SAMPLE;

                    addWaveFormInfo(dtWave, fPtRatio, fCtRatio, fPtRating, fCtRating, nSamplingRate, fFrequency, m_bWaveBuffer, nSamplings, m_nWaveIndex, m_nFaultPoint, m_clsSettingInfo.m_nGainData);
                    removeEventReq();
                    m_bReceiveWaveData = false;
                    sendMessage();
                    */
                }
            }
            else if (nEvent == clsConstant.PCS_TREND_DATA)
            {
                if (m_bReceiveTrendData)
                {
                    Array.Copy(m_bTrendDataBuffer, 0, m_TrendInfo.bTrendData, m_nModbusMapIndex * 256, 256);

                    if (m_nModbusMapIndex == (m_AddrInfoList.Count - 1))
                    {
                        m_clsTrend.ParseTrendInfo(m_TrendInfo);

                        TimeSpan ts = m_TrendInfo.dtTime - m_clsTrend.m_dtSave;

                        //...if (Math.Abs(ts.TotalSeconds) >= 1)
                        {
                            m_trendInfoList.Add(m_TrendInfo);

                            if (m_trendInfoList.Count > 100)
                            {
                                m_trendInfoList.RemoveAt(0);
                            }

                            m_clsTrend.m_dtSave = m_TrendInfo.dtTime;
                        }

                        //  실시간 정보 수신 시, 장치 시각 메시지 완료 
                        setMsgFinishMark(clsConstant.ESSPCS_CMD_TIME_GET, m_TrendInfo.dtTime);
                    }

                    m_bReceiveTrendData = false;

                    m_nModbusMapIndex++;
                    m_nModbusMapIndex %= m_AddrInfoList.Count;
                }
            }
            else
            {
                /*
                while (m_clsEvent.m_arrayEvent.Count > 0)
                {
                    if (m_clsEvent.m_arrayEvent[0].m_nEventType == clsConstant.PQMS_SYSTEM_EVENT)
                    {
                        addSystemEventInfo(m_clsEvent.m_arrayEvent[0].m_bEventData);
                    }
                    else if (m_clsEvent.m_arrayEvent[0].m_nEventType == clsConstant.PQMS_PQ_EVENT)
                    {
                        //addPqEventInfo(m_clsEvent.m_arrayEvent[0].m_bEventData);
                    }
                    else if (m_clsEvent.m_arrayEvent[0].m_nEventType == clsConstant.PQMS_LOAD_PROFILE)
                    {
                        addLoadProfileInfo(m_clsEvent.m_arrayEvent[0].m_bEventData);
                    }

                    m_clsEvent.m_arrayEvent.RemoveAt(0);
                }
                
                removeEventReq();

                sendMessage();
                */
            }
        }
    }
}
