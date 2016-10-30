using System.IO;
using System.Net.Sockets;
using System.Text;

namespace KKClientServer.Networking {

    internal class TransferData {
        #region Fields
        // message type read from prefix
        private MessageType type;

        // the buffer offset for receive operations
        private readonly int receiveBufferOffset;
        // the buffer offset for send operations
        private readonly int sendBufferOffset;

        // The byte sent counters
        private long remainingBytesToSend;
        private long bytesSent;

        // send file -------------------------------------------
        // The file stream
        private FileStream stream;
        // The remaining prefix bytes to send
        private int prefixAndFileNameBytesToSend;

        // receive ---------------------------------------------
        // The prefix byte array
        private byte[] prefix;
        // The data byte array
        private byte[] textData;
        // Received bytes counters
        private int prefixBytesReceived = 0;
        private int prefixBytesProcessed = 0;
        private int textBytesReceived = 0;
        private int textBytesProcessed = 0;
        private int fileBytesReceived = 0;
        // The buffer offset for the data
        private readonly int originalTextOffset;
        private int textOffset;
        private int fileOffset;
        // The file writer
        private BinaryWriter writer;
        // -----------------------------------------------------

        // text length read from prefix
        private int textLength;
        private long fileLength;
        #endregion

        /// <summary>
        /// Constructs a <see cref="TransferData"/> object.
        /// </summary>
        /// <param name="saea">The event args.</param>
        public TransferData(SocketAsyncEventArgs saea) {
            // set buffer offset
            this.receiveBufferOffset = saea.Offset;
            this.sendBufferOffset = saea.Offset + Constants.BUFFER_SIZE;
            // set data offset
            this.textOffset = this.receiveBufferOffset + Constants.PREFIX_SIZE;
            this.originalTextOffset = this.textOffset;
        }

        /// <summary>
        /// Gets the received text data as string.
        /// </summary>
        /// <param name="saea">The event args.</param>
        internal string GetReceivedText(SocketAsyncEventArgs saea) {
            TransferData token = (TransferData)saea.UserToken;
            return Encoding.Default.GetString(token.TextData);
        }

        /// <summary>
        /// Resets the state of the token.
        /// </summary>
        internal void Reset() {
            this.prefixBytesReceived = 0;
            this.prefixBytesProcessed = 0;
            this.textBytesReceived = 0;
            this.textBytesProcessed = 0;
            this.fileBytesReceived = 0;
            this.textOffset = this.originalTextOffset;

            if (this.stream != null) {
                this.stream.Close();
                this.stream = null;
            }
            if (this.writer != null) {
                this.writer.Close();
                this.writer = null;
            }
        }

        #region Properties
        public MessageType Type {
            get { return this.type; }
            set { this.type = value; }
        }

        public int ReceiveBufferOffset {
            get { return this.receiveBufferOffset; }
        }

        public int SendBufferOffset {
            get { return this.sendBufferOffset; }
        }

        public long RemainingBytesToSend {
            get { return this.remainingBytesToSend; }
            set { this.remainingBytesToSend = value; }
        }

        public long BytesSent {
            get { return this.bytesSent; }
            set { this.bytesSent = value; }
        }

        public byte[] Prefix {
            get { return this.prefix; }
            set { this.prefix = value; }
        }

        public byte[] TextData {
            get { return this.textData; }
            set { this.textData = value; }
        }

        public int TextOffset {
            get { return this.textOffset; }
            set { this.textOffset = value; }
        }

        public int FileOffset {
            get { return this.fileOffset; }
            set { this.fileOffset = value; }
        }

        public int PrefixBytesReceived {
            get { return this.prefixBytesReceived; }
            set { this.prefixBytesReceived = value; }
        }

        public int TextBytesReceived {
            get { return this.textBytesReceived; }
            set { this.textBytesReceived = value; }
        }

        public int FileBytesReceived {
            get { return this.fileBytesReceived; }
            set { this.fileBytesReceived= value; }
        }

        public int PrefixBytesProcessed {
            get { return this.prefixBytesProcessed; }
            set { this.prefixBytesProcessed = value; }
        }

        public int TextBytesProcessed {
            get { return this.textBytesProcessed; }
            set { this.textBytesProcessed = value; }
        }

        public FileStream FileStream {
            get { return this.stream; }
            set { this.stream = value; }
        }

        public BinaryWriter Writer {
            get { return this.writer; }
            set { this.writer = value; }
        }

        public int TextLength {
            get { return this.textLength; }
            set { this.textLength = value; }
        }

        public long FileLength {
            get { return this.fileLength; }
            set { this.fileLength = value; }
        }

        public int PrefixAndFileNameBytesToSend {
            get { return this.prefixAndFileNameBytesToSend; }
            set { this.prefixAndFileNameBytesToSend = value; }
        }
        #endregion
    }
}
