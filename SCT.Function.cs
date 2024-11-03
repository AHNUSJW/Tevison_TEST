using System;
using System.Linq;

namespace TVS
{
    public partial class SCT
    {
        public SCT()
        {
            //
            e_sidh = 0;
            e_sidm = 0;
            e_sidl = 0;
            e_seri = 0;

            //
            temperature = 0;
            battery = 0;

            //
            mode = TmpMode.NULL;
            value = 0;
            delay = 0;
            date = 0;
            time = 0;
            run = 0;
            sum = 0;
            max = 0;
            min = 0;

            //
            for (int i = 0; i < SZ.CHA; i++)
            {
                T10[i] = 1000;
                A10[i] = 103626;
                V10[i] = 0.268528464f;
                T30[i] = 3000;
                A30[i] = 111074;
                V30[i] = 0.27214587f;
                T50[i] = 5000;
                A50[i] = 118423;
                V50[i] = 0.274951883f;
                T60[i] = 6000;
                A60[i] = 122060;
                V60[i] = 0.276701716f;
                T70[i] = 7000;
                A70[i] = 125674;
                V70[i] = 0.279485746f;
                T90[i] = 9000;
                A90[i] = 132830;
                V90[i] = 0.281848929f;
                T95[i] = 9500;
                A95[i] = 134604;
                V95[i] = 0.281848929f;
            }

            strend = true;
            oldrnd = 0;
            overmax = 0;
            overavg = 0;
            outdim = 0;
            outstm = 0;

            upsecond = 0;
            dnsecond = 0;
            mrect = RECT.NULL;

            isAxis = true;
            tmpMax = 10000;
            tmpMin = 2500;
        }

        public void vitoUpdate()
        {
            for (int i = 0; i < SZ.CHA; i++)
            {
                V10[i] = ((float)(T30[i] - T10[i])) / ((float)(A30[i] - A10[i]));
                V30[i] = ((float)(T50[i] - T30[i])) / ((float)(A50[i] - A30[i]));
                V50[i] = ((float)(T60[i] - T50[i])) / ((float)(A60[i] - A50[i]));
                V60[i] = ((float)(T70[i] - T60[i])) / ((float)(A70[i] - A60[i]));
                V70[i] = ((float)(T90[i] - T70[i])) / ((float)(A90[i] - A70[i]));
                V90[i] = ((float)(T95[i] - T90[i])) / ((float)(A95[i] - A90[i]));
                V95[i] = V90[i];
            }
        }

        public void memtimeUpdate()
        {
            //初始
            //memdate是启动记录的日期
            //memtime是启动记录的时间
            //memstop是启动记录的时间
            //memstart是启动记录的毫秒值
            //memrun是温度数据最后的毫秒值

            UInt32 hour = memtime / 10000;
            UInt32 minute = (memtime / 100) % 100;
            UInt32 second = memtime % 100;

            //真正的开始记录的时间
            //memtime = memtime + memstart

            second += (memstart / 1000);
            minute += second / 60;
            hour += minute / 60;

            second %= 60;
            minute %= 60;
            hour %= 24;

            memtime = (hour * 10000) + (minute * 100) + second;

            //真正的结束记录的时间
            //memstop = memrun - memstart

            second += ((memrun - memstart) / 1000);
            minute += second / 60;
            hour += minute / 60;

            second %= 60;
            minute %= 60;
            hour %= 24;

            memstop = (hour * 10000) + (minute * 100) + second;

            //
            if (memrun > memstart)
            {
                memrun = memrun - memstart;
                memstep = (uint)(memrun / myMem.Count);
            }
            else
            {
                memrun = 0;
            }

            //更新后
            //memdate是启动记录的日期
            //memtime是启动记录的真实时间
            //memstop是停止记录的真实时间
            //memstart是启动记录的毫秒值
            //memrun是总的记录毫秒值
        }

        //带2小数的温度整数转成string
        public string GetTemp(int da)
        {
            float f = da / 100.0f;

            return f.ToString("f2");
        }

