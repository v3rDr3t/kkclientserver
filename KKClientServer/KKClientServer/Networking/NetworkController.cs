using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;
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
        private ReceiveMessageHandler receiveHandler;
        // The send message handler
        private SendMessageHandler sendHandler;
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
            this.sendRecEventArgsPool = new SocketOperationPool(Constants.MAX_ASYNC_SEND_REC_OPS);
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
            this.buffer.Initialize();
            
            // preallocate send/receive pool
            SocketAsyncEventArgs sendRecEA;
            for (int i = 0; i < 5; i++) {
                sendRecEA = new SocketAsyncEventArgs();
                sendRecEA.Completed += new EventHandler<SocketAsyncEventArgs>(onSendRec_Completed);
                this.buffer.Set(sendRecEA);
                // assign transfer data object
                TransferData token = new TransferData(sendRecEA);
                sendRecEA.UserToken = token;

                this.sendRecEventArgsPool.Push(sendRecEA);
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

            // report back
            IPEndPoint client = acceptEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
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
        private void onAccept_Completed(object sender, SocketAsyncEventArgs saea) {
            processAccept(saea);
        }

        /// <summary>
        /// Starts the receiving process.
        /// </summary>
        /// <param name="receiveEA">The receive event args.</param>
        private void startReceiving(SocketAsyncEventArgs receiveEA) {
            // set the buffer
            TransferData token = (TransferData)receiveEA.UserToken;
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
            TransferData token = (TransferData)receiveEA.UserToken;
            // close connection on socket error
            if (receiveEA.SocketError != SocketError.Success) {
                // report back
                IPEndPoint client = receiveEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
                controller.Print("A socket error occured while receiving data. Closing connection to \""
                    + client.Address + "\".");

                token.Reset();
                closeSocket(receiveEA.AcceptSocket);
                this.sendRecEventArgsPool.Push(receiveEA);
                return;
            }

            // client closed the socket voluntarily.
            if (receiveEA.BytesTransferred == 0) {
                // report back
                IPEndPoint client = receiveEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
                controller.Print("Client \"" + client.Address + "\" disconnected!");

                token.Reset();
                closeSocket(receiveEA.AcceptSocket);
                this.sendRecEventArgsPool.Push(receiveEA);
                return;
            }

            // handle the prefix
            int bytesToProcess = receiveEA.BytesTransferred;
            Console.WriteLine("\nReceive> Received " + bytesToProcess + " bytes.");
            if (token.PrefixBytesReceived < Constants.PREFIX_SIZE) {
                bytesToProcess = receiveHandler.HandlePrefix(receiveEA, token, bytesToProcess);
                // post another receive operation in case prefix is incomplete
                if (bytesToProcess == 0) {
                    startReceiving(receiveEA);
                    return;
                }
            }

            // handle the text data
            Console.WriteLine("Receive> " + bytesToProcess + " text bytes to process.");
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
                    string text = token.GetReceivedText(receiveEA);
                    IPEndPoint client = receiveEA.AcceptSocket.RemoteEndPoint as IPEndPoint;
                    controller.Print(client.Address + ": File \"" + text + "\"");

                    token.Reset();
                } else {
                    token.FileOffset = token.ReceiveBufferOffset;
                    Console.WriteLine("Receive> FileOffset = " + token.FileOffset);
                }
            } else {
                // report back                    
                string text = token.GetReceivedText(receiveEA);
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
            if (!pending) {
                processConnect(connectEA);
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
                    throw new ArgumentException("Unknown completed operation for connect!");
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
                    TransferData token = (TransferData)saea.UserToken;
                    if (token.Type == MessageType.Text) {
                        processSendText(saea);
                    } else {
                        processSendFile(saea);
                    }
                    break;

                case SocketAsyncOperation.Receive:
                    processReceive(saea);
                    break;

                default:
                    throw new ArgumentException("Unknown completed operation for send/receive!");
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
            this.connections.Remove(remoteEP);
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
            socket.Close();
        }

        /// <summary>
        /// Disconnects from a host.
        /// </summary>
        /// <param name="ip">The ip address.</param>
        public void DisconnectFrom(string ip) {
            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(address, Constants.DEFAULT_PORT);
            // try to disconnect
            startDisconnecting(this.connections[remoteEP]);
        }

        /// <summary>
        /// Starts the disconnection process.
        /// </summary>
        /// <param name="saea">The event args.</param>
        private void startDisconnecting(SocketAsyncEventArgs saea) {
            //saea.AcceptSocket.Shutdown(SocketShutdown.Both);
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
                SocketAsyncEventArgs sendEA = getSendReceiveEventArgs(remoteEP);
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
        /// Gets or creates send/receive event args for a given remote endpoint.
        /// </summary>
        /// <param name="remoteEP">The remote endpoint.</param>
        private SocketAsyncEventArgs getSendReceiveEventArgs(IPEndPoint remoteEP) {
            SocketAsyncEventArgs sendRecEA = this.sendRecEventArgsPool.Pop();
            if (sendRecEA == null) {
                sendRecEA = new SocketAsyncEventArgs();
                sendRecEA.Completed += new EventHandler<SocketAsyncEventArgs>(onSendRec_Completed);
                // assign transfer data object
                TransferData token = new TransferData(sendRecEA);
                sendRecEA.UserToken = token;
            }
            sendRecEA.RemoteEndPoint = remoteEP;
            return sendRecEA;
        }

        /// <summary>
        /// Starts the sending process for text.
        /// </summary>
        /// <param name="sendEA">The send event args.</param>
        private void startSendingText(SocketAsyncEventArgs sendEA) {
            TransferData token = (TransferData)sendEA.UserToken;
            int bytesLeft = checkedConversion(token.RemainingBytesToSend, "startSendingText: ulong to int");
            int bytesSent = checkedConversion(token.BytesSent, "startSendingText: ulong to int");

            // message fits into buffer
            Console.WriteLine("Send> Bytes left to send: " + bytesLeft);
            if (bytesLeft <= Constants.BUFFER_SIZE) {
                sendEA.SetBuffer(token.SendBufferOffset, bytesLeft);
                Buffer.BlockCopy(
                    token.TextData, bytesSent,
                    sendEA.Buffer, token.SendBufferOffset,
                    bytesLeft);
                byte[] array = new byte[bytesLeft];
                Buffer.BlockCopy(sendEA.Buffer, token.SendBufferOffset, array, 0, bytesLeft);
                Console.WriteLine("Send> Sending bytes (complete): " + BitConverter.ToString(array) + "\n");
            }
            // message doesn't fit into buffer (send as much as possible)
            else {
                sendEA.SetBuffer(token.SendBufferOffset, Constants.BUFFER_SIZE);
                Buffer.BlockCopy(
                    token.TextData, bytesSent,
                    sendEA.Buffer, token.SendBufferOffset,
                    Constants.BUFFER_SIZE);
                byte[] array = new byte[Constants.BUFFER_SIZE];
                Buffer.BlockCopy(sendEA.Buffer, token.SendBufferOffset, array, 0, Constants.BUFFER_SIZE);
                Console.WriteLine("Send> Sending bytes (incomplete): " + BitConverter.ToString(array) + "\n");
            }

            // post the send operation
            bool pending = sendEA.AcceptSocket.SendAsync(sendEA);
            if (!pending) {
                processSendText(sendEA);
            }
        }

        /// <summary>
        /// Processes a file sending operation.
        /// </summary>
        /// <param name="sendEA">The send event args.</param>
        private void processSendText(SocketAsyncEventArgs sendEA) {
            TransferData token = (TransferData)sendEA.UserToken;

            if (sendEA.SocketError == SocketError.Success) {
                token.RemainingBytesToSend -= sendEA.BytesTransferred;
                Console.WriteLine("Send> RemainingBytesToSend: " + token.RemainingBytesToSend);
                if (token.RemainingBytesToSend != 0L) {
                    // not all data has been successfully transfered yet
                    token.BytesSent += sendEA.BytesTransferred;
                    startSendingText(sendEA);
                } else {
                    this.sendRecEventArgsPool.Push(sendEA);
                    //report back
                    controller.Print("Text transfer complete.");
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
            Console.WriteLine("Send> Send file to...");
            IPAddress address = IPAddress.Parse(ip);
            IPEndPoint remoteEP = new IPEndPoint(address, Constants.DEFAULT_PORT);
            // try to send
            if (this.connections.ContainsKey(remoteEP)) {
                SocketAsyncEventArgs sendRecEA = getSendReceiveEventArgs(remoteEP);
                // assign socket from current connections
                sendRecEA.AcceptSocket = this.connections[remoteEP].AcceptSocket;
                // build tcp message
                this.messageBuilder.BuildFileInfoData(fileInfo, sendRecEA);
                startSendingFile(sendRecEA);
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
            TransferData token = (TransferData)sendEA.UserToken;
            int bytesLeft = checkedConversion(token.RemainingBytesToSend, "startSendingFile");

            // message fits into buffer
            if (bytesLeft <= Constants.BUFFER_SIZE) {
                sendEA.SetBuffer(token.SendBufferOffset, bytesLeft);
            }
            // message doesn't fit into buffer (send as much as possible)
            else {
                sendEA.SetBuffer(token.SendBufferOffset, Constants.BUFFER_SIZE);
            }
            // handle prefix
            long fileDataToProcess = sendHandler.HandlePrefixAndFileName(sendEA, token);
            // handle file
            sendHandler.HandleFile(sendEA, token, fileDataToProcess);
            
            // post the send operation
            bool pending = sendEA.AcceptSocket.SendAsync(sendEA);
            if (!pending) {
                processSendFile(sendEA);
            }
        }

        /// <summary>
        /// Processes a file sending operation.
        /// </summary>
        /// <param name="sendEA">The send event args.</param>
        private void processSendFile(SocketAsyncEventArgs sendEA) {
            TransferData token = (TransferData)sendEA.UserToken;

            if (sendEA.SocketError == SocketError.Success) {
                token.RemainingBytesToSend -= sendEA.BytesTransferred;
                if (token.RemainingBytesToSend > 0L) {
                    // not the whole message has been successfully transfered yet
                    token.BytesSent += sendEA.BytesTransferred;
                    int toSend = token.PrefixAndFileNameBytesToSend - sendEA.BytesTransferred;
                    token.PrefixAndFileNameBytesToSend = (toSend > 0) ? toSend : 0;
                    startSendingFile(sendEA);
                } else {
                    token.Reset();
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

        /// <summary>
        /// Performs a checked convertion of a given signed long to integer.
        /// </summary>
        /// <param name="n">The signed long.</param>
        /// <param name="err">An optional error message.</param>
        public static int checkedConversion(long n, string err) {
            int ret;
            try {
                ret = checked((int)n);
            } catch (OverflowException ex) {
                ex.Data.Add("Message", err);
                throw;
            }
            return ret;
        }
    }
}
