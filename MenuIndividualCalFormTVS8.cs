using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Text;
using System.Windows.Forms;

namespace TVS
{
    public partial class MenuIndividualCalFormTVS8 : Form
    {
        private Byte mdot = 0;//控制通讯顺序读数据

        //Item1- Item7:7个温度点, Point: X:adc Y:温度
        private List<Tuple<Point, Point, Point, Point, Point, Point, Point>> tempPointsList = new List<Tuple<Point, Point, Point, Point, Point, Point, Point>>();
        //当前选择的探头 探头1-15  =20时,全部
        int nowPoint = -1;
        //15个探头校准温度
        private List<int> calTemp = new List<int>() { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        //15个探头原对应曲线温度
        private List<int> calOriginalTemp = new List<int>();

        public MenuIndividualCalFormTVS8()
        {
            InitializeComponent();
        }

        public MenuIndividualCalFormTVS8(List<string> dataList)
        {
            InitializeComponent();

            button1.Text = "A1  " + dataList[0];
            button2.Text = "A4  " + dataList[1];
            button3.Text = "A7  " + dataList[2];
            button4.Text = "A10 " + dataList[3];
            button5.Text = "A12 " + dataList[4];
            button6.Text = "D1  " + dataList[5];
            button7.Text = "D7  " + dataList[6];
            button8.Text = "D12 " + dataList[7];
            button9.Text = "E4  " + dataList[8];
            button10.Text = "E10 " + dataList[9];
            button11.Text = "H1  " + dataList[10];
            button12.Text = "H4  " + dataList[11];
            button13.Text = "H7  " + dataList[12];
            button14.Text = "H10 " + dataList[13];
            button15.Text = "H12 " + dataList[14];

            //曲线上点击的点的探头1-15的温度
            calOriginalTemp = new List<int>(){
                (UInt16)(Convert.ToDouble(dataList[0]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[1]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[2]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[3]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[4]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[5]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[6]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[7]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[8]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[9]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[10]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[11]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[12]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[13]) * 100.0f),
                (UInt16)(Convert.ToDouble(dataList[14]) * 100.0f)};
        }

        //页面加载
        private void MenuIndividualCalForm_Load(object sender, EventArgs e)
        {
            //检验产品序列号
            string dataId = MyDefine.myXET.e_sidm_dat.ToString("X8") + MyDefine.myXET.e_sidl_dat.ToString("X8") + MyDefine.myXET.e_sidh_dat.ToString("X8");   //当前数据的序列号
            if (!dataId.Equals(MyDefine.myXET.e_connetid))
            {
                MessageBox.Show("产品序列号不一致", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }

            //加载接收触发
            MyDefine.myXET.myUpdate += new freshHandler(cal_DataReceived);

            MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT0);

            for (int i = 0; i < MyDefine.myXET.A10.Length; i++)
            {
                Point point10 = new Point((Int32)MyDefine.myXET.A10[i], MyDefine.myXET.T10[i]);
                Point point30 = new Point((Int32)MyDefine.myXET.A30[i], MyDefine.myXET.T30[i]);
                Point point50 = new Point((Int32)MyDefine.myXET.A50[i], MyDefine.myXET.T50[i]);
                Point point60 = new Point((Int32)MyDefine.myXET.A60[i], MyDefine.myXET.T60[i]);
                Point point70 = new Point((Int32)MyDefine.myXET.A70[i], MyDefine.myXET.T70[i]);
                Point point90 = new Point((Int32)MyDefine.myXET.A90[i], MyDefine.myXET.T90[i]);
                Point point95 = new Point((Int32)MyDefine.myXET.A95[i], MyDefine.myXET.T95[i]);

                Tuple<Point, Point, Point, Point, Point, Point, Point> tuple = new Tuple<Point, Point, Point, Point, Point, Point, Point>(point10, point30, point50, point60, point70, point90, point95);
                tempPointsList.Add(tuple);
            }
        }

        //关闭界面
        private void MenuIndividualCalForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //取消更新LabelText事件
            MyDefine.myXET.myUpdate -= new freshHandler(cal_DataReceived);
            //取消关闭事件
            this.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(this.MenuIndividualCalForm_FormClosing);

            MyDefine.myXET.rtCOM = RTCOM.COM_NULL;
        }

