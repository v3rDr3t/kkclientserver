﻿using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ClientServer {

    public partial class MainView : Form {
        #region Fields
        private Controller controller;
        #endregion

        /// <summary>
        /// Constructs a <see cref="MainView"/> object.
        /// </summary>
        public MainView() {
            InitializeComponent();
        }

        /// <summary>
        /// Adds the given controller to the main view.
        /// </summary>
        /// <param name="controller">The controller.</param>
        internal void AddController(Controller controller) {
            this.controller = controller;
        }

        /// <summary>
        /// Callback for 'Connect' button clicked.
        /// Initiates connection to a specified host.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void onConnectBtn_Click(object sender, EventArgs e) {
            string serverAddress = this.selectServerTB.Text;
            Print("Connecting to \"" + serverAddress + "\"...");
            controller.ConnectTo(serverAddress);
        }

        /// <summary>
        /// Callback for 'Browse' button clicked.
        /// Initiates file selection.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void onBrowseFileBtn_Click(object sender, EventArgs e) {
            using (OpenFileDialog fileBrowser = new OpenFileDialog()) {
                fileBrowser.Filter = "All Files (*.*)|*.*";
                if (fileBrowser.ShowDialog() == DialogResult.OK) {
                    this.sendFileTB.Text = fileBrowser.FileName;
                }
            }
        }

        /// <summary>
        /// Callback for 'Send' button clicked.
        /// Initiates message or file transfer to a specified (connected) host.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void onSendBtn_Click(object sender, EventArgs e) {
            // get selected client
            TabPage clientPage = this.fileTransferTabs.SelectedTab;
            if (clientPage != null) {
                string hostAddress = this.fileTransferTabs.SelectedTab.Name;
                
                // send text message
                if (!this.sendTextTB.Text.Equals("")) {
                    Print("Initiate sending text \"" + this.sendTextTB.Text + "\"...");
                    controller.SendMessageTo(hostAddress, this.sendTextTB.Text);
                }

                // send file
                if (File.Exists(this.sendFileTB.Text)) {
                    initiateSendFile(hostAddress);
                } else {
                    if (!this.sendFileTB.Text.Equals("")) {
                        Print("File \"" + this.sendFileTB.Text + "\" does not exist");
                    }
                }
            }
            this.sendTextTB.Clear();
            this.sendFileTB.Clear();
        }

        /// <summary>
        /// Initiates file transfer to a given host address.
        /// </summary>
        /// <param name="hostAddress">The host address.</param>
        private void initiateSendFile(string hostAddress) {
            FileInfo fileInfo = new FileInfo(this.sendFileTB.Text);
            if (fileInfo.Name.Length < int.MaxValue - Constants.PREFIX_SIZE) {
                // add item to list view
                TabPage tabPage = this.fileTransferTabs.TabPages[hostAddress];
                ListView listView = (ListView)tabPage.Controls["lv_" + hostAddress];
                int idx = listView.Items.Count + 1;
                string[] row = { idx.ToString(), Path.GetFileName(this.sendFileTB.Text), "0%" };
                var item = new ListViewItem(row);
                listView.Items.Add(item);
                // send file
                Print("Initiate sending file \"" + this.sendFileTB.Text + "\"...");
                controller.SendFileTo(hostAddress, fileInfo);
            } else {
                Print("File name \"" + this.sendFileTB.Text + "\" mustn't be longer than "
                    + (int.MaxValue - Constants.PREFIX_SIZE) + " characters!");
            }
        }

        /// <summary>
        /// Visually updates the progress of a file transfer.
        /// </summary>
        /// <param name="ip">The host address.</param>
        /// <param name="fileName">The file name.</param>
        /// <param name="progress">The progress.</param>
        internal void UpdateProgressOnFile(string ip, string fileName, double progress) {
            // get progress string
            int prog = Convert.ToInt32(progress * 100);
            string progressString = prog + "%";

            // handle main view modification from another thread
            try {
                if (this.fileTransferTabs.InvokeRequired) {
                    UpdateProgressOnFile_Callback callback = new UpdateProgressOnFile_Callback(UpdateProgressOnFile);
                    this.Invoke(callback, new object[] { ip, fileName, progress });
                } else {
                    TabPage tabPage = this.fileTransferTabs.TabPages[ip];
                    if (tabPage != null) {
                        ListView listView = (ListView)tabPage.Controls["lv_" + ip];
                        foreach (ListViewItem item in listView.Items) {
                            if (item.SubItems[1].Text.Equals(fileName) && !item.SubItems[2].Text.Equals("100%")) {
                                if (!item.SubItems[2].Text.Equals(progressString)) {
                                    item.SubItems[2].Text = progressString;
                                }
                            }
                        }
                    }
                }
            } catch (ObjectDisposedException) {
                // should only be caused on ungraceful application exit
            }
        }

        /// <summary>
        /// Adds a new connection tab to the main view.
        /// </summary>
        /// <param name="ip">The host ip address.</param>
        internal void AddConnectionTab(string ip) {
            TabPage newTabPage = createConnectionTab(ip);
            // handle main view modification from another thread
            if (this.fileTransferTabs.InvokeRequired) {
                AddTab_Callback callback = new AddTab_Callback(AddConnectionTab);
                this.Invoke(callback, new object[] { ip });
            } else {
                this.fileTransferTabs.TabPages.Add(newTabPage);
                Print("Connected to \"" + ip + "\".");
            }
        }

        /// <summary>
        /// Creates a new connection tab.
        /// </summary>
        /// <param name="id">The identifier for all controls.</param>
        private TabPage createConnectionTab(string id) {
            // create new tab
            TabPage tabPage = new TabPage();
            tabPage.Location = new Point(4, 24);
            tabPage.Name = id;
            tabPage.Padding = new Padding(3);
            tabPage.Size = new Size(523, 186);
            tabPage.TabIndex = 0;
            tabPage.Text = id;
            tabPage.UseVisualStyleBackColor = true;

            // create tool strip button control
            ToolStripButton toolStripButton1 = new ToolStripButton();
            toolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButton1.Image = MultipleClientServer.Properties.Resources.disconnect16;
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
            toolStripButton2.Image = MultipleClientServer.Properties.Resources.clear16;
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
            ColumnHeader fileTransferIDCol = new ColumnHeader();
            fileTransferIDCol.Text = "ID";

            ColumnHeader fileTransferFileNameCol = new ColumnHeader();
            fileTransferFileNameCol.Text = "Filename";
            fileTransferFileNameCol.Width = 390;

            ColumnHeader fileTransferProgressCol = new ColumnHeader();
            fileTransferProgressCol.Text = "Progress";
            fileTransferProgressCol.TextAlign = HorizontalAlignment.Right;
            fileTransferProgressCol.Width = 55;

            // create list view control
            ListView listView = new ListView();
            listView.Anchor = ((((AnchorStyles.Top | AnchorStyles.Bottom) | AnchorStyles.Left) | AnchorStyles.Right));
            listView.Location = new Point(3, 31);
            listView.Name = "lv_" + id;
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

        /// <summary>
        /// Callback for 'Disconnect' button clicked.
        /// Initiates disconnection from host.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void disconnectBtn_Click(object sender, EventArgs e) {
            string hostAddress = ((ToolStripButton)sender).Name;
            Print("Disconnecting from \"" + hostAddress + "\".");
            controller.DisconnectFrom(hostAddress);
        }

        /// <summary>
        /// Callback for 'Clear' button clicked.
        /// Removes all finished uploads from respective control.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event.</param>
        private void clearBtn_Click(object sender, EventArgs e) {
            ToolStripButton origin = (ToolStripButton)sender;
            string hostAddress = origin.Name;
            // remove all finished uploads from list view
            TabPage tabPage = (TabPage)origin.GetCurrentParent().Parent;
            ListView listView = (ListView)tabPage.Controls["lv_" + hostAddress];
            foreach (ListViewItem item in listView.Items) {
                if (item.SubItems[2].Text.Equals("100%")) {
                    item.Remove();
                }
            }
        }

        /// <summary>
        /// Removes a connection tab in the main view.
        /// </summary>
        /// <param name="ip">The host ip address.</param>
        internal void RemoveConnectionTab(string ip) {
            TabPage tabPage = this.fileTransferTabs.TabPages[ip];

            // handle main view modification from another thread
            try {
                if (this.fileTransferTabs.InvokeRequired) {
                    RemoveTab_Callback callback = new RemoveTab_Callback(RemoveConnectionTab);
                    this.Invoke(callback, new object[] { ip });
                } else {
                    if (tabPage != null) {
                        this.fileTransferTabs.TabPages.Remove(tabPage);
                        Print("Disconnected from \"" + ip + "\".");
                    }
                }
            } catch (ObjectDisposedException) {
                // should only be caused on ungraceful application exit
            }
        }

        /// <summary>
        /// Prints logging informatin to the log control.
        /// </summary>
        /// <param name="msg">The logging information.</param>
        internal void Print(string msg) {
            string[] rowInfo = new string[2];
            rowInfo[0] = DateTime.Now.ToString(Constants.LOG_DATETIME_FORMAT);
            rowInfo[1] = msg;

            // handle main view modification from another thread
            try {
                if (this.logLV.InvokeRequired) {
                    Print_Callback callback = new Print_Callback(Print);
                    this.Invoke(callback, new object[] { msg });
                } else {
                    this.logLV.Items.Add(new ListViewItem(rowInfo));
                }
            } catch (ObjectDisposedException) {
                // should only be caused on ungraceful application exit
            }
        }

        // delegates for access from another thread
        delegate void Print_Callback(string msg);
        delegate void AddTab_Callback(string ip);
        delegate void RemoveTab_Callback(string ip);
        delegate void UpdateProgressOnFile_Callback(string ip, string fileName, double progress);
    }
}
