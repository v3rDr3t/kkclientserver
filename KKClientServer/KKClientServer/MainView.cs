using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KKClientServer
{
    public partial class MainView : Form
    {
        public MainView()
        {
            InitializeComponent();
            this.fileTransferTabs.DrawItem += new DrawItemEventHandler(DrawOnTabPage);
        }

        private void onServerStartBtn_Click(object sender, EventArgs e)
        {
            // TODO: Start TCP listener
            print("Starting server...");

            // update controls enability
            this.serverStopBtn.Enabled = true;
            this.serverStartBtn.Enabled = false;
        }

        private void onServerStopBtn_Click(object sender, EventArgs e)
        {
            // TODO: Stop TCP listener
            print("Stopping server...");

            // update controls enability
            this.serverStartBtn.Enabled = true;
            this.serverStopBtn.Enabled = false;
        }

        private void onConnectBtn_Click(object sender, EventArgs e)
        {
            // TODO: Connect to server
            print("Initiate connection to \"" + this.selectServerTB.Text + "\"...");

            if (true /* connection is setup */)
            {
                print("Connected to \"" + this.selectServerTB.Text + "\".");
                addConnectionTab(this.selectServerTB.Text);

                // update controls enability
                updateSendControls();
            }
        }

        private void onSendTextBtn_Click(object sender, EventArgs e)
        {
            if (!this.sendTextTB.Text.Equals(""))
            {
                // TODO: Send text message
                print("Initiate sending text \"" + this.sendTextTB.Text + "\"...");
            }
        }

        private void onBrowseFileBtn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileBrowser = new OpenFileDialog())
            {
                fileBrowser.Filter = "All Files (*.*)|*.*";
                if (fileBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.sendFileTB.Text = fileBrowser.FileName;
                }
            }
        }

        private void onSendFileBtn_Click(object sender, EventArgs e)
        {
            if (File.Exists(this.sendFileTB.Text))
            {
                // TODO: Send file
                print("Initiate sending file \"" + this.sendFileTB.Text + "\"...");
            }
        }

        private void onFileTransferTabs_MouseDown(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < this.fileTransferTabs.TabPages.Count; i++)
            {
                Rectangle r = this.fileTransferTabs.GetTabRect(i);
                // get position of close icon.
                Rectangle closeButton = new Rectangle(r.Right - 15, r.Top + 4, 9, 7);
                //if (closeButton.Contains(e.Location))
                //{
                //    this.fileTransferTabs.TabPages.RemoveAt(i);
                //    break;
                //}
            }
        }

        private void DrawOnTabPage(object sender, DrawItemEventArgs e)
        {
            Graphics g = e.Graphics;
            Pen p = new Pen(Color.Blue);
            //g.DrawRectangle(p, myTabRect);
        }

        private void addConnectionTab(string serverName)
        {
            // create columns
            ColumnHeader fileTransferIDCol = new ColumnHeader();
            fileTransferIDCol.Text = "ID";
            fileTransferIDCol.Width = 60;
            ColumnHeader fileTransferNameCol = new ColumnHeader();
            fileTransferNameCol.Text = "Filename";
            fileTransferNameCol.Width = 212;
            ColumnHeader fileTransferProgressCol = new ColumnHeader();
            fileTransferProgressCol.Text = "%";
            fileTransferProgressCol.TextAlign = HorizontalAlignment.Right;
            fileTransferProgressCol.Width = 40;

            // create list view
            ListView fileTransferLV = new ListView();
            fileTransferLV.Anchor = (AnchorStyles)(((AnchorStyles.Top | AnchorStyles.Left) | AnchorStyles.Right));
            fileTransferLV.Columns.AddRange(new ColumnHeader[] {
                fileTransferIDCol, fileTransferNameCol, fileTransferProgressCol
            });
            fileTransferLV.Location = new Point(0, 0);
            fileTransferLV.Size = new Size(316, 150);
            fileTransferLV.UseCompatibleStateImageBehavior = false;
            fileTransferLV.View = View.Details;

            // create other controls
            Button disconnectBtn = new Button();
            disconnectBtn.Anchor = AnchorStyles.None;
            disconnectBtn.Location = new Point(121, 162);
            disconnectBtn.Size = new Size(75, 23);
            disconnectBtn.Text = "Disconnect";
            disconnectBtn.UseVisualStyleBackColor = true;

            // create tab page
            TabPage newTabPage = new TabPage(serverName);
            newTabPage.Location = new Point(4, 22);
            newTabPage.Padding = new Padding(3);
            newTabPage.Size = new Size(316, 193);
            newTabPage.UseVisualStyleBackColor = true;
            newTabPage.Controls.Add(fileTransferLV);
            newTabPage.Controls.Add(disconnectBtn);
            this.fileTransferTabs.TabPages.Add(newTabPage);
        }

        private void updateSendControls()
        {
            // TODO: Check for existing connection
            if (this.fileTransferTabs.TabCount > 0)
            {
                this.sendTextTB.Enabled = true;
                this.sendTextBtn.Enabled = true;
                this.sendFileTB.Enabled = true;
                this.browseFileBtn.Enabled = true;
                this.sendFileBtn.Enabled = true;
                this.fileTransferTabs.Enabled = true;
            }
            else
            {
                this.sendTextTB.Enabled = false;
                this.sendTextBtn.Enabled = false;
                this.sendFileTB.Enabled = false;
                this.browseFileBtn.Enabled = false;
                this.sendFileBtn.Enabled = false;
                this.fileTransferTabs.Enabled = false;
            }
        }

        internal void print(string message)
        {
            string[] rowInfo = new string[2];
            rowInfo[0] = DateTime.Now.ToString(Constants.LOG_DATETIME_FORMAT);
            rowInfo[1] = message;
            this.logLV.Items.Add(new ListViewItem(rowInfo));
        }

    }
}
