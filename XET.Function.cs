using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;

namespace TVS
{
    public partial class XET : SCT
    {
        public UInt32 errCount = 0;

        public XET()
        {
            //
            userName = "admin";
            userPassword = "";
            userCFG = Application.StartupPath + @"\cfg";
            userDAT = Application.StartupPath + @"\dat";
            userOUT = Application.StartupPath + @"\out";
            userPIC = Application.StartupPath + @"\pic";

            if (!Directory.Exists(userCFG))
            {
                Directory.CreateDirectory(userCFG);
            }
            if (!Directory.Exists(userDAT))
            {
                Directory.CreateDirectory(userDAT);
            }
            if (!Directory.Exists(userOUT))
            {
                Directory.CreateDirectory(userOUT);
            }
            if (!Directory.Exists(userPIC))
            {
                Directory.CreateDirectory(userPIC);
            }

            //
            mePort.PortName = "COM1";
            mePort.BaudRate = Convert.ToInt32(BAUT.B115200); //波特率固定
            mePort.DataBits = Convert.ToInt32("8"); //数据位固定
            mePort.StopBits = StopBits.One; //停止位固定
            mePort.Parity = Parity.None; //校验位固定
            mePort.ReceivedBytesThreshold = 1; //接收即通知
            mePort.DataReceived += new SerialDataReceivedEventHandler(mePort_DataReceived);

            //
            myTask = TASKS.disconnected;
            rtCOM = RTCOM.COM_READ_TVS;
            Array.Clear(meTXD, 0, meTXD.Length);
            Array.Clear(meRXD, 0, meRXD.Length);
            rxRead = 0;
            rxWrite = 0;
            myPC = 0;
        }

        //保存帐号
        public bool SaveToDat()
        {
            //空
            if (userCFG == null)
            {
                return false;
            }
            //创建新路径
            else if (!Directory.Exists(userCFG))
            {
                Directory.CreateDirectory(userCFG);
            }

            //写入
            try
            {
                String mePath = userCFG + @"\user." + userName + ".cfg";
                if (File.Exists(mePath))
                {
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                }
                FileStream meFS = new FileStream(mePath, FileMode.Create, FileAccess.Write);
                BinaryWriter meWrite = new BinaryWriter(meFS);
                //
                meWrite.Write(userName);
                meWrite.Write(userPassword);
                meWrite.Write(userCFG);
                //
                meWrite.Close();
                meFS.Close();
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
                return true;
            }
            catch
            {
                return false;
            }
        }

