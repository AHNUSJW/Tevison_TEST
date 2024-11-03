using System;
using System.Collections.Generic;

namespace TVS
{
    //EEPROM数据块使用
    public partial class SCT
    {

        //ID和序列号
        public UInt32 e_sidh;
        public UInt32 e_sidm;
        public UInt32 e_sidl;
        public UInt32 e_sidh_dat; //来自载入数据
        public UInt32 e_sidm_dat; //来自载入数据
        public UInt32 e_sidl_dat; //来自载入数据
        public UInt32 e_seri;
        public Byte e_type;//类型TVS,TVS8
        public Byte e_fv;  //固件版本
        public Byte e_hwv; //硬件版本
        public String e_connetid;  //产品连接时的ID  

        //温度电量
        public UInt16 temperature;
        public UInt16 battery;
        public TmpMode devStatus; //判断设备的工作状态
        public Byte devVersion; //判断设备版本TVS_F0~F5是老版本,F6开始新版本可以读到设备工作状态

        //记录工作参数
        public TmpMode mode; //模式
        public UInt16 value; //目标温度
        public UInt32 delay; //延时时间
        public UInt32 date;  //设置的日期
        public UInt32 time;  //设置的时间
        public UInt32 run;   //记录时间
        public UInt32 sum;   //记录条数
        public UInt16 max;   //目标阈值
        public UInt16 min;   //目标阈值

        //
        public UInt16[] T10 = new UInt16[SZ.CHA]; //温度点
        public UInt32[] A10 = new UInt32[SZ.CHA]; //对应ADC值
        public float[] V10 = new float[SZ.CHA]; //斜率
        public UInt16[] T30 = new UInt16[SZ.CHA];
        public UInt32[] A30 = new UInt32[SZ.CHA];
        public float[] V30 = new float[SZ.CHA];
        public UInt16[] T50 = new UInt16[SZ.CHA];
        public UInt32[] A50 = new UInt32[SZ.CHA];
        public float[] V50 = new float[SZ.CHA];
        public UInt16[] T60 = new UInt16[SZ.CHA];
        public UInt32[] A60 = new UInt32[SZ.CHA];
        public float[] V60 = new float[SZ.CHA];
        public UInt16[] T70 = new UInt16[SZ.CHA];
        public UInt32[] A70 = new UInt32[SZ.CHA];
        public float[] V70 = new float[SZ.CHA];
        public UInt16[] T90 = new UInt16[SZ.CHA];
        public UInt32[] A90 = new UInt32[SZ.CHA];
        public float[] V90 = new float[SZ.CHA];
        public UInt16[] T95 = new UInt16[SZ.CHA];
        public UInt32[] A95 = new UInt32[SZ.CHA];
        public float[] V95 = new float[SZ.CHA];

        //
        public TMP mtp = new TMP();
        //
        public bool strend;//斜率趋势,true上升,false下降
        public int oldrnd;//旧的outrnd
        public int overmax;//最大过冲
        public int overavg;//平均过冲
        public int outdim;//最大孔间差值
        public int outstm;//最大孔间标准差
        //
        public int upsecond;//50度到90度的升温时间
        public int dnsecond;//90度到50度的降温时间
        public DateTime dt90;//记录时间
        public DateTime dt50;//记录时间
        public RECT mrect;//升温状态机
        //
        public bool isAxis;//超出最大最小值需要重新画轴
        public int tmpMax;//画轴用最大值
        public int tmpMin;//画轴用最小值
        //
        public List<TMP> mySyn = new List<TMP>();//实时读出
        public String synFileName; //文件名
        public UInt32 syndate;  //开始记录的日期
        public UInt32 syntime;  //开始记录的时间
        public UInt32 syntimeMs;  //开始记录的时间,毫秒格式
        public UInt32 synstop;  //停止记录的时间
        public UInt32 synrun;   //停止记录时的毫秒,从245字节数据中获取的,动态更新的1ms心跳记录,等于记录数据的时间长度(毫秒数)
        public UInt32 synstep;  //采集间隔时间
        //
        public List<TMP> myMem = new List<TMP>();//导出TVS的flash
        public String memFileName; //文件名
        public TmpMode memmode;  //
        public UInt32 memvalue;  //
        public UInt32 memdelay;  //
        public UInt32 memdate;  //开始记录的日期
        public UInt32 memtime;  //开始记录的时间
        public UInt32 memstop;  //停止记录的时间
        public UInt32 memstart; //开始记录时的毫秒,从55字节head中获取的,开始记录时的毫秒
        public UInt32 memrun;   //停止记录时的毫秒,从245字节数据中获取的,动态更新的1ms心跳记录,memrun=(memrun-memstart)等于记录数据的时间长度(毫秒数)
        public UInt32 memstep;  //采集间隔时间
        public int memtotal;    //Par.rdmax, 读norFlash的最大地址
        public int type;        //类型
        //
        public List<TMP> myHom = new List<TMP>();//导入硬盘的数据
        public String homFileName; //文件名
        public String homsave;  //保存时间
        public UInt32 homdate;  //开始记录的日期
        public UInt32 homtime;  //开始记录的时间
        public UInt32 homstop;  //停止记录的时间
        public UInt32 homrun;   //停止记录时的毫秒,从245字节数据中获取的,动态更新的1ms心跳记录,等于记录数据的时间长度(毫秒数)
        public float homstep;   //采集间隔时间
        public int spanSopMax;  //一段时间最大温度变化率,升温
        public int spanSopMin;  //一段时间最小温度变化率,降温
        public int spanOutMax;  //一段时间孔温度最大值
        public int spanOutMin;  //一段时间孔温度最小值
        public int spanDifMax;  //一段时间孔间差最大差
        //
        public List<PRM> myPRM = new List<PRM>(); //编号 = 集合下标+1
    }
}

//end
