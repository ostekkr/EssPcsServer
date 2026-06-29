using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data;
using System.Data.SqlClient;
using C1.Win.FlexGrid;
using System.Data.Common;

namespace EssPcsServer
{
    public class clsMsSQL
    {
        public static bool connect(ref SqlConnection conSql, String strIp = null, Int32 nPort = -1, String strUser = null, String strPassword = null)
        {
            if (conSql != null)
            {
                if (conSql.State == ConnectionState.Open)
                {
                    conSql.Close();
                }
                conSql.Dispose();
                conSql = null;
            }

            if (strIp == null)
            {
                //strIp = Properties.Settings.Default.sqlServerIp;
            }

            if (nPort == -1)
            {
                //nPort = Properties.Settings.Default.sqlServerPort;
            }

            if (strUser == null)
            {
                //strUser = Properties.Settings.Default.sqlServerUser;
            }

            if (strPassword == null)
            {
                //strPassword = Properties.Settings.Default.sqlServerPassword;
            }

            conSql = new SqlConnection();

            conSql.ConnectionString = string.Format("Server={0},{1};database=esspcsdb;User ID={2};Pwd={3}", strIp, nPort, strUser, strPassword);

            try
            {
                conSql.Open();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static void disconnect(ref SqlConnection conSql)
        {
            if (conSql != null)
            {
                if (conSql.State == ConnectionState.Open)
                {
                    conSql.Close();
                }
                conSql.Dispose();
                conSql = null;
            }
        }
    }

    public partial class clsSqlServer
    {
        private SqlConnection m_conSql;

        private DataSet m_datasetSql;

        public bool g_bIsUse = false;

        public clsSqlServer()
        {
//            m_conSql = new SqlConnection();
        }

        public bool connectSqlServer()
        {
            if (clsMsSQL.connect(ref m_conSql))
            {
                m_datasetSql = new DataSet("esspcsdb");
                return true;
            }
            else
            {
                return false;
            }
        }

        public void disconnectSqlServer()
        {
            clsMsSQL.disconnect(ref m_conSql);
        }

        public float validateFloat(float fValue)
        {
            float fResult = fValue;

            fValue=Math.Abs(fValue);

            if(fValue < 0.000001 || fValue > float.MaxValue)
            {
                fResult=0;
            }

            return fResult;
        }

        public int GetDeviceIndex(int nDeviceNo)
        {
            int i;

            for (i = 0; i < m_clsSettingInfo.Count; i++)
            {
                if (m_clsSettingInfo[i].nDeviceNo == nDeviceNo)
                {
                    return i;
                }
            }

            return -1;
        }
    }
}
