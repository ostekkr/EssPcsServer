using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using EssPcsService;

namespace EssPcsServer
{
    public partial class clsSqlServer
    {
        private SqlDataAdapter m_adtTrendInfo;
        public String g_strTrendLogError = "";
        string m_strSQL;
        
        public bool Trend_Add(clsEssPcsTrendInfo trendInfo)
        {
            bool bResult = true;

#if USE_EXCEPTION
            try
            {
#endif
                /*
                m_adtTrendInfo.InsertCommand.Parameters["@DEVICE_TIME"].Value = trendInfo.m_dtTime;
                m_adtTrendInfo.InsertCommand.Parameters["@DEVICE_ID"].Value = trendInfo.m_nDevice;
                m_adtTrendInfo.InsertCommand.Parameters["@DEVICE_NAME"].Value = trendInfo.m_strDevice;

                //  주파수
                m_adtTrendInfo.InsertCommand.Parameters["@FREQ"].Value = trendInfo.m_fFreq;
                //  유효전력
                m_adtTrendInfo.InsertCommand.Parameters["@PT"].Value = trendInfo.m_fPt;
                //  무효전력
                m_adtTrendInfo.InsertCommand.Parameters["@QT"].Value = trendInfo.m_fQt;

                //  A상 전압 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@VAMAG"].Value = float.IsNaN(trendInfo.m_fMag[0]) ? 0 : trendInfo.m_fMag[0];
                //  B상 전압 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@VBMAG"].Value = float.IsNaN(trendInfo.m_fMag[1]) ? 0 : trendInfo.m_fMag[1];
                //  C상 전압 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@VCMAG"].Value = float.IsNaN(trendInfo.m_fMag[2]) ? 0 : trendInfo.m_fMag[2];
                //  N상 전압 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@VNMAG"].Value = float.IsNaN(trendInfo.m_fMag[3]) ? 0 : trendInfo.m_fMag[3];
                //  A상 전류 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@IAMAG"].Value = float.IsNaN(trendInfo.m_fMag[4]) ? 0 : trendInfo.m_fMag[4];
                //  B상 전류 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@IBMAG"].Value = float.IsNaN(trendInfo.m_fMag[5]) ? 0 : trendInfo.m_fMag[5];
                //  C상 전류 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@ICMAG"].Value = float.IsNaN(trendInfo.m_fMag[6]) ? 0 : trendInfo.m_fMag[6];
                //  N상 전류 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@INMAG"].Value = float.IsNaN(trendInfo.m_fMag[7]) ? 0 : trendInfo.m_fMag[7];
                //  전류 채널 1 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@I1MAG"].Value = float.IsNaN(trendInfo.m_fMag[8]) ? 0 : trendInfo.m_fMag[8];
                //  전류 채널 2 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@I2MAG"].Value = float.IsNaN(trendInfo.m_fMag[9]) ? 0 : trendInfo.m_fMag[9];
                //  전류 채널 3 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@I3MAG"].Value = float.IsNaN(trendInfo.m_fMag[10]) ? 0 : trendInfo.m_fMag[10];
                //  전류 채널 4 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@I4MAG"].Value = float.IsNaN(trendInfo.m_fMag[11]) ? 0 : trendInfo.m_fMag[11];
                //  전류 채널 5 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@I5MAG"].Value = float.IsNaN(trendInfo.m_fMag[12]) ? 0 : trendInfo.m_fMag[12];
                //  전류 채널 6 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@I6MAG"].Value = float.IsNaN(trendInfo.m_fMag[13]) ? 0 : trendInfo.m_fMag[13];
                //  전류 채널 7 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@I7MAG"].Value = float.IsNaN(trendInfo.m_fMag[14]) ? 0 : trendInfo.m_fMag[14];
                //  전류 채널 8 RMS
                m_adtTrendInfo.InsertCommand.Parameters["@I8MAG"].Value = float.IsNaN(trendInfo.m_fMag[15]) ? 0 : trendInfo.m_fMag[15];

                //  A상 전압 위상
                m_adtTrendInfo.InsertCommand.Parameters["@VAPHA"].Value = float.IsNaN(trendInfo.m_fPhasor[0]) ? 0 : trendInfo.m_fPhasor[0];
                //  B상 전압 위상
                m_adtTrendInfo.InsertCommand.Parameters["@VBPHA"].Value = float.IsNaN(trendInfo.m_fPhasor[1]) ? 0 : trendInfo.m_fPhasor[1];
                //  C상 전압 위상
                m_adtTrendInfo.InsertCommand.Parameters["@VCPHA"].Value = float.IsNaN(trendInfo.m_fPhasor[2]) ? 0 : trendInfo.m_fPhasor[2];
                //  N상 전압 위상
                m_adtTrendInfo.InsertCommand.Parameters["@VNPHA"].Value = float.IsNaN(trendInfo.m_fPhasor[3]) ? 0 : trendInfo.m_fPhasor[3];
                //  A상 전류 위상
                m_adtTrendInfo.InsertCommand.Parameters["@IAPHA"].Value = float.IsNaN(trendInfo.m_fPhasor[4]) ? 0 : trendInfo.m_fPhasor[4];
                //  B상 전류 위상
                m_adtTrendInfo.InsertCommand.Parameters["@IBPHA"].Value = float.IsNaN(trendInfo.m_fPhasor[5]) ? 0 : trendInfo.m_fPhasor[5];
                //  C상 전류 위상
                m_adtTrendInfo.InsertCommand.Parameters["@ICPHA"].Value = float.IsNaN(trendInfo.m_fPhasor[6]) ? 0 : trendInfo.m_fPhasor[6];
                //  N상 전류 위상
                m_adtTrendInfo.InsertCommand.Parameters["@INPHA"].Value = float.IsNaN(trendInfo.m_fPhasor[7]) ? 0 : trendInfo.m_fPhasor[7];
                //  전류 채널 1 위상
                m_adtTrendInfo.InsertCommand.Parameters["@I1PHA"].Value = float.IsNaN(trendInfo.m_fPhasor[8]) ? 0 : trendInfo.m_fPhasor[8];
                //  전류 채널 2 위상
                m_adtTrendInfo.InsertCommand.Parameters["@I2PHA"].Value = float.IsNaN(trendInfo.m_fPhasor[9]) ? 0 : trendInfo.m_fPhasor[9];
                //  전류 채널 3 위상
                m_adtTrendInfo.InsertCommand.Parameters["@I3PHA"].Value = float.IsNaN(trendInfo.m_fPhasor[10]) ? 0 : trendInfo.m_fPhasor[10];
                //  전류 채널 4 위상
                m_adtTrendInfo.InsertCommand.Parameters["@I4PHA"].Value = float.IsNaN(trendInfo.m_fPhasor[11]) ? 0 : trendInfo.m_fPhasor[11];
                //  전류 채널 5 위상
                m_adtTrendInfo.InsertCommand.Parameters["@I5PHA"].Value = float.IsNaN(trendInfo.m_fPhasor[12]) ? 0 : trendInfo.m_fPhasor[12];
                //  전류 채널 6 위상
                m_adtTrendInfo.InsertCommand.Parameters["@I6PHA"].Value = float.IsNaN(trendInfo.m_fPhasor[13]) ? 0 : trendInfo.m_fPhasor[13];
                //  전류 채널 7 위상
                m_adtTrendInfo.InsertCommand.Parameters["@I7PHA"].Value = float.IsNaN(trendInfo.m_fPhasor[14]) ? 0 : trendInfo.m_fPhasor[14];
                //  전류 채널 8 위상
                m_adtTrendInfo.InsertCommand.Parameters["@I8PHA"].Value = float.IsNaN(trendInfo.m_fPhasor[15]) ? 0 : trendInfo.m_fPhasor[15];
                */
                m_adtTrendInfo.InsertCommand.ExecuteNonQuery();

#if USE_EXCEPTION
            }
            catch (Exception e)
            {
                g_strTrendLogError = "Trend_Add : " + e.Message;
                clsLog.addSqlLog(g_strTrendLogError);
                bResult = false;
            }
#endif

            return bResult;
        }

