using KKClientServer.Client;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
using KKClientServer.Networking.Client;
using System.IO;

namespace KKClientServer.Networking {
    internal class NetworkController {
        #region Fields
        // The controller
        private Controller controller;
        // The server socket
        private Socket serverSocket;
        // Reusable set of buffers for all socket operations.
        private ManagedBuffer buffer;
        // Holds all currently open server connections
        private Dictionary<IPEndPoint, SocketAsyncEventArgs> connections;
        // Pool of reusable connect event args
        private SocketOperationPool connectEventArgsPool;
        // Pool of reusable accept event args
        private SocketOperationPool acceptEventArgsPool;
        // Pool of reusable send/receive event args
        private SocketOperationPool sendRecEventArgsPool;
        // The builder for tcp messages
        private TcpMessageBuilder messageBuilder;
        // The receive message handler
        private ReceiveMessageHandler messageHandler;
        #endregion

        /// <summary>
        /// Constructs a <see cref="NetworkController"/> object.
        /// </summary>
        internal NetworkController(Controller controller) {
            this.controller = controller;

            // create buffer
            int totalBufferSize = Constants.BUFFER_SIZE * Constants.MAX_NUM_CONNECTIONS * Constants.PREALLOCATION_OPS;
            int bufferSizePerEventArgs = Constants.BUFFER_SIZE * Constants.PREALLOCATION_OPS;
            this.buffer = new ManagedBuffer(totalBufferSize, bufferSizePerEventArgs);

            // create networking
            this.connections = new Dictionary<IPEndPoint, SocketAsyncEventArgs>();
            this.connectEventArgsPool = new SocketOperationPool(Constants.MAX_ASYNC_CONNECT_OPS);
            this.acceptEventArgsPool = new SocketOperationPool(Constants.MAX_ASYNC_ACCEPT_OPS);
            this.sendRecEventArgsPool = new SocketOperationPool(Constants.MAX_NUM_SEND_REC);
            this.messageBuilder = new TcpMessageBuilder();
            this.messageHandler = new ReceiveMessageHandler();

            initializePools();
            startListening();
        }

        /// <summary>
        /// Preallocates reusable buffers and event args.
        /// </summary>
        private void initializePools() {
            // allocate one large byte buffer block
            this.buffer.Initialize();
            
            // preallocate send/receive pool
            SocketAsyncEventArgs sendRecEA;
            for (int i = 0; i < 5; i++) {
                sendRecEA = new SocketAsyncEventArgs();
                sendRecEA.Completed += new EventHandler<SocketAsyncEventArgs>(onSendRec_Completed);
                this.buffer.Set(sendRecEA);

                DataUserToken receiveSendToken = new DataUserToken(sendRecEA);
                sendRecEA.UserToken = receiveSendToken;

                this.sendRecEventArgsPool.Push(sendRecEA);
            }
        }

        /// <summary>
        /// Starts listening for connection requests.
        /// </summary>
        private void startListening() {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, Constants.DEFAULT_PORT);
            serverSocket = new Socket(localEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(localEndPoint);

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
            if (!pending) {
                processAccept(acceptEA);
            }
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
            if (acceptEA.SocketError != SocketError.Success) {
                acceptEA.AcceptSocket.Close();
                this.acceptEventArgsPool.Push(acceptEA);
            }
            // continue accepting.
            startAccepting();
            // add to current client connections
            IPEndPoint client = acceptEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
            // report back
            controller.Print("\"" + client.Address + "\" connected.");

            // start receiving for accepted client connection
            SocketAsyncEventArgs sendRecEA = getSendReceiveEventArgs(client);
            sendRecEA.AcceptSocket = acceptEA.AcceptSocket;
            this.acceptEventArgsPool.Push(acceptEA);
            startReceiving(sendRecEA);
        }

        /// <summary>
        /// Callback for accept event args.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="saea">The accept event args.</param>
        private void onAccept_Completed(object sender, SocketAsyncEventArgs op) {
            processAccept(op);
        }

