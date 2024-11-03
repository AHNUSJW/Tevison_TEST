using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace TVS
{
    public partial class MenuFieldFormTVS8 : Form
    {
        public bool isOpen = false;

        private Color[] holeColor = {
            Color.FromArgb(255, 0, 0, 255),
            Color.FromArgb(255, 0, 125, 255),
            Color.FromArgb(255, 0, 255, 255),
            Color.FromArgb(255, 0, 255, 125),
            Color.FromArgb(255, 0, 200, 0),
            Color.FromArgb(255, 125, 255, 0),
            Color.FromArgb(255, 255, 255, 0),
            Color.FromArgb(255, 255, 125, 0),
            Color.FromArgb(255, 255, 0, 0), };

        private Color[][] fieldColor = new Color[9][];
        private Point[,] hole = new Point[12, 8]; //坐标
        private int wh;                          //孔占宽度
        private Byte[,] value = new Byte[12, 8];  //等级

        public MenuFieldFormTVS8()
        {
            InitializeComponent();
            fieldColor[0] = new Color[1] { holeColor[0] };
            fieldColor[1] = new Color[1] { holeColor[1] };
            fieldColor[2] = new Color[1] { holeColor[2] };
            fieldColor[3] = new Color[1] { holeColor[3] };
            fieldColor[4] = new Color[1] { holeColor[4] };
            fieldColor[5] = new Color[1] { holeColor[5] };
            fieldColor[6] = new Color[1] { holeColor[6] };
            fieldColor[7] = new Color[1] { holeColor[7] };
            fieldColor[8] = new Color[1] { holeColor[8] };
        }

        private void pictureBox_getholes()
        {
            int w = pictureBox1.Width / 12;
            int h = pictureBox1.Height / 8;
            int x = w / 2;
            int y = h / 2;

            wh = (int)(Math.Min(w, h) / 2.1);

            for (Byte k = 0; k < 8; k++)
            {
                x = w / 2;

                for (Byte i = 0; i < 12; i++)
                {
                    hole[i, k].X = x;
                    hole[i, k].Y = y;
                    x += w;
                }

                y += h;
            }
        }

        public void pictureBox_getvalue(TMP m, int avg)
        {
            int[,] v = new int[12, 8]; //96孔温度
            int dev;

            //
            v[0, 0] = m.OUT[0];
            v[1, 0] = m.OUT[1];
            v[2, 0] = m.OUT[2];
            v[3, 0] = m.OUT[3];
            v[4, 0] = m.OUT[4];
            v[5, 0] = m.OUT[5];
            v[6, 0] = m.OUT[6];
            v[7, 0] = m.OUT[7];

            //

            v[0, 0] = (v[0, 0] + v[1, 0]) / 2;
            v[1, 0] = (v[0, 0] + v[1, 0] + v[2, 0]) / 3;
            v[2, 0] = (v[1, 0] + v[2, 0] + v[3, 0]) / 3;
            v[3, 0] = (v[2, 0] + v[3, 0] + v[4, 0]) / 3;
            v[4, 0] = (v[3, 0] + v[4, 0] + v[5, 0]) / 3;
            v[5, 0] = (v[4, 0] + v[5, 0] + v[6, 0]) / 3;
            v[6, 0] = (v[5, 0] + v[6, 0] + v[7, 0]) / 3;
            v[7, 0] = (v[6, 0] + v[7, 0]) / 2;

            //
            for (Byte k = 0; k < 8; k++)
            {
                for (Byte i = 0; i < 12; i++)
                {
                    dev = v[i, k] - avg;

                    if (dev > 70)
                    {
                        value[i, k] = 8;
                    }
                    else if (dev > 50)
                    {
                        value[i, k] = 7;
                    }
                    else if (dev > 30)
                    {
                        value[i, k] = 6;
                    }
                    else if (dev > 10)
                    {
                        value[i, k] = 5;
                    }
                    else if (dev > -10)
                    {
                        value[i, k] = 4;
                    }
                    else if (dev > -30)
                    {
                        value[i, k] = 3;
                    }
                    else if (dev > -50)
                    {
                        value[i, k] = 2;
                    }
                    else if (dev > -70)
                    {
                        value[i, k] = 1;
                    }
                    else
                    {
                        value[i, k] = 0;
                    }
                }
            }

            //
            pictureBox_fielddraw();
            //pictureBox_fieldtest();
        }

        private void pictureBox_fieldtest()
        {
            //层图
            Bitmap img = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            //绘制
            Graphics g = Graphics.FromImage(img);

            //
            for (Byte k = 0; k < 8; k++)
            {
                for (Byte i = 0; i < 12; i++)
                {
                    g.DrawString(value[i, k].ToString(), new Font("Courier New", 12), Brushes.Blue, hole[i, k]);
                }
            }

            //铺图
            pictureBox1.Image = img;

            //
            g.Dispose();
        }

        private byte getAverage(byte i, byte k)
        {
            int a;
            int b;
            int sum = 0;
            int num = 0;

            //左边点
            a = i - 1;
            b = k;
            if (a >= 0)
            {
                sum += value[a, b];
                num++;
            }

            //上边点
            a = i;
            b = k - 1;
            if (b >= 0)
            {
                sum += value[a, b];
                num++;
            }

            //右边点
            a = i + 1;
            b = k;
            if (a < 12)
            {
                sum += value[a, b];
                num++;
            }

            //下边点
            a = i;
            b = k + 1;
            if (b < 8)
            {
                sum += value[a, b];
                num++;
            }

            return ((byte)(sum / num));
        }

        private void pictureBox_fielddraw()
        {
            //背景
            pictureBox1.BackColor = holeColor[4];

            //层图
            try
            {
                Bitmap img = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                //绘制
                Graphics g = Graphics.FromImage(img);

                //
                Point[] mp = new Point[4];
                GraphicsPath path = new GraphicsPath();
                PathGradientBrush pthGrBrush;

                //
                for (Byte k = 0; k < 1; k++)
                {
                    for (Byte i = 0; i < 8; i++)
                    {
                        //points
                        mp[0].X = mp[3].X = hole[i, k].X + wh;
                        mp[1].X = mp[2].X = hole[i, k].X - wh;
                        mp[0].Y = mp[1].Y = hole[i, k].Y - wh;
                        mp[2].Y = mp[3].Y = hole[i, k].Y + wh;

                        //curve
                        path.Reset();
                        path.AddClosedCurve(mp);

                        //brush
                        pthGrBrush = new PathGradientBrush(path);
                        pthGrBrush.CenterColor = holeColor[value[i, k]];
                        //pthGrBrush.SurroundColors = fieldColor[getAverage(i,k)];
                        pthGrBrush.SurroundColors = fieldColor[4];

                        //
                        g.FillClosedCurve(pthGrBrush, mp, FillMode.Winding, 0.1f);

                    }
                }

                for (Byte k = 0; k < 1; k++)
                {
                    for (Byte i = 0; i < 8; i++)
                    {
                        //points
                        mp[0].X = mp[3].X = hole[i, k].X + wh;
                        mp[1].X = mp[2].X = hole[i, k].X - wh;
                        mp[0].Y = mp[1].Y = hole[i, k].Y - wh;
                        mp[2].Y = mp[3].Y = hole[i, k].Y + wh;

                        string text = "";

                        if (k == 0)
                        {
                            switch (i)
                            {
                                case 0:
                                    text = "A1";
                                    pictureBox_DrawStr(g, mp[1].X, mp[0].Y, text);
                                    break;
                                case 1:
                                    text = "A4";
                                    pictureBox_DrawStr(g, mp[1].X, mp[0].Y, text);
                                    break;
                                case 2:
                                    text = "A7";
                                    pictureBox_DrawStr(g, mp[1].X, mp[0].Y, text);
                                    break;
                                case 3:
                                    text = "A10";
                                    pictureBox_DrawStr(g, mp[1].X, mp[0].Y, text);
                                    break;
                                case 4:
                                    text = "A12";
                                    pictureBox_DrawStr(g, mp[1].X, mp[0].Y, text);
                                    break;
                                case 5:
                                    text = "D1";
                                    pictureBox_DrawStr(g, mp[1].X, mp[0].Y, text);
                                    break;
                                case 6:
                                    text = "D7";
                                    pictureBox_DrawStr(g, mp[1].X, mp[0].Y, text);
                                    break;
                                case 7:
                                    text = "D12";
                                    pictureBox_DrawStr(g, mp[1].X, mp[0].Y, text);
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }

                //铺图
                pictureBox1.Image = img;

                //
                g.Dispose();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            catch { }

        }

        //画探头编号
        private void pictureBox_DrawStr(Graphics g, int pointX, int pointY, string text)
        {
            Font font = new Font("Arial", 9, FontStyle.Bold);
            Brush brush = Brushes.GreenYellow;
            PointF position = new PointF(pointX + 6, pointY + 9);

            g.DrawString(text, font, brush, position);
        }

        //窗口改变
        private void MenuFieldForm_Resized(object sender, EventArgs e)
        {
            double a;
            double b;
            double c;
            double y;

            if (Width < 400) Width = 400;
            if (Height < 300) Height = 300;

            a = 1.5;
            b = 84;
            c = 882 - (Width * Height);
            y = (Math.Sqrt(b * b - (4 * a * c)) - b) / 3;

            Height = (int)(y + 42);
            Width = (int)(y * 1.5 + 21);

            pictureBox_getholes();
        }

        //加载
        private void MenuFieldForm_Load(object sender, EventArgs e)
        {
            isOpen = true;
            pictureBox_getholes();
        }

        //退出
        private void MenuFieldForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            isOpen = false;
        }

    }
}