        //保存数据
        public bool syn_SaveToLog(String mePath, bool isCorrect = false)
        {
            //空
            if (mePath == null)
            {
                return false;
            }

            //写入
            try
            {
                String mStr;
                if (File.Exists(mePath))
                {
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                }
                FileStream meFS = new FileStream(mePath, FileMode.Create, FileAccess.Write);
                TextWriter meWrite = new StreamWriter(meFS);
                //
                meWrite.WriteLine(";" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                meWrite.WriteLine("type=" + MyDefine.myXET.e_type.ToString());
                meWrite.WriteLine(";===============================================================");
                if (isCorrect)
                {
                    for (int i = 0; i < myHom.Count; i++)
                    {
                        mStr = "";
                        for (int k = 0; k < SZ.CHA; k++)
                        {
                            mStr += myHom[i].OUT[k].ToString();
                            mStr += ",";
                        }
                        meWrite.WriteLine(mStr);
                    }
                    meWrite.WriteLine(";---------------------------------------------------------------");
                    meWrite.WriteLine("date=" + MyDefine.myXET.homdate.ToString());
                    meWrite.WriteLine("time=" + MyDefine.myXET.homtime.ToString());
                    meWrite.WriteLine("stop=" + MyDefine.myXET.homstop.ToString());
                    meWrite.WriteLine("duration=" + MyDefine.myXET.homrun.ToString());
                    meWrite.WriteLine("tvsid=" + e_sidm.ToString("X8") + e_sidl.ToString("X8") + e_sidh.ToString("X8"));
                    meWrite.WriteLine("tvssn=" + e_seri.ToString());
                }
                else
                {
                    for (int i = 0; i < mySyn.Count; i++)
                    {
                        mStr = "";
                        for (int k = 0; k < SZ.CHA; k++)
                        {
                            mStr += mySyn[i].OUT[k].ToString();
                            mStr += ",";
                        }
                        meWrite.WriteLine(mStr);
                    }
                    meWrite.WriteLine(";---------------------------------------------------------------");
                    meWrite.WriteLine("date=" + syndate.ToString());
                    meWrite.WriteLine("time=" + syntime.ToString());
                    meWrite.WriteLine("stop=" + synstop.ToString());
                    meWrite.WriteLine("duration=" + synrun.ToString());
                    meWrite.WriteLine("tvsid=" + e_sidm.ToString("X8") + e_sidl.ToString("X8") + e_sidh.ToString("X8"));
                    meWrite.WriteLine("tvssn=" + e_seri.ToString());
                }

                //
                meWrite.Close();
                meFS.Close();
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
                synFileName = mePath;
                return true;
            }
            catch
            {
                return false;
            }
        }

        //保存数据
        public bool mem_SaveToLog(String mePath)
        {
            //空
            if (mePath == null)
            {
                return false;
            }

            //写入
            try
            {
                String mStr;
                if (File.Exists(mePath))
                {
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                }
                FileStream meFS = new FileStream(mePath, FileMode.Create, FileAccess.Write);
                TextWriter meWrite = new StreamWriter(meFS);
                //
                meWrite.WriteLine(";" + System.DateTime.Now.ToString());
                meWrite.WriteLine("type=" + MyDefine.myXET.e_type.ToString());
                meWrite.WriteLine(";===============================================================");
                for (int i = 0; i < myMem.Count; i++)
                {
                    mStr = "";
                    for (int k = 0; k < SZ.CHA; k++)
                    {
                        mStr += myMem[i].OUT[k].ToString();
                        mStr += ",";
                    }
                    meWrite.WriteLine(mStr);
                }
                meWrite.WriteLine(";---------------------------------------------------------------");
                meWrite.WriteLine("date=" + memdate.ToString());
                meWrite.WriteLine("time=" + memtime.ToString());
                meWrite.WriteLine("stop=" + memstop.ToString());
                meWrite.WriteLine("duration=" + memrun.ToString());
                meWrite.WriteLine("tvsid=" + e_sidm.ToString("X8") + e_sidl.ToString("X8") + e_sidh.ToString("X8"));
                meWrite.WriteLine("tvssn=" + e_seri.ToString());
                //
                meWrite.Close();
                meFS.Close();
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
                memFileName = mePath;
                return true;
            }
            catch
            {
                return false;
            }
        }

        //保存数据
        public bool syn_SaveToExcel(String mePath)
        {
            //空
            if (mePath == null)
            {
                return false;
            }

            //写入
            try
            {
                String mStr;
                String csvTime;//输出到CSV文件中的Time
                String csvDate;//输出到CSV文件中的Date

                //起始时间
                //使用syntimeMs获取精确到毫秒的时间
                DateTime mDate = new DateTime(((int)syndate / 10000) + 2000, ((int)syndate / 100) % 100, (int)syndate % 100, (int)syntimeMs / 10000000, ((int)syntimeMs / 100000) % 100, ((int)syntimeMs / 1000) % 100, (int)syntimeMs % 1000);

                //条数mySyn.Count
                //时间synrun
                //AddMilliseconds(1000)
                //AddTicks(10000000)
                //1ms = 10000ticks

                long mGap = synrun * 10000L / mySyn.Count;

                //excel的每一行
                var lines = new List<string>();

                //
                if (File.Exists(mePath))
                {
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                }

                //
                lines.Add("Date,20" + syndate.ToString().Insert(2, "-").Insert(5, "-"));
                //起始时间
                lines.Add("start," + syntime.ToString("000000").Insert(2, ":").Insert(5, ":"));
                //结束时间
                lines.Add("stop," + synstop.ToString("000000").Insert(2, ":").Insert(5, ":"));
                lines.Add("duration(秒)," + (synrun / 1000.0f).ToString());
                lines.Add("ID," + e_sidm.ToString("X8") + "," + e_sidl.ToString("X8") + "," + e_sidh.ToString("X8"));
                lines.Add("SN," + e_seri.ToString());
                lines.Add("");
                lines.Add("Date,Time,A1,A4,A7,A10,A12,D1,D7,D12,E4,E10,H1,H4,H7,H10,H12");

                //
                for (int i = 0; i < mySyn.Count; i++)
                {
                    csvDate = mDate.ToString("yy/MM/dd");
                    csvTime = mDate.ToString("HH:mm:ss.fff");
                    mStr = null;
                    mDate = mDate.AddTicks(mGap);
                    for (int k = 0; k < SZ.CHA; k++)
                    {
                        mStr += (mySyn[i].OUT[k] / 100.0f).ToString("f2");
                        mStr += ",";
                    }
                    //给时间数据加=\，使csv文件用excel打开时能正常显示时间数据
                    lines.Add($"{csvDate},=\"{csvTime:HH:mm:ss.fff}\",{mStr}");
                }

                //
                File.WriteAllLines(mePath, lines, System.Text.Encoding.Default);
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
                synFileName = mePath;
                return true;
            }
            catch
            {
                return false;
            }
        }

        //保存数据
        public bool mem_SaveToExcel(String mePath)
        {
            //空
            if (mePath == null)
            {
                return false;
            }

            //写入
            try
            {
                String mStr;
                String csvTime;//输出到CSV文件中的Time
                String csvDate;//输出到CSV文件中的Date

                //起始时间
                //使用syntimeMs获取精确到毫秒的时间
                DateTime mDate = new DateTime(((int)syndate / 10000) + 2000, ((int)syndate / 100) % 100, (int)syndate % 100, (int)syntimeMs / 10000000, ((int)syntimeMs / 100000) % 100, ((int)syntimeMs / 1000) % 100, (int)syntimeMs % 1000);

                //条数myMem.Count
                //时间memrun
                //AddMilliseconds(1000)
                //AddTicks(10000000)
                //1ms = 10000ticks
                //long mGap = (long)memrun * (long)10000 / (long)myMem.Count;

                long mGap = memrun * 10000L / myMem.Count;

                //excel的每一行
                var lines = new List<string>();

                //
                if (File.Exists(mePath))
                {
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                }

                //
                lines.Add("Date,20" + memdate.ToString().Insert(2, "-").Insert(5, "-"));
                //起始时间
                lines.Add("start," + syntime.ToString("000000").Insert(2, ":").Insert(5, ":"));
                //结束时间
                lines.Add("stop," + synstop.ToString("000000").Insert(2, ":").Insert(5, ":"));
                lines.Add("duration(秒)," + (memrun / 1000.0f).ToString());
                lines.Add("ID," + e_sidm.ToString("X8") + "," + e_sidl.ToString("X8") + "," + e_sidh.ToString("X8"));
                lines.Add("SN," + e_seri.ToString());
                lines.Add("");
                lines.Add("Date,Time,A1,A4,A7,A10,A12,D1,D7,D12,E4,E10,H1,H4,H7,H10,H12");

                //
                for (int i = 0; i < mySyn.Count; i++)
                {
                    csvDate = mDate.ToString("yy/MM/dd");
                    csvTime = mDate.ToString("HH:mm:ss.fff");
                    mStr = null;
                    mDate = mDate.AddTicks(mGap);
                    for (int k = 0; k < SZ.CHA; k++)
                    {
                        mStr += (mySyn[i].OUT[k] / 100.0f).ToString("f2");
                        mStr += ",";
                    }
                    //给时间数据加=\，使csv文件用excel打开时能正常显示时间数据
                    lines.Add($"{csvDate},=\"{csvTime:HH:mm:ss.fff}\",{mStr}");
                }

                //
                File.WriteAllLines(mePath, lines, System.Text.Encoding.Default);
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
                memFileName = mePath;
                return true;
            }
            catch
            {
                return false;
            }
        }

        //保存数据
        public bool hom_SaveToExcel(String mePath)
        {
            //空
            if (mePath == null)
            {
                return false;
            }

            //写入
            try
            {
                String mStr;
                String csvTime;//输出到CSV文件中的Time
                String csvDate;//输出到CSV文件中的Date

                //起始时间
                //使用syntimeMs获取精确到毫秒的时间
                DateTime mDate = new DateTime(((int)homdate / 10000) + 2000, ((int)homdate / 100) % 100, (int)homdate % 100, (int)homtime / 10000, ((int)homtime / 100) % 100, (int)homtime % 100);

                //条数myMem.Count
                //时间memrun
                //AddMilliseconds(1000)
                //AddTicks(10000000)
                //1ms = 10000ticks
                //long mGap = (long)memrun * (long)10000 / (long)myMem.Count;

                long mGap = homrun * 10000L / myHom.Count;

                //excel的每一行
                var lines = new List<string>();

                //
                if (File.Exists(mePath))
                {
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                }

                //
                lines.Add("Date,20" + homdate.ToString().Insert(2, "-").Insert(5, "-"));
                //起始时间
                lines.Add("start," + homtime.ToString("000000").Insert(2, ":").Insert(5, ":"));
                //结束时间
                lines.Add("stop," + homstop.ToString("000000").Insert(2, ":").Insert(5, ":"));
                lines.Add("duration(秒)," + (homrun / 1000.0f).ToString());
                lines.Add("ID," + e_sidm.ToString("X8") + "," + e_sidl.ToString("X8") + "," + e_sidh.ToString("X8"));
                lines.Add("SN," + e_seri.ToString());
                lines.Add("");
                lines.Add("Date,Time,A1,A4,A7,A10,A12,D1,D7,D12,E4,E10,H1,H4,H7,H10,H12");

                //
                for (int i = 0; i < myHom.Count; i++)
                {
                    csvDate = mDate.ToString("yy/MM/dd");
                    csvTime = mDate.ToString("HH:mm:ss.fff");
                    mStr = null;
                    mDate = mDate.AddTicks(mGap);
                    for (int k = 0; k < SZ.CHA; k++)
                    {
                        mStr += (myHom[i].OUT[k] / 100.0f).ToString("f2");
                        mStr += ",";
                    }
                    //给时间数据加=\，使csv文件用excel打开时能正常显示时间数据
                    lines.Add($"{csvDate},=\"{csvTime:HH:mm:ss.fff}\",{mStr}");
                }

                //
                File.WriteAllLines(mePath, lines, System.Text.Encoding.Default);
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
                homFileName = mePath;
                return true;
            }
            catch
            {
                return false;
            }
        }

        //保存记录
        public void SaveUserPrm()
        {
            String mePath = userCFG + @"\user." + userName + ".prm";
            if (File.Exists(mePath))
            {
                System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
            }
            FileStream meFS = new FileStream(mePath, FileMode.Create, FileAccess.Write);
            TextWriter meWrite = new StreamWriter(meFS);

            for (int i = 0; i < myPRM.Count; i++)
            {
                meWrite.WriteLine("ID=" + (i + 1).ToString());
                meWrite.WriteLine("name=" + myPRM[i].name);

                for (int k = 0; k < myPRM[i].myTP.Count; k++)//写入按照放大100写入
                {
                    meWrite.WriteLine("step" + (k + 1).ToString() + "=" + myPRM[i].myTP[k].temperature.ToString() + "," + myPRM[i].myTP[k].time.ToString());
                }
            }

            meWrite.Close();
            meFS.Close();
            System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
        }

        //读取记录
        public void GetUserPrm()
        {
            String mePath = userCFG + @"\user." + userName + ".prm";

            if (File.Exists(mePath))
            {
                String[] meLines = File.ReadAllLines(mePath);

                int id = 0;
                myPRM.Clear();//清空规程

                foreach (String line in meLines)
                {
                    switch (line.Substring(0, line.IndexOf('=')))
                    {
                        case "ID":
                            id = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1)) - 1;
                            break;
                        case "name":
                            while (myPRM.Count <= id)
                            {
                                myPRM.Add(new PRM());
                            }
                            myPRM[id].name = line.Substring(line.IndexOf('=') + 1);
                            break;
                        case "step1":
                            while (myPRM[id].myTP.Count < 1)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[0].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[0].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step2":
                            while (myPRM[id].myTP.Count < 2)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[1].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[1].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step3":
                            while (myPRM[id].myTP.Count < 3)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[2].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[2].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step4":
                            while (myPRM[id].myTP.Count < 4)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[3].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[3].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step5":
                            while (myPRM[id].myTP.Count < 5)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[4].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[4].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step6":
                            while (myPRM[id].myTP.Count < 6)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[5].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[5].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step7":
                            while (myPRM[id].myTP.Count < 7)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[6].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[6].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step8":
                            while (myPRM[id].myTP.Count < 8)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[7].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[7].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step9":
                            while (myPRM[id].myTP.Count < 9)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[8].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[8].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step10":
                            while (myPRM[id].myTP.Count < 10)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[9].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[9].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step11":
                            while (myPRM[id].myTP.Count < 11)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[10].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[10].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step12":
                            while (myPRM[id].myTP.Count < 12)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[11].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[11].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step13":
                            while (myPRM[id].myTP.Count < 13)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[12].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[12].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step14":
                            while (myPRM[id].myTP.Count < 14)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[13].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[13].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step15":
                            while (myPRM[id].myTP.Count < 15)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[14].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[14].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        case "step16":
                            while (myPRM[id].myTP.Count < 16)
                            {
                                myPRM[id].myTP.Add(new tmpoint());
                            }
                            myPRM[id].myTP[15].temperature = Convert.ToInt32(line.Substring(line.IndexOf('=') + 1, line.IndexOf(',') - line.IndexOf('=') - 1));
                            myPRM[id].myTP[15].time = Convert.ToInt32(line.Substring(line.IndexOf(',') + 1));
                            break;
                        default: break;
                    }
                }
            }
            else
            {

                myPRM.Add(new PRM("JJF1527-2015"));
                myPRM[0].myTP.Add(new tmpoint(3000, 60));
                myPRM[0].myTP.Add(new tmpoint(9500, 180));
                myPRM[0].myTP.Add(new tmpoint(3000, 120));
                myPRM[0].myTP.Add(new tmpoint(9000, 180));
                myPRM[0].myTP.Add(new tmpoint(5000, 180));
                myPRM[0].myTP.Add(new tmpoint(7000, 180));
                myPRM[0].myTP.Add(new tmpoint(6000, 180));
                myPRM[0].myTP.Add(new tmpoint(3000, 60));

                myPRM.Add(new PRM("JJF1124-2016"));
                myPRM[1].myTP.Add(new tmpoint(3000, 60));
                myPRM[1].myTP.Add(new tmpoint(9500, 60));
                myPRM[1].myTP.Add(new tmpoint(3000, 60));
                myPRM[1].myTP.Add(new tmpoint(9000, 60));
                myPRM[1].myTP.Add(new tmpoint(5000, 60));
                myPRM[1].myTP.Add(new tmpoint(7000, 60));
                myPRM[1].myTP.Add(new tmpoint(6000, 60));
                myPRM[1].myTP.Add(new tmpoint(3000, 60));

                if (File.Exists(mePath))
                {
                    System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
                }
                FileStream meFS = new FileStream(mePath, FileMode.Create, FileAccess.Write);
                TextWriter meWrite = new StreamWriter(meFS);
                meWrite.WriteLine("ID=1");
                meWrite.WriteLine("name=JJF1527-2015");
                meWrite.WriteLine("step1=30,60");
                meWrite.WriteLine("step2=95,180");
                meWrite.WriteLine("step3=30,120");
                meWrite.WriteLine("step4=90,180");
                meWrite.WriteLine("step5=50,180");
                meWrite.WriteLine("step6=70,180");
                meWrite.WriteLine("step7=60,180");
                meWrite.WriteLine("step8=30,60");
                meWrite.WriteLine("ID=2");
                meWrite.WriteLine("name=JJF1124-2016");
                meWrite.WriteLine("step1=30,60");
                meWrite.WriteLine("step2=95,60");
                meWrite.WriteLine("step3=30,60");
                meWrite.WriteLine("step4=90,60");
                meWrite.WriteLine("step5=50,60");
                meWrite.WriteLine("step6=70,60");
                meWrite.WriteLine("step7=60,60");
                meWrite.WriteLine("step8=30,60");
                meWrite.Close();
                meFS.Close();
                System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
            }

