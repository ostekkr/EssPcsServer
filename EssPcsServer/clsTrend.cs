using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EssPcsService;

namespace EssPcsServer
{
    public class clsTrend
    {
        public DateTime m_dtSave = DateTime.Now;

        private Int32 getInt32Value(Byte[] bArr, Int32 nIndex)
        {
            Int32 nValue = 0;
            Byte[] buf = new byte[4];

            buf[0] = bArr[nIndex + 3];
            buf[1] = bArr[nIndex + 2];
            buf[2] = bArr[nIndex + 1];
            buf[3] = bArr[nIndex + 0];

            nValue = BitConverter.ToInt32(buf, 0);

            return nValue;
        }

        private UInt32 getUInt32Value(Byte[] bArr, Int32 nIndex)
        {
            UInt32 nValue = 0;
            Byte[] buf = new byte[4];

            buf[3] = bArr[nIndex + 3];
            buf[2] = bArr[nIndex + 2];
            buf[1] = bArr[nIndex + 1];
            buf[0] = bArr[nIndex + 0];

            nValue = BitConverter.ToUInt32(buf, 0);

            return nValue;
        }

        private Int16 getInt16Value(Byte[] bArr, Int32 nIndex)
        {
            Int16 nValue = 0;
            Byte[] buf = new byte[2];

            buf[1] = bArr[nIndex + 0];
            buf[0] = bArr[nIndex + 1];

            nValue = BitConverter.ToInt16(buf, 0);

            return nValue;
        }

        private UInt16 getUInt16Value(Byte[] bArr, Int32 nIndex)
        {
            UInt16 nValue = 0;
            Byte[] buf = new byte[2];

            buf[1] = bArr[nIndex + 0];
            buf[0] = bArr[nIndex + 1];

            nValue = BitConverter.ToUInt16(buf, 0);

            return nValue;
        }

        private float getFloatValue(Byte[] bArr, Int32 nIndex)
        {
            float fValue = 0;
            Byte[] buf = new byte[4];

            buf[3] = bArr[nIndex + 0];
            buf[2] = bArr[nIndex + 1];
            buf[1] = bArr[nIndex + 2];
            buf[0] = bArr[nIndex + 3];

            fValue = BitConverter.ToSingle(buf, 0);

            return fValue;
        }

