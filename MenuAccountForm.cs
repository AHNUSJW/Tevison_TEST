using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace TVS
{
    public partial class MenuAccountForm : Form
    {
        //
        private Boolean isSave = false;//用于退出后主界面使用
        private Boolean isNew = false;//用于退出后主界面使用
        private String myUser = "admin";
        private String myPSW = "";
        private String myDatPath = MyDefine.myXET.userCFG;

        //
        #region set and get
        //

        public Boolean IsSave
        {
            get
            {
                return isSave;
            }
        }
        public Boolean IsNew
        {
            get
            {
                return isNew;
            }
        }
        public String MyUser
        {
            set
            {
                myUser = value;
            }
            get
            {
                return myUser;
            }
        }
        public String MyPSW
        {
            set
            {
                myPSW = value;
            }
            get
            {
                return myPSW;
            }
        }
        public String MyDatPath
        {
            set
            {
                myDatPath = value;
            }
            get
            {
                return myDatPath;
            }
        }

        //
        #endregion
        //

        //
        public MenuAccountForm()
        {
            InitializeComponent();
        }

        //报警提示
        private void warning_NI(string meErr)
        {
            timer1.Enabled = true;
            timer1.Interval = 3000;
            notifyIcon1.Visible = true;
            notifyIcon1.ShowBalloonTip(3000, notifyIcon1.Text, meErr, ToolTipIcon.Info);
            label4.Location = new Point(28, 180);
            label4.Text = meErr;
            label4.Visible = true;
        }

        //账号密码规则
        private void psw_KeyPress(object sender, KeyPressEventArgs e)
        {
            //不可以有以下特殊字符
            // \/:*?"<>|
            // \\
            // \|
            // ""
            Regex meRgx = new Regex(@"[\\/:*?""<>\|]");
            if (meRgx.IsMatch(e.KeyChar.ToString()))
            {
                warning_NI("不能使用\\/:*?\"<>|");
                e.Handled = true;
            }
        }

        //登录创建保存按钮- 登录
        private void login_button1_Click()
        {
            //工厂使用超级账号密码
            if (((comboBox1.Text == "zhoup") && (textBox1.Text == "TVS")) || ((comboBox1.Text == "JDeGree") && (textBox1.Text == "leiG")))
            {
                myUser = comboBox1.Text;
                myPSW = textBox1.Text;
                myDatPath = MyDefine.myXET.userCFG;
                isSave = true;
                isNew = false;
                this.Hide();
            }
            //客户使用dat账号密码
            else
            {
                //用户文件
                String meString = myDatPath + @"\user." + comboBox1.Text + ".cfg";

                //验证用户
                if (File.Exists(meString))
                {
                    //读取用户信息
                    FileStream meFS = new FileStream(meString, FileMode.Open, FileAccess.Read);
                    BinaryReader meRead = new BinaryReader(meFS);
                    if (meFS.Length > 0)
                    {
                        //有内容文件
                        myUser = meRead.ReadString();
                        myPSW = meRead.ReadString();
                        myDatPath = meRead.ReadString();
                        myDatPath = MyDefine.myXET.userCFG;
                        isNew = false;
                    }
                    else
                    {
                        //空文件
                        myUser = comboBox1.Text;
                        myPSW = "";
                        myDatPath = MyDefine.myXET.userCFG;
                        isNew = true;
                    }
                    meRead.Close();
                    meFS.Close();

                    //验证密码
                    if (myPSW == textBox1.Text)
                    {
                        isSave = true;
                        this.Hide();
                    }
                    else
                    {
                        warning_NI("密码错误！");
                    }
                }
                else
                {
                    //不存在admin用户
                    if ((comboBox1.Text == "admin") && (textBox1.Text == ""))
                    {
                        myUser = "admin";
                        myPSW = "";
                        myDatPath = MyDefine.myXET.userCFG;
                        isSave = true;
                        isNew = true;
                        this.Hide();
                    }
                    else if ((comboBox1.Text == "bohui") && (textBox1.Text == "bohui"))
                    {
                        myUser = "bohui";
                        myPSW = "bohui";
                        myDatPath = MyDefine.myXET.userCFG;
                        isSave = true;
                        isNew = true;
                        this.Hide();
                    }
                    //不存在用户提示
                    else
                    {
                        warning_NI("不存在用户！");
                    }
                }
            }
        }

        //登录创建保存按钮- 创建
        private void create_button1_Click()
        {
            if (comboBox1.SelectedIndex < 0)//帐号验证
            {
                if (textBox1.Text == textBox2.Text)//密码验证
                {
                    myUser = comboBox1.Text;
                    myPSW = textBox1.Text;
                    myDatPath = MyDefine.myXET.userCFG;
                    isSave = true;
                    isNew = true;
                    this.Hide();
                }
                else
                {
                    warning_NI("密码错误！");
                }
            }
            else
            {
                warning_NI("已存在账号！");
            }
        }

        //登录创建保存按钮- 保存
        private void save_button1_Click()
        {
            String meString = myDatPath + @"\user." + comboBox1.Text + ".cfg";

            //验证用户
            if (File.Exists(meString))
            {
                //读取用户信息
                FileStream meFS = new FileStream(meString, FileMode.Open, FileAccess.Read);
                BinaryReader meRead = new BinaryReader(meFS);
                if (meFS.Length > 0)
                {
                    //有内容文件
                    myUser = meRead.ReadString();
                    myPSW = meRead.ReadString();
                    myDatPath = meRead.ReadString();
                    myDatPath = MyDefine.myXET.userCFG;
                    isNew = false;
                }
                else
                {
                    //空文件
                    myUser = comboBox1.Text;
                    myPSW = "";
                    myDatPath = MyDefine.myXET.userCFG;
                    isNew = true;
                }
                meRead.Close();
                meFS.Close();

                //验证密码
                if (myPSW == textBox1.Text)
                {
                    if (myPSW == textBox2.Text)
                    {
                        warning_NI("新密码不能与旧密码相同！");
                        return;
                    }

                    myUser = comboBox1.Text;
                    myPSW = textBox2.Text;
                    myDatPath = MyDefine.myXET.userCFG;
                    isSave = true;
                    isNew = true;
                    this.Hide();

                    MessageBox.Show("密码修改成功", "系统提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    warning_NI("旧密码错误！");
                }
            }
            else
            {
                warning_NI("不存在账户" + textBox1.Text + "!");
            }
        }

        //登录创建保存按钮
        private void button1_Click(object sender, EventArgs e)
        {
            if (button1.Text == "登 录")
            {
                login_button1_Click();
            }
            else if (button1.Text == "创 建")
            {
                create_button1_Click();
            }
        }

        //新建
        private void button2_Click(object sender, EventArgs e)
        {
            button2.Visible = false;
            button3.Visible = false;
            label3.Visible = true;
            textBox2.Visible = true;
            button1.Text = "创 建";
            this.Text = "新建账号";
        }

        //改密码
        private void button3_Click(object sender, EventArgs e)
        {
            //工厂使用超级账号密码
            if ((comboBox1.Text == "zhoup") || (comboBox1.Text == "JDeGree"))
            {
                warning_NI("账号" + comboBox1.Text + "不支持修改密码");
                return;
            }

            button1.Visible = false;
            button2.Visible = false;
            button3.Visible = false;
            button5.Visible = true;
            label3.Visible = true;
            textBox2.Visible = true;
            label2.Text = "旧密码：";
            label3.Text = "新密码：";
            this.Text = "修改密码";
        }

        //取消按钮
        private void button4_Click(object sender, EventArgs e)
        {
            if (this.Text == "欢迎使用！")
            {
                System.Environment.Exit(0);
            }
            else if (this.Text == "修改密码")
            {
                button1.Visible = true;
                button2.Visible = true;
                button3.Visible = true;
                button5.Visible = false;
                label3.Visible = false;
                textBox2.Visible = false;
                label2.Text = "密  码：";
                label3.Text = "确认密码：";
                this.Text = "欢迎使用！";

                notifyIcon1.Visible = false;
            }
            else if (this.Text == "新建账号")
            {
                button2.Visible = true;
                button3.Visible = true;
                label3.Visible = false;
                textBox2.Visible = false;
                button1.Text = "登 录";
                this.Text = "欢迎使用！";

                notifyIcon1.Visible = false;
            }
            else
            {
                isSave = false;
                isNew = false;
                this.Hide();
            }
        }

        //确认修改密码键
        private void button5_Click(object sender, EventArgs e)
        {
            save_button1_Click();
        }

        //时间控制
        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            notifyIcon1.Visible = false;
        }

        //帐号登录加载
        private void MenuAccountForm_Load(object sender, EventArgs e)
        {
            //窗口元素调整
            if (this.Text == "欢迎使用！")
            {
                button2.Visible = true;
                button3.Visible = true;
                button5.Visible = false;
                label3.Visible = false;
                textBox2.Visible = false;
            }
            else
            {
                button2.Visible = false;
                button3.Visible = true;
                button5.Visible = false;
                label3.Visible = false;
                textBox2.Visible = false;
            }

            //所有用户加载
            if (Directory.Exists(myDatPath))
            {
                //存在
                DirectoryInfo meDirectory = new DirectoryInfo(myDatPath);
                String meString = null;
                foreach (FileInfo meFiles in meDirectory.GetFiles("user.*.cfg"))
                {
                    meString = meFiles.Name;
                    meString = meString.Replace("user.", "");
                    meString = meString.Replace(".cfg", "");
                    comboBox1.Items.Add(meString);
                }
            }
            else
            {
                //不存在则创建文件夹
                Directory.CreateDirectory(myDatPath);
                //不存在则创建文件
                myUser = "admin";
                myPSW = "";
                myDatPath = MyDefine.myXET.userCFG;
                isSave = true;
                isNew = true;
                //增加初始用户
                comboBox1.Items.Add("admin");
            }

            //用户名加载
            comboBox1.Text = myUser;
            textBox1.Text = "";
        }

        //退出帐号登录
        private void MenuAccountForm_Closed(object sender, FormClosedEventArgs e)
        {
            timer1.Enabled = false;

            //关闭退出
            if (this.Text == "欢迎使用！")
            {
                System.Environment.Exit(0);
            }
            else
            {
                isSave = false;
                isNew = false;
            }
        }

        //按enter键登录
        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                this.button1.Focus();
                button1_Click(sender, e);   //调用登录按钮的事件处理代码
            }
        }

        //按enter键登录
        private void comboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13)
            {
                this.button1.Focus();
                button1_Click(sender, e);   //调用登录按钮的事件处理代码
            }
        }
    }
}

//end