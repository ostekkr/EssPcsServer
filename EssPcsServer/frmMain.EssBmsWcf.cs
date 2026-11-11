using EssBmsService;
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
        List<clsEssBmsMsg> m_EssBmsMsgList = new List<clsEssBmsMsg>();
        IEssBmsService m_EssBmsServiceProxy = null;
        DuplexChannelFactory<IEssBmsService> m_EssBmsChannelFactory = null;
        bool m_bEssBmsServiceFault = false;

        void onEssBmsChannelFaulted(object sender, EventArgs e)
        {
            String strLog = "";

            if (!m_bEssBmsServiceFault)
            {
                strLog = String.Format("{0} > {1} Faulted", DateTime.Now.ToString(), "onChannelFaulted");
                m_bEssBmsServiceFault = true;

                if (m_EssBmsServiceProxy != null)
                {
                    ((ICommunicationObject)m_EssBmsServiceProxy).Abort();
                }
            }
            else
            {
                strLog = String.Format("{0} > {1}", DateTime.Now.ToString(), "onChannelFaulted");
            }

            clsLog.addSystemLog(strLog);
        }

        void onEssBmsChannelClosed(object sender, EventArgs e)
        {
            if (m_bEssBmsServiceFault)
            {
                String strLog = "";
                Int32 i;

                strLog = String.Format("{0} > {1}", DateTime.Now.ToString(), "onChannelClosed");

                clsLog.addSystemLog(strLog);

                if (m_bDaqRun)
                {
                    ReconnectToBmsServer();
                }

                m_bEssBmsServiceFault = false;
            }
        }

        private void ReconnectToBmsServer()
        {
            if (m_EssBmsServiceProxy != null)
            {
                String strLog = "";

                try
                {
                    ((IClientChannel)m_EssBmsServiceProxy).Close();
                    m_EssBmsChannelFactory.Close();
                }
                catch (CommunicationException exception)
                {
                    strLog = "ReconnectToEssBmsServer: " + exception.Message + "\nTrack>\n" + exception.StackTrace;
                    displayLog(strLog);
                    clsLog.addExceptionLog(strLog);
                }
                catch (TimeoutException exception)
                {
                    strLog = "ReconnectToEssBmsServer: " + exception.Message + "\nTrack>\n" + exception.StackTrace;
                    displayLog(strLog);
                    clsLog.addExceptionLog(strLog);
                }
                finally
                {
                    m_EssBmsServiceProxy = null;
                }
            }

            openEssBmsWcf();
        }

        private void openEssBmsWcf()
        {
            //  ESS PCS WCF
            InstanceContext ctx = new InstanceContext(new EssBmsCallbackHandler(m_EssBmsMsgList));
            Uri uri = new Uri("net.tcp://localhost:60009/wcf/essbmsservice");
            NetTcpBinding myBinding = new NetTcpBinding();
            myBinding.SendTimeout = new TimeSpan(0, 0, 30);

            myBinding.Security.Mode = SecurityMode.None;

            ServiceEndpoint myEndPoint = new ServiceEndpoint(ContractDescription.GetContract(typeof(IEssBmsService)), myBinding, new EndpointAddress(uri));

            m_EssBmsChannelFactory = new DuplexChannelFactory<IEssBmsService>(ctx, myEndPoint);
            m_EssBmsServiceProxy = m_EssBmsChannelFactory.CreateChannel();
            ((ICommunicationObject)m_EssBmsServiceProxy).Faulted += new EventHandler(onEssBmsChannelFaulted);
            ((ICommunicationObject)m_EssBmsServiceProxy).Closed += new EventHandler(onEssBmsChannelClosed);

            m_EssBmsServiceProxy.StartService();
        }
    }

    public class EssBmsCallbackHandler : IEssBmsCallback
    {
        List<clsEssBmsMsg> m_EssBmsMsgList;

        public EssBmsCallbackHandler(List<clsEssBmsMsg> msgList)
        {
            m_EssBmsMsgList = msgList;
        }

        public void SendToMsg(clsEssBmsMsg msg)
        {
            //  System.Diagnostics.Debug.WriteLine(msg.ToString());
            m_EssBmsMsgList.Add(msg);
        }
    }
}
