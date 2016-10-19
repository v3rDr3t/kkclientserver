using KKClientServer.Client;
using KKClientServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer {
    internal class Controller {
        #region Fields
        private MainView mainView;
        private ClientManager clientManager;
        private ServerManager serverManager;
        #endregion

        /// <summary>
        /// Constructs a <code>Controller</code> object.
        /// </summary>
        /// <param name="mainView">The main view.</param>
        internal Controller(MainView mainView) {
            this.mainView = mainView;
            this.clientManager = new ClientManager(this);
            this.serverManager = new ServerManager(this);
            mainView.AddController(this);
        }

        /// <summary>
        /// Delegates the connection to a host to the connection manager.
        /// </summary>
        /// <param name="hostAddress">The host address to connect to.</param>
        internal void ConnectTo(string hostAddress) {
            clientManager.ConnectTo(hostAddress);
        }

        /// <summary>
        /// Delegates the disconnection from a host to the connection manager.
        /// </summary>
        /// <param name="hostAddress">The host address to disconnect from.</param>
        internal void DisconnectFrom(string hostAddress) {
            clientManager.DisconnectFrom(hostAddress);
        }

        /// <summary>
        /// Delegates a successful connection to a host to the main view.
        /// </summary>
        /// <param name="ip">The host address.</param>
        internal void Connected(string ip) {
            mainView.AddConnectionTab(ip, "Name");
        }

        /// <summary>
        /// Delegates a successful disconnection from a host to the main view.
        /// </summary>
        /// <param name="ip">The host address.</param>
        internal void Disconnected(string ip) {
            mainView.RemoveConnectionTab(ip);
        }

        internal void Print(string msg) {
            mainView.Print(msg);
        }
    }
}
