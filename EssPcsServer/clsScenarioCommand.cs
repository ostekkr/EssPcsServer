using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EssPcsServer
{
    /// <summary>
    /// 세 시나리오(PVCurtailment / PVESSControl / ESSFrequencySupport) 공통 헤더.
    /// PropertyGridControl.SelectedObject로 바인딩하면 "공통" 카테고리로 묶여 표시된다.
    /// </summary>
    public abstract class clsScenarioCommandBase
    {
        [Category("공통")]
        [DisplayName("시나리오명")]
        [JsonProperty("scenarioName")]
        public string ScenarioName { get; set; }

        [Category("공통")]
        [DisplayName("명령 ID")]
        [JsonProperty("commandId")]
        public string CommandId { get; set; }

        [Category("공통")]
        [DisplayName("발행 시각")]
        [JsonProperty("issuedTime")]
        public DateTimeOffset IssuedTime { get; set; }

        [Category("공통")]
        [DisplayName("시작 시각")]
        [JsonProperty("startTime")]
        public DateTimeOffset StartTime { get; set; }

        [Category("공통")]
        [DisplayName("종료 시각")]
        [JsonProperty("endTime")]
        public DateTimeOffset EndTime { get; set; }

        public override string ToString()
        {
            return string.IsNullOrEmpty(CommandId) ? ScenarioName : CommandId;
        }
    }

    /// <summary>PVCurtailment의 resources[] 배열 원소. ExpandableObjectConverter로 그리드에서 펼쳐 보인다.</summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class clsPvResourceLimit
    {
        [DisplayName("자원 ID")]
        [JsonProperty("resourceId")]
        public string ResourceId { get; set; }

        [DisplayName("최대 출력(MW)")]
        [JsonProperty("maxOutputPower")]
        public float MaxOutputPower { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1}MW)", ResourceId, MaxOutputPower);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class clsPvCurtailmentCommand : clsScenarioCommandBase
    {
        [Category("제어 대상")]
        [DisplayName("PV 자원 목록")]
        [JsonProperty("resources")]
        public List<clsPvResourceLimit> Resources { get; set; } = new List<clsPvResourceLimit>();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class clsChargeRule
    {
        [DisplayName("충전율(%)")]
        [JsonProperty("chargeRatio")]
        public int ChargeRatio { get; set; }

        public override string ToString()
        {
            return string.Format("{0}%", ChargeRatio);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class clsDischargeRule
    {
        [DisplayName("방전 출력 비율(%)")]
        [JsonProperty("dischargePowerPercent")]
        public int DischargePowerPercent { get; set; }

        public override string ToString()
        {
            return string.Format("{0}%", DischargePowerPercent);
        }
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class clsPvEssControlCommand : clsScenarioCommandBase
    {
        [Category("제어 대상")]
        [DisplayName("PV 자원 ID")]
        [JsonProperty("pvResourceId")]
        public string PvResourceId { get; set; }

        [Category("제어 대상")]
        [DisplayName("ESS 자원 ID")]
        [JsonProperty("essResourceId")]
        public string EssResourceId { get; set; }

        [Category("충전 규칙")]
        [DisplayName("충전 규칙")]
        [JsonProperty("chargeRule")]
        public clsChargeRule ChargeRule { get; set; } = new clsChargeRule();

        [Category("방전 규칙")]
        [DisplayName("방전 규칙")]
        [JsonProperty("dischargeRule")]
        public clsDischargeRule DischargeRule { get; set; } = new clsDischargeRule();
    }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class clsEssFrequencySupportCommand : clsScenarioCommandBase
    {
        [Category("제어 대상")]
        [DisplayName("ESS 자원 ID")]
        [JsonProperty("essResourceId")]
        public string EssResourceId { get; set; }

        [Category("동작")]
        [DisplayName("운전 모드")]
        [JsonProperty("operationMode")]
        public string OperationMode { get; set; }

        [Category("동작")]
        [DisplayName("목표 출력 비율(%)")]
        [JsonProperty("targetPowerPercent")]
        public int TargetPowerPercent { get; set; }
    }

    /// <summary>scenarioName을 보고 알맞은 커맨드 타입으로 역직렬화한다.</summary>
    public static class clsScenarioCommandParser
    {
        public static clsScenarioCommandBase Parse(string json)
        {
            JObject root = JObject.Parse(json);
            string scenarioName = (string)root["scenarioName"];

            switch (scenarioName)
            {
                case "PVCurtailment":
                    return root.ToObject<clsPvCurtailmentCommand>();
                case "PVESSControl":
                    return root.ToObject<clsPvEssControlCommand>();
                case "ESSFrequencySupport":
                    return root.ToObject<clsEssFrequencySupportCommand>();
                default:
                    throw new NotSupportedException("알 수 없는 scenarioName: " + scenarioName);
            }
        }
    }

    /// <summary>
    /// PVCurtailment / PVESSControl / ESSFrequencySupport 3개 커맨드를 한 번에 담아
    /// PropertyGridControl.SelectedObject 하나로 세 시나리오를 동시에 표시하기 위한 컨테이너.
    /// 각 항목은 ExpandableObjectConverter가 붙어 있어 그리드에서 펼침/접힘 노드로 보인다.
    /// </summary>
    public class clsScenarioCommandSet
    {
        [Category("시나리오")]
        [DisplayName("PV 출력제한 (PVCurtailment)")]
        public clsPvCurtailmentCommand PvCurtailment { get; set; }

        [Category("시나리오")]
        [DisplayName("PV-ESS 연계 제어 (PVESSControl)")]
        public clsPvEssControlCommand PvEssControl { get; set; }

        [Category("시나리오")]
        [DisplayName("ESS 주파수 지원 (ESSFrequencySupport)")]
        public clsEssFrequencySupportCommand EssFrequencySupport { get; set; }

        /// <summary>scenarioName을 보고 알맞은 슬롯에 채운다. 같은 시나리오가 다시 들어오면 덮어쓴다.</summary>
        public void AddJson(string json)
        {
            clsScenarioCommandBase cmd = clsScenarioCommandParser.Parse(json);

            clsPvCurtailmentCommand pvCurtailment = cmd as clsPvCurtailmentCommand;
            if (pvCurtailment != null)
            {
                PvCurtailment = pvCurtailment;
                return;
            }

            clsPvEssControlCommand pvEssControl = cmd as clsPvEssControlCommand;
            if (pvEssControl != null)
            {
                PvEssControl = pvEssControl;
                return;
            }

            clsEssFrequencySupportCommand essFrequencySupport = cmd as clsEssFrequencySupportCommand;
            if (essFrequencySupport != null)
            {
                EssFrequencySupport = essFrequencySupport;
            }
        }

        /// <summary>3개(또는 그 이하) JSON 문자열을 순서 상관없이 넣어 하나의 세트로 만든다.</summary>
        public static clsScenarioCommandSet FromJsonArray(IEnumerable<string> jsonList)
        {
            clsScenarioCommandSet set = new clsScenarioCommandSet();

            foreach (string json in jsonList)
            {
                set.AddJson(json);
            }

            return set;
        }
    }
}
