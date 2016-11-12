using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer.Networking {

    internal class BaseToken {
        #region Fields
        // the buffer offset for receive operations
        private readonly int bufferOffset;
        // message type read from prefix
        private MessageType type;
        // The prefix byte array (to send/receive)
        private byte[] prefix;
        // The data byte array (to send/receive)
        private byte[] data;
        // The size of the message to send/receive
        private long messageSize;
        #endregion

        /// <summary>
        /// Constructs a <see cref="BaseToken"/> object.
        /// </summary>
        /// <param name="saea">The event args.</param>
        public BaseToken(SocketAsyncEventArgs saea) {
            // set buffer offset
            this.bufferOffset = saea.Offset;
        }

        #region Properties
        public int BufferOffset {
            get { return this.bufferOffset; }
        }

        public MessageType Type {
            get { return this.type; }
            set { this.type = value; }
        }

        public byte[] Prefix {
            get { return this.prefix; }
            set { this.prefix = value; }
        }

        public byte[] Data {
            get { return this.data; }
            set { this.data = value; }
        }

        public long MessageSize {
            get { return this.messageSize; }
            set { this.messageSize = value; }
        }
        #endregion
    }
}
