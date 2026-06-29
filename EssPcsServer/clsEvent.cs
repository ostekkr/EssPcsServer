using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EssPcsServer
{
    public class clsEventInfo
    {
        public int m_nEventType;
        public int m_nEventLength;
        public byte[] m_bEventData;

        public clsEventInfo(int nEvent, byte[] byteEvent)
        {
            m_nEventType = nEvent;
            m_nEventLength = 0;

            if (m_nEventType == clsConstant.PQMS_SYSTEM_EVENT)
            {
                m_bEventData = new byte[6];
                m_nEventLength = 6;
                Array.Copy(byteEvent, 0, m_bEventData, 0, 6);
            }
            else if (m_nEventType == clsConstant.PQMS_PQ_EVENT)
            {
                m_bEventData = new byte[12];
                m_nEventLength = 12;
                Array.Copy(byteEvent, 0, m_bEventData, 0, 12);
            }
            else if (m_nEventType == clsConstant.PQMS_LOAD_PROFILE)
            {
                m_bEventData = new byte[12];
                m_nEventLength = 12;
                Array.Copy(byteEvent, 0, m_bEventData, 0, 12);
            }
        }
    }

    public class clsEvent
    {
        public List<clsEventInfo> m_arrayEvent = new List<clsEventInfo>();

        public void clearEvent()
        {
            m_arrayEvent.Clear();
        }

        public void addWaveInfo(byte[] receiveBytes)
        {
            byte[] buf = new byte[6];

            Array.Copy(receiveBytes, 3, buf, 0, 6);
            m_arrayEvent.Add(new clsEventInfo(clsConstant.PQMS_SYSTEM_EVENT, buf));
        }
    }

    public class clsStatusEventInfo
    {
        public int nDevice = 0;
        public int nEvent = 0;

        public clsStatusEventInfo(int device, int status)
        {
            nDevice = device;
            nEvent = status;
        }
    }
}
