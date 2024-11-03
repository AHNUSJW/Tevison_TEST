using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace TVS
{
    public partial class MenuCalFormTVS8 : Form
    {
        //
        private Byte mButton = 0; //标定按钮记录
        private TMP otp = new TMP(); //旧温度数据
        private UInt32[] num = new UInt32[SZ.CHA]; //温度稳定则计时
        private UInt32 mtick = 0; //总计时

        //
        public MenuCalFormTVS8()
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

        //input 线性检查
        private void input_Leave(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 0)
            {
                if (Convert.ToDouble(textBox1.Text) > 126.99)
                {
                    textBox1.BackColor = Color.Firebrick;
                }
                else
                {
                    textBox1.BackColor = SystemColors.Control;
                }
            }

            if (textBox2.TextLength > 0)
            {
                if (Convert.ToDouble(textBox2.Text) > 126.99)
                {
                    textBox2.BackColor = Color.Firebrick;
                }
                else
                {
                    textBox2.BackColor = SystemColors.Control;
                }
            }

            if (textBox3.TextLength > 0)
            {
                if (Convert.ToDouble(textBox3.Text) > 126.99)
                {
                    textBox3.BackColor = Color.Firebrick;
                }
                else
                {
                    textBox3.BackColor = SystemColors.Control;
                }
            }

            if (textBox4.TextLength > 0)
            {
                if (Convert.ToDouble(textBox4.Text) > 126.99)
                {
                    textBox4.BackColor = Color.Firebrick;
                }
                else
                {
                    textBox4.BackColor = SystemColors.Control;
                }
            }

            if (textBox5.TextLength > 0)
            {
                if (Convert.ToDouble(textBox5.Text) > 126.99)
                {
                    textBox5.BackColor = Color.Firebrick;
                }
                else
                {
                    textBox5.BackColor = SystemColors.Control;
                }
            }

            if (textBox6.TextLength > 0)
            {
                if (Convert.ToDouble(textBox6.Text) > 126.99)
                {
                    textBox6.BackColor = Color.Firebrick;
                }
                else
                {
                    textBox6.BackColor = SystemColors.Control;
                }
            }

            if (textBox7.TextLength > 0)
            {
                if (Convert.ToDouble(textBox7.Text) > 126.99)
                {
                    textBox7.BackColor = Color.Firebrick;
                }
                else
                {
                    textBox7.BackColor = SystemColors.Control;
                }
            }
        }

        //设置
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.TextLength > 0)
            {
                UInt16 tmp = (UInt16)(Convert.ToDouble(textBox1.Text) * 100.0f);

                mButton = 1;

                button1.BackColor = Color.Firebrick;

                MyDefine.myXET.mePort_Send_SetDot(DOT.DOT0, tmp);
            }
        }

        //设置
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.TextLength > 0)
            {
                UInt16 tmp = (UInt16)(Convert.ToDouble(textBox2.Text) * 100.0f);

                mButton = 2;

                button2.BackColor = Color.Firebrick;

                MyDefine.myXET.mePort_Send_SetDot(DOT.DOT1, tmp);
            }
        }

        //设置
        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox3.TextLength > 0)
            {
                UInt16 tmp = (UInt16)(Convert.ToDouble(textBox3.Text) * 100.0f);

                mButton = 3;

                button3.BackColor = Color.Firebrick;

                MyDefine.myXET.mePort_Send_SetDot(DOT.DOT2, tmp);
            }
        }

        //设置
        private void button4_Click(object sender, EventArgs e)
        {
            if (textBox4.TextLength > 0)
            {
                UInt16 tmp = (UInt16)(Convert.ToDouble(textBox4.Text) * 100.0f);

                mButton = 4;

                button4.BackColor = Color.Firebrick;

                MyDefine.myXET.mePort_Send_SetDot(DOT.DOT3, tmp);
            }
        }

        //设置
        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox5.TextLength > 0)
            {
                UInt16 tmp = (UInt16)(Convert.ToDouble(textBox5.Text) * 100.0f);

                mButton = 5;

                button5.BackColor = Color.Firebrick;

                MyDefine.myXET.mePort_Send_SetDot(DOT.DOT4, tmp);
            }
        }

        //设置
        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox6.TextLength > 0)
            {
                UInt16 tmp = (UInt16)(Convert.ToDouble(textBox6.Text) * 100.0f);

                mButton = 6;

                button6.BackColor = Color.Firebrick;

                MyDefine.myXET.mePort_Send_SetDot(DOT.DOT5, tmp);
            }
        }

        //设置
        private void button7_Click(object sender, EventArgs e)
        {
            if (textBox7.TextLength > 0)
            {
                UInt16 tmp = (UInt16)(Convert.ToDouble(textBox7.Text) * 100.0f);

                mButton = 7;

                button7.BackColor = Color.Firebrick;

                MyDefine.myXET.mePort_Send_SetDot(DOT.DOT6, tmp);
            }
        }

        //
        #region Realize component update between thread with delegate
        //

        //实际操作函数
        private void cal_DataReceived()
        {
            //其它线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(cal_DataReceived);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {
                    MyDefine.myXET.myUpdate -= new freshHandler(cal_DataReceived);
                }
            }
            //本线程的操作请求
            else
            {
                if (MyDefine.myXET.rtCOM == RTCOM.COM_NULL)
                {
                    switch (mButton)
                    {
                        default:
                            panel_tmp_display();
                            MyDefine.myXET.mePort_Send_ReadTmp();
                            break;
                        case 1:
                            panel_dot_display();
                            if (textBox1.TextLength > 0)
                            {
                                button1.BackColor = Color.Green;
                                mButton = 0;
                                MyDefine.myXET.mePort_Send_ReadTmp();
                            }
                            else
                            {
                                mButton = 2;
                                MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT1);
                            }
                            break;
                        case 2:
                            panel_dot_display();
                            if (textBox2.TextLength > 0)
                            {
                                button2.BackColor = Color.Green;
                                mButton = 0;
                                MyDefine.myXET.mePort_Send_ReadTmp();
                            }
                            else
                            {
                                mButton = 3;
                                MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT2);
                            }
                            break;
                        case 3:
                            panel_dot_display();
                            if (textBox3.TextLength > 0)
                            {
                                button3.BackColor = Color.Green;
                                mButton = 0;
                                MyDefine.myXET.mePort_Send_ReadTmp();
                            }
                            else
                            {
                                mButton = 4;
                                MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT3);
                            }
                            break;
                        case 4:
                            panel_dot_display();
                            if (textBox4.TextLength > 0)
                            {
                                button4.BackColor = Color.Green;
                                mButton = 0;
                                MyDefine.myXET.mePort_Send_ReadTmp();
                            }
                            else
                            {
                                mButton = 5;
                                MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT4);
                            }
                            break;
                        case 5:
                            panel_dot_display();
                            if (textBox5.TextLength > 0)
                            {
                                button5.BackColor = Color.Green;
                                mButton = 0;
                                MyDefine.myXET.mePort_Send_ReadTmp();
                            }
                            else
                            {
                                mButton = 6;
                                MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT5);
                            }
                            break;
                        case 6:
                            panel_dot_display();
                            if (textBox6.TextLength > 0)
                            {
                                button6.BackColor = Color.Green;
                                mButton = 0;
                                MyDefine.myXET.mePort_Send_ReadTmp();
                            }
                            else
                            {
                                mButton = 7;
                                MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT6);
                            }
                            break;
                        case 7:
                            panel_dot_display();
                            if (textBox7.TextLength > 0)
                            {
                                button7.BackColor = Color.Green;
                            }
                            else
                            {
                                mButton = 0;
                                MyDefine.myXET.mePort_Send_ReadTmp();
                            }
                            break;
                    }
                }
                else
                {
                    switch (MyDefine.myXET.rtCOM)
                    {
                        default:
                            break;

                        case RTCOM.COM_SET_DOT:
                            switch (mButton)
                            {
                                default:
                                    break;
                                case 1:
                                    button1_Click(null, null);
                                    break;
                                case 2:
                                    button2_Click(null, null);
                                    break;
                                case 3:
                                    button3_Click(null, null);
                                    break;
                                case 4:
                                    button4_Click(null, null);
                                    break;
                                case 5:
                                    button5_Click(null, null);
                                    break;
                                case 6:
                                    button6_Click(null, null);
                                    break;
                                case 7:
                                    button7_Click(null, null);
                                    break;
                            }
                            break;

                        case RTCOM.COM_READ_DOT:
                            switch (mButton)
                            {
                                default:
                                    break;
                                case 1:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT0);
                                    break;
                                case 2:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT1);
                                    break;
                                case 3:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT2);
                                    break;
                                case 4:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT3);
                                    break;
                                case 5:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT4);
                                    break;
                                case 6:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT5);
                                    break;
                                case 7:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT6);
                                    break;
                            }
                            break;

                        case RTCOM.COM_READ_TMP:
                            MyDefine.myXET.mePort_Send_ReadTmp();
                            break;
                    }
                }
            }
        }

        //
        #endregion
        //

        //
        private void panel_dot_display()
        {
            switch (mButton)
            {
                default:
                    break;
                case 1:
                    label23.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[0]) + "℃   " + MyDefine.myXET.A10[0].ToString() + "   " + MyDefine.myXET.V10[0].ToString();
                    label24.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[1]) + "℃   " + MyDefine.myXET.A10[1].ToString() + "   " + MyDefine.myXET.V10[1].ToString();
                    label25.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[2]) + "℃   " + MyDefine.myXET.A10[2].ToString() + "   " + MyDefine.myXET.V10[2].ToString();
                    label26.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[3]) + "℃   " + MyDefine.myXET.A10[3].ToString() + "   " + MyDefine.myXET.V10[3].ToString();
                    label27.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[4]) + "℃   " + MyDefine.myXET.A10[4].ToString() + "   " + MyDefine.myXET.V10[4].ToString();
                    label28.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[5]) + "℃   " + MyDefine.myXET.A10[5].ToString() + "   " + MyDefine.myXET.V10[5].ToString();
                    label29.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[6]) + "℃   " + MyDefine.myXET.A10[6].ToString() + "   " + MyDefine.myXET.V10[6].ToString();
                    label30.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[7]) + "℃   " + MyDefine.myXET.A10[7].ToString() + "   " + MyDefine.myXET.V10[7].ToString();
                    label31.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[8]) + "℃   " + MyDefine.myXET.A10[8].ToString() + "   " + MyDefine.myXET.V10[8].ToString();
                    label32.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[9]) + "℃   " + MyDefine.myXET.A10[9].ToString() + "   " + MyDefine.myXET.V10[9].ToString();
                    label33.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[10]) + "℃   " + MyDefine.myXET.A10[10].ToString() + "   " + MyDefine.myXET.V10[10].ToString();
                    label34.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[11]) + "℃   " + MyDefine.myXET.A10[11].ToString() + "   " + MyDefine.myXET.V10[11].ToString();
                    label35.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[12]) + "℃   " + MyDefine.myXET.A10[12].ToString() + "   " + MyDefine.myXET.V10[12].ToString();
                    label36.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[13]) + "℃   " + MyDefine.myXET.A10[13].ToString() + "   " + MyDefine.myXET.V10[13].ToString();
                    label37.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T10[14]) + "℃   " + MyDefine.myXET.A10[14].ToString() + "   " + MyDefine.myXET.V10[14].ToString();
                    break;
                case 2:
                    label23.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[0]) + "℃   " + MyDefine.myXET.A30[0].ToString() + "   " + MyDefine.myXET.V30[0].ToString();
                    label24.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[1]) + "℃   " + MyDefine.myXET.A30[1].ToString() + "   " + MyDefine.myXET.V30[1].ToString();
                    label25.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[2]) + "℃   " + MyDefine.myXET.A30[2].ToString() + "   " + MyDefine.myXET.V30[2].ToString();
                    label26.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[3]) + "℃   " + MyDefine.myXET.A30[3].ToString() + "   " + MyDefine.myXET.V30[3].ToString();
                    label27.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[4]) + "℃   " + MyDefine.myXET.A30[4].ToString() + "   " + MyDefine.myXET.V30[4].ToString();
                    label28.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[5]) + "℃   " + MyDefine.myXET.A30[5].ToString() + "   " + MyDefine.myXET.V30[5].ToString();
                    label29.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[6]) + "℃   " + MyDefine.myXET.A30[6].ToString() + "   " + MyDefine.myXET.V30[6].ToString();
                    label30.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[7]) + "℃   " + MyDefine.myXET.A30[7].ToString() + "   " + MyDefine.myXET.V30[7].ToString();
                    label31.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[8]) + "℃   " + MyDefine.myXET.A30[8].ToString() + "   " + MyDefine.myXET.V30[8].ToString();
                    label32.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[9]) + "℃   " + MyDefine.myXET.A30[9].ToString() + "   " + MyDefine.myXET.V30[9].ToString();
                    label33.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[10]) + "℃   " + MyDefine.myXET.A30[10].ToString() + "   " + MyDefine.myXET.V30[10].ToString();
                    label34.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[11]) + "℃   " + MyDefine.myXET.A30[11].ToString() + "   " + MyDefine.myXET.V30[11].ToString();
                    label35.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[12]) + "℃   " + MyDefine.myXET.A30[12].ToString() + "   " + MyDefine.myXET.V30[12].ToString();
                    label36.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[13]) + "℃   " + MyDefine.myXET.A30[13].ToString() + "   " + MyDefine.myXET.V30[13].ToString();
                    label37.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T30[14]) + "℃   " + MyDefine.myXET.A30[14].ToString() + "   " + MyDefine.myXET.V30[14].ToString();
                    break;
                case 3:
                    label23.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[0]) + "℃   " + MyDefine.myXET.A50[0].ToString() + "   " + MyDefine.myXET.V50[0].ToString();
                    label24.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[1]) + "℃   " + MyDefine.myXET.A50[1].ToString() + "   " + MyDefine.myXET.V50[1].ToString();
                    label25.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[2]) + "℃   " + MyDefine.myXET.A50[2].ToString() + "   " + MyDefine.myXET.V50[2].ToString();
                    label26.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[3]) + "℃   " + MyDefine.myXET.A50[3].ToString() + "   " + MyDefine.myXET.V50[3].ToString();
                    label27.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[4]) + "℃   " + MyDefine.myXET.A50[4].ToString() + "   " + MyDefine.myXET.V50[4].ToString();
                    label28.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[5]) + "℃   " + MyDefine.myXET.A50[5].ToString() + "   " + MyDefine.myXET.V50[5].ToString();
                    label29.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[6]) + "℃   " + MyDefine.myXET.A50[6].ToString() + "   " + MyDefine.myXET.V50[6].ToString();
                    label30.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[7]) + "℃   " + MyDefine.myXET.A50[7].ToString() + "   " + MyDefine.myXET.V50[7].ToString();
                    label31.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[8]) + "℃   " + MyDefine.myXET.A50[8].ToString() + "   " + MyDefine.myXET.V50[8].ToString();
                    label32.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[9]) + "℃   " + MyDefine.myXET.A50[9].ToString() + "   " + MyDefine.myXET.V50[9].ToString();
                    label33.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[10]) + "℃   " + MyDefine.myXET.A50[10].ToString() + "   " + MyDefine.myXET.V50[10].ToString();
                    label34.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[11]) + "℃   " + MyDefine.myXET.A50[11].ToString() + "   " + MyDefine.myXET.V50[11].ToString();
                    label35.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[12]) + "℃   " + MyDefine.myXET.A50[12].ToString() + "   " + MyDefine.myXET.V50[12].ToString();
                    label36.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[13]) + "℃   " + MyDefine.myXET.A50[13].ToString() + "   " + MyDefine.myXET.V50[13].ToString();
                    label37.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T50[14]) + "℃   " + MyDefine.myXET.A50[14].ToString() + "   " + MyDefine.myXET.V50[14].ToString();
                    break;
                case 4:
                    label23.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[0]) + "℃   " + MyDefine.myXET.A60[0].ToString() + "   " + MyDefine.myXET.V60[0].ToString();
                    label24.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[1]) + "℃   " + MyDefine.myXET.A60[1].ToString() + "   " + MyDefine.myXET.V60[1].ToString();
                    label25.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[2]) + "℃   " + MyDefine.myXET.A60[2].ToString() + "   " + MyDefine.myXET.V60[2].ToString();
                    label26.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[3]) + "℃   " + MyDefine.myXET.A60[3].ToString() + "   " + MyDefine.myXET.V60[3].ToString();
                    label27.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[4]) + "℃   " + MyDefine.myXET.A60[4].ToString() + "   " + MyDefine.myXET.V60[4].ToString();
                    label28.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[5]) + "℃   " + MyDefine.myXET.A60[5].ToString() + "   " + MyDefine.myXET.V60[5].ToString();
                    label29.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[6]) + "℃   " + MyDefine.myXET.A60[6].ToString() + "   " + MyDefine.myXET.V60[6].ToString();
                    label30.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[7]) + "℃   " + MyDefine.myXET.A60[7].ToString() + "   " + MyDefine.myXET.V60[7].ToString();
                    label31.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[8]) + "℃   " + MyDefine.myXET.A60[8].ToString() + "   " + MyDefine.myXET.V60[8].ToString();
                    label32.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[9]) + "℃   " + MyDefine.myXET.A60[9].ToString() + "   " + MyDefine.myXET.V60[9].ToString();
                    label33.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[10]) + "℃   " + MyDefine.myXET.A60[10].ToString() + "   " + MyDefine.myXET.V60[10].ToString();
                    label34.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[11]) + "℃   " + MyDefine.myXET.A60[11].ToString() + "   " + MyDefine.myXET.V60[11].ToString();
                    label35.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[12]) + "℃   " + MyDefine.myXET.A60[12].ToString() + "   " + MyDefine.myXET.V60[12].ToString();
                    label36.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[13]) + "℃   " + MyDefine.myXET.A60[13].ToString() + "   " + MyDefine.myXET.V60[13].ToString();
                    label37.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T60[14]) + "℃   " + MyDefine.myXET.A60[14].ToString() + "   " + MyDefine.myXET.V60[14].ToString();
                    break;
                case 5:
                    label23.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[0]) + "℃   " + MyDefine.myXET.A70[0].ToString() + "   " + MyDefine.myXET.V70[0].ToString();
                    label24.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[1]) + "℃   " + MyDefine.myXET.A70[1].ToString() + "   " + MyDefine.myXET.V70[1].ToString();
                    label25.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[2]) + "℃   " + MyDefine.myXET.A70[2].ToString() + "   " + MyDefine.myXET.V70[2].ToString();
                    label26.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[3]) + "℃   " + MyDefine.myXET.A70[3].ToString() + "   " + MyDefine.myXET.V70[3].ToString();
                    label27.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[4]) + "℃   " + MyDefine.myXET.A70[4].ToString() + "   " + MyDefine.myXET.V70[4].ToString();
                    label28.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[5]) + "℃   " + MyDefine.myXET.A70[5].ToString() + "   " + MyDefine.myXET.V70[5].ToString();
                    label29.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[6]) + "℃   " + MyDefine.myXET.A70[6].ToString() + "   " + MyDefine.myXET.V70[6].ToString();
                    label30.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[7]) + "℃   " + MyDefine.myXET.A70[7].ToString() + "   " + MyDefine.myXET.V70[7].ToString();
                    label31.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[8]) + "℃   " + MyDefine.myXET.A70[8].ToString() + "   " + MyDefine.myXET.V70[8].ToString();
                    label32.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[9]) + "℃   " + MyDefine.myXET.A70[9].ToString() + "   " + MyDefine.myXET.V70[9].ToString();
                    label33.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[10]) + "℃   " + MyDefine.myXET.A70[10].ToString() + "   " + MyDefine.myXET.V70[10].ToString();
                    label34.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[11]) + "℃   " + MyDefine.myXET.A70[11].ToString() + "   " + MyDefine.myXET.V70[11].ToString();
                    label35.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[12]) + "℃   " + MyDefine.myXET.A70[12].ToString() + "   " + MyDefine.myXET.V70[12].ToString();
                    label36.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[13]) + "℃   " + MyDefine.myXET.A70[13].ToString() + "   " + MyDefine.myXET.V70[13].ToString();
                    label37.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T70[14]) + "℃   " + MyDefine.myXET.A70[14].ToString() + "   " + MyDefine.myXET.V70[14].ToString();
                    break;
                case 6:
                    label23.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[0]) + "℃   " + MyDefine.myXET.A90[0].ToString() + "   " + MyDefine.myXET.V90[0].ToString();
                    label24.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[1]) + "℃   " + MyDefine.myXET.A90[1].ToString() + "   " + MyDefine.myXET.V90[1].ToString();
                    label25.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[2]) + "℃   " + MyDefine.myXET.A90[2].ToString() + "   " + MyDefine.myXET.V90[2].ToString();
                    label26.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[3]) + "℃   " + MyDefine.myXET.A90[3].ToString() + "   " + MyDefine.myXET.V90[3].ToString();
                    label27.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[4]) + "℃   " + MyDefine.myXET.A90[4].ToString() + "   " + MyDefine.myXET.V90[4].ToString();
                    label28.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[5]) + "℃   " + MyDefine.myXET.A90[5].ToString() + "   " + MyDefine.myXET.V90[5].ToString();
                    label29.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[6]) + "℃   " + MyDefine.myXET.A90[6].ToString() + "   " + MyDefine.myXET.V90[6].ToString();
                    label30.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[7]) + "℃   " + MyDefine.myXET.A90[7].ToString() + "   " + MyDefine.myXET.V90[7].ToString();
                    label31.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[8]) + "℃   " + MyDefine.myXET.A90[8].ToString() + "   " + MyDefine.myXET.V90[8].ToString();
                    label32.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[9]) + "℃   " + MyDefine.myXET.A90[9].ToString() + "   " + MyDefine.myXET.V90[9].ToString();
                    label33.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[10]) + "℃   " + MyDefine.myXET.A90[10].ToString() + "   " + MyDefine.myXET.V90[10].ToString();
                    label34.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[11]) + "℃   " + MyDefine.myXET.A90[11].ToString() + "   " + MyDefine.myXET.V90[11].ToString();
                    label35.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[12]) + "℃   " + MyDefine.myXET.A90[12].ToString() + "   " + MyDefine.myXET.V90[12].ToString();
                    label36.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[13]) + "℃   " + MyDefine.myXET.A90[13].ToString() + "   " + MyDefine.myXET.V90[13].ToString();
                    label37.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T90[14]) + "℃   " + MyDefine.myXET.A90[14].ToString() + "   " + MyDefine.myXET.V90[14].ToString();
                    break;
                case 7:
                    label23.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[0]) + "℃   " + MyDefine.myXET.A95[0].ToString() + "   " + MyDefine.myXET.V95[0].ToString();
                    label24.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[1]) + "℃   " + MyDefine.myXET.A95[1].ToString() + "   " + MyDefine.myXET.V95[1].ToString();
                    label25.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[2]) + "℃   " + MyDefine.myXET.A95[2].ToString() + "   " + MyDefine.myXET.V95[2].ToString();
                    label26.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[3]) + "℃   " + MyDefine.myXET.A95[3].ToString() + "   " + MyDefine.myXET.V95[3].ToString();
                    label27.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[4]) + "℃   " + MyDefine.myXET.A95[4].ToString() + "   " + MyDefine.myXET.V95[4].ToString();
                    label28.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[5]) + "℃   " + MyDefine.myXET.A95[5].ToString() + "   " + MyDefine.myXET.V95[5].ToString();
                    label29.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[6]) + "℃   " + MyDefine.myXET.A95[6].ToString() + "   " + MyDefine.myXET.V95[6].ToString();
                    label30.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[7]) + "℃   " + MyDefine.myXET.A95[7].ToString() + "   " + MyDefine.myXET.V95[7].ToString();
                    label31.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[8]) + "℃   " + MyDefine.myXET.A95[8].ToString() + "   " + MyDefine.myXET.V95[8].ToString();
                    label32.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[9]) + "℃   " + MyDefine.myXET.A95[9].ToString() + "   " + MyDefine.myXET.V95[9].ToString();
                    label33.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[10]) + "℃   " + MyDefine.myXET.A95[10].ToString() + "   " + MyDefine.myXET.V95[10].ToString();
                    label34.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[11]) + "℃   " + MyDefine.myXET.A95[11].ToString() + "   " + MyDefine.myXET.V95[11].ToString();
                    label35.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[12]) + "℃   " + MyDefine.myXET.A95[12].ToString() + "   " + MyDefine.myXET.V95[12].ToString();
                    label36.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[13]) + "℃   " + MyDefine.myXET.A95[13].ToString() + "   " + MyDefine.myXET.V95[13].ToString();
                    label37.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.T95[14]) + "℃   " + MyDefine.myXET.A95[14].ToString() + "   " + MyDefine.myXET.V95[14].ToString();
                    break;
            }

            mButton = 0;
        }

        //
        private void panel_tmp_display()
        {
            label8.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[0]);
            label9.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[1]);
            label10.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[2]);
            label11.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[3]);
            label12.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[4]);
            label13.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[5]);
            label14.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[6]);
            label15.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[7]);
            label16.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[8]);
            label17.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[9]);
            label18.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[10]);
            label19.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[11]);
            label20.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[12]);
            label21.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[13]);
            label22.Text = MyDefine.myXET.GetTemp(MyDefine.myXET.mtp.OUT[14]);

            int[] OutTVS8 = new int[8];
            Array.Copy(MyDefine.myXET.mtp.OUT, OutTVS8, 8);
            label39.Text = "avg " + MyDefine.myXET.GetTemp((int)OutTVS8.Average());
        }

        //
        private void timer1_Tick(object sender, EventArgs e)
        {
            mtick++;

            label38.Text = "tick " + mtick.ToString();

            for (Byte i = 0; i < SZ.CHA; i++)
            {
                if (otp.OUT[i] != MyDefine.myXET.mtp.OUT[i])
                {
                    mtick = 0;
                    num[i] = 0;
                    otp.OUT[i] = MyDefine.myXET.mtp.OUT[i];
                }
                else
                {
                    num[i]++;
                }
            }

            if (num[0] > 60)
            {
                label8.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label8.BackColor = SystemColors.Control;
            }

            if (num[1] > 60)
            {
                label9.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label9.BackColor = SystemColors.Control;
            }

            if (num[2] > 60)
            {
                label10.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label10.BackColor = SystemColors.Control;
            }

            if (num[3] > 60)
            {
                label11.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label11.BackColor = SystemColors.Control;
            }

            if (num[4] > 60)
            {
                label12.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label12.BackColor = SystemColors.Control;
            }

            if (num[5] > 60)
            {
                label13.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label13.BackColor = SystemColors.Control;
            }

            if (num[6] > 60)
            {
                label14.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label14.BackColor = SystemColors.Control;
            }

            if (num[7] > 60)
            {
                label15.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label15.BackColor = SystemColors.Control;
            }

            if (num[8] > 60)
            {
                label16.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label16.BackColor = SystemColors.Control;
            }

            if (num[9] > 60)
            {
                label17.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label17.BackColor = SystemColors.Control;
            }

            if (num[10] > 60)
            {
                label18.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label18.BackColor = SystemColors.Control;
            }

            if (num[11] > 60)
            {
                label19.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label19.BackColor = SystemColors.Control;
            }

            if (num[12] > 60)
            {
                label20.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label20.BackColor = SystemColors.Control;
            }

            if (num[13] > 60)
            {
                label21.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label21.BackColor = SystemColors.Control;
            }

            if (num[14] > 60)
            {
                label22.BackColor = Color.LightSteelBlue;
            }
            else
            {
                label22.BackColor = SystemColors.Control;
            }

            if (mtick > 60)
            {
                label38.BackColor = Color.LightGreen;
                label39.BackColor = Color.LightGreen;
            }
            else
            {
                label38.BackColor = SystemColors.Control;
                label39.BackColor = SystemColors.Control;
            }
        }

        //
        private void MenuCalForm_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void MenuCalForm_Load(object sender, EventArgs e)
        {
            //加载接收触发
            MyDefine.myXET.myUpdate += new freshHandler(cal_DataReceived);

            //启动计时
            timer1.Enabled = true;

            //初始化
            for (Byte i = 0; i < SZ.CHA; i++)
            {
                otp.OUT[i] = 0;
                num[i] = 0;
            }

            //先读取标定参数DOT0-DOT6
            mButton = 1;
            MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT0);
        }

        private void MenuCalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //取消更新LabelText事件
            MyDefine.myXET.myUpdate -= new freshHandler(cal_DataReceived);
            //取消关闭事件
            this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.MenuCalForm_FormClosing);
            //
            timer1.Enabled = false;
            //
            MyDefine.myXET.rtCOM = RTCOM.COM_NULL;
        }
    }
}

//end

