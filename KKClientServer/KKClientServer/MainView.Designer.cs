namespace KKClientServer
{
    partial class MainView
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
            this.clientGB = new System.Windows.Forms.GroupBox();
            this.selectServerTB = new System.Windows.Forms.TextBox();
            this.sendFileTB = new System.Windows.Forms.TextBox();
            this.sendTextTB = new System.Windows.Forms.TextBox();
            this.sendFileLabel = new System.Windows.Forms.Label();
            this.sendTextLabel = new System.Windows.Forms.Label();
            this.logGB = new System.Windows.Forms.GroupBox();
            this.logLV = new System.Windows.Forms.ListView();
            this.logTimeCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.logMessageCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.connectionsGB = new System.Windows.Forms.GroupBox();
            this.fileTransferTabs = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.listView1 = new System.Windows.Forms.ListView();
            this.fileTransferIDCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fileTransferFileNameCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fileTransferProgressCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.enterIPLabel = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton2 = new System.Windows.Forms.ToolStripButton();
            this.browseFileBtn = new System.Windows.Forms.Button();
            this.sendBtn = new System.Windows.Forms.Button();
            this.clientConnectBtn = new System.Windows.Forms.Button();
            this.clientGB.SuspendLayout();
            this.logGB.SuspendLayout();
            this.connectionsGB.SuspendLayout();
            this.fileTransferTabs.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // clientGB
            // 
            this.clientGB.Controls.Add(this.enterIPLabel);
            this.clientGB.Controls.Add(this.selectServerTB);
            this.clientGB.Controls.Add(this.clientConnectBtn);
            this.clientGB.Location = new System.Drawing.Point(12, 12);
            this.clientGB.Name = "clientGB";
            this.clientGB.Size = new System.Drawing.Size(560, 56);
            this.clientGB.TabIndex = 1;
            this.clientGB.TabStop = false;
            this.clientGB.Text = "Client";
            // 
            // selectServerTB
            // 
            this.selectServerTB.Location = new System.Drawing.Point(88, 21);
            this.selectServerTB.Name = "selectServerTB";
            this.selectServerTB.Size = new System.Drawing.Size(428, 20);
            this.selectServerTB.TabIndex = 3;
            // 
            // sendFileTB
            // 
            this.sendFileTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sendFileTB.Enabled = false;
            this.sendFileTB.Location = new System.Drawing.Point(43, 215);
            this.sendFileTB.Name = "sendFileTB";
            this.sendFileTB.Size = new System.Drawing.Size(390, 20);
            this.sendFileTB.TabIndex = 6;
            // 
            // sendTextTB
            // 
            this.sendTextTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sendTextTB.Enabled = false;
            this.sendTextTB.Location = new System.Drawing.Point(43, 188);
            this.sendTextTB.Name = "sendTextTB";
            this.sendTextTB.Size = new System.Drawing.Size(419, 20);
            this.sendTextTB.TabIndex = 5;
            // 
            // sendFileLabel
            // 
            this.sendFileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sendFileLabel.AutoSize = true;
            this.sendFileLabel.Location = new System.Drawing.Point(6, 219);
            this.sendFileLabel.Name = "sendFileLabel";
            this.sendFileLabel.Size = new System.Drawing.Size(26, 13);
            this.sendFileLabel.TabIndex = 1;
            this.sendFileLabel.Text = "File:";
            // 
            // sendTextLabel
            // 
            this.sendTextLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sendTextLabel.AutoSize = true;
            this.sendTextLabel.Location = new System.Drawing.Point(6, 191);
            this.sendTextLabel.Name = "sendTextLabel";
            this.sendTextLabel.Size = new System.Drawing.Size(31, 13);
            this.sendTextLabel.TabIndex = 0;
            this.sendTextLabel.Text = "Text:";
            // 
            // logGB
            // 
            this.logGB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logGB.Controls.Add(this.logLV);
            this.logGB.Location = new System.Drawing.Point(12, 387);
            this.logGB.Name = "logGB";
            this.logGB.Size = new System.Drawing.Size(560, 160);
            this.logGB.TabIndex = 2;
            this.logGB.TabStop = false;
            this.logGB.Text = "Log";
            // 
            // logLV
            // 
            this.logLV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logLV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.logTimeCol,
            this.logMessageCol});
            this.logLV.Location = new System.Drawing.Point(16, 19);
            this.logLV.Name = "logLV";
            this.logLV.Size = new System.Drawing.Size(530, 123);
            this.logLV.TabIndex = 0;
            this.logLV.UseCompatibleStateImageBehavior = false;
            this.logLV.View = System.Windows.Forms.View.Details;
            // 
            // logTimeCol
            // 
            this.logTimeCol.Text = "Time";
            this.logTimeCol.Width = 120;
            // 
            // logMessageCol
            // 
            this.logMessageCol.Text = "Message";
            this.logMessageCol.Width = 406;
            // 
            // connectionsGB
            // 
            this.connectionsGB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.connectionsGB.Controls.Add(this.fileTransferTabs);
            this.connectionsGB.Location = new System.Drawing.Point(12, 74);
            this.connectionsGB.Name = "connectionsGB";
            this.connectionsGB.Size = new System.Drawing.Size(560, 307);
            this.connectionsGB.TabIndex = 3;
            this.connectionsGB.TabStop = false;
            this.connectionsGB.Text = "Connections";
            // 
            // fileTransferTabs
            // 
            this.fileTransferTabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTransferTabs.Controls.Add(this.tabPage1);
            this.fileTransferTabs.Enabled = false;
            this.fileTransferTabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileTransferTabs.ItemSize = new System.Drawing.Size(80, 20);
            this.fileTransferTabs.Location = new System.Drawing.Point(16, 19);
            this.fileTransferTabs.Name = "fileTransferTabs";
            this.fileTransferTabs.SelectedIndex = 0;
            this.fileTransferTabs.Size = new System.Drawing.Size(531, 270);
            this.fileTransferTabs.TabIndex = 1;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.toolStrip1);
            this.tabPage1.Controls.Add(this.listView1);
            this.tabPage1.Controls.Add(this.browseFileBtn);
            this.tabPage1.Controls.Add(this.sendFileTB);
            this.tabPage1.Controls.Add(this.sendFileLabel);
            this.tabPage1.Controls.Add(this.sendTextLabel);
            this.tabPage1.Controls.Add(this.sendTextTB);
            this.tabPage1.Controls.Add(this.sendBtn);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(523, 242);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.fileTransferIDCol,
            this.fileTransferFileNameCol,
            this.fileTransferProgressCol});
            this.listView1.Location = new System.Drawing.Point(9, 31);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(508, 151);
            this.listView1.TabIndex = 10;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // fileTransferIDCol
            // 
            this.fileTransferIDCol.Text = "ID";
            // 
            // fileTransferFileNameCol
            // 
            this.fileTransferFileNameCol.Text = "Filename";
            this.fileTransferFileNameCol.Width = 390;
            // 
            // fileTransferProgressCol
            // 
            this.fileTransferProgressCol.Text = "Progress";
            this.fileTransferProgressCol.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.fileTransferProgressCol.Width = 55;
            // 
            // enterIPLabel
            // 
            this.enterIPLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.enterIPLabel.AutoSize = true;
            this.enterIPLabel.Location = new System.Drawing.Point(13, 25);
            this.enterIPLabel.Name = "enterIPLabel";
            this.enterIPLabel.Size = new System.Drawing.Size(69, 13);
            this.enterIPLabel.TabIndex = 4;
            this.enterIPLabel.Text = "Enter adress:";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolStripSeparator1,
            this.toolStripButton2});
            this.toolStrip1.Location = new System.Drawing.Point(3, 3);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(517, 25);
            this.toolStrip1.TabIndex = 12;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton1.Image = global::KKClientServer.Properties.Resources.disconnect16;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton1.Text = "toolStripButton1";
            // 
            // toolStripButton2
            // 
            this.toolStripButton2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButton2.Image = global::KKClientServer.Properties.Resources.clear16;
            this.toolStripButton2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton2.Name = "toolStripButton2";
            this.toolStripButton2.Size = new System.Drawing.Size(23, 22);
            this.toolStripButton2.Text = "toolStripButton2";
            // 
            // browseFileBtn
            // 
            this.browseFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.browseFileBtn.Enabled = false;
            this.browseFileBtn.Image = global::KKClientServer.Properties.Resources.browse16;
            this.browseFileBtn.Location = new System.Drawing.Point(439, 213);
            this.browseFileBtn.Name = "browseFileBtn";
            this.browseFileBtn.Size = new System.Drawing.Size(24, 24);
            this.browseFileBtn.TabIndex = 9;
            this.browseFileBtn.UseVisualStyleBackColor = true;
            // 
            // sendBtn
            // 
            this.sendBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendBtn.Enabled = false;
            this.sendBtn.Image = global::KKClientServer.Properties.Resources.send32;
            this.sendBtn.Location = new System.Drawing.Point(468, 187);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(50, 50);
            this.sendBtn.TabIndex = 7;
            this.sendBtn.UseVisualStyleBackColor = true;
            // 
            // clientConnectBtn
            // 
            this.clientConnectBtn.Image = global::KKClientServer.Properties.Resources.connect16;
            this.clientConnectBtn.Location = new System.Drawing.Point(522, 19);
            this.clientConnectBtn.Name = "clientConnectBtn";
            this.clientConnectBtn.Size = new System.Drawing.Size(24, 24);
            this.clientConnectBtn.TabIndex = 2;
            this.clientConnectBtn.UseVisualStyleBackColor = true;
            this.clientConnectBtn.Click += new System.EventHandler(this.onConnectBtn_Click);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 561);
            this.Controls.Add(this.connectionsGB);
            this.Controls.Add(this.logGB);
            this.Controls.Add(this.clientGB);
            this.Name = "MainView";
            this.Text = "KKClientServer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.onMainView_Closing);
            this.Shown += new System.EventHandler(this.onMainView_Shown);
            this.clientGB.ResumeLayout(false);
            this.clientGB.PerformLayout();
            this.logGB.ResumeLayout(false);
            this.connectionsGB.ResumeLayout(false);
            this.fileTransferTabs.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox clientGB;
        private System.Windows.Forms.TextBox sendFileTB;
        private System.Windows.Forms.TextBox sendTextTB;
        private System.Windows.Forms.TextBox selectServerTB;
        private System.Windows.Forms.Button clientConnectBtn;
        private System.Windows.Forms.Label sendFileLabel;
        private System.Windows.Forms.Label sendTextLabel;
        private System.Windows.Forms.Button browseFileBtn;
        private System.Windows.Forms.Button sendBtn;
        private System.Windows.Forms.GroupBox logGB;
        private System.Windows.Forms.ListView logLV;
        private System.Windows.Forms.ColumnHeader logTimeCol;
        private System.Windows.Forms.ColumnHeader logMessageCol;
        private System.Windows.Forms.GroupBox connectionsGB;
        private System.Windows.Forms.TabControl fileTransferTabs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader fileTransferIDCol;
        private System.Windows.Forms.ColumnHeader fileTransferFileNameCol;
        private System.Windows.Forms.ColumnHeader fileTransferProgressCol;
        private System.Windows.Forms.Label enterIPLabel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton2;
    }
}

