using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EssPcsServer
{
    public partial class frmMain
    {
        public void LoadScenarioCommand(string strJson)
        {
            try
            {
                var set = clsScenarioCommandSet.FromJsonArray(new[] { strJson });

                commandPropertyGridControl.SelectedObject = set;
            }
            catch (Exception ex)
            {
                clsLog.addSystemLog(string.Format("LoadScenarioCommand: Exception occurred: {0}", ex.Message));
            }
        }
    }
}
