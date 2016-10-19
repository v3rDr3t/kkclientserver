using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KKClientServer.Receiver {

    public class SendRecToken {
        #region Fields (SendRecToken)
        private DataHandler handler;
        private Data data;

        private Int32 socketHandleNumber;

        // receiving
        private readonly Int32 bufferOffset;
        private readonly Int32 permanentReceiveMessageOffset;
        private Int32 incomingMsgLength;

        private readonly Int32 receivePrefixLength;
        private Byte[] prefix;
        private Int32 receiveMessageOffset;
        internal Int32 receivedPrefixBytesDoneCount = 0;
        internal Int32 receivedMessageBytesDoneCount = 0;
        internal Int32 recPrefixBytesDoneThisOp = 0;

        // sending
        private readonly Int32 sendPrefixLength;
        private Int32 sendBytesRemaining;
        private Int32 bytesSent;
        private Byte[] dataToSend;

        private Int32 sessionId;
        #endregion

        public SendRecToken(SocketAsyncEventArgs op, Int32 offset, Int32 recPrefixLength, Int32 sendPrefixLength) {
            this.handler = new DataHandler(op);
            this.bufferOffset = offset;
            this.receivePrefixLength = recPrefixLength;
            this.sendPrefixLength = sendPrefixLength;
            this.receiveMessageOffset = offset + receivePrefixLength;
            this.permanentReceiveMessageOffset = this.receiveMessageOffset;
        }

        internal void CreateNewData() {
            this.data = new Data();
        }

        internal void CreateSessionId() {
            sessionId = Interlocked.Increment(ref Globals.Session_Id);
        }

        public void Reset() {
            this.receivedPrefixBytesDoneCount = 0;
            this.receivedMessageBytesDoneCount = 0;
            this.recPrefixBytesDoneThisOp = 0;
            this.receiveMessageOffset = this.permanentReceiveMessageOffset;
        }

        #region Properties (SendRecToken)
        public Int32 SessionId {
            get {
                return this.sessionId;
            }
        }

        public Int32 BufferOffset {
            get {
                return this.bufferOffset;
            }
        }

        public DataHandler DataHandler {
            get {
                return this.handler;
            }
            set {
                this.handler = value;
            }
        }

        public Data Data {
            get {
                return this.data;
            }
            set {
                this.data = value;
            }
        }

        public Int32 IncomingMessageLength {
            get {
                return this.incomingMsgLength;
            }
            set {
                this.incomingMsgLength = value;
            }
        }

        public Int32 ReceivedPrefixBytes {
            get {
                return this.receivedPrefixBytesDoneCount;
            }
            set {
                this.receivedPrefixBytesDoneCount = value;
            }
        }

        public Int32 ReceivedMessageBytes {
            get {
                return this.receivedMessageBytesDoneCount;
            }
            set {
                this.receivedMessageBytesDoneCount = value;
            }
        }

        public Byte[] Prefix {
            get {
                return this.prefix;
            }
            set {
                this.prefix = value;
            }
        }

        public Int32 ReceivePrefixLength {
            get {
                return this.receivePrefixLength;
            }
        }

        public Int32 SendPrefixLength {
            get {
                return this.sendPrefixLength;
            }
        }

        public Int32 ReceiveMessageOffset {
            get {
                return this.receiveMessageOffset;
            }
            set {
                this.receiveMessageOffset = value;
            }
        }

        public Int32 SendBytesRemaining {
            get {
                return this.sendBytesRemaining;
            }
            set {
                this.sendBytesRemaining = value;
            }
        }

        public Int32 BytesSent {
            get {
                return this.bytesSent;
            }
            set {
                this.bytesSent = value;
            }
        }

        public Byte[] DataToSend {
            get {
                return this.dataToSend;
            }
            set {
                this.dataToSend = value;
            }
        }
        #endregion
    }
}
