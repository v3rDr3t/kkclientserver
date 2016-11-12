using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using System.IO;
using System.Timers;

namespace ClientServer.Networking {
    internal class NetworkController {
        #region Fields
        // The controller
        private Controller controller;
        // The server socket
        private Socket serverSocket;
        // Reusable set of buffers for all socket operations.
        private ManagedBuffer sendBuffer;
        private ManagedBuffer receiveBuffer;
        // Holds all currently open server connections
        private Dictionary<IPEndPoint, SocketAsyncEventArgs> connections;
        // Pool of reusable connect event args
        private SocketOperationPool connectEventArgsPool;
        // Pool of reusable accept event args
        private SocketOperationPool acceptEventArgsPool;
        // Pool of reusable send event args
        private SocketOperationPool sendEventArgsPool;
        // Pool of reusable receive event args
        private SocketOperationPool receiveEventArgsPool;
        // The builder for tcp messages
        private TcpMessageBuilder messageBuilder;
        // The receive message handler
        private ReceiveMessageHandler receiveHandler;
        // The send message handler
        private SendMessageHandler sendHandler;
        #endregion

        /// <summary>
        /// Constructs a <see cref="NetworkController"/> object.
        /// </summary>
        internal NetworkController(Controller controller) {
            this.controller = controller;

            // create buffers
            int totalBufferSize = Constants.BUFFER_SIZE * Constants.MAX_ASYNC_SEND_OPS;
            this.sendBuffer = new ManagedBuffer(totalBufferSize, Constants.BUFFER_SIZE);
            totalBufferSize = Constants.BUFFER_SIZE * Constants.MAX_ASYNC_RECEIVE_OPS;
            this.receiveBuffer = new ManagedBuffer(totalBufferSize, Constants.BUFFER_SIZE);

            // create networking
            this.connections = new Dictionary<IPEndPoint, SocketAsyncEventArgs>();
            this.connectEventArgsPool = new SocketOperationPool(Constants.MAX_ASYNC_CONNECT_OPS);
            this.acceptEventArgsPool = new SocketOperationPool(Constants.MAX_ASYNC_ACCEPT_OPS);
            this.sendEventArgsPool = new SocketOperationPool(Constants.MAX_ASYNC_SEND_OPS);
            this.receiveEventArgsPool = new SocketOperationPool(Constants.MAX_ASYNC_RECEIVE_OPS);
            this.messageBuilder = new TcpMessageBuilder();
            this.receiveHandler = new ReceiveMessageHandler();
            this.sendHandler = new SendMessageHandler();

            initializePools();
            startListening();
        }

        /// <summary>
        /// Preallocates reusable buffers and event args.
        /// </summary>
        private void initializePools() {
            // allocate one large byte buffer block
            this.sendBuffer.Initialize();
            this.receiveBuffer.Initialize();

            // preallocate send pool
            SocketAsyncEventArgs sendEA;
            for (int i = 0; i < Constants.MAX_ASYNC_SEND_OPS; i++) {
                sendEA = new SocketAsyncEventArgs();
                sendEA.Completed += new EventHandler<SocketAsyncEventArgs>(onSend_Completed);
                this.sendBuffer.Set(sendEA);
                // assign transfer data object
                SendToken token = new SendToken(sendEA);
                sendEA.UserToken = token;

                this.sendEventArgsPool.Push(sendEA);
            }

            // preallocate receive pool
            SocketAsyncEventArgs receiveEA;
            for (int i = 0; i < Constants.MAX_ASYNC_RECEIVE_OPS; i++) {
                receiveEA = new SocketAsyncEventArgs();
                receiveEA.Completed += new EventHandler<SocketAsyncEventArgs>(onReceive_Completed);
                this.receiveBuffer.Set(receiveEA);
                // assign transfer data object
                ReceiveToken token = new ReceiveToken(receiveEA);
                receiveEA.UserToken = token;

                this.receiveEventArgsPool.Push(receiveEA);
            }
        }

        /// <summary>
        /// Starts listening for connection requests.
        /// </summary>
        private void startListening() {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, Constants.DEFAULT_PORT);
            serverSocket = new Socket(localEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(localEP);

            // start listener
            serverSocket.Listen(Constants.BACKLOG);
            startAccepting();
        }

