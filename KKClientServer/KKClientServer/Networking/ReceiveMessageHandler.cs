using KKClientServer.Networking.Client;
using System;
using System.Net.Sockets;
using System.Text;

namespace KKClientServer.Networking {

    internal class ReceiveMessageHandler {


        public int HandlePrefix(SocketAsyncEventArgs receiveEA, DataUserToken token, int bytesToProcess) {
            if (token.PrefixBytesReceived == 0) {
                token.PrefixData = new Byte[Constants.MSG_PREFIX_LENGTH];
            }

            if (bytesToProcess >= Constants.MSG_PREFIX_LENGTH - token.PrefixBytesReceived) {
                // prefix has been received completely
                Buffer.BlockCopy(
                    receiveEA.Buffer,
                    token.MessageOffset - Constants.MSG_PREFIX_LENGTH + token.PrefixBytesReceived,
                    token.PrefixData,
                    token.PrefixBytesReceived,
                    Constants.MSG_PREFIX_LENGTH - token.PrefixBytesReceived);
                
                bytesToProcess = bytesToProcess - Constants.MSG_PREFIX_LENGTH + token.PrefixBytesReceived;
                token.PrefixBytesProcessed = Constants.MSG_PREFIX_LENGTH - token.PrefixBytesReceived;
                token.PrefixBytesReceived = Constants.MSG_PREFIX_LENGTH;
                token.MessageLength = BitConverter.ToInt32(token.PrefixData, 1);
            } else {
                // prefix has been received incompletely
                Buffer.BlockCopy(
                    receiveEA.Buffer,
                    token.MessageOffset - Constants.MSG_PREFIX_LENGTH + token.PrefixBytesReceived,
                    token.PrefixData,
                    token.PrefixBytesReceived,
                    bytesToProcess);

                token.PrefixBytesProcessed = bytesToProcess;
                token.PrefixBytesReceived += bytesToProcess;
                bytesToProcess = 0;
            }
            
            if (bytesToProcess == 0) {
                token.MessageOffset -= token.PrefixBytesProcessed;
                token.PrefixBytesProcessed = 0;
            }
            return bytesToProcess;
        }

        public bool HandleMessage(SocketAsyncEventArgs receiveEA, DataUserToken token, int bytesToProcess) {
            bool msgComplete = false;

            // create the message byte array
            if (token.MessageBytesReceived == 0) {
                token.ReceiveData = new Byte[token.MessageLength];
            }

            if (bytesToProcess + token.MessageBytesReceived == token.MessageLength) {
                // message has been received completely.
                Buffer.BlockCopy(
                    receiveEA.Buffer,
                    token.MessageOffset,
                    token.ReceiveData,
                    token.MessageBytesReceived,
                    bytesToProcess);

                msgComplete = true;
            } else {
                // message has been received incompletely
                Buffer.BlockCopy(
                    receiveEA.Buffer,
                    token.MessageOffset,
                    token.ReceiveData,
                    token.MessageBytesReceived,
                    bytesToProcess);

                token.MessageOffset -= token.PrefixBytesProcessed;
                token.MessageBytesReceived += bytesToProcess;
            }

            return msgComplete;
        }
    }
}
