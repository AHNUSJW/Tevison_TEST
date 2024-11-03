using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;

namespace TVS
{
    public partial class MenuConnectForm : Form
    {
        //
        private String myCOM = "COM1";

        //
        #region set and get
        //

        public String MyCOM
        {
            set
            {
                myCOM = value;
            }
            get
            {
                return myCOM;
            }
        }

        //
        #endregion
        //

        //
        public MenuConnectForm()
        {
            InitializeComponent();
        }

        //更新串口名称
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //
            label1.Text = "串口：" + comboBox1.Text;
            //
            myCOM = comboBox1.Text;
        }

        //刷新
        private void button1_Click(object sender, EventArgs e)
        {
            //刷串口
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(SerialPort.GetPortNames());
            //无串口
            if (comboBox1.Items.Count == 0)
            {
                comboBox1.Text = null;
                label1.Text = "串口：";
                myCOM = null;
            }
            //有可用串口
            else
            {
                comboBox1.Text = MyDefine.myXET.mePort.PortName;
                if (comboBox1.SelectedIndex < 0)
                {
                    comboBox1.SelectedIndex = 0;
                }
                label1.Text = "串口：" + comboBox1.Text;
                myCOM = comboBox1.Text;
            }
        }

        //测试
        private void button2_Click(object sender, EventArgs e)
        {
            //可以测试串口
            if (myCOM != null)
            {
                //尝试打开串口
                if (MyDefine.myXET.mePort.IsOpen)
                {
                    MyDefine.myXET.mePort.Close();
                }

                //尝试打开串口
                try
                {
                    MyDefine.myXET.mePort.PortName = this.myCOM;
                    MyDefine.myXET.mePort.BaudRate = Convert.ToInt32(BAUT.B115200); //波特率固定
                    MyDefine.myXET.mePort.DataBits = Convert.ToInt32("8"); //数据位固定
                    MyDefine.myXET.mePort.StopBits = StopBits.One; //停止位固定
                    MyDefine.myXET.mePort.Parity = Parity.None; //校验位固定
                    MyDefine.myXET.mePort.ReceivedBytesThreshold = 1; //接收即通知
                    MyDefine.myXET.mePort.Open();
                }
                catch
                {
                    MyDefine.myXET.mePort.Close();
                    button1_Click(null, null);
                }

                //串口发送
                if (MyDefine.myXET.mePort.IsOpen == true)
                {
                    button2.BackColor = Color.Firebrick;
                    //
                    MyDefine.myXET.myTask = TASKS.setting;
                    //
                    MyDefine.myXET.mePort_Send_ReadTvs();
                }
            }
            else
            {
                //刷新
                button1_Click(null, null);
            }
        }

        //
        #region Realize component update between thread with delegate
        //

        //实际操作函数
        private void connect_DataReceived()
        {
            //其它线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(connect_DataReceived);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {
                    MyDefine.myXET.myUpdate -= new freshHandler(connect_DataReceived);
                }
            }
            //本线程的操作请求
            else
            {
                if(MyDefine.myXET.rtCOM == RTCOM.COM_NULL)
                {
                    float dat, p_dat;
                    button2.BackColor = Color.Green;
                    label5.Text = "产品序列号：" + MyDefine.myXET.e_sidm.ToString("X8") + MyDefine.myXET.e_sidl.ToString("X8") + MyDefine.myXET.e_sidh.ToString("X8");
                    MyDefine.myXET.e_connetid = MyDefine.myXET.e_sidm.ToString("X8") + MyDefine.myXET.e_sidl.ToString("X8") + MyDefine.myXET.e_sidh.ToString("X8");
                    label6.Text = "采集器批号：" + MyDefine.myXET.e_seri.ToString();
                    dat = ((3.3f * (float)MyDefine.myXET.battery / 4096.0f) / 2.0f * 3.0f + 0.08f);
                    if(dat >= 3.8f)
                    {
                        p_dat = 1;
                    }
                    else if(dat < 3.0f)
                    {
                        p_dat = 0;
                    }
                    else
                    {
                        p_dat = (dat - 3.0f) / 0.8f;
                    }
                    label7.Text = "锂电池电压：" + dat.ToString("f2").PadLeft(5) + "V    " + p_dat.ToString("p0");
                    dat = ((1.43f - (3.3f * (float)MyDefine.myXET.temperature / 4096.0f)) / 0.0043f + 25.0f);
                    label8.Text = "采集器温度：" + dat.ToString("f1");
                    if (MyDefine.myXET.devVersion > 0)
                    {
                        if(MyDefine.myXET.devStatus > TmpMode.NULL)
                        {
                            label8.Text += "℃    设备工作中";
                        }
                        else
                        {
                            label8.Text += "℃    设备待机";
                        }
                    }
                    MyDefine.myXET.myTask = TASKS.run;
                }
                else
                {
                    switch (MyDefine.myXET.rtCOM)
                    {
                        default:
                            break;
                        case RTCOM.COM_READ_TVS:
                            MyDefine.myXET.mePort_Send_ReadTvs(); 
                            break;
                        case RTCOM.COM_READ_PAR:
                            MyDefine.myXET.mePort_Send_ReadPar(); 
                            break;
                    }
                }
            }
        }

        //
        #endregion
        //

        //
        private void MenuConnectForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void MenuConnectForm_Load(object sender, EventArgs e)
        {
            //加载时执行一次刷新
            button1_Click(null, null);
            //加载接收触发
            MyDefine.myXET.myUpdate += new freshHandler(connect_DataReceived);
            //
            MyDefine.myXET.rtCOM = RTCOM.COM_NULL;
        }

        private void MenuConnectForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //取消更新LabelText事件
            MyDefine.myXET.myUpdate -= new freshHandler(connect_DataReceived);
            //取消关闭事件
            this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.MenuConnectForm_FormClosing);
            //
            MyDefine.myXET.rtCOM = RTCOM.COM_NULL;
        }
    }
}

//end

