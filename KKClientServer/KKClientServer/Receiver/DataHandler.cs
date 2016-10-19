using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KKClientServer.Receiver {

    public class DataHandler {
        private IncomingDataPreparer theIncomingDataPreparer;
        private OutgoingDataPreparer theOutgoingDataPreparer;
        private Data data;
        private SocketAsyncEventArgs operation;
        private SendRecToken receiveSendToken;

        public DataHandler(SocketAsyncEventArgs e) {
            this.operation = e;
            this.theIncomingDataPreparer = new IncomingDataPreparer(this.operation);
            this.theOutgoingDataPreparer = new OutgoingDataPreparer();
        }


        internal void Handle(Data incData) {
            this.data = theIncomingDataPreparer.HandleReceivedData(incData, this.operation);
        }

        internal void PrepareOutgoingData() {
            theOutgoingDataPreparer.PrepareOutgoingData(this.operation, this.data);
        }

        internal SocketAsyncEventArgs ReturnOperation() {
            return this.operation;
        }
    }

    public class IncomingDataPreparer {
        private Data data;
        private SocketAsyncEventArgs operation;

        public IncomingDataPreparer(SocketAsyncEventArgs e) {
            this.operation = e;
        }

        internal Data HandleReceivedData(Data incData, SocketAsyncEventArgs op) {
            SendRecToken recToken = (SendRecToken)op.UserToken;
            this.data = incData;
            this.data.SessionId = recToken.SessionId;
            Int32 recTransId = Interlocked.Increment(ref Globals.Transmission_Id);
            this.data.ReceivedTransMissionId = recTransId;
            return this.data;
        }
    }

    public class OutgoingDataPreparer {

        private Data data;

        public OutgoingDataPreparer() {
        }

        internal void PrepareOutgoingData(SocketAsyncEventArgs op, Data outData) {
            SendRecToken sendToken = (SendRecToken)op.UserToken;
            this.data = outData;

            // convert the receivedTransMissionId to bytes
            Byte[] bTransId = BitConverter.GetBytes(this.data.ReceivedTransMissionId);
            
            Int32 outMessageLength = bTransId.Length + this.data.DataReceived.Length;
            Byte[] bLength = BitConverter.GetBytes(outMessageLength);
            sendToken.DataToSend = new Byte[sendToken.SendPrefixLength + outMessageLength];

            // copy the 3 things to the theUserToken.dataToSend
            Buffer.BlockCopy(bLength, 0, sendToken.DataToSend, 0, sendToken.SendPrefixLength);
            Buffer.BlockCopy(bTransId, 0, sendToken.DataToSend, sendToken.SendPrefixLength, bTransId.Length);
            // echo data received
            Buffer.BlockCopy(this.data.DataReceived, 0,
                sendToken.DataToSend, sendToken.SendPrefixLength + bTransId.Length, this.data.DataReceived.Length);

            sendToken.SendBytesRemaining = sendToken.SendPrefixLength + outMessageLength;
            sendToken.BytesSent = 0;
        }
    }
}
