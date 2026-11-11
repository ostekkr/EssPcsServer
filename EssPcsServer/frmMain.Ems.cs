using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EssPcsServer
{
    public partial class frmMain
    {
        private void runEms_FrequencySupport(clsEssFrequencySupportCommand freqSupport)
        {
            DateTime dtNow = DateTime.Now;
            Int32 nControlPeriod = Int32.Parse(numEssControlPeriod.Text) * 1000;
            Int32 nEssPcsPower = Int32.Parse(numEssPcsPower.Text);
            Int32 nStatus = 0;
            float fSoc = 0;
            float fOutputP = 0;
            float fOutputQ = 0;
            bool bReqStop = false;
            bool bReqSet = false;

            if (freqSupport.OperationMode == "CHARGE")
            {
                nStatus = -1;
            }
            else if (freqSupport.OperationMode == "DISCHARGE")
            {
                nStatus = 1;
            }
            else
            {
                nStatus = 0;
            }

            //  충전
            if (nStatus == -1)
            {
                fSoc = Single.Parse(numEssChargeSocLimit.Text);

                //  SOC가 설정치 미만
                if (m_rtuClient[0].m_TrendInfo.fBatSoc < fSoc)
                {
                    //  인버터 정지 상태
                    if (m_rtuClient[0].m_TrendInfo.nInvStatus != 1)
                    {
                        if ((m_rtuClient[0] != null) && m_bServerRun)
                        {
                            EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvRun(0, true); });
                        }
                    }
                    //  인버터 동작 상태
                    else
                    {
                        float fOutGenPower = nEssPcsPower;
                        float fOutGenPercent = freqSupport.TargetPowerPercent;

                        if (fOutGenPower > 0)
                        {
                            fOutputP = fOutGenPower * fOutGenPercent * 0.01f;

                            if (fOutputP > nEssPcsPower)
                            {
                                fOutputP = nEssPcsPower;
                            }

                            fOutputP *= (-1);
                            bReqSet = true;
                        }
                        else
                        {
                            fOutputP = 0;
                            bReqSet = true;
                        }
                    }
                }
                //  soc가 설정치 이상
                else
                {
                    bReqStop = true;
                }
            }
            //  방전
            else if (nStatus == 1)
            {
                fSoc = Single.Parse(numEssDischargeSocLimit.Text);

                //  soc가 설정치 초과
                if (m_rtuClient[0].m_TrendInfo.fBatSoc > fSoc)
                {
                    //  인버터 정지
                    if (m_rtuClient[0].m_TrendInfo.nInvStatus != 1)
                    {
                        if ((m_rtuClient[0] != null) && m_bServerRun)
                        {
                            EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvRun(0, true); });
                        }
                    }
                    //  인버터 동작
                    else
                    {
                        float fOutGenPower = nEssPcsPower;
                        float fOutGenPercent = freqSupport.TargetPowerPercent;

                        if (fOutGenPower > 0)
                        {
                            fOutputP = fOutGenPower * fOutGenPercent * 0.01f;

                            if (fOutputP > nEssPcsPower)
                            {
                                fOutputP = nEssPcsPower;
                            }

                            bReqSet = true;
                        }
                        else
                        {
                            fOutputP = 0;
                            bReqSet = true;
                        }
                    }
                }
                //  soc가 기준치 이하
                else
                {
                    bReqStop = true;
                }
            }
            //  일반 상태
            else
            {
                bReqStop = true;
            }

            if (m_rtuClient[0].m_TrendInfo.nInvStatus != 1)
            {
                if ((m_rtuClient[0].m_TrendInfo.fReferenceP != 0) || (m_rtuClient[0].m_TrendInfo.fReferenceQ != 0))
                {
                    bReqSet = true;
                    fOutputP = 0;
                }
            }

            if (bReqSet)
            {
                if ((m_rtuClient[0].m_TrendInfo.fReferenceP != fOutputP) || (m_rtuClient[0].m_TrendInfo.fReferenceQ != fOutputQ))
                {
                    if (m_swEms.ElapsedMilliseconds >= nControlPeriod)
                    {
                        m_swEms.Restart();
                        float p = fOutputP, q = fOutputQ;
                        EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvSet(0, true, p, q); });
                    }
                }
            }

            if (bReqStop)
            {
                if (m_rtuClient[0].m_TrendInfo.nInvStatus == 1)
                {
                    if (m_rtuClient[0] != null)
                    {
                        EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvStop(0, true); });
                    }
                }
            }

            setEssControlInfo();
        }

        //  시나리오가 출력 제한을 걸어 둔 PV (PV-001 ~ PV-003)
        private readonly bool[] m_bPvLimitedByScenario = { false, false, false };

        //  BMS SoC 를 마지막으로 받은 시각(Ticks). EnqueueWcf 스레드에서 쓰므로 Interlocked 로 다룬다.
        private long m_lBmsSocTicks = 0;

        //  SoC 가 이 시간(초) 넘게 갱신되지 않으면 버전 2 ESS 시나리오는 정지한다.
        private const int BMS_SOC_TIMEOUT_SEC = 10;

        //  마지막으로 기록한 ESS 판단 문구(같은 판단을 매초 로그에 남기지 않기 위함)
        private string m_strLastEssDecision = "";

        private void markBmsSocUpdated()
        {
            System.Threading.Interlocked.Exchange(ref m_lBmsSocTicks, clsClock.Now().Ticks);
        }

        private bool isBmsSocValid()
        {
            long lTicks = System.Threading.Interlocked.Read(ref m_lBmsSocTicks);

            return (lTicks > 0) && ((clsClock.Now() - new DateTime(lTicks)).TotalSeconds <= BMS_SOC_TIMEOUT_SEC);
        }

        /// <summary>
        /// PV 출력제한 시나리오의 PV 별 상한을 적용한다(버전 1·2 공통, ESS 운영 선택과 무관).
        /// 상한이 없어진 PV 는 제한을 해제한다: pvReleaseOutput 이 0 이상이면 그 값을 한 번 보내고,
        /// 아니면 주기 전송만 멈춘다(RTU 의 해제 방법이 정해지면 설정으로 바꾼다).
        /// </summary>
        private void applyPvLimits(float?[] limits)
        {
            if (m_UdpClient == null)
            {
                return;
            }

            for (int i = 0; i < m_bPvLimitedByScenario.Length; i++)
            {
                int nId = i + 1;

                if (limits[i].HasValue)
                {
                    float fLimit = limits[i].Value;

                    if (!m_bPvLimitedByScenario[i] || !clsEmsPlanner.SameKw(m_UdpClient.m_nPcsOut[i], fLimit))
                    {
                        m_UdpClient.m_nPcsOut[i] = fLimit;
                        m_UdpClient.sendPcsOutNow(nId, false);
                        clsLog.addSystemLog(string.Format("시나리오 PV 출력 제한 : {0} {1:0.0}kW", clsEmsPlanner.PvResourceIds[i], fLimit));
                    }

                    m_bPvLimitedByScenario[i] = true;
                }
                else if (m_bPvLimitedByScenario[i])
                {
                    m_bPvLimitedByScenario[i] = false;

                    float fRelease = Properties.Settings.Default.pvReleaseOutput;

                    if (fRelease >= 0)
                    {
                        m_UdpClient.m_nPcsOut[i] = fRelease;
                        m_UdpClient.sendPcsOutNow(nId, true);
                        clsLog.addSystemLog(string.Format("시나리오 PV 출력 제한 해제 : {0} ({1:0.0}kW 전송)", clsEmsPlanner.PvResourceIds[i], fRelease));
                    }
                    else
                    {
                        m_UdpClient.stopPcsTimer(nId);
                        clsLog.addSystemLog(string.Format("시나리오 PV 출력 제한 해제 : {0} (주기 전송 중지)", clsEmsPlanner.PvResourceIds[i]));
                    }
                }
            }
        }

        /// <summary>버전 2 ESS 시나리오(구간 지령) 실행.</summary>
        private void runEms_EssSegments(clsScenarioCommandBase cmd, DateTime dtNow)
        {
            string strPvId = null;

            if (cmd is clsPvEssControlCommand) strPvId = ((clsPvEssControlCommand)cmd).PvResourceId;
            if (cmd is clsEssLoadReductionCommand) strPvId = ((clsEssLoadReductionCommand)cmd).PvResourceId;

            float fPvGen = 0;
            int nPv = clsEmsPlanner.PvIndex(strPvId);

            if (nPv >= 0 && m_UdpClient != null)
            {
                fPvGen = m_UdpClient.m_GenInfoList[nPv].m_fInvOut;
            }

            clsEssDecision d = clsEmsPlanner.DecideEss(
                clsEmsPlanner.ActiveSegment(cmd, dtNow),
                m_rtuClient[0].m_TrendInfo.fBatSoc,
                isBmsSocValid(),
                Single.Parse(numEssChargeSocLimit.Text),
                Single.Parse(numEssDischargeSocLimit.Text),
                Single.Parse(numEssPcsPower.Text),
                fPvGen);

            string strDecision = cmd.ScenarioName + " " + d.ToString();

            if (strDecision != m_strLastEssDecision)
            {
                m_strLastEssDecision = strDecision;
                clsLog.addSystemLog("EMS 판단 : " + strDecision);
            }

            applyEssDecision(d);
        }

        /// <summary>ESS 판단을 PCS 명령으로 옮긴다(운전·정지 요청, 제어 주기에 맞춘 출력 설정).</summary>
        private void applyEssDecision(clsEssDecision d)
        {
            int nControlPeriod = Int32.Parse(numEssControlPeriod.Text) * 1000;
            float fOutputP = d.OutputP;
            float fOutputQ = 0;
            bool bReqSet = false;
            bool bReqStop = false;

            if (d.Status != 0)
            {
                if (m_rtuClient[0].m_TrendInfo.nInvStatus != 1)
                {
                    if ((m_rtuClient[0] != null) && m_bServerRun)
                    {
                        EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvRun(0, true); });
                    }
                }
                else
                {
                    bReqSet = true;
                }
            }
            else
            {
                bReqStop = true;
            }

            if (m_rtuClient[0].m_TrendInfo.nInvStatus != 1)
            {
                if (!clsEmsPlanner.SameKw(m_rtuClient[0].m_TrendInfo.fReferenceP, 0) || !clsEmsPlanner.SameKw(m_rtuClient[0].m_TrendInfo.fReferenceQ, 0))
                {
                    bReqSet = true;
                    fOutputP = 0;
                }
            }

            if (bReqSet)
            {
                if (!clsEmsPlanner.SameKw(m_rtuClient[0].m_TrendInfo.fReferenceP, fOutputP) || !clsEmsPlanner.SameKw(m_rtuClient[0].m_TrendInfo.fReferenceQ, fOutputQ))
                {
                    if (m_swEms.ElapsedMilliseconds >= nControlPeriod)
                    {
                        m_swEms.Restart();
                        float p = fOutputP, q = fOutputQ;
                        EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvSet(0, true, p, q); });
                    }
                }
            }

            if (bReqStop)
            {
                if (m_rtuClient[0].m_TrendInfo.nInvStatus == 1)
                {
                    if (m_rtuClient[0] != null)
                    {
                        EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvStop(0, true); });
                    }
                }
            }

            setEssControlInfo();
        }

        private void runEms_PvEssControl(clsPvEssControlCommand pvEssControl)
        {
            DateTime dtNow = DateTime.Now;
            int nControlPeriod = Int32.Parse(numEssControlPeriod.Text) * 1000;
            Int32 nEssPcsPower = Int32.Parse(numEssPcsPower.Text);
            Int32 nStatus = 0;
            float fSoc = 0;
            float fOutputP = 0;
            float fOutputQ = 0;
            bool bReqStop = false;
            bool bReqSet = false;

            float fOutGenPower = 0;

            if (pvEssControl.PvResourceId == "PV-001")
            {
                fOutGenPower += m_UdpClient.m_GenInfoList[0].m_fInvOut;
            }
            
            if (pvEssControl.PvResourceId == "PV-002")
            {
                fOutGenPower += m_UdpClient.m_GenInfoList[1].m_fInvOut;
            }

            if (pvEssControl.PvResourceId == "PV-003")
            {
                fOutGenPower += m_UdpClient.m_GenInfoList[2].m_fInvOut;
            }

            if (chkPvGenZero.Checked)
            {
                nStatus = 1;
            }
            else
            {
                if (fOutGenPower > 0)
                {
                    nStatus = -1;
                }
                else if (fOutGenPower == 0)
                {
                    nStatus = 1;
                }
                else
                {
                    nStatus = 0;
                }
            }

            //  충전
            if (nStatus == -1)
            {
                fSoc = Single.Parse(numEssChargeSocLimit.Text);

                //  SOC가 설정치 미만
                if (m_rtuClient[0].m_TrendInfo.fBatSoc < fSoc)
                {
                    //  인버터 정지 상태
                    if (m_rtuClient[0].m_TrendInfo.nInvStatus != 1)
                    {
                        if ((m_rtuClient[0] != null) && m_bServerRun)
                        {
                            EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvRun(0, true); });
                        }
                    }
                    //  인버터 동작 상태
                    else
                    {
                        float fOutGenPercent = pvEssControl.ChargeRule.ChargeRatio;

                        fOutputP = fOutGenPower * fOutGenPercent * 0.01f;

                        if (fOutputP > nEssPcsPower)
                        {
                            fOutputP = nEssPcsPower;
                        }

                        fOutputP *= (-1);
                        bReqSet = true;
                    }
                }
                //  soc가 설정치 이상
                else
                {
                    bReqStop = true;
                }
            }
            //  방전
            else if (nStatus == 1)
            {
                fSoc = Single.Parse(numEssDischargeSocLimit.Text);

                //  soc가 설정치 초과
                if (m_rtuClient[0].m_TrendInfo.fBatSoc > fSoc)
                {
                    //  인버터 정지
                    if (m_rtuClient[0].m_TrendInfo.nInvStatus != 1)
                    {
                        if ((m_rtuClient[0] != null) && m_bServerRun)
                        {
                            EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvRun(0, true); });
                        }
                    }
                    //  인버터 동작
                    else
                    {
                        float fOutGenPercent = pvEssControl.DischargeRule.DischargePowerPercent;
                        fOutGenPower = nEssPcsPower;

                        if (fOutGenPower > 0)
                        {
                            fOutputP = fOutGenPower * fOutGenPercent * 0.01f;

                            if (fOutputP > nEssPcsPower)
                            {
                                fOutputP = nEssPcsPower;
                            }

                            bReqSet = true;
                        }
                        else
                        {
                            fOutputP = 0;
                            bReqSet = true;
                        }
                    }
                }
                //  soc가 기준치 이하
                else
                {
                    bReqStop = true;
                }
            }
            //  일반 상태
            else
            {
                bReqStop = true;
            }

            if (m_rtuClient[0].m_TrendInfo.nInvStatus != 1)
            {
                if ((m_rtuClient[0].m_TrendInfo.fReferenceP != 0) || (m_rtuClient[0].m_TrendInfo.fReferenceQ != 0))
                {
                    bReqSet = true;
                    fOutputP = 0;
                }
            }

            if (bReqSet)
            {
                if ((m_rtuClient[0].m_TrendInfo.fReferenceP != fOutputP) || (m_rtuClient[0].m_TrendInfo.fReferenceQ != fOutputQ))
                {
                    if (m_swEms.ElapsedMilliseconds >= nControlPeriod)
                    {
                        m_swEms.Restart();
                        float p = fOutputP, q = fOutputQ;
                        EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvSet(0, true, p, q); });
                    }
                }
            }

            if (bReqStop)
            {
                if (m_rtuClient[0].m_TrendInfo.nInvStatus == 1)
                {
                    if (m_rtuClient[0] != null)
                    {
                        EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvStop(0, true); });
                    }
                }
            }

            setEssControlInfo();
        }

        private void runEms_General()
        {
            DateTime dtNow = DateTime.Now;
            Int32 nChargeStart = Int32.Parse(numEssChargeStartTimeHour.Text) * 60 + Int32.Parse(numEssChargeStartTimeMin.Text);
            Int32 nChargeEnd = Int32.Parse(numEssChargeEndTimeHour.Text) * 60 + Int32.Parse(numEssChargeEndTimeMin.Text);
            Int32 nDischargeStart = Int32.Parse(numEssDischargeStartTimeHour.Text) * 60 + Int32.Parse(numEssDischargeStartTimeMin.Text);
            Int32 nDischargeEnd = Int32.Parse(numEssDischargeEndTimeHour.Text) * 60 + Int32.Parse(numEssDischargeEndTimeMin.Text);
            Int32 nNow = dtNow.Minute + dtNow.Hour * 60;
            Int32 nStatus = 0;
            float fSoc = 0;
            float fOutputP = 0;
            float fOutputQ = 0;
            bool bReqStop = false;
            bool bReqSet = false;
            int nControlPeriod = Int32.Parse(numEssControlPeriod.Text) * 1000;

            if (nChargeStart <= nChargeEnd)
            {
                if ((nNow >= nChargeStart) && (nNow <= nChargeEnd))
                {
                    nStatus = -1;
                }
            }
            else
            {
                if ((nNow >= nChargeStart) || (nNow <= nChargeEnd))
                {
                    nStatus = -1;
                }
            }

            if (nDischargeStart <= nDischargeEnd)
            {
                if ((nNow >= nDischargeStart) && (nNow <= nDischargeEnd))
                {
                    nStatus = 1;
                }
            }
            else
            {
                if ((nNow >= nDischargeStart) || (nNow <= nDischargeEnd))
                {
                    nStatus = 1;
                }
            }

            //  충전
            if (nStatus == -1)
            {
                fSoc = Single.Parse(numEssChargeSocLimit.Text);

                //  SOC가 설정치 미만
                if (m_rtuClient[0].m_TrendInfo.fBatSoc < fSoc)
                {
                    //  인버터 정지 상태
                    if (m_rtuClient[0].m_TrendInfo.nInvStatus != 1)
                    {
                        if ((m_rtuClient[0] != null) && m_bServerRun)
                        {
                            EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvRun(0, true); });
                        }
                    }
                    //  인버터 동작 상태
                    else
                    {
                        //  발전량 연계
                        if (chkEssGen.Checked)
                        {
                            float fOutGenPower = Single.Parse(numEssGenOutPower.Text);
                            float fOutGenPercent = Single.Parse(numEssGenOutPercent.Text);

                            if (fOutGenPower > 0)
                            {
                                fOutputP = fOutGenPower * fOutGenPercent * 0.01f;

                                if (fOutputP > Single.Parse(numEssChargeOutputLimit.Text))
                                {
                                    fOutputP = Single.Parse(numEssChargeOutputLimit.Text);
                                }

                                fOutputP *= (-1);
                                bReqSet = true;
                            }
                            else
                            {
                                fOutputP = 0;
                                bReqSet = true;
                            }
                        }
                        //  단독운전
                        else
                        {
                            fOutputP = Single.Parse(numEssChargeOutputLimit.Text) * (-1);
                            bReqSet = true;
                        }
                    }
                }
                //  soc가 설정치 이상
                else
                {
                    bReqStop = true;
                }
            }
            //  방전
            else if (nStatus == 1)
            {
                fSoc = Single.Parse(numEssDischargeSocLimit.Text);

                //  soc가 설정치 초과
                if (m_rtuClient[0].m_TrendInfo.fBatSoc > fSoc)
                {
                    fOutputP = Single.Parse(numEssDischargeOutputLimit.Text);

                    //  인버터 정지
                    if (m_rtuClient[0].m_TrendInfo.nInvStatus != 1)
                    {
                        if ((m_rtuClient[0] != null) && m_bServerRun)
                        {
                            EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvRun(0, true); });
                        }
                    }
                    //  인버터 동작
                    else
                    {
                        //  발전량 연계
                        if (chkEssGen.Checked)
                        {
                            float fOutGenPower = Single.Parse(numEssGenOutPower.Text);

                            fOutputP = Single.Parse(numEssDischargeOutputLimit.Text);

                            if (fOutGenPower < fOutputP)
                            {
                                fOutputP -= fOutGenPower;
                                bReqSet = true;
                            }
                            else
                            {
                                fOutputP = 0;
                                bReqSet = true;
                            }
                        }
                        //  단독 운전
                        else
                        {
                            bReqSet = true;
                        }
                    }
                }
                //  soc가 기준치 이하
                else
                {
                    bReqStop = true;
                }
            }
            //  일반 상태
            else
            {
                bReqStop = true;
            }

            if (m_rtuClient[0].m_TrendInfo.nInvStatus != 1)
            {
                if ((m_rtuClient[0].m_TrendInfo.fReferenceP != 0) || (m_rtuClient[0].m_TrendInfo.fReferenceQ != 0))
                {
                    bReqSet = true;
                    fOutputP = 0;
                }
            }

            if (bReqSet)
            {
                if ((m_rtuClient[0].m_TrendInfo.fReferenceP != fOutputP) || (m_rtuClient[0].m_TrendInfo.fReferenceQ != fOutputQ))
                {
                    if (m_swEms.ElapsedMilliseconds >= nControlPeriod)
                    {
                        m_swEms.Restart();
                        float p = fOutputP, q = fOutputQ;
                        EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvSet(0, true, p, q); });
                    }
                }
            }

            if (bReqStop)
            {
                if (m_rtuClient[0].m_TrendInfo.nInvStatus == 1)
                {
                    if (m_rtuClient[0] != null)
                    {
                        EnqueueWcf(() => { if (m_EssPcsServiceProxy != null) m_EssPcsServiceProxy.setInvStop(0, true); });
                    }
                }
            }

            setEssControlInfo();
        }
    }
}