        /// <summary>
        /// Starts accepting connection requests.
        /// </summary>
        private void startAccepting() {
            SocketAsyncEventArgs acceptEA = getAcceptEventArgs();
            bool pending = serverSocket.AcceptAsync(acceptEA);
            if (!pending)
                processAccept(acceptEA);
        }

        /// <summary>
        /// Gets or creates accept event args.
        /// </summary>
        private SocketAsyncEventArgs getAcceptEventArgs() {
            SocketAsyncEventArgs acceptEA = this.acceptEventArgsPool.Pop();
            if (acceptEA == null) {
                acceptEA = new SocketAsyncEventArgs();
                acceptEA.Completed += new EventHandler<SocketAsyncEventArgs>(onAccept_Completed);
            }
            return acceptEA;
        }

        /// <summary>
        /// Processes a accept operation of given event args.
        /// </summary>
        /// <param name="acceptEA">The accept event args.</param>
        private void processAccept(SocketAsyncEventArgs acceptEA) {
            switch (acceptEA.SocketError) {
                case SocketError.Success: 
                    // continue accepting.
                    startAccepting();

                    // report back
                    IPEndPoint client = acceptEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
                    controller.Print("\"" + client.Address + "\" connected.");

                    // start receiving for accepted client connection
                    SocketAsyncEventArgs receiveEA = getReceiveEventArgs(client);
                    receiveEA.AcceptSocket = acceptEA.AcceptSocket;
                    acceptEA.AcceptSocket = null;
                    this.acceptEventArgsPool.Push(acceptEA);
                    startReceiving(receiveEA);
                    break;

                case SocketError.OperationAborted:
                    acceptEA.AcceptSocket.Close();
                    this.acceptEventArgsPool.Push(acceptEA);
                    break;

                default:
                    // continue accepting.
                    startAccepting();

                    acceptEA.AcceptSocket.Close();
                    this.acceptEventArgsPool.Push(acceptEA);
                    break;
            }
        }
        
        /// <summary>
        /// Gets or creates receive event args for a given remote endpoint.
        /// </summary>
        /// <param name="remoteEP">The remote endpoint.</param>
        private SocketAsyncEventArgs getReceiveEventArgs(IPEndPoint remoteEP) {
            SocketAsyncEventArgs receiveEA = this.receiveEventArgsPool.Pop();
            if (receiveEA == null) {
                receiveEA = new SocketAsyncEventArgs();
                receiveEA.Completed += new EventHandler<SocketAsyncEventArgs>(onReceive_Completed);
                // assign transfer data object
                ReceiveToken token = new ReceiveToken(receiveEA);
                receiveEA.UserToken = token;
            }
            receiveEA.RemoteEndPoint = remoteEP;
            return receiveEA;
        }

        /// <summary>
        /// Starts the receiving process.
        /// </summary>
        /// <param name="receiveEA">The receive event args.</param>
        private void startReceiving(SocketAsyncEventArgs receiveEA) {
            // set the buffer
            ReceiveToken token = (ReceiveToken)receiveEA.UserToken;
            receiveEA.SetBuffer(token.BufferOffset, Constants.BUFFER_SIZE);

            // post receive operation
            bool pending = receiveEA.AcceptSocket.ReceiveAsync(receiveEA);
            if (!pending) {
                processReceive(receiveEA);
            }
        }