            //检查温度点是否放大100倍
            for (int i = 0; i < myPRM.Count; i++)
            {
                for (int j = 0; j < myPRM[i].myTP.Count; j++)
                {
                    if (myPRM[i].myTP[j].temperature <= 100)
                    {
                        myPRM[i].myTP[j].temperature *= 100;
                    }
                }
            }
        }

        private void rxBuff_Read_Inc()
        {
            rxRead++;
            rxCount--;

            while (rxRead >= SZ.RxSize)
            {
                rxRead = (Int16)(rxRead % SZ.RxSize);
            }
        }

        private void rxBuff_Read_Inc(Int16 num)
        {
            rxRead += num;
            rxCount -= num;

            while (rxRead >= SZ.RxSize)
            {
                rxRead = (Int16)(rxRead % SZ.RxSize);
            }
        }

        private void rxBuff_Write_Inc()
        {
            rxWrite++;
            rxCount++;

            while (rxWrite >= SZ.RxSize)
            {
                rxWrite = (Int16)(rxWrite % SZ.RxSize);
            }

            if (rxRead == rxWrite)
            {
                rxBuff_Read_Inc();
            }
        }

        private Byte rxBuff(Int16 idx)
        {
            idx += rxRead;

            if (idx >= SZ.RxSize)
            {
                return meRXD[idx % SZ.RxSize];
            }
            else
            {
                return meRXD[idx];
            }
        }

        private UInt32 getShift(byte b1, byte b2, byte b3, byte b4, byte b5)
        {
            UInt32 d1 = b1;//高位
            UInt32 d2 = b2;
            UInt32 d3 = b3;
            UInt32 d4 = b4;
            UInt32 d5 = b5;

            d1 = (d1 & 0x7F) << 28;
            d2 = (d2 & 0x7F) << 21;
            d3 = (d3 & 0x7F) << 14;
            d4 = (d4 & 0x7F) << 7;
            d5 = (d5 & 0x7F);

            return (d1 + d2 + d3 + d4 + d5);
        }

        private UInt32 getValue(byte b1, byte b2, byte b3, byte b4, byte b5)
        {
            UInt32 d1 = b1;//高位
            UInt32 d2 = b2;
            UInt32 d3 = b3;
            UInt32 d4 = b4;
            UInt32 d5 = b5;

            d1 -= 128;
            d2 -= 128;
            d3 -= 128;
            d4 -= 128;
            d5 -= 128;

            return ((d1 * 100000000) + (d2 * 1000000) + (d3 * 10000) + (d4 * 100) + d5);
        }

        private UInt32 getValue(byte b1, byte b2, byte b3, byte b4)
        {
            UInt32 d1 = b1;//高位
            UInt32 d2 = b2;
            UInt32 d3 = b3;
            UInt32 d4 = b4;

            d1 -= 128;
            d2 -= 128;
            d3 -= 128;
            d4 -= 128;

            return ((d1 * 1000000) + (d2 * 10000) + (d3 * 100) + d4);
        }

