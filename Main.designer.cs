namespace TVS
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.FileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AccountToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PrintToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ProjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ConnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RunToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TableToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.HelpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReferToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DebugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FactToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CurveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(40, 40);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileToolStripMenuItem,
            this.ProjectToolStripMenuItem,
            this.DataToolStripMenuItem,
            this.HelpToolStripMenuItem,
            this.DebugToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(5, 9, 10, 9);
            this.menuStrip1.Size = new System.Drawing.Size(800, 43);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // FileToolStripMenuItem
            // 
            this.FileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AccountToolStripMenuItem,
            this.PrintToolStripMenuItem,
            this.ExitToolStripMenuItem});
            this.FileToolStripMenuItem.Name = "FileToolStripMenuItem";
            this.FileToolStripMenuItem.Size = new System.Drawing.Size(56, 25);
            this.FileToolStripMenuItem.Text = "文件";
            // 
            // AccountToolStripMenuItem
            // 
            this.AccountToolStripMenuItem.Name = "AccountToolStripMenuItem";
            this.AccountToolStripMenuItem.Size = new System.Drawing.Size(114, 26);
            this.AccountToolStripMenuItem.Text = "帐号";
            this.AccountToolStripMenuItem.Click += new System.EventHandler(this.AccountToolStripMenuItem_Click);
            // 
            // PrintToolStripMenuItem
            // 
            this.PrintToolStripMenuItem.Name = "PrintToolStripMenuItem";
            this.PrintToolStripMenuItem.Size = new System.Drawing.Size(114, 26);
            this.PrintToolStripMenuItem.Text = "打印";
            this.PrintToolStripMenuItem.Click += new System.EventHandler(this.PrintToolStripMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(114, 26);
            this.ExitToolStripMenuItem.Text = "退出";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // ProjectToolStripMenuItem
            // 
            this.ProjectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ConnectToolStripMenuItem,
            this.SetToolStripMenuItem,
            this.ReadToolStripMenuItem,
            this.RunToolStripMenuItem});
            this.ProjectToolStripMenuItem.Name = "ProjectToolStripMenuItem";
            this.ProjectToolStripMenuItem.Size = new System.Drawing.Size(56, 25);
            this.ProjectToolStripMenuItem.Text = "工程";
            // 
            // ConnectToolStripMenuItem
            // 
            this.ConnectToolStripMenuItem.Name = "ConnectToolStripMenuItem";
            this.ConnectToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.ConnectToolStripMenuItem.Text = "设备连接";
            this.ConnectToolStripMenuItem.Click += new System.EventHandler(this.ConnectToolStripMenuItem_Click);
            // 
            // SetToolStripMenuItem
            // 
            this.SetToolStripMenuItem.Name = "SetToolStripMenuItem";
            this.SetToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.SetToolStripMenuItem.Text = "设备设置";
            this.SetToolStripMenuItem.Click += new System.EventHandler(this.SetToolStripMenuItem_Click);
            // 
            // ReadToolStripMenuItem
            // 
            this.ReadToolStripMenuItem.Name = "ReadToolStripMenuItem";
            this.ReadToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.ReadToolStripMenuItem.Text = "设备导出";
            this.ReadToolStripMenuItem.Click += new System.EventHandler(this.ReadToolStripMenuItem_Click);
            // 
            // RunToolStripMenuItem
            // 
            this.RunToolStripMenuItem.Name = "RunToolStripMenuItem";
            this.RunToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.RunToolStripMenuItem.Text = "实时监控";
            this.RunToolStripMenuItem.Click += new System.EventHandler(this.RunToolStripMenuItem_Click);
            // 
            // DataToolStripMenuItem
            // 
            this.DataToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ImportToolStripMenuItem,
            this.TableToolStripMenuItem,
            this.StepToolStripMenuItem,
            this.ReportToolStripMenuItem});
            this.DataToolStripMenuItem.Name = "DataToolStripMenuItem";
            this.DataToolStripMenuItem.Size = new System.Drawing.Size(56, 25);
            this.DataToolStripMenuItem.Text = "数据";
            // 
            // ImportToolStripMenuItem
            // 
            this.ImportToolStripMenuItem.Name = "ImportToolStripMenuItem";
            this.ImportToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.ImportToolStripMenuItem.Text = "数据载入";
            this.ImportToolStripMenuItem.Click += new System.EventHandler(this.ImportToolStripMenuItem_Click);
            // 
            // TableToolStripMenuItem
            // 
            this.TableToolStripMenuItem.Name = "TableToolStripMenuItem";
            this.TableToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.TableToolStripMenuItem.Text = "数据曲线";
            this.TableToolStripMenuItem.Click += new System.EventHandler(this.TableToolStripMenuItem_Click);
            // 
            // StepToolStripMenuItem
            // 
            this.StepToolStripMenuItem.Name = "StepToolStripMenuItem";
            this.StepToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.StepToolStripMenuItem.Text = "检测程序";
            this.StepToolStripMenuItem.Click += new System.EventHandler(this.StepToolStripMenuItem_Click);
            // 
            // ReportToolStripMenuItem
            // 
            this.ReportToolStripMenuItem.Name = "ReportToolStripMenuItem";
            this.ReportToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.ReportToolStripMenuItem.Text = "生成报告";
            this.ReportToolStripMenuItem.Click += new System.EventHandler(this.ReportToolStripMenuItem_Click);
            // 
            // HelpToolStripMenuItem
            // 
            this.HelpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ReferToolStripMenuItem,
            this.AboutToolStripMenuItem});
            this.HelpToolStripMenuItem.Name = "HelpToolStripMenuItem";
            this.HelpToolStripMenuItem.Size = new System.Drawing.Size(56, 25);
            this.HelpToolStripMenuItem.Text = "帮助";
            // 
            // ReferToolStripMenuItem
            // 
            this.ReferToolStripMenuItem.Name = "ReferToolStripMenuItem";
            this.ReferToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.ReferToolStripMenuItem.Text = "操作手册";
            this.ReferToolStripMenuItem.Click += new System.EventHandler(this.ReferToolStripMenuItem_Click);
            // 
            // AboutToolStripMenuItem
            // 
            this.AboutToolStripMenuItem.Name = "AboutToolStripMenuItem";
            this.AboutToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.AboutToolStripMenuItem.Text = "软件版本";
            this.AboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItem_Click);
            // 
            // DebugToolStripMenuItem
            // 
            this.DebugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FactToolStripMenuItem,
            this.CalToolStripMenuItem,
            this.CurveToolStripMenuItem});
            this.DebugToolStripMenuItem.Name = "DebugToolStripMenuItem";
            this.DebugToolStripMenuItem.Size = new System.Drawing.Size(56, 25);
            this.DebugToolStripMenuItem.Text = "调试";
            // 
            // FactToolStripMenuItem
            // 
            this.FactToolStripMenuItem.Name = "FactToolStripMenuItem";
            this.FactToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.FactToolStripMenuItem.Text = "厂家设置";
            this.FactToolStripMenuItem.Click += new System.EventHandler(this.FactToolStripMenuItem_Click);
            // 
            // CalToolStripMenuItem
            // 
            this.CalToolStripMenuItem.Name = "CalToolStripMenuItem";
            this.CalToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.CalToolStripMenuItem.Text = "设备校准";
            this.CalToolStripMenuItem.Click += new System.EventHandler(this.CalToolStripMenuItem_Click);
            // 
            // CurveToolStripMenuItem
            // 
            this.CurveToolStripMenuItem.Name = "CurveToolStripMenuItem";
            this.CurveToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.CurveToolStripMenuItem.Text = "校准曲线";
            this.CurveToolStripMenuItem.Click += new System.EventHandler(this.CurveToolStripMenuItem_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 415);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "TVS 0.2.6";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Main_FormClosing);
            this.Load += new System.EventHandler(this.Main_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem FileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AccountToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem PrintToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ExitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ProjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ConnectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem SetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ReadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem RunToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ImportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem TableToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem StepToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ReportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem HelpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ReferToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem AboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem DebugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem FactToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CurveToolStripMenuItem;
    }
}

