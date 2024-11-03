using System;
using System.Drawing;
using System.Windows.Forms;

namespace TVS
{
    public partial class MenuSetForm : Form
    {
        //
        public MenuSetForm()
        {
            InitializeComponent();
        }

        //数据输入
        private void data_KeyPress(object sender, KeyPressEventArgs e)
        {
            //只允许输入数字,负号,小数点和删除键
            if (((e.KeyChar < '0') || (e.KeyChar > '9')) && (e.KeyChar != '.') && (e.KeyChar != 8))
            {
                e.Handled = true;
                return;
            }

            //第一位不能为小数点
            if ((e.KeyChar == '.') && (((TextBox)sender).Text.Length == 0))
            {
                e.Handled = true;
                return;
            }

            //小数点只能输入一次
            if ((e.KeyChar == '.') && (((TextBox)sender).Text.IndexOf('.') != -1))
            {
                e.Handled = true;
                return;
            }

            //正数第一位是0,第二位必须为小数点
            if ((e.KeyChar != '.') && (e.KeyChar != 8) && (((TextBox)sender).Text == "0"))
            {
                e.Handled = true;
                return;
            }
        }

        //
        private void data_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.C))
            {
                try
                {
                    Convert.ToDecimal(((TextBox)sender).SelectedText.Trim()); //检查是否数字
                    Clipboard.SetText(((TextBox)sender).SelectedText.Trim()); //Ctrl+C 复制
                }
                catch (Exception)
                {
                    e.Handled = true;
                }
            }
            if (e.KeyData == (Keys.Control | Keys.V))
            {
                if (Clipboard.ContainsText())
                {
                    try
                    {
                        Decimal tmp = Convert.ToDecimal(Clipboard.GetText().Trim()); //检查是否数字
                        ((TextBox)sender).SelectedText = tmp.ToString(); //Ctrl+V 粘贴
                    }
                    catch (Exception)
                    {
                        e.Handled = true;
                    }
                }
            }
            if (e.KeyData == (Keys.Control | Keys.X))
            {
                if (Clipboard.ContainsText())
                {
                    try
                    {
                        ((TextBox)sender).Cut();
                    }
                    catch (Exception)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        //模式
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            float dat;

            button1.BackColor = SystemColors.Control;

            switch (comboBox1.SelectedIndex)
            {
                default:
                    break;
                case 0:
                    MyDefine.myXET.mode = TmpMode.NULL;
                    label1.Text = "未启动记录功能";
                    textBox1.Visible = false;
                    break;
                case 1:
                    MyDefine.myXET.mode = TmpMode.immediately;
                    label1.Text = "设置后立即开始记录";
                    textBox1.Visible = false;
                    break;
                case 2:
                    MyDefine.myXET.mode = TmpMode.waite;
                    label1.Text = "设置延时开始记录时间（秒）";
                    if(MyDefine.myXET.delay == 0)
                    {
                        textBox1.Text = "";
                    }
                    else
                    {
                        textBox1.Text = MyDefine.myXET.delay.ToString();
                    }
                    textBox1.Visible = true;
                    break;
                case 3:
                    MyDefine.myXET.mode = TmpMode.reach;
                    label1.Text = "启动记录的设定温度（范围1～120℃）";
                    if (MyDefine.myXET.value == 0)
                    {
                        textBox1.Text = "";
                    }
                    else
                    {
                        dat = (float)MyDefine.myXET.value / 100.0f;
                        textBox1.Text = dat.ToString();
                    }
                    textBox1.Visible = true;
                    break;
                case 4:
                    MyDefine.myXET.mode = TmpMode.threshold;
                    label1.Text = "启动记录的温度变化阈值（范围0～10℃）";
                    if (MyDefine.myXET.value == 0)
                    {
                        textBox1.Text = "";
                    }
                    else
                    {
                        dat = (float)MyDefine.myXET.value / 100.0f;
                        textBox1.Text = dat.ToString();
                    }
                    textBox1.Visible = true;
                    break;
                case 5:
                    MyDefine.myXET.mode = TmpMode.clear;
                    label1.Text = "清除设备记录的数据";
                    textBox1.Visible = false;
                    break;
            }
        }

        //设置
        private void button1_Click(object sender, EventArgs e)
        {
            //
            switch (MyDefine.myXET.mode)
            {
                default:
                    break;
                case TmpMode.immediately:
                    //重新开始
                    if ((MyDefine.myXET.devVersion > 0) && (MyDefine.myXET.devStatus > TmpMode.NULL))
                    {
                        if (DialogResult.OK != MessageBox.Show("设备工作中, 是否重新开始记录?", "提示", MessageBoxButtons.OKCancel))
                        {
                            return;
                        }
                    }
                    break;
                case TmpMode.waite:
                    if (textBox1.Text.Length == 0)
                    {
                        return;
                    }
                    //重新开始
                    MyDefine.myXET.delay = Convert.ToUInt32(textBox1.Text);
                    if ((MyDefine.myXET.devVersion > 0) && (MyDefine.myXET.devStatus > TmpMode.NULL))
                    {
                        if (DialogResult.OK != MessageBox.Show("设备工作中, 是否重新开始记录?", "提示", MessageBoxButtons.OKCancel))
                        {
                            return;
                        }
                    }
                    break;
                case TmpMode.reach:
                    if (textBox1.Text.Length == 0)
                    {
                        return;
                    }
                    MyDefine.myXET.value = (UInt16)(Convert.ToDouble(textBox1.Text) * 100.0 + 0.5);
                    if (MyDefine.myXET.value > SZ.TMAX)
                    {
                        MessageBox.Show("范围1～120℃");
                        return;
                    }
                    //重新开始
                    if ((MyDefine.myXET.devVersion > 0) && (MyDefine.myXET.devStatus > TmpMode.NULL))
                    {
                        if (DialogResult.OK != MessageBox.Show("设备工作中, 是否重新开始记录?", "提示", MessageBoxButtons.OKCancel))
                        {
                            return;
                        }
                    }
                    break;
                case TmpMode.threshold:
                    if (textBox1.Text.Length == 0)
                    {
                        return;
                    }
                    MyDefine.myXET.value = (UInt16)(Convert.ToDouble(textBox1.Text) * 100.0 + 0.5);
                    if (MyDefine.myXET.value > SZ.TMAX)
                    {
                        MessageBox.Show("范围1～120℃");
                        return;
                    }
                    //重新开始
                    if ((MyDefine.myXET.devVersion > 0) && (MyDefine.myXET.devStatus > TmpMode.NULL))
                    {
                        if (DialogResult.OK != MessageBox.Show("设备工作中, 是否重新开始记录?", "提示", MessageBoxButtons.OKCancel))
                        {
                            return;
                        }
                    }
                    break;
                case TmpMode.clear:
                    //
                    if ((MyDefine.myXET.devVersion > 0) && (MyDefine.myXET.devStatus > TmpMode.NULL))
                    {
                        if (DialogResult.OK != MessageBox.Show("设备工作中, 是否停止记录并清除数据?", "提示", MessageBoxButtons.OKCancel))
                        {
                            return;
                        }
                    }
                    //
                    button1.BackColor = Color.Firebrick;
                    //
                    MyDefine.myXET.myTask = TASKS.setting;
                    //
                    MyDefine.myXET.mePort_Send_ClearMem();
                    return;
            }

            //
            MyDefine.myXET.date = Convert.ToUInt32(System.DateTime.Now.ToString("yyMMdd"));
            MyDefine.myXET.time = Convert.ToUInt32(System.DateTime.Now.ToString("HHmmss"));

            //
            button1.BackColor = Color.Firebrick;
            //
            MyDefine.myXET.myTask = TASKS.setting;
            //
            MyDefine.myXET.mePort_Send_SetMem();
        }

        //
        #region Realize component update between thread with delegate
        //

        //实际操作函数
        private void set_DataReceived()
        {
            //其它线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(set_DataReceived);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {
                    MyDefine.myXET.myUpdate -= new freshHandler(set_DataReceived);
                }
            }
            //本线程的操作请求
            else
            {
                if (MyDefine.myXET.rtCOM == RTCOM.COM_NULL)
                {
                    button1.BackColor = Color.Green;
                    MyDefine.myXET.myTask = TASKS.run;
                }
                else
                {
                    switch (MyDefine.myXET.rtCOM)
                    {
                        default:
                            break;
                        case RTCOM.COM_ERR_MEM:
                            button1.Text = "无存储器";
                            button1.BackColor = Color.Firebrick;
                            break;
                        case RTCOM.COM_SET_MEM:
                            MyDefine.myXET.mePort_Send_SetMem();
                            break;
                        case RTCOM.COM_CLEAR_MEM:
                            MyDefine.myXET.mePort_Send_ClearMem();
                            break;
                    }
                }
            }
        }

        //
        #endregion
        //

        //
        private void MenuSetForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void MenuSetForm_Load(object sender, EventArgs e)
        {
            //
            if(MyDefine.myXET.devVersion > 0)
            {
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(new object[] {
                    "关闭记录",
                    "立即开始记录",
                    "延时开始记录",
                    "到达设定温度后开始记录",
                    "温度变化超过设定差值后开始记录",
                    "清除设备记录"});
            }
            else
            {
                comboBox1.Items.Clear();
                comboBox1.Items.AddRange(new object[] {
                    "关闭记录",
                    "立即开始记录",
                    "延时开始记录",
                    "到达设定温度后开始记录",
                    "温度变化超过设定差值后开始记录"});
            }

            //加载接收触发
            MyDefine.myXET.myUpdate += new freshHandler(set_DataReceived);
            //
            MyDefine.myXET.rtCOM = RTCOM.COM_NULL;
            //
            switch (MyDefine.myXET.mode)
            {
                default:
                case TmpMode.NULL:
                    comboBox1.SelectedIndex = 0;
                    break;
                case TmpMode.immediately:
                    comboBox1.SelectedIndex = 1;
                    break;
                case TmpMode.waite:
                    comboBox1.SelectedIndex = 2;
                    break;
                case TmpMode.reach:
                    comboBox1.SelectedIndex = 3;
                    break;
                case TmpMode.threshold:
                    comboBox1.SelectedIndex = 4;
                    break;
                case TmpMode.clear:
                    comboBox1.SelectedIndex = 5;
                    break;
            }
        }

        private void MenuSetForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //取消更新LabelText事件
            MyDefine.myXET.myUpdate -= new freshHandler(set_DataReceived);
            //取消关闭事件
            this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.MenuSetForm_FormClosing);
            //
            MyDefine.myXET.rtCOM = RTCOM.COM_NULL;
        }
    }
}

//end

