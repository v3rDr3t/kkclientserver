using KKClientServer.Client;
using System;
using System.IO;
using System.Net.Sockets;

namespace KKClientServer.Networking.Client {

    internal class TransferData {
        #region Fields
        // message type read from prefix
        private MessageType type;

        // the buffer offset for receive operations
        private readonly int receiveBufferOffset;
        // the buffer offset for send operations
        private readonly int sendBufferOffset;

        // The byte sent counters
        private ulong remainingBytesToSend;
        private ulong bytesSent;

        // the buffer offset for the actual message
        //private readonly int originalMessageOffset;
        //private int messageOffset;
        // received bytes counters
        //private int prefixBytesReceived = 0;
        //private int messageBytesReceived = 0;
        //private int prefixBytesProcessed = 0;
        // message length read from prefix
        //private int messageLength;

        // The byte array for prefix (receive)
        //private byte[] prefixData;
        // The byte array for message (receive)
        //private byte[] receiveData;

        // receive file ----------------------------------------
        //private string filePath = "Test.txt";
        //private BinaryWriter writer;

        // send file -------------------------------------------
        // The file stream
        private FileStream stream;
        // The remaining prefix bytes to send
        private int prefixBytesToSend;

        // send text -------------------------------------------
        // The byte array for message (send)
        private byte[] textData;
        #endregion

        public TransferData(SocketAsyncEventArgs saea) {
            //this.dataHandler = new ReceiveDataHandler();

            this.receiveBufferOffset = saea.Offset;
            this.sendBufferOffset = saea.Offset + Constants.BUFFER_SIZE;

            //this.messageOffset = this.receiveBufferOffset + Constants.MSG_PREFIX_LENGTH;
            //this.originalMessageOffset = this.messageOffset;
        }

        public void Reset() {
            this.messageOffset = this.originalMessageOffset;
            this.prefixBytesReceived = 0;
            this.messageBytesReceived = 0;
            this.prefixBytesProcessed = 0;
            this.prefixData = null;
            this.receiveData = null;
            if (this.stream != null) {
                this.stream.Close();
                this.stream = null;
            }
            if (this.writer != null) {
                Console.WriteLine("Receive> Close writer.");
                this.writer.Close();
                this.writer = null;
            }
        }

        #region Properties
        public int ReceiveBufferOffset {
            get { return this.receiveBufferOffset; }
        }
        public int SendBufferOffset {
            get { return this.sendBufferOffset; }
        }
        public int MessageOffset {
            get { return this.messageOffset; }
            set { this.messageOffset = value; }
        }
        public int PrefixBytesReceived {
            get { return this.prefixBytesReceived; }
            set { this.prefixBytesReceived = value; }
        }
        public int MessageBytesReceived {
            get { return this.messageBytesReceived; }
            set { this.messageBytesReceived = value; }
        }
        public int PrefixBytesProcessed {
            get { return this.prefixBytesProcessed; }
            set { this.prefixBytesProcessed = value; }
        }
        public byte[] TextData {
            get { return this.textData; }
            set { this.textData = value; }
        }
        public FileStream FileStream {
            get { return this.stream; }
            set { this.stream = value; }
        }
        public BinaryWriter Writer {
            get { return this.writer; }
            set { this.writer = value; }
        }
        public string FilePath {
            get { return this.filePath; }
            set { this.filePath = value; }
        }
        public byte[] PrefixData {
            get { return this.prefixData; }
            set { this.prefixData = value; }
        }
        public byte[] ReceiveData {
            get { return this.receiveData; }
            set { this.receiveData = value; }
        }
        public ulong RemainingBytesToSend {
            get { return this.remainingBytesToSend; }
            set { this.remainingBytesToSend = value; }
        }
        public ulong BytesSent {
            get { return this.bytesSent; }
            set { this.bytesSent = value; }
        }
        public int MessageLength {
            get { return this.messageLength; }
            set { this.messageLength = value; }
        }
        public MessageType Type {
            get { return this.type; }
            set { this.type = value; }
        }
        public int PrefixBytesToSend {
            get { return this.prefixBytesToSend; }
            set { this.prefixBytesToSend = value; }
        }
        public ReceiveDataHandler DataHandler {
            get { return this.dataHandler; }
        }
        #endregion
    }
}
