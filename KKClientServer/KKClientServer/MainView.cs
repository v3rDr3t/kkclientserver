using KKClientServer.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KKClientServer {

    public partial class MainView : Form {
        #region Fields
        private Controller controller;
        private int tabId = 1;
        #endregion

        public MainView() {
            InitializeComponent();
        }

        /// <summary>
        /// Adds the given <see cref="Controller"/> to the main view.
        /// </summary>
        /// <param name="controller">The controller.</param>
        internal void AddController(Controller controller) {
            this.controller = controller;
        }

        private void onMainView_Closing(object sender, FormClosingEventArgs e) {
            // TODO: stop TCP listener
            Print("Stopping server...");

            // TODO: misc cleanup
        }

        private void onConnectBtn_Click(object sender, EventArgs e) {
            string serverAddress = this.selectServerTB.Text;
            bool validInput = true;
            if (validInput /* TODO: Input validation */) {
                //serverAddress = "192.168.13." + tabId; // TEST
                Print("Connecting to \"" + serverAddress + "\"...");
                controller.ConnectTo(serverAddress);
                //AddConnectionTab(serverAddress, "Host" + (tabId++)); // TEST
            } else {
                Print("Invalid server address (" + serverAddress + ").");
            }
        }

        private void onSendTextBtn_Click(object sender, EventArgs e) {
            if (!this.sendTextTB.Text.Equals("")) {
                // TODO: Send text message
                Print("Initiate sending text \"" + this.sendTextTB.Text + "\"...");
            }
        }

        private void onBrowseFileBtn_Click(object sender, EventArgs e) {
            using (OpenFileDialog fileBrowser = new OpenFileDialog()) {
                fileBrowser.Filter = "All Files (*.*)|*.*";
                if (fileBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    this.sendFileTB.Text = fileBrowser.FileName;
                }
            }
        }

        private void onSendFileBtn_Click(object sender, EventArgs e) {
            if (File.Exists(this.sendFileTB.Text)) {
                // TODO: Send file
                Print("Initiate sending file \"" + this.sendFileTB.Text + "\"...");
            }
        }

        internal void AddConnectionTab(string ip, string hostName) {
            TabPage newTabPage = createConnectionTab(ip, hostName);
            if (this.fileTransferTabs.InvokeRequired) {
                AddTab_Callback callback = new AddTab_Callback(AddConnectionTab);
                this.Invoke(callback, new object[] { ip, hostName });
            } else {
                this.fileTransferTabs.TabPages.Add(newTabPage);
                Print("Connected to \"" + hostName + "\" (" + ip + ").");
            }
        }

        private TabPage createConnectionTab(string id, string text) {
            // create new tab
            TabPage tabPage = new TabPage();
            tabPage.Location = new Point(4, 24);
            tabPage.Name = id;
            tabPage.Padding = new Padding(3);
            tabPage.Size = new Size(523, 186);
            tabPage.TabIndex = 0;
            tabPage.Text = text;
            tabPage.UseVisualStyleBackColor = true;

            // create tool strip button control
            ToolStripButton toolStripButton1 = new ToolStripButton();
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.Image = Properties.Resources.disconnect16;
            toolStripButton1.ImageTransparentColor = Color.Magenta;
            toolStripButton1.Name = id;
            toolStripButton1.Size = new Size(23, 22);
            toolStripButton1.Text = "Disconnect";
            toolStripButton1.Click += new EventHandler(disconnectBtn_Click);

            // create tool strip separator control
            ToolStripSeparator toolStripSeparator = new ToolStripSeparator();
            toolStripSeparator.Name = id;
            toolStripSeparator.Size = new Size(6, 25);

            // create tool strip button control
            ToolStripButton toolStripButton2 = new ToolStripButton();
            toolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton2.Image = Properties.Resources.clear16;
            toolStripButton2.ImageTransparentColor = Color.Magenta;
            toolStripButton2.Name = id;
            toolStripButton2.Size = new Size(23, 22);
            toolStripButton2.Text = "Clear";
            toolStripButton2.Click += new EventHandler(clearBtn_Click);

            // create tool strip control
            ToolStrip toolStrip = new ToolStrip();
            toolStrip.Location = new Point(3, 3);
            toolStrip.Name = id;
            toolStrip.Size = new Size(517, 25);
            toolStrip.TabIndex = 12;
            toolStrip.Text = "toolStrip";
            toolStrip.Items.AddRange(new ToolStripItem[] {
            toolStripButton1,
            toolStripSeparator,
            toolStripButton2});

            // create column controls
            ColumnHeader fileTransferIDCol = ((ColumnHeader)(new ColumnHeader()));
            fileTransferIDCol.Text = "ID";

            ColumnHeader fileTransferFileNameCol = ((ColumnHeader)(new ColumnHeader()));
            fileTransferFileNameCol.Text = "Filename";
            fileTransferFileNameCol.Width = 390;

            ColumnHeader fileTransferProgressCol = ((ColumnHeader)(new ColumnHeader()));
            fileTransferProgressCol.Text = "Progress";
            fileTransferProgressCol.TextAlign = HorizontalAlignment.Right;
            fileTransferProgressCol.Width = 55;

            // create list view control
            ListView listView = new ListView();
            listView.Anchor = ((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right));
            listView.Location = new Point(3, 31);
            listView.Name = id;
            listView.Size = new Size(517, 149);
            listView.TabIndex = 10;
            listView.UseCompatibleStateImageBehavior = false;
            listView.View = View.Details;
            listView.Columns.AddRange(new ColumnHeader[] {
            fileTransferIDCol,
            fileTransferFileNameCol,
            fileTransferProgressCol});

            // add controls
            tabPage.Controls.Add(toolStrip);
            tabPage.Controls.Add(listView);
            return tabPage;
        }

        private void disconnectBtn_Click(Object sender, EventArgs e) {
            string hostAddress = ((ToolStripButton)sender).Name;
            Print("Disconnecting from \"" + hostAddress + "\".");
            controller.DisconnectFrom(hostAddress);
        }

        private void clearBtn_Click(Object sender, EventArgs e) {
            string origin = ((ToolStripButton)sender).Name;
            Print("Clearing finished uploads to \"" + origin + "\"...");
            // TODO
        }


        internal void RemoveConnectionTab(string ip) {
            TabPage tabPage = this.fileTransferTabs.TabPages[ip];
            if (this.fileTransferTabs.InvokeRequired) {
                RemoveTab_Callback callback = new RemoveTab_Callback(RemoveConnectionTab);
                this.Invoke(callback, new object[] { ip });
            } else {
                if (tabPage != null) {
                    this.fileTransferTabs.TabPages.Remove(tabPage);
                    Print("Disconnected from \"Name\" (" + ip + ").");
                }
            }
        }
    
        internal void Print(string msg) {
            string[] rowInfo = new string[2];
            rowInfo[0] = DateTime.Now.ToString(Constants.LOG_DATETIME_FORMAT);
            rowInfo[1] = msg;

            if (this.logLV.InvokeRequired) {
                Print_Callback callback = new Print_Callback(Print);
                this.Invoke(callback, new object[] { msg });
            } else {
                this.logLV.Items.Add(new ListViewItem(rowInfo));
            }
        }

        delegate void Print_Callback(string msg);
        delegate void AddTab_Callback(string ip, string hostName);
        delegate void RemoveTab_Callback(string ip);
    }
}
