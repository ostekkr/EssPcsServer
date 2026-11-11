using System;
using System.Collections.Generic;

namespace EssPcsServer
{
    /// <summary>EMS 가 쓰는 현재 시각. 시험할 때 Now 를 바꿔 시각을 주입한다.</summary>
    public static class clsClock
    {
        public static Func<DateTime> Now = () => DateTime.Now;

        /// <summary>현재 시각을 분 단위로 자른 값. 시나리오 시각 비교에 쓴다.</summary>
        public static DateTime NowMinute()
        {
            DateTime dt = Now();
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
        }
    }

    /// <summary>runEms 가 이번 주기에 실행할 ESS 운영 종류. 값은 운영모드 콤보 인덱스와 같다.</summary>
    public enum enEmsMode
    {
        General = 0,
        PvCurtailment = 1,
        PvEssControl = 2,
        FrequencySupport = 3,
        LoadReduction = 4,
    }

    /// <summary>ESS 판단 결과. Status -1 충전, 1 방전, 0 정지. PowerKw 는 크기(0 이상).</summary>
    public class clsEssDecision
    {
        public int Status;
        public float PowerKw;
        public string Reason = "";

        /// <summary>PCS 유효전력 지령(충전 음수, 방전 양수). 0.1 kW 단위로 반올림.</summary>
        public float OutputP
        {
            get { return (float)Math.Round(Status * PowerKw, 1); }
        }

        public override string ToString()
        {
            return string.Format("{0} {1:0.0}kW {2}", Status < 0 ? "충전" : (Status > 0 ? "방전" : "정지"), PowerKw, Reason);
        }
    }

    /// <summary>
    /// 시나리오 버전 2 의 EMS 판단(화면·통신과 무관한 순수 계산).
    /// - PV 출력제한(PVCurtailment)은 PV 만 다루므로 ESS 선택과 따로 계산한다.
    /// - ESS 는 부하감축 > 주파수 지원 > PV-ESS 연계 > 일반운영 순으로 하나만 실행한다.
    /// </summary>
    public static class clsEmsPlanner
    {
        public static readonly string[] PvResourceIds = { "PV-001", "PV-002", "PV-003" };

        public static int PvIndex(string strResourceId)
        {
            return Array.IndexOf(PvResourceIds, strResourceId);
        }

        /// <summary>전체 구간(startTime~endTime) 안인지. 버전 1 의 기존 판정과 같다(양 끝 포함).</summary>
        public static bool InWindow(clsScenarioCommandBase cmd, DateTime now)
        {
            return (cmd != null) && (now >= cmd.StartTime) && (now <= cmd.EndTime);
        }

        /// <summary>버전 2 에서 지금 적용할 구간. 없으면 null.</summary>
        public static clsScenarioSegment ActiveSegment(clsScenarioCommandBase cmd, DateTime now)
        {
            if (cmd == null || !cmd.IsVersion2 || cmd.Segments == null)
            {
                return null;
            }

            foreach (clsScenarioSegment seg in cmd.Segments)
            {
                if (seg != null && seg.Contains(now))
                {
                    return seg;
                }
            }

            return null;
        }

        /// <summary>
        /// PV 별 출력 상한. 값이 null 인 PV 는 제한 해제 대상이다.
        /// 버전 1 은 전체 구간 동안 resources 를, 버전 2 는 현재 구간의 resources 를 쓴다.
        /// </summary>
        public static float?[] PvLimits(clsPvCurtailmentCommand cmd, DateTime now)
        {
            float?[] limits = new float?[PvResourceIds.Length];

            if (cmd == null)
            {
                return limits;
            }

            List<clsPvResourceLimit> resources = null;

            if (cmd.IsVersion2)
            {
                clsScenarioSegment seg = ActiveSegment(cmd, now);
                resources = (seg != null) ? seg.Resources : null;
            }
            else if (InWindow(cmd, now))
            {
                resources = cmd.Resources;
            }

            if (resources != null)
            {
                foreach (clsPvResourceLimit res in resources)
                {
                    int i = PvIndex(res.ResourceId);

                    if (i >= 0)
                    {
                        limits[i] = res.MaxOutputPower;
                    }
                }
            }

            return limits;
        }

        /// <summary>ESS 를 맡을 시나리오를 우선순위대로 고른다. 전체 구간 안이면 활성이다.</summary>
        public static enEmsMode SelectEssMode(clsScenarioCommandSet set, DateTime now, out clsScenarioCommandBase selected)
        {
            selected = null;

            if (set == null)
            {
                return enEmsMode.General;
            }

            if (InWindow(set.EssLoadReduction, now))
            {
                selected = set.EssLoadReduction;
                return enEmsMode.LoadReduction;
            }

            if (InWindow(set.EssFrequencySupport, now))
            {
                selected = set.EssFrequencySupport;
                return enEmsMode.FrequencySupport;
            }

            if (InWindow(set.PvEssControl, now))
            {
                selected = set.PvEssControl;
                return enEmsMode.PvEssControl;
            }

            return enEmsMode.General;
        }

        /// <summary>
        /// 버전 2 ESS 구간의 판단. 구간이 없으면(지령 없음 시간) 정지한다.
        /// SoC 는 targetSoc 와 [일반운영] SOC 한계 중 먼저 닿는 쪽에서 멈춘다.
        /// </summary>
        public static clsEssDecision DecideEss(clsScenarioSegment seg, float fSoc, bool bSocValid,
            float fChargeSocLimit, float fDischargeSocLimit, float fPcsRatingKw, float fPvGenKw)
        {
            clsEssDecision d = new clsEssDecision();

            if (seg == null)
            {
                d.Reason = "지령 없음 시간";
                return d;
            }

            if (!bSocValid)
            {
                d.Reason = "SoC 갱신 없음";
                return d;
            }

            float fPower = seg.PowerKw.HasValue ? seg.PowerKw.Value : 0;

            if (seg.Mode == "CHARGE")
            {
                if (fSoc >= fChargeSocLimit)
                {
                    d.Reason = "충전 SOC 한계 도달";
                    return d;
                }

                if (seg.TargetSoc.HasValue && fSoc >= seg.TargetSoc.Value)
                {
                    d.Reason = "목표 SoC 도달";
                    return d;
                }

                if (seg.PowerBasis == "PV")
                {
                    float fPv = Math.Max(0, fPvGenKw) * (seg.ChargeRatio.HasValue ? seg.ChargeRatio.Value : 0) * 0.01f;
                    fPower = Math.Min(fPower, fPv);
                }

                d.Status = -1;
            }
            else if (seg.Mode == "DISCHARGE")
            {
                if (fSoc <= fDischargeSocLimit)
                {
                    d.Reason = "방전 SOC 한계 도달";
                    return d;
                }

                if (seg.TargetSoc.HasValue && fSoc <= seg.TargetSoc.Value)
                {
                    d.Reason = "목표 SoC 도달";
                    return d;
                }

                d.Status = 1;
            }
            else
            {
                d.Reason = "알 수 없는 mode";
                return d;
            }

            if (fPcsRatingKw > 0 && fPower > fPcsRatingKw)
            {
                fPower = fPcsRatingKw;
            }

            d.PowerKw = (float)Math.Round(Math.Max(0, fPower), 1);
            return d;
        }

        /// <summary>지령 비교는 0.1 kW 단위로 반올림해 같은 명령의 반복을 막는다.</summary>
        public static bool SameKw(float a, float b)
        {
            return Math.Round(a, 1) == Math.Round(b, 1);
        }
    }
}
