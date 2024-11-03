using System;
using System.IO.Ports;

namespace TVS
{
    //用户配置使用
    public partial class XET : SCT
    {
        //User function
        public String userName;
        public String userPassword;
        public String userCFG;
        public String userDAT;
        public String userOUT;
        public String userPIC;
        //
        public SerialPort mePort = new SerialPort(); //串口
        public RTCOM rtCOM = RTCOM.COM_NULL; //通讯状态机
        public TASKS myTask = TASKS.disconnected;//任务状态机
        public Byte[] meTXD = new Byte[SZ.TxSize]; //发送缓冲区
        public Byte[] meRXD = new Byte[SZ.RxSize]; //接收缓冲区
        public String meStr; //接收字符串
        public Int16 rxRead = 0; //接收缓冲区读指针
        public Int16 rxWrite = 0; //接收缓冲区写指针
        public Int16 rxCount = 0; //接收计数

        //User PC Copyright
        public Int64 myMac = 0;
        public Int64 myVar = 0;
        public Byte myPC = 0;

        //User Event 定义事件
        public event freshHandler myUpdate;
        public event freshHandler mfUpdate;

        //
        public String debugStr;

        public int count = 0;//记录异常数据条数
        public int old_index = 0;//记录异常数据的前一个正常数据的下标
        public int new_index;//记录新的数据下标
    }
}

//end
