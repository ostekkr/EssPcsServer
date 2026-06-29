using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using C1.Win.FlexGrid;

namespace EssPcsServer
{
    public partial class clsSqlServer
    {
        private SqlDataAdapter m_adtWaveInfo;
        private DataSet m_datasetWaveInfo;
        public String g_strWaveInfoLogError = "";

        public void WaveInfo_Make()
        {
#if USE_EXCEPTION
            try
            {
#endif
                m_adtWaveInfo = new SqlDataAdapter();

                SqlCommand command = new SqlCommand("SELECT * FROM tblWaveInfo", m_conSql);
                command.CommandType = CommandType.Text;

                command = new SqlCommand("INSERT INTO tblWaveInfo (ID,METER,LOC,FILEPOS,FILETYPE,FILETYPESTR,RAWDATA,DT,MS,EVENT,TRIG,CH,STARTREC,ENDREC,MSG,ACK) VALUES (@ID,@METER,@LOC,@FILEPOS,@FILETYPE,@FILETYPESTR,@RAWDATA,@DT,@MS,@EVENT,@TRIG,@CH,@STARTREC,@ENDREC,@MSG,@ACK)", m_conSql);
                command.CommandType = CommandType.Text;
                command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");
                command.Parameters.Add("@METER", SqlDbType.NVarChar, 255, "METER");
                command.Parameters.Add("@LOC", SqlDbType.NVarChar, 255, "LOC");
                command.Parameters.Add("@FILEPOS", SqlDbType.Int, 0, "FILEPOS");
                command.Parameters.Add("@FILETYPE", SqlDbType.Int, 0, "FILETYPE");
                command.Parameters.Add("@FILETYPESTR", SqlDbType.NVarChar, 50, "FILETYPESTR");
                command.Parameters.Add("@RAWDATA", SqlDbType.Binary, 16, "RAWDATA");
                command.Parameters.Add("@DT", SqlDbType.DateTime, 0, "DT");
                command.Parameters.Add("@MS", SqlDbType.Int, 0, "MS");
                command.Parameters.Add("@EVENT", SqlDbType.Int, 0, "EVENT");
                command.Parameters.Add("@TRIG", SqlDbType.Int, 0, "TRIG");
                command.Parameters.Add("@CH", SqlDbType.Int, 0, "CH");
                command.Parameters.Add("@STARTREC", SqlDbType.BigInt, 0, "STARTREC");
                command.Parameters.Add("@ENDREC", SqlDbType.BigInt, 0, "ENDREC");
                command.Parameters.Add("@MSG", SqlDbType.NVarChar, 255, "MSG");
                command.Parameters.Add("@ACK", SqlDbType.Bit, 0, "ACK");

                m_adtWaveInfo.InsertCommand = command;

                m_datasetWaveInfo = new DataSet();

#if USE_EXCEPTION
            }
            catch (Exception e)
            {
                g_strWaveInfoLogError = "WaveInfo_Make : " + e.Message;
            }
#endif
        }

        public bool WaveInfo_Add(clsWaveInfo waveInfo)
        {
            bool bResult = true;
            /*
#if USE_EXCEPTION
            try
            {
#endif
                m_adtWaveInfo.InsertCommand.Parameters["@ID"].Value = waveInfo.m_nDeviceNo;
                m_adtWaveInfo.InsertCommand.Parameters["@METER"].Value = waveInfo.m_strDeviceName;
                m_adtWaveInfo.InsertCommand.Parameters["@LOC"].Value = waveInfo.m_strLoc;
                m_adtWaveInfo.InsertCommand.Parameters["@FILEPOS"].Value = waveInfo.m_nFilePos;
                m_adtWaveInfo.InsertCommand.Parameters["@FILETYPE"].Value = waveInfo.m_nFileType;
                m_adtWaveInfo.InsertCommand.Parameters["@FILETYPESTR"].Value = waveInfo.m_strFileType;
                m_adtWaveInfo.InsertCommand.Parameters["@RAWDATA"].Value = waveInfo.m_bData;
                m_adtWaveInfo.InsertCommand.Parameters["@DT"].Value = waveInfo.m_dtEvent;
                m_adtWaveInfo.InsertCommand.Parameters["@MS"].Value = waveInfo.m_nMs;
                m_adtWaveInfo.InsertCommand.Parameters["@EVENT"].Value = waveInfo.m_nEvent;
                m_adtWaveInfo.InsertCommand.Parameters["@TRIG"].Value = waveInfo.m_nTrigger;
                m_adtWaveInfo.InsertCommand.Parameters["@CH"].Value = waveInfo.m_nEventCh;
                m_adtWaveInfo.InsertCommand.Parameters["@STARTREC"].Value = waveInfo.m_nStartRec;
                m_adtWaveInfo.InsertCommand.Parameters["@ENDREC"].Value = waveInfo.m_nEndRec;
                m_adtWaveInfo.InsertCommand.Parameters["@MSG"].Value = waveInfo.m_strMsg;
                m_adtWaveInfo.InsertCommand.Parameters["@ACK"].Value = false;

                m_adtWaveInfo.InsertCommand.ExecuteNonQuery();

#if USE_EXCEPTION
            }
            catch (Exception e)
            {
                g_strWaveInfoLogError = "WaveInfo_Add : " + e.Message;

                bResult = false;
            }
#endif
            */

            return bResult;
        }

        public bool WaveInfo_CheckDuplicate(clsWaveInfo sysEvent)
        {
            bool bResult = false;

            g_strWaveInfoLogError = "";

#if USE_EXCEPTION
            try
            {
#endif
                SqlCommand command = new SqlCommand("SELECT * FROM tblWaveInfo WHERE (METER=@METER) AND (DT=@DT) AND (MS=@MS) AND (FILETYPE=@FILETYPE)", m_conSql);

                command.CommandType = CommandType.Text;

                command.Parameters.Add("@METER", SqlDbType.NVarChar, 255, "METER");
                command.Parameters.Add("@DT", SqlDbType.DateTime, 0, "DT");
                command.Parameters.Add("@MS", SqlDbType.Int, 0, "MS");
                command.Parameters.Add("@FILETYPE", SqlDbType.Int, 0, "FILETYPE");
                command.Parameters["@METER"].Value = sysEvent.m_strDeviceName;
                command.Parameters["@DT"].Value = sysEvent.m_dtEvent;
                command.Parameters["@MS"].Value = sysEvent.m_nMs;
                command.Parameters["@FILETYPE"].Value = sysEvent.m_nFileType;

                m_adtWaveInfo.SelectCommand = command;

                m_adtWaveInfo.Fill(m_datasetWaveInfo);

                if (m_datasetWaveInfo.Tables[0].Rows.Count > 0)
                {
                    m_datasetWaveInfo.Clear();
                    bResult = true;
                }
#if USE_EXCEPTION
            }
            catch (Exception e)
            {
                g_strWaveInfoLogError = "WaveInfo_CheckDuplicate : " + e.Message;
                clsLog.addSqlLog(g_strWaveFormLogError);
            }
#endif
            return bResult;
        }
    }
}