        /// <summary>
        /// Starts the receiving process.
        /// </summary>
        /// <param name="receiveEA">The receive event args.</param>
        private void startReceiving(SocketAsyncEventArgs receiveEA) {
            // set the buffer
            DataUserToken token = (DataUserToken)receiveEA.UserToken;
            receiveEA.SetBuffer(token.ReceiveBufferOffset, Constants.BUFFER_SIZE);

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
            DataUserToken token = (DataUserToken)receiveEA.UserToken;
            // close connection on socket error
            if (receiveEA.SocketError != SocketError.Success) {
                token.Reset();
                closeSocket(receiveEA.AcceptSocket);
                this.sendRecEventArgsPool.Push(receiveEA);
                // report back
                IPEndPoint client = receiveEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
                controller.Print("A socket error occured while receiving data. Closing connection to \""
                    + client.Address + "\".");
                return;
            }

            // client closed the socket voluntarily.
            if (receiveEA.BytesTransferred == 0) {
                // report back
                IPEndPoint client = receiveEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
                controller.Print("Client \"" + client.Address + "\" disconnected!");

                token.Reset();
                this.sendRecEventArgsPool.Push(receiveEA);
                return;
            }

            // handle the prefix
            int bytesToProcess = receiveEA.BytesTransferred;
            if (token.PrefixBytesReceived < Constants.MSG_PREFIX_LENGTH) {
                bytesToProcess = messageHandler.HandlePrefix(receiveEA, token, bytesToProcess);
                // post another receive operation in case prefix is incomplete
                if (bytesToProcess == 0) {
                    startReceiving(receiveEA);
                    return;
                }
            }

            // handle the message content
            bool msgComplete = messageHandler.HandleMessage(receiveEA, token, bytesToProcess);
            if (msgComplete) {
                // handle received data                      
                string text = token.DataHandler.HandleTextData(receiveEA);
                // report back
                IPEndPoint client = receiveEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
                controller.Print(client.Address + ": \"" + text + "\"");
                token.Reset();
            } else {
                token.MessageOffset = token.ReceiveBufferOffset;
                token.PrefixBytesProcessed = 0;
            }
            startReceiving(receiveEA);
        }

        /// <summary>
        /// Connects to a host.
        /// </summary>
        /// <param name="ip">The host ip.</param>
        public void ConnectTo(string ip) {
            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint endPoint = new IPEndPoint(address, Constants.DEFAULT_PORT);
            // try to connect
            SocketAsyncEventArgs connectOp = getConnectEventArgs(endPoint);
            if (!this.connections.ContainsKey(endPoint)) {
                startConnecting(connectOp);
            } else {
                controller.Print("Already connected to host (" + ip + ").");
            }
        }

        /// <summary>
        /// Gets or creates connect event args for a given endpoint.
        /// </summary>
        /// <param name="ep">The endpoint.</param>
        private SocketAsyncEventArgs getConnectEventArgs(IPEndPoint ep) {
            SocketAsyncEventArgs connectEA = this.connectEventArgsPool.Pop();
            if (connectEA == null) {
                connectEA = new SocketAsyncEventArgs();
                connectEA.Completed += new EventHandler<SocketAsyncEventArgs>(onConnect_Completed);
            }
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            connectEA.RemoteEndPoint = ep;
            connectEA.AcceptSocket = clientSocket;
            return connectEA;
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
                    processSend(saea);
                    break;

                case SocketAsyncOperation.Receive:
                    processReceive(saea);
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
                this.connections.Add(host, saea);
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
            this.connections.Remove(host);
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
            startDisconnecting(this.connections[endPoint]);
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
            this.connections.Remove(host);
            this.connectEventArgsPool.Push(saea);
            // visualize
            controller.Disconnected(hostAddress);
        }

        /// <summary>
        /// Sends a text message to a connected host.
        /// </summary>
        /// <param name="ip">The host ip.</param>
        /// <param name="msg">The text message to send.</param>
        internal void SendMessageTo(string ip, string msg) {
            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint endPoint = new IPEndPoint(address, Constants.DEFAULT_PORT);

            // try to send
            if (this.connections.ContainsKey(endPoint)) {
                SocketAsyncEventArgs sendRecEA = getSendReceiveEventArgs(endPoint);
                // assign socket from current connections
                sendRecEA.AcceptSocket = this.connections[endPoint].AcceptSocket;
                // build tcp message
                this.messageBuilder.BuildTextData(msg, sendRecEA);
                startSending(sendRecEA);
            } else {
                throw new ArgumentException("Cannot send text message to unconnected host \""
                    + endPoint.Address.ToString() + "\"!");
            }
        }

        /// <summary>
        /// Gets or creates send/receive event args for a given endpoint.
        /// </summary>
        /// <param name="ep">The endpoint.</param>
        private SocketAsyncEventArgs getSendReceiveEventArgs(IPEndPoint ep) {
            SocketAsyncEventArgs sendRecEA = this.sendRecEventArgsPool.Pop();
            if (sendRecEA == null) {
                sendRecEA = new SocketAsyncEventArgs();
                sendRecEA.Completed += new EventHandler<SocketAsyncEventArgs>(onSendRec_Completed);
                DataUserToken receiveSendToken = new DataUserToken(sendRecEA);
                sendRecEA.UserToken = receiveSendToken;
            }
            sendRecEA.RemoteEndPoint = ep;
            return sendRecEA;
        }

