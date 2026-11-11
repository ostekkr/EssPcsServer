using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace EssPcsServer
{
    /// <summary>날짜/시각을 "yyyy-MM-dd HH:mm" 형식(초·오프셋 없이)으로 그리드에 표시한다.</summary>
    internal class clsDateTimeMinuteConverter : DateTimeConverter
    {
        private const string Format = "yyyy-MM-dd HH:mm";

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is DateTime dt)
            {
                return dt.ToString(Format, CultureInfo.InvariantCulture);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            string s = value as string;

            if (s != null && DateTime.TryParse(s, culture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }

    /// <summary>
    /// PropertyGridControl 표시용으로 붙인 [TypeConverter(ExpandableObjectConverter)]를
    /// Newtonsoft.Json이 "문자열로 변환 가능한 타입"으로 오인해 JSON 객체 역직렬화에
    /// 실패하는 문제(Cannot deserialize the current JSON object... requires a JSON string
    /// value)를 막기 위해, TypeConverter가 붙은 타입도 항상 일반 객체 컨트랙트로 취급한다.
    /// </summary>
    internal class clsIgnoreTypeConverterContractResolver : DefaultContractResolver
    {
        public static readonly clsIgnoreTypeConverterContractResolver Instance = new clsIgnoreTypeConverterContractResolver();

        protected override JsonContract CreateContract(Type objectType)
        {
            if (objectType.IsDefined(typeof(TypeConverterAttribute), true))
            {
                return CreateObjectContract(objectType);
            }

            return base.CreateContract(objectType);
        }
    }

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
        [TypeConverter(typeof(clsDateTimeMinuteConverter))]
        public DateTime IssuedTime { get; set; }

        [Category("공통")]
        [DisplayName("시작 시각")]
        [JsonProperty("startTime")]
        [TypeConverter(typeof(clsDateTimeMinuteConverter))]
        public DateTime StartTime { get; set; }

        [Category("공통")]
        [DisplayName("종료 시각")]
        [JsonProperty("endTime")]
        [TypeConverter(typeof(clsDateTimeMinuteConverter))]
        public DateTime EndTime { get; set; }

        /// <summary>규격 버전. 없으면 1(구간 없이 시작~종료 전체에 한 지령).</summary>
        [Category("공통")]
        [DisplayName("규격 버전")]
        [JsonProperty("version")]
        public int Version { get; set; } = 1;

        /// <summary>버전 2 시간 구간 목록. 버전 1 에서는 비어 있다.</summary>
        [Category("공통")]
        [DisplayName("시간 구간")]
        [JsonProperty("segments")]
        public List<clsScenarioSegment> Segments { get; set; } = new List<clsScenarioSegment>();

        /// <summary>true 이면 같은 종류·같은 commandId 의 시나리오를 취소하는 행이다.</summary>
        [Browsable(false)]
        [JsonProperty("cancel")]
        public bool Cancel { get; set; }

        [Browsable(false)]
        [JsonIgnore]
        public bool IsVersion2 { get { return Version >= 2; } }

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

        [DisplayName("최대 출력(kW)")]
        [JsonProperty("maxOutputPower")]
        public float MaxOutputPower { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1}kW)", ResourceId, MaxOutputPower);
        }
    }

    /// <summary>
    /// 버전 2 시간 구간. start ≤ 현재 < end 인 구간을 적용한다.
    /// PVCurtailment 는 Resources, ESS 종류는 Mode·PowerKw·TargetSoc 등을 쓴다.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class clsScenarioSegment
    {
        [DisplayName("시작")]
        [JsonProperty("start")]
        [TypeConverter(typeof(clsDateTimeMinuteConverter))]
        public DateTime Start { get; set; }

        [DisplayName("끝")]
        [JsonProperty("end")]
        [TypeConverter(typeof(clsDateTimeMinuteConverter))]
        public DateTime End { get; set; }

        [DisplayName("PV 제한 목록")]
        [JsonProperty("resources")]
        public List<clsPvResourceLimit> Resources { get; set; }

        [DisplayName("운전 모드")]
        [JsonProperty("mode")]
        public string Mode { get; set; }

        [DisplayName("출력(kW)")]
        [JsonProperty("powerKw")]
        public float? PowerKw { get; set; }

        [DisplayName("출력 기준")]
        [JsonProperty("powerBasis")]
        public string PowerBasis { get; set; }

        [DisplayName("충전 비율(%)")]
        [JsonProperty("chargeRatio")]
        public float? ChargeRatio { get; set; }

        [DisplayName("목표 SoC(%)")]
        [JsonProperty("targetSoc")]
        public float? TargetSoc { get; set; }

        public bool Contains(DateTime now)
        {
            return (Start <= now) && (now < End);
        }

        public override string ToString()
        {
            string strTime = string.Format("{0:HH:mm}~{1:HH:mm}", Start, End);

            if (Resources != null && Resources.Count > 0)
            {
                return strTime + " " + string.Join(", ", Resources);
            }

            return string.Format("{0} {1} {2}kW{3}", strTime, Mode, PowerKw, TargetSoc.HasValue ? " SoC " + TargetSoc.Value + "%" : "");
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

    /// <summary>S6 PV-ESS 활용 계통 부하감축(수요반응). 버전 2 전용.</summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class clsEssLoadReductionCommand : clsScenarioCommandBase
    {
        [Category("제어 대상")]
        [DisplayName("PV 자원 ID")]
        [JsonProperty("pvResourceId")]
        public string PvResourceId { get; set; }

        [Category("제어 대상")]
        [DisplayName("ESS 자원 ID")]
        [JsonProperty("essResourceId")]
        public string EssResourceId { get; set; }

        [Category("동작")]
        [DisplayName("DR 발령 ID")]
        [JsonProperty("eventId")]
        public string EventId { get; set; }
    }

    /// <summary>scenarioName을 보고 알맞은 커맨드 타입으로 역직렬화한다.</summary>
    public static class clsScenarioCommandParser
    {
        public const int MaxSupportedVersion = 2;

        private static readonly JsonSerializer s_serializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            ContractResolver = clsIgnoreTypeConverterContractResolver.Instance
        });

        public static clsScenarioCommandBase Parse(string json)
        {
            return Parse(JObject.Parse(json));
        }

        public static clsScenarioCommandBase Parse(JObject root)
        {
            string scenarioName = (string)root["scenarioName"];
            clsScenarioCommandBase cmd;

            switch (scenarioName)
            {
                case "PVCurtailment":
                    cmd = root.ToObject<clsPvCurtailmentCommand>(s_serializer);
                    break;
                case "PVESSControl":
                    cmd = root.ToObject<clsPvEssControlCommand>(s_serializer);
                    break;
                case "ESSFrequencySupport":
                    cmd = root.ToObject<clsEssFrequencySupportCommand>(s_serializer);
                    break;
                case "ESSLoadReduction":
                    cmd = root.ToObject<clsEssLoadReductionCommand>(s_serializer);
                    break;
                default:
                    throw new NotSupportedException("알 수 없는 scenarioName: " + scenarioName);
            }

            if (cmd.Segments == null)
            {
                cmd.Segments = new List<clsScenarioSegment>();
            }

            string strError = Validate(cmd);

            if (strError != null)
            {
                throw new FormatException(string.Format("{0}({1}) : {2}", scenarioName, cmd.CommandId, strError));
            }

            return cmd;
        }

        /// <summary>JSON 객체 하나 또는 배열(여러 시나리오)을 받아 차례로 돌려준다.</summary>
        public static List<JObject> SplitJson(string json)
        {
            List<JObject> list = new List<JObject>();
            JToken token = JToken.Parse(json);

            if (token.Type == JTokenType.Array)
            {
                foreach (JToken item in (JArray)token)
                {
                    if (item.Type == JTokenType.Object)
                    {
                        list.Add((JObject)item);
                    }
                }
            }
            else if (token.Type == JTokenType.Object)
            {
                list.Add((JObject)token);
            }

            return list;
        }

        /// <summary>규격 위반이면 이유를, 정상이면 null 을 돌려준다.</summary>
        public static string Validate(clsScenarioCommandBase cmd)
        {
            if (cmd.Version < 1 || cmd.Version > MaxSupportedVersion)
            {
                return "지원하지 않는 version " + cmd.Version;
            }

            if (string.IsNullOrEmpty(cmd.CommandId))
            {
                return "commandId 없음";
            }

            if (cmd.Cancel)
            {
                return null;
            }

            if (cmd.Version == 1)
            {
                return (cmd is clsEssLoadReductionCommand) ? "ESSLoadReduction 은 version 2 가 필요" : null;
            }

            if (cmd.EndTime <= cmd.StartTime)
            {
                return "endTime 이 startTime 보다 늦어야 함";
            }

            if (cmd.Segments.Count == 0)
            {
                return "segments 없음";
            }

            bool bPv = cmd is clsPvCurtailmentCommand;

            for (int i = 0; i < cmd.Segments.Count; i++)
            {
                clsScenarioSegment seg = cmd.Segments[i];

                if (seg == null || seg.End <= seg.Start)
                {
                    return string.Format("segments[{0}] 시작·끝이 잘못됨", i);
                }

                if (seg.Start < cmd.StartTime || seg.End > cmd.EndTime)
                {
                    return string.Format("segments[{0}] 가 전체 구간 밖", i);
                }

                if (i > 0 && seg.Start < cmd.Segments[i - 1].End)
                {
                    return string.Format("segments[{0}] 가 앞 구간과 겹치거나 순서가 다름", i);
                }

                if (bPv)
                {
                    if (seg.Resources == null || seg.Resources.Count == 0)
                    {
                        return string.Format("segments[{0}] resources 없음", i);
                    }

                    foreach (clsPvResourceLimit res in seg.Resources)
                    {
                        if (clsEmsPlanner.PvIndex(res.ResourceId) < 0)
                        {
                            return string.Format("segments[{0}] 알 수 없는 resourceId {1}", i, res.ResourceId);
                        }

                        if (!(res.MaxOutputPower > 0))
                        {
                            return string.Format("segments[{0}] maxOutputPower 는 0 보다 커야 함", i);
                        }
                    }
                }
                else
                {
                    if (seg.Mode != "CHARGE" && seg.Mode != "DISCHARGE")
                    {
                        return string.Format("segments[{0}] mode 는 CHARGE 또는 DISCHARGE", i);
                    }

                    if (!seg.PowerKw.HasValue || seg.PowerKw.Value < 0)
                    {
                        return string.Format("segments[{0}] powerKw 는 0 이상", i);
                    }

                    if (seg.PowerBasis != null && seg.PowerBasis != "FIXED" && seg.PowerBasis != "PV")
                    {
                        return string.Format("segments[{0}] powerBasis 는 FIXED 또는 PV", i);
                    }

                    if (seg.PowerBasis == "PV" && (seg.Mode != "CHARGE" || !seg.ChargeRatio.HasValue))
                    {
                        return string.Format("segments[{0}] powerBasis=PV 는 CHARGE 와 chargeRatio 가 필요", i);
                    }

                    if (seg.TargetSoc.HasValue && (seg.TargetSoc.Value < 0 || seg.TargetSoc.Value > 100))
                    {
                        return string.Format("segments[{0}] targetSoc 범위 0~100", i);
                    }
                }
            }

            return null;
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

        [Category("시나리오")]
        [DisplayName("부하감축 (ESSLoadReduction)")]
        public clsEssLoadReductionCommand EssLoadReduction { get; set; }

        /// <summary>
        /// JSON 객체 하나 또는 배열을 받아 종류별 슬롯에 채운다. 같은 종류가 다시 들어오면 덮어쓴다.
        /// 잘못된 항목은 건너뛰고 그 이유를 돌려준다(없으면 빈 목록).
        /// </summary>
        public List<string> AddJson(string json)
        {
            List<string> errors = new List<string>();

            foreach (JObject obj in clsScenarioCommandParser.SplitJson(json))
            {
                try
                {
                    Add(clsScenarioCommandParser.Parse(obj));
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                }
            }

            return errors;
        }

        /// <summary>취소 행이면 같은 종류·같은 commandId 슬롯을 비운다. 그 밖에는 슬롯을 덮어쓴다.</summary>
        public void Add(clsScenarioCommandBase cmd)
        {
            if (cmd.Cancel)
            {
                if (PvCurtailment != null && cmd is clsPvCurtailmentCommand && PvCurtailment.CommandId == cmd.CommandId) PvCurtailment = null;
                if (PvEssControl != null && cmd is clsPvEssControlCommand && PvEssControl.CommandId == cmd.CommandId) PvEssControl = null;
                if (EssFrequencySupport != null && cmd is clsEssFrequencySupportCommand && EssFrequencySupport.CommandId == cmd.CommandId) EssFrequencySupport = null;
                if (EssLoadReduction != null && cmd is clsEssLoadReductionCommand && EssLoadReduction.CommandId == cmd.CommandId) EssLoadReduction = null;
                return;
            }

            clsEssLoadReductionCommand loadReduction = cmd as clsEssLoadReductionCommand;
            if (loadReduction != null)
            {
                EssLoadReduction = loadReduction;
                return;
            }

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
