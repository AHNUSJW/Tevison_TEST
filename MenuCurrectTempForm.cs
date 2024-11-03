using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TVS
{
    public partial class MenuCurrectTempForm : Form
    {
        /// <summary>
        /// 是否所有探头
        /// </summary>
        private bool isAllTemp = false;
        /// <summary>
        /// 图像坐标点
        /// </summary>
        private Tmpicture _myPicture;
        /// <summary>
        /// 备份温度数据
        /// </summary>
        private List<TMP> _myHomBackUp;
        /// <summary>
        /// 刷新图像事件
        /// </summary>
        public event refreshPicDelegate refreshPicHand;

        private List<double> limitTemp = new List<double>();

        /// <summary>
        /// 关键温度区间  -  列表（  斜率 截距）
        /// </summary>
        private Dictionary<string, TempLineSlopeIntercept> tempSlopeInterceptDic = new Dictionary<string, TempLineSlopeIntercept>();

        /// <summary>
        /// 区间开始下标
        /// </summary>
        private int startIndex = 0;
        /// <summary>
        /// 区间结束下标
        /// </summary>
        private int endIndex = 0;
        /// <summary>
        /// 设置该区间温度
        /// </summary>
        private double temp = 0;
        /// <summary>
        /// 设置区间过冲温度
        /// </summary>
        private double limitTem = 0;
        /// <summary>
        /// 设置起始温度
        /// </summary>
        private double startTemp = 0;
        /// <summary>
        /// 设置结束温度
        /// </summary>
        private double endTemp = 0;


        public MenuCurrectTempForm(Tmpicture myPicture, List<TMP> myHomBackUp)
        {

            if (myPicture != null)
            {
                _myPicture = myPicture;
            }
            if (myHomBackUp != null)
            {
                _myHomBackUp = myHomBackUp;
            }

            InitializeComponent();

            //选择第一个探头号
            if (combTempNumber.Items.Count > 0)
            {
                combTempNumber.SelectedIndex = 0;
            }

            //默认全选所有探头号
            checkBTemNumber.Checked = true;
        }

        /// <summary>
        /// 取消 ，关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 修正温度数据  根据当前需修正温度所处区间 选择不同斜率进行温度值修正
        /// </summary>
        /// <param name="myPos">温度数据集合</param>
        /// <param name="proIndex">探头编号</param>
        public void CalCurrectTempValue(List<MyPoint> myPos, int proIndex)
        {
            try
            {
                for (int i = startIndex; i < endIndex; i++)
                {
                    string message;
                    //计算修正温度
                    for (int j = 0; j < limitTemp.Count; j++)
                    {
                        if (_myHomBackUp[i].OUT[proIndex - 1] < limitTemp[j])
                        {
                            message = limitTemp[j - 1] + "|" + limitTemp[j];
                            _myHomBackUp[i].OUT[proIndex - 1] = (int)(_myHomBackUp[i].OUT[proIndex - 1] * tempSlopeInterceptDic[message].slope
                                                             + tempSlopeInterceptDic[message].intercept);
                            break;
                        }
                    }
                    //计算新坐标
                    myPos[i].y = _myPicture.Height * (_myPicture.tmpHigh - _myHomBackUp[i].OUT[proIndex - 1]) / (_myPicture.tmpHigh - _myPicture.tmpLow);
                }
            }
            catch
            {

            }
        }

        /// <summary>
        /// 切换全选探头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBTemNumber_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBTemNumber.Checked)
            {
                combTempNumber.Enabled = false;
                label1.Enabled = false;
                isAllTemp = true;
            }
            else
            {
                combTempNumber.Enabled = true;
                label1.Enabled = true;
                isAllTemp = false;
            }
        }

        /// <summary>
        /// 根据选中探头号进行温度数据修正
        /// </summary>
        private void CorrectTemp()
        {
            try
            {
                if (_myPicture == null || _myPicture.probe1.Count <= 0)
                {
                    return;
                }

                if (isAllTemp)
                {
                    CalCurrectTempValue(_myPicture.probe1, 1);
                    CalCurrectTempValue(_myPicture.probe2, 2);
                    CalCurrectTempValue(_myPicture.probe3, 3);
                    CalCurrectTempValue(_myPicture.probe4, 4);
                    CalCurrectTempValue(_myPicture.probe5, 5);
                    CalCurrectTempValue(_myPicture.probe6, 6);
                    CalCurrectTempValue(_myPicture.probe7, 7);
                    CalCurrectTempValue(_myPicture.probe8, 8);
                    CalCurrectTempValue(_myPicture.probe9, 9);
                    CalCurrectTempValue(_myPicture.probe10, 10);
                    CalCurrectTempValue(_myPicture.probe11, 11);
                    CalCurrectTempValue(_myPicture.probe12, 12);
                    CalCurrectTempValue(_myPicture.probe13, 13);
                    CalCurrectTempValue(_myPicture.probe14, 14);
                    CalCurrectTempValue(_myPicture.probe15, 15);
                }
                else
                {
                    if (!string.IsNullOrEmpty(combTempNumber.Text))
                    {
                        switch (combTempNumber.Text)
                        {
                            case "A1":
                                CalCurrectTempValue(_myPicture.probe1, 1);
                                break;
                            case "A4":
                                CalCurrectTempValue(_myPicture.probe2, 2);
                                break;
                            case "A7":
                                CalCurrectTempValue(_myPicture.probe3, 3);
                                break;
                            case "A10":
                                CalCurrectTempValue(_myPicture.probe4, 4);
                                break;
                            case "A12":
                                CalCurrectTempValue(_myPicture.probe5, 5);
                                break;
                            case "D1":
                                CalCurrectTempValue(_myPicture.probe6, 6);
                                break;
                            case "D7":
                                CalCurrectTempValue(_myPicture.probe7, 7);
                                break;
                            case "D12":
                                CalCurrectTempValue(_myPicture.probe8, 8);
                                break;
                            case "E4":
                                CalCurrectTempValue(_myPicture.probe9, 9);
                                break;
                            case "E10":
                                CalCurrectTempValue(_myPicture.probe10, 10);
                                break;
                            case "H1":
                                CalCurrectTempValue(_myPicture.probe11, 11);
                                break;
                            case "H4":
                                CalCurrectTempValue(_myPicture.probe12, 12);
                                break;
                            case "H7":
                                CalCurrectTempValue(_myPicture.probe13, 13);
                                break;
                            case "H10":
                                CalCurrectTempValue(_myPicture.probe14, 14);
                                break;
                            case "H12":
                                CalCurrectTempValue(_myPicture.probe15, 15);
                                break;
                        }
                    }
                }
                //更新界面曲线
                if (refreshPicHand != null)
                {
                    refreshPicHand(true);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 确认修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("是否保持当前修正数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
        }

        /// <summary>
        /// 斜率 截距计算
        /// </summary>
        private void CalTempKB(List<Dictionary<double, double>> listTemp)
        {
            if (tempSlopeInterceptDic != null)
            {
                tempSlopeInterceptDic.Clear();
            }
            limitTemp.Clear();
            for (int i = 0; i < listTemp.Count; i++)
            {
                limitTemp.Add(listTemp[i].First().Key);

                if (i == 0)
                {
                    continue;
                }

                //斜率实例
                TempLineSlopeIntercept slopeIntercept = new TempLineSlopeIntercept();

                //计算 K = (y2-y1)/(x2-x1)
                slopeIntercept.slope = (listTemp[i].First().Value - listTemp[i - 1].First().Value)
                                      / (listTemp[i].First().Key - listTemp[i - 1].First().Key);
                //计算 B = y2-x2*k
                slopeIntercept.intercept = listTemp[i].First().Value - listTemp[i].First().Key * slopeIntercept.slope;

                tempSlopeInterceptDic.Add($"{listTemp[i - 1].First().Key}|{listTemp[i].First().Key}", slopeIntercept);
                double M = listTemp[i].First().Value;
            }
        }

        private void btnCorrectTemp_Click(object sender, EventArgs e)
        {
            try
            {
                //各指定温度对应偏移量
                double temp = 0;
                double tempSelf = 0;
                string erMsg = string.Empty;
                DateTime startTime;
                DateTime endTime;
                ///当前需修正关键温度列表 ( 温度值 -  需修值 )
                List<Dictionary<double, double>> listTemp = new List<Dictionary<double, double>>();

                //起始时间
                if (string.IsNullOrEmpty(tbStartTime.Text))
                {
                    MessageBox.Show("请先填写起始时间");
                    return;
                }
                //结束时间
                if (string.IsNullOrEmpty(tbEndTime.Text))
                {
                    MessageBox.Show("请先填写结束时间");
                    return;
                }
                if (DateTime.TryParse(tbStartTime.Text, out startTime) && DateTime.TryParse(tbEndTime.Text, out endTime))
                {
                    if(startTime < endTime)
                    {
                        int index;
                        for (index = 0; index < _myHomBackUp.Count; index++)
                        {
                            if (_myHomBackUp[index].time.ToLongTimeString() == startTime.ToLongTimeString())
                            {
                                startIndex = index;
                                break;
                            }
                        }
                        for(;index < _myHomBackUp.Count;index++)
                        {
                            if (_myHomBackUp[index].time.ToLongTimeString() == endTime.ToLongTimeString())
                            {
                                endIndex = index;
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("开始时间需要小于结束时间");
                    }
                }

                //0
                Dictionary<double, double> tempoffsetDic0 = new Dictionary<double, double>();
                tempoffsetDic0.Add(0, 0);
                listTemp.Add(tempoffsetDic0);

                //起始温度
                if (string.IsNullOrEmpty(tbStarTemp.Text))
                {
                    MessageBox.Show("请先输入起始温度");
                    return;
                }
                //结束温度
                if (string.IsNullOrEmpty(tbEndTemp.Text))
                {
                    MessageBox.Show("请先输入结束温度");
                    return;
                }

                if (double.TryParse(tbStarTemp.Text, out temp))
                {
                    //1000
                   startTemp = temp * 100;
                }

                if (double.TryParse(tbEndTemp.Text, out temp))
                {
                    endTemp = temp * 100;
                }

                if(startTemp < endTemp)
                {
                    Dictionary<double, double> tempoffsetDic1 = new Dictionary<double, double>();
                    tempoffsetDic1.Add(startTemp, startTemp);
                    listTemp.Add(tempoffsetDic1);
                    Dictionary<double, double> tempoffsetDic2 = new Dictionary<double, double>();
                    tempoffsetDic2.Add(endTemp, endTemp);
                    listTemp.Add(tempoffsetDic2);
                }
                else if(startTemp > endTemp)
                {
                    Dictionary<double, double> tempoffsetDic1 = new Dictionary<double, double>();
                    tempoffsetDic1.Add(endTemp, endTemp);
                    listTemp.Add(tempoffsetDic1);
                    Dictionary<double, double> tempoffsetDic2 = new Dictionary<double, double>();
                    tempoffsetDic2.Add(startTemp, startTemp);
                    listTemp.Add(tempoffsetDic2);
                }
                else
                {
                    MessageBox.Show("起始温度和结束温度不能相同");
                    return;
                }

                //自定义
                if (!string.IsNullOrEmpty(tbTempSelf.Text) && !string .IsNullOrEmpty(tbOffsetSelf.Text))
                {
                    Dictionary<double, double> tempoffsetDic3 = new Dictionary<double, double>();
                    if (double.TryParse(tbTempSelf.Text, out tempSelf) && double.TryParse(tbOffsetSelf.Text, out temp))
                    {
                        tempSelf *= 100;
                        temp = tempSelf + temp * 100;
                        tempoffsetDic3.Add(tempSelf, temp);
                    }
                    else
                    {
                        MessageBox.Show("请输入正确的格式");
                        return;
                    }
                    listTemp.Insert(2, tempoffsetDic3);
                    //for (int i = 0; i < listTemp.Count; i++)
                    //{
                    //    if (listTemp[i].First().Key > tempSelf)//当自定义温度点找到下限
                    //    {
                    //        listTemp.Insert(i, tempoffsetDic9);
                    //        break;
                    //    }
                    //}
                }

                //12000
                Dictionary<double, double> tempoffsetDic4 = new Dictionary<double, double>();
                tempoffsetDic4.Add(12000, 12000);
                listTemp.Add(tempoffsetDic4);

                ///计算斜率截距
                CalTempKB(listTemp);

                //修正温度数据
                CorrectTemp();

            }
            catch
            {
            }

        }

        private void tbOffset10_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox texbox = sender as TextBox;

            if (texbox.SelectedText != "0")
            {
                if (texbox.Text == "0" && e.KeyChar != '.' && e.KeyChar != '\b')
                {
                    e.Handled = true;
                    return;
                }
            }

            //只允许输入数字,负号,小数点和删除键
            if (((e.KeyChar < '0') || (e.KeyChar > '9')) && (e.KeyChar != '-') && (e.KeyChar != '.') && (e.KeyChar != 8))
            {
                e.Handled = true;
                return;
            }

            //负数符号只能出现在首位
            if ((e.KeyChar == '-') && (((TextBox)sender).Text.Length > 0))
            {
                e.Handled = true;
                return;
            }

            //负号只能输入一次
            if ((e.KeyChar == '-') && (((TextBox)sender).Text.IndexOf('-') != -1))
            {
                e.Handled = true;
                return;
            }

            //第一位不能为小数点
            if ((e.KeyChar == '.') && (((TextBox)sender).Text.Length == 0))
            {
                e.Handled = true;
                return;
            }

            //负号后不能为小数点
            if ((e.KeyChar == '.') && (((TextBox)sender).Text == "-"))
            {
                e.Handled = true;
                return;
            }

            //小数点只能输入一次
            if ((e.KeyChar == '.') && (((TextBox)sender).Text.IndexOf('.') != -1))
            {
                e.Handled = true;
                return;
            }

            if (texbox.SelectedText != "0")
            {
                //正数第一位是0,第二位必须为小数点
                if ((e.KeyChar != '.') && (e.KeyChar != 8) && (((TextBox)sender).Text == "0"))
                {
                    e.Handled = true;
                    return;
                }
            }

            //负数第一位是0,第二位必须为小数点
            if ((e.KeyChar != '.') && (e.KeyChar != 8) && (((TextBox)sender).Text == "-0"))
            {
                e.Handled = true;
                return;
            }
        }

        private void tbTempSelf_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox texbox = sender as TextBox;

            if (texbox.SelectedText != "0")
            {
                if (texbox.Text == "0" && e.KeyChar != '.' && e.KeyChar != '\b')
                {
                    e.Handled = true;
                    return;
                }
            }

            //只允许输入数字,小数点和删除键
            if (((e.KeyChar < '0') || (e.KeyChar > '9'))&& (e.KeyChar != '.') && (e.KeyChar != 8))
            {
                e.Handled = true;
                return;
            }

            //第一位不能为小数点
            if ((e.KeyChar == '.') && (((TextBox)sender).Text.Length == 0))
            {
                e.Handled = true;
                return;
            }

            //小数点只能输入一次
            if ((e.KeyChar == '.') && (((TextBox)sender).Text.IndexOf('.') != -1))
            {
                e.Handled = true;
                return;
            }

            if (texbox.SelectedText != "0")
            {
                //正数第一位是0,第二位必须为小数点
                if ((e.KeyChar != '.') && (e.KeyChar != 8) && (((TextBox)sender).Text == "0"))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void tbStartTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox texbox = sender as TextBox;

            //只允许输入数字和删除键,和空格(时间栏需要)
            if (e.KeyChar == '：')
            {
                //texbox.Text.Replace('：', ':');
                e.KeyChar = ':';
            }
            if (((e.KeyChar < '0') || (e.KeyChar > '9')) && (e.KeyChar != ':') && (e.KeyChar != 8) && (e.KeyChar != ' '))
            {
                e.Handled = true;
                return;
            }
        }
    }
}
