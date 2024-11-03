using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace TVS
{

    /// <summary>
    /// 修正数据刷新界面
    /// </summary>
    public delegate void refreshPicDelegateTVS8(bool isChange);

    public partial class MenuDataFormTVS8 : Form
    {
        private Tmpicture myPicture = new Tmpicture();

        private Int32 index;     //表格取温度数据指针
        private Int32 tmpMax;    //最高温度
        private Int32 tmpMin;    //最低温度

        private Boolean autoSelect = false; //自动选点

        private string machineType;         //
        private string machineSN;           //
        private string machineDescription;  //
        private string reportCompany;       //
        private string reportStaff;         //
        private string reportDepartment;    //
        private string reportNumber;        //
        private string reportDate;          //
        private string reportName;          //

        List<string> tmpPickData = new List<string>();

        public MenuDataFormTVS8()
        {
            //设置窗体的双缓冲
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();

            //
            InitializeComponent();

            //利用反射设置DataGridView的双缓冲
            Type myType = this.dataGridView1.GetType();
            PropertyInfo pi = myType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(this.dataGridView1, true, null);
        }

        //选折规程后, 更新温度点ComboBox
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((myPicture.isSet) && (comboBox1.SelectedIndex != myPicture.tpIDX))
            {
                if (MessageBox.Show("选择规程会清除手动选点, [确定]清除后切换规程", "是否清除手动选点?", MessageBoxButtons.OKCancel) == DialogResult.OK)
                {
                    myPicture.isSet = false;
                }
                else
                {
                    comboBox1.SelectedIndex = myPicture.tpIDX;
                    return;
                }
            }

            int idx = comboBox1.SelectedIndex;
            int num = MyDefine.myXET.myPRM[idx].myTP.Count * 2 + 2;

            ToolStripMenuItem item;

            //更新温度点ComboBox
            comboBox2.Items.Clear();

            //表格右键菜单
            contextMenuStrip1.Items.Clear();
            //
            item = new ToolStripMenuItem();
            item.Name = "setStartTSMI";
            item.Text = "设为数据起始";
            item.Click += new System.EventHandler(this.setStartPictureTSMenuItem_Click);
            contextMenuStrip1.Items.Add(item);
            //
            item = new ToolStripMenuItem();
            item.Name = "setStopTSMI";
            item.Text = "设为数据结束";
            item.Click += new System.EventHandler(this.setStopPictureTSMenuItem_Click);
            contextMenuStrip1.Items.Add(item);

            //曲线右键菜单
            contextMenuStrip2.Items.Clear();
            //
            item = new ToolStripMenuItem();
            item.Name = "clearStartStopPictureTSMI";
            item.Text = "清除起始结束点";
            item.Click += new System.EventHandler(this.clearStartStopPictureTSMenuItem_Click);
            contextMenuStrip2.Items.Add(item);
            //
            item = new ToolStripMenuItem();
            item.Name = "setStartPictureTSMI";
            item.Text = "设为数据起始";
            item.Click += new System.EventHandler(this.setStartPictureTSMenuItem_Click);
            contextMenuStrip2.Items.Add(item);
            //
            item = new ToolStripMenuItem();
            item.Name = "setStopPictureTSMI";
            item.Text = "设为数据结束";
            item.Click += new System.EventHandler(this.setStopPictureTSMenuItem_Click);
            contextMenuStrip2.Items.Add(item);

            //重置选择的温度点
            myPicture.myTP.Clear();
            myPicture.tpIDX = idx;
            myPicture.name = MyDefine.myXET.myPRM[idx].name;

            for (int i = 0; i < MyDefine.myXET.myPRM[idx].myTP.Count; i++)
            {
                //更新温度点ComboBox
                comboBox2.Items.Add("P" + (i + 1).ToString() + " " + (MyDefine.myXET.myPRM[idx].myTP[i].temperature / 100f).ToString() + "℃ " + MyDefine.myXET.myPRM[idx].myTP[i].time.ToString() + "sec");

                //表格右键菜单
                item = new ToolStripMenuItem();
                item.Name = "setP" + i.ToString("d2") + "StartTSMI";
                item.Text = "设为 P" + (i + 1).ToString() + " " + (MyDefine.myXET.myPRM[idx].myTP[i].temperature / 100f).ToString() + "℃ 起始";
                item.Click += new System.EventHandler(this.setPxStartTSMenuItem_Click);
                contextMenuStrip1.Items.Add(item);

                //表格右键菜单
                item = new ToolStripMenuItem();
                item.Name = "setP" + i.ToString("d2") + "StopTSMI";
                item.Text = "设为 P" + (i + 1).ToString() + " " + (MyDefine.myXET.myPRM[idx].myTP[i].temperature / 100f).ToString() + "℃ 结束";
                item.Click += new System.EventHandler(this.setPxStopTSMenuItem_Click);
                contextMenuStrip1.Items.Add(item);

                //曲线右键菜单
                item = new ToolStripMenuItem();
                item.Name = "setP" + i.ToString("d2") + "StartPictureTSMI";
                item.Text = "设为 P" + (i + 1).ToString() + " " + (MyDefine.myXET.myPRM[idx].myTP[i].temperature / 100f).ToString() + "℃ 起始";
                item.Click += new System.EventHandler(this.setPxStartTSMenuItem_Click);
                contextMenuStrip2.Items.Add(item);

                //曲线右键菜单
                item = new ToolStripMenuItem();
                item.Name = "setP" + i.ToString("d2") + "StopPictureTSMI";
                item.Text = "设为 P" + (i + 1).ToString() + " " + (MyDefine.myXET.myPRM[idx].myTP[i].temperature / 100f).ToString() + "℃ 结束";
                item.Click += new System.EventHandler(this.setPxStopTSMenuItem_Click);
                contextMenuStrip2.Items.Add(item);

                //拷贝规程
                myPicture.myTP.Add(new Tmpick());
                myPicture.myTP[i].temperature = MyDefine.myXET.myPRM[idx].myTP[i].temperature;
                myPicture.myTP[i].time = MyDefine.myXET.myPRM[idx].myTP[i].time;
            }

            //表格右键
            item = new ToolStripMenuItem();
            item.Name = "setHeatTaTSMI";
            item.Text = "设为升温起始点Ta";
            item.Click += new System.EventHandler(this.setTaTbTSMenuItem_Click);
            contextMenuStrip1.Items.Add(item);

            //表格右键
            item = new ToolStripMenuItem();
            item.Name = "setHeatTbTSMI";
            item.Text = "设为升温结束点Tb";
            item.Click += new System.EventHandler(this.setTaTbTSMenuItem_Click);
            contextMenuStrip1.Items.Add(item);

            //表格右键
            item = new ToolStripMenuItem();
            item.Name = "setCoolTaTSMI";
            item.Text = "设为降温起始点Ta";
            item.Click += new System.EventHandler(this.setTaTbTSMenuItem_Click);
            contextMenuStrip1.Items.Add(item);

            //表格右键
            item = new ToolStripMenuItem();
            item.Name = "setCoolTbTSMI";
            item.Text = "设为降温结束点Tb";
            item.Click += new System.EventHandler(this.setTaTbTSMenuItem_Click);
            contextMenuStrip1.Items.Add(item);

            //Picture右键
            item = new ToolStripMenuItem();
            item.Name = "setHeatTaPictureTSMI";
            item.Text = "设为升温起始点Ta";
            item.Click += new System.EventHandler(this.setTaTbTSMenuItem_Click);
            contextMenuStrip2.Items.Add(item);

            //Picture右键
            item = new ToolStripMenuItem();
            item.Name = "setHeatTbPictureTSMI";
            item.Text = "设为升温结束点Tb";
            item.Click += new System.EventHandler(this.setTaTbTSMenuItem_Click);
            contextMenuStrip2.Items.Add(item);

            //Picture右键
            item = new ToolStripMenuItem();
            item.Name = "setCoolTaPictureTSMI";
            item.Text = "设为降温起始点Ta";
            item.Click += new System.EventHandler(this.setTaTbTSMenuItem_Click);
            contextMenuStrip2.Items.Add(item);

            //Picture右键
            item = new ToolStripMenuItem();
            item.Name = "setCoolTbPictureTSMI";
            item.Text = "设为降温结束点Tb";
            item.Click += new System.EventHandler(this.setTaTbTSMenuItem_Click);
            contextMenuStrip2.Items.Add(item);

            //更新温度点ComboBox
            comboBox2.SelectedIndex = 0;
        }

        //选折温度点后, 更新按钮文字
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = comboBox1.SelectedIndex;
            int idy = comboBox2.SelectedIndex;

            //按钮信息
            button5.Text = "设置" + (MyDefine.myXET.myPRM[idx].myTP[idy].temperature / 100f).ToString() + "℃起始";
            button6.Text = "设置" + (MyDefine.myXET.myPRM[idx].myTP[idy].temperature / 100f).ToString() + "℃结束";

            //画顶层
            pictureBoxScope_draw();
        }

        //有效曲线 or 完整曲线
        private void button1_Click(object sender, EventArgs e)
        {
            if (myPicture.isLoad)
            {
                //切换状态
                if (myPicture.isFull)
                {
                    myPicture.isFull = false;
                    button1.Text = "显示完整";
                    button1.BackColor = myPicture.color_axis;
                }
                else
                {
                    myPicture.isFull = true;
                    button1.Text = "显示有效";
                    button1.BackColor = button2.BackColor;
                }

                //计算
                myPicture.getAxis_pictureBox(pictureBox1.Width, pictureBox1.Height, this.tmpMax, this.tmpMin);
                myPicture.getPoint_pictureBox();

                //画底层
                pictureBoxScope_axis();

                //画顶层
                pictureBoxScope_draw();
            }
        }

        //设置数据开始
        private void button2_Click(object sender, EventArgs e)
        {
            if (myPicture.isLoad)
            {
                if ((myPicture.xline_pick > 0) && (myPicture.tmpPick < myPicture.tmpStop))
                {
                    //初始化位置
                    myPicture.tmpStart = myPicture.tmpPick;
                    myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(-1, -1);

                    //计算
                    myPicture.getAxis_pictureBox(pictureBox1.Width, pictureBox1.Height, this.tmpMax, this.tmpMin);
                    myPicture.getPoint_pictureBox();

                    //画底层
                    pictureBoxScope_axis();

                    //画顶层
                    pictureBoxScope_draw();
                }
            }
        }

        //设置数据结束
        private void button3_Click(object sender, EventArgs e)
        {
            if (myPicture.isLoad)
            {
                if ((myPicture.xline_pick > 0) && (myPicture.tmpPick > myPicture.tmpStart))
                {
                    //初始化位置
                    myPicture.tmpStop = myPicture.tmpPick;
                    myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(-1, -1);

                    //计算顶层
                    myPicture.getAxis_pictureBox(pictureBox1.Width, pictureBox1.Height, this.tmpMax, this.tmpMin);
                    myPicture.getPoint_pictureBox();

                    //画底层
                    pictureBoxScope_axis();

                    //画顶层
                    pictureBoxScope_draw();
                }
            }
        }

        //自动选点
        private void button4_Click(object sender, EventArgs e)
        {
            if (myPicture.isLoad)
            {
                if (myPicture.isSet)
                {
                    if (MessageBox.Show("自动选点会清除手动选点, [确定]清除后自动选点", "是否清除手动选点?", MessageBoxButtons.OKCancel) == DialogResult.OK)
                    {
                        myPicture.isSet = false;
                    }
                    else
                    {
                        return;
                    }
                }
                //切换状态
                if (!autoSelect)
                {
                    //消除鼠标选点
                    myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(-1, -1);

                    //选点计算
                    if (!myPicture.autoGetTP_pictureBox())
                    {
                        MessageBox.Show("自动选点失败，请检查规程或切换为手动选点模式", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        autoSelect = true;
                        button4.Text = "取消选点";
                        button4.BackColor = myPicture.color_axis;
                    }

                    myPicture.getPoint_pictureBox();
                }
                else
                {
                    autoSelect = false;
                    button4.Text = "自动选点";
                    button4.BackColor = button2.BackColor;
                    comboBox1_SelectedIndexChanged(sender, e);
                }

                //画底层
                pictureBoxScope_axis();

                //画顶层
                pictureBoxScope_draw();
            }
        }

        //设置起始温度点
        private void button5_Click(object sender, EventArgs e)
        {
            if (myPicture.isLoad)
            {
                //获取指针
                int idx = comboBox2.SelectedIndex;

                //控制和稳定阶段
                if ((myPicture.tmpPick > 0) && (myPicture.tmpPick < MyDefine.myXET.myHom.Count))
                {
                    //
                    if (myPicture.myTP[idx].begin >= myPicture.tmpPick)
                    {
                        myPicture.myTP[idx].begin = myPicture.tmpPick - 1;
                    }

                    //温度稳定开始索引
                    myPicture.myTP[idx].const_begin = myPicture.tmpPick;

                    //温度稳定结束索引
                    if (myPicture.myTP[idx].const_end <= myPicture.tmpPick)
                    {
                        myPicture.myTP[idx].const_end = myPicture.tmpPick + 1;
                    }

                    //
                    if (myPicture.myTP[idx].end <= myPicture.tmpPick)
                    {
                        myPicture.myTP[idx].end = myPicture.tmpPick + 1;
                    }

                    //
                    myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(-1, -1);
                }

                //修改选择框
                myPicture.getPoint_pictureBox();

                //画底层
                pictureBoxScope_axis();

                //画顶层
                pictureBoxScope_draw();

                //
                myPicture.isSet = true;
            }
        }

        //设置结束温度点
        private void button6_Click(object sender, EventArgs e)
        {
            if (myPicture.isLoad)
            {
                //获取指针
                int idx = comboBox2.SelectedIndex;

                //控制和稳定阶段
                if ((myPicture.tmpPick > 0) && (myPicture.tmpPick < MyDefine.myXET.myHom.Count))
                {
                    //
                    if (myPicture.myTP[idx].begin >= myPicture.tmpPick)
                    {
                        myPicture.myTP[idx].begin = myPicture.tmpPick - 1;
                    }

                    //
                    if (myPicture.myTP[idx].const_begin >= myPicture.tmpPick)
                    {
                        myPicture.myTP[idx].const_begin = myPicture.tmpPick - 1;
                    }

                    //
                    myPicture.myTP[idx].const_end = myPicture.tmpPick;

                    //
                    if (myPicture.myTP[idx].end <= myPicture.tmpPick)
                    {
                        myPicture.myTP[idx].end = myPicture.tmpPick + 1;
                    }

                    //
                    myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(-1, -1);
                }

                //修改选择框
                myPicture.getPoint_pictureBox();

                //画底层
                pictureBoxScope_axis();

                //画顶层
                pictureBoxScope_draw();

                //
                myPicture.isSet = true;
            }
        }

        //生成报告
        private void button7_Click(object sender, EventArgs e)
        {
            //2022.10.12 zhoup
            //MenuReportForm中CreateReport里的分析数据,会计算无时间的斜率
            //如果不更新统计值,则每次MenuReportForm生成报告后,这边输出报告都会出错
            //所以更新一下防止交叉操作出错
            MyDefine.myXET.myHomUpdate();

            //构造窗口
            MenuDataPdfForm myDataPdfForm = new MenuDataPdfForm();

            //初始化数据
            myDataPdfForm.Text = "操作员信息";
            myDataPdfForm.nameProgram = myPicture.name;
            myDataPdfForm.nameFile = Path.GetFileNameWithoutExtension(MyDefine.myXET.homFileName);

            //打开窗口用户输入
            myDataPdfForm.StartPosition = FormStartPosition.CenterParent;
            if (myDataPdfForm.ShowDialog() == DialogResult.Cancel) //当关闭生成pdf窗口后 不创建pdf文档 Pengjun 20221215
            {
                return;
            }
            this.BringToFront();

            //没有选点
            for (int i = 1; i < myPicture.myTP.Count; i++)
            {
                if (myPicture.myTP[i].const_begin == 0)
                {
                    MessageBox.Show("请自动选点和手动选点, 设置报告数据的温度点");
                    return;
                }
            }

            //获取数据创建pdf
            if ((MyDefine.myXET.homFileName != null) && (MyDefine.myXET.myHom.Count > 0))
            {
                if (myPicture.isSet)
                {
                    myPicture.getTmpIdx();
                }
                this.machineType = myDataPdfForm.machineType;
                this.machineSN = myDataPdfForm.machineSN;
                this.machineDescription = myDataPdfForm.machineDescription;
                this.reportCompany = myDataPdfForm.reportCompany;
                this.reportStaff = myDataPdfForm.reportStaff;
                this.reportDepartment = myDataPdfForm.reportDepartment;
                this.reportNumber = myDataPdfForm.reportNumber;
                this.reportDate = myDataPdfForm.reportDate;
                this.reportName = myDataPdfForm.reportName;
                this.CreatDataPdf();
            }
        }

        //生成EXCEL
        private void button8_Click(object sender, EventArgs e)
        {
            string myExcel = MyDefine.myXET.userDAT + "\\" + MyDefine.myXET.homdate.ToString() + MyDefine.myXET.homtime.ToString() + "_" + MyDefine.myXET.homrun.ToString() + ".csv";
            if (MyDefine.myXET.hom_SaveToExcel(myExcel))
            {
                MessageBox.Show("导出EXCEL成功！");
            }
        }

        //清除起始结束点
        private void clearStartStopPictureTSMenuItem_Click(object sender, EventArgs e)
        {
            //
            myPicture.isFull = true;
            button1.Text = "显示有效";
            button1.BackColor = button2.BackColor;

            //初始化位置
            myPicture.tmpStart = 0;
            myPicture.tmpStop = MyDefine.myXET.myHom.Count - 1;
            myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(-1, -1);

            //计算顶层
            myPicture.getAxis_pictureBox(pictureBox1.Width, pictureBox1.Height, this.tmpMax, this.tmpMin);
            myPicture.getPoint_pictureBox();

            //画底层
            pictureBoxScope_axis();

            //画顶层
            pictureBoxScope_draw();
        }

        //设为数据起始
        private void setStartPictureTSMenuItem_Click(object sender, EventArgs e)
        {
            this.button2_Click(null, null);
        }

        //设为数据结束
        private void setStopPictureTSMenuItem_Click(object sender, EventArgs e)
        {
            this.button3_Click(null, null);
        }

        //设为温度点Px起始
        private void setPxStartTSMenuItem_Click(object sender, EventArgs e)
        {
            if (myPicture.isLoad)
            {
                //获取对象
                ToolStripMenuItem mts = (ToolStripMenuItem)sender;

                //根据对象名称获取指针
                int idx = Convert.ToInt32(mts.Name.Substring(4, 2));

                //控制和稳定阶段
                if ((myPicture.tmpPick > 0) && (myPicture.tmpPick < MyDefine.myXET.myHom.Count))
                {
                    //
                    if (myPicture.myTP[idx].begin >= myPicture.tmpPick)
                    {
                        myPicture.myTP[idx].begin = myPicture.tmpPick - 1;
                    }

                    //温度稳定开始索引
                    myPicture.myTP[idx].const_begin = myPicture.tmpPick;

                    //温度稳定结束索引
                    if (myPicture.myTP[idx].const_end <= myPicture.tmpPick)
                    {
                        myPicture.myTP[idx].const_end = myPicture.tmpPick + 1;
                    }

                    //
                    if (myPicture.myTP[idx].end <= myPicture.tmpPick)
                    {
                        myPicture.myTP[idx].end = myPicture.tmpPick + 1;
                    }

                    //
                    myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(-1, -1);
                }

                //修改选择框
                myPicture.getPoint_pictureBox();

                //画底层
                pictureBoxScope_axis();

                //画顶层
                pictureBoxScope_draw();

                //
                myPicture.isSet = true;
            }
        }

        //设为温度点Px结束
        private void setPxStopTSMenuItem_Click(object sender, EventArgs e)
        {
            if (myPicture.isLoad)
            {
                //获取对象
                ToolStripMenuItem mts = (ToolStripMenuItem)sender;

                //根据对象名称获取指针
                int idx = Convert.ToInt32(mts.Name.Substring(4, 2));

                //控制和稳定阶段
                if ((myPicture.tmpPick > 0) && (myPicture.tmpPick < MyDefine.myXET.myHom.Count))
                {
                    //
                    if (myPicture.myTP[idx].begin >= myPicture.tmpPick)
                    {
                        myPicture.myTP[idx].begin = myPicture.tmpPick - 1;
                    }

                    //
                    if (myPicture.myTP[idx].const_begin >= myPicture.tmpPick)
                    {
                        myPicture.myTP[idx].const_begin = myPicture.tmpPick - 1;
                    }

                    //
                    myPicture.myTP[idx].const_end = myPicture.tmpPick;

                    //
                    if (myPicture.myTP[idx].end <= myPicture.tmpPick)
                    {
                        myPicture.myTP[idx].end = myPicture.tmpPick + 1;
                    }

                    //
                    myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(-1, -1);
                }

                //修改选择框
                myPicture.getPoint_pictureBox();

                //画底层
                pictureBoxScope_axis();

                //画顶层
                pictureBoxScope_draw();

                //
                myPicture.isSet = true;
            }
        }

        //设置升降温起始结束点
        private void setTaTbTSMenuItem_Click(object sender, EventArgs e)
        {
            if (myPicture.isLoad)
            {
                //获取对象
                ToolStripMenuItem mts = (ToolStripMenuItem)sender;

                //先所有规程点位选完成后再可以选升温降温点位
                if (myPicture.myTP.Where(o => o.end == 0).ToArray().Length > 0)
                {
                    return;
                }

                //控制和稳定阶段
                if ((myPicture.tmpPick > 0) && (myPicture.tmpPick < MyDefine.myXET.myHom.Count))
                {
                    //升温起始点
                    if (mts.Name.Contains("HeatTa"))
                    {
                        for (int i = 0; i < myPicture.myTP.Count; i++)
                        {
                            myPicture.myTP[i].idx_50begin = myPicture.tmpPick;

                            if (i == 1)
                            {
                                myPicture.myTP[i].vutup = true;

                                myPicture.myTP[i].idx_vutbegin = myPicture.myTP[i - 1].const_end;
                            }

                            if (myPicture.myTP[i].idx_90end < myPicture.myTP[i].idx_50begin)
                            {
                                myPicture.myTP[i].idx_90end = myPicture.myTP[i].idx_50begin;
                            }
                        }

                    }//升温结束点
                    else if (mts.Name.Contains("HeatTb"))
                    {
                        for (int i = 0; i < myPicture.myTP.Count; i++)
                        {
                            myPicture.myTP[i].idx_90end = myPicture.tmpPick;

                            if (i == 1)
                            {
                                myPicture.myTP[i].vutup = true;
                            }

                            if (myPicture.myTP[i].idx_50begin > myPicture.myTP[i].idx_90end)
                            {
                                myPicture.myTP[i].idx_50begin = myPicture.myTP[i].idx_90end;
                            }
                        }

                    }//降温起始点
                    else if (mts.Name.Contains("CoolTa"))
                    {
                        for (int i = 0; i < myPicture.myTP.Count; i++)
                        {
                            myPicture.myTP[i].idx_90begin = myPicture.tmpPick;

                            if (i == 1)
                            {
                                myPicture.myTP[i].vutdown = true;
                            }

                            if (myPicture.myTP[i].idx_50end < myPicture.myTP[i].idx_90begin)
                            {
                                myPicture.myTP[i].idx_50end = myPicture.myTP[i].idx_90begin;
                            }
                        }

                    }//降温结束点
                    else if (mts.Name.Contains("CoolTb"))
                    {
                        for (int i = 0; i < myPicture.myTP.Count; i++)
                        {
                            myPicture.myTP[i].idx_50end = myPicture.tmpPick;

                            if (i == 1)
                            {
                                myPicture.myTP[i].vutdown = true;
                                myPicture.myTP[i].idx_vutend = myPicture.myTP[i + 1].const_begin;
                            }

                            if (myPicture.myTP[i].idx_90begin > myPicture.myTP[i].idx_50end)
                            {
                                myPicture.myTP[i].idx_90begin = myPicture.myTP[i].idx_50end;
                            }
                        }
                    }

                    //消除鼠标选点
                    myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(-1, -1);
                }

                //修改选择框
                myPicture.getPoint_pictureBox();

                //画底层
                pictureBoxScope_axis();

                //画顶层
                pictureBoxScope_draw();

                //手动选点
                myPicture.isSet = true;
            }
        }

        //更新表格
        private void dataGridView_update()
        {
            //
            dataGridView1.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.Font = myPicture.font_txt;

            //
            dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView1.EnableHeadersVisualStyles = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToOrderColumns = false;
            dataGridView1.AllowUserToResizeColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.ReadOnly = true;

            //
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns[0].HeaderText = "序号";
            dataGridView1.Columns[0].DataPropertyName = "Index";

            dataGridView1.Columns[1].HeaderText = "时间";
            dataGridView1.Columns[1].DataPropertyName = "Time";

            dataGridView1.Columns[2].HeaderText = "A1";
            dataGridView1.Columns[2].DataPropertyName = "A1";

            dataGridView1.Columns[3].HeaderText = "A4";
            dataGridView1.Columns[3].DataPropertyName = "A4";

            dataGridView1.Columns[4].HeaderText = "A7";
            dataGridView1.Columns[4].DataPropertyName = "A7";

            dataGridView1.Columns[5].HeaderText = "A10";
            dataGridView1.Columns[5].DataPropertyName = "A10";

            dataGridView1.Columns[6].HeaderText = "A12";
            dataGridView1.Columns[6].DataPropertyName = "A12";

            dataGridView1.Columns[7].HeaderText = "D1";
            dataGridView1.Columns[7].DataPropertyName = "D1";

            dataGridView1.Columns[8].HeaderText = "D7";
            dataGridView1.Columns[8].DataPropertyName = "D7";

            dataGridView1.Columns[9].HeaderText = "D12";
            dataGridView1.Columns[9].DataPropertyName = "D12";

            dataGridView1.Columns[10].HeaderText = "E4";
            dataGridView1.Columns[10].DataPropertyName = "E4";

            dataGridView1.Columns[11].HeaderText = "E10";
            dataGridView1.Columns[11].DataPropertyName = "E10";

            dataGridView1.Columns[12].HeaderText = "H1";
            dataGridView1.Columns[12].DataPropertyName = "H1";

            dataGridView1.Columns[13].HeaderText = "H4";
            dataGridView1.Columns[13].DataPropertyName = "H4";

            dataGridView1.Columns[14].HeaderText = "H7";
            dataGridView1.Columns[14].DataPropertyName = "H7";

            dataGridView1.Columns[15].HeaderText = "H10";
            dataGridView1.Columns[15].DataPropertyName = "H10";

            dataGridView1.Columns[16].HeaderText = "H12";
            dataGridView1.Columns[16].DataPropertyName = "H12";

            dataGridView1.Columns[10].Visible = false;
            dataGridView1.Columns[11].Visible = false;
            dataGridView1.Columns[12].Visible = false;
            dataGridView1.Columns[13].Visible = false;
            dataGridView1.Columns[14].Visible = false;
            dataGridView1.Columns[15].Visible = false;
            dataGridView1.Columns[16].Visible = false;
            //
            dataGridView1.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[1].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[2].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[3].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[5].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[6].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[7].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[8].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[9].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[10].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[11].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[12].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[13].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[14].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[15].SortMode = DataGridViewColumnSortMode.NotSortable;
            dataGridView1.Columns[16].SortMode = DataGridViewColumnSortMode.NotSortable;

            //
            index = 0;
            tmpMax = 0;
            tmpMin = 10000;
            dataGridView1.Rows.Clear();
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dataGridView1.Columns[0].Width = 40;
            dataGridView1.Columns[1].Width = 40;
            dataGridView1.Columns[2].Width = 30;
            dataGridView1.Columns[3].Width = 30;
            dataGridView1.Columns[4].Width = 30;
            dataGridView1.Columns[5].Width = 30;
            dataGridView1.Columns[6].Width = 30;
            dataGridView1.Columns[7].Width = 30;
            dataGridView1.Columns[8].Width = 30;
            dataGridView1.Columns[9].Width = 30;
            dataGridView1.Columns[10].Width = 0;
            dataGridView1.Columns[11].Width = 0;
            dataGridView1.Columns[12].Width = 0;
            dataGridView1.Columns[13].Width = 0;
            dataGridView1.Columns[14].Width = 0;
            dataGridView1.Columns[15].Width = 0;
            dataGridView1.Columns[16].Width = 0;
            if (MyDefine.myXET.myHom.Count > 0)
            {
                //触发加载数据timer
                timer1.Enabled = true;
            }
        }

        //实际操作函数
        private void dataForm_Update()
        {
            //其它线程的操作请求
            if (this.InvokeRequired)
            {
                try
                {
                    freshHandler meDelegate = new freshHandler(dataForm_Update);
                    this.Invoke(meDelegate, new object[] { });
                }
                catch
                {
                    MyDefine.myXET.myUpdate -= new freshHandler(dataForm_Update);
                }
            }
            //本线程的操作请求
            else
            {
                //规程ComboBox
                MyDefine.myXET.GetUserPrm();
                comboBox1.Items.Clear();
                for (int i = 0; i < MyDefine.myXET.myPRM.Count; i++)
                {
                    comboBox1.Items.Add("#" + (i + 1).ToString() + "  " + MyDefine.myXET.myPRM[i].name);
                }
                comboBox1.SelectedIndex = 0;

                //表格重新加载
                index = 0;
                tmpMax = 0;
                tmpMin = 10000;
                dataGridView1.Rows.Clear();
                dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dataGridView1.Columns[0].Width = 40;
                dataGridView1.Columns[1].Width = 40;
                dataGridView1.Columns[2].Width = 30;
                dataGridView1.Columns[3].Width = 30;
                dataGridView1.Columns[4].Width = 30;
                dataGridView1.Columns[5].Width = 30;
                dataGridView1.Columns[6].Width = 30;
                dataGridView1.Columns[7].Width = 30;
                dataGridView1.Columns[8].Width = 30;
                dataGridView1.Columns[9].Width = 30;
                dataGridView1.Columns[10].Width = 30;
                dataGridView1.Columns[11].Width = 30;
                dataGridView1.Columns[12].Width = 30;
                dataGridView1.Columns[13].Width = 30;
                dataGridView1.Columns[14].Width = 30;
                dataGridView1.Columns[15].Width = 30;
                dataGridView1.Columns[16].Width = 30;
                if (MyDefine.myXET.myHom.Count > 0)
                {
                    timer1.Enabled = true;
                }
            }
        }

        //显示界面后再刷新表格,每10ms加入部分数据
        private void timer1_Tick(object sender, EventArgs e)
        {
            //每次加入部分数据
            for (int i = 0; i < 200; i++)
            {
                if (index < MyDefine.myXET.myHom.Count)
                {
                    //行数
                    Int32 idx = dataGridView1.Rows.Add();

                    //数据
                    dataGridView1.Rows[idx].Cells[0].Value = (idx + 1).ToString();
                    dataGridView1.Rows[idx].Cells[1].Value = MyDefine.myXET.myHom[index].time.ToString("HH:mm:ss.fff");
                    dataGridView1.Rows[idx].Cells[2].Value = (MyDefine.myXET.myHom[index].OUT[0] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[3].Value = (MyDefine.myXET.myHom[index].OUT[1] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[4].Value = (MyDefine.myXET.myHom[index].OUT[2] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[5].Value = (MyDefine.myXET.myHom[index].OUT[3] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[6].Value = (MyDefine.myXET.myHom[index].OUT[4] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[7].Value = (MyDefine.myXET.myHom[index].OUT[5] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[8].Value = (MyDefine.myXET.myHom[index].OUT[6] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[9].Value = (MyDefine.myXET.myHom[index].OUT[7] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[10].Value = (MyDefine.myXET.myHom[index].OUT[8] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[11].Value = (MyDefine.myXET.myHom[index].OUT[9] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[12].Value = (MyDefine.myXET.myHom[index].OUT[10] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[13].Value = (MyDefine.myXET.myHom[index].OUT[11] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[14].Value = (MyDefine.myXET.myHom[index].OUT[12] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[15].Value = (MyDefine.myXET.myHom[index].OUT[13] / 100.0f).ToString("f2");
                    dataGridView1.Rows[idx].Cells[16].Value = (MyDefine.myXET.myHom[index].OUT[14] / 100.0f).ToString("f2");

                    //
                    for (int k = 0; k < SZ.CHA; k++)
                    {
                        tmpMax = Math.Max(tmpMax, MyDefine.myXET.myHom[index].OUT[k]);
                        tmpMin = Math.Min(tmpMin, MyDefine.myXET.myHom[index].OUT[k]);
                    }

                    //温度数据指针
                    index++;
                }
                else
                {
                    //用timer2来调整表格,防止本处触发多次
                    timer1.Enabled = false;
                    timer2.Enabled = true;
                    return;
                }
            }

            //移到最后一行
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.RowCount - 1;
        }

        //调整表格
        private void timer2_Tick(object sender, EventArgs e)
        {
            timer2.Enabled = false;

            //调整列宽
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;

            //移动到第一行
            dataGridView1.FirstDisplayedScrollingRowIndex = 0;

            //可以画曲线
            myPicture.isLoad = true;
            myPicture.isFull = true;
            myPicture.tmpStart = 0;
            myPicture.tmpStop = MyDefine.myXET.myHom.Count - 1;

            //画曲线
            this.MenuDataForm_SizeChanged(null, null);
        }

        //更新修正数据功能启用状态
        public void refreshCorrectionDataStatus()
        {
            if (((MyDefine.myXET.userName == "zhoup") && (MyDefine.myXET.userPassword == "TVS") && (MyDefine.myXET.myPC == 1))
                || ((MyDefine.myXET.userName == "JDeGree") && (MyDefine.myXET.userPassword == "leiG") && (MyDefine.myXET.myPC == 1)))
            {
                btnCorrectionTemp.Visible = true;
                btnCorrectionTopTemp.Visible = true;
            }
            else
            {
                btnCorrectionTemp.Visible = false;
                btnCorrectionTopTemp.Visible = false;
            }

            //规程ComboBox
            MyDefine.myXET.GetUserPrm();
            comboBox1.Items.Clear();
            for (int i = 0; i < MyDefine.myXET.myPRM.Count; i++)
            {
                if (MyDefine.myXET.myPRM[i].myTP.Count > 0)//规程没有关键点位的不添加规程选项
                {
                    comboBox1.Items.Add("#" + (i + 1).ToString() + "  " + MyDefine.myXET.myPRM[i].name);
                }
            }
            //触发comboBox1变化
            comboBox1.SelectedIndex = 0;
        }

        //启动
        public void MenuDataForm_Load(object sender, EventArgs e)
        {
            //更新修正数据功能启用状态
            refreshCorrectionDataStatus();


            //表格初始化
            dataGridView_update();

            //Main主菜单,加载数据,修改规程,等操作后需要更新本地窗口
            MyDefine.myXET.mfUpdate += new freshHandler(dataForm_Update);
        }

        //退出
        private void MenuDataForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            MyDefine.myXET.mfUpdate -= new freshHandler(dataForm_Update);
        }

        //重画曲线
        private void MenuDataForm_SizeChanged(object sender, EventArgs e)
        {
            if (myPicture.isLoad)
            {
                //初始化位置
                myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(-1, -1);

                //计算
                myPicture.getAxis_pictureBox(pictureBox1.Width, pictureBox1.Height, this.tmpMax, this.tmpMin);
                myPicture.getPoint_pictureBox();

                //画底层
                pictureBoxScope_axis();

                //画顶层
                pictureBoxScope_draw();
            }
        }

        //鼠标点击
        private void dataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (myPicture.isLoad)
            {
                //初始化位置
                myPicture.tmpPick = dataGridView1.CurrentRow.Index;

                //计算顶层
                myPicture.getPoint_pictureBox();

                //画顶层
                pictureBoxScope_draw();
            }
        }

        //鼠标点击
        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (myPicture.isLoad)
            {
                //取消选中
                for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
                {
                    dataGridView1.SelectedRows[i].Selected = false;
                }

                //初始化位置
                myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(e.Location.X, e.Location.Y);

                //计算顶层
                myPicture.getPoint_pictureBox();

                //画顶层
                pictureBoxScope_draw();

                //选中表格
                if (myPicture.tmpPick < dataGridView1.RowCount)
                {
                    dataGridView1.Rows[myPicture.tmpPick].Selected = true;

                    //移到表格
                    if (myPicture.tmpPick > 15)
                    {
                        dataGridView1.FirstDisplayedScrollingRowIndex = myPicture.tmpPick - 15;
                    }
                    else
                    {
                        dataGridView1.FirstDisplayedScrollingRowIndex = 0;
                    }
                }
            }
        }

        //双击单独标定
        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount == 0) return;

            //权限
            if (!(((MyDefine.myXET.userName == "zhoup") && (MyDefine.myXET.userPassword == "TVS") && (MyDefine.myXET.myPC == 1))
                || ((MyDefine.myXET.userName == "JDeGree") && (MyDefine.myXET.userPassword == "leiG") && (MyDefine.myXET.myPC == 1))
                || ((MyDefine.myXET.userName == "bohui") && (MyDefine.myXET.myPC == 1))))
            {
                return;
            }

            if (dataGridView1.Rows.Count >= 0)
            {
                // 获取第10行的单元格集合
                DataGridViewRow row = dataGridView1.Rows[myPicture.tmpPick]; // 第10行，索引从0开始
                DataGridViewCellCollection cells = row.Cells;

                tmpPickData.Clear();

                // 遍历单元格集合，将单元格的值添加到 List 中
                for (int i = 2; i < cells.Count; i++)
                {
                    tmpPickData.Add(cells[i].Value.ToString());
                }

                if (MyDefine.myXET.mePort.IsOpen)
                {
                    MenuIndividualCalFormTVS8 menuIndividualCalForm = new MenuIndividualCalFormTVS8(tmpPickData);
                    menuIndividualCalForm.StartPosition = FormStartPosition.CenterScreen;
                    menuIndividualCalForm.ShowDialog();
            }
        }
        }

        //画底层
        private void pictureBoxScope_axis(int index = 0)
        {
            //第idx条横线
            int idx = 1;

            //刻度温度值
            int tmp = myPicture.tmpHigh;

            //刻度Y坐标
            int yline = idx * 500 * myPicture.Height / (myPicture.tmpHigh - myPicture.tmpLow);

            //层图
            Bitmap img = new Bitmap(myPicture.Width, myPicture.Height);

            //绘制
            Graphics g = Graphics.FromImage(img);

            //画刻度
            while (yline < myPicture.Height)
            {
                tmp -= 500;

                yline = idx * 500 * myPicture.Height / (myPicture.tmpHigh - myPicture.tmpLow);

                //画横线
                g.DrawLine(new Pen(myPicture.color_axis, 1.0f), new Point(0, yline), new Point(myPicture.Width, yline));

                //温度刻度
                g.DrawString((tmp / 100).ToString() + "℃", myPicture.font_txt, myPicture.brush_axis, 0, yline - 12);

                idx++;
            }
            switch (index)
            {
                default:
                    //画温度线
                    g.DrawCurve(new Pen(myPicture.color_pb1, 1.0f), myPointToPoint(myPicture.probe1), 0);
                    g.DrawCurve(new Pen(myPicture.color_pb2, 1.0f), myPointToPoint(myPicture.probe2), 0);
                    g.DrawCurve(new Pen(myPicture.color_pb3, 1.0f), myPointToPoint(myPicture.probe3), 0);
                    g.DrawCurve(new Pen(myPicture.color_pb4, 1.0f), myPointToPoint(myPicture.probe4), 0);
                    g.DrawCurve(new Pen(myPicture.color_pb5, 1.0f), myPointToPoint(myPicture.probe5), 0);
                    g.DrawCurve(new Pen(myPicture.color_pb6, 1.0f), myPointToPoint(myPicture.probe6), 0);
                    g.DrawCurve(new Pen(myPicture.color_pb7, 1.0f), myPointToPoint(myPicture.probe7), 0);
                    g.DrawCurve(new Pen(myPicture.color_pb8, 1.0f), myPointToPoint(myPicture.probe8), 0);
                    break;
                case 1:
                    g.DrawCurve(new Pen(myPicture.color_pb1, 1.0f), myPointToPoint(myPicture.probe1), 0); break;
                case 2:
                    g.DrawCurve(new Pen(myPicture.color_pb2, 1.0f), myPointToPoint(myPicture.probe2), 0); break;
                case 3:
                    g.DrawCurve(new Pen(myPicture.color_pb3, 1.0f), myPointToPoint(myPicture.probe3), 0); break;
                case 4:
                    g.DrawCurve(new Pen(myPicture.color_pb4, 1.0f), myPointToPoint(myPicture.probe4), 0); break;
                case 5:
                    g.DrawCurve(new Pen(myPicture.color_pb5, 1.0f), myPointToPoint(myPicture.probe5), 0); break;
                case 6:
                    g.DrawCurve(new Pen(myPicture.color_pb6, 1.0f), myPointToPoint(myPicture.probe6), 0); break;
                case 7:
                    g.DrawCurve(new Pen(myPicture.color_pb7, 1.0f), myPointToPoint(myPicture.probe7), 0); break;
                case 8:
                    g.DrawCurve(new Pen(myPicture.color_pb8, 1.0f), myPointToPoint(myPicture.probe8), 0); break;
            }

            //铺图
            pictureBox1.BackgroundImage = img;

            //
            g.Dispose();
        }

        /// <summary>
        /// MyPoint转Point
        /// </summary>
        /// <param name="mp1"></param>mo
        /// <returns></returns>
        private Point[] myPointToPoint(List<MyPoint> mp1)
        {
            MyPoint[] mp = mp1.ToArray();
            Point[] arrayB = new Point[myPicture.probe1.Count];

            //给point[]赋值
            for (int i = 0; i < mp.Length; i++)
            {
                arrayB[i] = new Point(mp[i].x, mp[i].y);
            }

            return arrayB;
        }

        //画顶层
        private void pictureBoxScope_draw()
        {
            Color myColor;
            Brush myBrush;

            //层图
            Bitmap img = new Bitmap(myPicture.Width, myPicture.Height);

            //绘制
            Graphics g = Graphics.FromImage(img);

            //画线,数据起始,数据结束,选中的点
            if (myPicture.xline_start > myPicture.WTEXT)
            {
                g.DrawLine(new Pen(myPicture.color_axis, 3.0f), new Point(myPicture.xline_start, 0), new Point(myPicture.xline_start, myPicture.Height));
            }
            if (myPicture.xline_stop > myPicture.WTEXT)
            {
                g.DrawLine(new Pen(myPicture.color_axis, 3.0f), new Point(myPicture.xline_stop, 0), new Point(myPicture.xline_stop, myPicture.Height));
            }
            if (myPicture.xline_pick > myPicture.WTEXT)
            {
                g.DrawLine(new Pen(myPicture.color_info, 1.0f), new Point(myPicture.xline_pick, 0), new Point(myPicture.xline_pick, myPicture.Height));
            }

            //选点
            for (int i = 0; i < myPicture.myTP.Count; i++)
            {
                if (comboBox2.SelectedIndex == i)
                {
                    myColor = myPicture.color_active;
                    myBrush = myPicture.brush_active;
                }
                else
                {
                    myColor = myPicture.color_inactive;
                    myBrush = myPicture.brush_inactive;
                }

                if (myPicture.myTP[i].xline_const_end > myPicture.WTEXT)
                {
                    //不在最左边
                    if (myPicture.myTP[i].xline_const_begin > myPicture.WTEXT)
                    {
                        g.DrawLine(new Pen(myColor, 2.0f),
                            new Point(myPicture.myTP[i].xline_const_begin, myPicture.myTP[i].yline_top),
                            new Point(myPicture.myTP[i].xline_const_begin, myPicture.myTP[i].yline_bottom));

                        g.DrawLine(new Pen(myColor, 2.0f),
                            new Point(myPicture.myTP[i].xline_const_end, myPicture.myTP[i].yline_top),
                            new Point(myPicture.myTP[i].xline_const_end, myPicture.myTP[i].yline_bottom));

                        g.DrawLine(new Pen(myColor, 2.0f),
                            new Point(myPicture.myTP[i].xline_const_begin, myPicture.myTP[i].yline_top),
                            new Point(myPicture.myTP[i].xline_const_end, myPicture.myTP[i].yline_top));

                        g.DrawLine(new Pen(myColor, 2.0f),
                            new Point(myPicture.myTP[i].xline_const_begin, myPicture.myTP[i].yline_bottom),
                            new Point(myPicture.myTP[i].xline_const_end, myPicture.myTP[i].yline_bottom));

                        int bottom;
                        string str = ((myPicture.myTP[i].const_end - myPicture.myTP[i].const_begin) * MyDefine.myXET.homstep / 1000).ToString("f1");

                        //下面1/3的框框的文字
                        if (myPicture.myTP[i].temperature < ((myPicture.tmpHigh + myPicture.tmpLow) / 300))
                        {
                            bottom = myPicture.myTP[i].yline_top - 30;

                            g.DrawString("P" + (i + 1).ToString() + "," + (myPicture.myTP[i].temperature / 100f).ToString() + "℃",
                                myPicture.font_txt,
                                myBrush,
                                myPicture.myTP[i].xline_const_begin - 5,
                                bottom + 13);

                            g.DrawString(str + " sec",
                                myPicture.font_txt,
                                myBrush,
                                myPicture.myTP[i].xline_const_begin - 5,
                                bottom);
                        }
                        //上面2/3的框框的文字
                        else
                        {
                            bottom = myPicture.myTP[i].yline_bottom + 5;

                            g.DrawString("P" + (i + 1).ToString() + " " + (myPicture.myTP[i].temperature / 100f).ToString() + "℃",
                                myPicture.font_txt,
                                myBrush,
                                myPicture.myTP[i].xline_const_begin - 5,
                                bottom);

                            g.DrawString(str + " sec",
                                myPicture.font_txt,
                                myBrush,
                                myPicture.myTP[i].xline_const_begin - 5,
                                bottom + 13);
                        }
                    }
                    //在最左边只有部分框框
                    else
                    {
                        g.DrawLine(new Pen(myColor, 2.0f),
                            new Point(myPicture.myTP[i].xline_const_end, myPicture.myTP[i].yline_top),
                            new Point(myPicture.myTP[i].xline_const_end, myPicture.myTP[i].yline_bottom));

                        g.DrawLine(new Pen(myColor, 2.0f),
                            new Point(myPicture.WTEXT, myPicture.myTP[i].yline_top),
                            new Point(myPicture.myTP[i].xline_const_end, myPicture.myTP[i].yline_top));

                        g.DrawLine(new Pen(myColor, 2.0f),
                            new Point(myPicture.WTEXT, myPicture.myTP[i].yline_bottom),
                            new Point(myPicture.myTP[i].xline_const_end, myPicture.myTP[i].yline_bottom));
                    }
                }

                //升温框框
                if (myPicture.myTP[i].vutup)
                {
                    if (myPicture.myTP[i].xline_90end > myPicture.WTEXT)
                    {
                        //不在最左边
                        if (myPicture.myTP[i].xline_50begin > myPicture.WTEXT)
                        {
                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.myTP[i].xline_50begin, myPicture.myTP[i].yline_50),
                                new Point(myPicture.myTP[i].xline_50begin, myPicture.myTP[i].yline_90));

                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.myTP[i].xline_90end, myPicture.myTP[i].yline_50),
                                new Point(myPicture.myTP[i].xline_90end, myPicture.myTP[i].yline_90));

                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.myTP[i].xline_50begin, myPicture.myTP[i].yline_50),
                                new Point(myPicture.myTP[i].xline_90end, myPicture.myTP[i].yline_50));

                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.myTP[i].xline_50begin, myPicture.myTP[i].yline_90),
                                new Point(myPicture.myTP[i].xline_90end, myPicture.myTP[i].yline_90));
                        }
                        //在最左边只有部分框框
                        else
                        {
                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.myTP[i].xline_90end, myPicture.myTP[i].yline_50),
                                new Point(myPicture.myTP[i].xline_90end, myPicture.myTP[i].yline_90));

                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.WTEXT, myPicture.myTP[i].yline_50),
                                new Point(myPicture.myTP[i].xline_90end, myPicture.myTP[i].yline_50));

                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.WTEXT, myPicture.myTP[i].yline_90),
                                new Point(myPicture.myTP[i].xline_90end, myPicture.myTP[i].yline_90));
                        }
                    }
                }

                //降温框框
                if (myPicture.myTP[i].vutdown)
                {

                    if (myPicture.myTP[i].xline_50end > myPicture.WTEXT)
                    {
                        //不在最左边
                        if (myPicture.myTP[i].xline_90begin > myPicture.WTEXT)
                        {
                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.myTP[i].xline_90begin, myPicture.myTP[i].yline_50),
                                new Point(myPicture.myTP[i].xline_90begin, myPicture.myTP[i].yline_90));

                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.myTP[i].xline_50end, myPicture.myTP[i].yline_50),
                                new Point(myPicture.myTP[i].xline_50end, myPicture.myTP[i].yline_90));

                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.myTP[i].xline_90begin, myPicture.myTP[i].yline_50),
                                new Point(myPicture.myTP[i].xline_50end, myPicture.myTP[i].yline_50));

                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.myTP[i].xline_90begin, myPicture.myTP[i].yline_90),
                                new Point(myPicture.myTP[i].xline_50end, myPicture.myTP[i].yline_90));
                        }
                        //在最左边只有部分框框
                        else
                        {
                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.myTP[i].xline_50end, myPicture.myTP[i].yline_50),
                                new Point(myPicture.myTP[i].xline_50end, myPicture.myTP[i].yline_90));

                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.WTEXT, myPicture.myTP[i].yline_50),
                                new Point(myPicture.myTP[i].xline_50end, myPicture.myTP[i].yline_50));

                            g.DrawLine(new Pen(myPicture.color_vut, 2.0f),
                                new Point(myPicture.WTEXT, myPicture.myTP[i].yline_90),
                                new Point(myPicture.myTP[i].xline_50end, myPicture.myTP[i].yline_90));
                        }
                    }
                }
            }

            if (myPicture.tmpPick <= MyDefine.myXET.myHom.Count)//防止当前选择点超出新加载文件的最大点序号
            {
                //显示统计值
                if (myPicture.xline_pick > myPicture.WTEXT)
                {
                    int px = myPicture.xline_pick + 3;

                    //文字宽度
                    string str;
                    SizeF mySize;
                    if ((MyDefine.myXET.myHom[myPicture.tmpPick].sopMax + MyDefine.myXET.myHom[myPicture.tmpPick].sopMin) >= 0)
                    {
                        str = "最大温度变化率: " + MyDefine.myXET.GetTemp(MyDefine.myXET.myHom[myPicture.tmpPick].sopMax) + "℃/sec";
                    }
                    else
                    {
                        str = "最小温度变化率: " + MyDefine.myXET.GetTemp(MyDefine.myXET.myHom[myPicture.tmpPick].sopMin) + "℃/sec";
                    }
                    mySize = g.MeasureString(str, myPicture.font_txt);

                    //窗口右边文字调整
                    if (((px + mySize.Width) > myPicture.Width) && (px > (mySize.Width + 6)))
                    {
                        px = (int)(px - mySize.Width - 6);
                    }

                    //显示
                    if ((MyDefine.myXET.myHom[myPicture.tmpPick].sopMax + MyDefine.myXET.myHom[myPicture.tmpPick].sopMin) >= 0)
                    {
                        g.DrawString("最大温度变化率: " + MyDefine.myXET.GetTemp(MyDefine.myXET.myHom[myPicture.tmpPick].sopMax) + "℃/sec", myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick);
                        g.DrawString("最小温度变化率: " + MyDefine.myXET.GetTemp(MyDefine.myXET.myHom[myPicture.tmpPick].sopMin) + "℃/sec", myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 20);
                    }
                    else
                    {
                        g.DrawString("最大温度变化率: " + MyDefine.myXET.GetTemp(MyDefine.myXET.myHom[myPicture.tmpPick].sopMin) + "℃/sec", myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick);
                        g.DrawString("最小温度变化率: " + MyDefine.myXET.GetTemp(MyDefine.myXET.myHom[myPicture.tmpPick].sopMax) + "℃/sec", myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 20);
                    }

                    g.DrawString("平均温度变化率: " + MyDefine.myXET.GetTemp(MyDefine.myXET.myHom[myPicture.tmpPick].sopAvg) + "℃/sec", myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 40);
                    g.DrawString("最大值: " + MyDefine.myXET.GetTemp(MyDefine.myXET.myHom[myPicture.tmpPick].outMax) + "℃", myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 60);
                    g.DrawString("最小值: " + MyDefine.myXET.GetTemp(MyDefine.myXET.myHom[myPicture.tmpPick].outMin) + "℃", myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 80);
                    g.DrawString("孔间差: " + MyDefine.myXET.GetTemp(MyDefine.myXET.myHom[myPicture.tmpPick].outDif) + "℃", myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 100);
                    g.DrawString("平均值: " + MyDefine.myXET.GetTemp(MyDefine.myXET.myHom[myPicture.tmpPick].outAvg) + "℃", myPicture.font_txt, myPicture.brush_info, px, myPicture.yline_pick + 120);
                }
            }

            //铺图
            pictureBox1.Image = img;

            //
            g.Dispose();
        }

        //创建段落
        private Paragraph CreateParagraph(string str, iTextSharp.text.Font font, int align)
        {
            Paragraph mp = new Paragraph(str, font);

            //Element.ALIGN_LEFT
            //Element.ALIGN_CENTER
            //Element.ALIGN_RIGHT
            mp.Alignment = align;

            //
            mp.SpacingBefore = 5.0f;
            mp.SpacingAfter = 5.0f;

            return mp;
        }

        //创建段落
        private Paragraph CreateInformation(string str, iTextSharp.text.Font font, int align)
        {
            Paragraph mp = new Paragraph(str, font);

            //Element.ALIGN_LEFT
            //Element.ALIGN_CENTER
            //Element.ALIGN_RIGHT
            mp.Alignment = align;

            //
            mp.SpacingBefore = 0;
            mp.SpacingAfter = 0;

            return mp;
        }

        //字符串长度增加add值并控制等长,最小min
        private int GetJoinLen(List<String> str, int min, int add)
        {
            int max = min;
            int len = 0;

            for (int i = 0; i < str.Count; i++)
            {
                len = Encoding.Default.GetBytes(str[i]).Length;
                if (len > max)
                {
                    max = len;
                }
            }

            max += add;

            return max;
        }

        //页眉页脚水印
        private class IsHandF : PdfPageEventHelper, IPdfPageEvent
        {
            //页事件
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                //页眉页脚使用字体
                iTextSharp.text.Font fontJingdu = new iTextSharp.text.Font(BaseFont.CreateFont(@".\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 7.0f, iTextSharp.text.Font.NORMAL);

                //页眉 Top - y
                //页脚 Bottom + y
                PdfContentByte myIfo = writer.DirectContent;
                Phrase footer1 = new Phrase("Crystallinity Technology (Beijing) Co., Ltd.", fontJingdu);
                Phrase footer2 = new Phrase("Tel：400-101-0927  Email：18611724927@163.com", fontJingdu);
                Phrase footer3 = new Phrase("Web：www.jdujs.com", fontJingdu);
                ColumnText.ShowTextAligned(myIfo, Element.ALIGN_CENTER, footer1, document.Right / 2, document.Bottom + 32, 0);
                ColumnText.ShowTextAligned(myIfo, Element.ALIGN_CENTER, footer2, document.Right / 2, document.Bottom + 22, 0);
                ColumnText.ShowTextAligned(myIfo, Element.ALIGN_CENTER, footer3, document.Right / 2, document.Bottom + 12, 0);

                //水印
                PdfContentByte myPic = writer.DirectContentUnder;//水印在内容下方添加
                PdfGState myGS = new PdfGState();
                iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(MyDefine.myXET.userPIC + @"\logo.jpg");//水印图片
                image.RotationDegrees = 30;//旋转角度
                myGS.FillOpacity = 0.1f;//透明度
                myPic.SetGState(myGS);

                //document.Left = 48
                //document.Right = 579
                //document.Top = 826
                //document.Bottom = 0
                //
                //document.LeftMargin = 48
                //document.RightMargin = 16
                //document.TopMargin = 16
                //document.BottomMargin = 0
                //
                //image.Left = 0
                //image.Right = 53
                //image.Top = 43
                //image.Bottom = 0
                //
                float width = document.Right + document.RightMargin; //pdf宽
                float height = document.Top + document.TopMargin; //pdf高
                float xnum = 3; //一行3个logo
                float ynum = 5; //一列5个logo
                float xspace = (width - (xnum * image.Right)) / xnum; //logo间距
                float yspace = (height - (ynum * image.Top)) / ynum; //logo间距
                for (int x = 0; x < xnum; x++)
                {
                    for (int y = 0; y < ynum; y++)
                    {
                        image.SetAbsolutePosition(0.5f * xspace + x * (xspace + image.Right), 0.5f * yspace + y * (yspace + image.Top));
                        myPic.AddImage(image);
                    }
                }
            }
        }

        //生成报告
        private void CreatDataPdf()
        {
            #region 参数

            List<TMP> myRPT = MyDefine.myXET.myHom;  //温度数据列表

            const int LENL = 24;
            const int LENS = 8;
            const int LENW = 4;

            int page = 0;
            int idx;
            int myMax = 0;
            int myTmp = 0;

            String blankLine = " ";
            String mmStr = "";
            List<String> myLS = new List<String>();

            #endregion

            #region 报告文件

            //保存报告路径和文件名
            if (!Directory.Exists(MyDefine.myXET.userOUT))
            {
                Directory.CreateDirectory(MyDefine.myXET.userOUT);
            }

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "请选择文件";
            fileDialog.Filter = "新建文件(*.pdf)|*.pdf";
            fileDialog.RestoreDirectory = true;
            fileDialog.InitialDirectory = MyDefine.myXET.userOUT;
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(fileDialog.FileName))
                {
                    File.Delete(fileDialog.FileName);
                }
            }
            else
            {
                return;
            }

            //创建新文档对象,页边距(X,X,Y,Y)
            Document document = new Document(PageSize.A4, 48, 16, 16, 0);

            //路径设置; FileMode.Create文档不在会创建，存在会覆盖
            PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(fileDialog.FileName, FileMode.Create));

            //添加信息
            document.AddTitle("PCR校准报告");
            document.AddAuthor(reportCompany + " " + reportDepartment + " " + reportStaff);
            document.AddSubject(machineType + " 温度测量报告");
            document.AddKeywords("PCR");
            document.AddCreator(reportStaff);

            //创建字体，STSONG.TTF空格不等宽
            iTextSharp.text.Font fontTitle = new iTextSharp.text.Font(BaseFont.CreateFont(@".\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 14.0f, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fontItem = new iTextSharp.text.Font(BaseFont.CreateFont(@".\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 10.0f, iTextSharp.text.Font.BOLD);
            iTextSharp.text.Font fontContent = new iTextSharp.text.Font(BaseFont.CreateFont(@".\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 10.0f, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fontMessage = new iTextSharp.text.Font(BaseFont.CreateFont(@".\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 8.0f, iTextSharp.text.Font.NORMAL);
            iTextSharp.text.Font fontJingdu = new iTextSharp.text.Font(BaseFont.CreateFont(@".\SIMYOU.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED), 7.0f, iTextSharp.text.Font.NORMAL);

            //页眉页脚水印
            writer.PageEvent = new IsHandF();

            #endregion

            #region 第一页

            //打开
            document.Open();

            //第一页添加元素
            document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(CreateParagraph(reportName + " Result", fontTitle, Element.ALIGN_CENTER));
            document.Add(CreateParagraph("（" + reportName + " 报告）", fontItem, Element.ALIGN_CENTER));
            if (File.Exists(MyDefine.myXET.userPIC + @"\logo.jpg"))
            {
                //document.Add(iTextSharp.text.Image.GetInstance(MyDefine.myXET.userPIC + @"\logo.jpg"));
                document.Add(new Paragraph(blankLine, fontItem));
                document.Add(new Paragraph(blankLine, fontItem));
            }
            else
            {
                document.Add(new Paragraph(blankLine, fontItem));
                document.Add(new Paragraph(blankLine, fontItem));
            }
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("General information".PadRight(LENL, ' '));
            myLS.Add("Report number".PadRight(LENL, ' '));
            myLS.Add("Report date".PadRight(LENL, ' '));
            myLS.Add("Test date".PadRight(LENL, ' '));
            myLS.Add("Test start time".PadRight(LENL, ' '));
            myLS.Add("Test end time".PadRight(LENL, ' '));
            myLS.Add("Test program".PadRight(LENL, ' '));
            myLS.Add("Report version".PadRight(LENL, ' '));
            myLS.Add("Source".PadRight(LENL, ' '));
            myLS.Add("Record".PadRight(LENL, ' '));
            myLS[1] = myLS[1] + ":  " + reportNumber;
            myLS[2] = myLS[2] + ":  " + reportDate;
            myLS[3] = myLS[3] + ":  20" + MyDefine.myXET.homdate.ToString("000000");
            myLS[4] = myLS[4] + ":  " + MyDefine.myXET.homtime.ToString("000000").Insert(2, ":").Insert(5, ":");
            myLS[5] = myLS[5] + ":  " + MyDefine.myXET.homstop.ToString("000000").Insert(2, ":").Insert(5, ":");
            myLS[6] = myLS[6] + ":  " + myPicture.name;
            myLS[7] = myLS[7] + ":  " + "0.0.2";
            myLS[8] = myLS[8] + ":  " + "载入数据";
            myLS[9] = myLS[9] + ":  " + Path.GetFileName(MyDefine.myXET.homFileName);
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(检测信息)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "(报告编号)";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "(报告日期)";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "(检测日期)";
            myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "(检测开始时间)";
            myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "(检测结束时间)";
            myLS[6] = myLS[6].PadRight(myMax + myLS[6].Length - Encoding.Default.GetBytes(myLS[6]).Length, ' ') + "(检测温度程序)";
            myLS[7] = myLS[7].PadRight(myMax + myLS[7].Length - Encoding.Default.GetBytes(myLS[7]).Length, ' ') + "(报告版本)";
            myLS[8] = myLS[8].PadRight(myMax + myLS[8].Length - Encoding.Default.GetBytes(myLS[8]).Length, ' ') + "(数据源)";
            myLS[9] = myLS[9].PadRight(myMax + myLS[9].Length - Encoding.Default.GetBytes(myLS[9]).Length, ' ') + "(文件)";
            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(myLS[1], fontContent));
            document.Add(new Paragraph(myLS[2], fontContent));
            document.Add(new Paragraph(myLS[3], fontContent));
            document.Add(new Paragraph(myLS[4], fontContent));
            document.Add(new Paragraph(myLS[5], fontContent));
            document.Add(new Paragraph(myLS[6], fontContent));
            document.Add(new Paragraph(myLS[7], fontContent));
            document.Add(new Paragraph(myLS[8], fontContent));
            document.Add(new Paragraph(myLS[9], fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Device information".PadRight(LENL, ' '));
            myLS.Add("Device model".PadRight(LENL, ' '));
            myLS.Add("Device serial number".PadRight(LENL, ' '));
            myLS.Add("Device description".PadRight(LENL, ' '));
            myLS[1] = myLS[1] + ":  " + machineType;
            myLS[2] = myLS[2] + ":  " + machineSN;
            myLS[3] = myLS[3] + ":  " + machineDescription;
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(仪器信息)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "(仪器型号)";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "(仪器序列号)";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "(仪器描述)";
            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(myLS[1], fontContent));
            document.Add(new Paragraph(myLS[2], fontContent));
            document.Add(new Paragraph(myLS[3], fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Jingdu TVS information".PadRight(LENL, ' '));
            myLS.Add("TVS universal number".PadRight(LENL, ' '));
            myLS.Add("TVS serial number".PadRight(LENL, ' '));
            myLS.Add("TVS software version".PadRight(LENL, ' '));
            myLS.Add("TVS calibration date".PadRight(LENL, ' '));
            myLS[1] = myLS[1] + ":  " + MyDefine.myXET.e_sidm.ToString("X8") + MyDefine.myXET.e_sidl.ToString("X8") + MyDefine.myXET.e_sidh.ToString("X8");
            myLS[2] = myLS[2] + ":  " + MyDefine.myXET.e_seri.ToString();
            myLS[3] = myLS[3] + ":  " + "0.1.06";
            myLS[4] = myLS[4] + ":  " + ("20" + MyDefine.myXET.e_seri.ToString().Substring(0, 6)).Insert(4, "-").Insert(7, "-");
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(采集器信息)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "(采集器唯一序列号)";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "(采集器批号)";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "(采集器软件版本)";
            myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "(采集器工厂标定日期)";
            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(myLS[1], fontContent));
            document.Add(new Paragraph(myLS[2], fontContent));
            document.Add(new Paragraph(myLS[3], fontContent));
            document.Add(new Paragraph(myLS[4], fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("User information".PadRight(LENL, ' '));
            myLS.Add("Name".PadRight(LENL, ' '));
            myLS.Add("Department".PadRight(LENL, ' '));
            myLS.Add("Institute".PadRight(LENL, ' '));
            myLS[1] = myLS[1] + ":  " + reportStaff;
            myLS[2] = myLS[2] + ":  " + reportDepartment;
            myLS[3] = myLS[3] + ":  " + reportCompany;
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(检测人员信息)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "(姓名)";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "(部门)";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "(公司)";
            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(myLS[1], fontContent));
            document.Add(new Paragraph(myLS[2], fontContent));
            document.Add(new Paragraph(myLS[3], fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////

            #endregion

            #region 第二页

            //更新页数和创建新页
            document.NewPage();
            document.ResetPageCount();

            //第二页添加元素
            document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Protocol information".PadRight(LENL, ' '));
            for (int i = 0; i < myPicture.myTP.Count; i++)
            {
                myLS.Add(("Temperature step " + (i + 1).ToString()).PadRight(LENL, ' '));

                string tmp = (myPicture.myTP[i].temperature / 100f).ToString();
                if (tmp.Length <= 2)
                {
                    tmp += ".00";
                }
                else if (tmp.Length < 5)
                {
                    tmp += "0";
                }
                myLS[i + 1] = myLS[i + 1] + ":  " + $"{tmp} ℃  for    " + myPicture.myTP[i].time.ToString() + " seconds";

                //myLS[i + 1] = myLS[i + 1] + ":  " + (myPicture.myTP[i].temperature / 100f).ToString() + ".00 ℃  for    " + myPicture.myTP[i].time.ToString() + " seconds";
            }
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(检测程序)";
            for (int i = 0; i < myPicture.myTP.Count; i++)
            {
                myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + "(步骤" + (i + 1).ToString() + ")";
            }
            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            for (int i = 0; i < myPicture.myTP.Count; i++)
            {
                document.Add(new Paragraph(myLS[i + 1], fontContent));
            }
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Probe information".PadRight(LENL, ' '));
            for (int i = 0; i < SZ.CHA - 7; i++)
            {
                //myRPT.Count
                myLS.Add(("Temperature probe " + (i + 1).ToString()).PadRight(LENL, ' '));

                myLS[i + 1] = myLS[i + 1] + ":  " + myRPT.Count.ToString() + " samples ,   " + (MyDefine.myXET.homrun / 1000).ToString() + " seconds total";
            }
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "(温度探头取样清单)";
            for (int i = 0; i < SZ.CHA - 7; i++)
            {
                myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + "(探头" + (i + 1).ToString() + ")";
            }
            document.Add(new Paragraph(myLS[0], fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            for (int i = 0; i < SZ.CHA - 7; i++)
            {
                document.Add(new Paragraph(myLS[i + 1], fontContent));
            }
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Legend        (备注)", fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("This legend can be used in combination with your individual temperature results.", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("        √√   Measured value is better than specificatioon", fontContent));
            document.Add(new Paragraph("        √     Measured value meets specificatioon", fontContent));
            document.Add(new Paragraph("        X      Measured value does NOT meet specificatioon", fontContent));
            document.Add(new Paragraph("        ?      No specificatioons found", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("        √√   优于市场标准范围", fontContent));
            document.Add(new Paragraph("        √     在市场标准范围内", fontContent));
            document.Add(new Paragraph("        X      超出市场标准范围", fontContent));
            document.Add(new Paragraph("        ?      没有标准可参考", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////

            #endregion

            #region 第三页

            //更新页数和创建新页
            document.NewPage();
            document.ResetPageCount();
            //第三页添加元素
            document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
            document.Add(new Paragraph(blankLine, fontItem));
            //第三页添加信息
            document.Add(new Paragraph("Probe temperature curve        (8根探头温度曲线图)", fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            //第三页添加曲线图
            document.Add(iTextSharp.text.Image.GetInstance(GetBitmapPoint(myPicture.tmpStart, myPicture.tmpStop, 0), System.Drawing.Imaging.ImageFormat.Bmp));

            #endregion

            #region 第四页

            //更新页数和创建新页
            document.NewPage();
            document.ResetPageCount();

            //第四页添加元素
            document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Probe layout (8v - 8)        (8根探头在采集器中的分布位置)", fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("          A1  A4  A7  A10 A12 D1  D7  D12", fontContent));
            document.Add(new Paragraph("          ●  ●  ●  ●  ●  ●  ●  ●", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("        Probe 1     (探头1)         : A1", fontContent));
            document.Add(new Paragraph("        Probe 2     (探头2)         : A4", fontContent));
            document.Add(new Paragraph("        Probe 3     (探头3)         : A7", fontContent));
            document.Add(new Paragraph("        Probe 4     (探头4)         : A10", fontContent));
            document.Add(new Paragraph("        Probe 5     (探头5)         : A12", fontContent));
            document.Add(new Paragraph("        Probe 6     (探头6)         : D1", fontContent));
            document.Add(new Paragraph("        Probe 7     (探头7)         : D7", fontContent));
            document.Add(new Paragraph("        Probe 8     (探头8)         : D12", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Recommanded control positions", fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("Positive :        (正控制位置)", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("        [min]", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("Negative :        (负控制位置)", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph("        [max]", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////

            #endregion

            #region 计算参数

            int step_start = 0;  //起始设定温度索引
            int step_target = 0; //目标设定温度索引
            int step_next = 1;   //下个目标温度索引

            bool rise;       //升降温标记
            int begin;       //设定温度的起始
            int end;         //设定温度的结束
            int const_begin; //恒温起始的点
            int const_end;   //恒温结束的点

            float space;     //规程时间分割成5段的时间间隔
            int idx_30sec;   //30秒处的的点
            int idx_first;   //等间隔处的点
            int idx_second;  //等间隔处的点
            int idx_third;   //等间隔处的点
            int idx_fourth;  //等间隔处的点
            int idx_fifth;   //等间隔处的点
            int idx_50begin; //从50度开始
            int idx_90end;   //到90度结束
            int idx_90begin; //从90度开始
            int idx_50end;   //到50度结束

            int spec_dev;    //标准参考值.与目标值偏差
            int spec_rate;   //标准参考值.速率
            int spec_over;   //标准参考值.超调
            int spec_max;    //标准参考值.均匀度(孔间最大差)

            int slope_start; //速率计算起始的点
            int slope_stop;  //速率计算结束的点

            float slope_tmp; //升降温起始和结束时刻的温度差值   20221205 spj 改为浮点数
            int slope_get;   //升降温速率的值,单位摄氏度每秒

            int over_max;    //最大过冲的值
            int over_avg;    //平均过冲的值

            int temp_max;    //找最大值
            int temp_min;    //找最小值
            int mark_max;    //记录探头位置
            int mark_min;    //记录探头位置

            #endregion

            #region 第一点计算

            int org_temp = myRPT[0].outAvg / 100 * 100; //起始点不带小数的温度(/100去除小数，*100放大100倍数)

            //时间更新
            rise = myPicture.myTP[step_target].rise;
            begin = myPicture.myTP[step_target].begin;
            end = myPicture.myTP[step_target].end;
            const_begin = myPicture.myTP[step_target].const_begin;
            const_end = myPicture.myTP[step_target].const_end;
            space = myPicture.myTP[step_target].space;
            idx_30sec = myPicture.myTP[step_target].idx_30sec;
            idx_first = myPicture.myTP[step_target].idx_first;
            idx_second = myPicture.myTP[step_target].idx_second;
            idx_third = myPicture.myTP[step_target].idx_third;
            idx_fourth = myPicture.myTP[step_target].idx_fourth;
            idx_fifth = myPicture.myTP[step_target].idx_fifth;

            //高温
            if (myPicture.myTP[step_target].temperature > 9000)
            {
                spec_dev = 80;    //0.8℃
                spec_rate = 150;  //1.5℃
                spec_over = 600;  //6.0℃
                spec_max = 150;   //1.5℃
            }
            else if (myPicture.myTP[step_target].temperature > 7000)
            {
                spec_dev = 60;
                spec_rate = 150;
                spec_over = 500;
                spec_max = 150;
            }
            else
            {
                spec_dev = 50;
                spec_rate = 150;
                spec_over = 500;
                spec_max = 100;
            }

            //速率
            if ((org_temp + 1500) < myPicture.myTP[step_target].temperature)
            {
                //升温
                slope_start = myPicture.GetUpThrough(begin, end, (org_temp + 500));
                slope_stop = myPicture.GetUpThrough(begin, end, (myPicture.myTP[step_target].temperature - 500));
                slope_tmp = (myPicture.myTP[step_target].temperature - org_temp - 1000) / 100f;
                slope_get = myPicture.GetHeatRate(slope_start, slope_stop);

            }
            else if (org_temp > (myPicture.myTP[step_target].temperature + 1500))
            {
                //降温
                slope_start = myPicture.GetDownThrough(begin, end, (org_temp - 500));
                slope_stop = myPicture.GetDownThrough(begin, end, (myPicture.myTP[step_target].temperature + 500));
                slope_tmp = (org_temp - myPicture.myTP[step_target].temperature - 1000) / 100f;
                slope_get = myPicture.GetCoolRate(slope_start, slope_stop);
            }
            else
            {
                slope_start = 0;
                slope_stop = 0;
                slope_tmp = 0;
                slope_get = 0;
            }

            //过冲
            over_max = myPicture.GetOverMax(begin, const_end, rise);
            over_avg = myPicture.GetOverAvg(begin, const_end, rise);

            #endregion

            #region 第一点曲线图

            //更新页数和创建新页
            document.NewPage();
            document.ResetPageCount();
            //第三页添加元素
            document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
            document.Add(new Paragraph(blankLine, fontItem));
            //第三页添加信息
            mmStr = (myPicture.myTP[step_target].temperature / 100f).ToString();

            if (mmStr.Length <= 2)
            {
                mmStr += ".00";
            }
            else if (mmStr.Length < 5)
            {
                mmStr += "0";
            }

            document.Add(new Paragraph("Probe temperature curve :  " + mmStr + " ℃        (" + mmStr + "℃时温度曲线)", fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            //第三页添加曲线图
            document.Add(iTextSharp.text.Image.GetInstance(GetBitmapPoint(begin, end, myPicture.myTP[step_target].temperature), System.Drawing.Imaging.ImageFormat.Bmp));

            #endregion

            #region 第一点报告

            //更新页数和创建新页
            document.NewPage();
            document.ResetPageCount();

            //第n页添加元素
            document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            mmStr = (myPicture.myTP[step_target].temperature / 100f).ToString();
            if (mmStr.Length <= 2)
            {
                mmStr += ".00";
            }
            else if (mmStr.Length < 5)
            {
                mmStr += "0";
            }
            document.Add(new Paragraph("Temperature        :  " + mmStr + " ℃        (" + mmStr + "℃时的温度表现 " + MyDefine.myXET.myHom[begin].time.ToString("HH:mm:ss") + "-" + MyDefine.myXET.myHom[end].time.ToString("HH:mm:ss") + ")", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Values result           (" + MyDefine.myXET.myHom[idx_30sec].time.ToString("HH:mm:ss") + ", " + mmStr + "℃ 8孔的温度数据)", fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("".PadRight(LENL, ' '));
            myLS.Add("Probe 1 (A1)".PadRight(LENL, ' '));
            myLS.Add("Probe 2 (A4)".PadRight(LENL, ' '));
            myLS.Add("Probe 3 (A7)".PadRight(LENL, ' '));
            myLS.Add("Probe 4 (A10)".PadRight(LENL, ' '));
            myLS.Add("Probe 5 (A12)".PadRight(LENL, ' '));
            myLS.Add("Probe 6 (D1)".PadRight(LENL, ' '));
            myLS.Add("Probe 7 (D7)".PadRight(LENL, ' '));
            myLS.Add("Probe 8 (D12)".PadRight(LENL, ' '));
            myLS[0] = myLS[0] + "Measured";
            for (int i = 0; i < SZ.CHA - 7; i++)
            {
                myLS[i + 1] = myLS[i + 1] + MyDefine.myXET.GetTemp(myRPT[idx_30sec].OUT[i]) + " ℃";
            }
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "T(meas-set)";
            temp_max = myRPT[idx_30sec].OUT[0];
            temp_min = myRPT[idx_30sec].OUT[0];
            mark_max = 1;
            mark_min = 1;
            for (int i = 0; i < SZ.CHA - 7; i++)
            {
                myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + MyDefine.myXET.GetTemp(myRPT[idx_30sec].OUT[i] - (myPicture.myTP[step_target].temperature /** 100*/)) + " ℃";
                if (myRPT[idx_30sec].OUT[i] > temp_max)
                {
                    temp_max = myRPT[idx_30sec].OUT[i];
                    mark_max = i + 1;
                }
                if (myRPT[idx_30sec].OUT[i] < temp_min)
                {
                    temp_min = myRPT[idx_30sec].OUT[i];
                    mark_min = i + 1;
                }
            }
            myLS[mark_max] = myLS[mark_max] + " (max)";
            myLS[mark_min] = myLS[mark_min] + " (min)";
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Status";
            for (int i = 0; i < SZ.CHA - 7; i++)
            {
                myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + "Active";
            }
            document.Add(new Paragraph(myLS[0], fontContent));
            for (int i = 0; i < SZ.CHA - 7; i++)
            {
                document.Add(new Paragraph(myLS[i + 1], fontContent));
            }
            myLS.Clear();
            myLS.Add("Probe Max. temperature  (最大值)   :".PadRight(LENL, ' '));
            myLS.Add("Probe Min. temperature  (最小值)   :".PadRight(LENL, ' '));
            myLS.Add("Probe Avg. temperature  (平均值)   :".PadRight(LENL, ' '));
            myLS.Add("Probe nonuniformity     (均匀度)   :".PadRight(LENL, ' '));
            myLS.Add("Probe error on measured (示值误差) :".PadRight(LENL, ' '));
            myMax = GetJoinLen(myLS, LENL, LENW);
            int[] OutTVS8 = new int[8];
            Array.Copy(myRPT[idx_30sec].OUT, OutTVS8, 8);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + MyDefine.myXET.GetTemp(OutTVS8.Max()) + " ℃";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + MyDefine.myXET.GetTemp(OutTVS8.Min()) + " ℃";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + MyDefine.myXET.GetTemp(myRPT[idx_30sec].outAvg) + " ℃";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + MyDefine.myXET.GetTemp(OutTVS8.Max() - OutTVS8.Min()) + " ℃";
            myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + MyDefine.myXET.GetTempWithSign((myPicture.myTP[step_target].temperature) - myRPT[idx_30sec].outAvg) + " ℃ (" + MyDefine.myXET.GetTemp(myPicture.myTP[step_target].temperature) + " - " + MyDefine.myXET.GetTemp(myRPT[idx_30sec].outAvg) + ")";
            document.Add(new Paragraph(myLS[0], fontContent));
            document.Add(new Paragraph(myLS[1], fontContent));
            document.Add(new Paragraph(myLS[2], fontContent));
            document.Add(new Paragraph(myLS[3], fontContent));
            document.Add(new Paragraph(myLS[4], fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Step results            (" + mmStr + "℃温度控制性能)", fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Item (项目)");
            if (rise)
            {
                if (slope_get > 0)
                {
                    myLS.Add("Max. heat rate (最大升温速率)");
                    myLS.Add("Avg. heat rate (平均升温速率)");
                }
                myLS.Add("Max. overshoot (最大过冲)");
                myLS.Add("Avg. overshoot (平均过冲)");
                myLS.Add("Heat time      (升温时间)");
                myLS.Add("Hold time      (恒温时间)");
            }
            else
            {
                if (slope_get > 0)
                {
                    myLS.Add("Max. cool rate (最大降温速率)");
                    myLS.Add("Avg. cool rate (平均降温速率)");
                }
                myLS.Add("Max. overshoot (最大过冲)");
                myLS.Add("Avg. overshoot (平均过冲)");
                myLS.Add("Cool time      (降温时间)");
                myLS.Add("Hold time      (恒温时间)");
            }
            myMax = GetJoinLen(myLS, LENL, LENS);
            idx = 0;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Measured (测量值)";
            if (slope_get > 0)
            {
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (slope_get / 100.0f).ToString("F2") + " ℃/sec";
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (((slope_tmp * 100000 / MyDefine.myXET.homstep) / (slope_stop - slope_start)) / 100.0f).ToString("F2") + " ℃/sec";
            }
            if (rise)
            {
                if (over_max > (myPicture.myTP[step_target].temperature))
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + MyDefine.myXET.GetTemp(over_max) + " ℃";
                    myLS[idx] = myLS[idx] + " (+" + MyDefine.myXET.GetTemp(over_max - (myPicture.myTP[step_target].temperature)) + "℃)";
                }
                else
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                }
            }
            else
            {
                if (over_max < (myPicture.myTP[step_target].temperature))
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + MyDefine.myXET.GetTemp(over_max) + " ℃";
                    myLS[idx] = myLS[idx] + " (" + MyDefine.myXET.GetTemp(over_max - (myPicture.myTP[step_target].temperature)) + "℃)";
                }
                else
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                }
            }
            if (rise)
            {
                if (over_avg > (myPicture.myTP[step_target].temperature))
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + MyDefine.myXET.GetTemp(over_avg) + " ℃";
                    myLS[idx] = myLS[idx] + " (+" + MyDefine.myXET.GetTemp(over_avg - (myPicture.myTP[step_target].temperature)) + "℃)";
                }
                else
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                }
            }
            else
            {
                if (over_avg < (myPicture.myTP[step_target].temperature))
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + MyDefine.myXET.GetTemp(over_avg) + " ℃";
                    myLS[idx] = myLS[idx] + " (" + MyDefine.myXET.GetTemp(over_avg - (myPicture.myTP[step_target].temperature)) + "℃)";
                }
                else
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                }
            }
            idx++;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((const_begin - begin) * MyDefine.myXET.homstep / 1000).ToString("f1") + " sec";
            idx++;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((const_end - const_begin) * MyDefine.myXET.homstep / 1000).ToString("f1") + " sec";
            myMax = GetJoinLen(myLS, LENL, LENS);
            idx = 0;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Specification (参考)";
            if (slope_get > 0)
            {
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_rate / 100.0f).ToString("F1") + " ℃/sec";
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_rate / 100.0f).ToString("F1") + " ℃/sec";
            }
            idx++;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_over / 100.0f).ToString("F1") + " ℃";
            idx++;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_over / 100.0f).ToString("F1") + " ℃";
            myMax = GetJoinLen(myLS, LENL, LENW);
            idx = 0;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Result";
            if (slope_get > 0)
            {
                //
                myTmp = slope_get;
                if (myTmp >= (spec_rate * 2))
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                }
                else if (myTmp >= spec_rate)
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                }
                else
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                }

                //
                myTmp = (int)((slope_tmp * 100000 / MyDefine.myXET.homstep) / (slope_stop - slope_start));
                if (myTmp >= (spec_rate * 2))
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                }
                else if (myTmp >= spec_rate)
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                }
                else
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                }
            }
            if (rise)
            {
                if (over_max > (myPicture.myTP[step_target].temperature))
                {
                    myTmp = myPicture.Devation(over_max, (myPicture.myTP[step_target].temperature));

                    if (myTmp < (spec_over / 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_over)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }
                }
                else
                {
                    idx++;
                }
            }
            else
            {
                if (over_max < (myPicture.myTP[step_target].temperature))
                {
                    myTmp = myPicture.Devation(over_max, (myPicture.myTP[step_target].temperature));

                    if (myTmp < (spec_over / 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_over)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }
                }
                else
                {
                    idx++;
                }
            }

            if (rise)
            {
                if (over_avg > (myPicture.myTP[step_target].temperature))
                {
                    myTmp = myPicture.Devation(over_avg, (myPicture.myTP[step_target].temperature));

                    if (myTmp < (spec_over / 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_over)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }
                }
                else
                {
                    idx++;
                }
            }
            else
            {
                if (over_avg < (myPicture.myTP[step_target].temperature))
                {
                    myTmp = myPicture.Devation(over_avg, (myPicture.myTP[step_target].temperature));

                    if (myTmp < (spec_over / 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_over)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }
                }
                else
                {
                    idx++;
                }
            }
            document.Add(new Paragraph(myLS[0], fontContent));
            for (int i = 1; i < myLS.Count; i++)
            {
                document.Add(new Paragraph(myLS[i], fontContent));
            }
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Accuracy results        (" + MyDefine.myXET.myHom[idx_first].time.ToString("HH:mm:ss") + "-" + MyDefine.myXET.myHom[idx_fifth].time.ToString("HH:mm:ss") + ", " + mmStr + "℃温度控准确度)", fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Time (时间)".PadRight((LENL - 2), ' '));
            myLS.Add(("0 sec").PadRight(LENL, ' '));
            myLS.Add((space.ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS.Add(((space * 2).ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS.Add(((space * 3).ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS.Add(((space * 4).ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS[0] = myLS[0] + "Measured (测量值)";
            myLS[1] = myLS[1] + MyDefine.myXET.GetTemp(myRPT[idx_first].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_first].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
            myLS[2] = myLS[2] + MyDefine.myXET.GetTemp(myRPT[idx_second].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_second].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
            myLS[3] = myLS[3] + MyDefine.myXET.GetTemp(myRPT[idx_third].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_third].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
            myLS[4] = myLS[4] + MyDefine.myXET.GetTemp(myRPT[idx_fourth].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_fourth].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
            myLS[5] = myLS[5] + MyDefine.myXET.GetTemp(myRPT[idx_fifth].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_fifth].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Specification (参考)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
            myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
            myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
            myMax = GetJoinLen(myLS, LENL, LENW);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Result";
            myTmp = myPicture.Devation(myRPT[idx_first].outAvg, (myPicture.myTP[step_target].temperature));
            if (myTmp < (spec_dev / 2))
            {
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√";
            }
            else
            {
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "X";
            }
            myTmp = myPicture.Devation(myRPT[idx_second].outAvg, (myPicture.myTP[step_target].temperature));
            if (myTmp < (spec_dev / 2))
            {
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√";
            }
            else
            {
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "X";
            }
            myTmp = myPicture.Devation(myRPT[idx_third].outAvg, (myPicture.myTP[step_target].temperature));
            if (myTmp < (spec_dev / 2))
            {
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√";
            }
            else
            {
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "X";
            }
            myTmp = myPicture.Devation(myRPT[idx_fourth].outAvg, (myPicture.myTP[step_target].temperature));
            if (myTmp < (spec_dev / 2))
            {
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√";
            }
            else
            {
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "X";
            }
            myTmp = myPicture.Devation(myRPT[idx_fifth].outAvg, (myPicture.myTP[step_target].temperature));
            if (myTmp < (spec_dev / 2))
            {
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√";
            }
            else
            {
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "X";
            }
            document.Add(new Paragraph(myLS[0], fontContent));
            for (int i = 1; i < myLS.Count; i++)
            {
                document.Add(new Paragraph(myLS[i], fontContent));
            }
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Accuracy results        (" + MyDefine.myXET.myHom[idx_first].time.ToString("HH:mm:ss") + "-" + MyDefine.myXET.myHom[idx_fifth].time.ToString("HH:mm:ss") + ", " + mmStr + "℃温度控Block均匀性)", fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Time (时间)".PadRight((LENL - 2), ' '));
            myLS.Add(("0 sec").PadRight(LENL, ' '));
            myLS.Add((space.ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS.Add(((space * 2).ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS.Add(((space * 3).ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS.Add(((space * 4).ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS[0] = myLS[0] + "Measured (测量值)";
            int[] OutFirstTVS8 = new int[8];
            Array.Copy(myRPT[idx_first].OUT, OutFirstTVS8, 8);
            int[] OutSecondTVS8 = new int[8];
            Array.Copy(myRPT[idx_second].OUT, OutSecondTVS8, 8);
            int[] OutThirdTVS8 = new int[8];
            Array.Copy(myRPT[idx_third].OUT, OutThirdTVS8, 8);
            int[] OutFourthTVS8 = new int[8];
            Array.Copy(myRPT[idx_fourth].OUT, OutFourthTVS8, 8);
            int[] OutFifthTVS8 = new int[8];
            Array.Copy(myRPT[idx_fifth].OUT, OutFifthTVS8, 8);
            myLS[1] = myLS[1] + MyDefine.myXET.GetTemp(OutFirstTVS8.Max() - OutFirstTVS8.Min()) + " ℃";
            myLS[2] = myLS[2] + MyDefine.myXET.GetTemp(OutSecondTVS8.Max() - OutSecondTVS8.Min()) + " ℃";
            myLS[3] = myLS[3] + MyDefine.myXET.GetTemp(OutThirdTVS8.Max() - OutThirdTVS8.Min()) + " ℃";
            myLS[4] = myLS[4] + MyDefine.myXET.GetTemp(OutFourthTVS8.Max() - OutFourthTVS8.Min()) + " ℃";
            myLS[5] = myLS[5] + MyDefine.myXET.GetTemp(OutFifthTVS8.Max() - OutFifthTVS8.Min()) + " ℃";
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Specification (参考)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
            myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
            myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
            myMax = GetJoinLen(myLS, LENL, LENW);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Result";
            //zhoup
            myTmp = OutFirstTVS8.Max() - OutFirstTVS8.Min();
            if (myTmp < (spec_max / 2))
            {
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_max)
            {
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√";
            }
            else
            {
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "X";
            }
            myTmp = OutSecondTVS8.Max() - OutSecondTVS8.Min();
            if (myTmp < (spec_max / 2))
            {
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_max)
            {
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√";
            }
            else
            {
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "X";
            }
            myTmp = OutThirdTVS8.Max() - OutThirdTVS8.Min();
            if (myTmp < (spec_max / 2))
            {
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_max)
            {
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√";
            }
            else
            {
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "X";
            }
            myTmp = OutFourthTVS8.Max() - OutFourthTVS8.Min();
            if (myTmp < (spec_max / 2))
            {
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_max)
            {
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√";
            }
            else
            {
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "X";
            }
            myTmp = OutFifthTVS8.Max() - OutFifthTVS8.Min();
            if (myTmp < (spec_max / 2))
            {
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_max)
            {
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√";
            }
            else
            {
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "X";
            }
            document.Add(new Paragraph(myLS[0], fontContent));
            for (int i = 1; i < myLS.Count; i++)
            {
                document.Add(new Paragraph(myLS[i], fontContent));
            }
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////

            #endregion

            step_start = 0;
            step_target = 1;
            step_next = 2;

            for (int step = 0; step < (myPicture.myTP.Count - 2); step++)
            {
                #region 第n页计算

                //时间更新
                rise = myPicture.myTP[step_target].rise;
                begin = myPicture.myTP[step_target].begin;
                end = myPicture.myTP[step_target].end;
                const_begin = myPicture.myTP[step_target].const_begin;
                const_end = myPicture.myTP[step_target].const_end;
                space = myPicture.myTP[step_target].space;
                idx_30sec = myPicture.myTP[step_target].idx_30sec;
                idx_first = myPicture.myTP[step_target].idx_first;
                idx_second = myPicture.myTP[step_target].idx_second;
                idx_third = myPicture.myTP[step_target].idx_third;
                idx_fourth = myPicture.myTP[step_target].idx_fourth;
                idx_fifth = myPicture.myTP[step_target].idx_fifth;

                //高温
                if (myPicture.myTP[step_target].temperature > 9000)
                {
                    spec_dev = 80;    //0.8℃
                    spec_rate = 150;  //1.5℃
                    spec_over = 600;  //6.0℃
                    spec_max = 150;   //1.5℃
                }
                else if (myPicture.myTP[step_target].temperature > 7000)
                {
                    spec_dev = 60;
                    spec_rate = 150;
                    spec_over = 500;
                    spec_max = 150;
                }
                else
                {
                    spec_dev = 50;
                    spec_rate = 150;
                    spec_over = 500;
                    spec_max = 100;
                }

                //速率
                if ((myPicture.myTP[step_start].temperature + 1500) < myPicture.myTP[step_target].temperature)
                {
                    //升温
                    slope_start = myPicture.GetUpThrough(begin, end, (myPicture.myTP[step_start].temperature + 500));
                    slope_stop = myPicture.GetUpThrough(begin, end, (myPicture.myTP[step_target].temperature - 500));
                    slope_tmp = (myPicture.myTP[step_target].temperature - myPicture.myTP[step_start].temperature - 1000) / 100f;
                    slope_get = myPicture.GetHeatRate(slope_start, slope_stop);
                }
                else if (myPicture.myTP[step_start].temperature > (myPicture.myTP[step_target].temperature + 1500))
                {
                    //降温
                    slope_start = myPicture.GetDownThrough(begin, end, (myPicture.myTP[step_start].temperature - 500));
                    slope_stop = myPicture.GetDownThrough(begin, end, (myPicture.myTP[step_target].temperature + 500));
                    slope_tmp = (myPicture.myTP[step_start].temperature - myPicture.myTP[step_target].temperature - 1000) / 100f;
                    slope_get = myPicture.GetCoolRate(slope_start, slope_stop);
                }
                else
                {
                    slope_start = 0;
                    slope_stop = 0;
                    slope_tmp = 0;
                    slope_get = 0;
                }

                //过冲
                over_max = myPicture.GetOverMax(begin, const_end, rise);
                over_avg = myPicture.GetOverAvg(begin, const_end, rise);

                #endregion

                #region 第n点曲线图

                //更新页数和创建新页
                document.NewPage();
                document.ResetPageCount();
                //第n页添加元素
                document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
                document.Add(new Paragraph(blankLine, fontItem));
                //第n页添加信息
                mmStr = (myPicture.myTP[step_target].temperature / 100f).ToString();
                if (mmStr.Length <= 2)
                {
                    mmStr += ".00";
                }
                else if (mmStr.Length < 5)
                {
                    mmStr += "0";
                }
                document.Add(new Paragraph("Probe temperature curve :  " + mmStr + " ℃        (" + mmStr + "℃时温度曲线)", fontItem));
                document.Add(new Paragraph(blankLine, fontItem));
                //第n页添加曲线图
                document.Add(iTextSharp.text.Image.GetInstance(GetBitmapPoint(begin, end, myPicture.myTP[step_target].temperature), System.Drawing.Imaging.ImageFormat.Bmp));

                #endregion

                #region 第n页

                //更新页数和创建新页
                document.NewPage();
                document.ResetPageCount();

                //第n页添加元素
                document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                mmStr = (myPicture.myTP[step_target].temperature / 100f).ToString();
                document.Add(new Paragraph("Temperature        :  " + mmStr + ".00 ℃        (" + mmStr + "℃时的温度表现 " + MyDefine.myXET.myHom[begin].time.ToString("HH:mm:ss") + "-" + MyDefine.myXET.myHom[end].time.ToString("HH:mm:ss") + ")", fontContent));
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                document.Add(new Paragraph("Values result           (" + MyDefine.myXET.myHom[idx_30sec].time.ToString("HH:mm:ss") + ", " + mmStr + "℃ 8孔的温度数据)", fontItem));
                ////////////////////////////////////////////////////////////////////
                myLS.Clear();
                myLS.Add("".PadRight(LENL, ' '));
                myLS.Add("Probe 1 (A1)".PadRight(LENL, ' '));
                myLS.Add("Probe 2 (A4)".PadRight(LENL, ' '));
                myLS.Add("Probe 3 (A7)".PadRight(LENL, ' '));
                myLS.Add("Probe 4 (A10)".PadRight(LENL, ' '));
                myLS.Add("Probe 5 (A12)".PadRight(LENL, ' '));
                myLS.Add("Probe 6 (D1)".PadRight(LENL, ' '));
                myLS.Add("Probe 7 (D7)".PadRight(LENL, ' '));
                myLS.Add("Probe 8 (D12)".PadRight(LENL, ' '));
                myLS[0] = myLS[0] + "Measured";
                for (int i = 0; i < SZ.CHA - 7; i++)
                {
                    myLS[i + 1] = myLS[i + 1] + MyDefine.myXET.GetTemp(myRPT[idx_30sec].OUT[i]) + " ℃";
                }
                myMax = GetJoinLen(myLS, LENL, LENS);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "T(meas-set)";
                temp_max = myRPT[idx_30sec].OUT[0];
                temp_min = myRPT[idx_30sec].OUT[0];
                mark_max = 1;
                mark_min = 1;
                for (int i = 0; i < SZ.CHA - 7; i++)
                {
                    myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + MyDefine.myXET.GetTemp(myRPT[idx_30sec].OUT[i] - (myPicture.myTP[step_target].temperature)) + " ℃";
                    if (myRPT[idx_30sec].OUT[i] > temp_max)
                    {
                        temp_max = myRPT[idx_30sec].OUT[i];
                        mark_max = i + 1;
                    }
                    if (myRPT[idx_30sec].OUT[i] < temp_min)
                    {
                        temp_min = myRPT[idx_30sec].OUT[i];
                        mark_min = i + 1;
                    }
                }
                myLS[mark_max] = myLS[mark_max] + " (max)";
                myLS[mark_min] = myLS[mark_min] + " (min)";
                myMax = GetJoinLen(myLS, LENL, LENS);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Status";
                for (int i = 0; i < SZ.CHA - 7; i++)
                {
                    myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + "Active";
                }
                document.Add(new Paragraph(myLS[0], fontContent));
                for (int i = 0; i < SZ.CHA - 7; i++)
                {
                    document.Add(new Paragraph(myLS[i + 1], fontContent));
                }
                myLS.Clear();
                myLS.Add("Probe Max. temperature  (最大值)   :".PadRight(LENL, ' '));
                myLS.Add("Probe Min. temperature  (最小值)   :".PadRight(LENL, ' '));
                myLS.Add("Probe Avg. temperature  (平均值)   :".PadRight(LENL, ' '));
                myLS.Add("Probe nonuniformity     (均匀度)   :".PadRight(LENL, ' '));
                myLS.Add("Probe error on measured (示值误差) :".PadRight(LENL, ' '));
                myMax = GetJoinLen(myLS, LENL, LENW);
                Array.Clear(OutTVS8, 0, 8);
                Array.Copy(myRPT[idx_30sec].OUT, OutTVS8, 8);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + MyDefine.myXET.GetTemp(OutTVS8.Max()) + " ℃";
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + MyDefine.myXET.GetTemp(OutTVS8.Min()) + " ℃";
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + MyDefine.myXET.GetTemp(myRPT[idx_30sec].outAvg) + " ℃";
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + MyDefine.myXET.GetTemp(OutTVS8.Max() - OutTVS8.Min()) + " ℃";
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + MyDefine.myXET.GetTempWithSign((myPicture.myTP[step_target].temperature) - myRPT[idx_30sec].outAvg) + " ℃ (" + MyDefine.myXET.GetTemp(myPicture.myTP[step_target].temperature) + " - " + MyDefine.myXET.GetTemp(myRPT[idx_30sec].outAvg) + ")";
                document.Add(new Paragraph(myLS[0], fontContent));
                document.Add(new Paragraph(myLS[1], fontContent));
                document.Add(new Paragraph(myLS[2], fontContent));
                document.Add(new Paragraph(myLS[3], fontContent));
                document.Add(new Paragraph(myLS[4], fontContent));
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                document.Add(new Paragraph("Step results            (" + mmStr + "℃温度控制性能)", fontItem));
                ////////////////////////////////////////////////////////////////////
                myLS.Clear();
                myLS.Add("Item (项目)");
                if (rise)
                {
                    if (slope_get > 0)
                    {
                        myLS.Add("Max. heat rate (最大升温速率)");
                        myLS.Add("Avg. heat rate (平均升温速率)");
                    }
                    myLS.Add("Max. overshoot (最大过冲)");
                    myLS.Add("Avg. overshoot (平均过冲)");
                    myLS.Add("Heat time      (升温时间)");
                    myLS.Add("Hold time      (恒温时间)");
                }
                else
                {
                    if (slope_get > 0)
                    {
                        myLS.Add("Max. cool rate (最大降温速率)");
                        myLS.Add("Avg. cool rate (平均降温速率)");
                    }
                    myLS.Add("Max. overshoot (最大过冲)");
                    myLS.Add("Avg. overshoot (平均过冲)");
                    myLS.Add("Cool time      (降温时间)");
                    myLS.Add("Hold time      (恒温时间)");
                }
                myMax = GetJoinLen(myLS, LENL, LENS);
                idx = 0;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Measured (测量值)";
                if (slope_get > 0)
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (slope_get / 100.0f).ToString("F2") + " ℃/sec";
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (((slope_tmp * 100000 / MyDefine.myXET.homstep) / (slope_stop - slope_start)) / 100.0f).ToString("F2") + " ℃/sec";
                }
                if (rise)
                {
                    if (over_max > (myPicture.myTP[step_target].temperature))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + MyDefine.myXET.GetTemp(over_max) + " ℃";
                        myLS[idx] = myLS[idx] + " (+" + MyDefine.myXET.GetTemp(over_max - (myPicture.myTP[step_target].temperature)) + "℃)";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                    }
                }
                else
                {
                    if (over_max < (myPicture.myTP[step_target].temperature))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + MyDefine.myXET.GetTemp(over_max) + " ℃";
                        myLS[idx] = myLS[idx] + " (" + MyDefine.myXET.GetTemp(over_max - (myPicture.myTP[step_target].temperature)) + "℃)";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                    }
                }
                if (rise)
                {
                    if (over_avg > (myPicture.myTP[step_target].temperature))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + MyDefine.myXET.GetTemp(over_avg) + " ℃";
                        myLS[idx] = myLS[idx] + " (+" + MyDefine.myXET.GetTemp(over_avg - (myPicture.myTP[step_target].temperature)) + "℃)";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                    }
                }
                else
                {
                    if (over_avg < (myPicture.myTP[step_target].temperature))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + MyDefine.myXET.GetTemp(over_avg) + " ℃";
                        myLS[idx] = myLS[idx] + " (" + MyDefine.myXET.GetTemp(over_avg - (myPicture.myTP[step_target].temperature)) + "℃)";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                    }
                }
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((const_begin - begin) * MyDefine.myXET.homstep / 1000).ToString("f1") + " sec";
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((const_end - const_begin) * MyDefine.myXET.homstep / 1000).ToString("f1") + " sec";
                myMax = GetJoinLen(myLS, LENL, LENS);
                idx = 0;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Specification (参考)";
                if (slope_get > 0)
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_rate / 100.0f).ToString("F1") + " ℃/sec";
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_rate / 100.0f).ToString("F1") + " ℃/sec";
                }
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_over / 100.0f).ToString("F1") + " ℃";
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_over / 100.0f).ToString("F1") + " ℃";
                myMax = GetJoinLen(myLS, LENL, LENW);
                idx = 0;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Result";
                if (slope_get > 0)
                {
                    //
                    myTmp = slope_get;
                    if (myTmp >= (spec_rate * 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp >= spec_rate)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }

                    //
                    myTmp = (int)((slope_tmp * 100000 / MyDefine.myXET.homstep) / (slope_stop - slope_start));
                    if (myTmp >= (spec_rate * 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp >= spec_rate)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }
                }
                if (rise)
                {
                    if (over_max > (myPicture.myTP[step_target].temperature))
                    {
                        myTmp = myPicture.Devation(over_max, (myPicture.myTP[step_target].temperature));

                        if (myTmp < (spec_over / 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp < spec_over)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
                else
                {
                    if (over_max < (myPicture.myTP[step_target].temperature))
                    {
                        myTmp = myPicture.Devation(over_max, (myPicture.myTP[step_target].temperature));

                        if (myTmp < (spec_over / 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp < spec_over)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
                if (rise)
                {
                    if (over_avg > (myPicture.myTP[step_target].temperature))
                    {
                        myTmp = myPicture.Devation(over_avg, (myPicture.myTP[step_target].temperature));

                        if (myTmp < (spec_over / 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp < spec_over)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
                else
                {
                    if (over_avg < (myPicture.myTP[step_target].temperature))
                    {
                        myTmp = myPicture.Devation(over_avg, (myPicture.myTP[step_target].temperature));

                        if (myTmp < (spec_over / 2))
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                        }
                        else if (myTmp < spec_over)
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                        }
                        else
                        {
                            idx++;
                            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                        }
                    }
                    else
                    {
                        idx++;
                    }
                }
                document.Add(new Paragraph(myLS[0], fontContent));
                for (int i = 1; i < myLS.Count; i++)
                {
                    document.Add(new Paragraph(myLS[i], fontContent));
                }
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                document.Add(new Paragraph("Accuracy results        (" + MyDefine.myXET.myHom[idx_first].time.ToString("HH:mm:ss") + "-" + MyDefine.myXET.myHom[idx_fifth].time.ToString("HH:mm:ss") + ", " + mmStr + "℃温度控准确度)", fontItem));
                ////////////////////////////////////////////////////////////////////
                myLS.Clear();
                myLS.Add("Time (时间)".PadRight((LENL - 2), ' '));
                myLS.Add(("0 sec").PadRight(LENL, ' '));
                myLS.Add((space.ToString("f1") + " sec").PadRight(LENL, ' '));
                myLS.Add(((space * 2).ToString("f1") + " sec").PadRight(LENL, ' '));
                myLS.Add(((space * 3).ToString("f1") + " sec").PadRight(LENL, ' '));
                myLS.Add(((space * 4).ToString("f1") + " sec").PadRight(LENL, ' '));
                myLS[0] = myLS[0] + "Measured (测量值)";
                myLS[1] = myLS[1] + MyDefine.myXET.GetTemp(myRPT[idx_first].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_first].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
                myLS[2] = myLS[2] + MyDefine.myXET.GetTemp(myRPT[idx_second].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_second].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
                myLS[3] = myLS[3] + MyDefine.myXET.GetTemp(myRPT[idx_third].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_third].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
                myLS[4] = myLS[4] + MyDefine.myXET.GetTemp(myRPT[idx_fourth].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_fourth].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
                myLS[5] = myLS[5] + MyDefine.myXET.GetTemp(myRPT[idx_fifth].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_fifth].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
                myMax = GetJoinLen(myLS, LENL, LENS);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Specification (参考)";
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
                myMax = GetJoinLen(myLS, LENL, LENW);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Result";
                myTmp = myPicture.Devation(myRPT[idx_first].outAvg, (myPicture.myTP[step_target].temperature));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√";
                }
                else
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "X";
                }
                myTmp = myPicture.Devation(myRPT[idx_second].outAvg, (myPicture.myTP[step_target].temperature));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√";
                }
                else
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "X";
                }
                myTmp = myPicture.Devation(myRPT[idx_third].outAvg, (myPicture.myTP[step_target].temperature));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√";
                }
                else
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "X";
                }
                myTmp = myPicture.Devation(myRPT[idx_fourth].outAvg, (myPicture.myTP[step_target].temperature));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√";
                }
                else
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "X";
                }
                myTmp = myPicture.Devation(myRPT[idx_fifth].outAvg, (myPicture.myTP[step_target].temperature));
                if (myTmp < (spec_dev / 2))
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√";
                }
                else
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "X";
                }
                document.Add(new Paragraph(myLS[0], fontContent));
                for (int i = 1; i < myLS.Count; i++)
                {
                    document.Add(new Paragraph(myLS[i], fontContent));
                }
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////
                document.Add(new Paragraph("Accuracy results        (" + MyDefine.myXET.myHom[idx_first].time.ToString("HH:mm:ss") + "-" + MyDefine.myXET.myHom[idx_fifth].time.ToString("HH:mm:ss") + ", " + mmStr + "℃温度控Block均匀性)", fontItem));
                ////////////////////////////////////////////////////////////////////
                myLS.Clear();
                myLS.Add("Time (时间)".PadRight((LENL - 2), ' '));
                myLS.Add(("0 sec").PadRight(LENL, ' '));
                myLS.Add((space.ToString("f1") + " sec").PadRight(LENL, ' '));
                myLS.Add(((space * 2).ToString("f1") + " sec").PadRight(LENL, ' '));
                myLS.Add(((space * 3).ToString("f1") + " sec").PadRight(LENL, ' '));
                myLS.Add(((space * 4).ToString("f1") + " sec").PadRight(LENL, ' '));
                myLS[0] = myLS[0] + "Measured (测量值)";
                Array.Clear(OutFirstTVS8, 0, 8);
                Array.Copy(myRPT[idx_first].OUT, OutFirstTVS8, 8);
                Array.Clear(OutSecondTVS8, 0, 8);
                Array.Copy(myRPT[idx_second].OUT, OutSecondTVS8, 8);
                Array.Clear(OutThirdTVS8, 0, 8);
                Array.Copy(myRPT[idx_third].OUT, OutThirdTVS8, 8);
                Array.Clear(OutFourthTVS8, 0, 8);
                Array.Copy(myRPT[idx_fourth].OUT, OutFourthTVS8, 8);
                Array.Clear(OutFifthTVS8, 0, 8);
                Array.Copy(myRPT[idx_fifth].OUT, OutFifthTVS8, 8);
                myLS[1] = myLS[1] + MyDefine.myXET.GetTemp(OutFirstTVS8.Max() - OutFirstTVS8.Min()) + " ℃";
                myLS[2] = myLS[2] + MyDefine.myXET.GetTemp(OutSecondTVS8.Max() - OutSecondTVS8.Min()) + " ℃";
                myLS[3] = myLS[3] + MyDefine.myXET.GetTemp(OutThirdTVS8.Max() - OutThirdTVS8.Min()) + " ℃";
                myLS[4] = myLS[4] + MyDefine.myXET.GetTemp(OutFourthTVS8.Max() - OutFourthTVS8.Min()) + " ℃";
                myLS[5] = myLS[5] + MyDefine.myXET.GetTemp(OutFifthTVS8.Max() - OutFifthTVS8.Min()) + " ℃";
                myMax = GetJoinLen(myLS, LENL, LENS);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Specification (参考)";
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
                myMax = GetJoinLen(myLS, LENL, LENW);
                myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Result";
                //zhoup
                myTmp = OutFirstTVS8.Max() - OutFirstTVS8.Min();
                if (myTmp < (spec_max / 2))
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_max)
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√";
                }
                else
                {
                    myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "X";
                }
                myTmp = OutSecondTVS8.Max() - OutSecondTVS8.Min();
                if (myTmp < (spec_dev / 2))
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√";
                }
                else
                {
                    myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "X";
                }
                myTmp = OutThirdTVS8.Max() - OutThirdTVS8.Min();
                if (myTmp < (spec_dev / 2))
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√";
                }
                else
                {
                    myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "X";
                }
                myTmp = OutFourthTVS8.Max() - OutFourthTVS8.Min();
                if (myTmp < (spec_dev / 2))
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√";
                }
                else
                {
                    myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "X";
                }
                myTmp = OutFifthTVS8.Max() - OutFifthTVS8.Min();
                if (myTmp < (spec_dev / 2))
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√√";
                }
                else if (myTmp < spec_dev)
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√";
                }
                else
                {
                    myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "X";
                }
                document.Add(new Paragraph(myLS[0], fontContent));
                for (int i = 1; i < myLS.Count; i++)
                {
                    document.Add(new Paragraph(myLS[i], fontContent));
                }
                document.Add(new Paragraph(blankLine, fontItem));
                ////////////////////////////////////////////////////////////////////

                #endregion

                step_start++;
                step_target++;
                step_next++;
            }

            #region 最后一点计算

            //时间更新
            rise = myPicture.myTP[step_target].rise;
            begin = myPicture.myTP[step_target].begin;
            end = myPicture.myTP[step_target].end;
            const_begin = myPicture.myTP[step_target].const_begin;
            const_end = myPicture.myTP[step_target].const_end;
            space = myPicture.myTP[step_target].space;
            idx_30sec = myPicture.myTP[step_target].idx_30sec;
            idx_first = myPicture.myTP[step_target].idx_first;
            idx_second = myPicture.myTP[step_target].idx_second;
            idx_third = myPicture.myTP[step_target].idx_third;
            idx_fourth = myPicture.myTP[step_target].idx_fourth;
            idx_fifth = myPicture.myTP[step_target].idx_fifth;

            //高温
            if (myPicture.myTP[step_target].temperature > 9000)
            {
                spec_dev = 80;    //0.8℃
                spec_rate = 150;  //1.5℃
                spec_over = 600;  //6.0℃
                spec_max = 150;   //1.5℃
            }
            else if (myPicture.myTP[step_target].temperature > 7000)
            {
                spec_dev = 60;
                spec_rate = 150;
                spec_over = 500;
                spec_max = 150;
            }
            else
            {
                spec_dev = 50;
                spec_rate = 150;
                spec_over = 500;
                spec_max = 100;
            }

            //速率
            if ((myPicture.myTP[step_start].temperature + 1500) < myPicture.myTP[step_target].temperature)
            {
                //升温
                slope_start = myPicture.GetUpThrough(begin, end, (myPicture.myTP[step_start].temperature + 500));
                slope_stop = myPicture.GetUpThrough(begin, end, (myPicture.myTP[step_target].temperature - 500));
                slope_tmp = (myPicture.myTP[step_target].temperature - myPicture.myTP[step_start].temperature - 1000) / 100f;
                slope_get = myPicture.GetHeatRate(slope_start, slope_stop);
            }
            else if (myPicture.myTP[step_start].temperature > (myPicture.myTP[step_target].temperature + 1500))
            {
                //降温
                slope_start = myPicture.GetDownThrough(begin, end, (myPicture.myTP[step_start].temperature - 500));
                slope_stop = myPicture.GetDownThrough(begin, end, (myPicture.myTP[step_target].temperature + 500));
                slope_tmp = (myPicture.myTP[step_start].temperature - myPicture.myTP[step_target].temperature - 1000) / 100f;
                slope_get = myPicture.GetCoolRate(slope_start, slope_stop);
            }
            else
            {
                slope_start = 0;
                slope_stop = 0;
                slope_tmp = 0;
                slope_get = 0;
            }

            //过冲
            over_max = myPicture.GetOverMax(begin, const_end, rise);
            over_avg = myPicture.GetOverAvg(begin, const_end, rise);

            #endregion

            #region 最后点曲线图

            //更新页数和创建新页
            document.NewPage();
            document.ResetPageCount();
            //最后页添加元素
            document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
            document.Add(new Paragraph(blankLine, fontItem));
            //最后页添加信息
            mmStr = (myPicture.myTP[step_target].temperature / 100f).ToString();
            if (mmStr.Length <= 2)
            {
                mmStr += ".00";
            }
            else if (mmStr.Length < 5)
            {
                mmStr += "0";
            }
            document.Add(new Paragraph("Probe temperature curve :  " + mmStr + " ℃        (" + mmStr + "℃时温度曲线)", fontItem));
            document.Add(new Paragraph(blankLine, fontItem));
            //最后页添加曲线图
            document.Add(iTextSharp.text.Image.GetInstance(GetBitmapPoint(begin, end, myPicture.myTP[step_target].temperature), System.Drawing.Imaging.ImageFormat.Bmp));

            #endregion

            #region 最后一点报告

            //更新页数和创建新页
            document.NewPage();
            document.ResetPageCount();

            //第n页添加元素
            document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            mmStr = (myPicture.myTP[step_target].temperature / 100f).ToString();
            document.Add(new Paragraph("Temperature        :  " + mmStr + ".00 ℃        (" + mmStr + "℃时的温度表现 " + MyDefine.myXET.myHom[begin].time.ToString("HH:mm:ss") + "-" + MyDefine.myXET.myHom[end].time.ToString("HH:mm:ss") + ")", fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Values result           (" + MyDefine.myXET.myHom[idx_30sec].time.ToString("HH:mm:ss") + ", " + mmStr + "℃ 8孔的温度数据)", fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("".PadRight(LENL, ' '));
            myLS.Add("Probe 1 (A1)".PadRight(LENL, ' '));
            myLS.Add("Probe 2 (A4)".PadRight(LENL, ' '));
            myLS.Add("Probe 3 (A7)".PadRight(LENL, ' '));
            myLS.Add("Probe 4 (A10)".PadRight(LENL, ' '));
            myLS.Add("Probe 5 (A12)".PadRight(LENL, ' '));
            myLS.Add("Probe 6 (D1)".PadRight(LENL, ' '));
            myLS.Add("Probe 7 (D7)".PadRight(LENL, ' '));
            myLS.Add("Probe 8 (D12)".PadRight(LENL, ' '));
            myLS[0] = myLS[0] + "Measured";
            for (int i = 0; i < SZ.CHA - 7; i++)
            {
                myLS[i + 1] = myLS[i + 1] + MyDefine.myXET.GetTemp(myRPT[idx_30sec].OUT[i]) + " ℃";
            }
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "T(meas-set)";
            temp_max = myRPT[idx_30sec].OUT[0];
            temp_min = myRPT[idx_30sec].OUT[0];
            mark_max = 1;
            mark_min = 1;
            for (int i = 0; i < SZ.CHA - 7; i++)
            {
                myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + MyDefine.myXET.GetTemp(myRPT[idx_30sec].OUT[i] - (myPicture.myTP[step_target].temperature)) + " ℃";
                if (myRPT[idx_30sec].OUT[i] > temp_max)
                {
                    temp_max = myRPT[idx_30sec].OUT[i];
                    mark_max = i + 1;
                }
                if (myRPT[idx_30sec].OUT[i] < temp_min)
                {
                    temp_min = myRPT[idx_30sec].OUT[i];
                    mark_min = i + 1;
                }
            }
            myLS[mark_max] = myLS[mark_max] + " (max)";
            myLS[mark_min] = myLS[mark_min] + " (min)";
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Status";
            for (int i = 0; i < SZ.CHA - 7; i++)
            {
                myLS[i + 1] = myLS[i + 1].PadRight(myMax + myLS[i + 1].Length - Encoding.Default.GetBytes(myLS[i + 1]).Length, ' ') + "Active";
            }
            document.Add(new Paragraph(myLS[0], fontContent));
            for (int i = 0; i < SZ.CHA - 7; i++)
            {
                document.Add(new Paragraph(myLS[i + 1], fontContent));
            }
            myLS.Clear();
            myLS.Add("Probe Max. temperature  (最大值)   :".PadRight(LENL, ' '));
            myLS.Add("Probe Min. temperature  (最小值)   :".PadRight(LENL, ' '));
            myLS.Add("Probe Avg. temperature  (平均值)   :".PadRight(LENL, ' '));
            myLS.Add("Probe nonuniformity     (均匀度)   :".PadRight(LENL, ' '));
            myLS.Add("Probe error on measured (示值误差) :".PadRight(LENL, ' '));
            myMax = GetJoinLen(myLS, LENL, LENW);
            Array.Clear(OutTVS8, 0, 8);
            Array.Copy(myRPT[idx_30sec].OUT, OutTVS8, 8);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + MyDefine.myXET.GetTemp(OutTVS8.Max()) + " ℃";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + MyDefine.myXET.GetTemp(OutTVS8.Min()) + " ℃";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + MyDefine.myXET.GetTemp(myRPT[idx_30sec].outAvg) + " ℃";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + MyDefine.myXET.GetTemp(OutTVS8.Max() - OutTVS8.Min()) + " ℃";
            myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + MyDefine.myXET.GetTempWithSign((myPicture.myTP[step_target].temperature) - myRPT[idx_30sec].outAvg) + " ℃ (" + MyDefine.myXET.GetTemp(myPicture.myTP[step_target].temperature) + " - " + MyDefine.myXET.GetTemp(myRPT[idx_30sec].outAvg) + ")";
            document.Add(new Paragraph(myLS[0], fontContent));
            document.Add(new Paragraph(myLS[1], fontContent));
            document.Add(new Paragraph(myLS[2], fontContent));
            document.Add(new Paragraph(myLS[3], fontContent));
            document.Add(new Paragraph(myLS[4], fontContent));
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Step results            (" + mmStr + "℃温度控制性能)", fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Item (项目)");
            if (rise)
            {
                if (slope_get > 0)
                {
                    myLS.Add("Max. heat rate (最大升温速率)");
                    myLS.Add("Avg. heat rate (平均升温速率)");
                }
                myLS.Add("Max. overshoot (最大过冲)");
                myLS.Add("Avg. overshoot (平均过冲)");
                myLS.Add("Heat time      (升温时间)");
                myLS.Add("Hold time      (恒温时间)");
            }
            else
            {
                if (slope_get > 0)
                {
                    myLS.Add("Max. cool rate (最大降温速率)");
                    myLS.Add("Avg. cool rate (平均降温速率)");
                }
                myLS.Add("Max. overshoot (最大过冲)");
                myLS.Add("Avg. overshoot (平均过冲)");
                myLS.Add("Cool time      (降温时间)");
                myLS.Add("Hold time      (恒温时间)");
            }
            myMax = GetJoinLen(myLS, LENL, LENS);
            idx = 0;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Measured (测量值)";
            if (slope_get > 0)
            {
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (slope_get / 100.0f).ToString("F2") + " ℃/sec";
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (((slope_tmp * 100000 / MyDefine.myXET.homstep) / (slope_stop - slope_start)) / 100.0f).ToString("F2") + " ℃/sec";
            }
            if (rise)
            {
                if (over_max > (myPicture.myTP[step_target].temperature))
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + MyDefine.myXET.GetTemp(over_max) + " ℃";
                    myLS[idx] = myLS[idx] + " (+" + MyDefine.myXET.GetTemp(over_max - (myPicture.myTP[step_target].temperature)) + "℃)";
                }
                else
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                }
            }
            else
            {
                if (over_max < (myPicture.myTP[step_target].temperature))
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + MyDefine.myXET.GetTemp(over_max) + " ℃";
                    myLS[idx] = myLS[idx] + " (" + MyDefine.myXET.GetTemp(over_max - (myPicture.myTP[step_target].temperature)) + "℃)";
                }
                else
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                }
            }
            if (rise)
            {
                if (over_avg > (myPicture.myTP[step_target].temperature))
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + MyDefine.myXET.GetTemp(over_avg) + " ℃";
                    myLS[idx] = myLS[idx] + " (+" + MyDefine.myXET.GetTemp(over_avg - (myPicture.myTP[step_target].temperature)) + "℃)";
                }
                else
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                }
            }
            else
            {
                if (over_avg < (myPicture.myTP[step_target].temperature))
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + MyDefine.myXET.GetTemp(over_avg) + " ℃";
                    myLS[idx] = myLS[idx] + " (" + MyDefine.myXET.GetTemp(over_avg - (myPicture.myTP[step_target].temperature)) + "℃)";
                }
                else
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "无过冲";
                }
            }
            idx++;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((const_begin - begin) * MyDefine.myXET.homstep / 1000).ToString("f1") + " sec";
            idx++;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + ((const_end - const_begin) * MyDefine.myXET.homstep / 1000).ToString("f1") + " sec";
            myMax = GetJoinLen(myLS, LENL, LENS);
            idx = 0;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Specification (参考)";
            if (slope_get > 0)
            {
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_rate / 100.0f).ToString("F1") + " ℃/sec";
                idx++;
                myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_rate / 100.0f).ToString("F1") + " ℃/sec";
            }
            idx++;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_over / 100.0f).ToString("F1") + " ℃";
            idx++;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + (spec_over / 100.0f).ToString("F1") + " ℃";
            myMax = GetJoinLen(myLS, LENL, LENW);
            idx = 0;
            myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "Result";
            if (slope_get > 0)
            {
                //
                myTmp = slope_get;
                if (myTmp >= (spec_rate * 2))
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                }
                else if (myTmp >= spec_rate)
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                }
                else
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                }

                //
                myTmp = (int)((slope_tmp * 100000 / MyDefine.myXET.homstep) / (slope_stop - slope_start));
                if (myTmp >= (spec_rate * 2))
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                }
                else if (myTmp >= spec_rate)
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                }
                else
                {
                    idx++;
                    myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                }
            }
            if (rise)
            {
                if (over_max > (myPicture.myTP[step_target].temperature))
                {
                    myTmp = myPicture.Devation(over_max, (myPicture.myTP[step_target].temperature));

                    if (myTmp < (spec_over / 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_over)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }
                }
                else
                {
                    idx++;
                }
            }
            else
            {
                if (over_max < (myPicture.myTP[step_target].temperature))
                {
                    myTmp = myPicture.Devation(over_max, (myPicture.myTP[step_target].temperature));

                    if (myTmp < (spec_over / 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_over)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }
                }
                else
                {
                    idx++;
                }
            }
            if (rise)
            {
                if (over_avg > (myPicture.myTP[step_target].temperature))
                {
                    myTmp = myPicture.Devation(over_avg, (myPicture.myTP[step_target].temperature));

                    if (myTmp < (spec_over / 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_over)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }
                }
                else
                {
                    idx++;
                }
            }
            else
            {
                if (over_avg < (myPicture.myTP[step_target].temperature))
                {
                    myTmp = myPicture.Devation(over_avg, (myPicture.myTP[step_target].temperature));

                    if (myTmp < (spec_over / 2))
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√√";
                    }
                    else if (myTmp < spec_over)
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "√";
                    }
                    else
                    {
                        idx++;
                        myLS[idx] = myLS[idx].PadRight(myMax + myLS[idx].Length - Encoding.Default.GetBytes(myLS[idx]).Length, ' ') + "X";
                    }
                }
                else
                {
                    idx++;
                }
            }
            document.Add(new Paragraph(myLS[0], fontContent));
            for (int i = 1; i < myLS.Count; i++)
            {
                document.Add(new Paragraph(myLS[i], fontContent));
            }
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Accuracy results        (" + MyDefine.myXET.myHom[idx_first].time.ToString("HH:mm:ss") + "-" + MyDefine.myXET.myHom[idx_fifth].time.ToString("HH:mm:ss") + ", " + mmStr + "℃温度控准确度)", fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Time (时间)".PadRight((LENL - 2), ' '));
            myLS.Add(("0 sec").PadRight(LENL, ' '));
            myLS.Add((space.ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS.Add(((space * 2).ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS.Add(((space * 3).ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS.Add(((space * 4).ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS[0] = myLS[0] + "Measured (测量值)";
            myLS[1] = myLS[1] + MyDefine.myXET.GetTemp(myRPT[idx_first].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_first].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
            myLS[2] = myLS[2] + MyDefine.myXET.GetTemp(myRPT[idx_second].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_second].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
            myLS[3] = myLS[3] + MyDefine.myXET.GetTemp(myRPT[idx_third].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_third].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
            myLS[4] = myLS[4] + MyDefine.myXET.GetTemp(myRPT[idx_fourth].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_fourth].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
            myLS[5] = myLS[5] + MyDefine.myXET.GetTemp(myRPT[idx_fifth].outAvg) + " ℃" + " (" + MyDefine.myXET.GetTempWithSign(myRPT[idx_fifth].outAvg - (myPicture.myTP[step_target].temperature)) + "℃)";
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Specification (参考)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
            myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
            myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + (spec_dev / 100.0f).ToString("F1") + " ℃";
            myMax = GetJoinLen(myLS, LENL, LENW);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Result";
            myTmp = myPicture.Devation(myRPT[idx_first].outAvg, (myPicture.myTP[step_target].temperature));
            if (myTmp < (spec_dev / 2))
            {
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√";
            }
            else
            {
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "X";
            }
            myTmp = myPicture.Devation(myRPT[idx_second].outAvg, (myPicture.myTP[step_target].temperature));
            if (myTmp < (spec_dev / 2))
            {
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√";
            }
            else
            {
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "X";
            }
            myTmp = myPicture.Devation(myRPT[idx_third].outAvg, (myPicture.myTP[step_target].temperature));
            if (myTmp < (spec_dev / 2))
            {
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√";
            }
            else
            {
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "X";
            }
            myTmp = myPicture.Devation(myRPT[idx_fourth].outAvg, (myPicture.myTP[step_target].temperature));
            if (myTmp < (spec_dev / 2))
            {
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√";
            }
            else
            {
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "X";
            }
            myTmp = myPicture.Devation(myRPT[idx_fifth].outAvg, (myPicture.myTP[step_target].temperature));
            if (myTmp < (spec_dev / 2))
            {
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√";
            }
            else
            {
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "X";
            }
            document.Add(new Paragraph(myLS[0], fontContent));
            for (int i = 1; i < myLS.Count; i++)
            {
                document.Add(new Paragraph(myLS[i], fontContent));
            }
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////
            document.Add(new Paragraph("Accuracy results        (" + MyDefine.myXET.myHom[idx_first].time.ToString("HH:mm:ss") + "-" + MyDefine.myXET.myHom[idx_fifth].time.ToString("HH:mm:ss") + ", " + mmStr + "℃温度控Block均匀性)", fontItem));
            ////////////////////////////////////////////////////////////////////
            myLS.Clear();
            myLS.Add("Time (时间)".PadRight((LENL - 2), ' '));
            myLS.Add(("0 sec").PadRight(LENL, ' '));
            myLS.Add((space.ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS.Add(((space * 2).ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS.Add(((space * 3).ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS.Add(((space * 4).ToString("f1") + " sec").PadRight(LENL, ' '));
            myLS[0] = myLS[0] + "Measured (测量值)";
            Array.Clear(OutFirstTVS8, 0, 8);
            Array.Copy(myRPT[idx_first].OUT, OutFirstTVS8, 8);
            Array.Clear(OutSecondTVS8, 0, 8);
            Array.Copy(myRPT[idx_second].OUT, OutSecondTVS8, 8);
            Array.Clear(OutThirdTVS8, 0, 8);
            Array.Copy(myRPT[idx_third].OUT, OutThirdTVS8, 8);
            Array.Clear(OutFourthTVS8, 0, 8);
            Array.Copy(myRPT[idx_fourth].OUT, OutFourthTVS8, 8);
            Array.Clear(OutFifthTVS8, 0, 8);
            Array.Copy(myRPT[idx_fifth].OUT, OutFifthTVS8, 8);
            myLS[1] = myLS[1] + MyDefine.myXET.GetTemp(OutFirstTVS8.Max() - OutFirstTVS8.Min()) + " ℃";
            myLS[2] = myLS[2] + MyDefine.myXET.GetTemp(OutSecondTVS8.Max() - OutSecondTVS8.Min()) + " ℃";
            myLS[3] = myLS[3] + MyDefine.myXET.GetTemp(OutThirdTVS8.Max() - OutThirdTVS8.Min()) + " ℃";
            myLS[4] = myLS[4] + MyDefine.myXET.GetTemp(OutFourthTVS8.Max() - OutFourthTVS8.Min()) + " ℃";
            myLS[5] = myLS[5] + MyDefine.myXET.GetTemp(OutFifthTVS8.Max() - OutFifthTVS8.Min()) + " ℃";
            myMax = GetJoinLen(myLS, LENL, LENS);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Specification (参考)";
            myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
            myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
            myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
            myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
            myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + (spec_max / 100.0f).ToString("F1") + " ℃";
            myMax = GetJoinLen(myLS, LENL, LENW);
            myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + "Result";
            //zhoup
            myTmp = OutFirstTVS8.Max() - OutFirstTVS8.Min();
            if (myTmp < (spec_dev / 2))
            {
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "√";
            }
            else
            {
                myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + "X";
            }
            myTmp = OutSecondTVS8.Max() - OutSecondTVS8.Min();
            if (myTmp < (spec_dev / 2))
            {
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "√";
            }
            else
            {
                myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + "X";
            }
            myTmp = OutThirdTVS8.Max() - OutThirdTVS8.Min();
            if (myTmp < (spec_dev / 2))
            {
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "√";
            }
            else
            {
                myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + "X";
            }
            myTmp = OutFourthTVS8.Max() - OutFourthTVS8.Min();
            if (myTmp < (spec_dev / 2))
            {
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "√";
            }
            else
            {
                myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + "X";
            }
            myTmp = OutFifthTVS8.Max() - OutFifthTVS8.Min();
            if (myTmp < (spec_dev / 2))
            {
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√√";
            }
            else if (myTmp < spec_dev)
            {
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "√";
            }
            else
            {
                myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + "X";
            }
            document.Add(new Paragraph(myLS[0], fontContent));
            for (int i = 1; i < myLS.Count; i++)
            {
                document.Add(new Paragraph(myLS[i], fontContent));
            }
            document.Add(new Paragraph(blankLine, fontItem));
            ////////////////////////////////////////////////////////////////////

            #endregion

            for (int step = 1; step < (myPicture.myTP.Count - 1); step++)
            {
                if ((myPicture.myTP[step].vutup) || (myPicture.myTP[step].vutdown))
                {
                    #region 升降温计算

                    begin = myPicture.myTP[step].idx_vutbegin;
                    end = myPicture.myTP[step].idx_vutend;
                    idx_50begin = myPicture.myTP[step].idx_50begin;
                    idx_90end = myPicture.myTP[step].idx_90end;
                    idx_90begin = myPicture.myTP[step].idx_90begin;
                    idx_50end = myPicture.myTP[step].idx_50end;

                    #endregion

                    #region 升降温曲线

                    //更新页数和创建新页
                    document.NewPage();
                    document.ResetPageCount();
                    //最后页添加元素
                    document.Add(CreateParagraph("Page " + (++page), fontMessage, Element.ALIGN_RIGHT));
                    document.Add(new Paragraph(blankLine, fontItem));
                    //最后页添加信息
                    document.Add(new Paragraph("Heating and cooling temperature rate  (升降温速率曲线)", fontItem));
                    //最后页添加曲线图
                    document.Add(iTextSharp.text.Image.GetInstance(GetBitmapVut(begin, end, 50, 90), System.Drawing.Imaging.ImageFormat.Bmp));

                    #endregion

                    #region 升降温报告

                    if (myPicture.myTP[step].vutup)
                    {
                        document.Add(new Paragraph(blankLine, fontItem));
                        document.Add(new Paragraph("Heating temperature rate  (升温速率)", fontItem));
                        myLS.Clear();
                        myLS.Add("Measured Ta (Ta温度点测量值)".PadRight(LENL, ' '));
                        myLS.Add("Measured Tb (Tb温度点测量值)".PadRight(LENL, ' '));
                        myLS.Add("Time of Ta (Ta时间)".PadRight(LENL, ' '));
                        myLS.Add("Time of Ta (Tb时间)".PadRight(LENL, ' '));
                        myLS.Add("Seconds from Ta to Tb (从Ta达到Tb的时间)".PadRight(LENL, ' '));
                        myLS.Add("Heating rate Vut (升温速率)".PadRight(LENL, ' '));
                        myMax = GetJoinLen(myLS, LENL, LENW);
                        mmStr = ((myRPT[idx_90end].outAvg - myRPT[idx_50begin].outAvg) * 10.0f / ((idx_90end - idx_50begin) * MyDefine.myXET.homstep)).ToString("f2");
                        myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + MyDefine.myXET.GetTemp(myRPT[idx_50begin].outAvg) + " ℃";
                        myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + MyDefine.myXET.GetTemp(myRPT[idx_90end].outAvg) + " ℃";
                        myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + MyDefine.myXET.myHom[idx_50begin].time.ToString("HH:mm:ss");
                        myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + MyDefine.myXET.myHom[idx_90end].time.ToString("HH:mm:ss");
                        myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + ((idx_90end - idx_50begin) * MyDefine.myXET.homstep / 1000).ToString("f1") + " sec";
                        myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + mmStr + " ℃/sec";
                        document.Add(new Paragraph(myLS[0], fontContent));
                        document.Add(new Paragraph(myLS[1], fontContent));
                        document.Add(new Paragraph(myLS[2], fontContent));
                        document.Add(new Paragraph(myLS[3], fontContent));
                        document.Add(new Paragraph(myLS[4], fontContent));
                        document.Add(new Paragraph(myLS[5], fontContent));
                    }

                    if (myPicture.myTP[step].vutdown)
                    {
                        document.Add(new Paragraph(blankLine, fontItem));
                        document.Add(new Paragraph("Cooling temperature rate  (降温速率)", fontItem));
                        myLS.Clear();
                        myLS.Add("Measured Ta (Ta温度点测量值)".PadRight(LENL, ' '));
                        myLS.Add("Measured Tb (Tb温度点测量值)".PadRight(LENL, ' '));
                        myLS.Add("Time of Ta (Ta时间)".PadRight(LENL, ' '));
                        myLS.Add("Time of Ta (Tb时间)".PadRight(LENL, ' '));
                        myLS.Add("Seconds from Ta to Tb (从Ta达到Tb的时间)".PadRight(LENL, ' '));
                        myLS.Add("Cooling rate Vut (降温速率)".PadRight(LENL, ' '));
                        myMax = GetJoinLen(myLS, LENL, LENW);
                        mmStr = ((myRPT[idx_90end].outAvg - myRPT[idx_50begin].outAvg) * 10.0f / ((idx_50end - idx_90begin) * MyDefine.myXET.homstep)).ToString("f2");
                        myLS[0] = myLS[0].PadRight(myMax + myLS[0].Length - Encoding.Default.GetBytes(myLS[0]).Length, ' ') + MyDefine.myXET.GetTemp(myRPT[idx_90begin].outAvg) + " ℃";
                        myLS[1] = myLS[1].PadRight(myMax + myLS[1].Length - Encoding.Default.GetBytes(myLS[1]).Length, ' ') + MyDefine.myXET.GetTemp(myRPT[idx_50end].outAvg) + " ℃";
                        myLS[2] = myLS[2].PadRight(myMax + myLS[2].Length - Encoding.Default.GetBytes(myLS[2]).Length, ' ') + MyDefine.myXET.myHom[idx_90begin].time.ToString("HH:mm:ss");
                        myLS[3] = myLS[3].PadRight(myMax + myLS[3].Length - Encoding.Default.GetBytes(myLS[3]).Length, ' ') + MyDefine.myXET.myHom[idx_50end].time.ToString("HH:mm:ss");
                        myLS[4] = myLS[4].PadRight(myMax + myLS[4].Length - Encoding.Default.GetBytes(myLS[4]).Length, ' ') + ((idx_50end - idx_90begin) * MyDefine.myXET.homstep / 1000).ToString("f1") + " sec";
                        myLS[5] = myLS[5].PadRight(myMax + myLS[5].Length - Encoding.Default.GetBytes(myLS[5]).Length, ' ') + mmStr + " ℃/sec";
                        document.Add(new Paragraph(myLS[0], fontContent));
                        document.Add(new Paragraph(myLS[1], fontContent));
                        document.Add(new Paragraph(myLS[2], fontContent));
                        document.Add(new Paragraph(myLS[3], fontContent));
                        document.Add(new Paragraph(myLS[4], fontContent));
                        document.Add(new Paragraph(myLS[5], fontContent));
                    }

                    #endregion
                }
            }

            #region 关闭

            //关闭
            document.Close();
            writer.Close();

            //调出pdf
            Process.Start(fileDialog.FileName);

            #endregion
        }

        //温度点begin-end
        //温度target
        private Bitmap GetBitmapPoint(int begin, int end, int target)
        {
            const int Width = 510;
            const int Height = 680;
            const int Info = 22;

            //第idx条横线
            int idx = 1;

            //刻度温度值
            int tmp = myPicture.tmpHigh;

            //刻度Y坐标
            int yline = idx * 500 * Height / (myPicture.tmpHigh - myPicture.tmpLow);

            //颜色
            Color color_pb1 = Color.Black;
            Color color_pb2 = Color.Red;
            Color color_pb3 = Color.Blue;
            Color color_pb4 = Color.Fuchsia;
            Color color_pb5 = Color.ForestGreen;
            Color color_pb6 = Color.Coral;
            Color color_pb7 = Color.LimeGreen;
            Color color_pb8 = Color.Orange;

            //画图的点
            List<Point> probe1 = new List<Point>();
            List<Point> probe2 = new List<Point>();
            List<Point> probe3 = new List<Point>();
            List<Point> probe4 = new List<Point>();
            List<Point> probe5 = new List<Point>();
            List<Point> probe6 = new List<Point>();
            List<Point> probe7 = new List<Point>();
            List<Point> probe8 = new List<Point>();

            //计算时间, end后补10秒
            end += (int)(10000 / MyDefine.myXET.homstep);
            if (end > myPicture.tmpStop)
            {
                end = myPicture.tmpStop;
            }

            //计算
            if ((Width - myPicture.WTEXT) >= (end - begin))
            {
                for (int i = begin; i < end; i++)
                {
                    //坐标X
                    int px = i + myPicture.WTEXT;

                    //(坐标Y,温度)
                    //(0,myPicture.tmpHigh)
                    //(Height,myPicture.tmpLow)
                    //py = Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (myPicture.tmpHigh - myPicture.tmpLow);
                    probe1.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe2.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[1]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe3.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[2]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe4.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[3]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe5.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[4]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe6.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[5]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe7.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[6]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe8.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[7]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                }
            }
            else
            {
                for (int i = begin; i < end; i++)
                {
                    //(坐标X,温度点)
                    //(WTEXT,begin)
                    //(Width,end)
                    int px = (Width - myPicture.WTEXT) * (i - begin) / (end - begin) + myPicture.WTEXT;

                    //(坐标Y,温度)
                    //(0,myPicture.tmpHigh)
                    //(Height,myPicture.tmpLow)
                    //py = Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (myPicture.tmpHigh - myPicture.tmpLow);
                    probe1.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe2.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[1]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe3.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[2]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe4.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[3]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe5.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[4]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe6.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[5]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe7.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[6]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                    probe8.Add(new Point(px, (Height * (myPicture.tmpHigh - MyDefine.myXET.myHom[i].OUT[7]) / (myPicture.tmpHigh - myPicture.tmpLow))));
                }
            }

            //层图
            Bitmap img = new Bitmap(Width, Height + Info);

            //绘制
            Graphics g = Graphics.FromImage(img);

            //填充白色
            g.FillRectangle(Brushes.White, 0, 0, Width, Height + Info);

            //画刻度
            while (yline < Height)
            {
                tmp -= 500;

                yline = idx * 500 * Height / (myPicture.tmpHigh - myPicture.tmpLow);

                if (target == (tmp / 100))
                {
                    //画横线, 加粗加深
                    g.DrawLine(new Pen(Color.Black, 1.0f), new Point(0, yline), new Point(Width, yline));

                    //温度刻度, 加粗
                    g.DrawString((tmp / 100).ToString() + "℃", new System.Drawing.Font("Arial", 8.5f, FontStyle.Bold), Brushes.Black, 0, yline - 12);
                }
                else
                {
                    //画横线
                    g.DrawLine(new Pen(Color.LightGray, 1.0f), new Point(0, yline), new Point(Width, yline));

                    //温度刻度
                    g.DrawString((tmp / 100).ToString() + "℃", myPicture.font_txt, Brushes.Black, 0, yline - 12);
                }

                idx++;
            }

            //画温度线
            g.DrawCurve(new Pen(color_pb1, 1.0f), probe1.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb2, 1.0f), probe2.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb3, 1.0f), probe3.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb4, 1.0f), probe4.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb5, 1.0f), probe5.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb6, 1.0f), probe6.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb7, 1.0f), probe7.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb8, 1.0f), probe8.ToArray(), 0);

            //写信息
            System.Drawing.Font myFont = new System.Drawing.Font("Arial", 8.5f, FontStyle.Bold);
            String myStr = (end - begin).ToString() + " samples, " + ((end - begin) * MyDefine.myXET.homstep / 1000).ToString("f1") + " sec, " + MyDefine.myXET.myHom[begin].time.ToString("HH:mm:ss") + "-" + MyDefine.myXET.myHom[end].time.ToString("HH:mm:ss");
            SizeF mySize = g.MeasureString(myStr, myFont);
            g.DrawString(myStr, myFont, Brushes.Black, (Width - mySize.Width) / 2, Height + 2);

            //
            g.Dispose();

            return img;
        }

        //温度点begin-end
        //温度start,stop
        private Bitmap GetBitmapVut(int begin, int end, int start, int stop)
        {
            const int Width = 510;
            const int Height = 450;
            const int Info = 22;
            const int tmpHigh = 10000;
            const int tmpLow = 3000;

            //第idx条横线
            int idx = 1;

            //刻度温度值
            int tmp = tmpHigh;

            //刻度Y坐标
            int yline = idx * 500 * Height / (tmpHigh - tmpLow);

            //颜色
            Color color_pb1 = Color.Black;
            Color color_pb2 = Color.Red;
            Color color_pb3 = Color.Blue;
            Color color_pb4 = Color.Fuchsia;
            Color color_pb5 = Color.ForestGreen;
            Color color_pb6 = Color.Coral;
            Color color_pb7 = Color.LimeGreen;
            Color color_pb8 = Color.Orange;
            Color color_pb9 = Color.Cyan;
            Color color_pb10 = Color.Yellow;
            Color color_pb11 = Color.Indigo;
            Color color_pb12 = Color.Lime;
            Color color_pb13 = Color.OrangeRed;
            Color color_pb14 = Color.Magenta;
            Color color_pb15 = Color.Gold;

            //画图的点
            List<Point> probe1 = new List<Point>();
            List<Point> probe2 = new List<Point>();
            List<Point> probe3 = new List<Point>();
            List<Point> probe4 = new List<Point>();
            List<Point> probe5 = new List<Point>();
            List<Point> probe6 = new List<Point>();
            List<Point> probe7 = new List<Point>();
            List<Point> probe8 = new List<Point>();
            List<Point> probe9 = new List<Point>();
            List<Point> probe10 = new List<Point>();
            List<Point> probe11 = new List<Point>();
            List<Point> probe12 = new List<Point>();
            List<Point> probe13 = new List<Point>();
            List<Point> probe14 = new List<Point>();
            List<Point> probe15 = new List<Point>();

            //计算时间, end后补10秒
            end += (int)(10000 / MyDefine.myXET.homstep);
            if (end > myPicture.tmpStop)
            {
                end = myPicture.tmpStop;
            }

            //计算
            if ((Width - myPicture.WTEXT) >= (end - begin))
            {
                for (int i = begin; i < end; i++)
                {
                    //坐标X
                    int px = i + myPicture.WTEXT;

                    //(坐标Y,温度)
                    //(0,tmpHigh)
                    //(Height,tmpLow)
                    //py = Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (tmpHigh - tmpLow);
                    probe1.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (tmpHigh - tmpLow))));
                    probe2.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[1]) / (tmpHigh - tmpLow))));
                    probe3.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[2]) / (tmpHigh - tmpLow))));
                    probe4.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[3]) / (tmpHigh - tmpLow))));
                    probe5.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[4]) / (tmpHigh - tmpLow))));
                    probe6.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[5]) / (tmpHigh - tmpLow))));
                    probe7.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[6]) / (tmpHigh - tmpLow))));
                    probe8.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[7]) / (tmpHigh - tmpLow))));
                    probe9.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[8]) / (tmpHigh - tmpLow))));
                    probe10.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[9]) / (tmpHigh - tmpLow))));
                    probe11.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[10]) / (tmpHigh - tmpLow))));
                    probe12.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[11]) / (tmpHigh - tmpLow))));
                    probe13.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[12]) / (tmpHigh - tmpLow))));
                    probe14.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[13]) / (tmpHigh - tmpLow))));
                    probe15.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[14]) / (tmpHigh - tmpLow))));
                }
            }
            else
            {
                for (int i = begin; i < end; i++)
                {
                    //(坐标X,温度点)
                    //(WTEXT,begin)
                    //(Width,end)
                    int px = (Width - myPicture.WTEXT) * (i - begin) / (end - begin) + myPicture.WTEXT;

                    //(坐标Y,温度)
                    //(0,tmpHigh)
                    //(Height,tmpLow)
                    //py = Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (tmpHigh - tmpLow);
                    probe1.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[0]) / (tmpHigh - tmpLow))));
                    probe2.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[1]) / (tmpHigh - tmpLow))));
                    probe3.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[2]) / (tmpHigh - tmpLow))));
                    probe4.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[3]) / (tmpHigh - tmpLow))));
                    probe5.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[4]) / (tmpHigh - tmpLow))));
                    probe6.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[5]) / (tmpHigh - tmpLow))));
                    probe7.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[6]) / (tmpHigh - tmpLow))));
                    probe8.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[7]) / (tmpHigh - tmpLow))));
                    probe9.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[8]) / (tmpHigh - tmpLow))));
                    probe10.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[9]) / (tmpHigh - tmpLow))));
                    probe11.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[10]) / (tmpHigh - tmpLow))));
                    probe12.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[11]) / (tmpHigh - tmpLow))));
                    probe13.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[12]) / (tmpHigh - tmpLow))));
                    probe14.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[13]) / (tmpHigh - tmpLow))));
                    probe15.Add(new Point(px, (Height * (tmpHigh - MyDefine.myXET.myHom[i].OUT[14]) / (tmpHigh - tmpLow))));
                }
            }

            //层图
            Bitmap img = new Bitmap(Width, Height + Info);

            //绘制
            Graphics g = Graphics.FromImage(img);

            //填充白色
            g.FillRectangle(Brushes.White, 0, 0, Width, Height + Info);

            //画刻度
            while (yline < Height)
            {
                tmp -= 500;

                yline = idx * 500 * Height / (tmpHigh - tmpLow);

                if ((start == (tmp / 100)) || (stop == (tmp / 100)))
                {
                    //画横线, 加粗加深
                    g.DrawLine(new Pen(Color.Black, 1.0f), new Point(0, yline), new Point(Width, yline));

                    //温度刻度, 加粗
                    g.DrawString((tmp / 100).ToString() + "℃", new System.Drawing.Font("Arial", 8.5f, FontStyle.Bold), Brushes.Black, 0, yline - 12);
                }
                else
                {
                    //画横线
                    g.DrawLine(new Pen(Color.LightGray, 1.0f), new Point(0, yline), new Point(Width, yline));

                    //温度刻度
                    g.DrawString((tmp / 100).ToString() + "℃", myPicture.font_txt, Brushes.Black, 0, yline - 12);
                }

                idx++;
            }

            //画温度线
            g.DrawCurve(new Pen(color_pb1, 1.0f), probe1.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb2, 1.0f), probe2.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb3, 1.0f), probe3.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb4, 1.0f), probe4.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb5, 1.0f), probe5.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb6, 1.0f), probe6.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb7, 1.0f), probe7.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb8, 1.0f), probe8.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb9, 1.0f), probe9.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb10, 1.0f), probe10.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb11, 1.0f), probe11.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb12, 1.0f), probe12.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb13, 1.0f), probe13.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb14, 1.0f), probe14.ToArray(), 0);
            g.DrawCurve(new Pen(color_pb15, 1.0f), probe15.ToArray(), 0);

            //写信息
            System.Drawing.Font myFont = new System.Drawing.Font("Arial", 8.5f, FontStyle.Bold);
            String myStr = (end - begin).ToString() + " samples, " + ((end - begin) * MyDefine.myXET.homstep / 1000).ToString("f1") + " sec, " + MyDefine.myXET.myHom[begin].time.ToString("HH:mm:ss") + "-" + MyDefine.myXET.myHom[end].time.ToString("HH:mm:ss");
            SizeF mySize = g.MeasureString(myStr, myFont);
            g.DrawString(myStr, myFont, Brushes.Black, (Width - mySize.Width) / 2, Height + 2);

            //
            g.Dispose();

            return img;
        }

        /// <summary>
        /// 备份温度记录值
        /// </summary>
        /// <param name="tmps"></param>
        /// <returns></returns>
        private List<TMP> backUpProTMP(List<TMP> tmps)
        {
            List<TMP> temps = new List<TMP>();
            foreach (var v in tmps)
            {
                TMP tm = new TMP();
                tm.time = v.time;
                tm.OUT = new int[v.OUT.Length];
                for (int i = 0; i < v.OUT.Length; i++)
                {
                    tm.OUT[i] = v.OUT[i];
                }
                temps.Add(tm);
            }
            return temps;
        }

        /// <summary>
        /// 修正按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCorrectionTemp_Click(object sender, EventArgs e)
        {
            //备份数据
            List<TMP> myHomBackUp = backUpProTMP(MyDefine.myXET.myHom);

            //数据修正界面
            MenuCurrectTempForm mct = new MenuCurrectTempForm(myPicture, myHomBackUp);

            mct.refreshPicHand += refreshDraw;

            mct.ShowDialog();

            mct.refreshPicHand -= refreshDraw;

            if (mct.DialogResult == DialogResult.Yes)
            {
                index = 0;

                //将myHom数据更新
                for (int i = 0; i < myHomBackUp.Count; i++)
                {
                    for (int j = 0; j < myHomBackUp[i].OUT.Length; j++)
                    {
                        MyDefine.myXET.myHom[i].OUT[j] = myHomBackUp[i].OUT[j];
                    }
                }

                //保存修正文件
                string pathName = MyDefine.myXET.homFileName.Replace(".tmp", ".cor.tmp");
                MyDefine.myXET.syn_SaveToLog(pathName, true);

                //计算统计值
                MyDefine.myXET.myHomUpdate();

                //重新加载列表
                if (DialogResult.OK == MessageBox.Show("是否重新加载修正后的文件？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    dataForm_Update();
                }
            }
            else
            {
                //重新绘图
                refreshDraw(false);
            }

            myHomBackUp.Clear();
            myHomBackUp = null;
        }

        /// <summary>
        /// 修正顶峰按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCorrectionTopTemp_Click(object sender, EventArgs e)
        {
            //备份数据
            List<TMP> myHomBackUp = backUpProTMP(MyDefine.myXET.myHom);

            //数据修正界面
            MenuCurrectTopTempForm mctt = new MenuCurrectTopTempForm(myPicture, myHomBackUp);
            try
            {
                mctt.nowIndex = myPicture.tmpPick;
                mctt.avgTemp = MyDefine.myXET.myHom[myPicture.tmpPick].outAvg;
            }
            catch
            {
            }

            mctt.refreshPicHand += refreshDraw;

            mctt.ShowDialog();

            mctt.refreshPicHand -= refreshDraw;

            if (mctt.DialogResult == DialogResult.Yes)
            {
                index = 0;

                //将myHom数据更新
                for (int i = 0; i < myHomBackUp.Count; i++)
                {
                    for (int j = 0; j < myHomBackUp[i].OUT.Length; j++)
                    {
                        MyDefine.myXET.myHom[i].OUT[j] = myHomBackUp[i].OUT[j];
                    }
                }

                //保存修正文件
                string pathName = MyDefine.myXET.homFileName.Replace(".tmp", ".cor.tmp");
                MyDefine.myXET.syn_SaveToLog(pathName, true);

                //计算统计值
                MyDefine.myXET.myHomUpdate();

                //重新加载列表
                if (DialogResult.OK == MessageBox.Show("是否重新加载修正后的文件？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    dataForm_Update();
                }
            }
            else
            {
                //重新绘图
                refreshDraw(false);
            }

            myHomBackUp.Clear();
            myHomBackUp = null;
        }

        /// <summary>
        /// 修正更新曲线
        /// </summary>
        /// <param name="isChange">是否为修正触发</param>
        private void refreshDraw(bool isChange)
        {
            if (myPicture.isLoad)
            {
                //初始化位置
                myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(-1, -1);

                //如果不是实时更新则需要重新计算坐标点值
                if (!isChange)
                {
                    ////计算
                    myPicture.getAxis_pictureBox(pictureBox1.Width, pictureBox1.Height, this.tmpMax, this.tmpMin);
                    myPicture.getPoint_pictureBox();
                }

                //画底层
                pictureBoxScope_axis();

                //画顶层
                pictureBoxScope_draw();
            }
        }

        private void combTempNumber_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (myPicture.isLoad)
            {
                //初始化位置
                myPicture.tmpPick = myPicture.getTmpIdx_pictureBox(-1, -1);

                //计算
                myPicture.getAxis_pictureBox(pictureBox1.Width, pictureBox1.Height, this.tmpMax, this.tmpMin);
                myPicture.getPoint_pictureBox();

                //画底层
                pictureBoxScope_axis(combTempNumber.SelectedIndex);

                //画顶层
                pictureBoxScope_draw();
            }

        }
    }
}