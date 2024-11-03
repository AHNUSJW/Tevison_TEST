using System;
using System.Drawing;
using System.Windows.Forms;

namespace TVS
{
    public partial class MenuCurveFormTVS8 : Form
    {
        //
        #region parameter

        //范围
        private Int32 gridX;
        private Int32 gridY;

        //X坐标基准用
        private Int32 adc_zero;//
        private Int32 adc_full;//
        private Int32 aix_zero;//小
        private Int32 aix_full;//大

        //Y坐标基准用
        private Int32 tmp_zero;//温度
        private Int32 tmp_full;//温度
        private Int32 aiy_zero;//大
        private Int32 aiy_full;//小

        Point pointA;//画直线的端点A
        Point pointB;//画直线的端点B
        Point start;//画直线的延伸端点
        Point stop;//画直线的延伸端点

        private bool[] enc = new bool[SZ.CHA];//曲线显示通道使能

        private Byte mdot = 0;//控制通讯顺序读数据

        private bool isDraw = false;//有数据再使能画图防止按钮点击崩溃

        //
        #endregion
        //

        public MenuCurveFormTVS8()
        {
            InitializeComponent();
        }

        //
        private void button1_Click(object sender, EventArgs e)
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

            button2.BackColor = Color.Green;
            button3.BackColor = Color.Green;
            button4.BackColor = Color.Green;
            button5.BackColor = Color.Green;
            button6.BackColor = Color.Green;
            button7.BackColor = Color.Green;
            button8.BackColor = Color.Green;
            button9.BackColor = Color.Green;
            button10.BackColor = Color.Green;
            button11.BackColor = Color.Green;
            button12.BackColor = Color.Green;
            button13.BackColor = Color.Green;
            button14.BackColor = Color.Green;
            button15.BackColor = Color.Green;
            button16.BackColor = Color.Green;

            pictureBoxScope_draw();
        }

