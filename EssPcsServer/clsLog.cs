using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EssPcsServer
{
    public static class clsLog
    {
        private static String m_strRtuLogPath;
        private static String m_strSqlLogPath;
        private static String m_strExceptionLogPath;
        private static String m_strSystemLogPath;
        private static int m_nRtuLogDay = 0;
        private static int m_nSqlLogDay = 0;
        private static int m_nExceptionLogDay = 0;
        private static int m_nSysLogDay = 0;
        public static List<String> m_RtuLogList = new List<String>();
        public static List<String> m_SqlLogList = new List<String>();
        public static List<String> m_ExceptionLogList = new List<String>();
        public static List<String> m_SystemLogList = new List<String>();

        public static void openRtuLog()
        {
            try
            {
                DateTime t = DateTime.Now;

                m_strRtuLogPath = String.Format("{0}\\RtuLog_{1:0000}{2:00}{3:00}_{4:00}{5:00}{6:00}.txt",
                    clsConstant.APP_PATH + @"log\tcplog",
                    t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);

                if (!File.Exists(m_strRtuLogPath))
                {
                    using (File.CreateText(m_strRtuLogPath)) ;
                }

                m_nRtuLogDay = t.Day;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} > {1}", DateTime.Now.ToString(), ex.Message);
            }
            finally
            {
                closeRtuLog();
            }
        }

        public static void closeRtuLog()
        {
            m_RtuLogList.Clear();
        }

        public static void addRtuLog(String strLog)
        {
            m_RtuLogList.Add(strLog);
        }

        public static void writeRtuLog()
        {
            try
            {
                DateTime t = DateTime.Now;

                if (!File.Exists(m_strRtuLogPath))
                {
                    openRtuLog();
                }

                if (t.Day != m_nRtuLogDay)
                {
                    openRtuLog();
                }

                using (FileStream f = new FileStream(m_strRtuLogPath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter s = new StreamWriter(f))
                    {
                        foreach (String strLog in m_RtuLogList)
                        {
                            s.WriteLine(String.Format("{0}.{1:000} > {2}", t.ToString(), t.Millisecond, strLog));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} > {1}", DateTime.Now.ToString(), ex.Message);
            }
            finally
            {
                closeRtuLog();
            }
        }

        public static void openSqlLog()
        {
            try
            {
                DateTime t = DateTime.Now;

                m_strSqlLogPath = String.Format("{0}\\SqlLog_{1:0000}{2:00}{3:00}_{4:00}{5:00}{6:00}.txt",
                    clsConstant.APP_PATH + @"log\sqllog",
                    t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);

                if (!File.Exists(m_strSqlLogPath))
                {
                    using (File.CreateText(m_strSqlLogPath)) ;
                }

                m_nSqlLogDay = t.Day;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} > {1}", DateTime.Now.ToString(), ex.Message);
            }
            finally
            {
                closeSqlLog();
            }
        }

        public static void closeSqlLog()
        {
            m_SqlLogList.Clear();
        }

        public static void addSqlLog(String strLog)
        {
            m_SqlLogList.Add(strLog);
        }

        public static void writeSqlLog()
        {
            try
            {
                DateTime t = DateTime.Now;

                if (!File.Exists(m_strSqlLogPath))
                {
                    openSqlLog();
                }

                if (t.Day != m_nSqlLogDay)
                {
                    openSqlLog();
                }

                using (FileStream f = new FileStream(m_strSqlLogPath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter s = new StreamWriter(f))
                    {
                        foreach (String strLog in m_SqlLogList)
                        {
                            s.WriteLine(String.Format("{0}.{1:000} > {2}", t.ToString(), t.Millisecond, strLog));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} > {1}", DateTime.Now.ToString(), ex.Message);
            }
            finally
            {
                closeSqlLog();
            }
        }

        public static void openExceptiontLog()
        {
            try
            {
                DateTime t = DateTime.Now;

                m_strExceptionLogPath = String.Format("{0}\\ExceptionLog_{1:0000}{2:00}{3:00}_{4:00}{5:00}{6:00}.txt",
                    clsConstant.APP_PATH + @"log\exptlog",
                    t.Year, t.Month, t.Day, t.Hour, t.Minute, t.Second);

                if (!File.Exists(m_strExceptionLogPath))
                {
                    using (File.CreateText(m_strExceptionLogPath)) ;
                }

                m_nExceptionLogDay = t.Day;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} > {1}", DateTime.Now.ToString(), ex.Message);
            }
            finally
            {
                closeExceptionLog();
            }
        }

        public static void closeExceptionLog()
        {
            m_ExceptionLogList.Clear();
        }

        public static void addExceptionLog(String strLog)
        {
            m_ExceptionLogList.Add(strLog);
        }

        public static void writeExceptionLog()
        {
            try
            {
                DateTime t = DateTime.Now;

                if (!File.Exists(m_strExceptionLogPath))
                {
                    openExceptiontLog();
                }

                if (t.Day != m_nExceptionLogDay)
                {
                    openExceptiontLog();
                }

                using (FileStream f = new FileStream(m_strExceptionLogPath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter s = new StreamWriter(f))
                    {
                        foreach (String strLog in m_ExceptionLogList)
                        {
                            s.WriteLine(String.Format("{0}.{1:000} > {2}", t.ToString(), t.Millisecond, strLog));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} > {1}", DateTime.Now.ToString(), ex.Message);
            }
            finally
            {
                closeExceptionLog();
            }
        }

        public static void openSystemLog()
        {
            try
            {
                DateTime t = DateTime.Now;

                m_strSystemLogPath = String.Format(clsConstant.APP_PATH + @"log\syslog" + "\\SysLog_{0:0000}{1:00}{2:00}.txt",
                    t.Year, t.Month, t.Day);

                if (!File.Exists(m_strSystemLogPath))
                {
                    using (File.CreateText(m_strSystemLogPath)) ;
                }

                m_nSysLogDay = t.Day;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} > {1}", DateTime.Now.ToString(), ex.Message);
            }
            finally
            {
                closeSystemLog();
            }
        }

        public static void closeSystemLog()
        {
            m_SystemLogList.Clear();
        }

        public static void addSystemLog(String strLog)
        {
            m_SystemLogList.Add(strLog);
        }

        public static void writeSystemLog()
        {
            try
            {
                DateTime t = DateTime.Now;

                if (!File.Exists(m_strSystemLogPath))
                {
                    openSystemLog();
                }

                if (t.Day != m_nSysLogDay)
                {
                    openSystemLog();
                }

                using (FileStream f = new FileStream(m_strSystemLogPath, FileMode.Append, FileAccess.Write))
                {
                    using (StreamWriter s = new StreamWriter(f))
                    {
                        foreach (String strLog in m_SystemLogList)
                        {
                            s.WriteLine(String.Format("{0}.{1:000} > {2}", t.ToString(), t.Millisecond, strLog));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("{0} > {1}", DateTime.Now.ToString(), ex.Message);
            }
            finally
            {
                closeSystemLog();
            }
        }
    }
}
