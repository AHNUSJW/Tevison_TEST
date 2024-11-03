using System;
using System.Collections.Generic;
using System.Drawing;

namespace TVS
{
    public class Tmpick : tmpoint
    {
        public bool rise = false;   //true=升温过来的,false=降温过来的
        public bool vutup = false;  //是否计算升温速率
        public bool vutdown = false;//是否计算降温速率

        public int begin = 0;       //符合规程温度的数据索引起始,时间顺序:begin<const_begin<const_end<end
        public int end = 0;         //符合规程温度的数据索引结束
        public int const_begin = 0; //符合规程温度的稳定数据索引起始
        public int const_end = 0;   //符合规程温度的稳定数据索引结束

        public float space = 0;     //规程时间分割成5段的时间间隔
        public int idx_30sec = 0;   //30秒处的的点
        public int idx_first = 0;   //等间隔处的点
        public int idx_second = 0;  //等间隔处的点
        public int idx_third = 0;   //等间隔处的点
        public int idx_fourth = 0;  //等间隔处的点
        public int idx_fifth = 0;   //等间隔处的点

        public int idx_vutbegin = 0;//
        public int idx_50begin = 0; //从50度开始
        public int idx_90end = 0;   //到90度结束
        public int idx_90begin = 0; //从90度开始
        public int idx_50end = 0;   //到50度结束
        public int idx_vutend = 0;  //

        public int xline_const_begin = 0;   //符合规程温度的数据索引起始在picture中的坐标X
        public int xline_const_end = 0;     //符合规程温度的数据索引结束在picture中的坐标X
        public int xline_50begin = 0;   //符合规程温度的数据索引起始在picture中的坐标X
        public int xline_90end = 0;     //符合规程温度的数据索引结束在picture中的坐标X
        public int xline_90begin = 0;   //符合规程温度的数据索引起始在picture中的坐标X
        public int xline_50end = 0;     //符合规程温度的数据索引结束在picture中的坐标X

        public int yline_top = 0;       //符合规程温度的数据索引在picture中的坐标Y
        public int yline_bottom = 0;    //符合规程温度的数据索引在picture中的坐标Y
        public int yline_50 = 0;        //符合规程温度的数据索引在picture中的坐标Y
        public int yline_90 = 0;        //符合规程温度的数据索引在picture中的坐标Y
    }
    public class Tmpicture
    {
        //
        public bool isLoad; //数据已加载
        public bool isFull; //显示完整曲线
        public bool isSet;  //是否手动选点

        //
        public int tmpHigh; //最高温度
        public int tmpLow;  //最低温度
        public int tmpStart;//选取温度数据起始idx
        public int tmpStop; //选取温度数据结束idx
        public int tmpPick; //选中的温度数据idx

        //
        public int tpIDX;   //规程序号
        public String name; //规程名称
        public List<Tmpick> myTP = new List<Tmpick>();  //step = 集合下标+1

        //窗口大小
        public int Width;
        public int Height;
        public int WTEXT = 28;

        //绘图坐标
        public int xline_start; //选取温度数据起始idx之坐标X
        public int xline_stop;  //选取温度数据结束idx之坐标X
        public int xline_pick;  //选中的温度数据idx之坐标X
        public int yline_pick;  //选中的温度数据idx之坐标X

        //曲线颜色
        public Color color_axis;
        public Color color_info;
        public Color color_active;
        public Color color_inactive;
        public Color color_vut;
        public Color color_pb1;
        public Color color_pb2;
        public Color color_pb3;
        public Color color_pb4;
        public Color color_pb5;
        public Color color_pb6;
        public Color color_pb7;
        public Color color_pb8;
        public Color color_pb9;
        public Color color_pb10;
        public Color color_pb11;
        public Color color_pb12;
        public Color color_pb13;
        public Color color_pb14;
        public Color color_pb15;

        //文字颜色
        public Brush brush_axis;
        public Brush brush_info;
        public Brush brush_active;
        public Brush brush_inactive;
        public Brush brush_vut;

        //文字大小
        public Font font_txt;

        ////画图的点
        //public List<Point> probe1 = new List<Point>();
        //public List<Point> probe2 = new List<Point>();
        //public List<Point> probe3 = new List<Point>();
        //public List<Point> probe4 = new List<Point>();
        //public List<Point> probe5 = new List<Point>();
        //public List<Point> probe6 = new List<Point>();
        //public List<Point> probe7 = new List<Point>();
        //public List<Point> probe8 = new List<Point>();
        //public List<Point> probe9 = new List<Point>();
        //public List<Point> probe10 = new List<Point>();
        //public List<Point> probe11 = new List<Point>();
        //public List<Point> probe12 = new List<Point>();
        //public List<Point> probe13 = new List<Point>();
        //public List<Point> probe14 = new List<Point>();
        //public List<Point> probe15 = new List<Point>();
        ////画图的点
        public List<MyPoint> probe1 = new List<MyPoint>();
        public List<MyPoint> probe2 = new List<MyPoint>();
        public List<MyPoint> probe3 = new List<MyPoint>();
        public List<MyPoint> probe4 = new List<MyPoint>();
        public List<MyPoint> probe5 = new List<MyPoint>();
        public List<MyPoint> probe6 = new List<MyPoint>();
        public List<MyPoint> probe7 = new List<MyPoint>();
        public List<MyPoint> probe8 = new List<MyPoint>();
        public List<MyPoint> probe9 = new List<MyPoint>();
        public List<MyPoint> probe10 = new List<MyPoint>();
        public List<MyPoint> probe11 = new List<MyPoint>();
        public List<MyPoint> probe12 = new List<MyPoint>();
        public List<MyPoint> probe13 = new List<MyPoint>();
        public List<MyPoint> probe14 = new List<MyPoint>();
        public List<MyPoint> probe15 = new List<MyPoint>();

