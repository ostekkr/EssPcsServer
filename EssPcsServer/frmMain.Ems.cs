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

        private void runEms_PvCurtailment(clsPvCurtailmentCommand pvCurtailment)
        {
            int i;

            for (i = 0; i < pvCurtailment.Resources.Count; i++)
            {
                if (pvCurtailment.Resources[i].ResourceId == "PV-001")
                {
                    m_UdpClient.m_nPcsOut[0] = (Int32)pvCurtailment.Resources[i].MaxOutputPower;

                    if (!m_UdpClient.IsPcsTimerRunning(1))
                    {
                        m_UdpClient.runPcsTimer(1);
                    }
                }
                else if (pvCurtailment.Resources[i].ResourceId == "PV-002")
                {
                    m_UdpClient.m_nPcsOut[1] = (Int32)pvCurtailment.Resources[i].MaxOutputPower;

                    if (!m_UdpClient.IsPcsTimerRunning(1))
                    {
                        m_UdpClient.runPcsTimer(2);
                    }
                }
                else if (pvCurtailment.Resources[i].ResourceId == "PV-003")
                {
                    m_UdpClient.m_nPcsOut[2] = (Int32)pvCurtailment.Resources[i].MaxOutputPower;

                    if (!m_UdpClient.IsPcsTimerRunning(1))
                    {
                        m_UdpClient.runPcsTimer(3);
                    }
                }
            }
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
