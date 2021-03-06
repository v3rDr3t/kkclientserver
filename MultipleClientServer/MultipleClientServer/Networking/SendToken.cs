﻿using System.IO;
using System.Net.Sockets;

namespace ClientServer.Networking {

    internal class SendToken : BaseToken {
        #region Fields
        // The total byte counters
        private long remainingBytesToSend;
        private long bytesSent;

        // The file stream
        private FileStream stream;
        // The remaining prefix bytes to send
        private int prefixAndFileNameBytesToSend;

        // The file name
        string text;
        #endregion

        /// <summary>
        /// Constructs a <see cref="SendToken"/> object.
        /// </summary>
        /// <param name="saea">The event args.</param>
        public SendToken(SocketAsyncEventArgs saea) : base (saea) { }

        /// <summary>
        /// Resets the state of the token.
        /// </summary>
        internal void Reset() {
            // file stream
            if (this.stream != null) {
                this.stream.Close();
                this.stream = null;
            }
            this.text = string.Empty;
        }

        #region Properties
        public long RemainingBytesToSend {
            get { return this.remainingBytesToSend; }
            set { this.remainingBytesToSend = value; }
        }

        public long BytesSent {
            get { return this.bytesSent; }
            set { this.bytesSent = value; }
        }

        public FileStream FileStream {
            get { return this.stream; }
            set { this.stream = value; }
        }

        public int PrefixAndFileNameBytesToSend {
            get { return this.prefixAndFileNameBytesToSend; }
            set { this.prefixAndFileNameBytesToSend = value; }
        }

        public string Text {
            get { return this.text; }
            set { this.text = value; }
        }
        #endregion
    }
}