        /// <summary>
        /// Processes a receive operation of given event args.
        /// </summary>
        /// <param name="receiveEA">The receive event args.</param>
        private void processReceive(SocketAsyncEventArgs receiveEA) {
            ReceiveToken token = (ReceiveToken)receiveEA.UserToken;
            // close connection on socket error
            if (receiveEA.SocketError != SocketError.Success) {
                // report back
                IPEndPoint client = receiveEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
                controller.Print("A socket error occured while receiving data. Closing connection to \""
                    + client.Address + "\".");

                token.Reset();
                closeSocket(receiveEA.AcceptSocket);
                this.receiveEventArgsPool.Push(receiveEA);
                return;
            }

            // client closed the socket voluntarily.
            if (receiveEA.BytesTransferred == 0) {
                // report back
                IPEndPoint client = receiveEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
                controller.Print("Client \"" + client.Address + "\" disconnected!");

                token.Reset();
                closeSocket(receiveEA.AcceptSocket);
                this.receiveEventArgsPool.Push(receiveEA);
                return;
            }

            // handle the prefix
            int bytesToProcess = receiveEA.BytesTransferred;
            if (token.PrefixBytesReceived < Constants.PREFIX_SIZE) {
                bytesToProcess = receiveHandler.HandlePrefix(receiveEA, token, bytesToProcess);
                // post another receive operation in case prefix is incomplete
                if (bytesToProcess == 0) {
                    startReceiving(receiveEA);
                    return;
                }
            }

            // handle the text data
            if (token.TextBytesReceived < token.TextLength) {
                bytesToProcess = receiveHandler.HandleText(receiveEA, token, bytesToProcess);
                // post another receive operation in case text is incomplete
                if (bytesToProcess == 0 && token.FilePath.Equals("")) {
                    startReceiving(receiveEA);
                    return;
                }
            }

            // handle the file data
            if (token.Type == MessageType.File) {
                bool complete = receiveHandler.HandleFile(receiveEA, token, bytesToProcess);
                if (complete) {
                    // report back                    
                    string text = token.GetReceivedText();
                    IPEndPoint client = receiveEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
                    if (text != null) {
                        controller.Print(client.Address + ": File \"" + text + "\"");
                    }

                    token.Reset();
                } else {
                    token.FileOffset = token.BufferOffset;
                }
            } else {
                // report back                    
                string text = token.GetReceivedText();
                IPEndPoint client = receiveEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
                controller.Print(client.Address + ": Text \"" + text + "\"");

                token.Reset();
            }

            // continue receiving until disconnect
            startReceiving(receiveEA);
        }

        /// <summary>
        /// Connects to a host.
        /// </summary>
        /// <param name="ip">The host ip.</param>
        public void ConnectTo(string ip) {
            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(address, Constants.DEFAULT_PORT);

            // try to connect
            SocketAsyncEventArgs connectOp = getConnectEventArgs(remoteEP);
            if (!this.connections.ContainsKey(remoteEP)) {
                startConnecting(connectOp);
            } else {
                controller.Print("Already connected to host (" + ip + ").");
            }
        }

        /// <summary>
        /// Gets or creates connect event args for a given endpoint.
        /// </summary>
        /// <param name="remoteEP">The remote endpoint.</param>
        private SocketAsyncEventArgs getConnectEventArgs(IPEndPoint remoteEP) {
            SocketAsyncEventArgs connectEA = this.connectEventArgsPool.Pop();
            if (connectEA == null) {
                connectEA = new SocketAsyncEventArgs();
                connectEA.Completed += new EventHandler<SocketAsyncEventArgs>(onConnect_Completed);
            }
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connectEA.RemoteEndPoint = remoteEP;
            connectEA.AcceptSocket = clientSocket;
            return connectEA;
        }

        /// <summary>
        /// Starts the connection process.
        /// </summary>
        /// <param name="connectEA">The connect event args.</param>
        private void startConnecting(SocketAsyncEventArgs connectEA) {
            // post connect operation on the socket
            bool pending = connectEA.AcceptSocket.ConnectAsync(connectEA);
            if (!pending)
                processConnect(connectEA);

            // timeout
            Timer timer = new Timer();
            timer.Interval = Constants.CONNECT_TIMEOUT;
            timer.Elapsed += (sender, e) => onConnectTimer_Elapsed(sender, e, connectEA);
            timer.Start();
        }

        /// <summary>
        /// Callback for elapsed timer event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The elapsed event args.</param>
        /// <param name="connectEA">The connect event args.</param>
        private void onConnectTimer_Elapsed(object sender, ElapsedEventArgs e, SocketAsyncEventArgs connectEA) {
            Timer timer = (Timer)sender;
            if (!connectEA.AcceptSocket.Connected) {
                timer.Stop();
                // close socket
                connectEA.AcceptSocket.Close();
            }
        }