        private UInt32 getValue(byte b1, byte b2, byte b3)
        {
            UInt32 d1 = b1;//高位
            UInt32 d2 = b2;
            UInt32 d3 = b3;

            d1 -= 128;
            d2 -= 128;
            d3 -= 128;

            return ((d1 * 10000) + (d2 * 100) + d3);
        }

        private UInt32 getValue(byte b1, byte b2)
        {
            UInt32 d1 = b1;//高位
            UInt32 d2 = b2;

            d1 -= 128;
            d2 -= 128;

            return ((d1 * 100) + d2);
        }

        private UInt32 getValue(byte b1)
        {
            UInt32 d1 = b1;

            d1 -= 128;

            return (d1);
        }

        private void checkSpeedSecond()
        {
            switch (mrect)
            {
                default:
                case RECT.NULL:
                    if (mtp.outAvg < 4800)
                    {
                        mrect = RECT.up_45;
                    }
                    else if (mtp.outAvg > 9200)
                    {
                        mrect = RECT.dn_95;
                    }
                    break;
                case RECT.up_45:
                    if (mtp.outAvg > 5000)
                    {
                        mrect = RECT.up_50;
                        dt50 = System.DateTime.Now;
                    }
                    break;
                case RECT.up_50:
                    if (mtp.outAvg > 9000)
                    {
                        mrect = RECT.up_90;
                        dt90 = System.DateTime.Now;
                    }
                    break;
                case RECT.up_90:
                    if (mtp.outAvg > 9200)
                    {
                        mrect = RECT.dn_95;
                        upsecond = (int)(dt90 - dt50).TotalSeconds;
                    }
                    break;
                case RECT.dn_95:
                    if (mtp.outAvg < 9000)
                    {
                        mrect = RECT.dn_90;
                        dt90 = System.DateTime.Now;
                    }
                    break;
                case RECT.dn_90:
                    if (mtp.outAvg < 5000)
                    {
                        mrect = RECT.dn_50;
                        dt50 = System.DateTime.Now;
                    }
                    break;
                case RECT.dn_50:
                    if (mtp.outAvg < 4800)
                    {
                        mrect = RECT.up_45;
                        dnsecond = (int)(dt50 - dt90).TotalSeconds;
                    }
                    break;
            }
        }

