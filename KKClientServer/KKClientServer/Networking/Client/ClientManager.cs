using KKClientServer.Networking;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace KKClientServer.Client {

    internal class ClientManager {
        #region Fields
        private Controller controller;
        // Holds all currently open connections
        private Dictionary<IPEndPoint, SocketAsyncEventArgs> connections;
        // Pool of reusable connect event args
        private SocketOperationPool connectEventArgsPool;
        // Pool of reusable send/receive event args
        private SocketOperationPool sendRecEventArgsPool;
        #endregion

        /// <summary>
        /// Constructs a <code>ClientManager</code> object.
        /// </summary>
        internal ClientManager(Controller controller) {
            this.controller = controller;
            this.connections = new Dictionary<IPEndPoint, SocketAsyncEventArgs>();
            this.connectEventArgsPool = new SocketOperationPool(Constants.MAX_NUM_CONNECTIONS);
            this.sendRecEventArgsPool = new SocketOperationPool(Constants.MAX_NUM_SEND_REC);
        }

        /// <summary>
        /// Connects to a host.
        /// </summary>
        /// <param name="hostAddress">The host address.</param>
        public void ConnectTo(string hostAddress) {
            // create socket
            IPAddress ip = IPAddress.Parse(hostAddress);
            IPEndPoint endPoint = new IPEndPoint(ip, Constants.DEFAULT_PORT);

            // try to connect
            SocketAsyncEventArgs connectOp = getConnectOp(endPoint);
            if (!connections.ContainsKey(endPoint)) {
                startConnecting(connectOp);
            } else {
                controller.Print("Already connected to host (" + hostAddress + ").");
            }
        }

        /// <summary>
        /// Gets or creates connect event args for a given endpoint.
        /// </summary>
        /// <param name="ep">The endpoint.</param>
        private SocketAsyncEventArgs getConnectOp(IPEndPoint ep) {
            SocketAsyncEventArgs connectOp = this.connectEventArgsPool.Pop();
            if (connectOp == null) {
                connectOp = new SocketAsyncEventArgs();
                connectOp.Completed += new EventHandler<SocketAsyncEventArgs>(onConnect_Completed);
            }
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //ConnectUserToken token = new ConnectUserToken(ep.Address.ToString());
            connectOp.RemoteEndPoint = ep;
            connectOp.AcceptSocket = clientSocket;
            return connectOp;
        }

        /// <summary>
        /// Starts the connection process.
        /// </summary>
        /// <param name="saea">The connect event args.</param>
        private void startConnecting(SocketAsyncEventArgs saea) {
            // post connect operation on the socket
            bool pending = saea.AcceptSocket.ConnectAsync(saea);
            if (!pending) {
                processConnect(saea);
            }
        }

        /// <summary>
        /// Callback for connect event args.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="saea">The connect event args.</param>
        private void onConnect_Completed(object sender, SocketAsyncEventArgs saea) {
            switch (saea.LastOperation) {
                case SocketAsyncOperation.Connect:
                    processConnect(saea);
                    break;

                case SocketAsyncOperation.Disconnect:
                    processDisconnect(saea);
                    break;

                default:
                    throw new ArgumentException("Unknown last operation for connect!");
            }
        }

        /// <summary>
        /// Callback for send/receive event args.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="saea">The send/receive event args.</param>
        private void onSendRec_Completed(object sender, SocketAsyncEventArgs saea) {
            switch (saea.LastOperation) {
                case SocketAsyncOperation.Send:
                    // TODO: ...
                    break;

                case SocketAsyncOperation.Receive:
                    // TODO: ...
                    break;

                case SocketAsyncOperation.Disconnect:
                    // TODO: ...
                    break;

                default:
                    throw new ArgumentException("Unknown last operation for send/receive!");
            }
        }

        /// <summary>
        /// Processes a connect operation of given event args.
        /// </summary>
        /// <param name="saea">The event args.</param>
        private void processConnect(SocketAsyncEventArgs saea) {
            if (saea.SocketError == SocketError.Success) {
                IPEndPoint host = saea.RemoteEndPoint as IPEndPoint;
                connections.Add(host, saea);
                // visualize
                controller.Connected(host.Address.ToString());
            } else {
                processConnectionError(saea);
            }
        }

        /// <summary>
        /// Processes connection errors of given event args.
        /// </summary>
        /// <param name="saea">The event args.</param>  
        private void processConnectionError(SocketAsyncEventArgs saea) {
            // close socket
            closeSocket(saea.AcceptSocket);
            // release all resources
            IPEndPoint host = saea.RemoteEndPoint as IPEndPoint;
            connections.Remove(host);
            this.connectEventArgsPool.Push(saea);
            // report back
            controller.Print("Could not connect to host (" + host.Address.ToString() + ").");
        }

        /// <summary>
        /// Closes a given socket.
        /// </summary>
        /// <param name="socket">The socket.</param>  
        private void closeSocket(Socket socket) {
            try {
                socket.Shutdown(SocketShutdown.Both);
            } catch (Exception) {
                // socket already closed
            }
            socket.Close();
        }

        /// <summary>
        /// Disconnects from a host.
        /// </summary>
        /// <param name="hostAddress">The host address.</param>
        public void DisconnectFrom(string hostAddress) {
            IPAddress ip = IPAddress.Parse(hostAddress);
            IPEndPoint endPoint = new IPEndPoint(ip, Constants.DEFAULT_PORT);
            // try to disconnect
            startDisconnecting(connections[endPoint]);
        }

        /// <summary>
        /// Starts the disconnection process.
        /// </summary>
        /// <param name="saea">The event args.</param>
        private void startDisconnecting(SocketAsyncEventArgs saea) {
            saea.AcceptSocket.Shutdown(SocketShutdown.Both);
            bool pending = saea.AcceptSocket.DisconnectAsync(saea);
            if (!pending) {
                processDisconnect(saea);
            }
        }

        /// <summary>
        /// Processes a disconnect operation of given event args.
        /// </summary>
        /// <param name="saea">The event args.</param>
        private void processDisconnect(SocketAsyncEventArgs saea) {
            // close socket
            saea.AcceptSocket.Close();
            // release all resources
            IPEndPoint host = saea.RemoteEndPoint as IPEndPoint;
            string hostAddress = host.Address.ToString();
            connections.Remove(host);
            this.connectEventArgsPool.Push(saea);
            // visualize
            controller.Disconnected(hostAddress);
        }
    }
}