        /// <summary>
        /// Processes a connect operation of given event args.
        /// </summary>
        /// <param name="saea">The event args.</param>
        private void processConnect(SocketAsyncEventArgs saea) {
            if (saea.SocketError == SocketError.Success) {
                IPEndPoint remoteEP = saea.RemoteEndPoint as IPEndPoint;
                this.connections.Add(remoteEP, saea);
                // visualize
                controller.Connected(remoteEP.Address.ToString());
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
            IPEndPoint remoteEP = saea.RemoteEndPoint as IPEndPoint;
            this.connectEventArgsPool.Push(saea);
            // report back
            controller.Print("Could not connect to host (" + remoteEP.Address.ToString() + ").");
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
            if (socket != null)
                socket.Close();
        }

        /// <summary>
        /// Disconnects from a host.
        /// </summary>
        /// <param name="ip">The ip address.</param>
        internal void DisconnectFrom(string ip) {
            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(address, Constants.DEFAULT_PORT);
            // try to disconnect
            startDisconnecting(this.connections[remoteEP]);
        }

        /// <summary>
        /// Clean up routine on application exit.
        /// </summary>
        internal void Exit() {
            // disconnect from all connected clients
            foreach (KeyValuePair<IPEndPoint, SocketAsyncEventArgs> con in this.connections)
                startDisconnecting(con.Value);
            // close sockets and clear pools
            this.connectEventArgsPool.Clear();
            this.acceptEventArgsPool.Clear();
            this.sendEventArgsPool.Clear();
            this.receiveEventArgsPool.Clear();
            // clear buffers
            this.sendBuffer = null;
            this.receiveBuffer = null;
            // close server socket
            this.serverSocket.Close();
        }

        /// <summary>
        /// Starts the disconnection process.
        /// </summary>
        /// <param name="saea">The event args.</param>
        private void startDisconnecting(SocketAsyncEventArgs saea) {
            if (saea.AcceptSocket.Connected) {
                bool pending = saea.AcceptSocket.DisconnectAsync(saea);
                if (!pending)
                    processDisconnect(saea);
            }
        }

        /// <summary>
        /// Processes a disconnect operation of given event args.
        /// </summary>
        /// <param name="saea">The event args.</param>
        private void processDisconnect(SocketAsyncEventArgs saea) {
            closeSocket(saea.AcceptSocket);
            // release all resources
            IPEndPoint remoteEP = saea.RemoteEndPoint as IPEndPoint;
            string address = remoteEP.Address.ToString();
            this.connections.Remove(remoteEP);
            this.connectEventArgsPool.Push(saea);
            // visualize
            controller.Disconnected(address);
        }

        /// <summary>
        /// Sends a text message to a connected host.
        /// </summary>
        /// <param name="ip">The ip address.</param>
        /// <param name="text">The text message to send.</param>
        internal void SendTextTo(string ip, string text) {
            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(address, Constants.DEFAULT_PORT);

            // try to send text message
            if (this.connections.ContainsKey(remoteEP)) {
                SocketAsyncEventArgs sendEA = getSendEventArgs(remoteEP);
                // assign socket from current connections
                sendEA.AcceptSocket = this.connections[remoteEP].AcceptSocket;
                // build tcp message
                this.messageBuilder.BuildTextData(text, sendEA);
                startSendingText(sendEA);
            } else {
                throw new ArgumentException("Cannot send text message to unconnected host \""
                    + remoteEP.Address.ToString() + "\"!");
            }
        }

        /// <summary>
        /// Gets or creates send event args for a given remote endpoint.
        /// </summary>
        /// <param name="remoteEP">The remote endpoint.</param>
        private SocketAsyncEventArgs getSendEventArgs(IPEndPoint remoteEP) {
            SocketAsyncEventArgs sendEA = this.sendEventArgsPool.Pop();
            if (sendEA == null) {
                sendEA = new SocketAsyncEventArgs();
                sendEA.Completed += new EventHandler<SocketAsyncEventArgs>(onReceive_Completed);
                // assign transfer data object
                SendToken token = new SendToken(sendEA);
                sendEA.UserToken = token;
            }
            sendEA.RemoteEndPoint = remoteEP;
            return sendEA;
        }

        /// <summary>
        /// Starts the sending process for text.
        /// </summary>
        /// <param name="sendEA">The send event args.</param>
        private void startSendingText(SocketAsyncEventArgs sendEA) {
            SendToken token = (SendToken)sendEA.UserToken;
            int bytesLeft = checkedConversion(token.RemainingBytesToSend);
            int bytesSent = checkedConversion(token.BytesSent);

            // message fits into buffer
            if (bytesLeft <= Constants.BUFFER_SIZE) {
                sendEA.SetBuffer(token.BufferOffset, bytesLeft);
                Buffer.BlockCopy(
                    token.Data, bytesSent,
                    sendEA.Buffer, token.BufferOffset,
                    bytesLeft);
                byte[] array = new byte[bytesLeft];
                Buffer.BlockCopy(sendEA.Buffer, token.BufferOffset, array, 0, bytesLeft);
            }
            // message doesn't fit into buffer (send as much as possible)
            else {
                sendEA.SetBuffer(token.BufferOffset, Constants.BUFFER_SIZE);
                Buffer.BlockCopy(
                    token.Data, bytesSent,
                    sendEA.Buffer, token.BufferOffset,
                    Constants.BUFFER_SIZE);
                byte[] array = new byte[Constants.BUFFER_SIZE];
                Buffer.BlockCopy(sendEA.Buffer, token.BufferOffset, array, 0, Constants.BUFFER_SIZE);
            }

            // post the send operation
            if (sendEA.AcceptSocket.Connected) {
                bool pending = sendEA.AcceptSocket.SendAsync(sendEA);
                if (!pending)
                    processSendText(sendEA);
            }
        }

        /// <summary>
        /// Processes a file sending operation.
        /// </summary>
        /// <param name="sendEA">The send event args.</param>
        private void processSendText(SocketAsyncEventArgs sendEA) {
            SendToken token = (SendToken)sendEA.UserToken;

            if (sendEA.SocketError == SocketError.Success) {
                token.RemainingBytesToSend -= sendEA.BytesTransferred;
                if (token.RemainingBytesToSend != 0L) {
                    // not all data has been successfully transfered yet
                    token.BytesSent += sendEA.BytesTransferred;
                    startSendingText(sendEA);
                } else {
                    this.sendEventArgsPool.Push(sendEA);
                    //report back
                    controller.Print("Text transfer complete.");
                }
            } else {
                // a socket error occured
                token.Reset();
                processSendError(sendEA);
            }
        }

        /// <summary>
        /// Processes send errors of given event args.
        /// </summary>
        /// <param name="sendEA">The send event args.</param>  
        private void processSendError(SocketAsyncEventArgs sendEA) {
            closeSocket(sendEA.AcceptSocket);
            // release all resources
            IPEndPoint remoteEP = sendEA.RemoteEndPoint as IPEndPoint;
            string address = remoteEP.Address.ToString();
            this.connections.Remove(remoteEP);
            this.sendEventArgsPool.Push(sendEA);
            // report back and visualize
            controller.Print("Connection to host \"" + remoteEP.Address.ToString() + "\" has been lost.");
            controller.Disconnected(address);
        }

        /// <summary>
        /// Sends a file to a connected host.
        /// </summary>
        /// <param name="ip">The host ip.</param>
        /// <param name="fileInfo">The file information.</param>
        internal void SendFileTo(string ip, FileInfo fileInfo) {
            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(address, Constants.DEFAULT_PORT);

            // try to send
            if (this.connections.ContainsKey(remoteEP)) {
                SocketAsyncEventArgs sendEA = getSendEventArgs(remoteEP);
                // assign socket from current connections
                sendEA.AcceptSocket = this.connections[remoteEP].AcceptSocket;
                // build tcp message
                this.messageBuilder.BuildFileInfoData(fileInfo, sendEA);
                startSendingFile(sendEA);
            } else {
                throw new ArgumentException("Cannot send file to unconnected host \""
                    + remoteEP.Address.ToString() + "\"!");
            }
        }

        /// <summary>
        /// Starts the sending process of a file.
        /// </summary>
        /// <param name="sendEA">The send/receive event args.</param>
        private void startSendingFile(SocketAsyncEventArgs sendEA) {
            SendToken token = (SendToken)sendEA.UserToken;

            // message fits into buffer
            if (token.RemainingBytesToSend <= Constants.BUFFER_SIZE) {
                int bytesLeft = checkedConversion(token.RemainingBytesToSend);
                sendEA.SetBuffer(token.BufferOffset, bytesLeft);
            }
            // message doesn't fit into buffer (send as much as possible)
            else {
                sendEA.SetBuffer(token.BufferOffset, Constants.BUFFER_SIZE);
            }
            // handle prefix
            int fileDataToProcess = sendHandler.HandlePrefixAndFileName(sendEA, token);
            // handle file
            bool success = sendHandler.HandleFile(sendEA, token, fileDataToProcess);
            if (success) {
                // post the send operation
                if (sendEA.AcceptSocket.Connected) {
                    bool pending = sendEA.AcceptSocket.SendAsync(sendEA);
                    if (!pending)
                        processSendFile(sendEA);
                }
            } else {
                // report back
                controller.Print("An error occured while reading file \"" + token.Text + "\".");
                token.Reset();
                this.sendEventArgsPool.Push(sendEA);
            }
        }

        /// <summary>
        /// Processes a file sending operation.
        /// </summary>
        /// <param name="sendEA">The send event args.</param>
        private void processSendFile(SocketAsyncEventArgs sendEA) {
            SendToken token = (SendToken)sendEA.UserToken;
            if (sendEA.SocketError == SocketError.Success) {
                token.RemainingBytesToSend -= sendEA.BytesTransferred;
                token.BytesSent += sendEA.BytesTransferred;
                // report progress
                IPEndPoint remoteEP = sendEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
                double progress = (double)token.BytesSent / token.MessageSize;
                controller.UpdateProgress(remoteEP.Address, token.Text, progress);

                // file transfer incomplete
                if (token.RemainingBytesToSend > 0L) {
                    int toSend = token.PrefixAndFileNameBytesToSend - sendEA.BytesTransferred;
                    token.PrefixAndFileNameBytesToSend = (toSend > 0) ? toSend : 0;
                    startSendingFile(sendEA);
                }
                // file transfer complete
                else {
                    // report back
                    controller.Print("File transfer complete.");
                    token.Reset();
                    this.sendEventArgsPool.Push(sendEA);
                }
            } else {
                // report back
                controller.Print("File transfer incomplete.");
                token.Reset();
                processSendError(sendEA);
            }
        }

        /// <summary>
        /// Callback for accept event args.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="saea">The accept event args.</param>
        private void onAccept_Completed(object sender, SocketAsyncEventArgs saea) {
            processAccept(saea);
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
                    throw new ArgumentException("Unknown completed operation for connect!");
            }
        }

