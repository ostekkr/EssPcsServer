using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace EssPcsServer
{
    public class clsModBus
    {
        private readonly static int MAX_SEND_PACKET = 1500;
        public byte[] g_bSendBuffer = new byte[MAX_SEND_PACKET];
        public UInt16 m_nTxSeq = 0;

        public clsModBus()
        {
        }

        public int modbusSelectSBO(int nUnitId)
        {
            UInt16 crc16;
            int len = 0;

            g_bSendBuffer[len++] = (byte)nUnitId;
            g_bSendBuffer[len++] = 0x05;
            g_bSendBuffer[len++] = 0x00;
            g_bSendBuffer[len++] = 0x6E;
            g_bSendBuffer[len++] = 0xFF;
            g_bSendBuffer[len++] = 0x00;

            crc16 = clsCrc16.crc16(g_bSendBuffer, len);

            g_bSendBuffer[len++] = (byte)((crc16 >> 0) & 0xFF);
            g_bSendBuffer[len++] = (byte)((crc16 >> 8) & 0xFF);

            return len;
        }

        public int modbusWriteSingle(int nUnitId, int nAddress, int nData)
        {
            UInt16 crc16;
            int len = 0;

            g_bSendBuffer[len++] = (byte)nUnitId;
            g_bSendBuffer[len++] = 0x06;
            g_bSendBuffer[len++] = (byte)((nAddress >> 8) & 0xff);
            g_bSendBuffer[len++] = (byte)((nAddress >> 0) & 0xff);
            g_bSendBuffer[len++] = (byte)((nData >> 8) & 0xff);
            g_bSendBuffer[len++] = (byte)((nData >> 0) & 0xff);

            crc16 = clsCrc16.crc16(g_bSendBuffer, len);

            g_bSendBuffer[len++] = (byte)((crc16 >> 0) & 0xFF);
            g_bSendBuffer[len++] = (byte)((crc16 >> 8) & 0xFF);

            return len;
        }

        public int modbusWriteMultiple(int nUnitId, int nAddress, byte[] bTxBuf, int nTxLen)
        {
            UInt16 crc16;
            UInt16 nData;
            int len = 0;
            int i;

            g_bSendBuffer[len++] = (byte)nUnitId;
            g_bSendBuffer[len++] = 0x10;
            g_bSendBuffer[len++] = (byte)((nAddress >> 8) & 0xff);
            g_bSendBuffer[len++] = (byte)((nAddress >> 0) & 0xff);
            nData = (UInt16)(nTxLen / 2);
            g_bSendBuffer[len++] = (byte)((nData >> 8) & 0xff);
            g_bSendBuffer[len++] = (byte)((nData >> 0) & 0xff);
            g_bSendBuffer[len++] = (byte)(nTxLen);

            for (i = 0; i < nTxLen; i++)
            {
                g_bSendBuffer[len++] = bTxBuf[i];
            }

            crc16 = clsCrc16.crc16(g_bSendBuffer, len);

            g_bSendBuffer[len++] = (byte)((crc16 >> 0) & 0xFF);
            g_bSendBuffer[len++] = (byte)((crc16 >> 8) & 0xFF);

            return len;
        }

        public int modbusWriteSingleCoil(int nUnitId, int nAddress, bool on)
        {
            UInt16 crc16;
            int len = 0;

            g_bSendBuffer[len++] = (byte)nUnitId;
            g_bSendBuffer[len++] = 0x05;
            g_bSendBuffer[len++] = (byte)((nAddress >> 8) & 0xff);
            g_bSendBuffer[len++] = (byte)((nAddress >> 0) & 0xff);

            if (on)
            {
                g_bSendBuffer[len++] = 0xFF;
                g_bSendBuffer[len++] = 0x00;
            }
            else
            {
                g_bSendBuffer[len++] = 0x00;
                g_bSendBuffer[len++] = 0x00;
            }

            crc16 = clsCrc16.crc16(g_bSendBuffer, len);

            g_bSendBuffer[len++] = (byte)((crc16 >> 0) & 0xFF);
            g_bSendBuffer[len++] = (byte)((crc16 >> 8) & 0xFF);

            return len;
        }

        public int modbusReadPacket(int nUnitId, int nFunction, int nAddress, int nReqLength)
        {
            UInt16 crc16;
            int len = 0;

            g_bSendBuffer[len++] = (byte)nUnitId;
            g_bSendBuffer[len++] = (byte)nFunction;
            g_bSendBuffer[len++] = (byte)((nAddress >> 8) & 0xFF);
            g_bSendBuffer[len++] = (byte)((nAddress >> 0) & 0xFF);
            g_bSendBuffer[len++] = (byte)((nReqLength >> 8) & 0xFF);
            g_bSendBuffer[len++] = (byte)((nReqLength >> 0) & 0xFF);

            crc16 = clsCrc16.crc16(g_bSendBuffer, len);

            g_bSendBuffer[len++] = (byte)((crc16 >> 0) & 0xFF);
            g_bSendBuffer[len++] = (byte)((crc16 >> 8) & 0xFF);

            return len;
        }
    }

    public class clsModbusMapAddrInfo
    {
        public clsModbusMapAddrInfo()
        {
        }

        public clsModbusMapAddrInfo(Int32 nFc, Int32 nAddr, Int32 nLen)
        {
            m_nFuctionCode = nFc;
            m_nStartAddr = nAddr;
            m_nLength = nLen;
        }

        public Int32 m_nFuctionCode = 0;
        public Int32 m_nStartAddr = 0;
        public Int32 m_nLength = 64;
    }
}
