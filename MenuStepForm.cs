using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;

namespace TVS
{
    public partial class MenuStepForm : Form
    {
        private bool isSave = false;

        //
        public MenuStepForm()
        {
            InitializeComponent();
        }

        //
        private void digit_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox objControl = sender as TextBox;

            if (objControl is null)
            {
                return;
            }

            if (objControl.Name == "textBox1")
            {

                if (!IsDouble(e.KeyChar, objControl, 2))
                {
                    e.Handled = true;
                }
            }
            else
            {
                ////只允许输入数字和删除键
                if (((e.KeyChar < '0') || (e.KeyChar > '9')) && (e.KeyChar != 8))
                {
                    e.Handled = true;
                    return;
                }
                else
                {
                    button6.BackColor = Color.Firebrick;
                }
            }

        }
        // 判断浮点型 
        public static bool IsDouble(char keyChar, TextBox objControl, int numDecimalDigit = 1)
        {
            bool bolResult = false;

            string strValue = objControl.Text;

            // 第一位不能输入小数点
            if (strValue == "" && keyChar == '.')
                return false;

            if (strValue == "0" && keyChar != '.' && keyChar != '\b')
                return false;

            if (System.Text.RegularExpressions.Regex.IsMatch(keyChar.ToString(), "[0-9\b.]"))
            {
                // 输入小数点
                if (keyChar == '.')
                {
                    // 检测是否存在小数点，如果存在就不能输入小数点
                    if (!strValue.Contains("."))
                        bolResult = true;
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(keyChar.ToString(), "[0-9]"))
                {
                    string strSelectText = objControl.SelectedText;

                    // 判断是否有选择字符串
                    if (strSelectText.Trim() != "")
                    {
                        bolResult = true;
                    }
                    // 判断是否存在小数点
                    else if (strValue.Contains("."))
                    {
                        // 获取小数点位置
                        var numDigitIndex = strValue.IndexOf('.') + 1;
                        var numSelectionStart = objControl.SelectionStart;

                        // 判断光标在小数点后  && numDigitIndex > strValue.Length
                        if (numSelectionStart > numDigitIndex)
                        {
                            // 获取小数点后的小数位数
                            var strSmallNumber = strValue.Substring(numDigitIndex, strValue.Length - numDigitIndex);

                            // 判断小数位数
                            if (strSmallNumber.Length >= numDecimalDigit)
                            {
                                // 字符串最后一个字符替换成输入字符
                                objControl.Text = strValue.Substring(0, strValue.Length - 1) + keyChar.ToString();
                                objControl.SelectionStart = numSelectionStart;
                            }
                            else
                            {
                                bolResult = true;
                            }
                        }
                        else
                        {
                            bolResult = true;
                        }
                    }
                    else
                    {
                        bolResult = true;
                    }
                }
                else
                {
                    bolResult = true;
                }
            }
            return bolResult;
        }
        //
        private void digit_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.C))
            {
                try
                {
                    Convert.ToInt64(((TextBox)sender).SelectedText);  //检查是否数字
                    Clipboard.SetText(((TextBox)sender).SelectedText.Trim()); //Ctrl+C 复制
                }
                catch (Exception)
                {
                    e.Handled = true;
                }
            }
            if (e.KeyData == (Keys.Control | Keys.V))
            {
                if (Clipboard.ContainsText())
                {
                    try
                    {
                        UInt64 tmp = (UInt64)Convert.ToDecimal(Clipboard.GetText().Trim());  //检查是否数字
                        ((TextBox)sender).SelectedText = tmp.ToString(); //Ctrl+V 粘贴
                    }
                    catch (Exception)
                    {
                        e.Handled = true;
                    }
                }
            }
            if (e.KeyData == (Keys.Control | Keys.X))
            {
                if (Clipboard.ContainsText())
                {
                    try
                    {
                        ((TextBox)sender).Cut();
                    }
                    catch (Exception)
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void UpdatePrmListbox()
        {
            while (listBox1.Items.Count < MyDefine.myXET.myPRM.Count)
            {
                listBox1.Items.Add("");
            }

            for (int i = 0; i < MyDefine.myXET.myPRM.Count; i++)
            {
                listBox1.Items[i] = "#" + (i + 1).ToString() + "  " + MyDefine.myXET.myPRM[i].name;
            }

            while (listBox1.Items.Count > MyDefine.myXET.myPRM.Count)
            {
                listBox1.Items.RemoveAt(listBox1.Items.Count - 1);
            }
        }

        private void UpdateStepListbox()
        {
            if (listBox1.SelectedIndex >= 0)
            {
                int idx = listBox1.SelectedIndex;

                while (listBox2.Items.Count < MyDefine.myXET.myPRM[idx].myTP.Count)
                {
                    listBox2.Items.Add("");
                }

                for (int i = 0; i < MyDefine.myXET.myPRM[idx].myTP.Count; i++)
                {
                    listBox2.Items[i] = "步骤 " + (i + 1).ToString() + "      " + (MyDefine.myXET.myPRM[idx].myTP[i].temperature / 100f).ToString() + " ℃      " + MyDefine.myXET.myPRM[idx].myTP[i].time.ToString() + " sec";
                }

                while (listBox2.Items.Count > MyDefine.myXET.myPRM[idx].myTP.Count)
                {
                    listBox2.Items.RemoveAt(listBox2.Items.Count - 1);
                }
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button6.BackColor = button3.BackColor;

            UpdateStepListbox();
        }

        //删除程序
        private void button1_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > 1)
            {
                int idx = listBox1.SelectedIndex;

                //
                MyDefine.myXET.myPRM[idx].myTP.Clear();
                MyDefine.myXET.myPRM.RemoveAt(idx);

                //
                idx = Math.Min(listBox1.SelectedIndex, listBox1.Items.Count - 2);

                //
                UpdatePrmListbox();

                //
                listBox1.SelectedIndex = idx;

                //
                isSave = true;
            }
        }

        //保存程序
        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Length > 0)
            {
                int id = MyDefine.myXET.myPRM.Count;

                MyDefine.myXET.myPRM.Add(new PRM());
                MyDefine.myXET.myPRM[id].name = textBox3.Text;

                UpdatePrmListbox();

                textBox3.Text = "";
                listBox1.SelectedIndex = listBox1.Items.Count - 1;

                isSave = true;
            }
        }

        //上移
        private void button3_Click(object sender, EventArgs e)
        {
            if ((listBox1.SelectedIndex > 1) && (listBox2.SelectedIndex > 0))//if ( (listBox2.SelectedIndex > 0))
            {
                int idx = listBox1.SelectedIndex;
                int idy = listBox2.SelectedIndex;

                int temperature;
                int time;

                temperature = MyDefine.myXET.myPRM[idx].myTP[idy].temperature;
                time = MyDefine.myXET.myPRM[idx].myTP[idy].time;

                MyDefine.myXET.myPRM[idx].myTP[idy].temperature = MyDefine.myXET.myPRM[idx].myTP[idy - 1].temperature;
                MyDefine.myXET.myPRM[idx].myTP[idy].time = MyDefine.myXET.myPRM[idx].myTP[idy - 1].time;

                MyDefine.myXET.myPRM[idx].myTP[idy - 1].temperature = temperature;
                MyDefine.myXET.myPRM[idx].myTP[idy - 1].time = time;

                UpdateStepListbox();

                listBox2.SelectedIndex--;

                isSave = true;
            }
        }

        //下移
        private void button4_Click(object sender, EventArgs e)
        {
            //if ((listBox2.SelectedItems.Count >= 0) && (listBox2.SelectedIndex < (listBox2.Items.Count - 1)))
            if ((listBox1.SelectedIndex > 1) && (listBox2.SelectedIndex >= 0) && (listBox2.SelectedIndex < (listBox2.Items.Count - 1)))
            {
                int idx = listBox1.SelectedIndex;
                int idy = listBox2.SelectedIndex;

                int temperature;
                int time;

                temperature = MyDefine.myXET.myPRM[idx].myTP[idy].temperature;
                time = MyDefine.myXET.myPRM[idx].myTP[idy].time;

                MyDefine.myXET.myPRM[idx].myTP[idy].temperature = MyDefine.myXET.myPRM[idx].myTP[idy + 1].temperature;
                MyDefine.myXET.myPRM[idx].myTP[idy].time = MyDefine.myXET.myPRM[idx].myTP[idy + 1].time;

                MyDefine.myXET.myPRM[idx].myTP[idy + 1].temperature = temperature;
                MyDefine.myXET.myPRM[idx].myTP[idy + 1].time = time;

                UpdateStepListbox();

                listBox2.SelectedIndex++;

                isSave = true;
            }
        }

        //删除温度点
        private void button5_Click(object sender, EventArgs e)
        {
            if ((listBox1.SelectedIndex > 1) && (listBox2.SelectedIndex >= 0))
            {
                int idx = listBox2.SelectedIndex;

                MyDefine.myXET.myPRM[listBox1.SelectedIndex].myTP.RemoveAt(idx);

                idx = Math.Min(listBox2.SelectedIndex, listBox2.Items.Count - 2);

                UpdateStepListbox();

                listBox2.SelectedIndex = idx;

                isSave = true;
            }
        }

        //增加温度点
        private void button6_Click(object sender, EventArgs e)
        {
            if ((listBox1.SelectedIndex > 1) && (textBox1.Text.Length > 0) && (textBox2.Text.Length > 0))
            {
                int temperature;
                int time;

                float temperturef = float.Parse(textBox1.Text);
                temperature = (int)(temperturef * 100);
                //temperature = Convert.ToInt32(textBox1.Text);
                time = Convert.ToInt32(textBox2.Text);

                if ((temperature > 0) && (temperature < 10000) && (time > 10) && (time < 600))
                {
                    button6.BackColor = button3.BackColor;

                    MyDefine.myXET.myPRM[listBox1.SelectedIndex].myTP.Add(new tmpoint(temperature, time));

                    UpdateStepListbox();

                    listBox2.SelectedIndex = listBox2.Items.Count - 1;

                    isSave = true;
                }
            }
        }

        private void MenuStepForm_Load(object sender, EventArgs e)
        {
            MyDefine.myXET.GetUserPrm();
            UpdatePrmListbox();
            UpdateStepListbox();
        }

        private void MenuStepForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (isSave)
            {
                switch (MessageBox.Show("是否修改并保存？", "", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning))
                {
                    default:
                    case DialogResult.Cancel:
                        e.Cancel = true;
                        break;
                    case DialogResult.Yes:
                        this.DialogResult = DialogResult.Yes;
                        MyDefine.myXET.SaveUserPrm();
                        break;
                    case DialogResult.No:
                        break;
                }
            }
        }
    }
}

//end

