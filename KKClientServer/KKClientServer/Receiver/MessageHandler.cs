using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Receiver {

    public class PrefixHandler {

        public Int32 Handle(SocketAsyncEventArgs op, SendRecToken token, Int32 bytesToProcess) {
            if (token.receivedPrefixBytesDoneCount == 0) {
                token.Prefix = new Byte[token.ReceivePrefixLength];
            }

            // If this next if-statement is true, then we have received >=
            // enough bytes to have the prefix. So we can determine the 
            // length of the message that we are working on.
            if (bytesToProcess >= token.ReceivePrefixLength - token.receivedPrefixBytesDoneCount) {
                //Now copy that many bytes to byteArrayForPrefix.
                //We can use the variable receiveMessageOffset as our main
                //index to show which index to get data from in the TCP
                //buffer.
                Buffer.BlockCopy(op.Buffer,
                    token.ReceiveMessageOffset - token.ReceivePrefixLength + token.receivedPrefixBytesDoneCount,
                    token.Prefix, token.receivedPrefixBytesDoneCount,
                    token.ReceivePrefixLength - token.receivedPrefixBytesDoneCount);

                bytesToProcess -= (token.ReceivePrefixLength + token.receivedPrefixBytesDoneCount);
                token.recPrefixBytesDoneThisOp = token.ReceivePrefixLength - token.receivedPrefixBytesDoneCount;
                token.receivedPrefixBytesDoneCount = token.ReceivePrefixLength;
                token.IncomingMessageLength = BitConverter.ToInt32(token.Prefix, 0);
            } else {
                //Write the bytes to the array where we are putting the
                //prefix data, to save for the next loop.
                Buffer.BlockCopy(op.Buffer,
                    token.ReceiveMessageOffset - token.ReceivePrefixLength + token.receivedPrefixBytesDoneCount,
                    token.Prefix,
                    token.receivedPrefixBytesDoneCount, bytesToProcess);

                token.recPrefixBytesDoneThisOp = bytesToProcess;
                token.receivedPrefixBytesDoneCount += bytesToProcess;
                bytesToProcess = 0;
            }

            // This section is needed when we have received
            // an amount of data exactly equal to the amount needed for the prefix,
            // but no more. And also needed with the situation where we have received
            // less than the amount of data needed for prefix. 
            if (bytesToProcess == 0) {
                token.ReceiveMessageOffset -= token.recPrefixBytesDoneThisOp;
                token.recPrefixBytesDoneThisOp = 0;
            }
            return bytesToProcess;
        }
    }

    public class MessageHandler {

        public bool Handle(SocketAsyncEventArgs op, SendRecToken token, Int32 bytesToProcess) {
            bool complete = false;
            if (token.ReceivedMessageBytes == 0) {
                token.Data.DataReceived = new Byte[token.IncomingMessageLength];
            }
            
            if (bytesToProcess + token.ReceivedMessageBytes == token.IncomingMessageLength) {
                // write/append bytes received
                Buffer.BlockCopy(op.Buffer, token.ReceiveMessageOffset,
                    token.Data.DataReceived, token.ReceivedMessageBytes, bytesToProcess);
                complete = true;
            } else {
                // post another receive operation
                Buffer.BlockCopy(op.Buffer, token.ReceiveMessageOffset,
                    token.Data.DataReceived, token.receivedMessageBytesDoneCount, bytesToProcess);
                token.ReceiveMessageOffset = token.ReceiveMessageOffset - token.recPrefixBytesDoneThisOp;
                token.receivedMessageBytesDoneCount += bytesToProcess;
            }

            return complete;
        }
    }
}
