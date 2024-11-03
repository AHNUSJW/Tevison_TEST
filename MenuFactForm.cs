using System;
using System.Drawing;
using System.Windows.Forms;

namespace TVS
{
    public partial class MenuFactForm : Form
    {
        //
        private Byte mButton = 0;//按钮记录用于处理对应的操作

        //
        public MenuFactForm()
        {
            InitializeComponent();
        }

        //设置
        private void button1_Click(object sender, EventArgs e)
        {
            mButton = 1;
            //
            button1.BackColor = Color.Firebrick;
            //
            MyDefine.myXET.mePort_Send_FlashLock();
        }

        //设置
        private void button2_Click(object sender, EventArgs e)
        {
            mButton = 2;
            //
            button2.BackColor = Color.Firebrick;
            //
            MyDefine.myXET.mePort_Send_FlashUnlock();
        }

        //设置
        private void button3_Click(object sender, EventArgs e)
        {
            mButton = 3;
            //
            button3.BackColor = Color.Firebrick;
            //
            MyDefine.myXET.mePort_Send_FlashReset();
            //
            button3.Text = "waiting";
        }

        //设置
        private void button4_Click(object sender, EventArgs e)
        {
            mButton = 4;
            //
            button4.BackColor = Color.Firebrick;
            //
            MyDefine.myXET.mePort_Send_SetSeri();
        }

        //
        #region Realize component update between thread with delegate
        //

        //实际操作函数
        private void fact_DataReceived()
        {
            //其它线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(fact_DataReceived);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {
                    MyDefine.myXET.myUpdate -= new freshHandler(fact_DataReceived);
                }
            }
            //本线程的操作请求
            else
            {
                if(MyDefine.myXET.rtCOM == RTCOM.COM_NULL)
                {
                    switch (mButton)
                    {
                        default:
                            break;
                        case 1:
                            button1.BackColor = Color.Green;
                            break;
                        case 2:
                            button2.BackColor = Color.Green;
                            MyDefine.myXET.mePort.Close();
                            break;
                        case 3:
                            button3.BackColor = Color.Green;
                            button3.Text = "RESET";
                            break;
                        case 4:
                            button4.BackColor = Color.Green;
                            break;
                    }
                }
                else
                {
                    switch (MyDefine.myXET.rtCOM)
                    {
                        default:
                            break;
                        case RTCOM.COM_SET_LOCK:
                            MyDefine.myXET.mePort_Send_FlashLock();
                            break;
                        case RTCOM.COM_SET_UNLOCK:
                            MyDefine.myXET.mePort_Send_FlashUnlock();
                            break;
                        case RTCOM.COM_SET_RESET:
                            MyDefine.myXET.mePort_Send_FlashReset();
                            break;
                        case RTCOM.COM_SET_SERI:
                            MyDefine.myXET.mePort_Send_SetSeri();
                            break;
                    }
                }
            }
        }

        //
        #endregion
        //

        //
        private void MenuFactForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void MenuFactForm_Load(object sender, EventArgs e)
        {
            //加载接收触发
            MyDefine.myXET.myUpdate += new freshHandler(fact_DataReceived);
            //
            MyDefine.myXET.rtCOM = RTCOM.COM_NULL;
        }

        private void MenuFactForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //取消更新LabelText事件
            MyDefine.myXET.myUpdate -= new freshHandler(fact_DataReceived);
            //取消关闭事件
            this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.MenuFactForm_FormClosing);
            //
            MyDefine.myXET.rtCOM = RTCOM.COM_NULL;
        }
    }
}

//end