        public void Trend_Make()
        {
            m_adtTrendInfo = new SqlDataAdapter();

            m_strSQL = "INSERT INTO tblTrend (";
            m_strSQL += "DEVICE_TIME,DEVICE_ID,DEVICE_NAME,";
            m_strSQL += "FREQ,PT,QT,";
            m_strSQL += "VAMAG,VBMAG,VCMAG,VNMAG,IAMAG,IBMAG,ICMAG,INMAG,I1MAG,I2MAG,I3MAG,I4MAG,I5MAG,I6MAG,I7MAG,I8MAG,";
            m_strSQL += "VAPHA,VBPHA,VCPHA,VNPHA,IAPHA,IBPHA,ICPHA,INPHA,I1PHA,I2PHA,I3PHA,I4PHA,I5PHA,I6PHA,I7PHA,I8PHA";
            m_strSQL += ") VALUES (";
            m_strSQL += "@DEVICE_TIME,@DEVICE_ID,@DEVICE_NAME,";
            m_strSQL += "@FREQ,@PT,@QT,";
            m_strSQL += "@VAMAG,@VBMAG,@VCMAG,@VNMAG,@IAMAG,@IBMAG,@ICMAG,@INMAG,@I1MAG,@I2MAG,@I3MAG,@I4MAG,@I5MAG,@I6MAG,@I7MAG,@I8MAG,";
            m_strSQL += "@VAPHA,@VBPHA,@VCPHA,@VNPHA,@IAPHA,@IBPHA,@ICPHA,@INPHA,@I1PHA,@I2PHA,@I3PHA,@I4PHA,@I5PHA,@I6PHA,@I7PHA,@I8PHA";
            m_strSQL += ")";

            m_adtTrendInfo.InsertCommand = new SqlCommand(m_strSQL, m_conSql);

            m_adtTrendInfo.InsertCommand.Parameters.Add("@DEVICE_TIME", SqlDbType.DateTime, 0, "DEVICE_TIME");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@DEVICE_ID", SqlDbType.Int, 0, "DEVICE_ID");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@DEVICE_NAME", SqlDbType.NVarChar, 50, "DEVICE_NAME");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@FREQ", SqlDbType.Float, 0, "FREQ");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@PT", SqlDbType.Float, 0, "PT");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@QT", SqlDbType.Float, 0, "QT");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@VAMAG", SqlDbType.Float, 0, "VAMAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@VBMAG", SqlDbType.Float, 0, "VBMAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@VCMAG", SqlDbType.Float, 0, "VCMAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@VNMAG", SqlDbType.Float, 0, "VNMAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@IAMAG", SqlDbType.Float, 0, "IAMAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@IBMAG", SqlDbType.Float, 0, "IBMAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@ICMAG", SqlDbType.Float, 0, "ICMAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@INMAG", SqlDbType.Float, 0, "INMAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I1MAG", SqlDbType.Float, 0, "I1MAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I2MAG", SqlDbType.Float, 0, "I2MAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I3MAG", SqlDbType.Float, 0, "I3MAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I4MAG", SqlDbType.Float, 0, "I4MAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I5MAG", SqlDbType.Float, 0, "I5MAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I6MAG", SqlDbType.Float, 0, "I6MAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I7MAG", SqlDbType.Float, 0, "I7MAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I8MAG", SqlDbType.Float, 0, "I8MAG");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@VAPHA", SqlDbType.Float, 0, "VAPHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@VBPHA", SqlDbType.Float, 0, "VBPHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@VCPHA", SqlDbType.Float, 0, "VCPHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@VNPHA", SqlDbType.Float, 0, "VNPHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@IAPHA", SqlDbType.Float, 0, "IAPHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@IBPHA", SqlDbType.Float, 0, "IBPHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@ICPHA", SqlDbType.Float, 0, "ICPHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@INPHA", SqlDbType.Float, 0, "INPHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I1PHA", SqlDbType.Float, 0, "I1PHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I2PHA", SqlDbType.Float, 0, "I2PHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I3PHA", SqlDbType.Float, 0, "I3PHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I4PHA", SqlDbType.Float, 0, "I4PHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I5PHA", SqlDbType.Float, 0, "I5PHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I6PHA", SqlDbType.Float, 0, "I6PHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I7PHA", SqlDbType.Float, 0, "I7PHA");
            m_adtTrendInfo.InsertCommand.Parameters.Add("@I8PHA", SqlDbType.Float, 0, "I8PHA");
        }
    }
}
