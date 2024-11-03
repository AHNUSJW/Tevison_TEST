using Org.BouncyCastle.Crypto;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Windows.Forms;

namespace TVS
{
    public partial class MenuCurrectTopTempForm : Form
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
        private int temp = 0;
        /// <summary>
        /// 设置区间过冲温度
        /// </summary>
        private int limitTem = 0;
        /// <summary>
        /// 鼠标位置所在的下标
        /// </summary>
        public int nowIndex;
        /// <summary>
        /// 鼠标所在点的平均温度
        /// </summary>
        public int avgTemp;


        public MenuCurrectTopTempForm(Tmpicture myPicture, List<TMP> myHomBackUp)
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
        /// 修正温度数据  根据当前需修正温度所处区间 选择不同斜率进行温度值修正
        /// </summary>
        /// <param name="myPos">温度数据集合</param>
        /// <param name="proIndex">探头编号</param>
        public void CalCurrectTempValue(List<MyPoint> myPos, int proIndex)
        {
            try
            {
                int tempMax;
                int tempMin;
                int highData;
                int lowData;
                SectionTempData(proIndex, out tempMax, out tempMin, out highData,out lowData);
                if(tempMax == tempMin)
                {
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        _myHomBackUp[i].OUT[proIndex - 1] = (int)(_myHomBackUp[i].OUT[proIndex - 1] + limitTem);
                        //计算新坐标
                        myPos[i].y = _myPicture.Height * (_myPicture.tmpHigh - _myHomBackUp[i].OUT[proIndex - 1]) / (_myPicture.tmpHigh - _myPicture.tmpLow);
                    }
                }
                else
                {
                    double K = (highData - lowData) * 1.0 / (tempMax - tempMin);
                    for (int i = startIndex; i < endIndex; i++)
                    {
                        int data = _myHomBackUp[i].OUT[proIndex - 1];
                        _myHomBackUp[i].OUT[proIndex - 1] = (int)(highData -  K * (tempMax - data));
                        //计算新坐标
                        myPos[i].y = _myPicture.Height * (_myPicture.tmpHigh - _myHomBackUp[i].OUT[proIndex - 1]) / (_myPicture.tmpHigh - _myPicture.tmpLow);
                    }
                }
                
            }
            catch
            {

            }
        }

        public void SectionTempData(int proIndex, out int tempMax, out int TempMin, out int highData,out int lowData)
        {
            tempMax = _myHomBackUp[nowIndex].OUT[proIndex - 1];
            TempMin = tempMax;
            if(tempMax >= temp)
            {
                highData = temp + limitTem;
                lowData = temp;
                for (int i = nowIndex + 1; i < _myHomBackUp.Count; i++)
                {
                    if(_myHomBackUp[i].OUT[proIndex - 1] < temp)
                    {
                        endIndex = i;
                        break;
                    }
                    if (_myHomBackUp[i].OUT[proIndex - 1] > tempMax)
                    {
                        tempMax = _myHomBackUp[i].OUT[proIndex - 1];
                    }
                    else if (_myHomBackUp[i].OUT[proIndex - 1] < TempMin)
                    {
                        TempMin = _myHomBackUp[i].OUT[proIndex - 1];
                    }
                }
                for (int i = nowIndex; i > 0; i--)
                {
                    if (_myHomBackUp[i].OUT[proIndex - 1] < temp)
                    {
                        startIndex = i + 1;
                        break;
                    }
                    if (_myHomBackUp[i].OUT[proIndex - 1] > tempMax)
                    {
                        tempMax = _myHomBackUp[i].OUT[proIndex - 1];
                    }
                    else if (_myHomBackUp[i].OUT[proIndex - 1] < TempMin)
                    {
                        TempMin = _myHomBackUp[i].OUT[proIndex - 1];
                    }
                }
            }
            else
            {
                highData = temp;
                lowData = temp - limitTem;
                for (int i = nowIndex + 1; i < _myHomBackUp.Count; i++)
                {
                    if (_myHomBackUp[i].OUT[proIndex - 1] > temp)
                    {
                        endIndex = i;
                        break;
                    }
                    if (_myHomBackUp[i].OUT[proIndex - 1] > tempMax)
                    {
                        tempMax = _myHomBackUp[i].OUT[proIndex - 1];
                    }
                    else if (_myHomBackUp[i].OUT[proIndex - 1] < TempMin)
                    {
                        TempMin = _myHomBackUp[i].OUT[proIndex - 1];
                    }
                }
                for (int i = nowIndex; i > 0; i--)
                {
                    if (_myHomBackUp[i].OUT[proIndex - 1] > temp)
                    {
                        startIndex = i + 1;
                        break;
                    }
                    if (_myHomBackUp[i].OUT[proIndex - 1] > tempMax)
                    {
                        tempMax = _myHomBackUp[i].OUT[proIndex - 1];
                    }
                    else if (_myHomBackUp[i].OUT[proIndex - 1] < TempMin)
                    {
                        TempMin = _myHomBackUp[i].OUT[proIndex - 1];
                    }
                }
            }
        }

        #region 控件事件

        private void tbIndex_KeyPress(object sender, KeyPressEventArgs e)
        {
            //只允许输入数字和删除键
            if (((e.KeyChar < '0') || (e.KeyChar > '9')) && (e.KeyChar != 8))
            {
                e.Handled = true;
                return;
            }
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox texbox = sender as TextBox;

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

        /// <summary>
        /// 切换全选探头
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBTemNumber_CheckedChanged(object sender, System.EventArgs e)
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
        /// 加载界面
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuCurrectTopTempForm_Load(object sender, System.EventArgs e)
        {
        }

        /// <summary>
        /// 取消，关闭窗口
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }

        #endregion

        private void btnCorrectTemp_Click(object sender, System.EventArgs e)
        {
            try
            {
               
                if (string.IsNullOrEmpty(textBox2.Text))
                {
                    MessageBox.Show("请先选择温度");
                    return;
                }
                else
                {
                    double num;
                    double.TryParse(textBox2.Text, out num);
                    temp = (int)(num * 100);
                }

                if (string.IsNullOrEmpty(textBox1.Text))
                {
                    MessageBox.Show("请先输入过冲限值");
                    return;
                }
                else
                {
                    double num;
                    double.TryParse(textBox1.Text, out num);
                    limitTem = (int)(num * 100);
                }

                //修正温度数据
                CorrectTemp();
            }
            catch
            {
            }
        }

        /// <summary>
        ///  确认修改
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOk_Click(object sender, System.EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("是否保存当前修正数据？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
            {
                this.DialogResult = DialogResult.Yes;
                this.Close();
            }
        }

        
    }
}
