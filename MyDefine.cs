using System;
using System.Collections.Generic;

namespace TVS
{
    //
    #region RS232 COM Setting
    //

    public enum TYPE : Byte
    {
        TVS15 = 15,
        TVS8 = 8,
    }

    public enum BAUT : Int32
    {
        B4800 = 4800,
        B9600 = 9600,
        B14400 = 14400,
        B19200 = 19200,
        B38400 = 38400,
        B57600 = 57600,
        B115200 = 115200,
    }

    public enum RTCOM : Byte //通讯状态机
    {
        COM_NULL,
        COM_ERR_RST, //BLE复位
        COM_ERR_RPT, //读重传
        COM_ERR_MEM, //无Flash
        COM_SET_LOCK, //
        COM_SET_UNLOCK, //
        COM_SET_RESET, //
        COM_SET_SERI, //设置机器序列号
        COM_SET_DOT, //设置曲线标定点
        COM_SET_MEM, //写记录工作模式
        COM_SET_PROBE, //写探头曲线标定点(离线)
        COM_READ_TVS, //读板子ID和序列号
        COM_READ_PAR, //读板子电量和温度
        COM_READ_DOT, //读曲线标定点
        COM_READ_TMP, //读温度采集当前的结果
        COM_READ_MEM, //读温度采集记录的结果
        COM_CLEAR_MEM, //
    }

    public enum TASKS : Byte //任务状态机
    {
        disconnected = 0, //未连接
        setting = 1, //其它通讯操作
        run = 2, //实时监控工作中
        record = 3, //实时监控工作并且录制中
    }

    public enum TmpMode : Byte
    {
        NULL = 0,         //无
        immediately = 1,  //立即开始记录
        waite = 2,        //延时开始记录
        reach = 3,        //到达温度后开始记录
        threshold = 4,    //温度变化2度后开始记录
        clear = 5,        //清除数据
    }

    public enum DOT : byte
    {
        DOT0 = 0x80,
        DOT1 = 0x81,
        DOT2 = 0x82,
        DOT3 = 0x83,
        DOT4 = 0x84,
        DOT5 = 0x85,
        DOT6 = 0x86,
    }

    public enum PROBE : byte
    {
        PROBE1 = 0x80,
        PROBE2 = 0x81,
        PROBE3 = 0x82,
        PROBE4 = 0x83,
        PROBE5 = 0x84,
        PROBE6 = 0x85,
        PROBE7 = 0x86,
        PROBE8 = 0x87,
        PROBE9 = 0x88,
        PROBE10 = 0x89,
        PROBE11 = 0x8A,
        PROBE12 = 0x8B,
        PROBE13 = 0x8C,
        PROBE14 = 0x8D,
        PROBE15 = 0x8E,
    }

    public enum RECT : Byte //任务状态机
    {
        NULL,
        up_45,
        up_50,
        up_90,
        dn_95,
        dn_90,
        dn_50,
    }

    //
    #endregion
    //

    public static class SZ
    {
        public const Byte REC = 64;
        public const Byte CHA = 15;

        public const Int16 RxSize = 2048;
        public const Int16 TxSize = 2048;

        public const Int16 TMAX = 12699;
        public const Int16 TMIN = 0;
    }

    public static class Constants
    {
        public const Byte ERR_RST = 0x4E; //BLE复位
        public const Byte ERR_RPT = 0x4F; //读重传

        public const Byte SET_LOCK = 0x50; //
        public const Byte SET_UNLOCK = 0x51; //
        public const Byte SET_RESET = 0x52; //
        public const Byte SET_SERI = 0x53; //设置机器序列号
        public const Byte SET_DOT = 0x54; //设置曲线标定点
        public const Byte SET_MEM = 0x55; //写记录工作模式

        public const Byte READ_TVS = 0x56; //读板子ID和序列号
        public const Byte READ_PAR = 0x57; //读板子电量和温度
        public const Byte READ_DOT = 0x58; //读曲线标定点
        public const Byte READ_TMP = 0x59; //读温度采集当前的结果
        public const Byte READ_MEM = 0x5A; //读温度采集记录的结果
        public const Byte CLEAR_MEM = 0x5B; //
        public const Byte SET_PROBE = 0x5C; //离线设置探头曲线标定点

        public const Byte START = 0x02; //起始符
        public const Byte STOP = 0x03; //结束符
    }
    /// <summary>
    /// 斜率， 截距 类
    /// </summary>
    public class TempLineSlopeIntercept
    {
        public double slope { get; set; }//斜率

        public double intercept { get; set; }//截距
    }
    public class TMP
    {
        public DateTime time;               //15点温度记录时刻时间
        public int[] OUT = new int[SZ.CHA]; //15点温度记录值
        public int[] SOP = new int[SZ.CHA]; //15点温度变化率,前后12次采样相关

        public int sopMax; //15孔间最大温度变化率
        public int sopMin; //15孔间最小温度变化率
        public int sopAvg; //15孔间平均温度变化率
        public int outMax; //15孔间最大值
        public int outMin; //15孔间最小值
        public int outDif; //15孔间最大差
        public int outAvg; //15孔间平均值
        public int outStd; //15孔间标准差
        public int outRnd; //15孔平均值最接近的整数温度
        public int outDep; //15孔整数温度方差

        public TMP()
        {
            for (Byte i = 0; i < SZ.CHA; i++)
            {
                OUT[i] = 0;
            }
        }

        public TMP(TMP mP)
        {
            for (Byte i = 0; i < SZ.CHA; i++)
            {
                OUT[i] = mP.OUT[i];
            }
        }
    }

    public class tmpoint
    {
        public int temperature; //规程温度
        public int time;        //规程时间

        public tmpoint()
        {
            temperature = 0;
            time = 0;
        }

        public tmpoint(int a, int b)
        {
            temperature = a;
            time = b;
        }
    }

    public class PRM
    {
        //规程名称
        public String name;

        //step = 集合下标+1
        public List<tmpoint> myTP = new List<tmpoint>();

        public PRM()
        {
            name = "";
        }

        public PRM(String str)
        {
            name = str;
        }
    }
    /// <summary>
    ///  温度坐标点
    /// </summary>
    public class MyPoint
    {
        public int x;
        public int y;
        public MyPoint(int xValue, int yValue)
        {
            x = xValue;
            y = yValue;
        }
    }

    public static class MyDefine
    {
        public static UIT myUIT = new UIT();//数据转换使用
        public static XET myXET = new XET();//参数数据使用
    }

    public delegate void freshHandler();//定义委托
}

//end
