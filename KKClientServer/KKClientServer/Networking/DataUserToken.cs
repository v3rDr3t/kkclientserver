using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Networking.Client {

    internal class DataUserToken {
        #region Fields
        // the buffer offset for receive operations
        private readonly int receiveBufferOffset;
        // the buffer offset for send operations
        private readonly int sendBufferOffset;

        // the buffer offset for the actual message
        private readonly int originalMessageOffset;
        private int messageOffset;
        // received bytes counters
        private int prefixBytesReceived = 0;
        private int messageBytesReceived = 0;
        private int prefixBytesProcessed = 0;
        // message length read from prefix
        private int messageLength;

        // The byte array for prefix (receive)
        private byte[] prefixData;
        // The byte array for message (receive)
        private byte[] receiveData;

        // The byte array for message (send)
        private byte[] sendData;
        // The file stream (send)
        FileStream stream;
        // The byte sent counters
        private int remainingBytesToSend;
        private int bytesSent;

        // The data handler
        ReceiveDataHandler dataHandler;
        #endregion

        public DataUserToken(SocketAsyncEventArgs saea) {
            this.dataHandler = new ReceiveDataHandler();

            this.receiveBufferOffset = saea.Offset;
            this.sendBufferOffset = saea.Offset + Constants.BUFFER_SIZE;

            this.messageOffset = this.receiveBufferOffset + Constants.MSG_PREFIX_LENGTH;
            this.originalMessageOffset = this.messageOffset;
        }

        public void Reset() {
            this.messageOffset = this.originalMessageOffset;
            this.prefixBytesReceived = 0;
            this.messageBytesReceived = 0;
            this.prefixBytesProcessed = 0;
            this.prefixData = null;
            this.receiveData = null;
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
        public byte[] SendData {
            get { return this.sendData; }
            set { this.sendData = value; }
        }
        public FileStream FileStream {
            get { return this.stream; }
            set { this.stream = value; }
        }
        public byte[] PrefixData {
            get { return this.prefixData; }
            set { this.prefixData = value; }
        }
        public byte[] ReceiveData {
            get { return this.receiveData; }
            set { this.receiveData = value; }
        }
        public int RemainingBytesToSend {
            get { return this.remainingBytesToSend; }
            set { this.remainingBytesToSend = value; }
        }
        public int BytesSent {
            get { return this.bytesSent; }
            set { this.bytesSent = value; }
        }
        public int MessageLength {
            get { return this.messageLength; }
            set { this.messageLength = value; }
        }
        public ReceiveDataHandler DataHandler {
            get { return this.dataHandler; }
        }
        #endregion
    }
}
