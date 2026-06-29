using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EssPcsServer
{
    public partial class frmWaitThread : Form
    {
        private bool m_bRun = false;
        private DateTime m_dtStart = DateTime.Now;

        public frmWaitThread()
        {
            InitializeComponent();

            m_bRun = true;
        }

        private void timerWait_Tick(object sender, EventArgs e)
        {
            if (m_bRun)
            {
                TimeSpan ts = DateTime.Now - m_dtStart;
                labelTime.Text = String.Format("경과 시간 : {0:F0}초", ts.TotalSeconds);
            }
            else
            {
                Close();
            }
        }

        private void frmWaitThread_Load(object sender, EventArgs e)
        {
            m_bRun = true;
            m_dtStart = DateTime.Now;
            labelTime.Text = "경과 시간 : 0초";
        }

        public void stopWait()
        {
            m_bRun = false;
        }
    }
}
