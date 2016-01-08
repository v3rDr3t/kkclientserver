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
            this.serverGB = new System.Windows.Forms.GroupBox();
            this.serverStopBtn = new System.Windows.Forms.Button();
            this.serverStartBtn = new System.Windows.Forms.Button();
            this.clientGB = new System.Windows.Forms.GroupBox();
            this.browseFileBtn = new System.Windows.Forms.Button();
            this.sendFileBtn = new System.Windows.Forms.Button();
            this.sendTextBtn = new System.Windows.Forms.Button();
            this.sendFileTB = new System.Windows.Forms.TextBox();
            this.sendTextTB = new System.Windows.Forms.TextBox();
            this.selectServerLabel = new System.Windows.Forms.Label();
            this.selectServerTB = new System.Windows.Forms.TextBox();
            this.clientConnectBtn = new System.Windows.Forms.Button();
            this.sendFileLabel = new System.Windows.Forms.Label();
            this.sendTextLabel = new System.Windows.Forms.Label();
            this.logGB = new System.Windows.Forms.GroupBox();
            this.logLV = new System.Windows.Forms.ListView();
            this.logTimeCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.logMessageCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.fileTransferGB = new System.Windows.Forms.GroupBox();
            this.fileTransferTabs = new System.Windows.Forms.TabControl();
            this.serverGB.SuspendLayout();
            this.clientGB.SuspendLayout();
            this.logGB.SuspendLayout();
            this.fileTransferGB.SuspendLayout();
            this.SuspendLayout();
            // 
            // serverGB
            // 
            this.serverGB.Controls.Add(this.serverStopBtn);
            this.serverGB.Controls.Add(this.serverStartBtn);
            this.serverGB.Location = new System.Drawing.Point(12, 12);
            this.serverGB.Name = "serverGB";
            this.serverGB.Size = new System.Drawing.Size(300, 56);
            this.serverGB.TabIndex = 0;
            this.serverGB.TabStop = false;
            this.serverGB.Text = "Server";
            // 
            // serverStopBtn
            // 
            this.serverStopBtn.Enabled = false;
            this.serverStopBtn.Location = new System.Drawing.Point(164, 19);
            this.serverStopBtn.Name = "serverStopBtn";
            this.serverStopBtn.Size = new System.Drawing.Size(100, 22);
            this.serverStopBtn.TabIndex = 1;
            this.serverStopBtn.Text = "Stop";
            this.serverStopBtn.UseVisualStyleBackColor = true;
            this.serverStopBtn.Click += new System.EventHandler(this.onServerStopBtn_Click);
            // 
            // serverStartBtn
            // 
            this.serverStartBtn.Location = new System.Drawing.Point(36, 19);
            this.serverStartBtn.Name = "serverStartBtn";
            this.serverStartBtn.Size = new System.Drawing.Size(100, 22);
            this.serverStartBtn.TabIndex = 0;
            this.serverStartBtn.Text = "Start";
            this.serverStartBtn.UseVisualStyleBackColor = true;
            this.serverStartBtn.Click += new System.EventHandler(this.onServerStartBtn_Click);
            // 
            // clientGB
            // 
            this.clientGB.Controls.Add(this.browseFileBtn);
            this.clientGB.Controls.Add(this.sendFileBtn);
            this.clientGB.Controls.Add(this.sendTextBtn);
            this.clientGB.Controls.Add(this.sendFileTB);
            this.clientGB.Controls.Add(this.sendTextTB);
            this.clientGB.Controls.Add(this.selectServerLabel);
            this.clientGB.Controls.Add(this.selectServerTB);
            this.clientGB.Controls.Add(this.clientConnectBtn);
            this.clientGB.Controls.Add(this.sendFileLabel);
            this.clientGB.Controls.Add(this.sendTextLabel);
            this.clientGB.Location = new System.Drawing.Point(12, 74);
            this.clientGB.Name = "clientGB";
            this.clientGB.Size = new System.Drawing.Size(300, 195);
            this.clientGB.TabIndex = 1;
            this.clientGB.TabStop = false;
            this.clientGB.Text = "Client";
            // 
            // browseFileBtn
            // 
            this.browseFileBtn.Enabled = false;
            this.browseFileBtn.Location = new System.Drawing.Point(233, 154);
            this.browseFileBtn.Name = "browseFileBtn";
            this.browseFileBtn.Size = new System.Drawing.Size(24, 22);
            this.browseFileBtn.TabIndex = 9;
            this.browseFileBtn.Text = "...";
            this.browseFileBtn.UseVisualStyleBackColor = true;
            this.browseFileBtn.Click += new System.EventHandler(this.onBrowseFileBtn_Click);
            // 
            // sendFileBtn
            // 
            this.sendFileBtn.Enabled = false;
            this.sendFileBtn.Location = new System.Drawing.Point(263, 154);
            this.sendFileBtn.Name = "sendFileBtn";
            this.sendFileBtn.Size = new System.Drawing.Size(24, 22);
            this.sendFileBtn.TabIndex = 8;
            this.sendFileBtn.Text = ">";
            this.sendFileBtn.UseVisualStyleBackColor = true;
            this.sendFileBtn.Click += new System.EventHandler(this.onSendFileBtn_Click);
            // 
            // sendTextBtn
            // 
            this.sendTextBtn.Enabled = false;
            this.sendTextBtn.Location = new System.Drawing.Point(233, 98);
            this.sendTextBtn.Name = "sendTextBtn";
            this.sendTextBtn.Size = new System.Drawing.Size(54, 22);
            this.sendTextBtn.TabIndex = 7;
            this.sendTextBtn.Text = ">";
            this.sendTextBtn.UseVisualStyleBackColor = true;
            this.sendTextBtn.Click += new System.EventHandler(this.onSendTextBtn_Click);
            // 
            // sendFileTB
            // 
            this.sendFileTB.Enabled = false;
            this.sendFileTB.Location = new System.Drawing.Point(14, 155);
            this.sendFileTB.Name = "sendFileTB";
            this.sendFileTB.Size = new System.Drawing.Size(213, 20);
            this.sendFileTB.TabIndex = 6;
            // 
            // sendTextTB
            // 
            this.sendTextTB.Enabled = false;
            this.sendTextTB.Location = new System.Drawing.Point(14, 99);
            this.sendTextTB.Name = "sendTextTB";
            this.sendTextTB.Size = new System.Drawing.Size(213, 20);
            this.sendTextTB.TabIndex = 5;
            // 
            // selectServerLabel
            // 
            this.selectServerLabel.AutoSize = true;
            this.selectServerLabel.Location = new System.Drawing.Point(11, 26);
            this.selectServerLabel.Name = "selectServerLabel";
            this.selectServerLabel.Size = new System.Drawing.Size(72, 13);
            this.selectServerLabel.TabIndex = 4;
            this.selectServerLabel.Text = "Select server:";
            // 
            // selectServerTB
            // 
            this.selectServerTB.Location = new System.Drawing.Point(14, 43);
            this.selectServerTB.Name = "selectServerTB";
            this.selectServerTB.Size = new System.Drawing.Size(213, 20);
            this.selectServerTB.TabIndex = 3;
            // 
            // clientConnectBtn
            // 
            this.clientConnectBtn.Location = new System.Drawing.Point(233, 42);
            this.clientConnectBtn.Name = "clientConnectBtn";
            this.clientConnectBtn.Size = new System.Drawing.Size(54, 22);
            this.clientConnectBtn.TabIndex = 2;
            this.clientConnectBtn.Text = "C";
            this.clientConnectBtn.UseVisualStyleBackColor = true;
            this.clientConnectBtn.Click += new System.EventHandler(this.onConnectBtn_Click);
            // 
            // sendFileLabel
            // 
            this.sendFileLabel.AutoSize = true;
            this.sendFileLabel.Location = new System.Drawing.Point(11, 139);
            this.sendFileLabel.Name = "sendFileLabel";
            this.sendFileLabel.Size = new System.Drawing.Size(51, 13);
            this.sendFileLabel.TabIndex = 1;
            this.sendFileLabel.Text = "Send file:";
            // 
            // sendTextLabel
            // 
            this.sendTextLabel.AutoSize = true;
            this.sendTextLabel.Location = new System.Drawing.Point(11, 83);
            this.sendTextLabel.Name = "sendTextLabel";
            this.sendTextLabel.Size = new System.Drawing.Size(100, 13);
            this.sendTextLabel.TabIndex = 0;
            this.sendTextLabel.Text = "Send text message:";
            // 
            // logGB
            // 
            this.logGB.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logGB.Controls.Add(this.logLV);
            this.logGB.Location = new System.Drawing.Point(12, 275);
            this.logGB.Name = "logGB";
            this.logGB.Size = new System.Drawing.Size(660, 154);
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
            this.logLV.Location = new System.Drawing.Point(14, 22);
            this.logLV.Name = "logLV";
            this.logLV.Size = new System.Drawing.Size(632, 114);
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
            this.logMessageCol.Width = 508;
            // 
            // fileTransferGB
            // 
            this.fileTransferGB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTransferGB.Controls.Add(this.fileTransferTabs);
            this.fileTransferGB.Location = new System.Drawing.Point(318, 12);
            this.fileTransferGB.Name = "fileTransferGB";
            this.fileTransferGB.Size = new System.Drawing.Size(354, 257);
            this.fileTransferGB.TabIndex = 3;
            this.fileTransferGB.TabStop = false;
            this.fileTransferGB.Text = "File Transfer";
            // 
            // fileTransferTabs
            // 
            this.fileTransferTabs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fileTransferTabs.Enabled = false;
            this.fileTransferTabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileTransferTabs.ItemSize = new System.Drawing.Size(80, 20);
            this.fileTransferTabs.Location = new System.Drawing.Point(16, 19);
            this.fileTransferTabs.Name = "fileTransferTabs";
            this.fileTransferTabs.SelectedIndex = 0;
            this.fileTransferTabs.Size = new System.Drawing.Size(324, 219);
            this.fileTransferTabs.TabIndex = 1;
            this.fileTransferTabs.MouseDown += new System.Windows.Forms.MouseEventHandler(this.onFileTransferTabs_MouseDown);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 441);
            this.Controls.Add(this.fileTransferGB);
            this.Controls.Add(this.logGB);
            this.Controls.Add(this.clientGB);
            this.Controls.Add(this.serverGB);
            this.Name = "MainView";
            this.Text = "KKClientServer";
            this.serverGB.ResumeLayout(false);
            this.clientGB.ResumeLayout(false);
            this.clientGB.PerformLayout();
            this.logGB.ResumeLayout(false);
            this.fileTransferGB.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox serverGB;
        private System.Windows.Forms.Button serverStopBtn;
        private System.Windows.Forms.Button serverStartBtn;
        private System.Windows.Forms.GroupBox clientGB;
        private System.Windows.Forms.TextBox sendFileTB;
        private System.Windows.Forms.TextBox sendTextTB;
        private System.Windows.Forms.Label selectServerLabel;
        private System.Windows.Forms.TextBox selectServerTB;
        private System.Windows.Forms.Button clientConnectBtn;
        private System.Windows.Forms.Label sendFileLabel;
        private System.Windows.Forms.Label sendTextLabel;
        private System.Windows.Forms.Button browseFileBtn;
        private System.Windows.Forms.Button sendFileBtn;
        private System.Windows.Forms.Button sendTextBtn;
        private System.Windows.Forms.GroupBox logGB;
        private System.Windows.Forms.ListView logLV;
        private System.Windows.Forms.ColumnHeader logTimeCol;
        private System.Windows.Forms.ColumnHeader logMessageCol;
        private System.Windows.Forms.GroupBox fileTransferGB;
        private System.Windows.Forms.TabControl fileTransferTabs;
    }
}

