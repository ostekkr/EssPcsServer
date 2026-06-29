using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using C1.Win.FlexGrid;

namespace EssPcsServer
{
    public delegate void delegateDisplayLog(DateTime t, string msg);

    public partial class clsSqlServer
    {
        private SqlDataAdapter m_adtSysLog;
        private BindingSource m_bsSysLog;
        public String g_strSysLogError = "";

        public event delegateDisplayLog DisplayLog;

        public void SysLog_Make()
        {
            m_adtSysLog = new SqlDataAdapter();

            m_adtSysLog.SelectCommand = new SqlCommand("SELECT * FROM tblSysLog", m_conSql);
            m_adtSysLog.SelectCommand.CommandType = CommandType.Text;

            m_adtSysLog.InsertCommand = new SqlCommand("INSERT INTO tblSysLog (MSG,DT) VALUES (@MSG,@DT)", m_conSql);
            m_adtSysLog.InsertCommand.CommandType = CommandType.Text;

            m_adtSysLog.InsertCommand.Parameters.Add("@MSG", SqlDbType.NVarChar, 255, "MSG");
            m_adtSysLog.InsertCommand.Parameters.Add("@DT", SqlDbType.DateTime, 0, "DT");
        }

        public bool SysLog_Add(clsSystemLog sysLog)
        {
            bool bResult = true;

            try
            {
                String strLog;

                if ((sysLog.m_nDevice >= 0) && (sysLog.m_nDevice <= 31))
                {
                    strLog = String.Format("[{0}] {1}", m_clsSettingInfo[sysLog.m_nDevice].strDevice, sysLog.m_strLog);
                }
                else
                {
                    strLog = sysLog.m_strLog;
                }

                clsLog.addSystemLog(sysLog.m_strLog);

                DisplayLog(DateTime.Now, sysLog.m_strLog);
            }
            catch (Exception e)
            {
                g_strSysLogError = "SysLog_Add : " + e.Message;
                clsLog.addSqlLog(g_strSysLogError);
                bResult = false;
            }

            return bResult;
        }
    }
}
