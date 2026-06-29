using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Data;
using EssPcsService;

namespace EssPcsServer
{
    public class clsTblGroup
    {
        public List<clsGroupInfo> m_GroupList = new List<clsGroupInfo>();
        public SqlConnection m_conSql;

        public clsTblGroup()
        {
        }

        public bool Open()
        {
            return clsMsSQL.connect(ref m_conSql);
        }

        public void Close()
        {
            if (m_GroupList.Count > 0)
            {
                m_GroupList.Clear();
            }
            clsMsSQL.disconnect(ref m_conSql);
        }

        public bool Read()
        {
            SqlDataReader R = null;
            SqlCommand Com;
            String strSql;

            try
            {
                m_GroupList.Clear();

                strSql = String.Format("SELECT * FROM tblGroup ORDER BY GROUP_ORDER");
                Com = new SqlCommand(strSql, m_conSql);
                R = Com.ExecuteReader();

                while (R.Read())
                {
                    clsGroupInfo groupInfo = new clsGroupInfo();

                    groupInfo.nGroup = Int32.Parse(R["GROUP_NO"].ToString());
                    groupInfo.strGroup = R["GROUP_NAME"].ToString();
                    groupInfo.nOrder = Int32.Parse(R["GROUP_ORDER"].ToString());

                    m_GroupList.Add(groupInfo);
                }
                R.Close();

                return true;
            }
            catch (Exception ex)
            {
                if(R != null)
                {
                    if(!R.IsClosed)
                    {
                        R.Close();
                    }
                }

                MessageBox.Show("Group List Read : " + ex.Message, "SQL Error");

                return false;
            }
        }

        public bool Delete(int nNO)
        {
            SqlCommand Com;
            String strSql;
            SqlDataReader R;
            int nCount;

            try
            {
                strSql = String.Format("SELECT COUNT(NO) FROM tblSetting WHERE GROUPNO={0}", nNO);

                Com = new SqlCommand(strSql, m_conSql);                
                R = Com.ExecuteReader();
                
                R.Read();
                nCount = Int32.Parse(R[0].ToString());
                R.Close();

                if (nCount > 0)
                {
                    MessageBox.Show("삭제하려는 그룹에 포함된 장치가 있습니다.\n그룹에 포함된 장치를 제거하세요.", "그룹 삭제 오류");
                    return false;
                }

                Com.Dispose();

                strSql = String.Format("DELETE FROM tblGroup WHERE GROUP_NO={0}", nNO);
                Com = new SqlCommand(strSql, m_conSql);
                Com.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "그룹 삭제 오류");

                return false;
            }
        }

        public bool Update(int nNO, String strName, int nOrder)
        {
            SqlCommand Com;
            String strSql;

            try
            {
                strSql = "UPDATE tblGroup SET GROUP_NAME=@GROUP_NAME,GROUP_ORDER=@GROUP_ORDER WHERE GROUP_NO=@GROUP_NO";
                Com = new SqlCommand(strSql, m_conSql);

                Com.Parameters.Add("@GROUP_NAME", SqlDbType.NVarChar, 50);
                Com.Parameters.Add("@GROUP_ORDER", SqlDbType.Int);
                Com.Parameters.Add("@GROUP_NO", SqlDbType.BigInt);

                Com.Parameters["@GROUP_NO"].Value = nNO;
                Com.Parameters["@GROUP_NAME"].Value = strName;
                Com.Parameters["@GROUP_ORDER"].Value = nOrder;

                Com.ExecuteNonQuery();

                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "그룹 수정 오류");

                return false;
            }
        }

        public bool Group_Add(String strName, int nOrder)
        {
            SqlCommand Com;
            String strSql;

            try
            {
                strSql = "INSERT INTO tblGroup (GROUP_NAME,GROUP_ORDER) VALUES (@GROUP_NAME,@GROUP_ORDER)";
                Com = new SqlCommand(strSql, m_conSql);

                Com.Parameters.Add("@GROUP_NAME", SqlDbType.NVarChar, 50);
                Com.Parameters.Add("@GROUP_ORDER", SqlDbType.Int);

                Com.Parameters["@GROUP_NAME"].Value = strName;
                Com.Parameters["@GROUP_ORDER"].Value = nOrder;

                Com.ExecuteNonQuery();

                return true;
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("Group_Add: " + exception.Message);

                return false;
            }
        }

        public bool IsGroupName(String strGroup)
        {
            SqlCommand Com;
            String strSql;
            SqlDataReader R;
            int nCount;

            try
            {
                strSql = "SELECT COUNT(GROUP_NO) FROM tblGroup WHERE GROUP_NAME=@GROUP_NAME";
                Com = new SqlCommand(strSql, m_conSql);

                Com.Parameters.Add("@GROUP_NAME", SqlDbType.NVarChar, 50);
                Com.Parameters["@GROUP_NAME"].Value = strGroup;

                R = Com.ExecuteReader();
                nCount = Int32.Parse(R[0].ToString());
                R.Close();

                if (nCount > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                System.Diagnostics.Debug.WriteLine("IsGroupName: " + exception.Message);

                return true;
            }
        }
    }
}
