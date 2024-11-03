using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVS
{
    public partial class MenuReportForm : Form
    {
        String source;      //温度数据来源
        List<TMP> myRPT;    //温度数据列表
        String rptFileName; //温度数据来源文件名称
        String rptdate;     //温度数据的原始记录时间
        String rpttime;     //温度数据的开始时间(时分秒)
        String rptstop;     //温度数据的结束时间(时分秒)
        String rptrun;      //温度数据的总长时间(秒)

        //
        public MenuReportForm()
        {
            InitializeComponent();
        }

        //更新listbox
        private void GetListBox()
        {
            if ((MyDefine.myXET.memFileName != null) && (MyDefine.myXET.myMem.Count > 0))
            {
                listBox1.Items.Add("设备导出数据 " + Path.GetFileNameWithoutExtension(MyDefine.myXET.memFileName));
            }

            if ((MyDefine.myXET.synFileName != null) && (MyDefine.myXET.mySyn.Count > 0))
            {
                listBox1.Items.Add("实时记录数据 " + Path.GetFileNameWithoutExtension(MyDefine.myXET.synFileName));
            }

            if ((MyDefine.myXET.homFileName != null) && (MyDefine.myXET.myHom.Count > 0))
            {
                listBox1.Items.Add("文件载入数据 " + Path.GetFileNameWithoutExtension(MyDefine.myXET.homFileName));
            }

            if (listBox1.Items.Count > 0)
            {
                listBox1.SelectedIndex = 0;
            }
        }

        //更新combox
        private void GetCombox()
        {
            while (comboBox1.Items.Count < MyDefine.myXET.myPRM.Count)
            {
                comboBox1.Items.Add("");
            }

            for (int i = 0; i < MyDefine.myXET.myPRM.Count; i++)
            {
                comboBox1.Items[i] = "#" + (i + 1).ToString() + "  " + MyDefine.myXET.myPRM[i].name;
            }

            while (comboBox1.Items.Count > MyDefine.myXET.myPRM.Count)
            {
                comboBox1.Items.RemoveAt(comboBox1.Items.Count - 1);
            }

            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        //保存记录
        private void SaveUserInfo()
        {
            String mePath = MyDefine.myXET.userCFG + @"\user." + MyDefine.myXET.userName + ".ifo";
            if (File.Exists(mePath))
            {
                System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
            }
            FileStream meFS = new FileStream(mePath, FileMode.Create, FileAccess.Write);
            TextWriter meWrite = new StreamWriter(meFS);
            if (textBox1.TextLength > 0)
            {
                meWrite.WriteLine("machineType=" + textBox1.Text);
            }
            if (textBox2.TextLength > 0)
            {
                meWrite.WriteLine("machineSN=" + textBox2.Text);
            }
            if (textBox3.TextLength > 0)
            {
                meWrite.WriteLine("machineDescription=" + textBox3.Text);
            }
            if (textBox4.TextLength > 0)
            {
                meWrite.WriteLine("reportCompany=" + textBox4.Text);
            }
            if (textBox5.TextLength > 0)
            {
                meWrite.WriteLine("reportStaff=" + textBox5.Text);
            }
            if (textBox6.TextLength > 0)
            {
                meWrite.WriteLine("reportDepartment=" + textBox6.Text);
            }
            if (textBox7.TextLength > 0)
            {
                meWrite.WriteLine("reportNumber=" + textBox7.Text);
            }
            if (textBox8.TextLength > 0)
            {
                meWrite.WriteLine("reportDate=" + textBox8.Text);
            }
            if (textBox9.TextLength > 0)
            {
                meWrite.WriteLine("reportName=" + textBox9.Text);
            }
            meWrite.Close();
            meFS.Close();
            System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
        }

        //读取记录
        private void GetUserInfo()
        {
            String mePath = MyDefine.myXET.userCFG + @"\user." + MyDefine.myXET.userName + ".ifo";

            if (File.Exists(mePath))
            {
                String[] meLines = File.ReadAllLines(mePath);

                foreach (String line in meLines)
                {
                    switch (line.Substring(0, line.IndexOf('=')))
                    {
                        case "machineType": textBox1.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "machineSN": textBox2.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "machineDescription": textBox3.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportCompany": textBox4.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportStaff": textBox5.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportDepartment": textBox6.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportNumber": /*textBox7.Text = line.Substring(line.IndexOf('=') + 1)*/; break;
                        case "reportDate": /*textBox8.Text = line.Substring(line.IndexOf('=') + 1)*/; break;
                        case "reportName": textBox9.Text = line.Substring(line.IndexOf('=') + 1); break;
                        default: break;
                    }
                }
            }

            textBox7.Text = "JD" + ((System.DateTime.Now.Ticks - 621355968000000000) / 10000000L).ToString();
            textBox8.Text = System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
        }

        //选择温度程序
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP.Count;
        }

        //创建段落
        private Paragraph CreateParagraph(string str, iTextSharp.text.Font font, int align)
        {
            Paragraph mp = new Paragraph(str, font);

            //Element.ALIGN_LEFT
            //Element.ALIGN_CENTER
            //Element.ALIGN_RIGHT
            mp.Alignment = align;

            //
            mp.SpacingBefore = 5.0f;
            mp.SpacingAfter = 5.0f;

            return mp;
        }

        //字符串长度增加add值并控制等长,最小min
        private int GetJoinLen(List<String> str, int min, int add)
        {
            int max = min;
            int len = 0;

            for (int i = 0; i < str.Count; i++)
            {
                len = Encoding.Default.GetBytes(str[i]).Length;
                if (len > max)
                {
                    max = len;
                }
            }

            max += add;

            return max;
        }

        //差值并取正
        private int Devation(int a, int b)
        {
            if (a < b)
            {
                return (b - a);
            }
            else
            {
                return (a - b);
            }
        }

        //从org点开始寻找start温度到stop温度的转折点
        private int GetStep(int org, int start, int stop)
        {
            int tmhalf;             //一半温度
            int half;               //一半温度处
            int sopmax;             //最高变化率
            int sopdiv;             //十分之一变化率
            int min_to_target = 50;  //恒温起始点的偏差

            if (start > 90)
            {
                min_to_target = 80; //0.8度
            }
            else if (start > 70)
            {
                min_to_target = 60; //0.6度
            }
            else
            {
                min_to_target = 50; //0.5度
            }

            //带2小数点
            start = start * 100;
            stop = stop * 100;

            //初始化
            tmhalf = (start + stop) / 2;
            half = org;
            sopmax = myRPT[org].sopAvg;
            sopdiv = myRPT[org].sopAvg / 10;

            //升温
            if (start < stop)
            {
                //找一半温度处
                for (int i = org; i < myRPT.Count; i++)
                {
                    if ((Devation(myRPT[i].outAvg, tmhalf) < 200) && (myRPT[i].sopAvg > 0))
                    {
                        half = i;
                        sopmax = myRPT[i].sopAvg;
                        if (sopmax >= 20)
                        {
                            sopdiv = sopmax / 10;
                        }
                        else
                        {
                            sopdiv = 1;
                        }
                        break;
                    }
                }

                //反向寻找
                for (int i = half; i > org; i--)
                {
                    //温度接近
                    if (Devation(myRPT[i].outAvg, start) < min_to_target)
                    {
                        return i;
                    }

                    //斜率接近
                    if (myRPT[i].sopAvg < sopdiv)
                    {
                        return i;
                    }

                    //斜率更新
                    if (myRPT[i].sopAvg > sopmax)
                    {
                        sopmax = myRPT[i].sopAvg;
                        if (sopmax >= 20)
                        {
                            sopdiv = sopmax / 10;
                        }
                        else
                        {
                            sopdiv = 1;
                        }
                    }
                }
            }
            //降温
            else
            {
                //找一半温度处
                for (int i = org; i < myRPT.Count; i++)
                {
                    if ((Devation(myRPT[i].outAvg, tmhalf) < 200) && (myRPT[i].sopAvg < 0))
                    {
                        half = i;
                        sopmax = myRPT[i].sopAvg;
                        if (sopmax <= -20)
                        {
                            sopdiv = sopmax / 10;
                        }
                        else
                        {
                            sopdiv = -1;
                        }
                        break;
                    }
                }

                //反向寻找
                for (int i = half; i > org; i--)
                {
                    //温度接近
                    if (Devation(myRPT[i].outAvg, start) < min_to_target)
                    {
                        return i;
                    }

                    //斜率接近
                    if (myRPT[i].sopAvg > sopdiv)
                    {
                        return i;
                    }

                    //斜率更新
                    if (myRPT[i].sopAvg < sopmax)
                    {
                        sopmax = myRPT[i].sopAvg;
                        if (sopmax <= -20)
                        {
                            sopdiv = sopmax / 10;
                        }
                        else
                        {
                            sopdiv = -1;
                        }
                    }
                }
            }

            return org;
        }

        //从begin到end之间反向找温度temp的离开点
        private int GetFirstStart(int begin, int end, int idx30sec, int temp)
        {
            if (begin >= myRPT.Count) begin = myRPT.Count - 1;
            if (end >= myRPT.Count) end = myRPT.Count - 1;

            //
            int leave = myRPT[end].outAvg;

            //带2小数点
            temp = temp * 100;

            //防溢出
            if (idx30sec < begin)
            {
                idx30sec = begin;
            }

            //穿过温度点
            if (leave > temp)
            {
                leave = temp - (leave - temp);

                //寻找
                for (int i = idx30sec; i > begin; i--)
                {
                    if (myRPT[i].outAvg < leave)
                    {
                        return i;
                    }
                }
            }
            else
            {
                leave = temp + (temp - leave);

                //寻找
                for (int i = idx30sec; i > begin; i--)
                {
                    if (myRPT[i].outAvg > leave)
                    {
                        return i;
                    }
                }
            }

            return begin;
        }

        //从begin到end之间找到经过温度temp的点
        private int GetUpThrough(int begin, int end, int temp)
        {
            //带2小数点
            temp = temp * 100;

            //
            for (int i = begin; i < end; i++)
            {
                if (myRPT[i].outAvg > temp)
                {
                    return i;
                }
            }

            return begin;
        }

        //从begin到end之间找到经过温度temp的点
        private int GetDownThrough(int begin, int end, int temp)
        {
            //带2小数点
            temp = temp * 100;

            //
            for (int i = begin; i < end; i++)
            {
                if (myRPT[i].outAvg < temp)
                {
                    return i;
                }
            }

            return begin;
        }

        //找最大升温速率
        private int GetHeatRate(int begin, int end)
        {
            int rate = 0;

            for (int i = begin; i < end; i++)
            {
                if (myRPT[i].sopAvg > rate)
                {
                    rate = myRPT[i].sopAvg;
                }
            }

            return (rate / 100);
        }

        //找最大降温速率
        private int GetCoolRate(int begin, int end)
        {
            int rate = 0;

            for (int i = begin; i < end; i++)
            {
                if (myRPT[i].sopAvg < rate)
                {
                    rate = myRPT[i].sopAvg;
                }
            }

            return ((-rate) / 100);
        }

        //从begin到end之间找到持续温度temp的起始点
        private int GetConstStart(int begin, int end, int temp)
        {
            //恒温起始点的偏差
            int max_to_target = Devation(myRPT[begin].outAvg, myRPT[end].outAvg) / 2;

            //带2小数点
            temp = temp * 100;

            //升温
            if (myRPT[begin].outAvg < myRPT[end].outAvg)
            {
                //正向寻找
                for (int i = begin; i < end; i++)
                {
                    //温度穿过
                    if (myRPT[i].outAvg > temp)
                    {
                        return i;
                    }

                    //变化率为0
                    if ((myRPT[i].sopAvg <= 0) && (Devation(myRPT[i].outAvg, temp) < max_to_target))
                    {
                        return i;
                    }
                }
            }
            //降温
            else
            {
                //正向寻找
                for (int i = begin; i < end; i++)
                {
                    //温度穿过
                    if (myRPT[i].outAvg < temp)
                    {
                        return i;
                    }

                    //变化率为0
                    if ((myRPT[i].sopAvg >= 0) && (Devation(myRPT[i].outAvg, temp) < max_to_target))
                    {
                        return i;
                    }
                }
            }

            return begin;
        }

        //从begin到end之间找到持续温度temp的结束点
        private int GetConstStop(int begin, int end, int temp)
        {
            //恒温起始点的偏差
            int min_to_target;

            if (temp > 90)
            {
                min_to_target = 80; //0.8度
            }
            else if (temp > 70)
            {
                min_to_target = 60; //0.6度
            }
            else
            {
                min_to_target = 50; //0.5度
            }

            //带2小数点
            temp = temp * 100;

            //反向寻找
            for (int i = end; i > begin; i--)
            {
                //温度穿过
                if (Devation(myRPT[i].outAvg, temp) < min_to_target)
                {
                    return i;
                }

                //变化率为0
                if (Devation(myRPT[i].sopAvg, 0) < 50)
                {
                    return i;
                }
            }

            return begin;
        }

        //从begin到end之间找到持续温度temp的离开点
        private int GetConstLeave(int begin, int end, int idx30sec, int temp)
        {
            //变化率
            int tmpmax = myRPT[begin].outAvg;
            int tmpmin = myRPT[begin].outAvg;

            //变化率
            int sopmax = myRPT[begin].sopAvg;
            int sopmin = myRPT[begin].sopAvg;

            //恒温起始点的偏差
            int min_to_target;

            if (temp > 90)
            {
                min_to_target = 80; //0.8度
            }
            else if (temp > 70)
            {
                min_to_target = 60; //0.6度
            }
            else
            {
                min_to_target = 50; //0.5度
            }

            //带2小数点
            temp = temp * 100;

            //防溢出
            if (idx30sec >= end)
            {
                idx30sec = end;
            }

            //找极值
            for (int i = begin; i < idx30sec; i++)
            {
                if (tmpmax < myRPT[i].outAvg)
                {
                    tmpmax = myRPT[i].outAvg;
                }

                if (tmpmin > myRPT[i].outAvg)
                {
                    tmpmin = myRPT[i].outAvg;
                }

                if (sopmax < myRPT[i].sopAvg)
                {
                    sopmax = myRPT[i].sopAvg;
                }

                if (sopmin > myRPT[i].sopAvg)
                {
                    sopmin = myRPT[i].sopAvg;
                }
            }

            //温度范围
            if (tmpmax > temp)
            {
                tmpmax = tmpmax + min_to_target;
            }
            else
            {
                tmpmax = temp + min_to_target;
            }

            if (tmpmin < temp)
            {
                tmpmin = tmpmin - min_to_target;
            }
            else
            {
                tmpmin = temp - min_to_target;
            }

            //两倍变化率
            sopmax = sopmax * 2;
            sopmin = sopmin * 2;

            //寻找
            for (int i = begin; i < end; i++)
            {
                //超过最大值
                if (myRPT[i].outAvg > tmpmax)
                {
                    return i;
                }

                //超过最小值
                if (myRPT[i].outAvg < tmpmin)
                {
                    return i;
                }

                //超过最大值
                if (myRPT[i].sopAvg > sopmax)
                {
                    return i;
                }

                //超过最小值
                if (myRPT[i].sopAvg < sopmin)
                {
                    return i;
                }
            }

            return end;
        }

        //最大过冲
        private int GetOverMax(int begin, int end, bool rise)
        {
            int tmp = myRPT[begin].OUT[0];

            for (int i = begin; i < end; i++)
            {
                for (int k = 0; k < SZ.CHA; k++)
                {
                    if (rise)
                    {
                        if (myRPT[i].OUT[k] > tmp)
                        {
                            tmp = myRPT[i].OUT[k];
                        }
                    }
                    else
                    {
                        if (myRPT[i].OUT[k] < tmp)
                        {
                            tmp = myRPT[i].OUT[k];
                        }
                    }
                }
            }

            return tmp;
        }

        //平均过冲
        private int GetOverAvg(int begin, int end, bool rise)
        {
            int tmp = myRPT[begin].outAvg;

            for (int i = begin; i < end; i++)
            {
                if (rise)
                {
                    if (myRPT[i].outAvg > tmp)
                    {
                        tmp = myRPT[i].outAvg;
                    }
                }
                else
                {
                    if (myRPT[i].outAvg < tmp)
                    {
                        tmp = myRPT[i].outAvg;
                    }
                }
            }

            return tmp;
        }

        //带2小数的温度整数转成string
        private String GetTemp(int tmp)
        {
            double a = tmp / 100.0f;

            return a.ToString("F2");
        }

        //带2小数的温度整数转成string
        private String GetTempWithSign(int tmp)
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

        //生成报告
        private void CreateReport()
        {
            #region 参数

            //使用
            const int LENL = 24;
            const int LENS = 8;
            const int LENW = 4;
            int page = 0;
            int idx;
            int myMax = 0;
            int myTmp = 0;
            String blankLine = " ";
            String mmStr = "";
            List<String> myLS = new List<String>();

            //基本信息
            String machineType = textBox1.Text;
            String machineSN = textBox2.Text;
            String machineDescription = textBox3.Text;
            String reportCompany = textBox4.Text;
            String reportStaff = textBox5.Text;
            String reportDepartment = textBox6.Text;
            String reportNumber = textBox7.Text;
            String reportDate = textBox8.Text;
            String reportName = textBox9.Text;

            #endregion

            #region 温度数据

            //数据信息
            int hole_per_second;

            if (listBox1.SelectedItem.ToString().Contains("设备"))
            {
                source = "设备导出数据";
                myRPT = MyDefine.myXET.myMem;
                rptFileName = MyDefine.myXET.memFileName;
                rptdate = MyDefine.myXET.memdate.ToString();
                rpttime = MyDefine.myXET.memtime.ToString();
                rptstop = MyDefine.myXET.memstop.ToString();
                rptrun = MyDefine.myXET.memrun.ToString();
            }
            else if (listBox1.SelectedItem.ToString().Contains("实时"))
            {
                source = "实时记录数据";
                myRPT = MyDefine.myXET.mySyn;
                rptFileName = MyDefine.myXET.synFileName;
                rptdate = MyDefine.myXET.syndate.ToString();
                rpttime = MyDefine.myXET.syntime.ToString();
                rptstop = MyDefine.myXET.synstop.ToString();
                rptrun = MyDefine.myXET.synrun.ToString();
            }
            else if (listBox1.SelectedItem.ToString().Contains("文件"))
            {
                source = "文件载入数据";
                myRPT = MyDefine.myXET.myHom;
                rptFileName = MyDefine.myXET.homFileName;
                rptdate = MyDefine.myXET.homdate.ToString();
                rpttime = MyDefine.myXET.homtime.ToString();
                rptstop = MyDefine.myXET.homstop.ToString();
                rptrun = MyDefine.myXET.homrun.ToString();
            }
            else
            {
                return;
            }
            rptdate = ("20" + rptdate).Insert(4, "-").Insert(7, "-");
            if (rpttime.Length == 5)
            {
                rpttime = rpttime.Insert(1, ":").Insert(4, ":");
            }
            else if (rpttime.Length == 6)
            {
                rpttime = rpttime.Insert(2, ":").Insert(5, ":");
            }
            if (rptstop.Length == 5)
            {
                rptstop = rptstop.Insert(1, ":").Insert(4, ":");
            }
            else if (rptstop.Length == 6)
            {
                rptstop = rptstop.Insert(2, ":").Insert(5, ":");
            }
            rptrun = (Convert.ToInt32(rptrun) / 1000).ToString();
            hole_per_second = myRPT.Count / Convert.ToInt32(rptrun);

            #endregion

            #region 分析数据

            //时间长度
            int totaltime = 0;
            for (int i = 0; i < MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP.Count; i++)
            {
                totaltime += MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[i].time;
            }
            if (Convert.ToInt32(rptrun) < totaltime)
            {
                MessageBox.Show("温度数据" + rptrun.ToString() + "秒少于温度控制程序时间" + totaltime.ToString() + "秒");
                return;
            }

            //计算变化率,扩大100倍
            int mxy;
            int mxmy;
            for (int i = 0; i < (myRPT.Count - 11); i++)
            {
                for (int k = 0; k < SZ.CHA; k++)
                {
                    mxy = 0;
                    mxmy = 0;

                    //前后12个数求变化率
                    for (int s = 0; s < 12; s++)
                    {
                        mxy += myRPT[i + s].OUT[k] * s;
                        mxmy += myRPT[i + s].OUT[k];
                    }

                    //分子上是12和66，缩小6倍
                    mxy *= 2;
                    mxmy *= 11;

                    //分母是常数 n * sum(x^2) -sum(x) * sum(x) = 6072 - 4356 = 1716，缩小6倍，带2位小数
                    if (mxy >= mxmy)
                    {
                        myRPT[i + 5].SOP[k] = ((mxy - mxmy) * 100 + 143) / 286;
                    }
                    else
                    {
                        myRPT[i + 5].SOP[k] = ((mxy - mxmy) * 100 - 143) / 286;
                    }

                    //斜率没有计算时间
                }
            }

            //头尾的变化率
            for (int k = 0; k < SZ.CHA; k++)
            {
                myRPT[0].SOP[k] = myRPT[5].SOP[k];
                myRPT[1].SOP[k] = myRPT[5].SOP[k];
                myRPT[2].SOP[k] = myRPT[5].SOP[k];
                myRPT[3].SOP[k] = myRPT[5].SOP[k];
                myRPT[4].SOP[k] = myRPT[5].SOP[k];

                myRPT[myRPT.Count - 6].SOP[k] = myRPT[myRPT.Count - 7].SOP[k];
                myRPT[myRPT.Count - 5].SOP[k] = myRPT[myRPT.Count - 7].SOP[k];
                myRPT[myRPT.Count - 4].SOP[k] = myRPT[myRPT.Count - 7].SOP[k];
                myRPT[myRPT.Count - 3].SOP[k] = myRPT[myRPT.Count - 7].SOP[k];
                myRPT[myRPT.Count - 2].SOP[k] = myRPT[myRPT.Count - 7].SOP[k];
                myRPT[myRPT.Count - 1].SOP[k] = myRPT[myRPT.Count - 7].SOP[k];
            }

            //计算平均数
            for (int i = 0; i < myRPT.Count; i++)
            {
                myRPT[i].outAvg = (int)myRPT[i].OUT.Average();
                myRPT[i].sopAvg = (int)myRPT[i].SOP.Average();
            }

            #endregion

            #region 报告文件

            //保存报告路径和文件名
            if (!Directory.Exists(MyDefine.myXET.userOUT))
            {
                Directory.CreateDirectory(MyDefine.myXET.userOUT);
            }
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "新建文件(*.pdf)|*.pdf";
            fileDialog.RestoreDirectory = true;
            fileDialog.InitialDirectory = MyDefine.myXET.userOUT;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(fileDialog.FileName))
                {
                    File.Delete(fileDialog.FileName);
                }
            }
            else
            {
                return;
            }

            //创建新文档对象,页边距(X,X,Y,Y)
            Document document = new Document(PageSize.A4, 48, 16, 16, 16);

            //路径设置; FileMode.Create文档不在会创建，存在会覆盖
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(fileDialog.FileName, FileMode.Create));

            //添加信息
            document.AddTitle("PCR校准报告");
            document.AddAuthor(reportCompany + " " + reportDepartment + " " + reportStaff);
            document.AddSubject(machineType + " 温度测量报告");
            document.AddKeywords("PCR");
            document.AddCreator(reportStaff);

            //创建字体，STSONG.TTF空格不等宽
            iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(BaseFont.CreateFont(@"c:\windows\fonts\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 14.0f, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fontItem = new iTextSharp.text.Font(BaseFont.CreateFont(@"c:\windows\fonts\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 10.0f, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fontContent = new iTextSharp.text.Font(BaseFont.CreateFont(@"c:\windows\fonts\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 10.0f, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fontMessage = new iTextSharp.text.Font(BaseFont.CreateFont(@"c:\windows\fonts\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 8.0f, iTextSharp.text.Font.NORMAL);

            #endregion

            #region 第一页

            //打开
            document.Open();

            //第一页添加元素
            document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(CreateParagraph(reportName + " Result", fontTitle, Element.ALIGN_CENTER));
            document.Add(CreateParagraph("（" + reportName + " 报告）", fontItem, Element.ALIGN_CENTER));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("General information".PadRight(LENL, ' '));
            myLS.Add("Report number".PadRight(LENL, ' '));
            myLS.Add("Report date".PadRight(LENL, ' '));
            myLS.Add("Test date".PadRight(LENL, ' '));
            myLS.Add("Test start time".PadRight(LENL, ' '));
            myLS.Add("Test end time".PadRight(LENL, ' '));
            myLS.Add("Test program".PadRight(LENL, ' '));
            myLS.Add("Report version".PadRight(LENL, ' '));
            myLS.Add("Source".PadRight(LENL, ' '));
            myLS.Add("Record".PadRight(LENL, ' '));
            myLS[1] = myLS[1] + ":  " + reportNumber;
            myLS[2] = myLS[2] + ":  " + reportDate;
            myLS[3] = myLS[3] + ":  " + rptdate;
            myLS[4] = myLS[4] + ":  " + rpttime;
            myLS[5] = myLS[5] + ":  " + rptstop;
            myLS[6] = myLS[6] + ":  " + MyDefine.myXET.myPRM[comboBox1.SelectedIndex].name;
            myLS[7] = myLS[7] + ":  " + "0.0.1";
            myLS[8] = myLS[8] + ":  " + source;
            myLS[9] = myLS[9] + ":  " + Path.GetFileName(rptFileName);
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(检测信息)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "(报告编号)";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "(报告日期)";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "(检测日期)";
            myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "(检测开始时间)";
            myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "(检测结束时间)";
            myLS[6] = myLS[6].PadRight(myMax + myLS[6].Length - Encoding.Default.GetBytes(myLS[6]).Length, ' ') + "(检测温度程序)";
            myLS[7] = myLS[7].PadRight(myMax + myLS[7].Length - Encoding.Default.GetBytes(myLS[7]).Length, ' ') + "(报告版本)";
            myLS[8] = myLS[8].PadRight(myMax + myLS[8].Length - Encoding.Default.GetBytes(myLS[8]).Length, ' ') + "(数据源)";
            myLS[9] = myLS[9].PadRight(myMax + myLS[9].Length - Encoding.Default.GetBytes(myLS[9]).Length, ' ') + "(文件)";
            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(myLS[1], fontContent));
            document.Add(new Paragraph(myLS[2], fontContent));
            document.Add(new Paragraph(myLS[3], fontContent));
            document.Add(new Paragraph(myLS[4], fontContent));
            document.Add(new Paragraph(myLS[5], fontContent));
            document.Add(new Paragraph(myLS[6], fontContent));
            document.Add(new Paragraph(myLS[7], fontContent));
            document.Add(new Paragraph(myLS[8], fontContent));
            document.Add(new Paragraph(myLS[9], fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Device information".PadRight(LENL, ' '));
            myLS.Add("Device model".PadRight(LENL, ' '));
            myLS.Add("Device serial number".PadRight(LENL, ' '));
            myLS.Add("Device description".PadRight(LENL, ' '));
            myLS[1] = myLS[1] + ":  " + machineType;
            myLS[2] = myLS[2] + ":  " + machineSN;
            myLS[3] = myLS[3] + ":  " + machineDescription;
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(仪器信息)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "(仪器型号)";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "(仪器序列号)";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "(仪器描述)";
            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(myLS[1], fontContent));
            document.Add(new Paragraph(myLS[2], fontContent));
            document.Add(new Paragraph(myLS[3], fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Jingdu TVS information".PadRight(LENL, ' '));
            myLS.Add("TVS universal number".PadRight(LENL, ' '));
            myLS.Add("TVS serial number".PadRight(LENL, ' '));
            myLS.Add("TVS software version".PadRight(LENL, ' '));
            myLS.Add("TVS calibration date".PadRight(LENL, ' '));
            myLS[1] = myLS[1] + ":  " + MyDefine.myXET.e_sidm.ToString("X8") + MyDefine.myXET.e_sidl.ToString("X8") + MyDefine.myXET.e_sidh.ToString("X8");
            myLS[2] = myLS[2] + ":  " + MyDefine.myXET.e_seri.ToString();
            myLS[3] = myLS[3] + ":  " + "0.0.1";
            myLS[4] = myLS[4] + ":  " + ("20" + MyDefine.myXET.e_seri.ToString().Substring(0, 6)).Insert(4, "-").Insert(7, "-");
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(采集器信息)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "(采集器唯一序列号)";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "(采集器批号)";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "(采集器软件版本)";
            myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "(采集器工厂标定日期)";
            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(myLS[1], fontContent));
            document.Add(new Paragraph(myLS[2], fontContent));
            document.Add(new Paragraph(myLS[3], fontContent));
            document.Add(new Paragraph(myLS[4], fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("User information".PadRight(LENL, ' '));
            myLS.Add("Name".PadRight(LENL, ' '));
            myLS.Add("Department".PadRight(LENL, ' '));
            myLS.Add("Institute".PadRight(LENL, ' '));
            myLS[1] = myLS[1] + ":  " + reportStaff;
            myLS[2] = myLS[2] + ":  " + reportDepartment;
            myLS[3] = myLS[3] + ":  " + reportCompany;
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(检测人员信息)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "(姓名)";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "(部门)";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "(公司)";
            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(myLS[1], fontContent));
            document.Add(new Paragraph(myLS[2], fontContent));
            document.Add(new Paragraph(myLS[3], fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////

            #endregion

            #region 第二页

            //更新页数和创建新页
            document.NewPage();
            document.ResetPageCount();

            //第二页添加元素
            document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Protocol information".PadRight(LENL, ' '));
            for (int i = 0; i < MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP.Count; i++)
            {
                myLS.Add(("Temperature step " + (i + 1).ToString()).PadRight(LENL, ' '));

                myLS[i + 1] = myLS[i + 1] + ":  " + (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[i].temperature / 100f).ToString() + ".00 ℃  for    " + MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[i].time.ToString() + " seconds";
            }
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(检测程序)";
            for (int i = 0; i < MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP.Count; i++)
            {
                myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + "(步骤" + (i + 1).ToString() + ")";
            }
            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            for (int i = 0; i < MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP.Count; i++)
            {
                document.Add(new Paragraph(myLS[i + 1], fontContent));
            }
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Probe information".PadRight(LENL, ' '));
            for (int i = 0; i < SZ.CHA; i++)
            {
                //myRPT.Count
                myLS.Add(("Temperature probe " + (i + 1).ToString()).PadRight(LENL, ' '));

                myLS[i + 1] = myLS[i + 1] + ":  " + myRPT.Count.ToString() + " samples ,   " + rptrun.ToString() + " seconds total";
            }
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(温度探头取样清单)";
            for (int i = 0; i < SZ.CHA; i++)
            {
                myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + "(探头" + (i + 1).ToString() + ")";
            }
            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            for (int i = 0; i < SZ.CHA; i++)
            {
                document.Add(new Paragraph(myLS[i + 1], fontContent));
            }
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Legend        (备注)", fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("This legend can be used in combination with your individual temperature results.", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("        √√   Measured value is better than specificatioon", fontContent));
            document.Add(new Paragraph("        √     Measured value meets specificatioon", fontContent));
            document.Add(new Paragraph("        X      Measured value does NOT meet specificatioon", fontContent));
            document.Add(new Paragraph("        ?      No specificatioons found", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("        √√   优于市场标准范围", fontContent));
            document.Add(new Paragraph("        √     在市场标准范围内", fontContent));
            document.Add(new Paragraph("        X      超出市场标准范围", fontContent));
            document.Add(new Paragraph("        ?      没有标准可参考", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////

            #endregion

            #region 第三页

            //更新页数和创建新页
            document.NewPage();
            document.ResetPageCount();

            //第三页添加元素
            document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Probe layout (96v - 15)        (15根探头在采集器中的分布位置)", fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("          1  2  3  4  5  6  7  8  9  10 11 12", fontContent));
            document.Add(new Paragraph("        A ● ○ ○ ● ○ ○ ● ○ ○ ● ○ ●", fontContent));
            document.Add(new Paragraph("        B ○ ○ ○ ○ ○ ○ ○ ○ ○ ○ ○ ○", fontContent));
            document.Add(new Paragraph("        C ○ ○ ○ ○ ○ ○ ○ ○ ○ ○ ○ ○", fontContent));
            document.Add(new Paragraph("        D ● ○ ○ ○ ○ ○ ● ○ ○ ○ ○ ●", fontContent));
            document.Add(new Paragraph("        E ○ ○ ○ ● ○ ○ ○ ○ ○ ● ○ ○", fontContent));
            document.Add(new Paragraph("        F ○ ○ ○ ○ ○ ○ ○ ○ ○ ○ ○ ○", fontContent));
            document.Add(new Paragraph("        G ○ ○ ○ ○ ○ ○ ○ ○ ○ ○ ○ ○", fontContent));
            document.Add(new Paragraph("        H ● ○ ○ ● ○ ○ ● ○ ○ ● ○ ●", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("        Probe 1     (探头1)         : A1", fontContent));
            document.Add(new Paragraph("        Probe 2     (探头2)         : A4", fontContent));
            document.Add(new Paragraph("        Probe 3     (探头3)         : A7", fontContent));
            document.Add(new Paragraph("        Probe 4     (探头4)         : A10", fontContent));
            document.Add(new Paragraph("        Probe 5     (探头5)         : A12", fontContent));
            document.Add(new Paragraph("        Probe 6     (探头6)         : D1", fontContent));
            document.Add(new Paragraph("        Probe 7     (探头7)         : D7", fontContent));
            document.Add(new Paragraph("        Probe 8     (探头8)         : D12", fontContent));
            document.Add(new Paragraph("        Probe 9     (探头9)         : E4", fontContent));
            document.Add(new Paragraph("        Probe 10    (探头10)        : E10", fontContent));
            document.Add(new Paragraph("        Probe 11    (探头11)        : H1", fontContent));
            document.Add(new Paragraph("        Probe 12    (探头12)        : H4", fontContent));
            document.Add(new Paragraph("        Probe 13    (探头13)        : H7", fontContent));
            document.Add(new Paragraph("        Probe 14    (探头14)        : H10", fontContent));
            document.Add(new Paragraph("        Probe 15    (探头15)        : H12", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Recommanded control positions", fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("Positive :        (正控制位置)", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("        [min]", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("Negative :        (负控制位置)", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("        [max]", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////

            #endregion

            if (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP.Count >= 8)
            {
                #region 计算参数

                int step_start = 0;  //起始设定温度索引
                int step_target = 0; //目标设定温度索引
                int step_next = 1;   //下个目标温度索引

                int begin;       //设定温度的起始
                int end;         //设定温度的结束

                bool rise;       //升降温标记

                int temp_max;    //找最大值
                int temp_min;    //找最小值
                int mark_max;    //记录探头位置
                int mark_min;    //记录探头位置

                int slope_start; //速率计算起始的点
                int slope_stop;  //速率计算结束的点
                int slope_tmp;   //升降温差的值
                int slope_get;   //升降温速率的值

                int const_start; //恒温起始的点
                int const_stop;  //恒温结束的点

                int over_max;    //最大过冲的值
                int over_avg;    //平均过冲的值

                int idx_30sec;   //30秒处的的点
                int idx_time;    //等间时间的值
                int idx_first;   //等间隔处的点
                int idx_second;  //等间隔处的点
                int idx_third;   //等间隔处的点
                int idx_fourth;  //等间隔处的点
                int idx_fifth;   //等间隔处的点

                int spec_dev;    //标准参考值.与目标值偏差
                int spec_rate;   //标准参考值.速率
                int spec_over;   //标准参考值.超调
                int spec_max;    //标准参考值.均匀度(孔间最大差)

                #endregion

                #region 第一点计算

                int org_temp = myRPT[0].outAvg / 100; //起始点不带小数的温度

                //step 1-2
                begin = 0;
                end = GetStep(begin, MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature, MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_next].temperature);

                //升降温
                if (org_temp < MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature)
                {
                    rise = true;
                }
                else
                {
                    rise = false;
                }

                //高温
                if (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature > 90)
                {
                    spec_dev = 80;    //0.8℃
                    spec_rate = 150;  //1.5℃
                    spec_over = 600;  //6.0℃
                    spec_max = 150;   //1.5℃
                }
                else if (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature > 70)
                {
                    spec_dev = 60;
                    spec_rate = 150;
                    spec_over = 500;
                    spec_max = 150;
                }
                else
                {
                    spec_dev = 50;
                    spec_rate = 150;
                    spec_over = 500;
                    spec_max = 100;
                }

                //速率
                if ((org_temp + 15) < MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature)
                {
                    //升温
                    slope_start = GetUpThrough(begin, end, (org_temp + 5));
                    slope_stop = GetUpThrough(begin, end, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature - 5));
                    slope_tmp = MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature - org_temp - 10;
                    slope_get = GetHeatRate(slope_start, slope_stop);
                }
                else if (org_temp > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature + 15))
                {
                    //降温
                    slope_start = GetDownThrough(begin, end, (org_temp - 5));
                    slope_stop = GetDownThrough(begin, end, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature + 5));
                    slope_tmp = org_temp - MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature - 10;
                    slope_get = GetCoolRate(slope_start, slope_stop);
                }
                else
                {
                    slope_start = 0;
                    slope_stop = 0;
                    slope_tmp = 0;
                    slope_get = 0;
                }

                //恒定温度计算
                const_start = GetFirstStart(begin, end, (end - (30 * hole_per_second)), MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature);
                const_start = GetConstStart(const_start, end, MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature);
                const_stop = GetConstStop(const_start, end, MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature);

                //过冲
                over_max = GetOverMax(begin, const_stop, rise);
                over_avg = GetOverAvg(begin, const_stop, rise);

                //30秒处
                idx_30sec = const_start + (30 * hole_per_second);

                //间隔时间
                idx_time = MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].time / 4;
                idx_first = 0;
                idx_second = idx_first + idx_time;
                idx_third = idx_second + idx_time;
                idx_fourth = idx_third + idx_time;
                idx_fifth = idx_fourth + idx_time;

                //间隔处
                idx_first = const_start + (idx_first * hole_per_second);
                idx_second = const_start + (idx_second * hole_per_second);
                idx_third = const_start + (idx_third * hole_per_second);
                idx_fourth = const_start + (idx_fourth * hole_per_second);
                idx_fifth = const_start + (idx_fifth * hole_per_second);

                #endregion

                #region 第一点报告

                //更新页数和创建新页
                document.NewPage();
                document.ResetPageCount();

                //第n页添加元素
                document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                mmStr = (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature / 100f).ToString();
                document.Add(new Paragraph("Temperature        :  " + mmStr + ".00 ℃        (" + mmStr + "℃时的温度表现)", fontContent));
                document.Add(new Paragraph(blankLine, fontItem));
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                document.Add(new Paragraph("Values after 30 seconds        (" + mmStr + "℃第30秒时15孔的温度数据)", fontItem));
                document.Add(new Paragraph(blankLine, fontContent));
                ////////////////////////////////////////////////////////////////////
                myLS.Clear();
                myLS.Add("".PadRight(LENL, ' '));
                myLS.Add("Probe 1 (A1)".PadRight(LENL, ' '));
                myLS.Add("Probe 2 (A4)".PadRight(LENL, ' '));
                myLS.Add("Probe 3 (A7)".PadRight(LENL, ' '));
                myLS.Add("Probe 4 (A10)".PadRight(LENL, ' '));
                myLS.Add("Probe 5 (A12)".PadRight(LENL, ' '));
                myLS.Add("Probe 6 (D1)".PadRight(LENL, ' '));
                myLS.Add("Probe 7 (D7)".PadRight(LENL, ' '));
                myLS.Add("Probe 8 (D12)".PadRight(LENL, ' '));
                myLS.Add("Probe 9 (E4)".PadRight(LENL, ' '));
                myLS.Add("Probe 10 (E10)".PadRight(LENL, ' '));
                myLS.Add("Probe 11 (H1)".PadRight(LENL, ' '));
                myLS.Add("Probe 12 (H4)".PadRight(LENL, ' '));
                myLS.Add("Probe 13 (H7)".PadRight(LENL, ' '));
                myLS.Add("Probe 14 (H10)".PadRight(LENL, ' '));
                myLS.Add("Probe 15 (H12)".PadRight(LENL, ' '));
                myLS[0] = myLS[0] + "Measured";
                for (int i = 0; i < SZ.CHA; i++)
                {
                    myLS[i + 1] = myLS[i + 1] + GetTemp(myRPT[idx_30sec].OUT[i]) + " ℃";
                }
                myMax = GetJoinLen(myLS, LENL, LENS);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "T(meas-set)";
                temp_max = myRPT[idx_30sec].OUT[0];
                temp_min = myRPT[idx_30sec].OUT[0];
                mark_max = 1;
                mark_min = 1;
                for (int i = 0; i < SZ.CHA; i++)
                {
                    myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + GetTemp(myRPT[idx_30sec].OUT[i] - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + " ℃";
                    if (myRPT[idx_30sec].OUT[i] > temp_max)
                    {
                        temp_max = myRPT[idx_30sec].OUT[i];
                        mark_max = i + 1;
                    }
                    if (myRPT[idx_30sec].OUT[i] < temp_min)
                    {
                        temp_min = myRPT[idx_30sec].OUT[i];
                        mark_min = i + 1;
                    }
                }
                myLS[mark_max] = myLS[mark_max] + " (max)";
                myLS[mark_min] = myLS[mark_min] + " (min)";
                myMax = GetJoinLen(myLS, LENL, LENS);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Status";
                for (int i = 0; i < SZ.CHA; i++)
                {
                    myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + "Active";
                }
                document.Add(new Paragraph(myLS[0], fontContent));
                for (int i = 0; i < SZ.CHA; i++)
                {
                    document.Add(new Paragraph(myLS[i + 1], fontContent));
                }
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                document.Add(new Paragraph("Step results        (" + mmStr + "℃温度控制性能)", fontItem));
                document.Add(new Paragraph(blankLine, fontContent));
                ////////////////////////////////////////////////////////////////////
                myLS.Clear();
                myLS.Add("Item (项目)");
                if (rise)
                {
                    if (slope_get > 0)
                    {
                        myLS.Add("Max. heat rate (最大升温速率)");
                        myLS.Add("Avg. heat rate (平均升温速率)");
                    }
                    myLS.Add("Max. overshoot (最大过冲)");
                    myLS.Add("Avg. overshoot (平均过冲)");
                    myLS.Add("Heat time      (升温时间)");
                    myLS.Add("Hold time      (恒温时间)");
                }
                else
                {
                    if (slope_get > 0)
                    {
                        myLS.Add("Max. cool rate (最大降温速率)");
                        myLS.Add("Avg. cool rate (平均降温速率)");
                    }
                    myLS.Add("Max. overshoot (最大过冲)");
                    myLS.Add("Avg. overshoot (平均过冲)");
                    myLS.Add("Cool time      (降温时间)");
                    myLS.Add("Hold time      (恒温时间)");
                }
                myMax = GetJoinLen(myLS, LENL, LENS);
                idx = 0;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Measured (测量值)";
                if (slope_get > 0)
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((slope_get * hole_per_second) / 100.0f).ToString("F2") + " ℃/sec";
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (((slope_tmp * 100 * hole_per_second) / (slope_stop - slope_start)) / 100.0f).ToString("F2") + " ℃/sec";
                }
                if (rise)
                {
                    if (over_max > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + GetTemp(over_max) + " ℃";
                        myLS[idx] = myLS[idx] + " (+" + GetTemp(over_max - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                    }
                }
                else
                {
                    if (over_max < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + GetTemp(over_max) + " ℃";
                        myLS[idx] = myLS[idx] + " (" + GetTemp(over_max - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                    }
                }
                if (rise)
                {
                    if (over_avg > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + GetTemp(over_avg) + " ℃";
                        myLS[idx] = myLS[idx] + " (+" + GetTemp(over_avg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                    }
                }
                else
                {
                    if (over_avg < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + GetTemp(over_avg) + " ℃";
                        myLS[idx] = myLS[idx] + " (" + GetTemp(over_avg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                    }
                }
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((const_start - begin) / hole_per_second).ToString() + " sec";
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((const_stop - const_start) / hole_per_second).ToString() + " sec";
                myMax = GetJoinLen(myLS, LENL, LENS);
                idx = 0;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Specification (参考)";
                if (slope_get > 0)
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_rate / 100.0f).ToString("F1") + " ℃/sec";
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_rate / 100.0f).ToString("F1") + " ℃/sec";
                }
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_over / 100.0f).ToString("F1") + " ℃";
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_over / 100.0f).ToString("F1") + " ℃";
                myMax = GetJoinLen(myLS, LENL, LENW);
                idx = 0;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Result";
                if (slope_get > 0)
                {
                    //
                    myTmp = slope_get * hole_per_second;
                    if (myTmp > (spec_rate * 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp > spec_rate)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }

                    //
                    myTmp = (slope_tmp * 100 * hole_per_second) / (slope_stop - slope_start);
                    if (myTmp > (spec_rate * 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp > spec_rate)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }
                }
                if (rise)
                {
                    if (over_max > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        myTmp = Devation(over_max, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));

                        if (myTmp < (spec_over / 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp < spec_over)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
                else
                {
                    if (over_max < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        myTmp = Devation(over_max, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));

                        if (myTmp < (spec_over / 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp < spec_over)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
                if (rise)
                {
                    if (over_avg > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        myTmp = Devation(over_avg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));

                        if (myTmp < (spec_over / 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp < spec_over)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
                else
                {
                    if (over_avg < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        myTmp = Devation(over_avg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));

                        if (myTmp < (spec_over / 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp < spec_over)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
                document.Add(new Paragraph(myLS[0], fontContent));
                for (int i = 1; i < myLS.Count; i++)
                {
                    document.Add(new Paragraph(myLS[i], fontContent));
                }
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                document.Add(new Paragraph("Accuracy results        (" + mmStr + "℃温度控准确度)", fontItem));
                document.Add(new Paragraph(blankLine, fontContent));
                ////////////////////////////////////////////////////////////////////
                myLS.Clear();
                myLS.Add("Time (时间)".PadRight((LENL - 2), ' '));
                myLS.Add(("0 sec").PadRight(LENL, ' '));
                myLS.Add((idx_time.ToString() + " sec").PadRight(LENL, ' '));
                myLS.Add(((idx_time * 2).ToString() + " sec").PadRight(LENL, ' '));
                myLS.Add(((idx_time * 3).ToString() + " sec").PadRight(LENL, ' '));
                myLS.Add(((idx_time * 4).ToString() + " sec").PadRight(LENL, ' '));
                myLS[0] = myLS[0] + "Measured (测量值)";
                myLS[1] = myLS[1] + GetTemp(myRPT[idx_first].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_first].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                myLS[2] = myLS[2] + GetTemp(myRPT[idx_second].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_second].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                myLS[3] = myLS[3] + GetTemp(myRPT[idx_third].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_third].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                myLS[4] = myLS[4] + GetTemp(myRPT[idx_fourth].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_fourth].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                myLS[5] = myLS[5] + GetTemp(myRPT[idx_fifth].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_fifth].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                myMax = GetJoinLen(myLS, LENL, LENS);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Specification (参考)";
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myMax = GetJoinLen(myLS, LENL, LENW);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Result";
                myTmp = Devation(myRPT[idx_first].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√";
                }
                else
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "X";
                }
                myTmp = Devation(myRPT[idx_second].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√";
                }
                else
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "X";
                }
                myTmp = Devation(myRPT[idx_third].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√";
                }
                else
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "X";
                }
                myTmp = Devation(myRPT[idx_fourth].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√";
                }
                else
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "X";
                }
                myTmp = Devation(myRPT[idx_fifth].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√";
                }
                else
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "X";
                }
                document.Add(new Paragraph(myLS[0], fontContent));
                for (int i = 1; i < myLS.Count; i++)
                {
                    document.Add(new Paragraph(myLS[i], fontContent));
                }
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                document.Add(new Paragraph("Accuracy results        (" + mmStr + "℃温度控Block均匀性)", fontItem));
                document.Add(new Paragraph(blankLine, fontContent));
                ////////////////////////////////////////////////////////////////////
                myLS.Clear();
                myLS.Add("Time (时间)".PadRight((LENL - 2), ' '));
                myLS.Add(("0 sec").PadRight(LENL, ' '));
                myLS.Add((idx_time.ToString() + " sec").PadRight(LENL, ' '));
                myLS.Add(((idx_time * 2).ToString() + " sec").PadRight(LENL, ' '));
                myLS.Add(((idx_time * 3).ToString() + " sec").PadRight(LENL, ' '));
                myLS.Add(((idx_time * 4).ToString() + " sec").PadRight(LENL, ' '));
                myLS[0] = myLS[0] + "Measured (测量值)";
                myLS[1] = myLS[1] + GetTemp(myRPT[idx_first].OUT.Max() - myRPT[idx_first].OUT.Min()) + " ℃";
                myLS[2] = myLS[2] + GetTemp(myRPT[idx_second].OUT.Max() - myRPT[idx_second].OUT.Min()) + " ℃";
                myLS[3] = myLS[3] + GetTemp(myRPT[idx_third].OUT.Max() - myRPT[idx_third].OUT.Min()) + " ℃";
                myLS[4] = myLS[4] + GetTemp(myRPT[idx_fourth].OUT.Max() - myRPT[idx_fourth].OUT.Min()) + " ℃";
                myLS[5] = myLS[5] + GetTemp(myRPT[idx_fifth].OUT.Max() - myRPT[idx_fifth].OUT.Min()) + " ℃";
                myMax = GetJoinLen(myLS, LENL, LENS);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Specification (参考)";
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myMax = GetJoinLen(myLS, LENL, LENW);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Result";
                //zhoup
                myTmp = myRPT[idx_first].OUT.Max() - myRPT[idx_first].OUT.Min();
                if (myTmp < (spec_max / 2))
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_max)
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√";
                }
                else
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "X";
                }
                myTmp = myRPT[idx_second].OUT.Max() - myRPT[idx_second].OUT.Min();
                if (myTmp < (spec_max / 2))
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_max)
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√";
                }
                else
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "X";
                }
                myTmp = myRPT[idx_third].OUT.Max() - myRPT[idx_third].OUT.Min();
                if (myTmp < (spec_max / 2))
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_max)
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√";
                }
                else
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "X";
                }
                myTmp = myRPT[idx_fourth].OUT.Max() - myRPT[idx_fourth].OUT.Min();
                if (myTmp < (spec_max / 2))
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_max)
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√";
                }
                else
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "X";
                }
                myTmp = myRPT[idx_fifth].OUT.Max() - myRPT[idx_fifth].OUT.Min();
                if (myTmp < (spec_max / 2))
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_max)
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√";
                }
                else
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "X";
                }
                document.Add(new Paragraph(myLS[0], fontContent));
                for (int i = 1; i < myLS.Count; i++)
                {
                    document.Add(new Paragraph(myLS[i], fontContent));
                }
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////

                #endregion

                step_start = 0;
                step_target = 1;
                step_next = 2;

                for (int step = 0; step < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP.Count - 2); step++)
                {
                    #region 第n页计算

                    //step next
                    begin = end;
                    end = GetStep(begin, MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature, MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_next].temperature);

                    //升降温
                    if (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature < MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature)
                    {
                        rise = true;
                    }
                    else
                    {
                        rise = false;
                    }

                    //高温
                    if (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature > 90)
                    {
                        spec_dev = 80;    //0.8℃
                        spec_rate = 150;  //1.5℃
                        spec_over = 600;  //6.0℃
                        spec_max = 150;   //1.5℃
                    }
                    else if (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature > 70)
                    {
                        spec_dev = 60;
                        spec_rate = 150;
                        spec_over = 500;
                        spec_max = 150;
                    }
                    else
                    {
                        spec_dev = 50;
                        spec_rate = 150;
                        spec_over = 500;
                        spec_max = 100;
                    }

                    //速率
                    if ((MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature + 15) < MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature)
                    {
                        //升温
                        slope_start = GetUpThrough(begin, end, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature + 5));
                        slope_stop = GetUpThrough(begin, end, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature - 5));
                        slope_tmp = MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature - MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature - 10;
                        slope_get = GetHeatRate(slope_start, slope_stop);
                    }
                    else if (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature + 15))
                    {
                        //降温
                        slope_start = GetDownThrough(begin, end, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature - 5));
                        slope_stop = GetDownThrough(begin, end, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature + 5));
                        slope_tmp = MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature - MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature - 10;
                        slope_get = GetCoolRate(slope_start, slope_stop);
                    }
                    else
                    {
                        slope_start = 0;
                        slope_stop = 0;
                        slope_tmp = 0;
                        slope_get = 0;
                    }

                    //恒定温度计算
                    const_start = GetConstStart(begin, end, MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature);
                    const_stop = GetConstStop(begin, end, MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature);

                    //过冲
                    over_max = GetOverMax(begin, const_stop, rise);
                    over_avg = GetOverAvg(begin, const_stop, rise);

                    //30秒处
                    idx_30sec = const_start + (30 * hole_per_second);

                    //间隔时间
                    idx_time = MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].time / 4;
                    idx_first = 0;
                    idx_second = idx_first + idx_time;
                    idx_third = idx_second + idx_time;
                    idx_fourth = idx_third + idx_time;
                    idx_fifth = idx_fourth + idx_time;

                    //间隔处
                    idx_first = const_start + (idx_first * hole_per_second);
                    idx_second = const_start + (idx_second * hole_per_second);
                    idx_third = const_start + (idx_third * hole_per_second);
                    idx_fourth = const_start + (idx_fourth * hole_per_second);
                    idx_fifth = const_start + (idx_fifth * hole_per_second);

                    #endregion

                    #region 第n页

                    //更新页数和创建新页
                    document.NewPage();
                    document.ResetPageCount();

                    //第n页添加元素
                    document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
                    document.Add(new Paragraph(blankLine, fontItem));
                    ////////////////////////////////////////////////////////////////////
                    mmStr = (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature / 100f).ToString();
                    document.Add(new Paragraph("Temperature        :  " + mmStr + ".00 ℃        (" + mmStr + "℃时的温度表现)", fontContent));
                    document.Add(new Paragraph(blankLine, fontItem));
                    document.Add(new Paragraph(blankLine, fontItem));
                    ////////////////////////////////////////////////////////////////////
                    document.Add(new Paragraph("Values after 30 seconds        (" + mmStr + "℃第30秒时15孔的温度数据)", fontItem));
                    document.Add(new Paragraph(blankLine, fontContent));
                    ////////////////////////////////////////////////////////////////////
                    myLS.Clear();
                    myLS.Add("".PadRight(LENL, ' '));
                    myLS.Add("Probe 1 (A1)".PadRight(LENL, ' '));
                    myLS.Add("Probe 2 (A4)".PadRight(LENL, ' '));
                    myLS.Add("Probe 3 (A7)".PadRight(LENL, ' '));
                    myLS.Add("Probe 4 (A10)".PadRight(LENL, ' '));
                    myLS.Add("Probe 5 (A12)".PadRight(LENL, ' '));
                    myLS.Add("Probe 6 (D1)".PadRight(LENL, ' '));
                    myLS.Add("Probe 7 (D7)".PadRight(LENL, ' '));
                    myLS.Add("Probe 8 (D12)".PadRight(LENL, ' '));
                    myLS.Add("Probe 9 (E4)".PadRight(LENL, ' '));
                    myLS.Add("Probe 10 (E10)".PadRight(LENL, ' '));
                    myLS.Add("Probe 11 (H1)".PadRight(LENL, ' '));
                    myLS.Add("Probe 12 (H4)".PadRight(LENL, ' '));
                    myLS.Add("Probe 13 (H7)".PadRight(LENL, ' '));
                    myLS.Add("Probe 14 (H10)".PadRight(LENL, ' '));
                    myLS.Add("Probe 15 (H12)".PadRight(LENL, ' '));
                    myLS[0] = myLS[0] + "Measured";
                    for (int i = 0; i < SZ.CHA; i++)
                    {
                        myLS[i + 1] = myLS[i + 1] + GetTemp(myRPT[idx_30sec].OUT[i]) + " ℃";
                    }
                    myMax = GetJoinLen(myLS, LENL, LENS);
                    myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "T(meas-set)";
                    temp_max = myRPT[idx_30sec].OUT[0];
                    temp_min = myRPT[idx_30sec].OUT[0];
                    mark_max = 1;
                    mark_min = 1;
                    for (int i = 0; i < SZ.CHA; i++)
                    {
                        myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + GetTemp(myRPT[idx_30sec].OUT[i] - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + " ℃";
                        if (myRPT[idx_30sec].OUT[i] > temp_max)
                        {
                            temp_max = myRPT[idx_30sec].OUT[i];
                            mark_max = i + 1;
                        }
                        if (myRPT[idx_30sec].OUT[i] < temp_min)
                        {
                            temp_min = myRPT[idx_30sec].OUT[i];
                            mark_min = i + 1;
                        }
                    }
                    myLS[mark_max] = myLS[mark_max] + " (max)";
                    myLS[mark_min] = myLS[mark_min] + " (min)";
                    myMax = GetJoinLen(myLS, LENL, LENS);
                    myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Status";
                    for (int i = 0; i < SZ.CHA; i++)
                    {
                        myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + "Active";
                    }
                    document.Add(new Paragraph(myLS[0], fontContent));
                    for (int i = 0; i < SZ.CHA; i++)
                    {
                        document.Add(new Paragraph(myLS[i + 1], fontContent));
                    }
                    document.Add(new Paragraph(blankLine, fontItem));
                    ////////////////////////////////////////////////////////////////////
                    document.Add(new Paragraph("Step results        (" + mmStr + "℃温度控制性能)", fontItem));
                    document.Add(new Paragraph(blankLine, fontContent));
                    ////////////////////////////////////////////////////////////////////
                    myLS.Clear();
                    myLS.Add("Item (项目)");
                    if (rise)
                    {
                        if (slope_get > 0)
                        {
                            myLS.Add("Max. heat rate (最大升温速率)");
                            myLS.Add("Avg. heat rate (平均升温速率)");
                        }
                        myLS.Add("Max. overshoot (最大过冲)");
                        myLS.Add("Avg. overshoot (平均过冲)");
                        myLS.Add("Heat time      (升温时间)");
                        myLS.Add("Hold time      (恒温时间)");
                    }
                    else
                    {
                        if (slope_get > 0)
                        {
                            myLS.Add("Max. cool rate (最大降温速率)");
                            myLS.Add("Avg. cool rate (平均降温速率)");
                        }
                        myLS.Add("Max. overshoot (最大过冲)");
                        myLS.Add("Avg. overshoot (平均过冲)");
                        myLS.Add("Cool time      (降温时间)");
                        myLS.Add("Hold time      (恒温时间)");
                    }
                    myMax = GetJoinLen(myLS, LENL, LENS);
                    idx = 0;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Measured (测量值)";
                    if (slope_get > 0)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((slope_get * hole_per_second) / 100.0f).ToString("F2") + " ℃/sec";
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (((slope_tmp * 100 * hole_per_second) / (slope_stop - slope_start)) / 100.0f).ToString("F2") + " ℃/sec";
                    }
                    if (rise)
                    {
                        if (over_max > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + GetTemp(over_max) + " ℃";
                            myLS[idx] = myLS[idx] + " (+" + GetTemp(over_max - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                        }
                    }
                    else
                    {
                        if (over_max < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + GetTemp(over_max) + " ℃";
                            myLS[idx] = myLS[idx] + " (" + GetTemp(over_max - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                        }
                    }
                    if (rise)
                    {
                        if (over_avg > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + GetTemp(over_avg) + " ℃";
                            myLS[idx] = myLS[idx] + " (+" + GetTemp(over_avg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                        }
                    }
                    else
                    {
                        if (over_avg < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + GetTemp(over_avg) + " ℃";
                            myLS[idx] = myLS[idx] + " (" + GetTemp(over_avg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                        }
                    }
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((const_start - begin) / hole_per_second).ToString() + " sec";
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((const_stop - const_start) / hole_per_second).ToString() + " sec";
                    myMax = GetJoinLen(myLS, LENL, LENS);
                    idx = 0;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Specification (参考)";
                    if (slope_get > 0)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_rate / 100.0f).ToString("F1") + " ℃/sec";
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_rate / 100.0f).ToString("F1") + " ℃/sec";
                    }
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_over / 100.0f).ToString("F1") + " ℃";
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_over / 100.0f).ToString("F1") + " ℃";
                    myMax = GetJoinLen(myLS, LENL, LENW);
                    idx = 0;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Result";
                    if (slope_get > 0)
                    {
                        //
                        myTmp = slope_get * hole_per_second;
                        if (myTmp > (spec_rate * 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp > spec_rate)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }

                        //
                        myTmp = (slope_tmp * 100 * hole_per_second) / (slope_stop - slope_start);
                        if (myTmp > (spec_rate * 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp > spec_rate)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    if (rise)
                    {
                        if (over_max > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                        {
                            myTmp = Devation(over_max, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));

                            if (myTmp < (spec_over / 2))
                            {
                                idx++;
                                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                            }
                            else if (myTmp < spec_over)
                            {
                                idx++;
                                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                            }
                            else
                            {
                                idx++;
                                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                            }
                        }
                        else
                        {
                            idx++;
                        }
                    }
                    else
                    {
                        if (over_max < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                        {
                            myTmp = Devation(over_max, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));

                            if (myTmp < (spec_over / 2))
                            {
                                idx++;
                                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                            }
                            else if (myTmp < spec_over)
                            {
                                idx++;
                                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                            }
                            else
                            {
                                idx++;
                                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                            }
                        }
                        else
                        {
                            idx++;
                        }
                    }
                    if (rise)
                    {
                        if (over_avg > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                        {
                            myTmp = Devation(over_avg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));

                            if (myTmp < (spec_over / 2))
                            {
                                idx++;
                                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                            }
                            else if (myTmp < spec_over)
                            {
                                idx++;
                                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                            }
                            else
                            {
                                idx++;
                                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                            }
                        }
                        else
                        {
                            idx++;
                        }
                    }
                    else
                    {
                        if (over_avg < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                        {
                            myTmp = Devation(over_avg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));

                            if (myTmp < (spec_over / 2))
                            {
                                idx++;
                                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                            }
                            else if (myTmp < spec_over)
                            {
                                idx++;
                                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                            }
                            else
                            {
                                idx++;
                                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                            }
                        }
                        else
                        {
                            idx++;
                        }
                    }
                    document.Add(new Paragraph(myLS[0], fontContent));
                    for (int i = 1; i < myLS.Count; i++)
                    {
                        document.Add(new Paragraph(myLS[i], fontContent));
                    }
                    document.Add(new Paragraph(blankLine, fontItem));
                    ////////////////////////////////////////////////////////////////////
                    document.Add(new Paragraph("Accuracy results        (" + mmStr + "℃温度控准确度)", fontItem));
                    document.Add(new Paragraph(blankLine, fontContent));
                    ////////////////////////////////////////////////////////////////////
                    myLS.Clear();
                    myLS.Add("Time (时间)".PadRight((LENL - 2), ' '));
                    myLS.Add(("0 sec").PadRight(LENL, ' '));
                    myLS.Add((idx_time.ToString() + " sec").PadRight(LENL, ' '));
                    myLS.Add(((idx_time * 2).ToString() + " sec").PadRight(LENL, ' '));
                    myLS.Add(((idx_time * 3).ToString() + " sec").PadRight(LENL, ' '));
                    myLS.Add(((idx_time * 4).ToString() + " sec").PadRight(LENL, ' '));
                    myLS[0] = myLS[0] + "Measured (测量值)";
                    myLS[1] = myLS[1] + GetTemp(myRPT[idx_first].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_first].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    myLS[2] = myLS[2] + GetTemp(myRPT[idx_second].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_second].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    myLS[3] = myLS[3] + GetTemp(myRPT[idx_third].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_third].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    myLS[4] = myLS[4] + GetTemp(myRPT[idx_fourth].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_fourth].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    myLS[5] = myLS[5] + GetTemp(myRPT[idx_fifth].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_fifth].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    myMax = GetJoinLen(myLS, LENL, LENS);
                    myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Specification (参考)";
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                    myMax = GetJoinLen(myLS, LENL, LENW);
                    myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Result";
                    myTmp = Devation(myRPT[idx_first].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                    if (myTmp < (spec_dev / 2))
                    {
                        myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_dev)
                    {
                        myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√";
                    }
                    else
                    {
                        myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "X";
                    }
                    myTmp = Devation(myRPT[idx_second].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                    if (myTmp < (spec_dev / 2))
                    {
                        myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_dev)
                    {
                        myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√";
                    }
                    else
                    {
                        myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "X";
                    }
                    myTmp = Devation(myRPT[idx_third].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                    if (myTmp < (spec_dev / 2))
                    {
                        myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_dev)
                    {
                        myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√";
                    }
                    else
                    {
                        myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "X";
                    }
                    myTmp = Devation(myRPT[idx_fourth].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                    if (myTmp < (spec_dev / 2))
                    {
                        myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_dev)
                    {
                        myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√";
                    }
                    else
                    {
                        myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "X";
                    }
                    myTmp = Devation(myRPT[idx_fifth].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                    if (myTmp < (spec_dev / 2))
                    {
                        myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_dev)
                    {
                        myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√";
                    }
                    else
                    {
                        myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "X";
                    }
                    document.Add(new Paragraph(myLS[0], fontContent));
                    for (int i = 1; i < myLS.Count; i++)
                    {
                        document.Add(new Paragraph(myLS[i], fontContent));
                    }
                    document.Add(new Paragraph(blankLine, fontItem));
                    ////////////////////////////////////////////////////////////////////
                    document.Add(new Paragraph("Accuracy results        (" + mmStr + "℃温度控Block均匀性)", fontItem));
                    document.Add(new Paragraph(blankLine, fontContent));
                    ////////////////////////////////////////////////////////////////////
                    myLS.Clear();
                    myLS.Add("Time (时间)".PadRight((LENL - 2), ' '));
                    myLS.Add(("0 sec").PadRight(LENL, ' '));
                    myLS.Add((idx_time.ToString() + " sec").PadRight(LENL, ' '));
                    myLS.Add(((idx_time * 2).ToString() + " sec").PadRight(LENL, ' '));
                    myLS.Add(((idx_time * 3).ToString() + " sec").PadRight(LENL, ' '));
                    myLS.Add(((idx_time * 4).ToString() + " sec").PadRight(LENL, ' '));
                    myLS[0] = myLS[0] + "Measured (测量值)";
                    myLS[1] = myLS[1] + GetTemp(myRPT[idx_first].OUT.Max() - myRPT[idx_first].OUT.Min()) + " ℃";
                    myLS[2] = myLS[2] + GetTemp(myRPT[idx_second].OUT.Max() - myRPT[idx_second].OUT.Min()) + " ℃";
                    myLS[3] = myLS[3] + GetTemp(myRPT[idx_third].OUT.Max() - myRPT[idx_third].OUT.Min()) + " ℃";
                    myLS[4] = myLS[4] + GetTemp(myRPT[idx_fourth].OUT.Max() - myRPT[idx_fourth].OUT.Min()) + " ℃";
                    myLS[5] = myLS[5] + GetTemp(myRPT[idx_fifth].OUT.Max() - myRPT[idx_fifth].OUT.Min()) + " ℃";
                    myMax = GetJoinLen(myLS, LENL, LENS);
                    myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Specification (参考)";
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                    myMax = GetJoinLen(myLS, LENL, LENW);
                    myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Result";
                    //zhoup
                    myTmp = myRPT[idx_first].OUT.Max() - myRPT[idx_first].OUT.Min();
                    if (myTmp < (spec_max / 2))
                    {
                        myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_max)
                    {
                        myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√";
                    }
                    else
                    {
                        myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "X";
                    }
                    myTmp = myRPT[idx_second].OUT.Max() - myRPT[idx_second].OUT.Min();
                    if (myTmp < (spec_max / 2))
                    {
                        myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_max)
                    {
                        myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√";
                    }
                    else
                    {
                        myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "X";
                    }
                    myTmp = myRPT[idx_third].OUT.Max() - myRPT[idx_third].OUT.Min();
                    if (myTmp < (spec_max / 2))
                    {
                        myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_max)
                    {
                        myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√";
                    }
                    else
                    {
                        myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "X";
                    }
                    myTmp = myRPT[idx_fourth].OUT.Max() - myRPT[idx_fourth].OUT.Min();
                    if (myTmp < (spec_max / 2))
                    {
                        myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_max)
                    {
                        myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√";
                    }
                    else
                    {
                        myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "X";
                    }
                    myTmp = myRPT[idx_fifth].OUT.Max() - myRPT[idx_fifth].OUT.Min();
                    if (myTmp < (spec_max / 2))
                    {
                        myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_max)
                    {
                        myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√";
                    }
                    else
                    {
                        myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "X";
                    }
                    document.Add(new Paragraph(myLS[0], fontContent));
                    for (int i = 1; i < myLS.Count; i++)
                    {
                        document.Add(new Paragraph(myLS[i], fontContent));
                    }
                    document.Add(new Paragraph(blankLine, fontItem));
                    ////////////////////////////////////////////////////////////////////

                    #endregion

                    step_start++;
                    step_target++;
                    step_next++;
                }

                #region 最后一点计算

                //step next
                begin = end;
                end = myRPT.Count - 1;

                //升降温
                if (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature < MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature)
                {
                    rise = true;
                }
                else
                {
                    rise = false;
                }

                //高温
                if (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature > 90)
                {
                    spec_dev = 80;    //0.8℃
                    spec_rate = 150;  //1.5℃
                    spec_over = 600;  //6.0℃
                    spec_max = 150;   //1.5℃
                }
                else if (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature > 70)
                {
                    spec_dev = 60;
                    spec_rate = 150;
                    spec_over = 500;
                    spec_max = 150;
                }
                else
                {
                    spec_dev = 50;
                    spec_rate = 150;
                    spec_over = 500;
                    spec_max = 100;
                }

                //速率
                if ((MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature + 15) < MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature)
                {
                    //升温
                    slope_start = GetUpThrough(begin, end, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature + 5));
                    slope_stop = GetUpThrough(begin, end, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature - 5));
                    slope_tmp = MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature - MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature - 10;
                    slope_get = GetHeatRate(slope_start, slope_stop);
                }
                else if (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature + 15))
                {
                    //降温
                    slope_start = GetDownThrough(begin, end, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature - 5));
                    slope_stop = GetDownThrough(begin, end, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature + 5));
                    slope_tmp = MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_start].temperature - MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature - 10;
                    slope_get = GetCoolRate(slope_start, slope_stop);
                }
                else
                {
                    slope_start = 0;
                    slope_stop = 0;
                    slope_tmp = 0;
                    slope_get = 0;
                }

                //恒定温度计算
                const_start = GetConstStart(begin, end, MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature);
                const_stop = GetConstLeave(const_start, end, (const_start + (30 * hole_per_second)), MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature);

                //过冲
                over_max = GetOverMax(begin, const_stop, rise);
                over_avg = GetOverAvg(begin, const_stop, rise);

                //30秒处
                idx_30sec = const_start + (30 * hole_per_second);

                //间隔时间
                idx_time = MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].time / 4;
                idx_first = 0;
                idx_second = idx_first + idx_time;
                idx_third = idx_second + idx_time;
                idx_fourth = idx_third + idx_time;
                idx_fifth = idx_fourth + idx_time;

                //间隔处
                idx_first = const_start + (idx_first * hole_per_second);
                idx_second = const_start + (idx_second * hole_per_second);
                idx_third = const_start + (idx_third * hole_per_second);
                idx_fourth = const_start + (idx_fourth * hole_per_second);
                idx_fifth = const_start + (idx_fifth * hole_per_second);

                #endregion

                #region 最后一点报告

                //更新页数和创建新页
                document.NewPage();
                document.ResetPageCount();

                //第n页添加元素
                document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                mmStr = (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature / 100f).ToString();
                document.Add(new Paragraph("Temperature        :  " + mmStr + ".00 ℃        (" + mmStr + "℃时的温度表现)", fontContent));
                document.Add(new Paragraph(blankLine, fontItem));
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                document.Add(new Paragraph("Values after 30 seconds        (" + mmStr + "℃第30秒时15孔的温度数据)", fontItem));
                document.Add(new Paragraph(blankLine, fontContent));
                ////////////////////////////////////////////////////////////////////
                myLS.Clear();
                myLS.Add("".PadRight(LENL, ' '));
                myLS.Add("Probe 1 (A1)".PadRight(LENL, ' '));
                myLS.Add("Probe 2 (A4)".PadRight(LENL, ' '));
                myLS.Add("Probe 3 (A7)".PadRight(LENL, ' '));
                myLS.Add("Probe 4 (A10)".PadRight(LENL, ' '));
                myLS.Add("Probe 5 (A12)".PadRight(LENL, ' '));
                myLS.Add("Probe 6 (D1)".PadRight(LENL, ' '));
                myLS.Add("Probe 7 (D7)".PadRight(LENL, ' '));
                myLS.Add("Probe 8 (D12)".PadRight(LENL, ' '));
                myLS.Add("Probe 9 (E4)".PadRight(LENL, ' '));
                myLS.Add("Probe 10 (E10)".PadRight(LENL, ' '));
                myLS.Add("Probe 11 (H1)".PadRight(LENL, ' '));
                myLS.Add("Probe 12 (H4)".PadRight(LENL, ' '));
                myLS.Add("Probe 13 (H7)".PadRight(LENL, ' '));
                myLS.Add("Probe 14 (H10)".PadRight(LENL, ' '));
                myLS.Add("Probe 15 (H12)".PadRight(LENL, ' '));
                myLS[0] = myLS[0] + "Measured";
                for (int i = 0; i < SZ.CHA; i++)
                {
                    myLS[i + 1] = myLS[i + 1] + GetTemp(myRPT[idx_30sec].OUT[i]) + " ℃";
                }
                myMax = GetJoinLen(myLS, LENL, LENS);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "T(meas-set)";
                temp_max = myRPT[idx_30sec].OUT[0];
                temp_min = myRPT[idx_30sec].OUT[0];
                mark_max = 1;
                mark_min = 1;
                for (int i = 0; i < SZ.CHA; i++)
                {
                    myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + GetTemp(myRPT[idx_30sec].OUT[i] - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + " ℃";
                    if (myRPT[idx_30sec].OUT[i] > temp_max)
                    {
                        temp_max = myRPT[idx_30sec].OUT[i];
                        mark_max = i + 1;
                    }
                    if (myRPT[idx_30sec].OUT[i] < temp_min)
                    {
                        temp_min = myRPT[idx_30sec].OUT[i];
                        mark_min = i + 1;
                    }
                }
                myLS[mark_max] = myLS[mark_max] + " (max)";
                myLS[mark_min] = myLS[mark_min] + " (min)";
                myMax = GetJoinLen(myLS, LENL, LENS);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Status";
                for (int i = 0; i < SZ.CHA; i++)
                {
                    myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + "Active";
                }
                document.Add(new Paragraph(myLS[0], fontContent));
                for (int i = 0; i < SZ.CHA; i++)
                {
                    document.Add(new Paragraph(myLS[i + 1], fontContent));
                }
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                document.Add(new Paragraph("Step results        (" + mmStr + "℃温度控制性能)", fontItem));
                document.Add(new Paragraph(blankLine, fontContent));
                ////////////////////////////////////////////////////////////////////
                myLS.Clear();
                myLS.Add("Item (项目)");
                if (rise)
                {
                    if (slope_get > 0)
                    {
                        myLS.Add("Max. heat rate (最大升温速率)");
                        myLS.Add("Avg. heat rate (平均升温速率)");
                    }
                    myLS.Add("Max. overshoot (最大过冲)");
                    myLS.Add("Avg. overshoot (平均过冲)");
                    myLS.Add("Heat time      (升温时间)");
                    myLS.Add("Hold time      (恒温时间)");
                }
                else
                {
                    if (slope_get > 0)
                    {
                        myLS.Add("Max. cool rate (最大降温速率)");
                        myLS.Add("Avg. cool rate (平均降温速率)");
                    }
                    myLS.Add("Max. overshoot (最大过冲)");
                    myLS.Add("Avg. overshoot (平均过冲)");
                    myLS.Add("Cool time      (降温时间)");
                    myLS.Add("Hold time      (恒温时间)");
                }
                myMax = GetJoinLen(myLS, LENL, LENS);
                idx = 0;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Measured (测量值)";
                if (slope_get > 0)
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((slope_get * hole_per_second) / 100.0f).ToString("F2") + " ℃/sec";
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (((slope_tmp * 100 * hole_per_second) / (slope_stop - slope_start)) / 100.0f).ToString("F2") + " ℃/sec";
                }
                if (rise)
                {
                    if (over_max > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + GetTemp(over_max) + " ℃";
                        myLS[idx] = myLS[idx] + " (+" + GetTemp(over_max - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                    }
                }
                else
                {
                    if (over_max < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + GetTemp(over_max) + " ℃";
                        myLS[idx] = myLS[idx] + " (" + GetTemp(over_max - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                    }
                }
                if (rise)
                {
                    if (over_avg > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + GetTemp(over_avg) + " ℃";
                        myLS[idx] = myLS[idx] + " (+" + GetTemp(over_avg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                    }
                }
                else
                {
                    if (over_avg < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + GetTemp(over_avg) + " ℃";
                        myLS[idx] = myLS[idx] + " (" + GetTemp(over_avg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                    }
                }
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((const_start - begin) / hole_per_second).ToString() + " sec";
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((const_stop - const_start) / hole_per_second).ToString() + " sec";
                myMax = GetJoinLen(myLS, LENL, LENS);
                idx = 0;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Specification (参考)";
                if (slope_get > 0)
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_rate / 100.0f).ToString("F1") + " ℃/sec";
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_rate / 100.0f).ToString("F1") + " ℃/sec";
                }
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_over / 100.0f).ToString("F1") + " ℃";
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_over / 100.0f).ToString("F1") + " ℃";
                myMax = GetJoinLen(myLS, LENL, LENW);
                idx = 0;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Result";
                if (slope_get > 0)
                {
                    //
                    myTmp = slope_get * hole_per_second;
                    if (myTmp > (spec_rate * 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp > spec_rate)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }

                    //
                    myTmp = (slope_tmp * 100 * hole_per_second) / (slope_stop - slope_start);
                    if (myTmp > (spec_rate * 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp > spec_rate)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }
                }
                if (rise)
                {
                    if (over_max > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        myTmp = Devation(over_max, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));

                        if (myTmp < (spec_over / 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp < spec_over)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
                else
                {
                    if (over_max < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        myTmp = Devation(over_max, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));

                        if (myTmp < (spec_over / 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp < spec_over)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
                if (rise)
                {
                    if (over_avg > (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        myTmp = Devation(over_avg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));

                        if (myTmp < (spec_over / 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp < spec_over)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
                else
                {
                    if (over_avg < (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100))
                    {
                        myTmp = Devation(over_avg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));

                        if (myTmp < (spec_over / 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp < spec_over)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
                document.Add(new Paragraph(myLS[0], fontContent));
                for (int i = 1; i < myLS.Count; i++)
                {
                    document.Add(new Paragraph(myLS[i], fontContent));
                }
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                document.Add(new Paragraph("Accuracy results        (" + mmStr + "℃温度控准确度)", fontItem));
                document.Add(new Paragraph(blankLine, fontContent));
                ////////////////////////////////////////////////////////////////////
                myLS.Clear();
                myLS.Add("Time (时间)".PadRight((LENL - 2), ' '));
                myLS.Add(("0 sec").PadRight(LENL, ' '));
                myLS.Add((idx_time.ToString() + " sec").PadRight(LENL, ' '));
                myLS.Add(((idx_time * 2).ToString() + " sec").PadRight(LENL, ' '));
                myLS.Add(((idx_time * 3).ToString() + " sec").PadRight(LENL, ' '));
                myLS.Add(((idx_time * 4).ToString() + " sec").PadRight(LENL, ' '));
                myLS[0] = myLS[0] + "Measured (测量值)";
                myLS[1] = myLS[1] + GetTemp(myRPT[idx_first].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_first].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                myLS[2] = myLS[2] + GetTemp(myRPT[idx_second].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_second].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                myLS[3] = myLS[3] + GetTemp(myRPT[idx_third].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_third].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                myLS[4] = myLS[4] + GetTemp(myRPT[idx_fourth].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_fourth].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                myLS[5] = myLS[5] + GetTemp(myRPT[idx_fifth].outAvg) + " ℃" + " (" + GetTempWithSign(myRPT[idx_fifth].outAvg - (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100)) + "℃)";
                myMax = GetJoinLen(myLS, LENL, LENS);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Specification (参考)";
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myMax = GetJoinLen(myLS, LENL, LENW);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Result";
                myTmp = Devation(myRPT[idx_first].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√";
                }
                else
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "X";
                }
                myTmp = Devation(myRPT[idx_second].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√";
                }
                else
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "X";
                }
                myTmp = Devation(myRPT[idx_third].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√";
                }
                else
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "X";
                }
                myTmp = Devation(myRPT[idx_fourth].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√";
                }
                else
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "X";
                }
                myTmp = Devation(myRPT[idx_fifth].outAvg, (MyDefine.myXET.myPRM[comboBox1.SelectedIndex].myTP[step_target].temperature * 100));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√";
                }
                else
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "X";
                }
                document.Add(new Paragraph(myLS[0], fontContent));
                for (int i = 1; i < myLS.Count; i++)
                {
                    document.Add(new Paragraph(myLS[i], fontContent));
                }
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                document.Add(new Paragraph("Accuracy results        (" + mmStr + "℃温度控Block均匀性)", fontItem));
                document.Add(new Paragraph(blankLine, fontContent));
                ////////////////////////////////////////////////////////////////////
                myLS.Clear();
                myLS.Add("Time (时间)".PadRight((LENL - 2), ' '));
                myLS.Add(("0 sec").PadRight(LENL, ' '));
                myLS.Add((idx_time.ToString() + " sec").PadRight(LENL, ' '));
                myLS.Add(((idx_time * 2).ToString() + " sec").PadRight(LENL, ' '));
                myLS.Add(((idx_time * 3).ToString() + " sec").PadRight(LENL, ' '));
                myLS.Add(((idx_time * 4).ToString() + " sec").PadRight(LENL, ' '));
                myLS[0] = myLS[0] + "Measured (测量值)";
                myLS[1] = myLS[1] + GetTemp(myRPT[idx_first].OUT.Max() - myRPT[idx_first].OUT.Min()) + " ℃";
                myLS[2] = myLS[2] + GetTemp(myRPT[idx_second].OUT.Max() - myRPT[idx_second].OUT.Min()) + " ℃";
                myLS[3] = myLS[3] + GetTemp(myRPT[idx_third].OUT.Max() - myRPT[idx_third].OUT.Min()) + " ℃";
                myLS[4] = myLS[4] + GetTemp(myRPT[idx_fourth].OUT.Max() - myRPT[idx_fourth].OUT.Min()) + " ℃";
                myLS[5] = myLS[5] + GetTemp(myRPT[idx_fifth].OUT.Max() - myRPT[idx_fifth].OUT.Min()) + " ℃";
                myMax = GetJoinLen(myLS, LENL, LENS);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Specification (参考)";
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myMax = GetJoinLen(myLS, LENL, LENW);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Result";
                //zhoup
                myTmp = myRPT[idx_first].OUT.Max() - myRPT[idx_first].OUT.Min();
                if (myTmp < (spec_max / 2))
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_max)
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√";
                }
                else
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "X";
                }
                myTmp = myRPT[idx_second].OUT.Max() - myRPT[idx_second].OUT.Min();
                if (myTmp < (spec_max / 2))
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_max)
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√";
                }
                else
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "X";
                }
                myTmp = myRPT[idx_third].OUT.Max() - myRPT[idx_third].OUT.Min();
                if (myTmp < (spec_max / 2))
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_max)
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√";
                }
                else
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "X";
                }
                myTmp = myRPT[idx_fourth].OUT.Max() - myRPT[idx_fourth].OUT.Min();
                if (myTmp < (spec_max / 2))
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_max)
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√";
                }
                else
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "X";
                }
                myTmp = myRPT[idx_fifth].OUT.Max() - myRPT[idx_fifth].OUT.Min();
                if (myTmp < (spec_max / 2))
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_max)
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√";
                }
                else
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "X";
                }
                document.Add(new Paragraph(myLS[0], fontContent));
                for (int i = 1; i < myLS.Count; i++)
                {
                    document.Add(new Paragraph(myLS[i], fontContent));
                }
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////

                #endregion
            }

            #region 关闭

            //关闭
            document.Close();

            //调出pdf
            Process.Start(fileDialog.FileName);

            #endregion
        }

        //生成报告
        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.Items.Count > 0)
            {
                CreateReport();
                SaveUserInfo();
            }
        }

        private void MenuReportForm_Load(object sender, EventArgs e)
        {
            MyDefine.myXET.GetUserPrm();
            GetListBox();
            GetCombox();
            GetUserInfo();
        }

        private void MenuReportForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //取消关闭事件
            this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.MenuReportForm_FormClosing);
        }
    }
}
