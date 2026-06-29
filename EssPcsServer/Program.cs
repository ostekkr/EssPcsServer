using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace EssPcsServer
{
    static class Program
    {
        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            String mtxName = "EssPcsServer";
            Mutex mtx = new Mutex(true, mtxName);

            TimeSpan tsWait = new TimeSpan(0, 0, 1);
            bool success = mtx.WaitOne(tsWait);

            if (!success)
            {
                MessageBox.Show("이미 실행 중입니다.", "ESS-PCS Data Server");
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            frmMain frmM = new frmMain();

            Application.Run(frmM);
        }
    }
}
