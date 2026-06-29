using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using System.Windows.Forms;
using C1.Win.FlexGrid;
using System.IO;

namespace EssPcsServer
{
    public partial class clsSqlServer
    {
        private SqlDataAdapter m_adtWaveForm;
        private DataSet m_datasetWaveForm;
        public string g_strWaveFormLogError = "";

        public void WaveForm_Make()
        {
#if USE_EXCEPTION
            try
            {
#endif
                m_adtWaveForm = new SqlDataAdapter();

                SqlCommand command = new SqlCommand("SELECT * FROM tblWaveForm", m_conSql);
                command.CommandType = CommandType.Text;

                command = new SqlCommand("INSERT INTO tblWaveForm (ID,METER,DT,MS,EVENT,TRIG,CH,FILEPOS,FILETYPE,FILETYPESTR,MSG,SETUPSIZE,WAVESIZE,SETUP,WAVE,ACK) VALUES (@ID,@METER,@DT,@MS,@EVENT,@TRIG,@CH,@FILEPOS,@FILETYPE,@FILETYPESTR,@MSG,@SETUPSIZE,@WAVESIZE,@SETUP,@WAVE,@ACK)", m_conSql);
                command.Parameters.Add("@ID", SqlDbType.Int, 0, "ID");
                command.Parameters.Add("@METER", SqlDbType.NVarChar, 255, "METER");
                command.Parameters.Add("@DT", SqlDbType.DateTime, 0, "DT");
                command.Parameters.Add("@MS", SqlDbType.Int, 0, "MS");
                command.Parameters.Add("@EVENT", SqlDbType.Int, 0, "EVENT");
                command.Parameters.Add("@TRIG", SqlDbType.Int, 0, "TRIG");
                command.Parameters.Add("@CH", SqlDbType.Int, 0, "CH");
                command.Parameters.Add("@FILEPOS", SqlDbType.Int, 0, "FILEPOS");
                command.Parameters.Add("@FILETYPE", SqlDbType.Int, 0, "FILETYPE");
                command.Parameters.Add("@FILETYPESTR", SqlDbType.NVarChar, 50, "FILETYPESTR");
                command.Parameters.Add("@MSG", SqlDbType.NVarChar, 255, "LOG");
                command.Parameters.Add("@SETUPSIZE", SqlDbType.Int, 0, "SETUPSIZE");
                command.Parameters.Add("@WAVESIZE", SqlDbType.Int, 0, "WAVESIZE");
                command.Parameters.Add("@SETUP", SqlDbType.VarBinary, clsConstant.PCS_SETUP_DATA_NUM, "SETUP");
                command.Parameters.Add("@WAVE", SqlDbType.VarBinary, clsConstant.CDFR_WAVE_SIZE, "WAVE");
                command.Parameters.Add("@ACK", SqlDbType.Bit, 0, "ACK");
                
                m_adtWaveForm.InsertCommand = command;

                m_datasetWaveForm = new DataSet();

#if USE_EXCEPTION
            }
            catch (Exception e)
            {
                g_strWaveFormLogError = "WaveForm_Make : " + e.Message;
            }
#endif
        }