        //串口接收
        private void mePort_ReceiveFlashLock()
        {
            //读
            meStr += mePort.ReadExisting();

            //判断协议
            if (meStr.Contains("FLASH LOCK"))
            {
                rtCOM = RTCOM.COM_NULL;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //串口接收
        private void mePort_ReceiveFlashUnlock()
        {
            //读取Byte
            meStr += mePort.ReadExisting();

            //判断协议
            if (meStr.Contains("FLASH UNLOCK"))
            {
                rtCOM = RTCOM.COM_NULL;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //串口接收
        private void mePort_ReceiveFlashReset()
        {
            //读取Byte
            meStr += mePort.ReadExisting();

            //判断协议
            if (meStr.Contains("FLASH RESET"))
            {
                rtCOM = RTCOM.COM_NULL;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //串口接收
        private void mePort_ReceiveSetSeri()
        {
            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if (meRXD[rxRead] == Constants.START)
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //长度
            if (rxCount < 10)
            {
                return;
            }

            //协议
            if (rxBuff(9) != Constants.STOP)
            {
                rxBuff_Read_Inc();
                mePort_ReceiveSetSeri();//帧尾不对循环
                return;
            }

            //协议
            if ((rxBuff(1) == Constants.SET_SERI) && (rxBuff(2) == 0x05))
            {
                if (e_seri == getValue(rxBuff(3), rxBuff(4), rxBuff(5), rxBuff(6), rxBuff(7)))
                {
                    rtCOM = RTCOM.COM_NULL;
                }
                rxBuff_Read_Inc(10);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveSetSeri();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //串口接收
        private void mePort_ReceiveSetDot()
        {
            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if (meRXD[rxRead] == Constants.START)
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //长度
            if (rxCount < 81)
            {
                return;
            }

            //协议
            if (rxBuff(80) != Constants.STOP)
            {
                rxBuff_Read_Inc();
                mePort_ReceiveSetDot();//帧尾不对循环
                return;
            }

            //协议
            if ((rxBuff(1) == Constants.SET_DOT) && (rxBuff(2) == 0x4C))
            {
                int k;

                rtCOM = RTCOM.COM_NULL;

                switch (rxBuff(3))
                {
                    default:
                        break;
                    case 0x80:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T10[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A10[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                    case 0x81:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T30[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A30[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                    case 0x82:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T50[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A50[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                    case 0x83:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T60[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A60[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                    case 0x84:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T70[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A70[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                    case 0x85:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T90[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A90[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                    case 0x86:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T95[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A95[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                }
                vitoUpdate();
                rxBuff_Read_Inc(81);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveSetDot();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //串口接收
        private void mePort_ReceiveSetPROBE()
        {
            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if (meRXD[rxRead] == Constants.START)
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //长度
            if (rxCount < 41)
            {
                return;
            }

            //协议
            if (rxBuff(40) != Constants.STOP)
            {
                rxBuff_Read_Inc();
                mePort_ReceiveSetPROBE();//帧尾不对循环
                return;
            }

            //协议
            if ((rxBuff(1) == Constants.SET_PROBE) && (rxBuff(2) == 0x24))
            {
                int k = 4;
                int index = rxBuff(3) - (Byte)PROBE.PROBE1;
                rtCOM = RTCOM.COM_NULL;

                T10[index] = (ushort)getValue(rxBuff((short)k++), rxBuff((short)(k++)));
                A10[index] = getValue(rxBuff((short)(k++)), rxBuff((short)(k++)), rxBuff((short)(k++)));
                T30[index] = (ushort)getValue(rxBuff((short)k++), rxBuff((short)(k++)));
                A30[index] = getValue(rxBuff((short)(k++)), rxBuff((short)(k++)), rxBuff((short)(k++)));
                T50[index] = (ushort)getValue(rxBuff((short)k++), rxBuff((short)(k++)));
                A50[index] = getValue(rxBuff((short)(k++)), rxBuff((short)(k++)), rxBuff((short)(k++)));
                T60[index] = (ushort)getValue(rxBuff((short)k++), rxBuff((short)(k++)));
                A60[index] = getValue(rxBuff((short)(k++)), rxBuff((short)(k++)), rxBuff((short)(k++)));
                T70[index] = (ushort)getValue(rxBuff((short)k++), rxBuff((short)(k++)));
                A70[index] = getValue(rxBuff((short)(k++)), rxBuff((short)(k++)), rxBuff((short)(k++)));
                T90[index] = (ushort)getValue(rxBuff((short)k++), rxBuff((short)(k++)));
                A90[index] = getValue(rxBuff((short)(k++)), rxBuff((short)(k++)), rxBuff((short)(k++)));
                T95[index] = (ushort)getValue(rxBuff((short)k++), rxBuff((short)(k++)));
                A95[index] = getValue(rxBuff((short)(k++)), rxBuff((short)(k++)), rxBuff((short)(k++)));

                vitoUpdate();
                rxBuff_Read_Inc(41);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveSetDot();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //串口接收
        private void mePort_ReceiveSetMem()
        {
            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if (meRXD[rxRead] == Constants.START)
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //长度
            if (rxCount < 6)
            {
                return;
            }

            //协议
            if (rxBuff(1) == Constants.SET_MEM)
            {
                switch (rxBuff(2))
                {
                    default:
                        rxBuff_Read_Inc();
                        mePort_ReceiveSetMem();//协议不对循环
                        return;

                    case 0x01:
                        if (rxBuff(5) != Constants.STOP)
                        {
                            rxBuff_Read_Inc();
                            mePort_ReceiveSetMem();//帧尾不对循环
                            return;
                        }
                        else
                        {
                            if (rxBuff(3) == 0xEE)//无Flash
                            {
                                rtCOM = RTCOM.COM_ERR_MEM;
                                rxBuff_Read_Inc(6);
                            }
                            else
                            {
                                rxBuff_Read_Inc();
                                mePort_ReceiveSetMem();//帧尾不对循环
                                return;
                            }
                        }
                        break;

                    case 0x12:
                        //
                        if (rxCount < 23)
                        {
                            //mePort_Send_Repeat();//不满长度重发
                            return;
                        }
                        //
                        if (rxBuff(22) != Constants.STOP)
                        {
                            rxBuff_Read_Inc();
                            mePort_ReceiveSetMem();//帧尾不对循环
                            return;
                        }
                        //
                        rtCOM = RTCOM.COM_NULL;
                        if (mode != (TmpMode)getValue(rxBuff(3)))
                        {
                            rtCOM = RTCOM.COM_SET_MEM;
                        }
                        else if (MyDefine.myXET.devVersion > 0)
                        {
                            MyDefine.myXET.devStatus = mode;//设置工作模式后更新状态
                        }
                        if (value != getValue(rxBuff(4), rxBuff(5)))
                        {
                            rtCOM = RTCOM.COM_SET_MEM;
                        }
                        if (delay != getValue(rxBuff(6), rxBuff(7), rxBuff(8), rxBuff(9)))
                        {
                            rtCOM = RTCOM.COM_SET_MEM;
                        }
                        if (date != getValue(rxBuff(10), rxBuff(11), rxBuff(12), rxBuff(13)))
                        {
                            rtCOM = RTCOM.COM_SET_MEM;
                        }
                        if (time != getValue(rxBuff(14), rxBuff(15), rxBuff(16)))
                        {
                            rtCOM = RTCOM.COM_SET_MEM;
                        }
                        if (rtCOM == RTCOM.COM_NULL)
                        {
                            max = (ushort)getValue(rxBuff(18), rxBuff(19));
                            min = (ushort)getValue(rxBuff(20), rxBuff(21));
                        }
                        rxBuff_Read_Inc(23);
                        break;
                }
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveSetMem();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //串口接收
        private void mePort_ReceiveReadTvs()
        {
            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if (meRXD[rxRead] == Constants.START)
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //长度
            if (rxCount < 20)
            {
                return;
            }

            //协议
            if (rxBuff(24) != Constants.STOP && rxBuff(28) != Constants.STOP)
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadTvs();//帧尾不对循环
                return;
            }

            //协议
            if ((rxBuff(1) == Constants.READ_TVS) && (rxBuff(2) == 0x14))
            {
                e_sidh = getValue(rxBuff(3), rxBuff(4), rxBuff(5), rxBuff(6), rxBuff(7));
                e_sidm = getValue(rxBuff(8), rxBuff(9), rxBuff(10), rxBuff(11), rxBuff(12));
                e_sidl = getValue(rxBuff(13), rxBuff(14), rxBuff(15), rxBuff(16), rxBuff(17));
                e_seri = getValue(rxBuff(18), rxBuff(19), rxBuff(20), rxBuff(21), rxBuff(22));
                rtCOM = RTCOM.COM_READ_PAR;//继续读参数
                rxBuff_Read_Inc(25);
            }
            else if ((rxBuff(1) == Constants.READ_TVS) && (rxBuff(2) == 0x18))
            {
                e_sidh = getValue(rxBuff(3), rxBuff(4), rxBuff(5), rxBuff(6), rxBuff(7));
                e_sidm = getValue(rxBuff(8), rxBuff(9), rxBuff(10), rxBuff(11), rxBuff(12));
                e_sidl = getValue(rxBuff(13), rxBuff(14), rxBuff(15), rxBuff(16), rxBuff(17));
                e_seri = getValue(rxBuff(18), rxBuff(19), rxBuff(20), rxBuff(21), rxBuff(22));
                e_type = rxBuff(23);
                e_fv = rxBuff(24);
                e_hwv = rxBuff(25);
                rtCOM = RTCOM.COM_READ_PAR;//继续读参数
                rxBuff_Read_Inc(29);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadTvs();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //串口接收
        private void mePort_ReceiveReadPar()
        {
            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if (meRXD[rxRead] == Constants.START)
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //长度
            if (rxCount < 9)
            {
                return;
            }

            //协议
            if ((rxBuff(8) != Constants.STOP) && (rxBuff(9) != Constants.STOP))
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadPar();//帧尾不对循环
                return;
            }

            //协议
            if (rxBuff(1) == Constants.READ_PAR)
            {
                if (rxBuff(2) == 0x04)
                {
                    battery = (ushort)getValue(rxBuff(3), rxBuff(4));
                    temperature = (ushort)getValue(rxBuff(5), rxBuff(6));

                    //
                    devVersion = 0;

                    //
                    rtCOM = RTCOM.COM_NULL;
                    rxBuff_Read_Inc(9);
                }
                else if (rxBuff(2) == 0x05)
                {
                    battery = (ushort)getValue(rxBuff(3), rxBuff(4));
                    temperature = (ushort)getValue(rxBuff(5), rxBuff(6));
                    devStatus = (TmpMode)(rxBuff(7) & 0x7F);

                    //TVS_F6开始多加1字节的devStatus
                    devVersion = 1;

                    //
                    rtCOM = RTCOM.COM_NULL;
                    rxBuff_Read_Inc(10);
                }
                else
                {
                    rxBuff_Read_Inc();
                    mePort_ReceiveReadPar();//校验不对循环
                    return;
                }
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadPar();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //串口接收
        private void mePort_ReceiveReadDot()
        {
            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if (meRXD[rxRead] == Constants.START)
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //长度
            if (rxCount < 81)
            {
                return;
            }

            //协议
            if (rxBuff(80) != Constants.STOP)
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadDot();//帧尾不对循环
                return;
            }

            //协议
            if ((rxBuff(1) == Constants.READ_DOT) && (rxBuff(2) == 0x4C))
            {
                int k;

                rtCOM = RTCOM.COM_NULL;

                switch (rxBuff(3))
                {
                    default:
                        break;
                    case 0x80:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T10[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A10[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                    case 0x81:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T30[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A30[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                    case 0x82:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T50[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A50[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                    case 0x83:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T60[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A60[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                    case 0x84:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T70[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A70[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                    case 0x85:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T90[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A90[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                    case 0x86:
                        for (int i = 0; i < SZ.CHA; i++)
                        {
                            k = i * 5 + 4;
                            T95[i] = (ushort)getValue(rxBuff((short)k), rxBuff((short)(k + 1)));
                            A95[i] = getValue(rxBuff((short)(k + 2)), rxBuff((short)(k + 3)), rxBuff((short)(k + 4)));
                        }
                        break;
                }
                vitoUpdate();
                rxBuff_Read_Inc(81);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadDot();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //串口接收
        private void mePort_ReceiveReadTmp()
        {
            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if (meRXD[rxRead] == Constants.START)
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //长度
            if (rxCount < 245)
            {
                return;
            }

            //协议
            if (rxBuff(244) != Constants.STOP)
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadTmp();//帧尾不对循环
                return;
            }

            //协议
            if ((rxBuff(1) == Constants.READ_TMP) && (rxBuff(2) == 0xF0))
            {
                int m;
                int n;
                int offset;
                int idx;
                int t1;
                int erro;//记录探头异常数
                //校验
                Byte dat = 0;
                Byte crc = 0x59;
                for (Int16 i = 2; i < 243; i++)
                {
                    dat = rxBuff(i);

                    if (dat < 0x80)
                    {
                        return;
                    }

                    crc += dat;
                }
                if (crc != rxBuff(243))
                {
                    return;
                }

                //
                for (m = 0; m < 8; m++)//组
                {
                    offset = m * 30 + 3;
                    erro = 0;
                    new_index = mySyn.Count - 1;

                    for (n = 0; n < SZ.CHA; n++)//通道
                    {
                        idx = (n << 1) + offset;

                        mtp.OUT[n] = (int)getValue(rxBuff((short)idx), rxBuff((short)(idx + 1)));//温度

                        if (new_index >= 2)
                        {
                            t1 = mySyn[new_index].OUT[n] - mySyn[old_index].OUT[n];
                            if (Math.Abs(t1) >= 500)
                            {
                                erro++;
                            }
                        }

                    }
                    if (erro == SZ.CHA)
                    {
                        count++;
                    }
                    else
                    {
                        if (count != 0)
                        {
                            if (count <= 10)
                            {
                                for (int i = old_index + 1; i < new_index; i++)
                                {
                                    mySyn[i] = mySyn[old_index];
                                }
                            }
                        }
                        count = 0;
                        old_index = new_index;
                    }

                    if (MyDefine.myXET.myType == TYPE.TVS8)
                    {
                        int[] OutTVS8 = new int[8];
                        Array.Copy(mtp.OUT, OutTVS8, 8);

                        mtp.outMax = OutTVS8.Max();
                        mtp.outMin = OutTVS8.Min();
                        mtp.outDif = mtp.outMax - mtp.outMin;
                        mtp.outAvg = (int)OutTVS8.Average();
                        mtp.outStd = getStdevpTVS8(OutTVS8);
                        mtp.outRnd = getRound(mtp.outAvg);
                        mtp.outDep = getDepartureTVS8(OutTVS8);
                    }
                    else
                    {
                        mtp.outMax = mtp.OUT.Max();
                        mtp.outMin = mtp.OUT.Min();
                        mtp.outDif = mtp.outMax - mtp.outMin;
                        mtp.outAvg = (int)mtp.OUT.Average();
                        mtp.outStd = getStdevp(mtp.OUT);
                        mtp.outRnd = getRound(mtp.outAvg);
                        mtp.outDep = getDeparture(mtp.OUT);
                    }
                    checkSpeedSecond();

                    if (oldrnd < mtp.outRnd)
                    {
                        strend = true;//上升
                        oldrnd = mtp.outRnd;
                        overmax = SZ.TMIN;
                        overavg = SZ.TMIN;
                        outdim = 0;
                        outstm = 0;
                    }
                    else if (oldrnd > mtp.outRnd)
                    {
                        strend = false;//下降
                        oldrnd = mtp.outRnd;
                        overmax = SZ.TMAX;
                        overavg = SZ.TMAX;
                        outdim = 0;
                        outstm = 0;
                    }
                    else
                    {
                        if (strend)
                        {
                            if ((mtp.outMax > mtp.outRnd) && (mtp.outMax > overmax))
                            {
                                overmax = mtp.outMax;
                            }
                            if ((mtp.outAvg > mtp.outRnd) && (mtp.outAvg > overavg))
                            {
                                overavg = mtp.outAvg;
                            }
                        }
                        else
                        {
                            if ((mtp.outMax < mtp.outRnd) && (mtp.outMax < overmax))
                            {
                                overmax = mtp.outMax;
                            }
                            if ((mtp.outAvg < mtp.outRnd) && (mtp.outAvg < overavg))
                            {
                                overavg = mtp.outAvg;
                            }
                        }
                    }

                    if (outdim < mtp.outDif)
                    {
                        outdim = mtp.outDif;
                    }

                    if (outstm < mtp.outStd)
                    {
                        outstm = mtp.outStd;
                    }

                    if (tmpMax < mtp.outMax)
                    {
                        tmpMax = mtp.outMax;
                        isAxis = true;
                    }

                    if (tmpMin > mtp.outMin)
                    {
                        tmpMin = mtp.outMin;
                        isAxis = true;
                    }

                    mySyn.Add(new TMP(mtp));
                    if (mySyn.Count > 0x20000)
                    {
                        mySyn.RemoveAt(0);
                    }
                }

                //
                rtCOM = RTCOM.COM_NULL;
                //
                rxBuff_Read_Inc(245);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadTmp();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //串口接收
        private void mePort_ReceiveReadMem()
        {
            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if (meRXD[rxRead] == Constants.START)
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //长度
            if (rxCount < 6)
            {
                return;
            }

            //协议
            if (rxBuff(1) == Constants.READ_MEM)
            {
                switch (rxBuff(2))
                {
                    default:
                        rxBuff_Read_Inc();
                        mePort_ReceiveReadMem();//协议不对循环
                        return;

                    case 0x01: //1
                        if (rxBuff(5) != Constants.STOP)
                        {
                            rxBuff_Read_Inc();
                            mePort_ReceiveReadMem();//帧尾不对循环
                            return;
                        }
                        else
                        {
                            if (rxBuff(3) == 0xFF)//读取结束
                            {
                                memtimeUpdate();
                                rtCOM = RTCOM.COM_NULL;
                                rxBuff_Read_Inc(6);
                            }
                            else if (rxBuff(3) == 0xEE)//无Flash
                            {
                                rtCOM = RTCOM.COM_ERR_MEM;
                                rxBuff_Read_Inc(6);
                            }
                            else
                            {
                                rxBuff_Read_Inc();
                                mePort_ReceiveReadMem();//帧尾不对循环
                                return;
                            }
                        }
                        break;

                    case 0x37: //55
                        //
                        if (rxCount < 60)
                        {
                            //mePort_Send_Repeat();//不满长度重发
                            return;
                        }
                        //
                        if (rxBuff(59) != Constants.STOP)
                        {
                            rxBuff_Read_Inc();
                            mePort_ReceiveReadMem();//帧尾不对循环
                            return;
                        }
                        else
                        {
                            memmode = (TmpMode)getValue(rxBuff(24));
                            memvalue = getValue(rxBuff(25), rxBuff(26));
                            memdelay = getShift(rxBuff(27), rxBuff(28), rxBuff(29), rxBuff(30), rxBuff(31));
                            memdate = getShift(rxBuff(32), rxBuff(33), rxBuff(34), rxBuff(35), rxBuff(36));
                            memtime = getShift(rxBuff(37), rxBuff(38), rxBuff(39), rxBuff(40), rxBuff(41));
                            memstop = memtime;
                            memstart = getShift(rxBuff(42), rxBuff(43), rxBuff(44), rxBuff(45), rxBuff(46));
                            memtotal = (int)getShift(rxBuff(47), rxBuff(48), rxBuff(49), rxBuff(50), rxBuff(51)) / 256;
                            myMem.Clear();
                            if (memdate == 0xFFFFFFFF)//norFlash无数据,结束read mem
                            {
                                rtCOM = RTCOM.COM_NULL;
                            }
                            rxBuff_Read_Inc(60);
                        }
                        break;

                    case 0xF5: //245
                        //不满帧
                        if (rxBuff((short)(rxCount - 1)) != Constants.STOP)
                        {
                            return;
                        }
                        //250字节
                        if (rxCount == 250)
                        {
                            int m;
                            int n;
                            int offset;
                            int idx;
                            int t1;
                            int erro;//记录探头异常数

                            for (m = 0; m < 8; m++)//组
                            {
                                offset = m * 30 + 3;
                                erro = 0;
                                new_index = myMem.Count - 1;//记录新的数据下标

                                for (n = 0; n < SZ.CHA; n++)//通道
                                {
                                    idx = (n << 1) + offset;

                                    mtp.OUT[n] = (int)getValue(rxBuff((short)idx), rxBuff((short)(idx + 1)));//温度

                                    if (new_index >= 2)
                                    {
                                        t1 = myMem[new_index].OUT[n] - myMem[old_index].OUT[n];
                                        if (Math.Abs(t1) >= 500)
                                        {
                                            erro++;
                                        }
                                    }
                                }
                                if (erro == SZ.CHA)
                                {
                                    count++;
                                }
                                else
                                {
                                    if (count != 0)
                                    {
                                        if (count <= 10)
                                        {
                                            for (int i = old_index + 1; i < new_index; i++)
                                            {
                                                myMem[i] = myMem[old_index];
                                            }
                                        }
                                    }
                                    count = 0;
                                    old_index = new_index;
                                }

                                myMem.Add(new TMP(mtp));

                                if (myMem.Count > 0x20000)
                                {
                                    myMem.RemoveAt(0);
                                }
                            }

                            memrun = getShift(rxBuff(243), rxBuff(244), rxBuff(245), rxBuff(246), rxBuff(247));
                            rxBuff_Read_Inc(250);
                        }
                        // BLE接收缺少字节, 使用历史数据
                        else
                        {
                            errCount++;

                            myMem.Add(new TMP(mtp));

                            if (myMem.Count > 0x20000)
                            {
                                myMem.RemoveAt(0);
                            }

                            rxBuff_Read_Inc(rxCount);
                        }
                        break;
                }
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveReadMem();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //串口接收
        private void mePort_ReceiveClearMem()
        {
            //读
            while (mePort.BytesToRead > 0)
            {
                meRXD[rxWrite] = (byte)mePort.ReadByte();

                rxBuff_Write_Inc();
            }

            //帧头
            while (rxCount > 0)
            {
                if (meRXD[rxRead] == Constants.START)
                {
                    break;
                }
                else
                {
                    rxBuff_Read_Inc();
                }
            }

            //长度
            if (rxCount < 5)
            {
                return;
            }

            //协议
            if (rxBuff(4) != Constants.STOP)
            {
                rxBuff_Read_Inc();
                mePort_ReceiveClearMem();//帧尾不对循环
                return;
            }

            //协议
            if ((rxBuff(1) == Constants.CLEAR_MEM) && (rxBuff(2) == 0x00))
            {
                rtCOM = RTCOM.COM_NULL;
                rxBuff_Read_Inc(5);
            }
            else
            {
                rxBuff_Read_Inc();
                mePort_ReceiveClearMem();//校验不对循环
                return;
            }

            //委托
            if (myUpdate != null)
            {
                myUpdate();
            }
        }

        //接收触发函数,实际会由串口线程创建
        private void mePort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            switch (rtCOM)
            {
                default:
                    break;
                case RTCOM.COM_SET_LOCK:
                    mePort_ReceiveFlashLock();
                    break;
                case RTCOM.COM_SET_UNLOCK:
                    mePort_ReceiveFlashUnlock();
                    break;
                case RTCOM.COM_SET_RESET:
                    mePort_ReceiveFlashReset();
                    break;
                case RTCOM.COM_SET_SERI:
                    mePort_ReceiveSetSeri();
                    break;
                case RTCOM.COM_SET_DOT:
                    mePort_ReceiveSetDot();
                    break;
                case RTCOM.COM_SET_PROBE:
                    mePort_ReceiveSetPROBE();
                    break;
                case RTCOM.COM_SET_MEM:
                    mePort_ReceiveSetMem();
                    break;
                case RTCOM.COM_READ_TVS:
                    mePort_ReceiveReadTvs();
                    break;
                case RTCOM.COM_READ_PAR:
                    mePort_ReceiveReadPar();
                    break;
                case RTCOM.COM_READ_DOT:
                    mePort_ReceiveReadDot();
                    break;
                case RTCOM.COM_READ_TMP:
                    mePort_ReceiveReadTmp();
                    break;
                case RTCOM.COM_READ_MEM:
                    mePort_ReceiveReadMem();
                    break;
                case RTCOM.COM_CLEAR_MEM:
                    mePort_ReceiveClearMem();
                    break;
            }
        }

        //发送
        public bool mePort_Send_BleReset()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //rtCOM = RTCOM.COM_ERR_RST;
                meTXD[0] = Constants.START;
                meTXD[1] = Constants.ERR_RST;
                meTXD[2] = 0;
                meTXD[3] = Constants.ERR_RST;
                meTXD[4] = Constants.STOP;
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_Repeat()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                //rtCOM = RTCOM.COM_ERR_RPT;
                meTXD[0] = Constants.START;
                meTXD[1] = Constants.ERR_RPT;
                meTXD[2] = 0;
                meTXD[3] = Constants.ERR_RPT;
                meTXD[4] = Constants.STOP;
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_FlashLock()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                rtCOM = RTCOM.COM_SET_LOCK;
                meStr = "";
                meTXD[0] = Constants.START;
                meTXD[1] = Constants.SET_LOCK;
                meTXD[2] = 0;
                meTXD[3] = Constants.SET_LOCK;
                meTXD[4] = Constants.STOP;
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_FlashUnlock()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                rtCOM = RTCOM.COM_SET_UNLOCK;
                meStr = "";
                meTXD[0] = Constants.START;
                meTXD[1] = Constants.SET_UNLOCK;
                meTXD[2] = 0;
                meTXD[3] = Constants.SET_UNLOCK;
                meTXD[4] = Constants.STOP;
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_FlashReset()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                rtCOM = RTCOM.COM_SET_RESET;
                meStr = "";
                meTXD[0] = Constants.START;
                meTXD[1] = Constants.SET_RESET;
                meTXD[2] = 0;
                meTXD[3] = Constants.SET_RESET;
                meTXD[4] = Constants.STOP;
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_SetSeri()
        {
            UInt32 dat = 0;
            Byte num = 0;
            Byte sum = 0;

            //串口发送
            if (mePort.IsOpen == true)
            {
                e_seri = Convert.ToUInt32(System.DateTime.Now.ToString("yyMMddHHmm"));
                rtCOM = RTCOM.COM_SET_SERI;
                meTXD[num++] = Constants.START;
                meTXD[num++] = Constants.SET_SERI;
                meTXD[num++] = 5;
                dat = 128 + (e_seri / 100000000) % 100;//高位
                meTXD[num++] = (byte)dat;
                dat = 128 + (e_seri / 1000000) % 100;
                meTXD[num++] = (byte)dat;
                dat = 128 + (e_seri / 10000) % 100;
                meTXD[num++] = (byte)dat;
                dat = 128 + (e_seri / 100) % 100;
                meTXD[num++] = (byte)dat;
                dat = 128 + e_seri % 100;
                meTXD[num++] = (byte)dat;
                for (byte i = 1; i < num; i++)
                {
                    sum += meTXD[i];
                }
                meTXD[num++] = sum;
                meTXD[num++] = Constants.STOP;
                mePort.Write(meTXD, 0, num);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_SetDot(DOT dot, UInt16 tmp)
        {
            Byte num = 0;
            Byte sum = 0;

            //串口发送
            if (mePort.IsOpen == true)
            {
                rtCOM = RTCOM.COM_SET_DOT;
                meTXD[num++] = Constants.START;
                meTXD[num++] = Constants.SET_DOT;
                meTXD[num++] = 3;
                meTXD[num++] = (byte)dot;
                meTXD[num++] = (byte)(0x80 | (tmp / 100));//高位
                meTXD[num++] = (byte)(0x80 | (tmp % 100));//小数位
                for (byte i = 1; i < num; i++)
                {
                    sum += meTXD[i];
                }
                meTXD[num++] = sum;
                meTXD[num++] = Constants.STOP;
                System.Threading.Thread.Sleep(200);//延时后插入按键发送的指令,防止通讯过快BLE死机
                mePort.Write(meTXD, 0, num);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_SetPROBE(PROBE dot)
        {
            Byte num = 0;
            Byte sum = 0;

            //串口发送
            if (mePort.IsOpen == true)
            {
                rtCOM = RTCOM.COM_SET_PROBE;
                meTXD[num++] = Constants.START;
                meTXD[num++] = Constants.SET_PROBE;
                meTXD[num++] = 0x24;
                meTXD[num++] = (byte)dot;

                int index = (byte)dot - (byte)PROBE.PROBE1;
                meTXD[num++] = (byte)(0x80 | (T10[index] / 100));//高位
                meTXD[num++] = (byte)(0x80 | (T10[index] % 100));//小数位
                meTXD[num++] = (byte)(0x80 | (A10[index] / 10000));//高位
                meTXD[num++] = (byte)(0x80 | (A10[index] / 100 % 100));//中位
                meTXD[num++] = (byte)(0x80 | (A10[index] % 100));//小数位

                meTXD[num++] = (byte)(0x80 | (T30[index] / 100));//高位
                meTXD[num++] = (byte)(0x80 | (T30[index] % 100));//小数位
                meTXD[num++] = (byte)(0x80 | (A30[index] / 10000));//高位
                meTXD[num++] = (byte)(0x80 | (A30[index] / 100 % 100));//中位
                meTXD[num++] = (byte)(0x80 | (A30[index] % 100));//小数位

                meTXD[num++] = (byte)(0x80 | (T50[index] / 100));//高位
                meTXD[num++] = (byte)(0x80 | (T50[index] % 100));//小数位
                meTXD[num++] = (byte)(0x80 | (A50[index] / 10000));//高位
                meTXD[num++] = (byte)(0x80 | (A50[index] / 100 % 100));//中位
                meTXD[num++] = (byte)(0x80 | (A50[index] % 100));//小数位

                meTXD[num++] = (byte)(0x80 | (T60[index] / 100));//高位
                meTXD[num++] = (byte)(0x80 | (T60[index] % 100));//小数位
                meTXD[num++] = (byte)(0x80 | (A60[index] / 10000));//高位
                meTXD[num++] = (byte)(0x80 | (A60[index] / 100 % 100));//中位
                meTXD[num++] = (byte)(0x80 | (A60[index] % 100));//小数位

                meTXD[num++] = (byte)(0x80 | (T70[index] / 100));//高位
                meTXD[num++] = (byte)(0x80 | (T70[index] % 100));//小数位
                meTXD[num++] = (byte)(0x80 | (A70[index] / 10000));//高位
                meTXD[num++] = (byte)(0x80 | (A70[index] / 100 % 100));//中位
                meTXD[num++] = (byte)(0x80 | (A70[index] % 100));//小数位

                meTXD[num++] = (byte)(0x80 | (T90[index] / 100));//高位
                meTXD[num++] = (byte)(0x80 | (T90[index] % 100));//小数位
                meTXD[num++] = (byte)(0x80 | (A90[index] / 10000));//高位
                meTXD[num++] = (byte)(0x80 | (A90[index] / 100 % 100));//中位
                meTXD[num++] = (byte)(0x80 | (A90[index] % 100));//小数位

                meTXD[num++] = (byte)(0x80 | (T95[index] / 100));//高位
                meTXD[num++] = (byte)(0x80 | (T95[index] % 100));//小数位
                meTXD[num++] = (byte)(0x80 | (A95[index] / 10000));//高位
                meTXD[num++] = (byte)(0x80 | (A95[index] / 100 % 100));//中位
                meTXD[num++] = (byte)(0x80 | (A95[index] % 100));//小数位

                for (byte i = 1; i < num; i++)
                {
                    sum += meTXD[i];
                }
                meTXD[num++] = sum;
                meTXD[num++] = Constants.STOP;
                System.Threading.Thread.Sleep(200);//延时后插入按键发送的指令,防止通讯过快BLE死机
                mePort.Write(meTXD, 0, num);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_SetMem()
        {
            UInt16 dat = 0;
            Byte num = 0;
            Byte sum = 0;

            //串口发送
            if (mePort.IsOpen == true)
            {
                rtCOM = RTCOM.COM_SET_MEM;
                meTXD[num++] = Constants.START;
                meTXD[num++] = Constants.SET_MEM;
                meTXD[num++] = 15;
                meTXD[num++] = (byte)mode;
                meTXD[num++] = (byte)(0x80 | (value / 100));//高位
                meTXD[num++] = (byte)(0x80 | (value % 100));//小数位
                dat = (ushort)(128 + (delay / 1000000) % 100);
                meTXD[num++] = (byte)dat;
                dat = (ushort)(128 + (delay / 10000) % 100);
                meTXD[num++] = (byte)dat;
                dat = (ushort)(128 + (delay / 100) % 100);
                meTXD[num++] = (byte)dat;
                dat = (ushort)(128 + delay % 100);
                meTXD[num++] = (byte)dat;
                dat = (ushort)(128 + (date / 1000000) % 100);//高位
                meTXD[num++] = (byte)dat;
                dat = (ushort)(128 + (date / 10000) % 100);
                meTXD[num++] = (byte)dat;
                dat = (ushort)(128 + (date / 100) % 100);
                meTXD[num++] = (byte)dat;
                dat = (ushort)(128 + date % 100);
                meTXD[num++] = (byte)dat;
                dat = (ushort)(128 + (time / 1000000) % 100);//高位
                meTXD[num++] = (byte)dat;
                dat = (ushort)(128 + (time / 10000) % 100);
                meTXD[num++] = (byte)dat;
                dat = (ushort)(128 + (time / 100) % 100);
                meTXD[num++] = (byte)dat;
                dat = (ushort)(128 + time % 100);
                meTXD[num++] = (byte)dat;
                for (byte i = 1; i < num; i++)
                {
                    sum += meTXD[i];
                }
                meTXD[num++] = sum;
                meTXD[num++] = Constants.STOP;
                mePort.Write(meTXD, 0, num);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_ReadTvs()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                rtCOM = RTCOM.COM_READ_TVS;
                meTXD[0] = Constants.START;
                meTXD[1] = Constants.READ_TVS;
                meTXD[2] = 0;
                meTXD[3] = Constants.READ_TVS;
                meTXD[4] = Constants.STOP;
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_ReadPar()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                rtCOM = RTCOM.COM_READ_PAR;
                meTXD[0] = Constants.START;
                meTXD[1] = Constants.READ_PAR;
                meTXD[2] = 0;
                meTXD[3] = Constants.READ_PAR;
                meTXD[4] = Constants.STOP;
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_ReadDot(DOT dot)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                rtCOM = RTCOM.COM_READ_DOT;
                meTXD[0] = Constants.START;
                meTXD[1] = Constants.READ_DOT;
                meTXD[2] = 1;
                meTXD[3] = (byte)dot;
                meTXD[4] = (byte)(meTXD[1] + meTXD[2] + meTXD[3]);
                meTXD[5] = Constants.STOP;
                mePort.Write(meTXD, 0, 6);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_ReadTmp()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                rtCOM = RTCOM.COM_READ_TMP;
                meTXD[0] = Constants.START;
                meTXD[1] = Constants.READ_TMP;
                meTXD[2] = 0;
                meTXD[3] = Constants.READ_TMP;
                meTXD[4] = Constants.STOP;
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_ReadMem(Byte wh)
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                rtCOM = RTCOM.COM_READ_MEM;
                meTXD[0] = Constants.START;
                meTXD[1] = Constants.READ_MEM;
                meTXD[2] = 1;
                meTXD[3] = wh;
                meTXD[4] = (byte)(meTXD[1] + meTXD[2] + meTXD[3]);
                meTXD[5] = Constants.STOP;
                mePort.Write(meTXD, 0, 6);
                return true;
            }
            else
            {
                return false;
            }
        }

        //发送
        public bool mePort_Send_ClearMem()
        {
            //串口发送
            if (mePort.IsOpen == true)
            {
                rtCOM = RTCOM.COM_CLEAR_MEM;
                meTXD[0] = Constants.START;
                meTXD[1] = Constants.CLEAR_MEM;
                meTXD[2] = 0;
                meTXD[3] = Constants.CLEAR_MEM;
                meTXD[4] = Constants.STOP;
                mePort.Write(meTXD, 0, 5);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void form_Update()
        {
            //委托
            if (mfUpdate != null)
            {
                mfUpdate();
            }
        }
    }
}

//end
