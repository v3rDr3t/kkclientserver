using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace KKClientServer.Networking {

    internal class ReceiveMessageHandler {

        /// <summary>
        /// Handles prefix processing for incoming messages.
        /// </summary>
        /// <param name="receiveEA">The receive event agrs.</param>
        /// <param name="token">The user token of the event agrs.</param>
        /// <param name="bytesToProcess">The amount of bytes that need to be processed.</param>
        public int HandlePrefix(SocketAsyncEventArgs receiveEA, TransferData token, int bytesToProcess) {
            // create the prefix byte array
            if (token.PrefixBytesReceived == 0) {
                token.Prefix = new byte[Constants.PREFIX_SIZE];
            }

            // prefix has been received completely
            if (bytesToProcess >= Constants.PREFIX_SIZE - token.PrefixBytesReceived) {
                Console.WriteLine("Receive> Prefix has been received completely.");
                // copy data bytes
                Buffer.BlockCopy(
                    receiveEA.Buffer,
                    token.TextOffset - Constants.PREFIX_SIZE + token.PrefixBytesReceived,
                    token.Prefix,
                    token.PrefixBytesReceived,
                    Constants.PREFIX_SIZE - token.PrefixBytesReceived);

                byte[] array = new byte[Constants.PREFIX_SIZE - token.PrefixBytesReceived];
                Buffer.BlockCopy(receiveEA.Buffer, token.TextOffset - Constants.PREFIX_SIZE + token.PrefixBytesReceived,
                    array, 0,
                    Constants.PREFIX_SIZE - token.PrefixBytesReceived);
                Console.WriteLine("Receive> Prefix bytes: " + BitConverter.ToString(array));

                // process prefix
                token.Type = (MessageType)token.Prefix[0];
                token.TextLength = BitConverter.ToInt32(getSubArray(token.Prefix,
                    Constants.TEXT_LENGTH_PREFIX_OFFSET,
                    Constants.LENGTH_PREFIX_SIZE), 0);
                token.FileLength = BitConverter.ToInt64(getSubArray(token.Prefix,
                    Constants.FILE_LENGTH_PREFIX_OFFSET,
                    Constants.LENGTH_PREFIX_SIZE * 2), 0);

                bytesToProcess = bytesToProcess - Constants.PREFIX_SIZE + token.PrefixBytesReceived;
                token.PrefixBytesProcessed = Constants.PREFIX_SIZE - token.PrefixBytesReceived;
                token.PrefixBytesReceived = Constants.PREFIX_SIZE;

                // append file offset
                token.FileOffset += token.TextLength;
                Console.WriteLine("Receive> FileOffset = " + token.FileOffset);
            }
            // prefix has been received incompletely
            else {
                Console.WriteLine("Receive> Prefix has been received incompletely.");
                // copy prefix bytes
                Buffer.BlockCopy(
                    receiveEA.Buffer,
                    token.TextOffset - Constants.PREFIX_SIZE + token.PrefixBytesReceived,
                    token.Prefix,
                    token.PrefixBytesReceived,
                    bytesToProcess);

                byte[] array = new byte[bytesToProcess];
                Buffer.BlockCopy(receiveEA.Buffer, token.TextOffset - Constants.PREFIX_SIZE + token.PrefixBytesReceived,
                    array, 0,
                    bytesToProcess);
                Console.WriteLine("Receive> Prefix bytes: " + BitConverter.ToString(array));

                token.PrefixBytesProcessed = bytesToProcess;
                token.PrefixBytesReceived += bytesToProcess;
                bytesToProcess = 0;
            }

            // set new offset
            if (bytesToProcess == 0) {
                token.TextOffset -= token.PrefixBytesProcessed;
                //token.FileOffset -= token.PrefixBytesProcessed;
                //Console.WriteLine("Receive> FileOffset = " + token.FileOffset);
                token.PrefixBytesProcessed = 0;
            }
            return bytesToProcess;
        }

        /// <summary>
        /// Handles data processing for incoming messages.
        /// </summary>
        /// <param name="receiveEA">The receive event agrs.</param>
        /// <param name="token">The user token of the event agrs.</param>
        /// <param name="bytesToProcess">The amount of bytes that need to be processed.</param>
        public int HandleText(SocketAsyncEventArgs receiveEA, TransferData token, int bytesToProcess) {
            // create the text byte array
            if (token.TextBytesReceived == 0) {
                token.TextData = new byte[token.TextLength];
            }

            // text data has been received completely
            if (bytesToProcess >= token.TextLength - token.TextBytesReceived) {
                Console.WriteLine("Receive> Text has been received completely.");
                // copy text data bytes
                Buffer.BlockCopy(
                    receiveEA.Buffer,
                    token.TextOffset,
                    token.TextData,
                    token.TextBytesReceived,
                    token.TextLength - token.TextBytesReceived);

                byte[] array = new byte[token.TextLength - token.TextBytesReceived];
                Buffer.BlockCopy(receiveEA.Buffer, token.TextOffset, array, 0, token.TextLength - token.TextBytesReceived);
                Console.WriteLine("Receive> Text bytes: " + BitConverter.ToString(array) + " = " + Encoding.Default.GetString(array));

                // set byte counters
                bytesToProcess = bytesToProcess - token.TextLength + token.TextBytesReceived;
                token.TextBytesProcessed = token.TextLength - token.TextBytesReceived;
                token.TextBytesReceived = token.TextLength;
                
                token.FilePath = Encoding.Default.GetString(token.TextData);
            }
            // text data has been received incompletely
            else {
                Console.WriteLine("Receive> Text has been received incompletely.");
                // copy text data bytes
                Buffer.BlockCopy(
                    receiveEA.Buffer,
                    token.TextOffset,
                    token.TextData,
                    token.TextBytesReceived,
                    bytesToProcess);

                byte[] array = new byte[bytesToProcess];
                Buffer.BlockCopy(receiveEA.Buffer, token.TextOffset, array, 0, bytesToProcess);
                Console.WriteLine("Receive> Text bytes: " + BitConverter.ToString(array) + " = " + Encoding.Default.GetString(array));

                token.TextBytesProcessed = bytesToProcess;
                token.TextBytesReceived += bytesToProcess;
                bytesToProcess = 0;
            }

            // set new offset
            if (bytesToProcess == 0) {
                token.TextOffset = token.ReceiveBufferOffset;
                token.FileOffset -= token.TextBytesProcessed;
                Console.WriteLine("Receive> FileOffset = " + token.FileOffset);
                token.TextBytesProcessed = 0;
            }
            return bytesToProcess;
        }

        /// <summary>
        /// Helper function to handle file processing.
        /// </summary>
        /// <param name="receiveEA">The receive event agrs.</param>
        /// <param name="token">The user token of the event agrs.</param>
        /// <param name="bytesToProcess">The amount of bytes that need to be processed.</param>
        internal bool HandleFile(SocketAsyncEventArgs receiveEA, TransferData token, int bytesToProcess) {
            // create writer
            if (token.FileBytesReceived == 0) {
                // append or create
                if (!File.Exists(token.FilePath)) {
                    Console.WriteLine("Receive> Create Writer (create).");
                    token.Writer = new BinaryWriter(File.Open(token.FilePath, FileMode.Create));
                } else {
                    if (new FileInfo(token.FilePath).Length != token.FileLength) {
                        Console.WriteLine("Receive> Create Writer (append).");
                        token.Writer = new BinaryWriter(File.Open(token.FilePath, FileMode.Append));
                    }
                    // exact same file already exists (!!!)
                    else {
                        Console.WriteLine("Receive> Same file.");
                        return true;
                    }
                }
            }

            // file has been received completely.
            Console.WriteLine("Receive> FileOffset = " + token.FileOffset);
            if (bytesToProcess + token.FileBytesReceived == token.FileLength) {
                Console.WriteLine("Receive> File complete: Writing " + bytesToProcess + " bytes.");
                token.Writer.Write(receiveEA.Buffer, token.FileOffset, bytesToProcess);

                byte[] array = new byte[bytesToProcess];
                Buffer.BlockCopy(receiveEA.Buffer, token.FileOffset, array, 0, bytesToProcess);
                Console.WriteLine("Receive> File bytes: " + BitConverter.ToString(array) + " = " + Encoding.Default.GetString(array));

                return true;
            }
            // file has been received incompletely
            else {
                Console.WriteLine("Receive> File incomplete: Writing " + bytesToProcess + " bytes.");
                token.Writer.Write(receiveEA.Buffer, token.FileOffset, bytesToProcess);

                byte[] array = new byte[bytesToProcess];
                Buffer.BlockCopy(receiveEA.Buffer, token.FileOffset, array, 0, bytesToProcess);
                Console.WriteLine("Receive> File bytes: " + BitConverter.ToString(array) + " = " + Encoding.Default.GetString(array));

                token.FileBytesReceived += bytesToProcess;
                return false;
            }
        }

        /// <summary>
        /// Helper function to get a sub (byte) array.
        /// </summary>
        /// <param name="array">The source array.</param>
        /// <param name="offset">The source offset.</param>
        /// <param name="count">The amount of bytes for the sub array.</param>
        private byte[] getSubArray(byte[] array, int offset, int count) {
            byte[] subArray = new byte[count];
            Buffer.BlockCopy(
                array,
                offset,
                subArray,
                0,
                count);
            return subArray;
        }
    }
}