        //构造函数
        public Tmpicture()
        {
            Width = 600;
            Height = 600;

            //Color.Silver
            //Color.Red;
            //Color.Orange;
            //Color.Gold;
            //Color.LawnGreen;
            //Color.LimeGreen;
            //Color.Cyan;
            //Color.DeepSkyBlue;
            //Color.Fuchsia;
            //Color.FloralWhite;
            //Color.SpringGreen

            color_axis = Color.DimGray;
            color_info = Color.Gold;
            color_active = Color.LimeGreen;
            color_inactive = Color.DarkGreen;
            color_vut = Color.Firebrick;
            color_pb1 = Color.White;
            color_pb2 = Color.Red;
            color_pb3 = Color.Blue;
            color_pb4 = Color.Pink;
            color_pb5 = Color.GreenYellow;
            color_pb6 = Color.Coral;
            color_pb7 = Color.LimeGreen;
            color_pb8 = Color.Orange;
            color_pb9 = Color.Cyan;
            color_pb10 = Color.Yellow;
            color_pb11 = Color.Honeydew;
            color_pb12 = Color.Lime;
            color_pb13 = Color.OrangeRed;
            color_pb14 = Color.Magenta;
            color_pb15 = Color.Gold;

            brush_axis = Brushes.Silver;
            brush_info = Brushes.Gold;
            brush_active = Brushes.LimeGreen;
            brush_inactive = Brushes.DarkGreen;
            brush_vut = Brushes.Firebrick;

            font_txt = new System.Drawing.Font("Arial", 8);

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

        //计算坐标轴
        public void getAxis_pictureBox(int w, int h, int high, int low)
        {
            int px; //求x坐标

            //窗口宽度
            if (w < 200)
            {
                Width = 200;
            }
            else
            {
                Width = w;
            }

            //窗口高度
            if (h < 200)
            {
                Height = 200;
            }
            else
            {
                Height = h;
            }

            //温度取整5度
            tmpHigh = high / 500 * 500 + 500;
            tmpLow = low / 500 * 500;

            //
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

            //
            if ((Width - WTEXT) >= MyDefine.myXET.myHom.Count)
            {
                for (int i = 0; i < MyDefine.myXET.myHom.Count; i++)
                {
                    //坐标X
                    px = i + WTEXT;

                    //(坐标Y,温度)
                    //(0,tmpHigh)
                    //(Height,tmpLow)
                    //py = Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (tmpHigh - tmpLow);
                    probe1.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (tmpHigh - tmpLow))));
                    probe2.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[1]) / (tmpHigh - tmpLow))));
                    probe3.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[2]) / (tmpHigh - tmpLow))));
                    probe4.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[3]) / (tmpHigh - tmpLow))));
                    probe5.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[4]) / (tmpHigh - tmpLow))));
                    probe6.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[5]) / (tmpHigh - tmpLow))));
                    probe7.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[6]) / (tmpHigh - tmpLow))));
                    probe8.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[7]) / (tmpHigh - tmpLow))));
                    probe9.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[8]) / (tmpHigh - tmpLow))));
                    probe10.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[9]) / (tmpHigh - tmpLow))));
                    probe11.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[10]) / (tmpHigh - tmpLow))));
                    probe12.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[11]) / (tmpHigh - tmpLow))));
                    probe13.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[12]) / (tmpHigh - tmpLow))));
                    probe14.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[13]) / (tmpHigh - tmpLow))));
                    probe15.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[14]) / (tmpHigh - tmpLow))));
                }
            }
            else
            {
                if (isFull)
                {
                    for (int i = 0; i < MyDefine.myXET.myHom.Count; i++)
                    {
                        //(坐标X,温度点)
                        //(WTEXT,0)
                        //(Width,Count)
                        px = (Width - WTEXT) * i / MyDefine.myXET.myHom.Count + WTEXT;

                        //(坐标Y,温度)
                        //(0,tmpHigh)
                        //(Height,tmpLow)
                        //py = Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (tmpHigh - tmpLow);
                        probe1.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (tmpHigh - tmpLow))));
                        probe2.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[1]) / (tmpHigh - tmpLow))));
                        probe3.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[2]) / (tmpHigh - tmpLow))));
                        probe4.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[3]) / (tmpHigh - tmpLow))));
                        probe5.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[4]) / (tmpHigh - tmpLow))));
                        probe6.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[5]) / (tmpHigh - tmpLow))));
                        probe7.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[6]) / (tmpHigh - tmpLow))));
                        probe8.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[7]) / (tmpHigh - tmpLow))));
                        probe9.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[8]) / (tmpHigh - tmpLow))));
                        probe10.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[9]) / (tmpHigh - tmpLow))));
                        probe11.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[10]) / (tmpHigh - tmpLow))));
                        probe12.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[11]) / (tmpHigh - tmpLow))));
                        probe13.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[12]) / (tmpHigh - tmpLow))));
                        probe14.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[13]) / (tmpHigh - tmpLow))));
                        probe15.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[14]) / (tmpHigh - tmpLow))));
                    }
                }
                else
                {
                    for (int i = tmpStart; i < tmpStop; i++)
                    {
                        //(坐标X,温度点)
                        //(WTEXT,tmpStart)
                        //(Width,tmpStop)
                        px = (Width - WTEXT) * (i - tmpStart) / (tmpStop - tmpStart) + WTEXT;

                        //(坐标Y,温度)
                        //(0,tmpHigh)
                        //(Height,tmpLow)
                        //py = Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (tmpHigh - tmpLow);
                        probe1.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (tmpHigh - tmpLow))));
                        probe2.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[1]) / (tmpHigh - tmpLow))));
                        probe3.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[2]) / (tmpHigh - tmpLow))));
                        probe4.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[3]) / (tmpHigh - tmpLow))));
                        probe5.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[4]) / (tmpHigh - tmpLow))));
                        probe6.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[5]) / (tmpHigh - tmpLow))));
                        probe7.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[6]) / (tmpHigh - tmpLow))));
                        probe8.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[7]) / (tmpHigh - tmpLow))));
                        probe9.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[8]) / (tmpHigh - tmpLow))));
                        probe10.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[9]) / (tmpHigh - tmpLow))));
                        probe11.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[10]) / (tmpHigh - tmpLow))));
                        probe12.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[11]) / (tmpHigh - tmpLow))));
                        probe13.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[12]) / (tmpHigh - tmpLow))));
                        probe14.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[13]) / (tmpHigh - tmpLow))));
                        probe15.Add(new MyPoint(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[14]) / (tmpHigh - tmpLow))));
                    }
                }
            }
        }

        //计算分析数据点
        public void getPoint_pictureBox()
        {
            if ((Width - WTEXT) >= MyDefine.myXET.myHom.Count)
            {
                xline_start = tmpStart + WTEXT;
                xline_stop = tmpStop + WTEXT;
                xline_pick = tmpPick + WTEXT;
            }
            else
            {
                if (isFull)
                {
                    //(坐标X,温度点)
                    //(WTEXT,0)
                    //(Width,Count)
                    xline_start = (Width - WTEXT) * tmpStart / MyDefine.myXET.myHom.Count + WTEXT;
                    xline_stop = (Width - WTEXT) * tmpStop / MyDefine.myXET.myHom.Count + WTEXT;
                    xline_pick = (Width - WTEXT) * tmpPick / MyDefine.myXET.myHom.Count + WTEXT;
                }
                else
                {
                    //(坐标X,温度点)
                    //(WTEXT,tmpStart)
                    //(Width,tmpStop)
                    xline_start = WTEXT;
                    xline_stop = Width;
                    xline_pick = (Width - WTEXT) * (tmpPick - tmpStart) / (tmpStop - tmpStart) + WTEXT;
                }
            }

            //规程温度点的画线计算
            for (int step = 0; step < myTP.Count; step++)
            {
                //坐标X
                if ((Width - WTEXT) >= MyDefine.myXET.myHom.Count)
                {
                    myTP[step].xline_const_begin = myTP[step].const_begin + WTEXT;
                    myTP[step].xline_const_end = myTP[step].const_end + WTEXT;
                    myTP[step].xline_50begin = myTP[step].idx_50begin + WTEXT;
                    myTP[step].xline_90end = myTP[step].idx_90end + WTEXT;
                    myTP[step].xline_90begin = myTP[step].idx_90begin + WTEXT;
                    myTP[step].xline_50end = myTP[step].idx_50end + WTEXT;
                }
                else
                {
                    if (isFull)
                    {
                        myTP[step].xline_const_begin = (Width - WTEXT) * myTP[step].const_begin / MyDefine.myXET.myHom.Count + WTEXT;
                        myTP[step].xline_const_end = (Width - WTEXT) * myTP[step].const_end / MyDefine.myXET.myHom.Count + WTEXT;
                        myTP[step].xline_50begin = (Width - WTEXT) * myTP[step].idx_50begin / MyDefine.myXET.myHom.Count + WTEXT;
                        myTP[step].xline_90end = (Width - WTEXT) * myTP[step].idx_90end / MyDefine.myXET.myHom.Count + WTEXT;
                        myTP[step].xline_90begin = (Width - WTEXT) * myTP[step].idx_90begin / MyDefine.myXET.myHom.Count + WTEXT;
                        myTP[step].xline_50end = (Width - WTEXT) * myTP[step].idx_50end / MyDefine.myXET.myHom.Count + WTEXT;
                    }
                    else
                    {
                        myTP[step].xline_const_begin = (Width - WTEXT) * (myTP[step].const_begin - tmpStart) / (tmpStop - tmpStart) + WTEXT;
                        myTP[step].xline_const_end = (Width - WTEXT) * (myTP[step].const_end - tmpStart) / (tmpStop - tmpStart) + WTEXT;
                        myTP[step].xline_50begin = (Width - WTEXT) * (myTP[step].idx_50begin - tmpStart) / (tmpStop - tmpStart) + WTEXT;
                        myTP[step].xline_90end = (Width - WTEXT) * (myTP[step].idx_90end - tmpStart) / (tmpStop - tmpStart) + WTEXT;
                        myTP[step].xline_90begin = (Width - WTEXT) * (myTP[step].idx_90begin - tmpStart) / (tmpStop - tmpStart) + WTEXT;
                        myTP[step].xline_50end = (Width - WTEXT) * (myTP[step].idx_50end - tmpStart) / (tmpStop - tmpStart) + WTEXT;
                    }
                }

                //坐标Y
                //myTP[step].yline_top = Height * (tmpHigh - (myTP[step].temperature + 5) * 100) / (tmpHigh - tmpLow);
                //myTP[step].yline_bottom = Height * (tmpHigh - (myTP[step].temperature - 5) * 100) / (tmpHigh - tmpLow);
                myTP[step].yline_top = Height * (tmpHigh - (myTP[step].temperature + 500)) / (tmpHigh - tmpLow);//已放大100倍，现不放大
                myTP[step].yline_bottom = Height * (tmpHigh - (myTP[step].temperature - 500)) / (tmpHigh - tmpLow);
                myTP[step].yline_50 = Height * (tmpHigh - 5000) / (tmpHigh - tmpLow);
                myTP[step].yline_90 = Height * (tmpHigh - 9000) / (tmpHigh - tmpLow);

                //向下移动
                if (myTP[step].yline_top < 0)
                {
                    myTP[step].yline_bottom -= myTP[step].yline_top;
                    myTP[step].yline_top = 0;
                }

                //向上移动
                if (myTP[step].yline_bottom > Height)
                {
                    myTP[step].yline_top = myTP[step].yline_top + Height - myTP[step].yline_bottom;
                    myTP[step].yline_bottom = Height;
                }
            }
        }

        //根据鼠标位置计算温度位置
        public int getTmpIdx_pictureBox(int ix, int iy)
        {
            int index;

            xline_pick = ix;
            yline_pick = iy;

            //   X坐标小于 =无效点位则未选点
            if (xline_pick < WTEXT)
            {
                //没有选点
                index = 0;
            }
            else if ((Width - WTEXT) >= MyDefine.myXET.myHom.Count)
            {
                //温度数据太少
                index = System.Math.Min(xline_pick - WTEXT, (MyDefine.myXET.myHom.Count - 1));
            }
            else
            {
                //折算
                if (isFull)
                {
                    //(WTEXT, 0)
                    //(Width, Count)
                    index = (xline_pick - WTEXT) * MyDefine.myXET.myHom.Count / (Width - WTEXT);
                }
                else
                {
                    //(WTEXT, tmpStart)
                    //(Width, tmpStop)
                    index = (xline_pick - WTEXT) * (tmpStop - tmpStart) / (Width - WTEXT) + tmpStart;
                }
            }

            //防止index溢出
            if (index < MyDefine.myXET.myHom.Count)
            {
                return index;
            }
            else
            {
                return (MyDefine.myXET.myHom.Count - 1);
            }
        }

        //
        //
        //
        //
        //
        //差值并取正
        public int Devation(int a, int b)
        {
            if (a < b)
            {
                return (b - a);
            }
            else
            {
                return (a - b);
            }
        }

        //从org点开始寻找start温度到stop温度的转折点
        public int GetStep(int org, int start, int stop)
        {
            int tmhalf;             //一半温度
            int half;               //一半温度处
            int sopmax;             //最高变化率
            int sopdiv;             //十分之一变化率
            int min_to_target = 50;  //恒温起始点的偏差

            if (start > 9000)
            {
                min_to_target = 80; //0.8度
            }
            else if (start > 7000)
            {
                min_to_target = 60; //0.6度
            }
            else
            {
                min_to_target = 50; //0.5度
            }

            //带2小数点
            start = start;
            stop = stop;

            //初始化
            tmhalf = (start + stop) / 2;
            half = org;
            sopmax = MyDefine.myXET.myHom[org].sopAvg;
            sopdiv = MyDefine.myXET.myHom[org].sopAvg / 10;

            //升温
            if (start < stop)
            {
                //找一半温度处
                for (int i = org; i < MyDefine.myXET.myHom.Count; i++)
                {
                    if ((Devation(MyDefine.myXET.myHom[i].outAvg, tmhalf) < 200) && (MyDefine.myXET.myHom[i].sopAvg > 0))
                    {
                        half = i;
                        sopmax = MyDefine.myXET.myHom[i].sopAvg;
                        if (sopmax >= 20)
                        {
                            sopdiv = sopmax / 10;
                        }
                        else
                        {
                            sopdiv = 1;
                        }
                        break;
                    }
                }

                //反向寻找
                for (int i = half; i > org; i--)
                {
                    //温度接近
                    if (Devation(MyDefine.myXET.myHom[i].outAvg, start) < min_to_target)
                    {
                        return i;
                    }

                    //斜率接近
                    if (MyDefine.myXET.myHom[i].sopAvg < sopdiv)
                    {
                        return i;
                    }

                    //斜率更新
                    if (MyDefine.myXET.myHom[i].sopAvg > sopmax)
                    {
                        sopmax = MyDefine.myXET.myHom[i].sopAvg;
                        if (sopmax >= 20)
                        {
                            sopdiv = sopmax / 10;
                        }
                        else
                        {
                            sopdiv = 1;
                        }
                    }
                }
            }
            //降温
            else
            {
                //找一半温度处
                for (int i = org; i < MyDefine.myXET.myHom.Count; i++)
                {
                    if ((Devation(MyDefine.myXET.myHom[i].outAvg, tmhalf) < 200) && (MyDefine.myXET.myHom[i].sopAvg < 0))
                    {
                        half = i;
                        sopmax = MyDefine.myXET.myHom[i].sopAvg;
                        if (sopmax <= -20)
                        {
                            sopdiv = sopmax / 10;
                        }
                        else
                        {
                            sopdiv = -1;
                        }
                        break;
                    }
                }

                //反向寻找
                for (int i = half; i > org; i--)
                {
                    //温度接近
                    if (Devation(MyDefine.myXET.myHom[i].outAvg, start) < min_to_target)
                    {
                        return i;
                    }

                    //斜率接近
                    if (MyDefine.myXET.myHom[i].sopAvg > sopdiv)
                    {
                        return i;
                    }

                    //斜率更新
                    if (MyDefine.myXET.myHom[i].sopAvg < sopmax)
                    {
                        sopmax = MyDefine.myXET.myHom[i].sopAvg;
                        if (sopmax <= -20)
                        {
                            sopdiv = sopmax / 10;
                        }
                        else
                        {
                            sopdiv = -1;
                        }
                    }
                }
            }

            return org;
        }

        //从begin到end之间反向找温度temp的离开点
        public int GetFirstStart(int begin, int end, int idx30sec, int temp)
        {
            if (begin >= MyDefine.myXET.myHom.Count) begin = MyDefine.myXET.myHom.Count - 1;
            if (end >= MyDefine.myXET.myHom.Count) end = MyDefine.myXET.myHom.Count - 1;

            //
            int leave = MyDefine.myXET.myHom[end].outAvg;

            //带2小数点
            temp = temp;

            //防溢出
            if (idx30sec < begin)
            {
                idx30sec = begin;
            }

            //穿过温度点
            if (leave > temp)
            {
                leave = temp - (leave - temp);

                //寻找
                for (int i = idx30sec; i > begin; i--)
                {
                    if (MyDefine.myXET.myHom[i].outAvg < leave)
                    {
                        return Math.Max(i, 1);
                    }
                }
            }
            else
            {
                leave = temp + (temp - leave);

                //寻找
                for (int i = idx30sec; i > begin; i--)
                {
                    if (MyDefine.myXET.myHom[i].outAvg > leave)
                    {
                        return Math.Max(i, 1);
                    }
                }
            }

            return Math.Max(begin, 1);
        }

        //从begin到end之间找到经过温度temp的点
        public int GetUpThrough(int begin, int end, int temp)
        {
            if (begin >= MyDefine.myXET.myHom.Count) begin = MyDefine.myXET.myHom.Count - 1;
            if (end >= MyDefine.myXET.myHom.Count) end = MyDefine.myXET.myHom.Count - 1;

            //带2小数点
            temp = temp;

            //
            for (int i = begin; i < end; i++)
            {
                if (MyDefine.myXET.myHom[i].outAvg > temp)
                {
                    return i;
                }
            }

            return begin;
        }

        //从begin到end之间找到经过温度temp的点
        public int GetDownThrough(int begin, int end, int temp)
        {
            if (begin >= MyDefine.myXET.myHom.Count) begin = MyDefine.myXET.myHom.Count - 1;
            if (end >= MyDefine.myXET.myHom.Count) end = MyDefine.myXET.myHom.Count - 1;

            //带2小数点
            temp = temp;

            //
            for (int i = begin; i < end; i++)
            {
                if (MyDefine.myXET.myHom[i].outAvg < temp)
                {
                    return i;
                }
            }

            return begin;
        }

        //找最大升温速率
        public int GetHeatRate(int begin, int end)
        {
            if (begin >= MyDefine.myXET.myHom.Count) begin = MyDefine.myXET.myHom.Count - 1;
            if (end >= MyDefine.myXET.myHom.Count) end = MyDefine.myXET.myHom.Count - 1;

            int rate = 0;

            for (int i = begin; i < end; i++)
            {
                if (MyDefine.myXET.myHom[i].sopAvg > rate)
                {
                    rate = MyDefine.myXET.myHom[i].sopAvg;
                }
            }

            return (rate);
        }

        //找最大降温速率
        public int GetCoolRate(int begin, int end)
        {
            if (begin >= MyDefine.myXET.myHom.Count) begin = MyDefine.myXET.myHom.Count - 1;
            if (end >= MyDefine.myXET.myHom.Count) end = MyDefine.myXET.myHom.Count - 1;

            int rate = 0;

            for (int i = begin; i < end; i++)
            {
                if (MyDefine.myXET.myHom[i].sopAvg < rate)
                {
                    rate = MyDefine.myXET.myHom[i].sopAvg;
                }
            }

            return (-rate);
        }

        //从begin到end之间找到持续温度temp的起始点
        public int GetConstStart(int begin, int end, int temp)
        {
            if (begin >= MyDefine.myXET.myHom.Count) begin = MyDefine.myXET.myHom.Count - 1;
            if (end >= MyDefine.myXET.myHom.Count) end = MyDefine.myXET.myHom.Count - 1;

            //恒温起始点的偏差
            int max_to_target = Devation(MyDefine.myXET.myHom[begin].outAvg, MyDefine.myXET.myHom[end].outAvg) / 2;

            //升温
            if (MyDefine.myXET.myHom[begin].outAvg < MyDefine.myXET.myHom[end].outAvg)
            {
                //正向寻找
                for (int i = begin; i < end; i++)
                {
                    //温度穿过
                    if (MyDefine.myXET.myHom[i].outAvg > temp)
                    {
                        return i;
                    }

                    //变化率为0
                    if ((MyDefine.myXET.myHom[i].sopAvg <= 0) && (Devation(MyDefine.myXET.myHom[i].outAvg, temp) < max_to_target))
                    {
                        return i;
                    }
                }
            }
            //降温
            else
            {
                //正向寻找
                for (int i = begin; i < end; i++)
                {
                    //温度穿过
                    if (MyDefine.myXET.myHom[i].outAvg < temp)
                    {
                        return i;
                    }

                    //变化率为0
                    if ((MyDefine.myXET.myHom[i].sopAvg >= 0) && (Devation(MyDefine.myXET.myHom[i].outAvg, temp) < max_to_target))
                    {
                        return i;
                    }
                }
            }

            return begin;
        }

        //从begin到end之间找到持续温度temp的结束点
        public int GetConstStop(int begin, int end, int temp)
        {
            if (begin >= MyDefine.myXET.myHom.Count) begin = MyDefine.myXET.myHom.Count - 1;
            if (end >= MyDefine.myXET.myHom.Count) end = MyDefine.myXET.myHom.Count - 1;

            //恒温起始点的偏差
            int min_to_target;

            if (temp > 90)
            {
                min_to_target = 80; //0.8度
            }
            else if (temp > 70)
            {
                min_to_target = 60; //0.6度
            }
            else
            {
                min_to_target = 50; //0.5度
            }

            //带2小数点
            temp = temp;

            //反向寻找
            for (int i = end; i > begin; i--)
            {
                //温度穿过
                if (Devation(MyDefine.myXET.myHom[i].outAvg, temp) < min_to_target)
                {
                    return i;
                }

                //变化率为0
                if (Devation(MyDefine.myXET.myHom[i].sopAvg, 0) < 50)
                {
                    return i;
                }
            }

            return begin;
        }

        //从begin到end之间找到持续温度temp的离开点
        public int GetConstLeave(int begin, int end, int idx30sec, int temp)
        {
            if (begin >= MyDefine.myXET.myHom.Count) begin = MyDefine.myXET.myHom.Count - 1;
            if (end >= MyDefine.myXET.myHom.Count) end = MyDefine.myXET.myHom.Count - 1;

            //变化率
            int tmpmax = MyDefine.myXET.myHom[begin].outAvg;
            int tmpmin = MyDefine.myXET.myHom[begin].outAvg;

            //变化率
            int sopmax = MyDefine.myXET.myHom[begin].sopAvg;
            int sopmin = MyDefine.myXET.myHom[begin].sopAvg;

            //恒温起始点的偏差
            int min_to_target;

            if (temp > 90)
            {
                min_to_target = 80; //0.8度
            }
            else if (temp > 70)
            {
                min_to_target = 60; //0.6度
            }
            else
            {
                min_to_target = 50; //0.5度
            }

            //带2小数点
            temp = temp * 100;

            //防溢出
            if (idx30sec > end)
            {
                idx30sec = end;
            }

            //找极值
            for (int i = begin; i < idx30sec; i++)
            {
                if (tmpmax < MyDefine.myXET.myHom[i].outAvg)
                {
                    tmpmax = MyDefine.myXET.myHom[i].outAvg;
                }

                if (tmpmin > MyDefine.myXET.myHom[i].outAvg)
                {
                    tmpmin = MyDefine.myXET.myHom[i].outAvg;
                }

                if (sopmax < MyDefine.myXET.myHom[i].sopAvg)
                {
                    sopmax = MyDefine.myXET.myHom[i].sopAvg;
                }

                if (sopmin > MyDefine.myXET.myHom[i].sopAvg)
                {
                    sopmin = MyDefine.myXET.myHom[i].sopAvg;
                }
            }

            //温度范围
            if (tmpmax > temp)
            {
                tmpmax = tmpmax + min_to_target;
            }
            else
            {
                tmpmax = temp + min_to_target;
            }

            if (tmpmin < temp)
            {
                tmpmin = tmpmin - min_to_target;
            }
            else
            {
                tmpmin = temp - min_to_target;
            }

            //两倍变化率
            sopmax = sopmax * 2;
            sopmin = sopmin * 2;

            //寻找
            for (int i = begin; i < end; i++)
            {
                //超过最大值
                if (MyDefine.myXET.myHom[i].outAvg > tmpmax)
                {
                    return i;
                }

                //超过最小值
                if (MyDefine.myXET.myHom[i].outAvg < tmpmin)
                {
                    return i;
                }

                //超过最大值
                if (MyDefine.myXET.myHom[i].sopAvg > sopmax)
                {
                    return i;
                }

                //超过最小值
                if (MyDefine.myXET.myHom[i].sopAvg < sopmin)
                {
                    return i;
                }
            }

            return end;
        }

        //最大过冲
        public int GetOverMax(int begin, int end, bool rise)
        {
            if (begin >= MyDefine.myXET.myHom.Count) begin = MyDefine.myXET.myHom.Count - 1;
            if (end >= MyDefine.myXET.myHom.Count) end = MyDefine.myXET.myHom.Count - 1;

            int tmp = MyDefine.myXET.myHom[begin].OUT[0];

            for (int i = begin; i < end; i++)
            {
                for (int k = 0; k < SZ.CHA; k++)
                {
                    if (rise)
                    {
                        if (MyDefine.myXET.myHom[i].OUT[k] > tmp)
                        {
                            tmp = MyDefine.myXET.myHom[i].OUT[k];
                        }
                    }
                    else
                    {
                        if (MyDefine.myXET.myHom[i].OUT[k] < tmp)
                        {
                            tmp = MyDefine.myXET.myHom[i].OUT[k];
                        }
                    }
                }
            }

            return tmp;
        }

        //平均过冲
        public int GetOverAvg(int begin, int end, bool rise)
        {
            if (begin >= MyDefine.myXET.myHom.Count) begin = MyDefine.myXET.myHom.Count - 1;
            if (end >= MyDefine.myXET.myHom.Count) end = MyDefine.myXET.myHom.Count - 1;

            int tmp = MyDefine.myXET.myHom[begin].outAvg;

            for (int i = begin; i < end; i++)
            {
                if (rise)
                {
                    if (MyDefine.myXET.myHom[i].outAvg > tmp)
                    {
                        tmp = MyDefine.myXET.myHom[i].outAvg;
                    }
                }
                else
                {
                    if (MyDefine.myXET.myHom[i].outAvg < tmp)
                    {
                        tmp = MyDefine.myXET.myHom[i].outAvg;
                    }
                }
            }

            return tmp;
        }

        //自动选点
        public bool autoGetTP_pictureBox()//新增返回值 判断自动选点完整性 Pengjun 20221215
        {
            int step_start = 0;  //起始设定温度索引
            int step_target = 0; //目标设定温度索引
            int step_next = 1;   //下个目标温度索引

            #region 第一点计算

            //起始点不带小数的温度
            int org_temp = MyDefine.myXET.myHom[tmpStart].outAvg / 100;

            //升降温
            if (org_temp < (myTP[step_target].temperature / 100f))
            {
                myTP[step_target].rise = true;
            }
            else
            {
                myTP[step_target].rise = false;
            }

            //控制阶段计算 step 1-2
            myTP[step_target].begin = tmpStart;
            myTP[step_target].end = GetStep(myTP[step_target].begin, myTP[step_target].temperature, myTP[step_next].temperature);

            //稳定阶段计算
            //先反向寻找,防止第一个实际温度段不是规程要的第一个温度段
            myTP[step_target].const_begin = GetFirstStart(myTP[step_target].begin, myTP[step_target].end, (myTP[step_target].end - (int)(30000 / MyDefine.myXET.homstep)), myTP[step_target].temperature);
            myTP[step_target].const_begin = GetConstStart(myTP[step_target].const_begin, myTP[step_target].end, myTP[step_target].temperature);
            myTP[step_target].const_end = GetConstStop(myTP[step_target].const_begin, myTP[step_target].end, myTP[step_target].temperature);

            //30秒处
            myTP[step_target].idx_30sec = myTP[step_target].const_begin + (int)(30000 / MyDefine.myXET.homstep);

            //间隔处
            myTP[step_target].space = myTP[step_target].time / 4.0f;
            myTP[step_target].idx_first = myTP[step_target].const_begin;
            myTP[step_target].idx_second = myTP[step_target].const_begin + (int)(myTP[step_target].space * 1000 / MyDefine.myXET.homstep);
            myTP[step_target].idx_third = myTP[step_target].const_begin + (int)(myTP[step_target].space * 2000 / MyDefine.myXET.homstep);
            myTP[step_target].idx_fourth = myTP[step_target].const_begin + (int)(myTP[step_target].space * 3000 / MyDefine.myXET.homstep);
            myTP[step_target].idx_fifth = myTP[step_target].const_begin + (int)(myTP[step_target].space * 4000 / MyDefine.myXET.homstep);

            #endregion

            step_start = 0;
            step_target = 1;
            step_next = 2;

            for (int step = 0; step < (myTP.Count - 2); step++)
            {
                #region 第n页计算

                //重置   Pengjun 20221111 自动选点 重置 升温降温标识
                myTP[step_start].vutup = false;
                myTP[step_start].vutdown = false;

                //升降温
                if (myTP[step_start].temperature < myTP[step_target].temperature)
                {
                    myTP[step_target].rise = true;
                }
                else
                {
                    myTP[step_target].rise = false;
                }

                //控制阶段计算 step next
                myTP[step_target].begin = myTP[step_start].end;
                myTP[step_target].end = GetStep(myTP[step_target].begin, myTP[step_target].temperature, myTP[step_next].temperature);

                //恒定温度计算
                myTP[step_target].const_begin = GetConstStart(myTP[step_target].begin, myTP[step_target].end, myTP[step_target].temperature);
                myTP[step_target].const_end = GetConstStop(myTP[step_target].begin, myTP[step_target].end, myTP[step_target].temperature);

                //30秒处
                myTP[step_target].idx_30sec = myTP[step_target].const_begin + (int)(30000 / MyDefine.myXET.homstep);

                //间隔处
                myTP[step_target].space = myTP[step_target].time / 4.0f;
                myTP[step_target].idx_first = myTP[step_target].const_begin;
                myTP[step_target].idx_second = myTP[step_target].const_begin + (int)(myTP[step_target].space * 1000 / MyDefine.myXET.homstep);
                myTP[step_target].idx_third = myTP[step_target].const_begin + (int)(myTP[step_target].space * 2000 / MyDefine.myXET.homstep);
                myTP[step_target].idx_fourth = myTP[step_target].const_begin + (int)(myTP[step_target].space * 3000 / MyDefine.myXET.homstep);
                myTP[step_target].idx_fifth = myTP[step_target].const_begin + (int)(myTP[step_target].space * 4000 / MyDefine.myXET.homstep);


                //检查是否超出当前总数据范围，超出范围则返回，不继续自动选点 Pengjun 20221215
                if (myTP[step_target].begin > MyDefine.myXET.myHom.Count - 1
                    || myTP[step_target].end > MyDefine.myXET.myHom.Count - 1
                    || myTP[step_target].const_begin > MyDefine.myXET.myHom.Count - 1
                    || myTP[step_target].const_end > MyDefine.myXET.myHom.Count - 1
                    || myTP[step_target].idx_30sec > MyDefine.myXET.myHom.Count - 1
                    || myTP[step_target].idx_first > MyDefine.myXET.myHom.Count - 1
                    || myTP[step_target].idx_second > MyDefine.myXET.myHom.Count - 1
                    || myTP[step_target].idx_third > MyDefine.myXET.myHom.Count - 1
                    || myTP[step_target].idx_fourth > MyDefine.myXET.myHom.Count - 1
                    || myTP[step_target].idx_fifth > MyDefine.myXET.myHom.Count - 1)
                {
                    myTP[step_target].begin = 0;
                    myTP[step_target].end = 0;
                    myTP[step_target].const_begin = 0;
                    myTP[step_target].const_end = 0;
                    myTP[step_target].idx_30sec = 0;
                    myTP[step_target].idx_first = 0;
                    myTP[step_target].idx_second = 0;
                    myTP[step_target].idx_third = 0;
                    myTP[step_target].idx_fourth = 0;
                    myTP[step_target].idx_fifth = 0;
                    return false;
                }
                #endregion

                step_start++;
                step_target++;
                step_next++;
            }

            #region 最后一点计算

            //升降温
            if (myTP[step_start].temperature < myTP[step_target].temperature)
            {
                myTP[step_target].rise = true;
            }
            else
            {
                myTP[step_target].rise = false;
            }

            //控制阶段计算 step next
            myTP[step_target].begin = myTP[step_start].end;
            myTP[step_target].end = tmpStop;

            //恒定温度计算
            myTP[step_target].const_begin = GetConstStart(myTP[step_target].begin, myTP[step_target].end, myTP[step_target].temperature);
            myTP[step_target].const_end = GetConstLeave(myTP[step_target].const_begin, myTP[step_target].end, (myTP[step_target].const_begin + (int)(30000 / MyDefine.myXET.homstep)), myTP[step_target].temperature);

            //30秒处
            myTP[step_target].idx_30sec = myTP[step_target].const_begin + (int)(30000 / MyDefine.myXET.homstep);


            //Pengjun 20221215 最后一个点的计算防止数据不够情况。判断数据是否够。 根据实际恒温时长是否达到规程恒温时长，未达到则根据实际时长4等分来计算
            TimeSpan secondSpan = new TimeSpan(MyDefine.myXET.myHom[MyDefine.myXET.myHom.Count - 1].time.Ticks - MyDefine.myXET.myHom[myTP[step_target].const_begin].time.Ticks);
            if (secondSpan.TotalSeconds < myTP[myTP.Count - 1].time)
            {
                //间隔处
                myTP[step_target].space = (float)(secondSpan.TotalSeconds) / 4.0f;
            }
            else
            {
                //间隔处
                myTP[step_target].space = myTP[step_target].time / 4.0f;
            }

            myTP[step_target].idx_first = myTP[step_target].const_begin;
            myTP[step_target].idx_second = myTP[step_target].const_begin + (int)(myTP[step_target].space * 1000 / MyDefine.myXET.homstep);
            myTP[step_target].idx_third = myTP[step_target].const_begin + (int)(myTP[step_target].space * 2000 / MyDefine.myXET.homstep);
            myTP[step_target].idx_fourth = myTP[step_target].const_begin + (int)(myTP[step_target].space * 3000 / MyDefine.myXET.homstep);
            myTP[step_target].idx_fifth = myTP[step_target].const_begin + (int)(myTP[step_target].space * 4000 / MyDefine.myXET.homstep);
            if (myTP[step_target].idx_fifth > MyDefine.myXET.myHom.Count)
            {
                myTP[step_target].idx_fifth = MyDefine.myXET.myHom.Count - 1;
            }

            #endregion

            //排除第一个点和最后一个点
            for (int idx = 1; idx < (myTP.Count - 1); idx++)
            {
                #region 寻找升降温点
                myTP[idx].vutup = false; //重置计算升降温标识
                myTP[idx].vutdown = false;//重置计算升降温标识

                //超过90度
                if (myTP[idx].temperature >= 9500) //
                {
                    //区间
                    myTP[idx].idx_vutbegin = myTP[idx].begin;
                    myTP[idx].idx_vutend = myTP[idx + 1].const_begin;

                    //升温前低于50度
                    if (myTP[idx - 1].temperature <= 4500) //
                    {
                        myTP[idx].vutup = true;
                        myTP[idx].idx_50begin = GetUpThrough(myTP[idx].idx_vutbegin, myTP[idx].const_end, 5000);//
                        myTP[idx].idx_90end = GetUpThrough(myTP[idx].idx_50begin, myTP[idx].const_end, 9000);
                    }

                    //降温后低于50度
                    if (myTP[idx + 1].temperature <= 4500)//
                    {
                        myTP[idx].vutdown = true;
                        myTP[idx].idx_90begin = GetDownThrough(myTP[idx].const_end, myTP[idx].idx_vutend, 9000);
                        myTP[idx].idx_50end = GetDownThrough(myTP[idx].idx_90begin, myTP[idx].idx_vutend, 5000);//
                    }
                }
                #endregion
            }
            return true;
        }

        //更新idx
        public void getTmpIdx()
        {
            for (int idx = 0; idx < myTP.Count; idx++)
            {
                //30秒处
                //myTP[idx].idx_30sec = myTP[idx].const_begin + (int)(30000 / MyDefine.myXET.homstep);

                //间隔处
                myTP[idx].space = (myTP[idx].const_end - myTP[idx].const_begin) * MyDefine.myXET.homstep / 4000.0f;
                myTP[idx].idx_first = myTP[idx].const_begin;
                myTP[idx].idx_second = myTP[idx].const_begin + (int)(myTP[idx].space * 1000 / MyDefine.myXET.homstep);
                myTP[idx].idx_third = myTP[idx].const_begin + (int)(myTP[idx].space * 2000 / MyDefine.myXET.homstep);
                myTP[idx].idx_fourth = myTP[idx].const_begin + (int)(myTP[idx].space * 3000 / MyDefine.myXET.homstep);
                myTP[idx].idx_fifth = myTP[idx].const_begin + (int)(myTP[idx].space * 4000 / MyDefine.myXET.homstep);

                //中间处
                myTP[idx].idx_30sec = myTP[idx].idx_third;
            }
        }
    }
}
