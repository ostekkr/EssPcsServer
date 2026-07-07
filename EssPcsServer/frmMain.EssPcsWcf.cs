using EssPcsService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace EssPcsServer
{
    public partial class frmMain
    {
        List<clsEssPcsMsg> m_EssPcsMsgList = new List<clsEssPcsMsg>();
        IEssPcsService m_EssPcsServiceProxy = null;
        DuplexChannelFactory<IEssPcsService> m_EssPcsChannelFactory = null;
        bool m_bEssPcsServiceFault = false;

        void onEssPcsChannelFaulted(object sender, EventArgs e)
        {
            String strLog = "";

            if (!m_bEssPcsServiceFault)
            {
                strLog = String.Format("{0} > {1} Faulted", DateTime.Now.ToString(), "onChannelFaulted");
                m_bEssPcsServiceFault = true;
                if (m_EssPcsServiceProxy != null)
                {
                    ((ICommunicationObject)m_EssPcsServiceProxy).Abort();
                }
            }
            else
            {
                strLog = String.Format("{0} > {1}", DateTime.Now.ToString(), "onChannelFaulted");
            }

            clsLog.addSystemLog(strLog);
        }

        void onEssPcsChannelClosed(object sender, EventArgs e)
        {
            if (m_bEssPcsServiceFault)
            {
                String strLog = "";
                Int32 i;

                strLog = String.Format("{0} > {1}", DateTime.Now.ToString(), "onChannelClosed");

                clsLog.addSystemLog(strLog);

                m_EssPcsServiceProxy = m_EssPcsChannelFactory.CreateChannel();
                ((ICommunicationObject)m_EssPcsServiceProxy).Faulted += new EventHandler(onEssPcsChannelFaulted);
                ((ICommunicationObject)m_EssPcsServiceProxy).Closed += new EventHandler(onEssPcsChannelClosed);

                while (true)
                {
                    try
                    {
                        m_EssPcsServiceProxy.StartService(true);
                        break;
                    }
                    catch (Exception ex)
                    {
                        strLog = String.Format("{0} > {1} : {2}", DateTime.Now.ToString(), "onChannelClosed", ex.Message);
                        clsLog.addSystemLog(strLog);
                        /*
                        MessageBox.Show(exception.Message, "서비스 시작 오류");
                        Application.ExitThread();
                        Environment.Exit(0);
                        return;
                        */
                    }
                }

                m_EssPcsServiceProxy.ClearSetting();

                System.Diagnostics.Debug.WriteLine("Server Session ID [" + m_EssPcsServiceProxy.SessionId() + "]");

                for (i = 0; i < g_clsSqlServer.m_clsSettingInfo.Count; i++)
                {
                    m_EssPcsServiceProxy.AddSetting(g_clsSqlServer.m_clsSettingInfo[i], g_clsSqlServer.m_clsTrendInfo[i]);
                }

                for (i = 0; i < 32; i++)
                {
                    m_clsGateway[i].setProxy(ref m_EssPcsServiceProxy);
                }

                m_bEssPcsServiceFault = false;
            }
        }

        private void openEssPcsWcf()
        {
            //  ESS PCS WCF
            InstanceContext ctx = new InstanceContext(new EssPcsCallbackHandler(m_EssPcsMsgList));
            Uri uri = new Uri("net.tcp://localhost:60008/wcf/esspcsservice");
            NetTcpBinding myBinding = new NetTcpBinding();
            myBinding.SendTimeout = new TimeSpan(0, 0, 30);

            myBinding.Security.Mode = SecurityMode.None;

            ServiceEndpoint myEndPoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(IEssPcsService)), myBinding, new EndpointAddress(uri));

            m_EssPcsChannelFactory = new DuplexChannelFactory<IEssPcsService>(ctx, myEndPoint);
            m_EssPcsServiceProxy = m_EssPcsChannelFactory.CreateChannel();
            ((ICommunicationObject)m_EssPcsServiceProxy).Faulted += new EventHandler(onEssPcsChannelFaulted);
            ((ICommunicationObject)m_EssPcsServiceProxy).Closed += new EventHandler(onEssPcsChannelClosed);
        }
    }

    public class EssPcsCallbackHandler : IEssPcsCallback
    {
        List<clsEssPcsMsg> m_EssPcsMsgList;

        public EssPcsCallbackHandler(List<clsEssPcsMsg> msgList)
        {
            m_EssPcsMsgList = msgList;
        }

        public void SendToMsg(clsEssPcsMsg msg)
        {
            //  System.Diagnostics.Debug.WriteLine(msg.ToString());
            m_EssPcsMsgList.Add(msg);
        }
    }
}
