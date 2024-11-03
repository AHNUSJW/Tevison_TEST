using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace TVS
{
    public partial class MenuRunForm : Form
    {
        private bool[] enc = new bool[SZ.CHA];//曲线显示通道使能

        private Byte mcyc = 0;//控制读温度和参数的节奏
        private Int16 mtick = 0;//控制读温度和参数的节奏

        private Picture myp = new Picture();//画图
        private DateTime start_time;//开始检测录制的时间
        private DateTime stop_time;//停止检测录制的时间
        private String myPath = MyDefine.myXET.userDAT;
        private String myName = "backups";
        private String myExcel = "backups";

        private MenuFieldForm myField = new MenuFieldForm();

        public MenuRunForm()
        {
            InitializeComponent();
        }

        private void checkBoxAllOn_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAllOn.Checked)
            {
                enc[0] = true;
                enc[1] = true;
                enc[2] = true;
                enc[3] = true;
                enc[4] = true;
                enc[5] = true;
                enc[6] = true;
                enc[7] = true;
                enc[8] = true;
                enc[9] = true;
                enc[10] = true;
                enc[11] = true;
                enc[12] = true;
                enc[13] = true;
                enc[14] = true;

                checkBoxAllOff.Checked = false;
                checkBox1.Checked = true;
                checkBox2.Checked = true;
                checkBox3.Checked = true;
                checkBox4.Checked = true;
                checkBox5.Checked = true;
                checkBox6.Checked = true;
                checkBox7.Checked = true;
                checkBox8.Checked = true;
                checkBox9.Checked = true;
                checkBox10.Checked = true;
                checkBox11.Checked = true;
                checkBox12.Checked = true;
                checkBox13.Checked = true;
                checkBox14.Checked = true;
                checkBox15.Checked = true;

                pictureBoxScope_draw();
            }
        }

        private void checkBoxAllOff_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxAllOff.Checked)
            {
                enc[0] = false;
                enc[1] = false;
                enc[2] = false;
                enc[3] = false;
                enc[4] = false;
                enc[5] = false;
                enc[6] = false;
                enc[7] = false;
                enc[8] = false;
                enc[9] = false;
                enc[10] = false;
                enc[11] = false;
                enc[12] = false;
                enc[13] = false;
                enc[14] = false;

                checkBoxAllOn.Checked = false;
                checkBox1.Checked = false;
                checkBox2.Checked = false;
                checkBox3.Checked = false;
                checkBox4.Checked = false;
                checkBox5.Checked = false;
                checkBox6.Checked = false;
                checkBox7.Checked = false;
                checkBox8.Checked = false;
                checkBox9.Checked = false;
                checkBox10.Checked = false;
                checkBox11.Checked = false;
                checkBox12.Checked = false;
                checkBox13.Checked = false;
                checkBox14.Checked = false;
                checkBox15.Checked = false;

                pictureBoxScope_draw();
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[0] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[0] = false;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[1] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[1] = false;
            }
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox3.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[2] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[2] = false;
            }
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox4.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[3] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[3] = false;
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox5.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[4] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[4] = false;
            }
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox6.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[5] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[5] = false;
            }
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox7.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[6] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[6] = false;
            }
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox8.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[7] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[7] = false;
            }
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox9.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[8] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[8] = false;
            }
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox10.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[9] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[9] = false;
            }
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox11.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[10] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[10] = false;
            }
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox12.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[11] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[11] = false;
            }
        }

        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox13.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[12] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[12] = false;
            }
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox14.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[13] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[13] = false;
            }
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox15.Checked)
            {
                checkBoxAllOff.Checked = false;
                enc[14] = true;
            }
            else
            {
                checkBoxAllOn.Checked = false;
                enc[14] = false;
            }
        }

        //按钮
        private void button1_Click(object sender, EventArgs e)
        {
            if ((MyDefine.myXET.devVersion > 0) && (MyDefine.myXET.devStatus > TmpMode.NULL))
            {
                MessageBox.Show("设备工作中, 无法实时记录");
            }
            else
            {
                //初始化数据
                MyDefine.myXET.old_index = 0;
                MyDefine.myXET.count = 0;
                //开始检测录制
                if (MyDefine.myXET.myTask == TASKS.run)
                {
                    //
                    button1.Text = "停 止";
                    button1.BackColor = Color.Firebrick;
                    start_time = System.DateTime.Now;
                    MyDefine.myXET.myTask = TASKS.record;

                    //
                    MyDefine.myXET.syndate = Convert.ToUInt32(start_time.ToString("yyMMdd"));
                    MyDefine.myXET.syntime = Convert.ToUInt32(start_time.ToString("HHmmss"));
                    MyDefine.myXET.syntimeMs = Convert.ToUInt32(start_time.ToString("HHmmssfff"));

                    //
                    MyDefine.myXET.strend = true;
                    MyDefine.myXET.oldrnd = 0;
                    MyDefine.myXET.overmax = 0;
                    MyDefine.myXET.overavg = 0;
                    MyDefine.myXET.outdim = 0;
                    MyDefine.myXET.outstm = 0;

                    //
                    MyDefine.myXET.mySyn.Clear();
                }
                //停止检测录制
                else if (MyDefine.myXET.myTask == TASKS.record)
                {
                    //初始化数据
                    MyDefine.myXET.old_index = 0;
                    MyDefine.myXET.count = 0;
                    //
                    button1.Text = "开始检测录制";
                    button1.BackColor = SystemColors.Control;
                    stop_time = System.DateTime.Now;
                    MyDefine.myXET.myTask = TASKS.run;

                    //
                    MyDefine.myXET.synstop = Convert.ToUInt32(stop_time.ToString("HHmmss"));
                    MyDefine.myXET.synrun = (uint)((stop_time - start_time).TotalMilliseconds);
                    MyDefine.myXET.synstep = (uint)(MyDefine.myXET.synrun / MyDefine.myXET.mySyn.Count);

                    //
                    //不存在则创建文件夹
                    if (!Directory.Exists(myPath))
                    {
                        Directory.CreateDirectory(myPath);
                    }

                    //不存在则创建文件
                    if (textBox1.TextLength > 0)
                    {
                        myName = myPath + "\\" + MyDefine.myXET.syndate.ToString() + MyDefine.myXET.syntime.ToString() + "_" + MyDefine.myXET.synrun.ToString() + "_" + textBox1.Text + ".tmp";
                        myExcel = myPath + "\\" + MyDefine.myXET.syndate.ToString() + MyDefine.myXET.syntime.ToString() + "_" + MyDefine.myXET.synrun.ToString() + "_" + textBox1.Text + ".csv";
                    }
                    else
                    {
                        myName = myPath + "\\" + MyDefine.myXET.syndate.ToString() + MyDefine.myXET.syntime.ToString() + "_" + MyDefine.myXET.synrun.ToString() + ".tmp";
                        myExcel = myPath + "\\" + MyDefine.myXET.syndate.ToString() + MyDefine.myXET.syntime.ToString() + "_" + MyDefine.myXET.synrun.ToString() + ".csv";
                    }

                    //保存
                    MyDefine.myXET.syn_SaveToLog(myName);
                    MyDefine.myXET.syn_SaveToExcel(myExcel);
                }
            }
        }

        //
        private void button2_Click(object sender, EventArgs e)
        {
            if (myField.isOpen)
            {
                myField.BringToFront();
            }
            else
            {
                myField = new MenuFieldForm();
                myField.Show();
                myField.BringToFront();
            }
        }

        //画曲线
        private void pictureBoxScope_draw()
        {
            //数据范围
            if (MyDefine.myXET.mySyn.Count < 3)
            {
                return;
            }
            else if (MyDefine.myXET.mySyn.Count < (myp.xw_stop - myp.xw_start))
            {
                myp.start = 0;
                myp.stop = MyDefine.myXET.mySyn.Count;
            }
            else
            {
                myp.start = MyDefine.myXET.mySyn.Count - (myp.xw_stop - myp.xw_start);
                myp.stop = MyDefine.myXET.mySyn.Count;
            }

            //层图
            Bitmap img = new Bitmap(myp.Width, myp.Height);

            //绘制
            Graphics g = Graphics.FromImage(img);

            //画曲线
            if (enc[0])
            {
                myp.getProbe1();
                g.DrawCurve(new Pen(myp.tmp_curve_1, 1.0f), myp.probe1.ToArray(), 0);
            }
            if (enc[1])
            {
                myp.getProbe2();
                g.DrawCurve(new Pen(myp.tmp_curve_2, 1.0f), myp.probe2.ToArray(), 0);
            }
            if (enc[2])
            {
                myp.getProbe3();
                g.DrawCurve(new Pen(myp.tmp_curve_3, 1.0f), myp.probe3.ToArray(), 0);
            }
            if (enc[3])
            {
                myp.getProbe4();
                g.DrawCurve(new Pen(myp.tmp_curve_4, 1.0f), myp.probe4.ToArray(), 0);
            }
            if (enc[4])
            {
                myp.getProbe5();
                g.DrawCurve(new Pen(myp.tmp_curve_5, 1.0f), myp.probe5.ToArray(), 0);
            }
            if (enc[5])
            {
                myp.getProbe6();
                g.DrawCurve(new Pen(myp.tmp_curve_6, 1.0f), myp.probe6.ToArray(), 0);
            }
            if (enc[6])
            {
                myp.getProbe7();
                g.DrawCurve(new Pen(myp.tmp_curve_7, 1.0f), myp.probe7.ToArray(), 0);
            }
            if (enc[7])
            {
                myp.getProbe8();
                g.DrawCurve(new Pen(myp.tmp_curve_8, 1.0f), myp.probe8.ToArray(), 0);
            }
            if (enc[8])
            {
                myp.getProbe9();
                g.DrawCurve(new Pen(myp.tmp_curve_9, 1.0f), myp.probe9.ToArray(), 0);
            }
            if (enc[9])
            {
                myp.getProbe10();
                g.DrawCurve(new Pen(myp.tmp_curve_10, 1.0f), myp.probe10.ToArray(), 0);
            }
            if (enc[10])
            {
                myp.getProbe11();
                g.DrawCurve(new Pen(myp.tmp_curve_11, 1.0f), myp.probe11.ToArray(), 0);
            }
            if (enc[11])
            {
                myp.getProbe12();
                g.DrawCurve(new Pen(myp.tmp_curve_12, 1.0f), myp.probe12.ToArray(), 0);
            }
            if (enc[12])
            {
                myp.getProbe13();
                g.DrawCurve(new Pen(myp.tmp_curve_13, 1.0f), myp.probe13.ToArray(), 0);
            }
            if (enc[13])
            {
                myp.getProbe14();
                g.DrawCurve(new Pen(myp.tmp_curve_14, 1.0f), myp.probe14.ToArray(), 0);
            }
            if (enc[14])
            {
                myp.getProbe15();
                g.DrawCurve(new Pen(myp.tmp_curve_15, 1.0f), myp.probe15.ToArray(), 0);
            }

            //铺图
            pictureBox1.Image = img;

            //
            g.Dispose();
        }

        //画坐标线
        private void pictureBoxScope_axis()
        {
            //
            myp.getAxis_pictureBox(pictureBox1);

            //层图
            Bitmap img = new Bitmap(myp.Width, myp.Height);

            //绘制
            Graphics g = Graphics.FromImage(img);

            //格线
            for (int i = myp.yh_grid, k = 5; i < myp.Height; i += myp.yh_grid)
            {
                g.DrawLine(new Pen(myp.tmp_lines, 1f), new Point(myp.xw_start, i), new Point(myp.xw_stop, i));
                g.DrawString((((myp.tmax / 100) - k).ToString() + "℃"), new Font("Arial", 9, FontStyle.Italic), Brushes.Black, myp.xw_text, i - 9);
                k += 5;
            }

            //竖线
            g.DrawLine(new Pen(myp.tmp_lines, 1f), new Point(myp.xw_start - 1, 0), new Point(myp.xw_start - 1, myp.Height));
            g.DrawLine(new Pen(myp.tmp_lines, 1f), new Point(myp.xw_stop + 1, 0), new Point(myp.xw_stop + 1, myp.Height));

            //外框
            g.DrawLine(new Pen(myp.tmp_outline, 4f), new Point(0, 0), new Point(0, myp.Height));
            g.DrawLine(new Pen(myp.tmp_outline, 4f), new Point(0, myp.Height), new Point(myp.Width, myp.Height));
            g.DrawLine(new Pen(myp.tmp_outline, 4f), new Point(myp.Width, 0), new Point(myp.Width, myp.Height));
            g.DrawLine(new Pen(myp.tmp_outline, 4f), new Point(0, 0), new Point(myp.Width, 0));

            //铺图
            pictureBox1.BackgroundImage = img;

            //
            g.Dispose();
        }

        //显示温度表
        private void tableTemp_display()
        {
            label1.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[0]) + "\n  A1";
            label2.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[1]) + "\n  A4";
            label3.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[2]) + "\n  A7";
            label4.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[3]) + "\n  A10";
            label5.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[4]) + "\n  A12";
            label6.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[5]) + "\n  D1";
            label7.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[6]) + "\n  D7";
            label8.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[7]) + "\n  D12";
            label9.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[8]) + "\n  E4";
            label10.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[9]) + "\n  E10";
            label11.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[10]) + "\n  H1";
            label12.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[11]) + "\n  H4";
            label13.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[12]) + "\n  H7";
            label14.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[13]) + "\n  H10";
            label15.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[14]) + "\n  H12";
        }

        //显示参数
        private void labelPar_display()
        {
            float dat, p_dat;

            //
            label16.Text = "孔最大温度：" + MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.outMax) + "℃";
            label17.Text = "孔最小温度：" + MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.outMin) + "℃";
            label18.Text = "孔间最大差：" + MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.outDif) + "℃";
            label19.Text = "孔间标准差：" + MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.outStd);
            label20.Text = (MyDefine.myXET.mtp.outRnd / 100).ToString() + "度标准差：" + MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.outDep);
            label21.Text = "孔间平均值：" + MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.outAvg) + "℃";

            //
            if (MyDefine.myXET.upsecond > 0)
            {
                label22.Text = "循环平均升温速度：" + (40.0f / MyDefine.myXET.upsecond).ToString("f2") + "℃/sec";
            }
            if (MyDefine.myXET.dnsecond > 0)
            {
                label23.Text = "循环平均降温速度：" + (40.0f / MyDefine.myXET.dnsecond).ToString("f2") + "℃/sec";
            }
            label24.Text = "循环最大孔间差值：" + MyDefine.myXET.GetTemp(MyDefine.myXET.outdim) + "℃";
            label25.Text = "循环最大孔间标准差：" + MyDefine.myXET.GetTemp(MyDefine.myXET.outstm);
            if (MyDefine.myXET.overmax > 0)
            {
                label26.Text = "循环" + (MyDefine.myXET.mtp.outRnd / 100).ToString() + "度最大过冲：" + MyDefine.myXET.GetTemp(Math.Abs(MyDefine.myXET.overmax - MyDefine.myXET.mtp.outRnd)) + "℃";
            }
            if (MyDefine.myXET.overavg > 0)
            {
                label27.Text = "循环" + (MyDefine.myXET.mtp.outRnd / 100).ToString() + "度平均过冲：" + MyDefine.myXET.GetTemp(Math.Abs(MyDefine.myXET.overavg - MyDefine.myXET.mtp.outRnd)) + "℃";
            }

            //
            if (MyDefine.myXET.myTask == TASKS.record)
            {
                label29.Text = "计时：" + (System.DateTime.Now - start_time).Seconds.ToString();
            }
            else
            {
                label29.Text = "时间：" + System.DateTime.Now.ToString("hh:mm:ss");
            }

            //
            dat = ((3.3f * (float)MyDefine.myXET.battery / 4096.0f) / 2.0f * 3.0f + 0.08f);
            if (dat >= 3.8f)
            {
                p_dat = 1;
            }
            else if (dat < 3.0f)
            {
                p_dat = 0;
            }
            else
            {
                p_dat = (dat - 3.0f) / 0.8f;
            }
            labelBattery.Text = "锂电池电压：\r\n" + dat.ToString("f2") + "V(" + p_dat.ToString("p0") + ")";
            dat = ((1.43f - (3.3f * (float)MyDefine.myXET.temperature / 4096.0f)) / 0.0043f + 25.0f);
            labelTemperature.Text = "采集器温度：\r\n" + dat.ToString("f1") + "℃";
        }

        //实际操作函数
        private void run_DataReceived()
        {
            //其它线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(run_DataReceived);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {
                    MyDefine.myXET.myUpdate -= new freshHandler(run_DataReceived);
                }
            }
            //本线程的操作请求
            else
            {
                if (MyDefine.myXET.myTask >= TASKS.run)
                {
                    mtick = 0;

                    //间隔
                    switch (mcyc)
                    {
                        default:
                            mcyc++;
                            MyDefine.myXET.mePort_Send_ReadTmp();
                            break;
                        case 4:
                            mcyc = 0;
                            MyDefine.myXET.mePort_Send_ReadPar();
                            break;
                    }
                }
            }
        }

        //定时处理
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (MyDefine.myXET.myTask >= TASKS.run)
            {
                //超出范围调整
                if (MyDefine.myXET.isAxis)
                {
                    myp.getAxis_pictureBox(MyDefine.myXET.tmpMax, MyDefine.myXET.tmpMin);
                    pictureBoxScope_axis();
                }

                //
                labelPar_display();
                tableTemp_display();
                pictureBoxScope_draw();

                //
                if (myField.isOpen)
                {
                    myField.pictureBox_getvalue(MyDefine.myXET.mtp, MyDefine.myXET.mtp.outAvg);
                }

                //间隔
                if ((++mtick) > 3)
                {
                    mtick = 0;
                    MyDefine.myXET.mePort_Send_ReadTmp();
                }
            }
        }

        //窗口改变
        private void MenuRunForm_Resized(object sender, EventArgs e)
        {
            if (this.Width < 720) this.Width = 720;
            if (this.Height < 400) this.Height = 400;
            pictureBoxScope_axis();
            pictureBoxScope_draw();
        }

        //启动
        private void MenuRunForm_Load(object sender, EventArgs e)
        {
            //加载接收触发
            MyDefine.myXET.myUpdate += new freshHandler(run_DataReceived);
            MyDefine.myXET.mySyn.Clear();
            MyDefine.myXET.old_index = 0;
            MyDefine.myXET.count = 0;
            //
            enc[0] = true;
            enc[1] = true;
            enc[2] = true;
            enc[3] = true;
            enc[4] = true;
            enc[5] = true;
            enc[6] = true;
            enc[7] = true;
            enc[8] = true;
            enc[9] = true;
            enc[10] = true;
            enc[11] = true;
            enc[12] = true;
            enc[13] = true;
            enc[14] = true;
            //
            pictureBoxScope_axis();
            pictureBoxScope_draw();
            //
            if ((MyDefine.myXET.devVersion > 0) && (MyDefine.myXET.devStatus > TmpMode.NULL))
            {
                MessageBox.Show("设备工作中, 无法实时记录");
            }
            else
            {
                timer1.Enabled = true;
            }
        }

        //退出
        private void MenuRunForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;

            //
            if (MyDefine.myXET.myTask == TASKS.record)
            {
                if (DialogResult.Yes == MessageBox.Show("正在进行检测录制，是否放弃录制保存并关闭系统？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    MyDefine.myXET.myTask = TASKS.run;
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }

            //取消更新LabelText事件
            MyDefine.myXET.myUpdate -= new freshHandler(run_DataReceived);
            //
            MyDefine.myXET.rtCOM = RTCOM.COM_NULL;
            //
            myField.Close();
        }
    }
}
