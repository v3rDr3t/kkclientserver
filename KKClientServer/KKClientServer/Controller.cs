using KKClientServer.Networking;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer {

    internal class Controller {
        #region Fields
        private MainView mainView;
        private NetworkController netController;
        #endregion

        /// <summary>
        /// Constructs a <see cref="Controller"/> object.
        /// </summary>
        /// <param name="mainView">The main view.</param>
        internal Controller(MainView mainView) {
            this.mainView = mainView;
            this.netController = new NetworkController(this);
            mainView.AddController(this);
        }


        /// <summary>
        /// Delegates the connection to a host to the connection manager.
        /// </summary>
        /// <param name="hostAddress">The host address to connect to.</param>
        internal void ConnectTo(string hostAddress) {
            this.netController.ConnectTo(hostAddress);
        }

        /// <summary>
        /// Delegates the disconnection from a host to the connection manager.
        /// </summary>
        /// <param name="hostAddress">The host address to disconnect from.</param>
        internal void DisconnectFrom(string hostAddress) {
            this.netController.DisconnectFrom(hostAddress);
        }

        /// <summary>
        /// Delegates a successful connection to a host to the main view.
        /// </summary>
        /// <param name="ip">The host address.</param>
        internal void Connected(string ip) {
            mainView.AddConnectionTab(ip);
        }

        /// <summary>
        /// Delegates a successful disconnection from a host to the main view.
        /// </summary>
        /// <param name="ip">The host address.</param>
        internal void Disconnected(string ip) {
            mainView.RemoveConnectionTab(ip);
        }

        /// <summary>
        /// Delegates a message transfer to the connection manager.
        /// </summary>
        /// <param name="ip">The host address to send to.</param>
        /// <param name="msg">The text message to send.</param>
        internal void SendMessageTo(string ip, string msg) {
            this.netController.SendTextTo(ip, msg);
        }

        /// <summary>
        /// Delegates a file transfer to the connection manager.
        /// </summary>
        /// <param name="ip">The host address to send to.</param>
        /// <param name="fileInfo">The file information.</param>
        internal void SendFileTo(string ip, FileInfo fileInfo) {
            this.netController.SendFileTo(ip, fileInfo);
        }


        /// <summary>
        /// Delegates the logging of information to the main view.
        /// </summary>
        /// <param name="msg">The logging information.</param>
        internal void Print(string msg) {
            mainView.Print(msg);
        }
    }
}