        //数据输入
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
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

        //检测是否数字
        private void textBox1_KeyUp(object sender, KeyEventArgs e)
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
        }

        //更新15个点校准温度list
        private void update_CalTemp()
        {
            string buttonName = "button" + nowPoint;
            Button button = Controls.Find(buttonName, true).FirstOrDefault() as Button;
            if (textBox1.Text.Equals(""))
            {
                calTemp[nowPoint - 1] = -1;
                button.BackColor = SystemColors.ControlLight;
            }
            else
            {
                calTemp[nowPoint - 1] = (UInt16)(Convert.ToDouble(textBox1.Text) * 100.0f);
                button.BackColor = Color.MediumAquamarine;
            }
        }

        #region 探头按键

        private void button1_Click(object sender, EventArgs e)
        {
            nowPoint = 1;
            update_CalTemp();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            nowPoint = 2;
            update_CalTemp();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            nowPoint = 3;
            update_CalTemp();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            nowPoint = 4;
            update_CalTemp();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            nowPoint = 5;
            update_CalTemp();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            nowPoint = 6;
            update_CalTemp();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            nowPoint = 7;
            update_CalTemp();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            nowPoint = 8;
            update_CalTemp();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            nowPoint = 9;
            update_CalTemp();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            nowPoint = 10;
            update_CalTemp();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            nowPoint = 11;
            update_CalTemp();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            nowPoint = 12;
            update_CalTemp();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            nowPoint = 13;
            update_CalTemp();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            nowPoint = 14;
            update_CalTemp();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            nowPoint = 15;
            update_CalTemp();
        }

        #endregion

        //全选按键
        private void buttonAll_Click(object sender, EventArgs e)
        {
            nowPoint = 20;

            //统一设置calTemp的15个探头值
            if (textBox1.Text.Equals(""))
            {
                calTemp.Clear();
                calTemp.AddRange(Enumerable.Repeat(-1, 15));

                //修改按键背景颜色
                for (int i = 1; i <= 15; i++)
                {
                    string buttonName = "button" + i;
                    Button button = Controls.Find(buttonName, true).FirstOrDefault() as Button;

                    if (button != null)
                    {
                        button.BackColor = SystemColors.ControlLight;
                    }
                }
            }
            else
            {
                //统一设置calTemp的15个探头值
                calTemp.Clear();
                calTemp.AddRange(Enumerable.Repeat((int)(Convert.ToDouble(textBox1.Text) * 100.0f), 15));

                //修改按键背景颜色
                for (int i = 1; i <= 15; i++)
                {
                    string buttonName = "button" + i;
                    Button button = Controls.Find(buttonName, true).FirstOrDefault() as Button;

                    if (button != null)
                    {
                        button.BackColor = Color.MediumAquamarine;
                    }
                }
            }
        }