        /// <summary>
        /// Starts the (text) sending process.
        /// </summary>
        /// <param name="sendRecEA">The send/receive event args.</param>
        private void startSending(SocketAsyncEventArgs sendRecEA) {
            DataUserToken token = (DataUserToken)sendRecEA.UserToken;
            if (token.RemainingBytesToSend <= Constants.BUFFER_SIZE) {
                // data fits into buffer
                sendRecEA.SetBuffer(token.SendBufferOffset, token.RemainingBytesToSend);
                //Copy the bytes to the buffer associated with the event args.
                Buffer.BlockCopy(token.SendData, token.BytesSent,
                    sendRecEA.Buffer, token.SendBufferOffset, token.RemainingBytesToSend);
            } else {
                // data doesn't fit into buffer so send as much as possible
                sendRecEA.SetBuffer(token.SendBufferOffset, Constants.BUFFER_SIZE);
                // copy bytes to the buffer associated with the event args.
                Buffer.BlockCopy(token.SendData, token.BytesSent,
                    sendRecEA.Buffer, token.SendBufferOffset, Constants.BUFFER_SIZE);
            }

            // post the send operation
            bool pending = sendRecEA.AcceptSocket.SendAsync(sendRecEA);
            if (!pending) {
                processSend(sendRecEA);
            }
        }

        /// <summary>
        /// Processes a send operation of given event args.
        /// </summary>
        /// <param name="sendEA">The event args.</param>
        private void processSend(SocketAsyncEventArgs sendEA) {
            DataUserToken token = (DataUserToken)sendEA.UserToken;
            if (sendEA.SocketError == SocketError.Success) {
                token.RemainingBytesToSend -= sendEA.BytesTransferred;
                if (token.RemainingBytesToSend != 0) {
                    // not all data has been successfully transfered yet
                    token.BytesSent += sendEA.BytesTransferred;
                    startSending(sendEA);
                } else {
                    this.sendRecEventArgsPool.Push(sendEA);
                    //report back
                    controller.Print("Text message transfer complete.");
                }
            } else {
                // a socket error occured
                token.Reset();
                startDisconnecting(sendEA);
            }
        }

        /// <summary>
        /// Sends a file to a connected host.
        /// </summary>
        /// <param name="ip">The host ip.</param>
        /// <param name="fileInfo">The file information.</param>
        internal void SendFileTo(string ip, FileInfo fileInfo) {
            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint endPoint = new IPEndPoint(address, Constants.DEFAULT_PORT);
            // try to send
            if (this.connections.ContainsKey(endPoint)) {
                SocketAsyncEventArgs sendRecEA = getSendReceiveEventArgs(endPoint);
                // assign socket from current connections
                sendRecEA.AcceptSocket = this.connections[endPoint].AcceptSocket;
                // build tcp message
                this.messageBuilder.BuildFileInfoData(fileInfo, sendRecEA);
                startSendingFile(sendRecEA);
            } else {
                throw new ArgumentException("Cannot send file to unconnected host \""
                    + endPoint.Address.ToString() + "\"!");
            }
        }

        /// <summary>
        /// Starts the (file) sending process.
        /// </summary>
        /// <param name="sendRecEA">The send/receive event args.</param>
        private void startSendingFile(SocketAsyncEventArgs sendRecEA) {
            DataUserToken token = (DataUserToken)sendRecEA.UserToken;

            // transfer whole message
            if (token.RemainingBytesToSend <= Constants.BUFFER_SIZE) {
                Console.WriteLine("prefix+file fit into buffer.");
            }
            // transfer message blockwise
            else {
                // transfer whole prefix
                if (Constants.MSG_PREFIX_LENGTH <= Constants.BUFFER_SIZE) {
                    Console.WriteLine("prefix fits into buffer.");
                }
                // transfer prefix blockwise
                else {
                    Console.WriteLine("prefix does not fit into buffer.");
                }
            }
            
            // post the send operation
            bool pending = sendRecEA.AcceptSocket.SendAsync(sendRecEA);
            if (!pending) {
                processFileSend(sendRecEA);
            }
        }

        private void processFileSend(SocketAsyncEventArgs sendEA) {
            DataUserToken token = (DataUserToken)sendEA.UserToken;
            if (sendEA.SocketError == SocketError.Success) {
                token.RemainingBytesToSend -= sendEA.BytesTransferred;
                if (token.RemainingBytesToSend != 0) {
                    // not the whole message has been successfully transfered yet
                    token.BytesSent += sendEA.BytesTransferred;
                    startSending(sendEA);
                } else {
                    this.sendRecEventArgsPool.Push(sendEA);
                    //report back
                    controller.Print("File transfer complete.");
                }
            } else {
                // a socket error occured
                token.Reset();
                startDisconnecting(sendEA);
            }
        }
    }
}