        //带2小数的温度整数转成string
        public String GetTempWithSign(int tmp)
        {
            double a = tmp / 100.0f;

            if (tmp > 0)
            {
                return ("+" + a.ToString("F2"));
            }
            else
            {
                return a.ToString("F2");
            }
        }

        //求标准差
        public int getStdevp(int[] array)
        {
            int[] da = new int[SZ.CHA];
            int avg = (int)array.Average();
            int sum = 0;

            for (byte i = 0; i < SZ.CHA; i++)
            {
                da[i] = array[i] - avg;
                da[i] = da[i] * da[i];
                sum += da[i];
            }

            return ((int)Math.Sqrt(sum / SZ.CHA));
        }

        //求标准差
        public int getStdevpTVS8(int[] array)
        {
            int[] da = new int[SZ.CHA - 7];
            int avg = (int)array.Average();
            int sum = 0;

            for (byte i = 0; i < SZ.CHA - 7; i++)
            {
                da[i] = array[i] - avg;
                da[i] = da[i] * da[i];
                sum += da[i];
            }

            return ((int)Math.Sqrt(sum / (SZ.CHA - 7)));
        }

        //取10倍整
        public int getRound(int tmp)
        {
            //30
            //50
            //60
            //70
            //90
            //95
            if (tmp < 4000)
            {
                return 3000;
            }
            else if (tmp < 5500)
            {
                return 5000;
            }
            else if (tmp < 6500)
            {
                return 6000;
            }
            else if (tmp < 8000)
            {
                return 7000;
            }
            else if (tmp < 9250)
            {
                return 9000;
            }
            else
            {
                return 9500;
            }
        }

        //求方差
        public int getDeparture(int[] array)
        {
            int[] da = new int[SZ.CHA];
            int avg = (int)array.Average();
            int sum = 0;

            for (byte i = 0; i < SZ.CHA; i++)
            {
                da[i] = array[i] - mtp.outRnd;
                da[i] = da[i] * da[i];
                sum += da[i];
            }

            return ((int)Math.Sqrt(sum / SZ.CHA));
        }

        //求方差
        public int getDepartureTVS8(int[] array)
        {
            int[] da = new int[SZ.CHA - 7];
            int avg = (int)array.Average();
            int sum = 0;

            for (byte i = 0; i < SZ.CHA - 7; i++)
            {
                da[i] = array[i] - mtp.outRnd;
                da[i] = da[i] * da[i];
                sum += da[i];
            }

            return ((int)Math.Sqrt(sum / (SZ.CHA - 7)));
        }