        //标定按键
        private void buttonCal_Click(object sender, EventArgs e)
        {
            nowPoint = -1;

            List<int> closestPointIndexList = new List<int>() { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };   //15个探头分别是哪些温度点需要修改

            //找选择的探头原始温度最接近哪个点，在那一段，再更新tempPointsList
            for (int i = 0; i < calTemp.Count; i++)
            {
                //不为-1 需要标定
                if (calTemp[i] == -1) continue;

                buttonCal.BackColor = Color.Firebrick;

                //在对应的tempPointsList找最接近哪个点，在那一段
                Point[] points = new Point[] { tempPointsList[i].Item1, tempPointsList[i].Item2, tempPointsList[i].Item3, tempPointsList[i].Item4, tempPointsList[i].Item5, tempPointsList[i].Item6, tempPointsList[i].Item7 };   //7个点
                Point closestPoint = points[6];  //最接近的点
                int closestPointIndex = 6;      //最接近的点下标

                //标定温度小于原温度，两点Y值相同时取前一个点 
                for (int j = 6; j > -1; j--)
                {
                    if (Math.Abs(points[j].Y - calOriginalTemp[i]) < Math.Abs(closestPoint.Y - calOriginalTemp[i]))
                    {
                        closestPoint = points[j];
                        closestPointIndex = j;
                    }
                    //两点Y值相同时
                    else if ((Math.Abs(points[j].Y - calOriginalTemp[i]) == Math.Abs(closestPoint.Y - calOriginalTemp[i])) && j < 6)
                    {
                        //标定温度大于两点的温度
                        if (calTemp[i] > points[j].Y)
                        {
                            closestPoint = points[j + 1];
                            closestPointIndex = j + 1;
                        }
                        //标定温度小于等于两点的温度
                        else
                        {
                            closestPoint = points[j];
                            closestPointIndex = j;
                        }
                    }
                }

                //依据温度（y值）计算对应的adc（x值）
                //公式：x = x1 + (y - y1) * (x2 - x1) / (y2 - y1)
                Point calOriginalPoint = new Point(-1, calOriginalTemp[i]);    //被标定的点
                //在最接近的点的右侧
                if (calOriginalPoint.Y >= points[closestPointIndex].Y)
                {
                    if (closestPointIndex < 6)
                    {
                        if (points[closestPointIndex + 1].Y == points[closestPointIndex].Y)
                        {
                            calOriginalPoint.X = points[closestPointIndex].X;
                        }
                        else
                        {
                            calOriginalPoint.X = (int)(points[closestPointIndex].X + (calOriginalPoint.Y - points[closestPointIndex].Y) * (points[closestPointIndex + 1].X - points[closestPointIndex].X) / (points[closestPointIndex + 1].Y - points[closestPointIndex].Y));
                        }
                    }
                    else if (closestPointIndex == 6)
                    {
                        if (points[closestPointIndex].Y == points[closestPointIndex - 1].Y)
                        {
                            calOriginalPoint.X = points[closestPointIndex].X;
                        }
                        else
                        {
                            calOriginalPoint.X = (int)(points[closestPointIndex - 1].X + (calOriginalPoint.Y - points[closestPointIndex - 1].Y) * (points[closestPointIndex].X - points[closestPointIndex - 1].X) / (points[closestPointIndex].Y - points[closestPointIndex - 1].Y));
                        }
                    }
                }
                //在最接近的点的左侧
                else
                {
                    if (closestPointIndex > 0)
                    {
                        if (points[closestPointIndex].Y == points[closestPointIndex - 1].Y)
                        {

                            calOriginalPoint.X = points[closestPointIndex].X;
                        }
                        else
                        {
                            calOriginalPoint.X = (int)(points[closestPointIndex - 1].X + (calOriginalPoint.Y - points[closestPointIndex - 1].Y) * (points[closestPointIndex].X - points[closestPointIndex - 1].X) / (points[closestPointIndex].Y - points[closestPointIndex - 1].Y));
                        }
                    }
                    else if (closestPointIndex == 0)
                    {
                        if (points[closestPointIndex + 1].Y == points[closestPointIndex].Y)
                        {
                            calOriginalPoint.X = points[closestPointIndex].X;
                        }
                        else
                        {
                            calOriginalPoint.X = (int)(points[closestPointIndex].X + (calOriginalPoint.Y - points[closestPointIndex].Y) * (points[closestPointIndex + 1].X - points[closestPointIndex].X) / (points[closestPointIndex + 1].Y - points[closestPointIndex].Y));
                        }
                    }
                }

                Point calPoint = new Point(calOriginalPoint.X, calTemp[i]);    //标定温度点

                //将最接近的点修改为新温度和adc
                switch (closestPointIndex)
                {
                    case 0:
                        tempPointsList[i] = new Tuple<Point, Point, Point, Point, Point, Point, Point>(calPoint, tempPointsList[i].Item2, tempPointsList[i].Item3, tempPointsList[i].Item4, tempPointsList[i].Item5, tempPointsList[i].Item6, tempPointsList[i].Item7);
                        break;
                    case 1:
                        tempPointsList[i] = new Tuple<Point, Point, Point, Point, Point, Point, Point>(tempPointsList[i].Item1, calPoint, tempPointsList[i].Item3, tempPointsList[i].Item4, tempPointsList[i].Item5, tempPointsList[i].Item6, tempPointsList[i].Item7);
                        break;
                    case 2:
                        tempPointsList[i] = new Tuple<Point, Point, Point, Point, Point, Point, Point>(tempPointsList[i].Item1, tempPointsList[i].Item2, calPoint, tempPointsList[i].Item4, tempPointsList[i].Item5, tempPointsList[i].Item6, tempPointsList[i].Item7);
                        break;
                    case 3:
                        tempPointsList[i] = new Tuple<Point, Point, Point, Point, Point, Point, Point>(tempPointsList[i].Item1, tempPointsList[i].Item2, tempPointsList[i].Item3, calPoint, tempPointsList[i].Item5, tempPointsList[i].Item6, tempPointsList[i].Item7);
                        break;
                    case 4:
                        tempPointsList[i] = new Tuple<Point, Point, Point, Point, Point, Point, Point>(tempPointsList[i].Item1, tempPointsList[i].Item2, tempPointsList[i].Item3, tempPointsList[i].Item4, calPoint, tempPointsList[i].Item6, tempPointsList[i].Item7);
                        break;
                    case 5:
                        tempPointsList[i] = new Tuple<Point, Point, Point, Point, Point, Point, Point>(tempPointsList[i].Item1, tempPointsList[i].Item2, tempPointsList[i].Item3, tempPointsList[i].Item4, tempPointsList[i].Item5, calPoint, tempPointsList[i].Item7);
                        break;
                    case 6:
                        tempPointsList[i] = new Tuple<Point, Point, Point, Point, Point, Point, Point>(tempPointsList[i].Item1, tempPointsList[i].Item2, tempPointsList[i].Item3, tempPointsList[i].Item4, tempPointsList[i].Item5, tempPointsList[i].Item6, calPoint);
                        break;
                    default:
                        break;
                }

                closestPointIndexList[i] = closestPointIndex;
            }

            //修改MyDefine.myXET中的温度值和adc值
            for (int i = 0; i < closestPointIndexList.Count; i++)
            {
                //不为-1 需要标定
                if (calTemp[i] == -1) continue;

                switch (closestPointIndexList[i])
                {
                    case 0:
                        MyDefine.myXET.A10[i] = (uint)tempPointsList[i].Item1.X;
                        MyDefine.myXET.T10[i] = (ushort)tempPointsList[i].Item1.Y;
                        break;
                    case 1:
                        MyDefine.myXET.A30[i] = (uint)tempPointsList[i].Item2.X;
                        MyDefine.myXET.T30[i] = (ushort)tempPointsList[i].Item2.Y;
                        break;
                    case 2:
                        MyDefine.myXET.A50[i] = (uint)tempPointsList[i].Item3.X;
                        MyDefine.myXET.T50[i] = (ushort)tempPointsList[i].Item3.Y;
                        break;
                    case 3:
                        MyDefine.myXET.A60[i] = (uint)tempPointsList[i].Item4.X;
                        MyDefine.myXET.T60[i] = (ushort)tempPointsList[i].Item4.Y;
                        break;
                    case 4:
                        MyDefine.myXET.A70[i] = (uint)tempPointsList[i].Item5.X;
                        MyDefine.myXET.T70[i] = (ushort)tempPointsList[i].Item5.Y;
                        break;
                    case 5:
                        MyDefine.myXET.A90[i] = (uint)tempPointsList[i].Item6.X;
                        MyDefine.myXET.T90[i] = (ushort)tempPointsList[i].Item6.Y;
                        break;
                    case 6:
                        MyDefine.myXET.A95[i] = (uint)tempPointsList[i].Item7.X;
                        MyDefine.myXET.T95[i] = (ushort)tempPointsList[i].Item7.Y;
                        break;
                    default:
                        break;
                }
            }

            //发指令修改
            for (int i = 0; i < closestPointIndexList.Count; i++)
            {
                //不为-1 需要标定
                if (calTemp[i] == -1) continue;
                bool isComplete = false;

                switch (i)
                {
                    default:
                        break;
                    case 0:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE1);
                        break;
                    case 1:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE2);
                        break;
                    case 2:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE3);
                        break;
                    case 3:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE4);
                        break;
                    case 4:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE5);
                        break;
                    case 5:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE6);
                        break;
                    case 6:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE7);
                        break;
                    case 7:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE8);
                        break;
                    case 8:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE9);
                        break;
                    case 9:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE10);
                        break;
                    case 10:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE11);
                        break;
                    case 11:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE12);
                        break;
                    case 12:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE13);
                        break;
                    case 13:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE14);
                        break;
                    case 14:
                        isComplete = MyDefine.myXET.mePort_Send_SetPROBE(PROBE.PROBE15);
                        break;
                }
                ui_Finish_Cal(isComplete);
            }

            calTemp = new List<int>() { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
        }

        //cal_DataReceived委托
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
                    switch (mdot)
                    {
                        default:
                            break;
                        case 0:
                            mdot = 1;
                            MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT1);
                            break;
                        case 1:
                            mdot = 2;
                            MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT2);
                            break;
                        case 2:
                            mdot = 3;
                            MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT3);
                            break;
                        case 3:
                            mdot = 4;
                            MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT4);
                            break;
                        case 4:
                            mdot = 5;
                            MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT5);
                            break;
                        case 5:
                            mdot = 6;
                            MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT6);
                            break;
                        case 6:
                            mdot = 7;

                            //获取15×7个温度点的值
                            tempPointsList.Clear();
                            for (int i = 0; i < MyDefine.myXET.A10.Length; i++)
                            {
                                Point point10 = new Point((Int32)MyDefine.myXET.A10[i], MyDefine.myXET.T10[i]);
                                Point point30 = new Point((Int32)MyDefine.myXET.A30[i], MyDefine.myXET.T30[i]);
                                Point point50 = new Point((Int32)MyDefine.myXET.A50[i], MyDefine.myXET.T50[i]);
                                Point point60 = new Point((Int32)MyDefine.myXET.A60[i], MyDefine.myXET.T60[i]);
                                Point point70 = new Point((Int32)MyDefine.myXET.A70[i], MyDefine.myXET.T70[i]);
                                Point point90 = new Point((Int32)MyDefine.myXET.A90[i], MyDefine.myXET.T90[i]);
                                Point point95 = new Point((Int32)MyDefine.myXET.A95[i], MyDefine.myXET.T95[i]);

                                Tuple<Point, Point, Point, Point, Point, Point, Point> tuple = new Tuple<Point, Point, Point, Point, Point, Point, Point>(point10, point30, point50, point60, point70, point90, point95);
                                tempPointsList.Add(tuple);
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

                        case RTCOM.COM_READ_DOT:
                            switch (mdot)
                            {
                                default:
                                    break;
                                case 0:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT0);
                                    break;
                                case 1:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT1);
                                    break;
                                case 2:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT2);
                                    break;
                                case 3:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT3);
                                    break;
                                case 4:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT4);
                                    break;
                                case 5:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT5);
                                    break;
                                case 6:
                                    MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT6);
                                    break;
                            }
                            break;

                        case RTCOM.COM_SET_PROBE:
                            switch (nowPoint)
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
                                case 8:
                                    button8_Click(null, null);
                                    break;
                                case 9:
                                    button9_Click(null, null);
                                    break;
                                case 10:
                                    button10_Click(null, null);
                                    break;
                                case 11:
                                    button11_Click(null, null);
                                    break;
                                case 12:
                                    button12_Click(null, null);
                                    break;
                                case 13:
                                    button13_Click(null, null);
                                    break;
                                case 14:
                                    button14_Click(null, null);
                                    break;
                                case 15:
                                    button15_Click(null, null);
                                    break;
                                case 20:
                                    buttonAll_Click(null, null);
                                    break;
                                case -1:
                                    buttonCal_Click(null, null);
                                    break;
                            }
                            break;
                    }
                }
            }
        }

        //标定按下后
        private void ui_Finish_Cal(bool isComplete)
        {
            Refresh();
            if (isComplete)
            {
                buttonCal.BackColor = Color.Green;
                textBox1.Text = "";
            }
            else
            {
                buttonCal.BackColor = Color.Firebrick;
            }
        }
    }
}
