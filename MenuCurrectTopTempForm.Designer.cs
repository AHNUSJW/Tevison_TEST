namespace TVS
{
    partial class MenuCurrectTopTempForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuCurrectTopTempForm));
            this.checkBTemNumber = new System.Windows.Forms.CheckBox();
            this.combTempNumber = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.btnCorrectTemp = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // checkBTemNumber
            // 
            this.checkBTemNumber.AutoSize = true;
            this.checkBTemNumber.Location = new System.Drawing.Point(23, 12);
            this.checkBTemNumber.Name = "checkBTemNumber";
            this.checkBTemNumber.Size = new System.Drawing.Size(84, 16);
            this.checkBTemNumber.TabIndex = 29;
            this.checkBTemNumber.Text = "所有探头号";
            this.checkBTemNumber.UseVisualStyleBackColor = true;
            this.checkBTemNumber.CheckedChanged += new System.EventHandler(this.checkBTemNumber_CheckedChanged);
            // 
            // combTempNumber
            // 
            this.combTempNumber.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.combTempNumber.Font = new System.Drawing.Font("宋体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.combTempNumber.FormattingEnabled = true;
            this.combTempNumber.Items.AddRange(new object[] {
            "A1",
            "A4",
            "A7",
            "A10",
            "A12",
            "D1",
            "D7",
            "D12",
            "E4",
            "E10",
            "H1",
            "H4",
            "H7",
            "H10",
            "H12"});
            this.combTempNumber.Location = new System.Drawing.Point(173, 5);
            this.combTempNumber.Name = "combTempNumber";
            this.combTempNumber.Size = new System.Drawing.Size(121, 27);
            this.combTempNumber.TabIndex = 28;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(114, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 27;
            this.label1.Text = "探头编号";
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.textBox2);
            this.panel2.Controls.Add(this.textBox1);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Location = new System.Drawing.Point(23, 38);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(278, 113);
            this.panel2.TabIndex = 69;
            // 
            // textBox2
            // 
            this.textBox2.Font = new System.Drawing.Font("宋体", 18F);
            this.textBox2.Location = new System.Drawing.Point(113, 20);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(148, 34);
            this.textBox2.TabIndex = 68;
            this.textBox2.Text = "0";
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("宋体", 18F);
            this.textBox1.Location = new System.Drawing.Point(113, 61);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(148, 34);
            this.textBox1.TabIndex = 65;
            this.textBox1.Text = "0";
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox1_KeyPress);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(24, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(73, 20);
            this.label4.TabIndex = 67;
            this.label4.Text = "修正温度";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(24, 69);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 20);
            this.label5.TabIndex = 66;
            this.label5.Text = "过冲变量 ≤";
            // 
            // btnCorrectTemp
            // 
            this.btnCorrectTemp.Location = new System.Drawing.Point(24, 164);
            this.btnCorrectTemp.Name = "btnCorrectTemp";
            this.btnCorrectTemp.Size = new System.Drawing.Size(75, 35);
            this.btnCorrectTemp.TabIndex = 72;
            this.btnCorrectTemp.Text = "修正";
            this.btnCorrectTemp.UseVisualStyleBackColor = true;
            this.btnCorrectTemp.Click += new System.EventHandler(this.btnCorrectTemp_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(226, 164);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 35);
            this.btnCancel.TabIndex = 71;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(125, 164);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 35);
            this.btnOk.TabIndex = 70;
            this.btnOk.Text = "确认";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // MenuCurrectTopTempForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(330, 221);
            this.Controls.Add(this.btnCorrectTemp);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.checkBTemNumber);
            this.Controls.Add(this.combTempNumber);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(346, 260);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(346, 260);
            this.Name = "MenuCurrectTopTempForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "峰点修正";
            this.Load += new System.EventHandler(this.MenuCurrectTopTempForm_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox checkBTemNumber;
        private System.Windows.Forms.ComboBox combTempNumber;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnCorrectTemp;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox textBox2;
    }
}