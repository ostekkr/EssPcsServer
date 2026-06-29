using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EssPcsServer
{
    public class clsRtc
    {
        public byte YY;
        public byte MM;
        public byte DD;
        public byte hh;
        public byte mm;
        public byte ss;
        public byte DoW;
        public int ms;

        public DateTime ToDateTime()
        {
            DateTime dt;

            if((YY < 100) && (MM < 12) && (DD < 31) && (hh < 23) && (mm < 59) && (ss < 59))
            {
                dt = new DateTime(2000 + YY, MM, DD, hh, mm, ss);
            }
            else
            {
                dt = DateTime.Now;
            }
           
            return dt;
        }
    }

    public static class clsDateTime
    {
        private static readonly int YOONBASE = (4 * 365 + 1);	//	4년 윤년 포함
        private static readonly int BASEYEAR = 2000;

        private static readonly byte[] gcbDayOfMonNorm = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static readonly byte[] gcbDayOfMonYoon = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private static readonly int[] gcbSumDayOfMonNorm = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };
        private static readonly int[] gcbSumDayOfMonYoon = { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };
        private static readonly int[] gcbSumDaysOfYear = { 0, 366, 366 + 365, 366 + 365 + 365 };

        public static clsRtc fcLongToRtc(long lTime)
        {
            clsRtc stRtc = new clsRtc();
            int wDate;

            wDate = (int)(lTime / 86400);
            lTime -= (wDate * 86400);
            fcWordToDate(wDate, stRtc);

            stRtc.hh = (byte)(lTime / 3600);
            lTime -= (3600 * stRtc.hh);

            stRtc.mm = (byte)(lTime / 60);
            lTime -= 60 * stRtc.mm;
            stRtc.ss = (byte)lTime;

            stRtc.DoW = fcGregorianDayofWeek(stRtc.YY, stRtc.MM, stRtc.DD);

            return stRtc;
        }

        private static void fcWordToDate(int wDate, clsRtc stRtc)
        {
            int i;

            stRtc.YY = (byte)(wDate / YOONBASE);
            stRtc.YY *= 4;
            wDate = wDate % YOONBASE;

            //	  0 --> 0/1/1,  365 --> 0/12/31.
            //  366 --> 1/1/1,  730 --> 1/12/31.
            //  731 --> 2/1/1, 1095 --> 2/12/31.
            // 1096 --> 3/1/1, 1460 --> 3/12/31.

            if (wDate >= gcbSumDaysOfYear[3])
            {
                stRtc.YY += 3;
                wDate -= gcbSumDaysOfYear[3];
            }
            else if (wDate >= gcbSumDaysOfYear[2])
            {
                stRtc.YY += 2;
                wDate -= gcbSumDaysOfYear[2];
            }
            else if (wDate >= gcbSumDaysOfYear[1])
            {
                stRtc.YY += 1;
                wDate -= gcbSumDaysOfYear[1];
            }

            if ((stRtc.YY % 4) != 0)
            {	//평년.
                for (i = 1; i <= 12; i++)
                {
                    if (wDate < gcbSumDayOfMonNorm[i])
                    {
                        stRtc.MM = (byte)i;
                        stRtc.DD = (byte)(wDate + 1 - gcbSumDayOfMonNorm[i - 1]);
                        break;
                    }
                }
            }
            else
            {
                for (i = 1; i <= 12; i++)
                {
                    if (wDate < gcbSumDayOfMonYoon[i])
                    {
                        stRtc.MM = (byte)i;
                        stRtc.DD = (byte)(wDate + 1 - gcbSumDayOfMonYoon[i - 1]);
                        break;
                    }
                }
            }
        }

        private static byte fcGregorianDayofWeek(byte dY, byte dM, byte dD)
        {
            int dYY, dMM, dMtmp;

            dMtmp = (14 - (int)(dM & 0xFF)) / 12;

            dYY = BASEYEAR + (int)(dY & 0xFF) - dMtmp;
            dMM = dM + (12 * dMtmp) - 12;

            dMtmp = (int)(dD & 0xFF) + dYY;
            dMtmp += dYY / 4;
            dMtmp -= (dYY / 100);
            dMtmp += (dYY / 400);
            dMtmp += (31 * dMM / 12);
            dMtmp = dMtmp % 7;

            return (byte)dMtmp;
        }
    }
}
