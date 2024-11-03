using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TVS
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        //账号
        private void AccountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Login_Account("切换用户！");
            this.BringToFront();
        }

        //打印
        private void PrintToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("开发中...");
            this.BringToFront();
            //myClass.ShowDialog();
            //debug
        }

        //退出
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //
            if (MyDefine.myXET.myTask == TASKS.record)
            {
                if (DialogResult.Yes == MessageBox.Show("正在进行检测录制，是否放弃录制保存并关闭系统？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
                {
                    MyDefine.myXET.myTask = TASKS.disconnected;
                }
                else
                {
                    return;
                }
            }

            //关闭串口
            while (MyDefine.myXET.mePort.IsOpen)
            {
                MyDefine.myXET.mePort.Close();
            }

            //退出所有窗口
            System.Environment.Exit(0);
        }

        //设备连接
        private void ConnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuConnectForm myConnectForm = new MenuConnectForm();
            myConnectForm.StartPosition = FormStartPosition.CenterParent;
            myConnectForm.ShowDialog();
            this.BringToFront();
        }

        //设备设置
        private void SetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuSetForm mySetForm = new MenuSetForm();
            mySetForm.StartPosition = FormStartPosition.CenterParent;
            mySetForm.ShowDialog();
            this.BringToFront();
        }

        //设备导出
        private void ReadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuReadForm myReadForm = new MenuReadForm();
            myReadForm.StartPosition = FormStartPosition.CenterParent;
            myReadForm.ShowDialog();
            this.BringToFront();
        }

        //实时监控
        private void RunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form;
            string formName;
            if (MyDefine.myXET.myType == TYPE.TVS15)
            {
                formName = "MenuRunForm";
                form = new MenuRunForm();
            }
            else
            {
                formName = "MenuRunFormTVS8";
                form = new MenuRunFormTVS8();
            }

            foreach (Form fr in this.MdiChildren)
            {
                if (fr.GetType().Name == formName)
                {
                    fr.BringToFront();
                    return;
                }
            }
            form.MdiParent = this;
            form.Show();
            form.WindowState = FormWindowState.Maximized;
        }

        //数据载入
        private void ImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(MyDefine.myXET.userDAT))
            {
                Directory.CreateDirectory(MyDefine.myXET.userDAT);
            }
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "请选择数据";
            fileDialog.Filter = "数据(*.tmp)|*.tmp";
            fileDialog.RestoreDirectory = true;
            fileDialog.InitialDirectory = MyDefine.myXET.userDAT;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {

                String[] meLines = File.ReadAllLines(fileDialog.FileName);

                MyDefine.myXET.homsave = meLines[0].Substring(meLines[0].IndexOf('=') + 1);

                MyDefine.myXET.myHom.Clear();

                int t1;
                int count = 0;//记录异常数据条数
                int erro;//记录探头异常数
                int old_index = 0;//记录异常数据的前一个正常数据的下标
                int new_index;//记录新的数据下标
                foreach (String line in meLines)
                {
                    new_index = MyDefine.myXET.myHom.Count - 1;
                    if (line.Contains(";"))
                    {
                    }
                    else if (line.Contains("="))
                    {
                        switch (line.Substring(0, line.IndexOf('=')))
                        {
                            //
                            case "date":
                                //日期
                                MyDefine.myXET.homdate = Convert.ToUInt32(line.Substring(line.IndexOf('=') + 1));
                                break;
                            case "time":
                                //起始时刻
                                MyDefine.myXET.homtime = Convert.ToUInt32(line.Substring(line.IndexOf('=') + 1));
                                break;
                            case "stop":
                                //停止时刻
                                MyDefine.myXET.homstop = Convert.ToUInt32(line.Substring(line.IndexOf('=') + 1));
                                break;
                            case "duration":
                                //持续时间
                                MyDefine.myXET.homrun = Convert.ToUInt32(line.Substring(line.IndexOf('=') + 1));
                                MyDefine.myXET.homstep = ((float)MyDefine.myXET.homrun / (float)MyDefine.myXET.myHom.Count);
                                break;
                            case "tvsid":
                                MyDefine.myXET.e_sidm = Convert.ToUInt32(line.Substring(line.IndexOf('=') + 1, 8), 16);
                                MyDefine.myXET.e_sidl = Convert.ToUInt32(line.Substring(line.IndexOf('=') + 9, 8), 16);
                                MyDefine.myXET.e_sidh = Convert.ToUInt32(line.Substring(line.IndexOf('=') + 17), 16);
                                MyDefine.myXET.e_sidm_dat = Convert.ToUInt32(line.Substring(line.IndexOf('=') + 1, 8), 16);
                                MyDefine.myXET.e_sidl_dat = Convert.ToUInt32(line.Substring(line.IndexOf('=') + 9, 8), 16);
                                MyDefine.myXET.e_sidh_dat = Convert.ToUInt32(line.Substring(line.IndexOf('=') + 17), 16);
                                break;
                            case "type":
                                //类型
                                MyDefine.myXET.type = Convert.ToByte(line.Substring(line.IndexOf('=') + 1));
                                break;
                            case "tvssn":
                                MyDefine.myXET.e_seri = Convert.ToUInt32(line.Substring(line.IndexOf('=') + 1));
                                if (MyDefine.myXET.myHom.Count > 0)
                                {
                                    MessageBox.Show("成功载入" + (MyDefine.myXET.homrun / 1000).ToString() + "秒" + MyDefine.myXET.myHom.Count.ToString() + "点数据");

                                    MyDefine.myXET.homFileName = fileDialog.FileName;

                                    //起始时间
                                    DateTime mDate = new DateTime(((int)MyDefine.myXET.homdate / 10000) + 2000,
                                        ((int)MyDefine.myXET.homdate / 100) % 100,
                                        (int)MyDefine.myXET.homdate % 100,
                                        (int)MyDefine.myXET.homtime / 10000,
                                        ((int)MyDefine.myXET.homtime / 100) % 100,
                                        (int)MyDefine.myXET.homtime % 100);

                                    //条数myHom.Count
                                    //时间homrun
                                    //AddMilliseconds(1000)
                                    //AddTicks(10000000)
                                    //1ms = 10000ticks
                                    long mGap = 10000L * MyDefine.myXET.homrun / MyDefine.myXET.myHom.Count;

                                    //计算时间
                                    for (int i = 0; i < MyDefine.myXET.myHom.Count; i++)
                                    {
                                        MyDefine.myXET.myHom[i].time = mDate;

                                        mDate = mDate.AddTicks(mGap);
                                    }

                                    //计算统计值
                                    MyDefine.myXET.myHomUpdate();

                                    //委托
                                    MyDefine.myXET.form_Update();
                                }
                                else
                                {
                                    MessageBox.Show("空数据");
                                }
                                break;
                            default: break;
                        }
                    }
                    else
                    {
                        String mStr = line;
                        erro = 0;

                        int mySZCHA;
                        if (MyDefine.myXET.meType == TYPE.TVS15)
                        {
                            mySZCHA = SZ.CHA;
                        }
                        else
                        {
                            mySZCHA = SZ.CHA - 7;
                        }

                        for (int k = 0; k < mySZCHA; k++)
                        {
                            MyDefine.myXET.mtp.OUT[k] = Convert.ToInt32(mStr.Substring(0, mStr.IndexOf(',')));

                            mStr = mStr.Substring(mStr.IndexOf(',') + 1);

                            if (new_index >= 1)
                            {
                                t1 = MyDefine.myXET.myHom[new_index].OUT[k] - MyDefine.myXET.myHom[old_index].OUT[k];
                                if (Math.Abs(t1) >= 500)
                                {
                                    erro++;
                                }
                            }
                        }

                        if (erro == mySZCHA)
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
                                        MyDefine.myXET.myHom[i] = MyDefine.myXET.myHom[old_index];
                                    }
                                }
                            }

                            count = 0;
                            old_index = new_index;
                        }

                        if (MyDefine.myXET.mtp.OUT.Average() < 12699)
                        {
                            MyDefine.myXET.myHom.Add(new TMP(MyDefine.myXET.mtp));
                        }
                    }
                }
            }
        }

        //数据列表
        private void TableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MyDefine.myXET.myHom.Count <= 0)
            {
                MessageBox.Show("请先导入数据");
                return;
            }

            Form form;
            string formName;
            if (MyDefine.myXET.meType == TYPE.TVS15)
            {
                formName = "MenuDataForm";
                form = new MenuDataForm();
            }
            else
            {
                formName = "MenuDataFormTVS8";
                form = new MenuDataFormTVS8();
            }

            foreach (Form fr in this.MdiChildren)
            {
                if (fr.GetType().Name == formName)
                {
                    fr.BringToFront();
                    return;
                }
            }
            form.MdiParent = this;
            form.Show();
            form.WindowState = FormWindowState.Maximized;
        }

        //检测程序
        private void StepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuStepForm myStepForm = new MenuStepForm();
            myStepForm.StartPosition = FormStartPosition.CenterParent;
            myStepForm.ShowDialog();
            this.BringToFront();

            if (myStepForm.DialogResult == DialogResult.Yes)
            {
                //修改完成规程后，数据曲线界面刷新规程选项卡
                foreach (var mdi in this.MdiChildren)
                {
                    if (mdi.Name == "MenuDataForm")
                    {
                        MenuDataForm menuDataForm = mdi as MenuDataForm;
                        menuDataForm.refreshCorrectionDataStatus();
                        break;
                    }
                }
            }
        }

        //生产报告
        private void ReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuReportForm myReportForm = new MenuReportForm();
            myReportForm.StartPosition = FormStartPosition.CenterParent;
            myReportForm.ShowDialog();
            this.BringToFront();
        }

        //操作手册
        private void ReferToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(MyDefine.myXET.userPIC + @"\Process.pdf"))
            {
                System.Diagnostics.Process.Start(MyDefine.myXET.userPIC + @"\Process.pdf");
                this.BringToFront();
            }
        }

        //软件版本
        private void AboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuAboutBox myAboutBox = new MenuAboutBox();
            myAboutBox.ShowDialog();
            this.BringToFront();
        }

        //厂家设置
        private void FactToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MenuFactForm myFactForm = new MenuFactForm();
            myFactForm.StartPosition = FormStartPosition.CenterParent;
            myFactForm.ShowDialog();
            this.BringToFront();
        }

        //设备校准
        private void CalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form;
            if (MyDefine.myXET.myType == TYPE.TVS15)
            {
                form = new MenuCalForm();
            }
            else
            {
                form = new MenuCalFormTVS8();
            }
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog();
            this.BringToFront();
        }

        //校准曲线
        private void CurveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form form;
            string formName;
            if (MyDefine.myXET.myType == TYPE.TVS15)
            {
                formName = "MenuCurveForm";
                form = new MenuCurveForm();
            }
            else
            {
                formName = "MenuCurveFormTVS8";
                form = new MenuCurveFormTVS8();
            }

            foreach (Form fr in this.MdiChildren)
            {
                if (fr.GetType().Name == formName)
                {
                    fr.BringToFront();
                    return;
                }
            }
            form.MdiParent = this;
            form.Show();
            form.WindowState = FormWindowState.Maximized;
        }

        //注册
        private void InitializationRegister()
        {
            //
            MyDefine.myXET.devStatus = TmpMode.NULL;

            //验证MAC地址
            Int64 net_Mac = 0;
            Int64 net_Var = 0;
            //验证regedit
            Int64 reg_Mac = 0;
            Int64 reg_Var = 0;
            //验证C盘文件
            Int64 sys_Mac = 0;
            Int64 sys_Var = 0;
            Int32 sys_num = 0;
            //验证本地文件
            Int64 use_Mac = 0;
            Int64 use_Var = 0;
            Int32 use_num = 0;

            //验证MAC地址
            string macAddress = "";
            Process myProcess = null;
            StreamReader reader = null;
            try
            {
                ProcessStartInfo start = new ProcessStartInfo("cmd.exe");

                start.FileName = "ipconfig";
                start.Arguments = "/all";
                start.CreateNoWindow = true;
                start.RedirectStandardOutput = true;
                start.RedirectStandardInput = true;
                start.UseShellExecute = false;
                myProcess = Process.Start(start);
                reader = myProcess.StandardOutput;
                string line = reader.ReadLine();
                while (!reader.EndOfStream)
                {
                    if (line.ToLower().IndexOf("physical address") > 0 || line.ToLower().IndexOf("物理地址") > 0)
                    {
                        int index = line.IndexOf(":");
                        index += 2;
                        macAddress = line.Substring(index);
                        macAddress = macAddress.Replace('-', ':');
                        break;
                    }
                    line = reader.ReadLine();
                }
            }
            catch
            {

            }
            finally
            {
                if (myProcess != null)
                {
                    reader.ReadToEnd();
                    myProcess.WaitForExit();
                    myProcess.Close();
                }
                if (reader != null)
                {
                    reader.Close();
                }
            }

            if (macAddress.Length == 17)
            {
                macAddress = macAddress.Replace(":", "");
                net_Mac = Convert.ToInt64(macAddress, 16);
                net_Var = net_Mac;
                while ((net_Var % 2) == 0)
                {
                    net_Var = net_Var / 2;
                }
                while ((net_Var % 3) == 0)
                {
                    net_Var = net_Var / 3;
                }
                while ((net_Var % 5) == 0)
                {
                    net_Var = net_Var / 5;
                }
                while ((net_Var % 7) == 0)
                {
                    net_Var = net_Var / 7;
                }
            }

            //验证regedit
            RegistryKey myKey = Registry.LocalMachine.OpenSubKey("software");
            string[] names = myKey.GetSubKeyNames();
            foreach (string keyName in names)
            {
                if (keyName == "WinES")
                {
                    myKey = Registry.LocalMachine.OpenSubKey("software\\WinES");
                    reg_Mac = Convert.ToInt64(myKey.GetValue("input").ToString());
                    reg_Var = Convert.ToInt64(myKey.GetValue("ouput").ToString());
                }
            }
            myKey.Close();

            //验证C盘文件
            if (!File.Exists("C:\\Windows\\user.cfg"))
            {
                if (File.Exists(MyDefine.myXET.userCFG + @"\user.num"))
                {
                    File.Copy((MyDefine.myXET.userCFG + @"\user.num"), ("C:\\Windows\\user.cfg"), true);
                }
            }
            if (File.Exists("C:\\Windows\\user.cfg"))
            {
                //读取用户信息
                FileStream meFS = new FileStream("C:\\Windows\\user.cfg", FileMode.Open, FileAccess.Read);
                BinaryReader meRead = new BinaryReader(meFS);
                if (meFS.Length > 0)
                {
                    //有内容文件
                    sys_Mac = meRead.ReadInt64();
                    sys_Var = meRead.ReadInt64();
                    sys_num = meRead.ReadInt32();
                }
                meRead.Close();
                meFS.Close();
            }

            //验证本地文件
            if (!File.Exists(MyDefine.myXET.userCFG + @"\user.num"))
            {
                if (File.Exists("C:\\Windows\\user.cfg"))
                {
                    File.Copy(("C:\\Windows\\user.cfg"), (MyDefine.myXET.userCFG + @"\user.num"), true);
                }
            }
            if (File.Exists(MyDefine.myXET.userCFG + @"\user.num"))
            {
                //读取用户信息
                FileStream meFS = new FileStream((MyDefine.myXET.userCFG + @"\user.num"), FileMode.Open, FileAccess.Read);
                BinaryReader meRead = new BinaryReader(meFS);
                if (meFS.Length > 0)
                {
                    //有内容文件
                    use_Mac = meRead.ReadInt64();
                    use_Var = meRead.ReadInt64();
                    use_num = meRead.ReadInt32();
                }
                meRead.Close();
                meFS.Close();
            }

            //注册分析
            if ((net_Mac == reg_Mac) && (net_Var == reg_Var) && (sys_Mac == use_Mac) && (sys_Var == use_Var) && (net_Mac == use_Mac) && (net_Var == use_Var))
            {
                MyDefine.myXET.myPC = 1;
                MyDefine.myXET.myMac = sys_Mac;
                MyDefine.myXET.myVar = sys_Var;
            }
            else
            {
                this.Text += " - no signed";
                MyDefine.myXET.myPC = 1;  //20200629加调试功能给杨磊
                MyDefine.myXET.myMac = sys_Mac;  //20200629加调试功能给杨磊
                MyDefine.myXET.myVar = sys_Var;  //20200629加调试功能给杨磊
            }
        }

        //登录处理
        private void Login_Account(string meTitle)
        {
            //
            MenuAccountForm myAccountForm = new MenuAccountForm();
            //
            myAccountForm.MyUser = MyDefine.myXET.userName;
            myAccountForm.MyPSW = MyDefine.myXET.userPassword;
            //
            myAccountForm.Text = meTitle;
            myAccountForm.StartPosition = FormStartPosition.CenterScreen;
            myAccountForm.ShowDialog();

            //加载
            if (myAccountForm.IsSave)
            {
                MyDefine.myXET.userName = myAccountForm.MyUser;
                MyDefine.myXET.userPassword = myAccountForm.MyPSW;
            }

            //创建
            if (myAccountForm.IsNew)
            {
                MyDefine.myXET.SaveToDat();
            }

            //超级账户
            if (((MyDefine.myXET.userName == "zhoup") && (MyDefine.myXET.userPassword == "TVS") && (MyDefine.myXET.myPC == 1))
                || ((MyDefine.myXET.userName == "JDeGree") && (MyDefine.myXET.userPassword == "leiG") && (MyDefine.myXET.myPC == 1)))
            {
                DebugToolStripMenuItem.Visible = true;
            }
            else
            {
                //zhoup
                DebugToolStripMenuItem.Visible = false;
            }

            //找到数据曲线界面并更新修正数据功能
            foreach (var mdi in this.MdiChildren)
            {
                if (MyDefine.myXET.myType == TYPE.TVS15)
                {
                    if (mdi.Name == "MenuDataForm")
                    {
                        MenuDataForm menuDataForm = mdi as MenuDataForm;
                        menuDataForm.refreshCorrectionDataStatus();
                        break;
                    }
                }
                else
                {
                    if (mdi.Name == "MenuDataFormTVS8")
                    {
                        MenuDataFormTVS8 menuDataForm = mdi as MenuDataFormTVS8;
                        menuDataForm.refreshCorrectionDataStatus();
                        break;
                    }
                }
            }
        }

        //启动
        private void Main_Load(object sender, EventArgs e)
        {
            this.Hide();
            InitializationRegister();
            Login_Account("欢迎使用！");
            this.Show();
            this.BringToFront();
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("是否退出当前系统？", "退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //关闭串口
                while (MyDefine.myXET.mePort.IsOpen)
                {
                    MyDefine.myXET.mePort.Close();
                }
                //退出所有窗口
                System.Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }
    }
}
