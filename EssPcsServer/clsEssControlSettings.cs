using System;
using System.ComponentModel;
using EssPcsService;

namespace EssPcsServer
{
    /// <summary>
    /// DevExpress PropertyGridControl의 SelectedObject로 바인딩할 ESS 제어 설정 모델.
    /// checkEssControlInfo()/setEssControlInfo()가 개별 컨트롤로 하던 get/set을 대신한다.
    /// </summary>
    public class clsEssControlSettings
    {
        [Category("충전")]
        [DisplayName("시작 시각(시)")]
        public int ChargeStartHour { get; set; }

        [Category("충전")]
        [DisplayName("시작 시각(분)")]
        public int ChargeStartMinute { get; set; }

        [Category("충전")]
        [DisplayName("종료 시각(시)")]
        public int ChargeEndHour { get; set; }

        [Category("충전")]
        [DisplayName("종료 시각(분)")]
        public int ChargeEndMinute { get; set; }

        [Category("충전")]
        [DisplayName("SOC 제한(%)")]
        public float ChargeSocLimit { get; set; }

        [Category("충전")]
        [DisplayName("출력 제한(kW)")]
        public float ChargeOutputLimit { get; set; }

        [Category("방전")]
        [DisplayName("시작 시각(시)")]
        public int DischargeStartHour { get; set; }

        [Category("방전")]
        [DisplayName("시작 시각(분)")]
        public int DischargeStartMinute { get; set; }

        [Category("방전")]
        [DisplayName("종료 시각(시)")]
        public int DischargeEndHour { get; set; }

        [Category("방전")]
        [DisplayName("종료 시각(분)")]
        public int DischargeEndMinute { get; set; }

        [Category("방전")]
        [DisplayName("SOC 제한(%)")]
        public float DischargeSocLimit { get; set; }

        [Category("방전")]
        [DisplayName("출력 제한(kW)")]
        public float DischargeOutputLimit { get; set; }

        [Category("발전량 연계")]
        [DisplayName("사용 여부")]
        public bool EssGenUse { get; set; }

        [Category("발전량 연계")]
        [DisplayName("발전 전력(kW)")]
        public float EssGenOutPower { get; set; }

        [Category("발전량 연계")]
        [DisplayName("충전율(%)")]
        public float EssGenOutPercent { get; set; }

        [Category("제어")]
        [DisplayName("제어 주기(초)")]
        public int EssControlPeriod { get; set; }

        [Category("제어")]
        [DisplayName("EMS 동작")]
        public bool EmsRun { get; set; }

        /// <summary>WCF 서비스의 현재 값을 읽어와 채운다 (checkEssControlInfo()의 읽기 부분과 동일한 값).</summary>
        public void LoadFrom(IEssPcsService proxy)
        {
            bool bSet;
            DateTime dt;

            dt = proxy.getDtEssChargeStart(ref bSet);
            ChargeStartHour = dt.Hour;
            ChargeStartMinute = dt.Minute;

            dt = proxy.getDtEssChargeEnd(ref bSet);
            ChargeEndHour = dt.Hour;
            ChargeEndMinute = dt.Minute;

            ChargeSocLimit = proxy.getEssChargeSocLimit(ref bSet);
            ChargeOutputLimit = proxy.getEssChargeOutput(ref bSet);

            dt = proxy.getDtEssDischargeStart(ref bSet);
            DischargeStartHour = dt.Hour;
            DischargeStartMinute = dt.Minute;

            dt = proxy.getDtEssDischargeEnd(ref bSet);
            DischargeEndHour = dt.Hour;
            DischargeEndMinute = dt.Minute;

            DischargeSocLimit = proxy.getEssDischargeSocLimit(ref bSet);
            DischargeOutputLimit = proxy.getEssDischargeOutput(ref bSet);

            EssGenUse = proxy.getEssGen(ref bSet);
            EssGenOutPower = proxy.getEssGenOutput(ref bSet);
            EssGenOutPercent = proxy.getEssGenPercent(ref bSet);

            EssControlPeriod = proxy.getEssControlPeriod(ref bSet);
            EmsRun = proxy.getEmsRun(ref bSet) == 1;
        }

        /// <summary>현재 값을 WCF 서비스에 반영한다 (setEssControlInfo()와 동일한 동작).</summary>
        public void ApplyTo(IEssPcsService proxy)
        {
            DateTime today = DateTime.Now.Date;

            proxy.setDtEssChargeStart(today.AddHours(ChargeStartHour).AddMinutes(ChargeStartMinute), false);
            proxy.setDtEssChargeEnd(today.AddHours(ChargeEndHour).AddMinutes(ChargeEndMinute), false);
            proxy.setEssChargeSocLimit(ChargeSocLimit, false);
            proxy.setEssChargeOutput(ChargeOutputLimit, false);

            proxy.setDtEssDischargeStart(today.AddHours(DischargeStartHour).AddMinutes(DischargeStartMinute), false);
            proxy.setDtEssDischargeEnd(today.AddHours(DischargeEndHour).AddMinutes(DischargeEndMinute), false);
            proxy.setEssDischargeSocLimit(DischargeSocLimit, false);
            proxy.setEssDischargeOutput(DischargeOutputLimit, false);

            proxy.setEssGen(EssGenUse, false);
            proxy.setEssGenOutput(EssGenOutPower, false);
            proxy.setEssGenPercent(EssGenOutPercent, false);

            proxy.setEssControlPeriod(EssControlPeriod, false);
            proxy.setEmsRun(EmsRun ? 1 : 0, false);
        }
    }
}
