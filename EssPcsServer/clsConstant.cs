using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace EssPcsServer
{
    class clsConstant
    {
        public const int CDFR_ONE_REC_SIZE = 512;

#if LONG_PACKET

        public const int CDFR_TBLOCK_SIZE = 4096;
        public const int CDFR_ONE_TBLOCK_RECS = 8;
        public const int CDFR_EXPECTED_WAVE_LENGTH = 4100;

#else

        public const int CDFR_TBLOCK_SIZE = 1024;
        public const int CDFR_ONE_TBLOCK_RECS = 2;
        public const int CDFR_EXPECTED_WAVE_LENGTH = 1028;

#endif
        public static readonly String APP_PATH    = ConfigurationManager.AppSettings["AppPath"]   ?? @"c:\pms\keri_ess_pcs\";
        public static readonly String APP_DB_NAME = ConfigurationManager.AppSettings["AppDbName"] ?? @"c:\pms\keri_ess_pcs\db\ess_pcs.db";
        public static readonly String APP_DB_ORIG = ConfigurationManager.AppSettings["AppDbOrig"] ?? @"c:\pms\keri_ess_pcs\db\ess_pcs.orig";

        public static readonly int CDFR_ETHERNET_DATA_LENGTH = 8192;

        public const int ESSPCS_MAX_NUM = 1;
        public const int ESSPCS_CMD_INFO_GET = 0;
        public const int ESSPCS_CMD_INV_STOP = 1;
        public const int ESSPCS_CMD_INV_RUN = 2;
        public const int ESSPCS_CMD_INV_PARAMETER = 3;
        public const int ESSPCS_CMD_OUTPUT_SET = 4;
        public const int ESSPCS_CMD_TIME_GET = 5;
        public const int ESSPCS_CMD_INFO_UPDATE = 6;
        public const int ESSPCS_CMD_MEMORY_INIT = 7;
        public const int ESSPCS_CMD_REBOOT = 8;
        public const int ESSPCS_CMD_PREV_TRIGGER = 9;
        public const int ESSPCS_CMD_MANUAL_TRIGGER = 10;
        public const int ESSPCS_CMD_CAL_START = 11;
        public const int ESSPCS_CMD_CAL_STOP = 12;
        public const int ESSPCS_CMD_RELAY_CONTROL_ON = 13;
        public const int ESSPCS_CMD_RELAY_CONTROL_OFF = 14;
        public const int ESSPCS_CMD_FAULT_CLEAR = 15;
        public const int ESSPCS_CMD_WARN_CLEAR = 16;
        public const int ESSPCS_CMD_INS_WAVE = 17;
        public const int ESSPCS_CMD_PHA_WAVE = 18;
        public const int ESSPCS_CMD_TRIGGER_LOCK_ON = 19;
        public const int ESSPCS_CMD_TRIGGER_LOCK_OFF = 20;
        public const int ESSPCS_CMD_RELAY = 21;
        public const int ESSPCS_CMD_FAULT_LOC = 22;

        public const int ESSBMS_CMD_RESET = 3;
        public const int ESSBMS_CMD_DCLINK_ON = 4;
        public const int ESSBMS_CMD_DCLINK_OFF = 5;
        public const int ESSBMS_CMD_WATCHDOG = 12;

        public static readonly int MODBUS_MODE_NONE = -1;
        public static readonly int MODBUS_MODE_NORMAL = 0;
        public static readonly int MODBUS_MODE_SBO = 1;
        public static readonly int MODBUS_MODE_SBO_RELAY = 2;
        public static readonly int MODBUS_MODE_RUN_TREND = 3;

        public static readonly int PQMS_SYSTEM_EVENT = 0;
        public static readonly int PQMS_PQ_EVENT = 1;
        public static readonly int PQMS_LOAD_PROFILE = 2;
        public static readonly int PQMS_WAVE_DATA = 3;
        public static readonly int PCS_TREND_DATA = 4;
        public static readonly int CDFR_TREND_REQ = 5;
        public static readonly int PQMS_GAIN_DATA = 6;

        public const int PCS_RTU_CONNECTED = -1;
        public const int PCS_RTU_DISCONNECTED = -2;
        public const int PCS_RTU_CONNECTION_FAILED = -3;
        public const int PCS_RTU_CONNECTING = -4;
        public const int PCS_RTU_COM_ERROR = -5;
        public const int PCS_RTU_SHUTDOWN = -6;

        public static readonly int PQMS_REMOVE_EVENT = -6;

        public static readonly int PQMS_MAX_EVENT = 128;

        public static readonly int PQMS_COMMAND_CONNECTION_CHECK = 0;
        public static readonly int PQMS_COMMAND_SBO = 1;

        public const int CDFR_DEVICE_MEMORY_INIT = 2;
        public const int CDFR_DEVICE_REBOOT = 3;
        public const int CDFR_DEVICE_PREV_TRIGGER = 4;
        public const int CDFR_DEVICE_MANUAL_TRIGGER = 5;
        public const int CDFR_DEVICE_CAL_START = 6;
        public const int CDFR_DEVICE_CAL_STOP = 7;
        public const int CDFR_DEVICE_RELAY_CONTROL_ON = 8;
        public const int CDFR_DEVICE_RELAY_CONTROL_OFF = 9;
        public const int CDFR_DEVICE_TRIGGER_LOCK_ON = 10;
        public const int CDFR_DEVICE_TRIGGER_LOCK_OFF = 11;
        public const int CDFR_DEVICE_RELAY = 12;
        public const int ESSPCS_DEVICE_INV_STOP = 13;
        public const int ESSPCS_DEVICE_INV_RUN = 14;
        public const int ESSPCS_DEVICE_INV_PARAMETER = 15;
        public const int CDFR_DEVICE_RECEIVE_SETUP_DATA = 16;
        public const int ESSPCS_DEVICE_FAULT_CLEAR = 17;
        public const int ESSPCS_DEVICE_WARN_CLEAR = 18;
        public const int CDFR_DEVICE_INS_WAVE = 19;
        public const int CDFR_DEVICE_PHA_WAVE = 20;
        public const int CDFR_DEVICE_FAULT_LOC = 21;
        public const int PCS_DEVICE_TREND_DATA_RECEIVE = 22;
        public const int ESSPCS_DEVICE_OUTPUT_SET = 23;

        public static readonly int MODBUS_COMMAND_READ_HOLD_REGISTER = 0x03;
        public const int MODBUS_FUNC_READ_INPUT = 0x04;
        public const int ESSPCS_COMMAND_SINGLE_COIL = 0x05;
        public const int ESSPCS_COMMAND_SINGLE_WRITE = 0x06;
        public const int ESSPCS_COMMAND_MULTIPLE_WRITE = 0x10;
        public const int CDFR_COMMAND_WRITE_AFE = 0x10;
        public static readonly int MODBUS_UNIT_SIZE = 2;

        public static readonly int CDFR_WAVE_SIZE = 1024 * 1024 * 5;

        public static readonly int PCS_TREND_DATA_START = 0;
        public static readonly int PCS_TREND_DATA_END = 127;
        public static readonly int PCS_TREND_DATA_NUM = 128;

        public static readonly int PCS_SETUP_DATA_START = 1000;
        public static readonly int PCS_SETUP_DATA_END = 1603;
        public static readonly int PCS_SETUP_DATA_NUM = 844;

        public static readonly int CDFR_FAULT_LOC_START = 1200;
        public static readonly int CDFR_FAULT_LOC_END = 1279;
        public static readonly int CDFR_FAULT_LOC_NUM = 80;

        public const int CDFR_INS_REC_START = 2000;
        public const int CDFR_INS_REC_NUM = 128;
        public const int CDFR_INS_REC_SIZE = 16;
        public const int CDFR_PHA_REC_START = 4200;
        public const int CDFR_PHA_REC_NUM = 128;
        public const int CDFR_PHA_REC_SIZE = 16;

        public static readonly int ETHERNET_PORT = 502;

        public static readonly float[] PQMS_PT_RATING = { 63.5f, 110.0f, 115.0f, 220.0f };
        public static readonly float[] PQMS_CT_RATING = { 5.0f, 1.0f, 120.0f, 40.0f };
        public static readonly float[] PQMS_FREQUENCY = { 60.0f, 50.0f };
        public static readonly int[] PQMS_SAMPLING_RATE = { 32, 64, 128, 256 };
        public static readonly int[] PQMS_SAMPLES = { 4096, 8192, 12288, 16384 };
        public static readonly float PQMS_PT_GAIN = 0.333f;
        public static readonly float PQMS_CT_GAIN = 779.357f;

        public static readonly float GAIN_ADC = (float)(131072.0f / 1200.0f);
        //  public static readonly float ADIN_mV_at_220V = 730.9f;
        public static readonly float ADIN_mV_at_220V = 495.0f;
        public static readonly float ADC_RMS_at_220V = (float)(ADIN_mV_at_220V * GAIN_ADC);
        public static readonly float gfVrmsGain = (float)(220.0f / ADC_RMS_at_220V);

        public static readonly float ADIN_mV_at_5A = 66.67f;
        public static readonly float ADC_RMS_at_5A = (float)(ADIN_mV_at_5A * GAIN_ADC);
        public static readonly float gfIrmsGain = (float)(5.0f / ADC_RMS_at_5A);

        public static readonly DateTime REF_DATETIME = new DateTime(2000, 1, 1, 0, 0, 0);

        public const int PQMS_TAB_REAL_TIME = 0;
        public const int PQMS_TAB_SYSTEM_EVENT = 1;
        public const int PQMS_TAB_PQ_EVENT = 2;
        public const int PQMS_TAB_WAVEFORM = 3;
        public const int PQMS_TAB_LOAD_PROFILE = 4;
        public const int PQMS_TAB_SYSTEM_LOG = 5;
        public const int PQMS_TAB_DEVICE_SETTING = 6;

        //  EVENT 발생 상황 : Sys Event
        public static readonly byte PQMS_STATUS_SYSEVENT = 0x01;
        //  EVENT 발생 상황 : PQ Event
        public static readonly byte PQMS_STATUS_PQEVENT = 0x02;
        //  EVENT 발생 상황 : LP exist
        public static readonly byte PQMS_STATUS_LPEXIST = 0x04;
        //  EVENT 발생 상황 : On PQ Event
        public static readonly byte PQMS_STATUS_ONPQEVENT = 0x08;
        //  DI 상태 : DI1
        public static readonly byte PQMS_STATUS_DI1 = 0x10;
        //  DI 상태 : DI2
        public static readonly byte PQMS_STATUS_DI2 = 0x20;
        //  DI 상태 : DI3
        public static readonly byte PQMS_STATUS_DI3 = 0x40;
        //  DI 상태 : DI4
        public static readonly byte PQMS_STATUS_DI4 = 0x80;

        //  Relay 상태 : Relay1
        public static readonly byte PQMS_STATUS_RELAY1 = 0x01;
        //  Relay 상태 : Relay2
        public static readonly byte PQMS_STATUS_RELAY2 = 0x02;
        //  Relay 상태 : BAT 상태
        public static readonly byte PQMS_STATUS_BAT = 0x04;
        //  Relay 상태 : PQ Enable
        public static readonly byte PQMS_STATUS_PQENABLE = 0x08;
        //  무선통신 : no module
        public static readonly byte PQMS_STATUS_WIRELESS_NONE = 0x00;
        //  무선통신 : Bluetooth
        public static readonly byte PQMS_STATUS_WIRELESS_BT = 0x10;
        //  무선 통신 : Wifi
        public static readonly byte PQMS_STATUS_WIRELESS_WIFI = 0x20;
        //  485모드 (0 : 서버, 1 : Host)
        public static readonly byte PQMS_STATUS_RS485 = 0x40;
        //  LAN (1 : Link, 0 : Not)
        public static readonly byte PQMS_STATUS_LAN = 0x80;

        //  PCS 상태 : 발전 정지
        public static readonly byte PQMS_STATUS_PCS_STOP = 0x01;
        //  PCS 상태 : MPP
        public static readonly byte PQMS_STATUS_PCS_MPP = 0x02;
        //  PCS 상태 : 주파수 OK
        public static readonly byte PQMS_STATUS_PCS_FREQOK = 0x04;
        //  PCS 상태 : VPV OK
        public static readonly byte PQMS_STATUS_PCS_VPVOK = 0x08;
        //  PCS 상태 : Vdclink OK
        public static readonly byte PQMS_STATUS_PCS_VDCLINKOK = 0x10;
        //  PCS 상태 : Vac OK
        public static readonly byte PQMS_STATUS_PCS_VACOK = 0x20;
        //  PCS 상태 : MC 동작
        public static readonly byte PQMS_STATUS_PCS_MCOK = 0x40;
        //  PCS 상태 : PV셀 과전류
        public static readonly byte PQMS_STATUS_PCS_PVCELLOC = 0x80;

        //  PCS 상태 : 계통 과전류
        public static readonly byte PQMS_STATUS_PCS_PQOC = 0x01;
        //  PCS 상태 : 계통 주파수
        public static readonly byte PQMS_STATUS_PCS_PQFREQ = 0x02;
        //  PCS 상태 : PCS 제어오류
        public static readonly byte PQMS_STATUS_PCS_ERROR = 0x04;
        //  PCS 상태 : 과열 방지
        public static readonly byte PQMS_STATUS_PCS_PROTOH = 0x08;
        //  PCS 상태 : 단독 운전
        public static readonly byte PQMS_STATUS_PCS_STRUN = 0x10;
        //  무선통신 상태 : Blue Pairing - Bluetooth의 Pairing 상태
        public static readonly byte PQMS_STATUS_PCS_BTPAIRING = 0x20;
        //  무선통신 상태 : Wifi Association - Wifi가 AP 모드로 또는 AP에 Association 상태 표시
        public static readonly byte PQMS_STATUS_PCS_WIFIASSOCI = 0x40;
        //  Wifi Env - Wifi 설정에 필요한 Setup정보 설정 유무. (프로토콜 F120으로 설정이 되었으면 1)
        public static readonly byte PQMS_STATUS_PCS_WIFIENV = 0x80;

        public static readonly int PQMS_WIRELESS_NONE = 0;
        public static readonly int PQMS_WIRELESS_BLUETOOTH = 1;
        public static readonly int PQMS_WIRELESS_WIFI = 2;
        public static readonly int PQMS_RS485_SERVER = 0;
        public static readonly int PQMS_RS485_HOST = 1;

        public const float IGNORE_VALUE_PERCENT = 0.005f;
    }
}