        public bool WaveForm_Add(clsWaveFormInfo waveForm, ref String strFile)
        {
            bool bResult = true;

#if USE_EXCEPTION
            try
            {
#endif
                /*
                m_adtWaveForm.InsertCommand.Parameters["@ID"].Value = waveForm.m_nDeviceNo;
                m_adtWaveForm.InsertCommand.Parameters["@METER"].Value = waveForm.m_strDeviceName;
                m_adtWaveForm.InsertCommand.Parameters["@DT"].Value = waveForm.m_dtWaveForm;
                m_adtWaveForm.InsertCommand.Parameters["@MS"].Value = waveForm.m_nMs;
                m_adtWaveForm.InsertCommand.Parameters["@EVENT"].Value = waveForm.m_nEvent;
                m_adtWaveForm.InsertCommand.Parameters["@TRIG"].Value = waveForm.m_nTrigger;
                m_adtWaveForm.InsertCommand.Parameters["@CH"].Value = waveForm.m_nEventCh;
                m_adtWaveForm.InsertCommand.Parameters["@FILEPOS"].Value = waveForm.m_nFilePos;
                m_adtWaveForm.InsertCommand.Parameters["@FILETYPE"].Value = waveForm.m_nFileType;
                m_adtWaveForm.InsertCommand.Parameters["@FILETYPESTR"].Value = waveForm.m_strFileType;
                m_adtWaveForm.InsertCommand.Parameters["@MSG"].Value = waveForm.m_strMsg;
                m_adtWaveForm.InsertCommand.Parameters["@SETUPSIZE"].Value = waveForm.m_nSetupSize;
                m_adtWaveForm.InsertCommand.Parameters["@WAVESIZE"].Value = waveForm.m_nWaveSize;
                m_adtWaveForm.InsertCommand.Parameters["@SETUP"].Value = waveForm.m_bSetupData;
                m_adtWaveForm.InsertCommand.Parameters["@WAVE"].Value = waveForm.m_bWaveData;
                m_adtWaveForm.InsertCommand.Parameters["@ACK"].Value = false;

                m_adtWaveForm.InsertCommand.ExecuteNonQuery();
                */
                strFile = String.Format(clsConstant.APP_PATH + "fault\\trigger_{0}_{1:0000}{2:00}{3:00}_{4:00}{5:00}{6:00}_{7:000}.dat", 
                    waveForm.m_strDeviceName,
                    waveForm.m_dtWaveForm.Year,
                    waveForm.m_dtWaveForm.Month,
                    waveForm.m_dtWaveForm.Day,
                    waveForm.m_dtWaveForm.Hour,
                    waveForm.m_dtWaveForm.Minute,
                    waveForm.m_dtWaveForm.Second,
                    waveForm.m_nMs);

                using (FileStream writeStream = new FileStream(strFile, FileMode.Create))
                {
                    BinaryWriter writeBinay = new BinaryWriter(writeStream);

                    writeBinay.Write(waveForm.m_nDeviceNo);
                    writeBinay.Write(waveForm.m_strDeviceName);
                    writeBinay.Write(waveForm.m_dtWaveForm.Year);
                    writeBinay.Write(waveForm.m_dtWaveForm.Month);
                    writeBinay.Write(waveForm.m_dtWaveForm.Day);
                    writeBinay.Write(waveForm.m_dtWaveForm.Hour);
                    writeBinay.Write(waveForm.m_dtWaveForm.Minute);
                    writeBinay.Write(waveForm.m_dtWaveForm.Second);
                    writeBinay.Write(waveForm.m_nMs);
                    writeBinay.Write(waveForm.m_nEvent);
                    writeBinay.Write(waveForm.m_nTrigger);
                    writeBinay.Write(waveForm.m_nEventCh);
                    writeBinay.Write(waveForm.m_nFilePos);
                    writeBinay.Write(waveForm.m_nFileType);
                    writeBinay.Write(waveForm.m_strFileType);
                    writeBinay.Write(waveForm.m_strMsg);
                    writeBinay.Write(waveForm.m_nSetupSize);
                    writeBinay.Write(waveForm.m_nWaveSize);
                    writeBinay.Write(waveForm.m_bSetupData, 0, waveForm.m_nSetupSize);
                    writeBinay.Write(waveForm.m_bWaveData, 0, (int)waveForm.m_nWaveSize);
                    writeBinay.Close();
                }

                if (waveForm.m_nFileType == 0)
                {
                    DisplayLog(waveForm.m_dtWaveForm, string.Format("[{0}] : {1} 순시 사고 파형 데이터 저장", waveForm.m_strDeviceName, waveForm.m_dtWaveForm));
                }
                else
                {
                    DisplayLog(waveForm.m_dtWaveForm, string.Format("[{0}] : {1} 페이저 사고 파형 데이터 저장", waveForm.m_strDeviceName, waveForm.m_dtWaveForm));
                }
#if USE_EXCEPTION
            }
            catch (Exception e)
            {
                g_strWaveFormLogError = "WaveForm_Add : " + e.Message;
                bResult = false;
            }
#endif
            return bResult;
        }

        public bool WaveForm_CheckDuplicate(clsWaveFormInfo waveForm)
        {
            bool bResult = false;

            g_strWaveFormLogError = "";

#if USE_EXCEPTION
            try
            {
#endif
                SqlCommand command = new SqlCommand("SELECT * FROM tblWaveForm WHERE (METER=@METER) AND (DT=@DT) AND (MS=@MS) AND (FILETYPE=@FILETYPE)", m_conSql);

                command.CommandType = CommandType.Text;

                command.Parameters.Add("@METER", SqlDbType.NVarChar, 255, "METER");
                command.Parameters.Add("@DT", SqlDbType.DateTime, 0, "DT");
                command.Parameters.Add("@MS", SqlDbType.Int, 0, "MS");
                command.Parameters.Add("@FILETYPE", SqlDbType.Int, 0, "FILETYPE");
                command.Parameters["@METER"].Value = waveForm.m_strDeviceName;
                command.Parameters["@DT"].Value = waveForm.m_dtWaveForm;
                command.Parameters["@MS"].Value = waveForm.m_nMs;
                command.Parameters["@FILETYPE"].Value = waveForm.m_nFileType;

                m_adtWaveForm.SelectCommand = command;

                m_adtWaveForm.Fill(m_datasetWaveForm);

                if (m_datasetWaveForm.Tables[0].Rows.Count > 0)
                {
                    m_datasetWaveForm.Clear();
                    bResult = true;
                }
#if USE_EXCEPTION
            }
            catch (Exception e)
            {
                g_strWaveFormLogError = "WaveForm_CheckDuplicate : " + e.Message;
            }
#endif
            return bResult;
        }
    }
}