        /// <summary>
        /// Callback for send event args.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="saea">The send event args.</param>
        private void onSend_Completed(object sender, SocketAsyncEventArgs saea) {
            switch (saea.LastOperation) {
                case SocketAsyncOperation.Send:
                    BaseToken token = (BaseToken)saea.UserToken;
                    if (token.Type == MessageType.Text) {
                        processSendText(saea);
                    } else {
                        processSendFile(saea);
                    }
                    break;

                case SocketAsyncOperation.Disconnect:
                    processDisconnect(saea);
                    //report back
                    controller.Print("Client closed connection. Message transfer incomplete.");
                    break;

                default:
                    throw new ArgumentException("Unknown completed operation for send/receive!");
            }
        }

        /// <summary>
        /// Callback for receive event args.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="saea">The receive event args.</param>
        private void onReceive_Completed(object sender, SocketAsyncEventArgs saea) {
            switch (saea.LastOperation) {
                case SocketAsyncOperation.Receive:
                    processReceive(saea);
                    break;

                default:
                    throw new ArgumentException("Unknown completed operation for send/receive!");
            }
        }

        /// <summary>
        /// Performs a checked convertion of a given signed long to integer.
        /// </summary>
        /// <param name="n">The signed long.</param>
        public static int checkedConversion(long n) {
            int ret;
            ret = checked((int)n);
            return ret;
        }
    }
}