        public void ParseTrendInfo(clsEssPcsTrendInfo s)
        {
            Int32 nIndex = 0;

            s.dtTime = DateTime.Now;

            nIndex = 0;

            //  Function Code 4

            //  000: AC 전류 - R
            s.fCurrentR = getUInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  001: AC 전류 - S
            s.fCurrentS = getUInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  002: AC 전류 - T
            s.fCurrentT = getUInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  AC 전류
            s.fGRID_Current = s.fCurrentR + s.fCurrentS + s.fCurrentT;

            //  003: DC 전류
            s.fBatteryCurrent = getUInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  004: AC 전압 - R
            s.fVoltR = getUInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  005: AC 전압 - S
            s.fVoltS = getUInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  006: AC 전압 - T
            s.fVoltT = getUInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  AC 퍙균 전압
            s.fGRID_Volt = (s.fVoltR + s.fVoltS + s.fVoltT) / 3;

            //  007: DC 전압  
            s.fBatteryVoltage = getUInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  008: 유효 전력
            s.fGRID_Power = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  009: 무효 전력
            s.fGRID_ReactivePower = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  010: DC 전력
            s.fBatteryPower = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  011: 주파수
            s.fGRID_Frequency = getInt16Value(s.bTrendData, nIndex) * 0.001f;
            nIndex += 2;

            //  012: 역률
            s.fTotalPowerFactor = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  013: Temperature Internal-PCS
            s.fTemp_InternalPcs = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  014: INV-Status
            s.nInvStatus = getUInt16Value(s.bTrendData, nIndex);
            nIndex += 2;

            //  015: Warning Status
            s.m_nWarningStatus = getUInt16Value(s.bTrendData, nIndex);
            nIndex += 2;

            s.m_flagWarningStatus.bit00_InverterFault = (s.m_nWarningStatus & 0x0001) != 0 ? true : false;
            s.m_flagWarningStatus.bit01_GridOverVoltage = (s.m_nWarningStatus & 0x0002) != 0 ? true : false;
            s.m_flagWarningStatus.bit02_GridOverFrequency = (s.m_nWarningStatus & 0x0004) != 0 ? true : false;
            s.m_flagWarningStatus.bit03_Reserved = (s.m_nWarningStatus & 0x0008) != 0 ? true : false;
            s.m_flagWarningStatus.bit04_TransformerOverHeat = (s.m_nWarningStatus & 0x0010) != 0 ? true : false;
            s.m_flagWarningStatus.bit05_FilterInductorOverHeat = (s.m_nWarningStatus & 0x0020) != 0 ? true : false;
            s.m_flagWarningStatus.bit06_FilterCapacitorOverHeat = (s.m_nWarningStatus & 0x0040) != 0 ? true : false;
            s.m_flagWarningStatus.bit07_EnclosureInternalOverHeat = (s.m_nWarningStatus & 0x0080) != 0 ? true : false;
            s.m_flagWarningStatus.bit08_FaultMainLoss = (s.m_nWarningStatus & 0x0100) != 0 ? true : false;
            s.m_flagWarningStatus.bit09_DcImd1 = (s.m_nWarningStatus & 0x0200) != 0 ? true : false;
            s.m_flagWarningStatus.bit10_ControlSpd1Open = (s.m_nWarningStatus & 0x0400) != 0 ? true : false;
            s.m_flagWarningStatus.bit11_BatteryOverHeat = (s.m_nWarningStatus & 0x0800) != 0 ? true : false;
            s.m_flagWarningStatus.bit12_DcCb1Trip = (s.m_nWarningStatus & 0x1000) != 0 ? true : false;
            s.m_flagWarningStatus.bit13_AcOutCb2Trip = (s.m_nWarningStatus & 0x2000) != 0 ? true : false;
            s.m_flagWarningStatus.bit14_Reserved = (s.m_nWarningStatus & 0x4000) != 0 ? true : false;
            s.m_flagWarningStatus.bit15_EstopPushedOrDoorOpen = (s.m_nWarningStatus & 0x8000) != 0 ? true : false;

            //  016: Fault Status
            s.m_nFaultStatus = getUInt16Value(s.bTrendData, nIndex);
            nIndex += 2;

            s.m_flagWarningStatus.bit00_InverterFault = (s.m_nWarningStatus & 0x0001) != 0 ? true : false;
            s.m_flagWarningStatus.bit01_GridOverVoltage = (s.m_nWarningStatus & 0x0002) != 0 ? true : false;
            s.m_flagWarningStatus.bit02_GridOverFrequency = (s.m_nWarningStatus & 0x0004) != 0 ? true : false;
            s.m_flagWarningStatus.bit03_Reserved = (s.m_nWarningStatus & 0x0008) != 0 ? true : false;
            s.m_flagWarningStatus.bit04_TransformerOverHeat = (s.m_nWarningStatus & 0x0010) != 0 ? true : false;
            s.m_flagWarningStatus.bit05_FilterInductorOverHeat = (s.m_nWarningStatus & 0x0020) != 0 ? true : false;
            s.m_flagWarningStatus.bit06_FilterCapacitorOverHeat = (s.m_nWarningStatus & 0x0040) != 0 ? true : false;
            s.m_flagWarningStatus.bit07_EnclosureInternalOverHeat = (s.m_nWarningStatus & 0x0080) != 0 ? true : false;
            s.m_flagWarningStatus.bit08_FaultMainLoss = (s.m_nWarningStatus & 0x0100) != 0 ? true : false;
            s.m_flagWarningStatus.bit09_DcImd1 = (s.m_nWarningStatus & 0x0200) != 0 ? true : false;
            s.m_flagWarningStatus.bit10_ControlSpd1Open = (s.m_nWarningStatus & 0x0400) != 0 ? true : false;
            s.m_flagWarningStatus.bit11_BatteryOverHeat = (s.m_nWarningStatus & 0x0800) != 0 ? true : false;
            s.m_flagWarningStatus.bit12_DcCb1Trip = (s.m_nWarningStatus & 0x1000) != 0 ? true : false;
            s.m_flagWarningStatus.bit13_AcOutCb2Trip = (s.m_nWarningStatus & 0x2000) != 0 ? true : false;
            s.m_flagWarningStatus.bit14_Reserved = (s.m_nWarningStatus & 0x4000) != 0 ? true : false;
            s.m_flagWarningStatus.bit15_EstopPushedOrDoorOpen = (s.m_nWarningStatus & 0x8000) != 0 ? true : false;

            //  017: 누적 충전량
            s.fAccCharge = getUInt32Value(s.bTrendData, nIndex) * 0.000001f;
            nIndex += 4;

            //  018: 누적 방전량
            s.fAccDischarge = getUInt32Value(s.bTrendData, nIndex) * 0.000001f;
            nIndex += 4;

            nIndex = 256; 


            //  Function Code 3

            //  000: Reference Active power
            s.fReferenceP = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  001: Reference Reactive power
            s.fReferenceQ = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  002: Fixed PF control reference
            s.fFixedPF_ref = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  003: Volt-Var control reference
            s.nVolt_Var_ref = getUInt16Value(s.bTrendData, nIndex);
            nIndex += 2;

            //  004: pu
            s.fVolt_Var_V1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  005: pu
            s.fVolt_Var_V2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  006: pu
            s.fVolt_Var_V3 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  007: pu
            s.fVolt_Var_V4 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  008: pu
            s.fVolt_Var_Q1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  009: pu
            s.fVolt_Var_Q2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  010: pu
            s.fVolt_Var_Q3 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  011: pu
            s.fVolt_Var_Q4 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  012: Watt-Var control reference
            s.nWatt_Var_ref = getUInt16Value(s.bTrendData, nIndex);
            nIndex += 2;

            //  013: pu
            s.fWatt_Var_P1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  014: pu
            s.fWatt_Var_P2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  015: pu
            s.fWatt_Var_P3 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  016: pu
            s.fWatt_Var_P4 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  017: pu
            s.fWatt_Var_Q1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  018: pu
            s.fWatt_Var_Q2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  019: pu
            s.fWatt_Var_Q3 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  020: pu
            s.fWatt_Var_Q4 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  021: Q set control reference
            s.nQ_set_ref = getUInt16Value(s.bTrendData, nIndex);
            nIndex += 2;

            //  022: Volt-Watt control reference
            s.nVolt_Watt_ref = getUInt16Value(s.bTrendData, nIndex);
            nIndex += 2;

            //  023: pu
            s.fVolt_Watt_P1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  024: pu
            s.fVolt_Watt_P2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  025: pu
            s.fVolt_Watt_V1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  026: pu
            s.fVolt_Watt_V2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  027: LVRT  control reference
            s.nLVRT_ref = getUInt16Value(s.bTrendData, nIndex);
            nIndex += 2;

            //  028: pu
            s.fLVRT_UV1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  029: pu
            s.fLVRT_UV2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  030: s
            s.fLVRT_UT1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  031: s
            s.fLVRT_UT2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  032: HVRT control reference
            s.nHVRT_ref = getUInt16Value(s.bTrendData, nIndex);
            nIndex += 2;

            //  033: pu
            s.fHVRT_HV1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  034: pu
            s.fHVRT_HV2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  035: s
            s.fHVRT_HT1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  036: s
            s.fHVRT_HT2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  037: 
            s.nLFRT_ref = getUInt16Value(s.bTrendData, nIndex);
            nIndex += 2;

            //  038: pu
            s.fLFRT_UF1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  039: pu
            s.fLFRT_UF2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  040: s
            s.fLFRT_UFT1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  041: s
            s.fLFRT_UFT2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  042: 
            s.nHFRT_ref = getUInt16Value(s.bTrendData, nIndex);
            nIndex += 2;

            //  043: pu
            s.fHFRT_HF1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  044: pu
            s.fHFRT_HF2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  045: s
            s.fHFRT_HFT1 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  046: s
            s.fHFRT_HFT2 = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  047: Frq-Watt control reference
            s.nFrq_Watt_ref = getUInt16Value(s.bTrendData, nIndex);
            nIndex += 2;

            //  048: 
            s.fFrq_Watt_dbuf = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  049: 
            s.fFrq_Watt_dbof = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  050: 
            s.fFrq_Watt_Kuf = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  051: 
            s.fFrq_Watt_Kof = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  052: P RAMP control reference
            s.fP_RAMP_ref = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  053: P Limit control reference
            s.fP_Limit_ref = getInt16Value(s.bTrendData, nIndex) * 0.01f;
            nIndex += 2;

            //  054: Set under voltage value(fault)
            s.fUV_set = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  055: Set over voltage value(fault)
            s.fOV_set = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  056: Set low voltage value(warning)
            s.fLV_set = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  057: Set high voltage value(warning)
            s.fHV_set = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  058: Set over current value(fault)  
            s.fOC_set = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  059: Set high current value(warning)
            s.fHC_set = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  060: Set under frequency value(warning)
            s.fUF_set = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  061: Set over frequency value(fault)
            s.fOF_set = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  062: Set low frequency value(warning)
            s.fLF_set = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  063: Set high frequency value(fault)
            s.fHF_set = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;

            //  064: Set over temperature value(fault)
            s.fOT_set = getInt16Value(s.bTrendData, nIndex) * 0.1f;
            nIndex += 2;
        }
    }
}
