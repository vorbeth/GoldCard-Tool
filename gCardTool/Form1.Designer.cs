namespace gCardTool
{
    partial class Form1
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
            this.button1 = new System.Windows.Forms.Button();
            this.txtMmcCid = new System.Windows.Forms.TextBox();
            this.btnGetCid = new System.Windows.Forms.Button();
            this.cmbMmc = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.btnRefreshMmc = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.radMMC0 = new System.Windows.Forms.RadioButton();
            this.radMMC1 = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtDbgOut = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(6, 75);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(113, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Patch MMC";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtMmcCid
            // 
            this.txtMmcCid.Location = new System.Drawing.Point(87, 21);
            this.txtMmcCid.Name = "txtMmcCid";
            this.txtMmcCid.Size = new System.Drawing.Size(258, 20);
            this.txtMmcCid.TabIndex = 2;
            // 
            // btnGetCid
            // 
            this.btnGetCid.Location = new System.Drawing.Point(6, 19);
            this.btnGetCid.Name = "btnGetCid";
            this.btnGetCid.Size = new System.Drawing.Size(75, 23);
            this.btnGetCid.TabIndex = 3;
            this.btnGetCid.Text = "Get CID";
            this.btnGetCid.UseVisualStyleBackColor = true;
            this.btnGetCid.Click += new System.EventHandler(this.btnGetCid_Click);
            // 
            // cmbMmc
            // 
            this.cmbMmc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMmc.FormattingEnabled = true;
            this.cmbMmc.Location = new System.Drawing.Point(72, 19);
            this.cmbMmc.Name = "cmbMmc";
            this.cmbMmc.Size = new System.Drawing.Size(192, 21);
            this.cmbMmc.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "HTC MMC:";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(6, 46);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(113, 23);
            this.button2.TabIndex = 6;
            this.button2.Text = "Load GoldCard.img";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnRefreshMmc
            // 
            this.btnRefreshMmc.Location = new System.Drawing.Point(270, 17);
            this.btnRefreshMmc.Name = "btnRefreshMmc";
            this.btnRefreshMmc.Size = new System.Drawing.Size(75, 23);
            this.btnRefreshMmc.TabIndex = 7;
            this.btnRefreshMmc.Text = "Refresh";
            this.btnRefreshMmc.UseVisualStyleBackColor = true;
            this.btnRefreshMmc.Click += new System.EventHandler(this.btnRefreshMmc_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.radMMC0);
            this.groupBox1.Controls.Add(this.radMMC1);
            this.groupBox1.Controls.Add(this.txtMmcCid);
            this.groupBox1.Controls.Add(this.btnGetCid);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(353, 75);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ADB Tools";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(36, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Select MMC Device (MMC1 is default)";
            // 
            // radMMC0
            // 
            this.radMMC0.AutoSize = true;
            this.radMMC0.Location = new System.Drawing.Point(291, 47);
            this.radMMC0.Name = "radMMC0";
            this.radMMC0.Size = new System.Drawing.Size(56, 17);
            this.radMMC0.TabIndex = 5;
            this.radMMC0.Text = "MMC0";
            this.radMMC0.UseVisualStyleBackColor = true;
            // 
            // radMMC1
            // 
            this.radMMC1.AutoSize = true;
            this.radMMC1.Checked = true;
            this.radMMC1.Location = new System.Drawing.Point(229, 47);
            this.radMMC1.Name = "radMMC1";
            this.radMMC1.Size = new System.Drawing.Size(56, 17);
            this.radMMC1.TabIndex = 4;
            this.radMMC1.TabStop = true;
            this.radMMC1.Text = "MMC1";
            this.radMMC1.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbMmc);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnRefreshMmc);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Location = new System.Drawing.Point(12, 140);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(353, 107);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "MMC Tools";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.linkLabel1);
            this.groupBox3.Location = new System.Drawing.Point(12, 93);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(353, 41);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "GoldCard Gen";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(52, 16);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(237, 13);
            this.linkLabel1.TabIndex = 0;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Open revskills.de Free HTC Gold Card Generator";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(265, 372);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(100, 13);
            this.linkLabel2.TabIndex = 12;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "www.vorbeth.co.uk";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtDbgOut);
            this.groupBox4.Location = new System.Drawing.Point(12, 253);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(353, 116);
            this.groupBox4.TabIndex = 13;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Debug Log";
            // 
            // txtDbgOut
            // 
            this.txtDbgOut.Location = new System.Drawing.Point(6, 19);
            this.txtDbgOut.Multiline = true;
            this.txtDbgOut.Name = "txtDbgOut";
            this.txtDbgOut.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDbgOut.Size = new System.Drawing.Size(341, 91);
            this.txtDbgOut.TabIndex = 0;
            this.txtDbgOut.TextChanged += new System.EventHandler(this.txtDbgOut_TextChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(377, 394);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GoldCard Tool";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.formClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtMmcCid;
        private System.Windows.Forms.Button btnGetCid;
        private System.Windows.Forms.ComboBox cmbMmc;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnRefreshMmc;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.LinkLabel linkLabel2;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox txtDbgOut;
        private System.Windows.Forms.RadioButton radMMC0;
        private System.Windows.Forms.RadioButton radMMC1;
        private System.Windows.Forms.Label label2;
    }
}

