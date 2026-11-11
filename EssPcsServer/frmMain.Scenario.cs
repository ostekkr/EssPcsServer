using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EssPcsServer
{
    public partial class frmMain
    {
        private string m_strLastScenarioJson = "";
        private clsScenarioCommandSet m_scenarioCommandSet = new clsScenarioCommandSet();

        /// <summary>getScenarioJson()을 백그라운드 WCF 큐에 넣어 호출하고, 새 JSON이 오면 그리드에 반영한다.</summary>
        public void PollScenarioJson()
        {
            EnqueueWcf(() =>
            {
                if (m_EssPcsServiceProxy == null) return;

                string strJson = m_EssPcsServiceProxy.getScenarioJson();

                if (string.IsNullOrWhiteSpace(strJson)) return;
                if (strJson == m_strLastScenarioJson) return;

                m_strLastScenarioJson = strJson;

                this.BeginInvoke(new Action(() => LoadScenarioCommand(strJson)));
            });
        }

        /// <summary>JSON 하나를 해당 시나리오 슬롯에만 반영한다. 이미 받아둔 다른 시나리오는 유지된다.</summary>
        public void LoadScenarioCommand(string strJson)
        {
            try
            {
                foreach (string strError in m_scenarioCommandSet.AddJson(strJson))
                {
                    clsLog.addSystemLog("시나리오 거부 : " + strError);
                }

                commandPropertyGridControl.SelectedObject = m_scenarioCommandSet;
                commandPropertyGridControl.Refresh();
            }
            catch (Exception ex)
            {
                clsLog.addSystemLog(string.Format("LoadScenarioCommand: Exception occurred: {0}", ex.Message));
            }
        }
    }
}
