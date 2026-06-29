using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EssPcsServer
{
    public partial class clsRtuClient
    {
        /*
        private void printSystemEventInfo()
        {
            int i;

            if (m_SystemEventList.Count > 0)
            {
                for (i = 0; i < m_SystemEventList.Count; i++)
                {
                    m_swSystemEvent.WriteLine("{0} : {1}", m_SystemEventList[i].m_rtcEvent.ToDateTime().ToLocalTime(), m_SystemEventList[i].m_strLog);
                }
            }
        }
        */
        /*
        private void printPqEventInfo()
        {
            int i;

            if (m_PqEventList.Count > 0)
            {
                for (i = 0; i < m_PqEventList.Count; i++)
                {
                    m_swPqEvent.WriteLine("{0} : {1} <{2} {3} {4} {5} {6} {7} {8} {9} {10} {11}>", 
                        m_PqEventList[i].m_rtcEvent.ToDateTime().ToLocalTime(), 
                        m_PqEventList[i].m_strLog,
                        Convert.ToString(m_PqEventList[i].m_bData[0], 16).PadLeft(2,'0'),
                        Convert.ToString(m_PqEventList[i].m_bData[1], 16).PadLeft(2, '0'),
                        Convert.ToString(m_PqEventList[i].m_bData[2], 16).PadLeft(2, '0'),
                        Convert.ToString(m_PqEventList[i].m_bData[3], 16).PadLeft(2, '0'),
                        Convert.ToString(m_PqEventList[i].m_bData[4], 16).PadLeft(2, '0'),
                        Convert.ToString(m_PqEventList[i].m_bData[5], 16).PadLeft(2, '0'),
                        Convert.ToString(m_PqEventList[i].m_bData[6], 16).PadLeft(2, '0'),
                        Convert.ToString(m_PqEventList[i].m_bData[7], 16).PadLeft(2, '0'),
                        Convert.ToString(m_PqEventList[i].m_bData[8], 16).PadLeft(2, '0'),
                        Convert.ToString(m_PqEventList[i].m_bData[9], 16).PadLeft(2, '0'));
                }
            }
        }
        */
        /*
        private void printLoadProfileInfo()
        {
            int i;

            if (m_LoadProfileList.Count > 0)
            {
                for (i = 0; i < m_LoadProfileList.Count; i++)
                {
                    m_swLoadProfile.WriteLine("{0} : {1}", m_LoadProfileList[i].m_rtcEvent.ToDateTime().ToLocalTime(), m_LoadProfileList[i].m_strLog);
                }
            }
        }
        */
        /*
        private void printWaveFormInfo()
        {
            int i, j;
            string strTemp;

            if (m_WaveFormList.Count > 0)
            {
                StreamWriter sw = new StreamWriter(@"c:\\waveform.txt", true);

                for (i = 0; i < m_WaveFormList.Count; i++)
                {
                    m_swWaveform.WriteLine("");
                    m_swWaveform.WriteLine("");
                    m_swWaveform.WriteLine("{0}", m_WaveFormList[i].m_strLog);
                    m_swWaveform.WriteLine("TIME : {0}", m_WaveFormList[i].m_dtWaveForm.ToLocalTime());

                    for (j = 0; j < (clsConstant.PQMS_WAVE_LENGTH * clsConstant.PQMS_WAVE_NUM_BLOCK); j++)
                    {
                        if ((j % 16) == 0)
                        {
                            if (j > 0)
                            {
                                m_swWaveform.WriteLine("");
                            }

                            strTemp=string.Format("{0} : ", Convert.ToString(j, 16).PadLeft(4,'0'));
                            m_swWaveform.Write(strTemp);
                        }

                        strTemp = string.Format("{0}", Convert.ToString(m_WaveFormList[i].m_bData[j], 16).PadLeft(2, '0'));
                        m_swWaveform.Write(strTemp);
                    }
                }
            }
        }
        */
        /*
        private void printTrendInfo(clsTrend trendInfo)
        {
            int nDevice = trendInfo.m_nDevice;

            m_swTrend.WriteLine("DEVICE : {0}", nDevice);


            //  CDFR 상태
            m_swTrend.WriteLine("TIME : {0}{1}{2}{3}",
                Convert.ToString(trendInfo.m_bStatus[0], 16).PadLeft(2, '0'),
                Convert.ToString(trendInfo.m_bStatus[1], 16).PadLeft(2, '0'),
                Convert.ToString(trendInfo.m_bStatus[2], 16).PadLeft(2, '0'),
                Convert.ToString(trendInfo.m_bStatus[3], 16).PadLeft(2, '0'));
            //  Date Time (in Second)
            m_swTrend.WriteLine("TIME : {0}", trendInfo.m_dtTrend);
            //  R상 전류
            m_swTrend.WriteLine("R상 전류 : {0:F3}", trendInfo.m_fIR);
            //  S상 전류
            m_swTrend.WriteLine("S상 전류 : {0:F3}", trendInfo.m_fIS);
            //  T상 전류
            m_swTrend.WriteLine("T상 전류 : {0:F3}", trendInfo.m_fIT);
            //  N상 전류
            m_swTrend.WriteLine("N상 전류 : {0:F3}", trendInfo.m_fIN);
            //  R상 전압 (상전압)
            m_swTrend.WriteLine("R상 전압 (상전압) : {0:F3}", trendInfo.m_fVR);
            //  S상 전압 (상전압)
            m_swTrend.WriteLine("S상 전압 (상전압) : {0:F3}", trendInfo.m_fVS);
            //  T상 전압 (상전압)
            m_swTrend.WriteLine("T상 전압 (상전압) : {0:F3}", trendInfo.m_fVT);
            //  N상 전압
            m_swTrend.WriteLine("N상 전압 : {0:F3}", trendInfo.m_fVN);
            //  R상 전압 (상전압)
            m_swTrend.WriteLine("RS상 전압 (선간전압) : {0:F3}", trendInfo.m_fVRS);
            //  S상 전압 (상전압)
            m_swTrend.WriteLine("ST상 전압 (선간전압) : {0:F3}", trendInfo.m_fVST);
            //  T상 전압 (상전압)
            m_swTrend.WriteLine("TR상 전압 (선간전압) : {0:F3}", trendInfo.m_fVTR);
            //  3상 Total 역률
            m_swTrend.WriteLine("3상 Total 역률 : {0:F3}", trendInfo.m_f3PhPF);
            //  3상 Total 유효전력
            m_swTrend.WriteLine("3상 Total 유효전력 : {0:F3}", trendInfo.m_f3PhP);
            //  3상 Total 무효전력
            m_swTrend.WriteLine("3상 Total 무효전력 : {0:F3}", trendInfo.m_f3PhQ);
            //  3상 Total 피상전력
            m_swTrend.WriteLine("3상 Total 피상전력 : {0:F3}", trendInfo.m_f3PhS);
            //  주파수
            m_swTrend.WriteLine("주파수 : {0:F3}", trendInfo.m_fFreq);
            //  수전 전력량
            m_swTrend.WriteLine("수전 전력량 : {0:F3}", trendInfo.m_fPowerR);
            //  송전 전력량
            m_swTrend.WriteLine("송전 전력량 : {0:F3}", trendInfo.m_fPowerT);
            //  발전 전력량
            m_swTrend.WriteLine("발전 전력량 : {0:F3}", trendInfo.m_fPowerG);
            //  소비 전력량
            m_swTrend.WriteLine("소비 전력량 : {0:F3}", trendInfo.m_fPowerC);
            //  정상 전압
            m_swTrend.WriteLine("정상 전압 : {0:F3}", trendInfo.m_fPositivePhaseVoltage);
            //  역상 전압
            m_swTrend.WriteLine("역상 전압 : {0:F3}", trendInfo.m_fNegativePhaseVoltage);
            //  전압 불평형률
            m_swTrend.WriteLine("전압 불평형률 : {0:F3}", trendInfo.m_fVoltageUnbalanceRate);
            //  정상 전류
            m_swTrend.WriteLine("정상 전류 : {0:F3}", trendInfo.m_fPositivePhaseCurrent);
            //  역상 전류
            m_swTrend.WriteLine("역상 전류 : {0:F3}", trendInfo.m_fNegativePhaseCurrent);
            //  전류 불평형률
            m_swTrend.WriteLine("전류 불평형률 : {0:F3}", trendInfo.m_fCurrentUnbalanceRate);
            //  R상 전압 위상
            m_swTrend.WriteLine("R상 전압 위상 : {0:F3}", trendInfo.m_fPhaseVR);
            //  S상 전압 위상
            m_swTrend.WriteLine("S상 전압 위상 : {0:F3}", trendInfo.m_fPhaseVS);
            //  T상 전압 위상
            m_swTrend.WriteLine("T상 전압 위상 : {0:F3}", trendInfo.m_fPhaseVT);
            //  RS상 전압 위상
            m_swTrend.WriteLine("RS상 전압 위상 : {0:F3}", trendInfo.m_fPhaseVRS);
            //  ST상 전압 위상
            m_swTrend.WriteLine("ST상 전압 위상 : {0:F3}", trendInfo.m_fPhaseVST);
            //  TR상 전압 위상
            m_swTrend.WriteLine("TR상 전압 위상 : {0:F3}", trendInfo.m_fPhaseVTR);
            //  R상 전류 위상
            m_swTrend.WriteLine("R상 전류 위상 : {0:F3}", trendInfo.m_fPhaseIR);
            //  S상 전류 위상
            m_swTrend.WriteLine("S상 전류 위상 : {0:F3}", trendInfo.m_fPhaseIS);
            //  T상 전류 위상
            m_swTrend.WriteLine("T상 전류 위상 : {0:F3}", trendInfo.m_fPhaseIT);
            //  R상 전류 역률
            m_swTrend.WriteLine("R상 전류 역률 : {0:F3}", trendInfo.m_fPFIR);
            //  S상 전류 역률
            m_swTrend.WriteLine("S상 전류 역률 : {0:F3}", trendInfo.m_fPFIS);
            //  T상 전류 역률
            m_swTrend.WriteLine("T상 전류 역률 : {0:F3}", trendInfo.m_fPFIT);
            //  R상 유효전력
            m_swTrend.WriteLine("R상 유효전력 : {0:F3}", trendInfo.m_fPR);
            //  S상 유효전력
            m_swTrend.WriteLine("S상 유효전력 : {0:F3}", trendInfo.m_fPS);
            //  T상 유효전력
            m_swTrend.WriteLine("T상 유효전력 : {0:F3}", trendInfo.m_fPT);
            //  R상 무효전력
            m_swTrend.WriteLine("R상 무효전력 : {0:F3}", trendInfo.m_fQR);
            //  S상 무효전력
            m_swTrend.WriteLine("S상 무효전력 : {0:F3}", trendInfo.m_fQS);
            //  T상 무효전력
            m_swTrend.WriteLine("T상 무효전력 : {0:F3}", trendInfo.m_fQT);
            //  R상 피상전력
            m_swTrend.WriteLine("R상 피상전력 : {0:F3}", trendInfo.m_fSR);
            //  S상 피상전력
            m_swTrend.WriteLine("S상 피상전력 : {0:F3}", trendInfo.m_fSS);
            //  T상 피상전력
            m_swTrend.WriteLine("T상 피상전력 : {0:F3}", trendInfo.m_fST);
            //  Max DEMAND W
            m_swTrend.WriteLine("Max DEMAND W : {0:F3}", trendInfo.m_fMaxDemandW);
            //  Max W
            m_swTrend.WriteLine("Max W : {0:F3}", trendInfo.m_fMaxW);
            //  Max V0
            m_swTrend.WriteLine("Max V0 : {0:F3}", trendInfo.m_fMaxV0);
            //  R상 전압 THD
            m_swTrend.WriteLine("R상 전압 THD : {0:F3}", trendInfo.m_fThdVR);
            //  S상 전압 THD
            m_swTrend.WriteLine("S상 전압 THD : {0:F3}", trendInfo.m_fThdVS);
            //  T상 전압 THD
            m_swTrend.WriteLine("T상 전압 THD : {0:F3}", trendInfo.m_fThdVT);
            //  R상 전류 THD
            m_swTrend.WriteLine("R상 전류 THD : {0:F3}", trendInfo.m_fThdIR);
            //  S상 전류 THD
            m_swTrend.WriteLine("S상 전류 THD : {0:F3}", trendInfo.m_fThdIS);
            //  T상 전류 THD
            m_swTrend.WriteLine("T상 전류 THD : {0:F3}", trendInfo.m_fThdIT);
            //  R상 전류 TDD
            m_swTrend.WriteLine("R상 전류 TDD : {0:F3}", trendInfo.m_fTddIR);
            //  S상 전류 TDD
            m_swTrend.WriteLine("S상 전류 TDD : {0:F3}", trendInfo.m_fTddIS);
            //  T상 전류 TDD
            m_swTrend.WriteLine("T상 전류 TDD : {0:F3}", trendInfo.m_fTddIT);
            //  R상 전류 Kfactor
            m_swTrend.WriteLine("R상 전류 Kfactor : {0:F3}", trendInfo.m_fKfactorIR);
            //  S상 전류 Kfactor
            m_swTrend.WriteLine("S상 전류 Kfactor : {0:F3}", trendInfo.m_fKfactorIS);
            //  T상 전류 Kfactor
            m_swTrend.WriteLine("T상 전류 Kfactor : {0:F3}", trendInfo.m_fKfactorIT);
            //  R상 전압 기본파
            m_swTrend.WriteLine("R상 전압 기본파 : {0:F3}", trendInfo.m_fHarmonicsVR[0]);
            //  R상 전압 2고조파
            m_swTrend.WriteLine("R상 전압 2고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[1]);
            //  R상 전압 3고조파
            m_swTrend.WriteLine("R상 전압 3고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[2]);
            //  R상 전압 4고조파
            m_swTrend.WriteLine("R상 전압 4고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[3]);
            //  R상 전압 5고조파
            m_swTrend.WriteLine("R상 전압 5고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[4]);
            //  R상 전압 6고조파
            m_swTrend.WriteLine("R상 전압 6고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[5]);
            //  R상 전압 7고조파
            m_swTrend.WriteLine("R상 전압 7고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[6]);
            //  R상 전압 8고조파
            m_swTrend.WriteLine("R상 전압 8고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[7]);
            //  R상 전압 9고조파
            m_swTrend.WriteLine("R상 전압 9고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[8]);
            //  R상 전압 10고조파
            m_swTrend.WriteLine("R상 전압 10고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[9]);
            //  R상 전압 11고조파
            m_swTrend.WriteLine("R상 전압 11고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[10]);
            //  R상 전압 12고조파
            m_swTrend.WriteLine("R상 전압 12고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[11]);
            //  R상 전압 13고조파
            m_swTrend.WriteLine("R상 전압 13고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[12]);
            //  R상 전압 14고조파
            m_swTrend.WriteLine("R상 전압 14고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[13]);
            //  R상 전압 15고조파
            m_swTrend.WriteLine("R상 전압 15고조파 : {0:F3}", trendInfo.m_fHarmonicsVR[14]);
            //  S상 전압 기본파
            m_swTrend.WriteLine("S상 전압 기본파 : {0:F3}", trendInfo.m_fHarmonicsVS[0]);
            //  S상 전압 2고조파
            m_swTrend.WriteLine("S상 전압 2고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[1]);
            //  S상 전압 3고조파
            m_swTrend.WriteLine("S상 전압 3고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[2]);
            //  S상 전압 4고조파
            m_swTrend.WriteLine("S상 전압 4고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[3]);
            //  S상 전압 5고조파
            m_swTrend.WriteLine("S상 전압 5고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[4]);
            //  S상 전압 6고조파
            m_swTrend.WriteLine("S상 전압 6고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[5]);
            //  S상 전압 7고조파
            m_swTrend.WriteLine("S상 전압 7고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[6]);
            //  S상 전압 8고조파
            m_swTrend.WriteLine("S상 전압 8고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[7]);
            //  S상 전압 9고조파
            m_swTrend.WriteLine("S상 전압 9고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[8]);
            //  S상 전압 10고조파
            m_swTrend.WriteLine("S상 전압 10고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[9]);
            //  S상 전압 11고조파
            m_swTrend.WriteLine("S상 전압 11고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[10]);
            //  S상 전압 12고조파
            m_swTrend.WriteLine("S상 전압 12고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[11]);
            //  S상 전압 13고조파
            m_swTrend.WriteLine("S상 전압 13고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[12]);
            //  S상 전압 14고조파
            m_swTrend.WriteLine("S상 전압 14고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[13]);
            //  S상 전압 15고조파
            m_swTrend.WriteLine("S상 전압 15고조파 : {0:F3}", trendInfo.m_fHarmonicsVS[14]);
            //  T상 전압 기본파
            m_swTrend.WriteLine("T상 전압 기본파 : {0:F3}", trendInfo.m_fHarmonicsVT[0]);
            //  T상 전압 2고조파
            m_swTrend.WriteLine("T상 전압 2고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[1]);
            //  T상 전압 3고조파
            m_swTrend.WriteLine("T상 전압 3고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[2]);
            //  T상 전압 4고조파
            m_swTrend.WriteLine("T상 전압 4고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[3]);
            //  T상 전압 5고조파
            m_swTrend.WriteLine("T상 전압 5고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[4]);
            //  T상 전압 6고조파
            m_swTrend.WriteLine("T상 전압 6고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[5]);
            //  T상 전압 7고조파
            m_swTrend.WriteLine("T상 전압 7고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[6]);
            //  T상 전압 8고조파
            m_swTrend.WriteLine("T상 전압 8고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[7]);
            //  T상 전압 9고조파
            m_swTrend.WriteLine("T상 전압 9고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[8]);
            //  T상 전압 10고조파
            m_swTrend.WriteLine("T상 전압 10고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[9]);
            //  T상 전압 11고조파
            m_swTrend.WriteLine("T상 전압 11고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[10]);
            //  T상 전압 12고조파
            m_swTrend.WriteLine("T상 전압 12고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[11]);
            //  T상 전압 13고조파
            m_swTrend.WriteLine("T상 전압 13고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[12]);
            //  T상 전압 14고조파
            m_swTrend.WriteLine("T상 전압 14고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[13]);
            //  T상 전압 15고조파
            m_swTrend.WriteLine("T상 전압 15고조파 : {0:F3}", trendInfo.m_fHarmonicsVT[14]);
            //  R상 전류 기본파
            m_swTrend.WriteLine("R상 전류 기본파 : {0:F3}", trendInfo.m_fHarmonicsIR[0]);
            //  R상 전류 2고조파
            m_swTrend.WriteLine("R상 전류 2고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[1]);
            //  R상 전류 3고조파
            m_swTrend.WriteLine("R상 전류 3고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[2]);
            //  R상 전류 4고조파
            m_swTrend.WriteLine("R상 전류 4고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[3]);
            //  R상 전류 5고조파
            m_swTrend.WriteLine("R상 전류 5고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[4]);
            //  R상 전류 6고조파
            m_swTrend.WriteLine("R상 전류 6고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[5]);
            //  R상 전류 7고조파
            m_swTrend.WriteLine("R상 전류 7고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[6]);
            //  R상 전류 8고조파
            m_swTrend.WriteLine("R상 전류 8고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[7]);
            //  R상 전류 9고조파
            m_swTrend.WriteLine("R상 전류 9고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[8]);
            //  R상 전류 10고조파
            m_swTrend.WriteLine("R상 전류 10고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[9]);
            //  R상 전류 11고조파
            m_swTrend.WriteLine("R상 전류 11고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[10]);
            //  R상 전류 12고조파
            m_swTrend.WriteLine("R상 전류 12고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[11]);
            //  R상 전류 13고조파
            m_swTrend.WriteLine("R상 전류 13고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[12]);
            //  R상 전류 14고조파
            m_swTrend.WriteLine("R상 전류 14고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[13]);
            //  R상 전류 15고조파
            m_swTrend.WriteLine("R상 전류 15고조파 : {0:F3}", trendInfo.m_fHarmonicsIR[14]);
            //  S상 전류 기본파
            m_swTrend.WriteLine("S상 전류 기본파 : {0:F3}", trendInfo.m_fHarmonicsIS[0]);
            //  S상 전류 2고조파
            m_swTrend.WriteLine("S상 전류 2고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[1]);
            //  S상 전류 3고조파
            m_swTrend.WriteLine("S상 전류 3고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[2]);
            //  S상 전류 4고조파
            m_swTrend.WriteLine("S상 전류 4고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[3]);
            //  S상 전류 5고조파
            m_swTrend.WriteLine("S상 전류 5고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[4]);
            //  S상 전류 6고조파
            m_swTrend.WriteLine("S상 전류 6고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[5]);
            //  S상 전류 7고조파
            m_swTrend.WriteLine("S상 전류 7고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[6]);
            //  S상 전류 8고조파
            m_swTrend.WriteLine("S상 전류 8고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[7]);
            //  S상 전류 9고조파
            m_swTrend.WriteLine("S상 전류 9고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[8]);
            //  S상 전류 10고조파
            m_swTrend.WriteLine("S상 전류 10고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[9]);
            //  S상 전류 11고조파
            m_swTrend.WriteLine("S상 전류 11고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[10]);
            //  S상 전류 12고조파
            m_swTrend.WriteLine("S상 전류 12고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[11]);
            //  S상 전류 13고조파
            m_swTrend.WriteLine("S상 전류 13고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[12]);
            //  S상 전류 14고조파
            m_swTrend.WriteLine("S상 전류 14고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[13]);
            //  S상 전류 15고조파
            m_swTrend.WriteLine("S상 전류 15고조파 : {0:F3}", trendInfo.m_fHarmonicsIS[14]);
            //  T상 전류 기본파
            m_swTrend.WriteLine("T상 전류 기본파 : {0:F3}", trendInfo.m_fHarmonicsIT[0]);
            //  T상 전류 2고조파
            m_swTrend.WriteLine("T상 전류 2고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[1]);
            //  T상 전류 3고조파
            m_swTrend.WriteLine("T상 전류 3고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[2]);
            //  T상 전류 4고조파
            m_swTrend.WriteLine("T상 전류 4고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[3]);
            //  T상 전류 5고조파
            m_swTrend.WriteLine("T상 전류 5고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[4]);
            //  T상 전류 6고조파
            m_swTrend.WriteLine("T상 전류 6고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[5]);
            //  T상 전류 7고조파
            m_swTrend.WriteLine("T상 전류 7고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[6]);
            //  T상 전류 8고조파
            m_swTrend.WriteLine("T상 전류 8고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[7]);
            //  T상 전류 9고조파
            m_swTrend.WriteLine("T상 전류 9고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[8]);
            //  T상 전류 10고조파
            m_swTrend.WriteLine("T상 전류 10고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[9]);
            //  T상 전류 11고조파
            m_swTrend.WriteLine("T상 전류 11고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[10]);
            //  T상 전류 12고조파
            m_swTrend.WriteLine("T상 전류 12고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[11]);
            //  T상 전류 13고조파
            m_swTrend.WriteLine("T상 전류 13고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[12]);
            //  T상 전류 14고조파
            m_swTrend.WriteLine("T상 전류 14고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[13]);
            //  T상 전류 15고조파
            m_swTrend.WriteLine("T상 전류 15고조파 : {0:F3}", trendInfo.m_fHarmonicsIT[14]);

            m_swTrend.WriteLine("");
        }
         * */
    }
}