        /*
        public void myHomUpdate()
        {
            //用12个数计算斜率

            //计算变化率,扩大100倍
            int mxy;
            int mxmy;
        
            if (myHom.Count <= 12) return;

            for (int i = 0; i < (myHom.Count - 11); i++)
            {
                for (int k = 0; k < SZ.CHA; k++)
                {
                    mxy = 0;
                    mxmy = 0;

                    //前后12个数求变化率
                    for (int s = 0; s < 12; s++)
                    {
                        mxy += myHom[i + s].OUT[k] * s;
                        mxmy += myHom[i + s].OUT[k];
                    }

                    //分子上是12和66，缩小6倍
                    mxy *= 2;
                    mxmy *= 11;

                    //分母是常数 n * sum(x^2) - sum(x) * sum(x) = 6072 - 4356 = 1716，缩小6倍，带2位小数
                    if (mxy >= mxmy)
                    {
                        myHom[i + 5].SOP[k] = ((mxy - mxmy) * 100 + 143) / 286;
                    }
                    else
                    {
                        myHom[i + 5].SOP[k] = ((mxy - mxmy) * 100 - 143) / 286;
                    }

                    //斜率已计算时间, (sop * 1000 / homstep)/100
                    myHom[i + 5].SOP[k] = (int)(myHom[i + 5].SOP[k] * 10 / homstep);
                }
            }

            //头尾的变化率
            for (int k = 0; k < SZ.CHA; k++)
            {
                myHom[0].SOP[k] = myHom[5].SOP[k];
                myHom[1].SOP[k] = myHom[5].SOP[k];
                myHom[2].SOP[k] = myHom[5].SOP[k];
                myHom[3].SOP[k] = myHom[5].SOP[k];
                myHom[4].SOP[k] = myHom[5].SOP[k];

                myHom[myHom.Count - 6].SOP[k] = myHom[myHom.Count - 7].SOP[k];
                myHom[myHom.Count - 5].SOP[k] = myHom[myHom.Count - 7].SOP[k];
                myHom[myHom.Count - 4].SOP[k] = myHom[myHom.Count - 7].SOP[k];
                myHom[myHom.Count - 3].SOP[k] = myHom[myHom.Count - 7].SOP[k];
                myHom[myHom.Count - 2].SOP[k] = myHom[myHom.Count - 7].SOP[k];
                myHom[myHom.Count - 1].SOP[k] = myHom[myHom.Count - 7].SOP[k];
            }

            //计算
            for (int i = 0; i < myHom.Count; i++)
            {
                myHom[i].sopMax = myHom[i].SOP.Max();
                myHom[i].sopMin = myHom[i].SOP.Min();
                myHom[i].sopAvg = (int)myHom[i].SOP.Average();
                myHom[i].outMax = myHom[i].OUT.Max();
                myHom[i].outMin = myHom[i].OUT.Min();
                myHom[i].outDif = myHom[i].outMax - myHom[i].outMin;
                myHom[i].outAvg = (int)myHom[i].OUT.Average();
                myHom[i].outStd = getStdevp(myHom[i].OUT);
                myHom[i].outRnd = getRound(myHom[i].outAvg);
                myHom[i].outDep = getDeparture(myHom[i].OUT);
            }
        }
        */
        public void myHomUpdate()
        {
            //用30个数计算斜率

            //计算变化率,扩大100倍
            int mxy;
            int mxmy;

            if (myHom.Count <= 30) return;

            for (int i = 0; i < (myHom.Count - 29); i++)
            {
                for (int k = 0; k < SZ.CHA; k++)
                {
                    mxy = 0;
                    mxmy = 0;

                    //前后30个数求变化率
                    for (int s = 0; s < 30; s++)
                    {
                        mxy += myHom[i + s].OUT[k] * s;
                        mxmy += myHom[i + s].OUT[k];
                    }

                    //分子公式(mxy * n) + (mxmy * sum(x))
                    //分子上的30和435
                    mxy *= 30;
                    mxmy *= 435;

                    //n = 30
                    //x = 0~29
                    //n * sum(x^2) - sum(x) * sum(x) = 256650 - 189225 = 67425
                    //分母是常数, 67425
                    //带2位小数,分子*100
                    if (mxy >= mxmy)
                    {
                        myHom[i + 5].SOP[k] = ((mxy - mxmy) * 100 + 33713) / 67425;
                    }
                    else
                    {
                        myHom[i + 5].SOP[k] = ((mxy - mxmy) * 100 - 33713) / 67425;
                    }

                    //斜率已计算时间, (sop * 1000 / homstep)/100
                    myHom[i + 5].SOP[k] = (int)(myHom[i + 5].SOP[k] * 10 / homstep);
                }
            }

            //头尾的变化率
            for (int k = 0; k < SZ.CHA; k++)
            {
                myHom[0].SOP[k] = myHom[18].SOP[k];
                myHom[1].SOP[k] = myHom[18].SOP[k];
                myHom[2].SOP[k] = myHom[18].SOP[k];
                myHom[3].SOP[k] = myHom[18].SOP[k];
                myHom[4].SOP[k] = myHom[18].SOP[k];
                myHom[5].SOP[k] = myHom[18].SOP[k];
                myHom[6].SOP[k] = myHom[18].SOP[k];
                myHom[7].SOP[k] = myHom[18].SOP[k];
                myHom[8].SOP[k] = myHom[18].SOP[k];
                myHom[9].SOP[k] = myHom[18].SOP[k];
                myHom[10].SOP[k] = myHom[18].SOP[k];
                myHom[11].SOP[k] = myHom[18].SOP[k];
                myHom[12].SOP[k] = myHom[18].SOP[k];
                myHom[13].SOP[k] = myHom[18].SOP[k];
                myHom[14].SOP[k] = myHom[18].SOP[k];
                myHom[15].SOP[k] = myHom[18].SOP[k];
                myHom[16].SOP[k] = myHom[18].SOP[k];
                myHom[17].SOP[k] = myHom[18].SOP[k];

                myHom[myHom.Count - 1].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 2].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 3].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 4].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 5].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 6].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 7].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 8].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 9].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 10].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 11].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 12].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 13].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 14].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 15].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 16].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 17].SOP[k] = myHom[myHom.Count - 19].SOP[k];
                myHom[myHom.Count - 18].SOP[k] = myHom[myHom.Count - 19].SOP[k];
            }

            //计算
            if (MyDefine.myXET.myType == TYPE.TVS8 || MyDefine.myXET.meType == TYPE.TVS8)
            {
                for (int i = 0; i < myHom.Count; i++)
                {
                    myHom[i].sopMax = myHom[i].SOP.Max();
                    myHom[i].sopMin = myHom[i].SOP.Min();
                    myHom[i].sopAvg = (int)myHom[i].SOP.Average();

                    int[] OutTVS8 = new int[8];
                    Array.Copy(myHom[i].OUT, OutTVS8, 8);
                    myHom[i].outMax = OutTVS8.Max();
                    myHom[i].outMin = OutTVS8.Min();
                    myHom[i].outDif = myHom[i].outMax - myHom[i].outMin;
                    myHom[i].outAvg = (int)OutTVS8.Average();
                    myHom[i].outStd = getStdevpTVS8(OutTVS8);
                    myHom[i].outRnd = getRound(myHom[i].outAvg);
                    myHom[i].outDep = getDepartureTVS8(OutTVS8);
                }
            }
            else
            {
                for (int i = 0; i < myHom.Count; i++)
                {
                    myHom[i].sopMax = myHom[i].SOP.Max();
                    myHom[i].sopMin = myHom[i].SOP.Min();
                    myHom[i].sopAvg = (int)myHom[i].SOP.Average();
                    myHom[i].outMax = myHom[i].OUT.Max();
                    myHom[i].outMin = myHom[i].OUT.Min();
                    myHom[i].outDif = myHom[i].outMax - myHom[i].outMin;
                    myHom[i].outAvg = (int)myHom[i].OUT.Average();
                    myHom[i].outStd = getStdevp(myHom[i].OUT);
                    myHom[i].outRnd = getRound(myHom[i].outAvg);
                    myHom[i].outDep = getDeparture(myHom[i].OUT);
                }
            }
        }

        public void myHomSpan(int start, int end)
        {
            if (start < 0) start = 0;
            if (end >= myHom.Count) end = myHom.Count - 1;
            if (end < start) end = start;

            spanSopMax = myHom[start].sopMax;
            spanSopMin = myHom[start].sopMin;
            spanOutMax = myHom[start].outMax;
            spanOutMin = myHom[start].outMin;
            spanDifMax = myHom[start].outDif;

            if (end == start) return;

            for (int i = start; i <= end; i++)
            {
                if (myHom[i].sopMax > spanSopMax) spanSopMax = myHom[i].sopMax;
                if (myHom[i].sopMin < spanSopMax) spanSopMin = myHom[i].sopMin;
                if (myHom[i].outMax > spanSopMax) spanOutMax = myHom[i].outMax;
                if (myHom[i].outMin < spanSopMax) spanOutMin = myHom[i].outMin;
                if (myHom[i].outDif > spanSopMax) spanDifMax = myHom[i].outDif;
            }
        }

        public TYPE myType
        {
            get
            {
                switch (e_type)
                {
                    default:
                    case 15:
                        return TYPE.TVS15;
                    case 8:
                        return TYPE.TVS8;
                }
            }
        }

        public TYPE meType
        {
            get
            {
                switch (type)
                {
                    default:
                    case 15:
                        return TYPE.TVS15;
                    case 8:
                        return TYPE.TVS8;
                }
            }
        }
    }
}

//end
