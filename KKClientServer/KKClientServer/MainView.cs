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
        }
        private void onMainView_Shown(object sender, EventArgs e)
        {
            // TODO: start TCP listener
            print("Starting server...");
        }

        private void onMainView_Closing(object sender, FormClosingEventArgs e)
        {
            // TODO: stop TCP listener
            print("Stopping server...");
            
            // TODO: misc cleanup
        }

        private void onConnectBtn_Click(object sender, EventArgs e)
        {
            // TODO: Connect to server
            print("Initiate connection to \"" + this.selectServerTB.Text + "\"...");

            if (true /* connection is setup */)
            {
                print("Connected to \"" + this.selectServerTB.Text + "\".");
                addConnectionTab(this.selectServerTB.Text);
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

        private void addConnectionTab(string serverName)
        {
            // TODO: create columns

            // TODO: create list view

            // TODO: create send text controls

            // TODO: create send file controls

            TabPage newTabPage = new TabPage(serverName);
            // TODO: add controls
            this.fileTransferTabs.TabPages.Add(newTabPage);
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