        //
        private void button2_Click(object sender, EventArgs e)
        {
            if (enc[0])
            {
                enc[0] = false;
                button2.BackColor = SystemColors.Control;
            }
            else
            {
                enc[0] = true;
                button2.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button3_Click(object sender, EventArgs e)
        {
            if (enc[1])
            {
                enc[1] = false;
                button3.BackColor = SystemColors.Control;
            }
            else
            {
                enc[1] = true;
                button3.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button4_Click(object sender, EventArgs e)
        {
            if (enc[2])
            {
                enc[2] = false;
                button4.BackColor = SystemColors.Control;
            }
            else
            {
                enc[2] = true;
                button4.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button5_Click(object sender, EventArgs e)
        {
            if (enc[3])
            {
                enc[3] = false;
                button5.BackColor = SystemColors.Control;
            }
            else
            {
                enc[3] = true;
                button5.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button6_Click(object sender, EventArgs e)
        {
            if (enc[4])
            {
                enc[4] = false;
                button6.BackColor = SystemColors.Control;
            }
            else
            {
                enc[4] = true;
                button6.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button7_Click(object sender, EventArgs e)
        {
            if (enc[5])
            {
                enc[5] = false;
                button7.BackColor = SystemColors.Control;
            }
            else
            {
                enc[5] = true;
                button7.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button8_Click(object sender, EventArgs e)
        {
            if (enc[6])
            {
                enc[6] = false;
                button8.BackColor = SystemColors.Control;
            }
            else
            {
                enc[6] = true;
                button8.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button9_Click(object sender, EventArgs e)
        {
            if (enc[7])
            {
                enc[7] = false;
                button9.BackColor = SystemColors.Control;
            }
            else
            {
                enc[7] = true;
                button9.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button10_Click(object sender, EventArgs e)
        {
            if (enc[8])
            {
                enc[8] = false;
                button10.BackColor = SystemColors.Control;
            }
            else
            {
                enc[8] = true;
                button10.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button11_Click(object sender, EventArgs e)
        {
            if (enc[9])
            {
                enc[9] = false;
                button11.BackColor = SystemColors.Control;
            }
            else
            {
                enc[9] = true;
                button11.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button12_Click(object sender, EventArgs e)
        {
            if (enc[10])
            {
                enc[10] = false;
                button12.BackColor = SystemColors.Control;
            }
            else
            {
                enc[10] = true;
                button12.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button13_Click(object sender, EventArgs e)
        {
            if (enc[11])
            {
                enc[11] = false;
                button13.BackColor = SystemColors.Control;
            }
            else
            {
                enc[11] = true;
                button13.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button14_Click(object sender, EventArgs e)
        {
            if (enc[12])
            {
                enc[12] = false;
                button14.BackColor = SystemColors.Control;
            }
            else
            {
                enc[12] = true;
                button14.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button15_Click(object sender, EventArgs e)
        {
            if (enc[13])
            {
                enc[13] = false;
                button15.BackColor = SystemColors.Control;
            }
            else
            {
                enc[13] = true;
                button15.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button16_Click(object sender, EventArgs e)
        {
            if (enc[14])
            {
                enc[14] = false;
                button16.BackColor = SystemColors.Control;
            }
            else
            {
                enc[14] = true;
                button16.BackColor = Color.Green;
            }

            pictureBoxScope_draw();
        }

        //
        private void button17_Click(object sender, EventArgs e)
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

            button2.BackColor = SystemColors.Control;
            button3.BackColor = SystemColors.Control;
            button4.BackColor = SystemColors.Control;
            button5.BackColor = SystemColors.Control;
            button6.BackColor = SystemColors.Control;
            button7.BackColor = SystemColors.Control;
            button8.BackColor = SystemColors.Control;
            button9.BackColor = SystemColors.Control;
            button10.BackColor = SystemColors.Control;
            button11.BackColor = SystemColors.Control;
            button12.BackColor = SystemColors.Control;
            button13.BackColor = SystemColors.Control;
            button14.BackColor = SystemColors.Control;
            button15.BackColor = SystemColors.Control;
            button16.BackColor = SystemColors.Control;

            pictureBoxScope_draw();
        }

        //
        private Int32 getValueAdc(UInt32 dat)
        {
            Int32 adc;

            adc = (Int32)dat;

            return (((adc - adc_zero) * (aix_full - aix_zero) / (adc_full - adc_zero)) + aix_zero);
        }

        //
        private Int32 getValueTmp(UInt16 tmp)
        {
            return (((tmp - tmp_zero) * (aiy_full - aiy_zero) / (tmp_full - tmp_zero)) + aiy_zero);
        }

        //
        private void pictureBoxScope_draw()
        {
            if (isDraw == false) return;

            Byte i;

            //层图
            Bitmap img = new Bitmap(pictureBoxScope.Width, pictureBoxScope.Height);

            //绘制
            Graphics g = Graphics.FromImage(img);

            for(i=0; i<SZ.CHA - 7; i++)
            {
                if(enc[i])
                {
                    //画点
                    g.DrawEllipse(new Pen(Color.Bisque, 6.0f), (getValueAdc(MyDefine.myXET.A10[i]) - 2), (getValueTmp(MyDefine.myXET.T10[i]) - 2), 4, 4);
                    g.DrawEllipse(new Pen(Color.Orange, 6.0f), (getValueAdc(MyDefine.myXET.A30[i]) - 2), (getValueTmp(MyDefine.myXET.T30[i]) - 2), 4, 4);
                    g.DrawEllipse(new Pen(Color.Orange, 6.0f), (getValueAdc(MyDefine.myXET.A50[i]) - 2), (getValueTmp(MyDefine.myXET.T50[i]) - 2), 4, 4);
                    g.DrawEllipse(new Pen(Color.Orange, 6.0f), (getValueAdc(MyDefine.myXET.A60[i]) - 2), (getValueTmp(MyDefine.myXET.T60[i]) - 2), 4, 4);
                    g.DrawEllipse(new Pen(Color.Orange, 6.0f), (getValueAdc(MyDefine.myXET.A70[i]) - 2), (getValueTmp(MyDefine.myXET.T70[i]) - 2), 4, 4);
                    g.DrawEllipse(new Pen(Color.Orange, 6.0f), (getValueAdc(MyDefine.myXET.A90[i]) - 2), (getValueTmp(MyDefine.myXET.T90[i]) - 2), 4, 4);
                    g.DrawEllipse(new Pen(Color.IndianRed, 6.0f), (getValueAdc(MyDefine.myXET.A95[i]) - 2), (getValueTmp(MyDefine.myXET.T95[i]) - 2), 4, 4);

                    //画前曲线
                    pointA.X = getValueAdc(MyDefine.myXET.A10[i]);
                    pointA.Y = getValueTmp(MyDefine.myXET.T10[i]);
                    pointB.X = getValueAdc(MyDefine.myXET.A30[i]);
                    pointB.Y = getValueTmp(MyDefine.myXET.T30[i]);
                    //
                    if ((pointA.X == pointB.X) || (pointA.Y == pointB.Y))
                    {
                        start.X = pointA.X;
                        start.Y = pointA.Y;
                        stop.X = pointB.X;
                        stop.Y = pointB.Y;
                    }
                    else if (pointA.X < pointB.X)
                    {
                        start.X = 20;
                        start.Y = pointA.Y - ((pointA.X - start.X) * (pointB.Y - pointA.Y) / (pointB.X - pointA.X));
                        if (start.Y < 20)
                        {
                            start.Y = 20;
                            start.X = pointA.X - ((pointA.Y - start.Y) * (pointB.X - pointA.X) / (pointB.Y - pointA.Y));
                        }
                        else if (start.Y > (pictureBoxScope.Height - 20))
                        {
                            start.Y = pictureBoxScope.Height - 20;
                            start.X = pointA.X - ((pointA.Y - start.Y) * (pointB.X - pointA.X) / (pointB.Y - pointA.Y));
                        }
                        g.DrawLine(new Pen(Color.Orange, 1.00f), start, pointA);
                    }
                    else
                    {
                        stop.X = pictureBoxScope.Width - 20;
                        stop.Y = pointA.Y - ((pointA.X - stop.X) * (pointB.Y - pointA.Y) / (pointB.X - pointA.X));
                        if (stop.Y < 20)
                        {
                            stop.Y = 20;
                            stop.X = pointA.X - ((pointA.Y - stop.Y) * (pointB.X - pointA.X) / (pointB.Y - pointA.Y));
                        }
                        else if (stop.Y > (pictureBoxScope.Height - 20))
                        {
                            stop.Y = pictureBoxScope.Height - 20;
                            stop.X = pointA.X - ((pointA.Y - stop.Y) * (pointB.X - pointA.X) / (pointB.Y - pointA.Y));
                        }
                        g.DrawLine(new Pen(Color.Orange, 1.00f), pointB, stop);
                    }

                    //画中间曲线
                    g.DrawLine(new Pen(Color.Orange, 1.00f), new Point(getValueAdc(MyDefine.myXET.A10[i]), getValueTmp(MyDefine.myXET.T10[i])), new Point(getValueAdc(MyDefine.myXET.A30[i]), getValueTmp(MyDefine.myXET.T30[i])));
                    g.DrawLine(new Pen(Color.Orange, 1.00f), new Point(getValueAdc(MyDefine.myXET.A30[i]), getValueTmp(MyDefine.myXET.T30[i])), new Point(getValueAdc(MyDefine.myXET.A50[i]), getValueTmp(MyDefine.myXET.T50[i])));
                    g.DrawLine(new Pen(Color.Orange, 1.00f), new Point(getValueAdc(MyDefine.myXET.A50[i]), getValueTmp(MyDefine.myXET.T50[i])), new Point(getValueAdc(MyDefine.myXET.A60[i]), getValueTmp(MyDefine.myXET.T60[i])));
                    g.DrawLine(new Pen(Color.Orange, 1.00f), new Point(getValueAdc(MyDefine.myXET.A60[i]), getValueTmp(MyDefine.myXET.T60[i])), new Point(getValueAdc(MyDefine.myXET.A70[i]), getValueTmp(MyDefine.myXET.T70[i])));
                    g.DrawLine(new Pen(Color.Orange, 1.00f), new Point(getValueAdc(MyDefine.myXET.A70[i]), getValueTmp(MyDefine.myXET.T70[i])), new Point(getValueAdc(MyDefine.myXET.A90[i]), getValueTmp(MyDefine.myXET.T90[i])));
                    g.DrawLine(new Pen(Color.Orange, 1.00f), new Point(getValueAdc(MyDefine.myXET.A90[i]), getValueTmp(MyDefine.myXET.T90[i])), new Point(getValueAdc(MyDefine.myXET.A95[i]), getValueTmp(MyDefine.myXET.T95[i])));

                    //画后曲线
                    pointA.X = getValueAdc(MyDefine.myXET.A90[i]);
                    pointA.Y = getValueTmp(MyDefine.myXET.T90[i]);
                    pointB.X = getValueAdc(MyDefine.myXET.A95[i]);
                    pointB.Y = getValueTmp(MyDefine.myXET.T95[i]);
                    //
                    if ((pointA.X == pointB.X) || (pointA.Y == pointB.Y))
                    {
                        start.X = pointA.X;
                        start.Y = pointA.Y;
                        stop.X = pointB.X;
                        stop.Y = pointB.Y;
                    }
                    else if (pointA.X < pointB.X)
                    {
                        stop.X = pictureBoxScope.Width - 20;
                        stop.Y = pointA.Y - ((pointA.X - stop.X) * (pointB.Y - pointA.Y) / (pointB.X - pointA.X));
                        if (stop.Y < 20)
                        {
                            stop.Y = 20;
                            stop.X = pointA.X - ((pointA.Y - stop.Y) * (pointB.X - pointA.X) / (pointB.Y - pointA.Y));
                        }
                        else if (stop.Y > (pictureBoxScope.Height - 20))
                        {
                            stop.Y = pictureBoxScope.Height - 20;
                            stop.X = pointA.X - ((pointA.Y - stop.Y) * (pointB.X - pointA.X) / (pointB.Y - pointA.Y));
                        }
                        g.DrawLine(new Pen(Color.Orange, 1.00f), pointB, stop);
                    }
                    else
                    {
                        start.X = 20;
                        start.Y = pointA.Y - ((pointA.X - start.X) * (pointB.Y - pointA.Y) / (pointB.X - pointA.X));
                        if (start.Y < 20)
                        {
                            start.Y = 20;
                            start.X = pointA.X - ((pointA.Y - start.Y) * (pointB.X - pointA.X) / (pointB.Y - pointA.Y));
                        }
                        else if (start.Y > (pictureBoxScope.Height - 20))
                        {
                            start.Y = pictureBoxScope.Height - 20;
                            start.X = pointA.X - ((pointA.Y - start.Y) * (pointB.X - pointA.X) / (pointB.Y - pointA.Y));
                        }
                        g.DrawLine(new Pen(Color.Orange, 1.00f), start, pointA);
                    }
                }
            }

            //铺图
            pictureBoxScope.Image = img;

            //
            g.Dispose();
        }

        //
        private void pictureBoxScope_setX()
        {
            UInt32 min = 0x80000;
            UInt32 max = 0;
            int gap = 0;

            //极值
            for (Byte i = 0; i < SZ.CHA - 7; i++)
            {
                if (MyDefine.myXET.A10[i] < min) min = MyDefine.myXET.A10[i];
                if (MyDefine.myXET.A30[i] < min) min = MyDefine.myXET.A30[i];
                if (MyDefine.myXET.A50[i] < min) min = MyDefine.myXET.A50[i];
                if (MyDefine.myXET.A60[i] < min) min = MyDefine.myXET.A60[i];
                if (MyDefine.myXET.A70[i] < min) min = MyDefine.myXET.A70[i];
                if (MyDefine.myXET.A90[i] < min) min = MyDefine.myXET.A90[i];
                if (MyDefine.myXET.A95[i] < min) min = MyDefine.myXET.A95[i];

                if (MyDefine.myXET.A10[i] > max) max = MyDefine.myXET.A10[i];
                if (MyDefine.myXET.A30[i] > max) max = MyDefine.myXET.A30[i];
                if (MyDefine.myXET.A50[i] > max) max = MyDefine.myXET.A50[i];
                if (MyDefine.myXET.A60[i] > max) max = MyDefine.myXET.A60[i];
                if (MyDefine.myXET.A70[i] > max) max = MyDefine.myXET.A70[i];
                if (MyDefine.myXET.A90[i] > max) max = MyDefine.myXET.A90[i];
                if (MyDefine.myXET.A95[i] > max) max = MyDefine.myXET.A95[i];
            }

            //范围10/95度调整为0/120度
            adc_zero = (int)(min - ((max - min) * 10 / 85));
            adc_full = (int)(min + ((max - min) * 110 / 85));

            //取整
            gap = (adc_full - adc_zero) / 20;
            adc_zero = adc_zero / 1000;
            adc_zero = adc_zero * 1000;
            adc_full = adc_zero + gap * 20;

            //范围检查
            if ((adc_full - adc_zero) < 20000)
            {
                adc_zero = ((adc_full + adc_zero) / 2) - 10000;
                adc_full = ((adc_full + adc_zero) / 2) + 10000;
            }

            //
            gridX = pictureBoxScope.Width / 21;
            aix_zero = gridX / 2;
            aix_full = aix_zero + (gridX * 20);
        }

        //
        private void pictureBoxScope_setY()
        {
            //固定0～120度
            tmp_zero = 0;
            tmp_full = 12000;

            //
            gridY = pictureBoxScope.Height / 13;
            aiy_zero = pictureBoxScope.Height - (gridY / 2);
            aiy_full = aiy_zero - (gridY * 12);
        }

        //
        private void pictureBoxScope_axis()
        {
            //
            int loop;

            //层图
            Bitmap img = new Bitmap(pictureBoxScope.Width, pictureBoxScope.Height);

            //绘制
            Graphics g = Graphics.FromImage(img);

            //aix_zero右边垂直线
            for (loop = aix_zero; loop < pictureBoxScope.Width; loop += gridX)
            {
                g.DrawLine(Pens.Black, new Point(loop, 0), new Point(loop, pictureBoxScope.Height));
            }

            //aix_zero左边垂直线
            for (loop = aix_zero; loop > 0; loop -= gridX)
            {
                g.DrawLine(Pens.Black, new Point(loop, 0), new Point(loop, pictureBoxScope.Height));
            }

            //aiy_zero下边水平线
            for (loop = aiy_zero; loop < pictureBoxScope.Height; loop += gridY)
            {
                g.DrawLine(Pens.Black, new Point(0, loop), new Point(pictureBoxScope.Width, loop));
            }

            //aiy_zero上边水平线
            for (loop = aiy_zero; loop > 0; loop -= gridY)
            {
                g.DrawLine(Pens.Black, new Point(0, loop), new Point(pictureBoxScope.Width, loop));
            }

            //粗基准0度120度
            g.DrawLine(new Pen(Color.Black, 3.0f), new Point(0, aiy_zero), new Point(pictureBoxScope.Width, aiy_zero));
            g.DrawLine(new Pen(Color.Black, 2.0f), new Point(0, aiy_full), new Point(pictureBoxScope.Width, aiy_full));

            //X坐标值
            g.DrawString(((adc_zero / 10000).ToString() + "w"), new Font("Arial", 14), Brushes.Black, aix_zero, (aiy_zero + 5));
            g.DrawString(((adc_full / 10000).ToString() + "w"), new Font("Arial", 14), Brushes.Black, aix_full, (aiy_zero + 5));

            //Y坐标值
            g.DrawString("120(℃)", new Font("Arial", 14), Brushes.Black, aix_zero, aiy_full);
            g.DrawString("110", new Font("Arial", 14), Brushes.Black, aix_zero, (aiy_full + gridY));
            g.DrawString("100", new Font("Arial", 14), Brushes.Black, aix_zero, (aiy_full + gridY * 2));
            g.DrawString("90", new Font("Arial", 14), Brushes.Black, aix_zero, (aiy_full + gridY * 3));
            g.DrawString("80", new Font("Arial", 14), Brushes.Black, aix_zero, (aiy_full + gridY * 4));
            g.DrawString("70", new Font("Arial", 14), Brushes.Black, aix_zero, (aiy_full + gridY * 5));
            g.DrawString("60", new Font("Arial", 14), Brushes.Black, aix_zero, (aiy_full + gridY * 6));
            g.DrawString("50", new Font("Arial", 14), Brushes.Black, aix_zero, (aiy_full + gridY * 7));
            g.DrawString("40", new Font("Arial", 14), Brushes.Black, aix_zero, (aiy_full + gridY * 8));
            g.DrawString("30", new Font("Arial", 14), Brushes.Black, aix_zero, (aiy_full + gridY * 9));
            g.DrawString("20", new Font("Arial", 14), Brushes.Black, aix_zero, (aiy_full + gridY * 10));
            g.DrawString("10", new Font("Arial", 14), Brushes.Black, aix_zero, (aiy_full + gridY * 11));

            //铺图
            pictureBoxScope.BackgroundImage = img;

            //
            g.Dispose();
        }

        //
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x112)
            {
                switch ((int)m.WParam)
                {
                    //禁止双击标题栏关闭窗体
                    //case 0xF063:
                    //case 0xF093:
                    //    m.WParam = IntPtr.Zero;
                    //    break;
                    //禁止拖拽标题栏还原窗体
                    //case 0xF012:
                    //case 0xF010:
                    //    m.WParam = IntPtr.Zero;
                    //    break;
                    //禁止双击标题栏
                    case 0xf122:
                        m.WParam = IntPtr.Zero;
                        break;
                    //禁止关闭按钮
                    //case 0xF060:
                    //    m.WParam = IntPtr.Zero;
                    //    break;
                    //禁止最大化按钮
                    //case 0xf020:
                    //    m.WParam = IntPtr.Zero;
                    //    break;
                    //禁止最小化按钮
                    //case 0xf030:
                    //    m.WParam = IntPtr.Zero;
                    //    break;
                    //禁止还原按钮
                    //case 0xf120:
                    //    m.WParam = IntPtr.Zero;
                    //    break;
                }
            }
            base.WndProc(ref m);
        }

        //实际操作函数
        private void curve_DataReceived()
        {
            //其它线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(curve_DataReceived);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {
                    MyDefine.myXET.myUpdate -= new freshHandler(curve_DataReceived);
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
                            isDraw = true;
                            pictureBoxScope_setX();
                            pictureBoxScope_setY();
                            pictureBoxScope_axis();
                            pictureBoxScope_draw();
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
                    }
                }
            }
        }

        //
        private void maximizeForm_move(object sender, EventArgs e)
        {
            //
            this.WindowState = FormWindowState.Maximized;
            this.BringToFront();
        }

        //
        private void MenuCurveForm_Load(object sender, EventArgs e)
        {
            //加载接收触发
            MyDefine.myXET.myUpdate += new freshHandler(curve_DataReceived);
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
            button2.BackColor = Color.Green;
            button3.BackColor = Color.Green;
            button4.BackColor = Color.Green;
            button5.BackColor = Color.Green;
            button6.BackColor = Color.Green;
            button7.BackColor = Color.Green;
            button8.BackColor = Color.Green;
            button9.BackColor = Color.Green;
            button10.BackColor = Color.Green;
            button11.BackColor = Color.Green;
            button12.BackColor = Color.Green;
            button13.BackColor = Color.Green;
            button14.BackColor = Color.Green;
            button15.BackColor = Color.Green;
            button16.BackColor = Color.Green;
            //
            MyDefine.myXET.mePort_Send_ReadDot(DOT.DOT0);
        }

        //
        private void MenuCurveForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //取消更新LabelText事件
            MyDefine.myXET.myUpdate -= new freshHandler(curve_DataReceived);
            //
            MyDefine.myXET.rtCOM = RTCOM.COM_NULL;
        }
    }
}
