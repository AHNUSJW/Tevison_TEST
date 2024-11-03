using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace TVS
{
    public partial class MenuReadForm : Form
    {
        //
        private Boolean isRun = false;//使能导出和取消后停止通讯保存
        private Boolean isStop = false;//是否先停止再读出
        private int total = 0;//控制进度条
        private String myPath = MyDefine.myXET.userDAT;
        private String myName = "backups";
        private String myExcel = "backups";

        //
        public MenuReadForm()
        {
            InitializeComponent();
        }

        //报警提示
        private void warning_NI(string meErr)
        {
            timer1.Enabled = true;
            timer1.Interval = 3000;
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(3000, notifyIcon1.Text, meErr, ToolTipIcon.Info);
        }

        //文件名规则
        private void psw_KeyPress(object sender, KeyPressEventArgs e)
        {
            //不可以有以下特殊字符
            // \/:*?"<>|
            // \\
            // \|
            // ""
            Regex meRgx = new Regex(@"[\\/:*?""<>\|]");
            if (meRgx.IsMatch(e.KeyChar.ToString()))
            {
                warning_NI("不能使用\\/:*?\"<>|");
                e.Handled = true;
            }
        }

        //时间控制
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            notifyIcon1.Visible = false;
        }

        //取消
        private void button1_Click(object sender, EventArgs e)
        {
            isRun = false;
            button2.Text = "开 始 读 出";
            button2.BackColor = SystemColors.Control;
            textBox2.Text += "已取消\r\n";
            progressBar1.Value = progressBar1.Minimum;
        }

        //开始读出
        private void button2_Click(object sender, EventArgs e)
        {
            //先停再读
            if (MyDefine.myXET.devVersion == 0)
            {
                MyDefine.myXET.mode = TmpMode.NULL;
                MyDefine.myXET.date = Convert.ToUInt32(System.DateTime.Now.ToString("yyMMdd"));
                MyDefine.myXET.time = Convert.ToUInt32(System.DateTime.Now.ToString("HHmmss"));
                MyDefine.myXET.myTask = TASKS.setting;
                MyDefine.myXET.mePort_Send_SetMem();
                isStop = true;
            }
            //询问是否读
            else if (MyDefine.myXET.devStatus > TmpMode.NULL)
            {
                //不停止
                if (DialogResult.OK != MessageBox.Show("设备工作中, 是否停止记录并读取数据?", "提示", MessageBoxButtons.OKCancel))
                {
                    isStop = false;
                    return;
                }
                //先停再读
                else
                {
                    MyDefine.myXET.mode = TmpMode.NULL;
                    MyDefine.myXET.date = Convert.ToUInt32(System.DateTime.Now.ToString("yyMMdd"));
                    MyDefine.myXET.time = Convert.ToUInt32(System.DateTime.Now.ToString("HHmmss"));
                    MyDefine.myXET.myTask = TASKS.setting;
                    MyDefine.myXET.mePort_Send_SetMem();
                    isStop = true;
                    textBox2.Text = "停止设备记录\r\n";
                }
            }
            //直接读
            else
            {
                MyDefine.myXET.errCount = 0;
                total = 0;
                isRun = true;
                isStop = false;
                button2.Text = "正 在 读 出";
                button2.BackColor = Color.Firebrick;
                textBox2.Text = "开始读出\r\n";
                progressBar1.Value = progressBar1.Minimum;
                MyDefine.myXET.memdate = 0;
                MyDefine.myXET.memtime = 0;
                MyDefine.myXET.memstop = 0;
                MyDefine.myXET.memrun = 0;
                MyDefine.myXET.myMem.Clear();
                MyDefine.myXET.myTask = TASKS.setting;
                MyDefine.myXET.mePort_Send_ReadMem(0x80);
                textBox2.Text += "正在读出...\r\n";
            }
        }

        //
        #region Realize component update between thread with delegate
        //

        //实际操作函数
        private void read_DataReceived()
        {
            //其它线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(read_DataReceived);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {
                    MyDefine.myXET.myUpdate -= new freshHandler(read_DataReceived);
                }
            }
            //本线程的操作请求
            else
            {
                if (isStop)
                {
                    switch (MyDefine.myXET.rtCOM)
                    {
                        default:
                            isStop = false;
                            break;
                        case RTCOM.COM_NULL:
                            MyDefine.myXET.errCount = 0;
                            total = 0;
                            isRun = true;
                            isStop = false;
                            button2.Text = "正 在 读 出";
                            button2.BackColor = Color.Firebrick;
                            textBox2.Text += "开始读出\r\n";
                            progressBar1.Value = progressBar1.Minimum;
                            MyDefine.myXET.memdate = 0;
                            MyDefine.myXET.memtime = 0;
                            MyDefine.myXET.memstop = 0;
                            MyDefine.myXET.memrun = 0;
                            MyDefine.myXET.myMem.Clear();
                            MyDefine.myXET.myTask = TASKS.setting;
                            MyDefine.myXET.mePort_Send_ReadMem(0x80);
                            textBox2.Text += "正在读出...\r\n";
                            break;
                        case RTCOM.COM_SET_MEM:
                            MyDefine.myXET.mePort_Send_SetMem();
                            break;
                    }
                }
                else if (isRun)
                {
                    if (MyDefine.myXET.rtCOM == RTCOM.COM_NULL)
                    {
                        //
                        if(MyDefine.myXET.errCount > 0)
                        {
                            button2.Text = "完 成 " + MyDefine.myXET.errCount.ToString();
                        }
                        else
                        {
                            button2.Text = "完 成";
                        }
                        button2.BackColor = Color.Green;
                        progressBar1.Value = progressBar1.Maximum;
                        textBox2.Text += "读出完成\r\n";

                        //norFlash无数据
                        if (MyDefine.myXET.memdate == 0xFFFFFFFF)
                        {
                            textBox2.Text += "设备无数据\r\n";
                        }
                        //有数据
                        else
                        {
                            //
                            textBox2.Text += "日期:" + MyDefine.myXET.memdate.ToString() + "\r\n";
                            textBox2.Text += "开始时刻:" + MyDefine.myXET.memtime.ToString() + "\r\n";
                            textBox2.Text += "结束时刻:" + MyDefine.myXET.memstop.ToString() + "\r\n";
                            textBox2.Text += "时间(秒):" + (MyDefine.myXET.memrun / 1000).ToString() + "\r\n";

                            //不存在则创建文件夹
                            if (!Directory.Exists(myPath))
                            {
                                Directory.CreateDirectory(myPath);
                            }

                            //不存在则创建文件
                            if (textBox1.TextLength > 0)
                            {
                                myName = myPath + "\\" + MyDefine.myXET.memdate.ToString() + MyDefine.myXET.memtime.ToString() + "_" + MyDefine.myXET.memrun.ToString() + "_" + textBox1.Text + ".tmp";
                                myExcel = myPath + "\\" + MyDefine.myXET.memdate.ToString() + MyDefine.myXET.memtime.ToString() + "_" + MyDefine.myXET.memrun.ToString() + "_" + textBox1.Text + ".csv";
                            }
                            else
                            {
                                myName = myPath + "\\" + MyDefine.myXET.memdate.ToString() + MyDefine.myXET.memtime.ToString() + "_" + MyDefine.myXET.memrun.ToString() + ".tmp";
                                myExcel = myPath + "\\" + MyDefine.myXET.memdate.ToString() + MyDefine.myXET.memtime.ToString() + "_" + MyDefine.myXET.memrun.ToString() + ".csv";
                            }

                            //保存
                            textBox2.Text += "正在保存到数据文件...\r\n";
                            MyDefine.myXET.mem_SaveToLog(myName);
                            MyDefine.myXET.mem_SaveToExcel(myExcel);
                            textBox2.Text += "保存完成\r\n";
                            MyDefine.myXET.myTask = TASKS.run;
                            MyDefine.myXET.old_index = 0;
                            MyDefine.myXET.count = 0;
                        }
                    }
                    else
                    {
                        switch (MyDefine.myXET.rtCOM)
                        {
                            default:
                                break;
                            case RTCOM.COM_ERR_MEM:
                                button2.Text = "无 存 储 器";
                                button2.BackColor = Color.Green;
                                textBox2.Text += "无存储器\r\n";
                                progressBar1.Value = progressBar1.Maximum;
                                MyDefine.myXET.myTask = TASKS.run;
                                break;
                            case RTCOM.COM_READ_MEM:
                                if (total < 131072)
                                {
                                    total++;
                                    if (MyDefine.myXET.memtotal > 0)
                                    {
                                        if (total < MyDefine.myXET.memtotal)
                                        {
                                            progressBar1.Value = progressBar1.Maximum * total / MyDefine.myXET.memtotal;
                                        }
                                    }
                                    else
                                    {
                                        progressBar1.Value = progressBar1.Maximum * total / 131072;
                                    }
                                }
                                else
                                {
                                    progressBar1.Value = progressBar1.Maximum;
                                }
                                MyDefine.myXET.mePort_Send_ReadMem(0x81);
                                break;
                        }
                    }
                }
            }
        }

        //
        #endregion
        //

        //
        private void MenuReadForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void MenuReadForm_Load(object sender, EventArgs e)
        {
            //加载接收触发
            MyDefine.myXET.myUpdate += new freshHandler(read_DataReceived);
            //
            MyDefine.myXET.rtCOM = RTCOM.COM_NULL;
        }

        private void MenuReadForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            timer1.Enabled = false;

            //取消更新LabelText事件
            MyDefine.myXET.myUpdate -= new freshHandler(read_DataReceived);
            //取消关闭事件
            this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.MenuReadForm_FormClosing);
            //
            MyDefine.myXET.rtCOM = RTCOM.COM_NULL;
        }
    }
}

//end

