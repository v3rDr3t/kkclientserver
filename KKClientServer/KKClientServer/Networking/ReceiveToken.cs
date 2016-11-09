using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Networking {

    internal class ReceiveToken : BaseToken {
        #region Fields
        // the buffer offset for receive operations
        private readonly int receiveBufferOffset;

        // Received bytes counters
        private int prefixBytesReceived = 0;
        private int prefixBytesProcessed = 0;

        private int textBytesReceived = 0;
        private int textBytesProcessed = 0;

        private long fileBytesReceived = 0;

        // The original buffer offset for text data
        private readonly int originalTextOffset;
        // The buffer offset for text data
        private int textOffset;
        // The buffer offset for file data
        private int fileOffset;

        // The file writer
        private BinaryWriter writer;

        bool respectPrefixRemainderForOffset;
        // -----------------------------------------------------

        // The Text length read from prefix
        private int textLength = 0;
        // The file length read from prefix
        private long fileLength = 0L;

        private string filePath = String.Empty;
        #endregion

        /// <summary>
        /// Constructs a <see cref="ReceiveToken"/> object.
        /// </summary>
        /// <param name="saea">The event args.</param>
        public ReceiveToken(SocketAsyncEventArgs saea) {
            // set buffer offset
            this.receiveBufferOffset = saea.Offset;
            // set text data offset
            this.textOffset = this.receiveBufferOffset + Constants.PREFIX_SIZE;
            this.originalTextOffset = this.textOffset;
            // set file data offset (will be appended when prefix is received)
            this.fileOffset = this.receiveBufferOffset + Constants.PREFIX_SIZE;
        }

        /// <summary>
        /// Gets the received text data as string.
        /// </summary>
        /// <param name="saea">The event args.</param>
        internal string GetReceivedText() {
            return Encoding.Default.GetString(base.TextData);
        }

        /// <summary>
        /// Resets the state of the token.
        /// </summary>
        internal void Reset() {
            // byte counters
            this.prefixBytesReceived = 0;
            this.prefixBytesProcessed = 0;
            this.textBytesReceived = 0;
            this.textBytesProcessed = 0;
            this.fileBytesReceived = 0;
            // data offsets
            this.textOffset = this.originalTextOffset;
            this.fileOffset = this.receiveBufferOffset + Constants.PREFIX_SIZE;
            // file writer
            if (this.writer != null) {
                this.writer.Close();
                this.writer = null;
            }
            this.textLength = 0;
            this.fileLength = 0L;
            this.filePath = String.Empty;
        }

        #region Properties
        public int ReceiveBufferOffset {
            get { return this.receiveBufferOffset; }
        }

        public int PrefixBytesReceived {
            get { return this.prefixBytesReceived; }
            set { this.prefixBytesReceived = value; }
        }
        public int PrefixBytesProcessed {
            get { return this.prefixBytesProcessed; }
            set { this.prefixBytesProcessed = value; }
        }

        public int TextBytesReceived {
            get { return this.textBytesReceived; }
            set { this.textBytesReceived = value; }
        }
        public int TextBytesProcessed {
            get { return this.textBytesProcessed; }
            set { this.textBytesProcessed = value; }
        }

        public long FileBytesReceived {
            get { return this.fileBytesReceived; }
            set { this.fileBytesReceived = value; }
        }

        public int TextOffset {
            get { return this.textOffset; }
            set { this.textOffset = value; }
        }

        public int FileOffset {
            get { return this.fileOffset; }
            set { this.fileOffset = value; }
        }

        public BinaryWriter Writer {
            get { return this.writer; }
            set { this.writer = value; }
        }
        
        public bool RespectPrefixRemainderForOffset {
            get { return this.respectPrefixRemainderForOffset; }
            set { this.respectPrefixRemainderForOffset = value; }
        }

        public int TextLength {
            get { return this.textLength; }
            set { this.textLength = value; }
        }

        public long FileLength {
            get { return this.fileLength; }
            set { this.fileLength = value; }
        }

        public string FilePath {
            get { return this.filePath; }
            set { this.filePath = value; }
        }
        #endregion
    }
}
