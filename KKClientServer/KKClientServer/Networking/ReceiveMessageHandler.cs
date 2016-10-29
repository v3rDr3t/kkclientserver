using KKClientServer.Client;
using KKClientServer.Networking.Client;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace KKClientServer.Networking {

    internal class ReceiveMessageHandler {


        public int HandlePrefix(SocketAsyncEventArgs receiveEA, TransferData token, int bytesToProcess) {
            if (token.PrefixBytesReceived == 0) {
                token.PrefixData = new byte[Constants.PREFIX_SIZE];
            }

            if (bytesToProcess >= Constants.PREFIX_SIZE - token.PrefixBytesReceived) {
                Console.WriteLine("Receive> Prefix has been received completely.");
                // prefix has been received completely
                Buffer.BlockCopy(
                    receiveEA.Buffer,
                    token.MessageOffset - Constants.PREFIX_SIZE + token.PrefixBytesReceived,
                    token.PrefixData,
                    token.PrefixBytesReceived,
                    Constants.PREFIX_SIZE - token.PrefixBytesReceived);

                bytesToProcess = bytesToProcess - Constants.PREFIX_SIZE + token.PrefixBytesReceived;
                token.PrefixBytesProcessed = Constants.PREFIX_SIZE - token.PrefixBytesReceived;
                Console.WriteLine("Receive> .. PrefixBytesProcessed = " + token.PrefixBytesProcessed);
                token.PrefixBytesReceived = Constants.PREFIX_SIZE;
                Console.WriteLine("Receive> .. PrefixBytesReceived = " + token.PrefixBytesReceived);
                token.Type = (MessageType)token.PrefixData[0];
                Console.WriteLine("Receive> .. Type = " + token.Type.ToString());
                token.MessageLength = BitConverter.ToInt32(token.PrefixData, 1);
                Console.WriteLine("Receive> .. MessageLength = " + token.MessageLength);
            } else {
                Console.WriteLine("Receive> Prefix has been received incompletely.");
                // prefix has been received incompletely
                Buffer.BlockCopy(
                    receiveEA.Buffer,
                    token.MessageOffset - Constants.PREFIX_SIZE + token.PrefixBytesReceived,
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
            Console.WriteLine("Receive> Bytes to process: " + bytesToProcess);
            return bytesToProcess;
        }

        public bool HandleMessage(SocketAsyncEventArgs receiveEA, TransferData token, int bytesToProcess) {
            bool complete = false;
            switch (token.Type) {
                case MessageType.File:
                    Console.WriteLine("Receive> Handle MessageType = File");
                    complete = handleFile(receiveEA, token, bytesToProcess);
                    break;
                case MessageType.Text:
                default:
                    Console.WriteLine("Receive> Handle MessageType = Text");
                    complete = handleText(receiveEA, token, bytesToProcess);
                    break;
            }
            return complete;
        }

        private bool handleText(SocketAsyncEventArgs receiveEA, TransferData token, int bytesToProcess) {
            // create the text byte array
            if (token.MessageBytesReceived == 0) {
                token.ReceiveData = new byte[token.MessageLength];
            }
            // message has been received completely.
            if (bytesToProcess + token.MessageBytesReceived == token.MessageLength) {
                Buffer.BlockCopy(
                    receiveEA.Buffer,
                    token.MessageOffset,
                    token.ReceiveData,
                    token.MessageBytesReceived,
                    bytesToProcess);
                return true;
            }
            // message has been received incompletely
            else {
                Buffer.BlockCopy(
                    receiveEA.Buffer,
                    token.MessageOffset,
                    token.ReceiveData,
                    token.MessageBytesReceived,
                    bytesToProcess);
                token.MessageOffset -= token.PrefixBytesProcessed;
                token.MessageBytesReceived += bytesToProcess;
                return false;
            }
        }

        private bool handleFile(SocketAsyncEventArgs receiveEA, TransferData token, int bytesToProcess) {
            // create writer
            if (token.MessageBytesReceived == 0) {
                Console.WriteLine("Receive> Create Writer.");
                // append or create
                if (!File.Exists(token.FilePath)) {
                    token.Writer = new BinaryWriter(File.Open(token.FilePath, FileMode.Create));
                } else {
                    token.Writer = new BinaryWriter(File.Open(token.FilePath, FileMode.Append));
                }
            }
            // file has been received completely.
            if (bytesToProcess + token.MessageBytesReceived == token.MessageLength) {
                Console.WriteLine("Receive> File complete: Write " + bytesToProcess + " bytes.");
                token.Writer.Write(receiveEA.Buffer, token.MessageOffset, bytesToProcess);
                return true;
            }
            // file has been received incompletely
            else {
                Console.WriteLine("Receive> File incomplete: Write " + bytesToProcess + " bytes.");
                token.Writer.Write(receiveEA.Buffer, token.MessageOffset, bytesToProcess);
                token.MessageOffset -= token.PrefixBytesProcessed;
                token.MessageBytesReceived += bytesToProcess;
                return false;
            }
        }
    }
}
