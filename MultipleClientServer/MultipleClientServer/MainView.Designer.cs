namespace ClientServer {
    partial class MainView {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (components != null)
                    components.Dispose();
                controller.Exiting();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            this.clientGB = new System.Windows.Forms.GroupBox();
            this.enterIPLabel = new System.Windows.Forms.Label();
            this.selectServerTB = new System.Windows.Forms.TextBox();
            this.clientConnectBtn = new System.Windows.Forms.Button();
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
            this.browseFileBtn = new System.Windows.Forms.Button();
            this.sendBtn = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.clientGB.SuspendLayout();
            this.logGB.SuspendLayout();
            this.connectionsGB.SuspendLayout();
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
            // selectServerTB
            // 
            this.selectServerTB.Location = new System.Drawing.Point(88, 21);
            this.selectServerTB.Name = "selectServerTB";
            this.selectServerTB.Size = new System.Drawing.Size(428, 20);
            this.selectServerTB.TabIndex = 3;
            // 
            // clientConnectBtn
            // 
            this.clientConnectBtn.Image = global::MultipleClientServer.Properties.Resources.connect16;
            this.clientConnectBtn.Location = new System.Drawing.Point(522, 19);
            this.clientConnectBtn.Name = "clientConnectBtn";
            this.clientConnectBtn.Size = new System.Drawing.Size(24, 24);
            this.clientConnectBtn.TabIndex = 2;
            this.toolTip.SetToolTip(this.clientConnectBtn, "Connect");
            this.clientConnectBtn.UseVisualStyleBackColor = true;
            this.clientConnectBtn.Click += new System.EventHandler(this.onConnectBtn_Click);
            // 
            // sendFileTB
            // 
            this.sendFileTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sendFileTB.Location = new System.Drawing.Point(50, 272);
            this.sendFileTB.Name = "sendFileTB";
            this.sendFileTB.Size = new System.Drawing.Size(410, 20);
            this.sendFileTB.TabIndex = 6;
            // 
            // sendTextTB
            // 
            this.sendTextTB.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sendTextTB.Location = new System.Drawing.Point(50, 244);
            this.sendTextTB.Name = "sendTextTB";
            this.sendTextTB.Size = new System.Drawing.Size(440, 20);
            this.sendTextTB.TabIndex = 5;
            // 
            // sendFileLabel
            // 
            this.sendFileLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sendFileLabel.AutoSize = true;
            this.sendFileLabel.Location = new System.Drawing.Point(13, 275);
            this.sendFileLabel.Name = "sendFileLabel";
            this.sendFileLabel.Size = new System.Drawing.Size(26, 13);
            this.sendFileLabel.TabIndex = 1;
            this.sendFileLabel.Text = "File:";
            // 
            // sendTextLabel
            // 
            this.sendTextLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.sendTextLabel.AutoSize = true;
            this.sendTextLabel.Location = new System.Drawing.Point(13, 247);
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
            this.connectionsGB.Controls.Add(this.sendTextTB);
            this.connectionsGB.Controls.Add(this.browseFileBtn);
            this.connectionsGB.Controls.Add(this.sendBtn);
            this.connectionsGB.Controls.Add(this.sendFileTB);
            this.connectionsGB.Controls.Add(this.sendTextLabel);
            this.connectionsGB.Controls.Add(this.sendFileLabel);
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
            this.fileTransferTabs.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileTransferTabs.ItemSize = new System.Drawing.Size(80, 20);
            this.fileTransferTabs.Location = new System.Drawing.Point(16, 19);
            this.fileTransferTabs.Name = "fileTransferTabs";
            this.fileTransferTabs.SelectedIndex = 0;
            this.fileTransferTabs.Size = new System.Drawing.Size(531, 214);
            this.fileTransferTabs.TabIndex = 1;
            // 
            // browseFileBtn
            // 
            this.browseFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.browseFileBtn.Image = global::MultipleClientServer.Properties.Resources.browse16;
            this.browseFileBtn.Location = new System.Drawing.Point(466, 270);
            this.browseFileBtn.Name = "browseFileBtn";
            this.browseFileBtn.Size = new System.Drawing.Size(24, 24);
            this.browseFileBtn.TabIndex = 9;
            this.toolTip.SetToolTip(this.browseFileBtn, "Browse");
            this.browseFileBtn.UseVisualStyleBackColor = true;
            this.browseFileBtn.Click += new System.EventHandler(this.onBrowseFileBtn_Click);
            // 
            // sendBtn
            // 
            this.sendBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.sendBtn.Image = global::MultipleClientServer.Properties.Resources.send32;
            this.sendBtn.Location = new System.Drawing.Point(496, 244);
            this.sendBtn.Name = "sendBtn";
            this.sendBtn.Size = new System.Drawing.Size(50, 50);
            this.sendBtn.TabIndex = 7;
            this.toolTip.SetToolTip(this.sendBtn, "Send");
            this.sendBtn.UseVisualStyleBackColor = true;
            this.sendBtn.Click += new System.EventHandler(this.onSendBtn_Click);
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
            this.Text = "MultipleClientServer";
            this.clientGB.ResumeLayout(false);
            this.clientGB.PerformLayout();
            this.logGB.ResumeLayout(false);
            this.connectionsGB.ResumeLayout(false);
            this.connectionsGB.PerformLayout();
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
        private System.Windows.Forms.Label enterIPLabel;
        private System.Windows.Forms.ToolTip toolTip;
    }
}

