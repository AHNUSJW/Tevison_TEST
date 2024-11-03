namespace TVS
{
    partial class MenuCurrectTempForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MenuCurrectTempForm));
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.combTempNumber = new System.Windows.Forms.ComboBox();
            this.checkBTemNumber = new System.Windows.Forms.CheckBox();
            this.tbStartTime = new System.Windows.Forms.TextBox();
            this.tbEndTime = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.btnCorrectTemp = new System.Windows.Forms.Button();
            this.tbOffsetSelf = new System.Windows.Forms.TextBox();
            this.tbTempSelf = new System.Windows.Forms.TextBox();
            this.tbStarTemp = new System.Windows.Forms.TextBox();
            this.tbEndTemp = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(128, 345);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 35);
            this.btnOk.TabIndex = 0;
            this.btnOk.Text = "确认";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(229, 345);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 35);
            this.btnCancel.TabIndex = 1;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(127, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "探头编号";
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
            this.combTempNumber.Location = new System.Drawing.Point(186, 17);
            this.combTempNumber.Name = "combTempNumber";
            this.combTempNumber.Size = new System.Drawing.Size(121, 27);
            this.combTempNumber.TabIndex = 3;
            // 
            // checkBTemNumber
            // 
            this.checkBTemNumber.AutoSize = true;
            this.checkBTemNumber.Location = new System.Drawing.Point(21, 24);
            this.checkBTemNumber.Name = "checkBTemNumber";
            this.checkBTemNumber.Size = new System.Drawing.Size(84, 16);
            this.checkBTemNumber.TabIndex = 26;
            this.checkBTemNumber.Text = "所有探头号";
            this.checkBTemNumber.UseVisualStyleBackColor = true;
            this.checkBTemNumber.CheckedChanged += new System.EventHandler(this.checkBTemNumber_CheckedChanged);
            // 
            // tbStartTime
            // 
            this.tbStartTime.Font = new System.Drawing.Font("宋体", 18F);
            this.tbStartTime.Location = new System.Drawing.Point(127, 68);
            this.tbStartTime.Multiline = true;
            this.tbStartTime.Name = "tbStartTime";
            this.tbStartTime.Size = new System.Drawing.Size(148, 34);
            this.tbStartTime.TabIndex = 52;
            this.tbStartTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbStartTime_KeyPress);
            // 
            // tbEndTime
            // 
            this.tbEndTime.Font = new System.Drawing.Font("宋体", 18F);
            this.tbEndTime.Location = new System.Drawing.Point(127, 109);
            this.tbEndTime.Multiline = true;
            this.tbEndTime.Name = "tbEndTime";
            this.tbEndTime.Size = new System.Drawing.Size(148, 34);
            this.tbEndTime.TabIndex = 53;
            this.tbEndTime.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbStartTime_KeyPress);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(49, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 20);
            this.label5.TabIndex = 61;
            this.label5.Text = "起始时间";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(49, 116);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(73, 20);
            this.label6.TabIndex = 60;
            this.label6.Text = "结束时间";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(50, 256);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 20);
            this.label7.TabIndex = 59;
            this.label7.Text = "修正温度";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(62, 297);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(57, 20);
            this.label8.TabIndex = 58;
            this.label8.Text = "修正值";
            // 
            // btnCorrectTemp
            // 
            this.btnCorrectTemp.Location = new System.Drawing.Point(27, 345);
            this.btnCorrectTemp.Name = "btnCorrectTemp";
            this.btnCorrectTemp.Size = new System.Drawing.Size(75, 35);
            this.btnCorrectTemp.TabIndex = 64;
            this.btnCorrectTemp.Text = "修正";
            this.btnCorrectTemp.UseVisualStyleBackColor = true;
            this.btnCorrectTemp.Click += new System.EventHandler(this.btnCorrectTemp_Click);
            // 
            // tbOffsetSelf
            // 
            this.tbOffsetSelf.Font = new System.Drawing.Font("宋体", 18F);
            this.tbOffsetSelf.Location = new System.Drawing.Point(127, 289);
            this.tbOffsetSelf.Multiline = true;
            this.tbOffsetSelf.Name = "tbOffsetSelf";
            this.tbOffsetSelf.Size = new System.Drawing.Size(147, 34);
            this.tbOffsetSelf.TabIndex = 68;
            this.tbOffsetSelf.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbOffset10_KeyPress);
            // 
            // tbTempSelf
            // 
            this.tbTempSelf.Font = new System.Drawing.Font("宋体", 18F);
            this.tbTempSelf.Location = new System.Drawing.Point(127, 248);
            this.tbTempSelf.Multiline = true;
            this.tbTempSelf.Name = "tbTempSelf";
            this.tbTempSelf.Size = new System.Drawing.Size(147, 34);
            this.tbTempSelf.TabIndex = 66;
            this.tbTempSelf.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbTempSelf_KeyPress);
            // 
            // tbStarTemp
            // 
            this.tbStarTemp.Font = new System.Drawing.Font("宋体", 18F);
            this.tbStarTemp.Location = new System.Drawing.Point(126, 159);
            this.tbStarTemp.Multiline = true;
            this.tbStarTemp.Name = "tbStarTemp";
            this.tbStarTemp.Size = new System.Drawing.Size(148, 34);
            this.tbStarTemp.TabIndex = 69;
            this.tbStarTemp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbTempSelf_KeyPress);
            // 
            // tbEndTemp
            // 
            this.tbEndTemp.Font = new System.Drawing.Font("宋体", 18F);
            this.tbEndTemp.Location = new System.Drawing.Point(126, 200);
            this.tbEndTemp.Multiline = true;
            this.tbEndTemp.Name = "tbEndTemp";
            this.tbEndTemp.Size = new System.Drawing.Size(148, 34);
            this.tbEndTemp.TabIndex = 70;
            this.tbEndTemp.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbTempSelf_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(48, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 20);
            this.label2.TabIndex = 72;
            this.label2.Text = "起始温度";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(48, 207);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 20);
            this.label3.TabIndex = 71;
            this.label3.Text = "结束温度";
            // 
            // MenuCurrectTempForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(331, 401);
            this.Controls.Add(this.tbStarTemp);
            this.Controls.Add(this.tbEndTemp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbOffsetSelf);
            this.Controls.Add(this.tbTempSelf);
            this.Controls.Add(this.btnCorrectTemp);
            this.Controls.Add(this.tbStartTime);
            this.Controls.Add(this.tbEndTime);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.checkBTemNumber);
            this.Controls.Add(this.combTempNumber);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(347, 513);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(347, 39);
            this.Name = "MenuCurrectTempForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox combTempNumber;
        private System.Windows.Forms.CheckBox checkBTemNumber;
        private System.Windows.Forms.TextBox tbStartTime;
        private System.Windows.Forms.TextBox tbEndTime;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnCorrectTemp;
        private System.Windows.Forms.TextBox tbOffsetSelf;
        private System.Windows.Forms.TextBox tbTempSelf;
        private System.Windows.Forms.TextBox tbStarTemp;
        private System.Windows.Forms.TextBox tbEndTemp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}