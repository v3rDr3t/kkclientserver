using KKClientServer.Networking;
using KKClientServer.Networking.Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace KKClientServer.Server {

    internal class ServerManager {
        #region Fields
        private Controller controller;
        private SocketOperationPool poolOfAcceptOps;

        private Socket serverSocket;
        private Semaphore connectionLimiter;
        #endregion

        /// <summary>
        /// Constructs a <code>ClientManager</code> object.
        /// </summary>
        internal ServerManager(Controller controller) {
            this.controller = controller;

            this.poolOfAcceptOps = new SocketOperationPool(Constants.MAX_NUM_CONNECTIONS);

            // create connections count limiter
            this.connectionLimiter = new Semaphore(Constants.MAX_NUM_CONNECTIONS, Constants.MAX_NUM_CONNECTIONS);

            startListening();
        }

        /// <summary>
        /// Starts listening for connection requests.
        /// </summary>
        private void startListening() {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Constants.DEFAULT_PORT);
            serverSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(localEndPoint);

            // start listener
            serverSocket.Listen(Constants.CONNECTIONS_QUEUE_SIZE);
            startAccepting();
        }
        
        /// <summary>
        /// Starts accepting connection requests.
        /// </summary>
        private void startAccepting() {
            SocketAsyncEventArgs acceptOp = getAcceptOp();

            // enter the semaphore            
            this.connectionLimiter.WaitOne();
                       
            bool pending = serverSocket.AcceptAsync(acceptOp);
            if (!pending) {
                processAccept(acceptOp);
            }
        }

        /// <summary>
        /// Gets or creates a accept operation.
        /// </summary>
        /// <param name="socket">The socket.</param>
        private SocketAsyncEventArgs getAcceptOp() {
            SocketAsyncEventArgs acceptOp = this.poolOfAcceptOps.Pop();
            if (acceptOp == null) {
                acceptOp = new SocketAsyncEventArgs();
            }
            AcceptUserToken token = new AcceptUserToken("");
            acceptOp.UserToken = token;
            acceptOp.Completed += new EventHandler<SocketAsyncEventArgs>(onAccept_Completed);
            return acceptOp;
        }

        /// <summary>
        /// Callback for an accept operation.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="op">The accept operation.</param>
        private void onAccept_Completed(object sender, SocketAsyncEventArgs op) {
            processAccept(op);
        }

        /// <summary>
        /// Processes a given accept operation.
        /// </summary>
        /// <param name="acceptOp">The accept operation.</param>
        private void processAccept(SocketAsyncEventArgs acceptOp) {
            if (acceptOp.SocketError != SocketError.Success) {
                startAccepting();
                acceptOp.AcceptSocket.Close();
                poolOfAcceptOps.Push(acceptOp);
                return;
            }

            // continue accepting.
            startAccepting();
        }
    }
}