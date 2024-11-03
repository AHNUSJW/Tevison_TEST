using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace TVS
{
    public partial class MenuDataPdfForm : Form
    {
        //规程和数据
        public string nameProgram;
        public string nameFile;

        //基本信息
        public string machineType;
        public string machineSN;
        public string machineDescription;
        public string reportCompany;
        public string reportStaff;
        public string reportDepartment;
        public string reportNumber;
        public string reportDate;
        public string reportName;

        //
        public MenuDataPdfForm()
        {
            InitializeComponent();
        }

        //更新listbox
        private void GetListBox()
        {
            listBox1.Items.Add("程序 " + nameProgram);
            listBox1.Items.Add("数据 " + nameFile);
        }

        //保存记录
        private void SaveUserInfo()
        {
            String mePath = MyDefine.myXET.userCFG + @"\user." + MyDefine.myXET.userName + ".ifo";
            if (File.Exists(mePath))
            {
                System.IO.File.SetAttributes(mePath, FileAttributes.Normal);
            }
            FileStream meFS = new FileStream(mePath, FileMode.Create, FileAccess.Write);
            TextWriter meWrite = new StreamWriter(meFS);
            if (textBox1.TextLength > 0)
            {
                meWrite.WriteLine("machineType=" + textBox1.Text);
            }
            if (textBox2.TextLength > 0)
            {
                meWrite.WriteLine("machineSN=" + textBox2.Text);
            }
            if (textBox3.TextLength > 0)
            {
                meWrite.WriteLine("machineDescription=" + textBox3.Text);
            }
            if (textBox4.TextLength > 0)
            {
                meWrite.WriteLine("reportCompany=" + textBox4.Text);
            }
            if (textBox5.TextLength > 0)
            {
                meWrite.WriteLine("reportStaff=" + textBox5.Text);
            }
            if (textBox6.TextLength > 0)
            {
                meWrite.WriteLine("reportDepartment=" + textBox6.Text);
            }
            if (textBox7.TextLength > 0)
            {
                meWrite.WriteLine("reportNumber=" + textBox7.Text);
            }
            if (textBox8.TextLength > 0)
            {
                meWrite.WriteLine("reportDate=" + textBox8.Text);
            }
            if (textBox9.TextLength > 0)
            {
                meWrite.WriteLine("reportName=" + textBox9.Text);
            }
            meWrite.Close();
            meFS.Close();
            System.IO.File.SetAttributes(mePath, FileAttributes.ReadOnly);
        }

        //读取记录
        private void GetUserInfo()
        {
            String mePath = MyDefine.myXET.userCFG + @"\user." + MyDefine.myXET.userName + ".ifo";

            if (File.Exists(mePath))
            {
                String[] meLines = File.ReadAllLines(mePath);

                foreach (String line in meLines)
                {
                    switch (line.Substring(0, line.IndexOf('=')))
                    {
                        case "machineType": textBox1.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "machineSN": textBox2.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "machineDescription": textBox3.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportCompany": textBox4.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportStaff": textBox5.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportDepartment": textBox6.Text = line.Substring(line.IndexOf('=') + 1); break;
                        case "reportNumber": /*textBox7.Text = line.Substring(line.IndexOf('=') + 1)*/; break;
                        case "reportDate": /*textBox8.Text = line.Substring(line.IndexOf('=') + 1)*/; break;
                        case "reportName": textBox9.Text = line.Substring(line.IndexOf('=') + 1); break;
                        default: break;
                    }
                }
            }

            textBox7.Text = "JD" + ((System.DateTime.Now.Ticks - 621355968000000000) / 10000000L).ToString();
            textBox8.Text = System.DateTime.Now.ToString("yyyyMMdd HH:mm:ss");
        }

        //完成信息修改
        private void button1_Click(object sender, EventArgs e)
        {
            machineType = textBox1.Text;
            machineSN = textBox2.Text;
            machineDescription = textBox3.Text;
            reportCompany = textBox4.Text;
            reportStaff = textBox5.Text;
            reportDepartment = textBox6.Text;
            reportNumber = textBox7.Text;
            reportDate = textBox8.Text;
            reportName = textBox9.Text;

            //保存生成pdf文件配置
            SaveUserInfo();

            this.DialogResult = DialogResult.OK;//不隐藏窗口。关闭此窗口，减少资源占用， Pengjun 20221215
            //this.Hide();
        }

        /// <summary>
        /// 生成pdf窗体加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuDataPdfForm_Load(object sender, EventArgs e)
        {
            //获取程序 数据显示到数据规程栏
            GetListBox();

            //获取生成pdf配置信息
            GetUserInfo();
        }
    }
}
