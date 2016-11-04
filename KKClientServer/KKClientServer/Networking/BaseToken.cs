using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Networking {

    internal class BaseToken {
        #region Fields
        // message type read from prefix
        private MessageType type;
        // The prefix byte array (to send/receive)
        private byte[] prefix;
        // The data byte array (to send/receive)
        private byte[] textData;
        // The size of the message to send/receive
        private long messageSize;
        #endregion

        #region Properties
        public MessageType Type {
            get { return this.type; }
            set { this.type = value; }
        }

        public byte[] Prefix {
            get { return this.prefix; }
            set { this.prefix = value; }
        }

        public byte[] TextData {
            get { return this.textData; }
            set { this.textData = value; }
        }

        public long MessageSize {
            get { return this.messageSize; }
            set { this.messageSize = value; }
        }
        #endregion
    }
}
