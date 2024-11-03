using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace TVS
{
    public class Picture
    {
        public const int XW = 125;

        //温度坐标设置
        public int tmax;//坐标温度最大值
        public int tmin;//坐标温度最小值
        public int step;//坐标温度分度值
        public int div;//格

        //窗口大小
        public int Height;
        public int Width;

        //格点
        public int yh_grid;//Y格
        public int yh_full;//
        public int yh_zero;//
        public int xw_start;//
        public int xw_stop;//

        //文字坐标
        public int xw_text;

        //曲线颜色和文字颜色
        public Color tmp_outline;//外框
        public Color tmp_lines;//格线
        public Color tmp_curve_1;//曲线
        public Color tmp_curve_2;
        public Color tmp_curve_3;
        public Color tmp_curve_4;
        public Color tmp_curve_5;
        public Color tmp_curve_6;
        public Color tmp_curve_7;
        public Color tmp_curve_8;
        public Color tmp_curve_9;
        public Color tmp_curve_10;
        public Color tmp_curve_11;
        public Color tmp_curve_12;
        public Color tmp_curve_13;
        public Color tmp_curve_14;
        public Color tmp_curve_15;
        public Brush tmp_brush_1;
        public Brush tmp_brush_2;
        public Brush tmp_brush_3;
        public Brush tmp_brush_4;
        public Brush tmp_brush_5;
        public Brush tmp_brush_6;
        public Brush tmp_brush_7;
        public Brush tmp_brush_8;
        public Brush tmp_brush_9;
        public Brush tmp_brush_10;
        public Brush tmp_brush_11;
        public Brush tmp_brush_12;
        public Brush tmp_brush_13;
        public Brush tmp_brush_14;
        public Brush tmp_brush_15;

        //
        public List<Point> probe1 = new List<Point>();
        public List<Point> probe2 = new List<Point>();
        public List<Point> probe3 = new List<Point>();
        public List<Point> probe4 = new List<Point>();
        public List<Point> probe5 = new List<Point>();
        public List<Point> probe6 = new List<Point>();
        public List<Point> probe7 = new List<Point>();
        public List<Point> probe8 = new List<Point>();
        public List<Point> probe9 = new List<Point>();
        public List<Point> probe10 = new List<Point>();
        public List<Point> probe11 = new List<Point>();
        public List<Point> probe12 = new List<Point>();
        public List<Point> probe13 = new List<Point>();
        public List<Point> probe14 = new List<Point>();
        public List<Point> probe15 = new List<Point>();

        public int start;//probe从mySyn里面的索引
        public int stop;//probe从mySyn里面的索引

        public Picture()
        {
            tmax = 10000;
            tmin = 0;
            step = 500;
            div = (tmax - tmin) / step;

            Height = 768;
            Width = 1024;

            yh_grid = Height / div;
            yh_full = 0;
            yh_zero = div * yh_grid;

            xw_stop = Width - XW;
            xw_start = XW;

            xw_text = xw_stop + 5;

            tmp_outline = Color.DimGray;
            tmp_lines = Color.LightGray;

            tmp_curve_1 = Color.Crimson;
            tmp_curve_2 = Color.Firebrick;
            tmp_curve_3 = Color.MediumVioletRed;
            tmp_curve_4 = Color.DarkOrange;
            tmp_curve_5 = Color.Goldenrod;
            tmp_curve_6 = Color.SaddleBrown;
            tmp_curve_7 = Color.SeaGreen;
            tmp_curve_8 = Color.DarkGreen;
            tmp_curve_9 = Color.Blue;
            tmp_curve_10 = Color.Navy;
            tmp_curve_11 = Color.DarkMagenta;
            tmp_curve_12 = Color.Indigo;
            tmp_curve_13 = Color.Purple;
            tmp_curve_14 = Color.DarkSlateGray;
            tmp_curve_15 = Color.Black;

            tmp_brush_1 = Brushes.Crimson;
            tmp_brush_2 = Brushes.Firebrick;
            tmp_brush_3 = Brushes.MediumVioletRed;
            tmp_brush_4 = Brushes.DarkOrange;
            tmp_brush_5 = Brushes.Goldenrod;
            tmp_brush_6 = Brushes.SaddleBrown;
            tmp_brush_7 = Brushes.SeaGreen;
            tmp_brush_8 = Brushes.DarkGreen;
            tmp_brush_9 = Brushes.Blue;
            tmp_brush_10 = Brushes.Navy;
            tmp_brush_11 = Brushes.DarkMagenta;
            tmp_brush_12 = Brushes.Indigo;
            tmp_brush_13 = Brushes.Purple;
            tmp_brush_14 = Brushes.DarkSlateGray;
            tmp_brush_15 = Brushes.Black;
        }

        public void ProbeClear()
        {
            probe1.Clear();
            probe2.Clear();
            probe3.Clear();
            probe4.Clear();
            probe5.Clear();
            probe6.Clear();
            probe7.Clear();
            probe8.Clear();
            probe9.Clear();
            probe10.Clear();
            probe11.Clear();
            probe12.Clear();
            probe13.Clear();
            probe14.Clear();
            probe15.Clear();
        }

        public void getAxis_pictureBox(PictureBox mpic)
        {
            if (mpic.Height < 120)
            {
                Height = 120;
            }
            else
            {
                Height = mpic.Height;
            }

            if (mpic.Width < 600)
            {
                Width = 600;
            }
            else
            {
                Width = mpic.Width;
            }

            yh_grid = Height / div;
            yh_full = 0;
            yh_zero = div * yh_grid;

            xw_stop = Width - XW;
            xw_start = XW;

            xw_text = xw_stop + 5;
        }

        public void getAxis_pictureBox(int max, int min)
        {
            if (min < tmin)
            {
                tmin = min / step;
                tmin = tmin * step;
            }
            else if (max > tmax)
            {
                tmax = tmax / step + 1;
                tmax = tmax * step;
            }

            div = (tmax - tmin) / step;

            yh_grid = Height / div;
            yh_full = 0;
            yh_zero = div * yh_grid;

            xw_stop = Width - XW;
            xw_start = XW;

            xw_text = xw_stop + 5;
        }

        public int getPosy(int tmp)
        {
            if (tmp < tmin)
            {
                return yh_zero;
            }
            else if (tmp > tmax)
            {
                return yh_full;
            }
            else
            {
                return ((tmax - tmp) * (yh_zero - yh_full) / (tmax - tmin) + yh_full);
            }
        }

        public void getProbe1()
        {
            int yh;
            int xw;
            int xline;

            probe1.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[0]);

                    probe1.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[0]);

                    xw = xw_start + (i * xline / stop);

                    probe1.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe2()
        {
            int yh;
            int xw;
            int xline;

            probe2.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[1]);

                    probe2.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[1]);

                    xw = xw_start + (i * xline / stop);

                    probe2.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe3()
        {
            int yh;
            int xw;
            int xline;

            probe3.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[2]);

                    probe3.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[2]);

                    xw = xw_start + (i * xline / stop);

                    probe3.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe4()
        {
            int yh;
            int xw;
            int xline;

            probe4.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[3]);

                    probe4.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[3]);

                    xw = xw_start + (i * xline / stop);

                    probe4.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe5()
        {
            int yh;
            int xw;
            int xline;

            probe5.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[4]);

                    probe5.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[4]);

                    xw = xw_start + (i * xline / stop);

                    probe5.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe6()
        {
            int yh;
            int xw;
            int xline;

            probe6.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[5]);

                    probe6.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[5]);

                    xw = xw_start + (i * xline / stop);

                    probe6.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe7()
        {
            int yh;
            int xw;
            int xline;

            probe7.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[6]);

                    probe7.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[6]);

                    xw = xw_start + (i * xline / stop);

                    probe7.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe8()
        {
            int yh;
            int xw;
            int xline;

            probe8.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[7]);

                    probe8.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[7]);

                    xw = xw_start + (i * xline / stop);

                    probe8.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe9()
        {
            int yh;
            int xw;
            int xline;

            probe9.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[8]);

                    probe9.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[8]);

                    xw = xw_start + (i * xline / stop);

                    probe9.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe10()
        {
            int yh;
            int xw;
            int xline;

            probe10.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[9]);

                    probe10.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[9]);

                    xw = xw_start + (i * xline / stop);

                    probe10.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe11()
        {
            int yh;
            int xw;
            int xline;

            probe11.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[10]);

                    probe11.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[10]);

                    xw = xw_start + (i * xline / stop);

                    probe11.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe12()
        {
            int yh;
            int xw;
            int xline;

            probe12.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[11]);

                    probe12.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[11]);

                    xw = xw_start + (i * xline / stop);

                    probe12.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe13()
        {
            int yh;
            int xw;
            int xline;

            probe13.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[12]);

                    probe13.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[12]);

                    xw = xw_start + (i * xline / stop);

                    probe13.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe14()
        {
            int yh;
            int xw;
            int xline;

            probe14.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[13]);

                    probe14.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[13]);

                    xw = xw_start + (i * xline / stop);

                    probe14.Add(new Point(xw, yh));
                }
            }
        }

        public void getProbe15()
        {
            int yh;
            int xw;
            int xline;

            probe15.Clear();

            xline = xw_stop - xw_start;

            //数据转换
            if (stop <= xline)
            {
                xw = xw_start;

                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[14]);

                    probe15.Add(new Point(xw, yh));

                    xw++;
                }
            }
            else
            {
                //数据范围[0,stop]
                //坐标范围[xw_start,xw_stop]
                //点(0,xw_start)和点(stop,xw_stop)方程
                for (int i = 0; i < stop; i++)
                {
                    yh = getPosy(MyDefine.myXET.mySyn[i].OUT[14]);

                    xw = xw_start + (i * xline / stop);

                    probe15.Add(new Point(xw, yh));
                }
            }
        }
    }
}
