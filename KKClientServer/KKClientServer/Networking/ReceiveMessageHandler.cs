using System;
using System.Net.Sockets;

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

                // process prefix
                token.Type = (MessageType)token.Prefix[0];
                //Console.WriteLine("Receive> .. Type = " + token.Type.ToString());
                token.TextLength = BitConverter.ToInt32(getSubArray(token.Prefix,
                    Constants.TEXT_LENGTH_PREFIX_OFFSET,
                    Constants.LENGTH_PREFIX_SIZE), 0);
                //Console.WriteLine("Receive> .. TextLength = " + token.TextLength);
                token.FileLength = BitConverter.ToInt64(getSubArray(token.Prefix,
                    Constants.FILE_LENGTH_PREFIX_OFFSET,
                    Constants.LENGTH_PREFIX_SIZE * 2), 0);
                //Console.WriteLine("Receive> .. FileLength = " + token.FileLength);

                bytesToProcess = bytesToProcess - Constants.PREFIX_SIZE + token.PrefixBytesReceived;
                token.PrefixBytesProcessed = Constants.PREFIX_SIZE - token.PrefixBytesReceived;
                //token.TextBytesReceived = Constants.PREFIX_SIZE - token.PrefixBytesReceived;
                token.PrefixBytesReceived = Constants.PREFIX_SIZE;
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

                token.PrefixBytesProcessed = bytesToProcess;
                token.PrefixBytesReceived += bytesToProcess;
                bytesToProcess = 0;
            }

            // set new offset
            if (bytesToProcess == 0) {
                token.TextOffset -= token.PrefixBytesProcessed;
                token.PrefixBytesProcessed = 0;
            }
            Console.WriteLine("Receive> Bytes to process: " + bytesToProcess);
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

                // process text
                token.Type = (MessageType)token.Prefix[0];
                //Console.WriteLine("Receive> .. Type = " + token.Type.ToString());
                token.TextLength = BitConverter.ToInt32(getSubArray(token.Prefix,
                    Constants.TEXT_LENGTH_PREFIX_OFFSET,
                    Constants.LENGTH_PREFIX_SIZE), 0);
                //Console.WriteLine("Receive> .. TextLength = " + token.TextLength);

                bytesToProcess = bytesToProcess - Constants.PREFIX_SIZE + token.PrefixBytesReceived;
                token.PrefixBytesProcessed = Constants.PREFIX_SIZE - token.PrefixBytesReceived;
                token.TextBytesReceived = Constants.PREFIX_SIZE - token.PrefixBytesReceived;
                token.PrefixBytesReceived = Constants.PREFIX_SIZE;
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

                token.TextBytesProcessed = bytesToProcess;
                token.TextBytesReceived += bytesToProcess;
                bytesToProcess = 0;
            }

            Console.WriteLine("Receive> Bytes to process: " + bytesToProcess);
            return bytesToProcess;
        }

        /// <summary>
        /// Helper function to handle file processing.
        /// </summary>
        /// <param name="receiveEA">The receive event agrs.</param>
        /// <param name="token">The user token of the event agrs.</param>
        /// <param name="bytesToProcess">The amount of bytes that need to be processed.</param>
        //internal bool HandleFile(SocketAsyncEventArgs receiveEA, TransferData token, int bytesToProcess) {
        //    // create the text byte array (for file name)
        //    if (token.TextBytesReceived == 0) {
        //        token.TextData = new byte[token.TextLength];
        //    }
        //    // file name has been received completely.
        //    if (token.TextBytesReceived == token.TextLength) {
        //        Console.WriteLine("Receive> File name has been received completely.");
        //        Console.WriteLine("Receive> .. bytesToProcess = " + bytesToProcess);
        //        Console.WriteLine("Receive> .. DataBytesReceived = " + token.TextBytesReceived);
        //        Console.WriteLine("Receive> .. TextLength = " + token.TextLength);
        //        Buffer.BlockCopy(
        //            receiveEA.Buffer,
        //            token.DataOffset,
        //            token.TextData,
        //            token.TextBytesReceived,
        //            bytesToProcess);
        //        return true;
        //    }
        //    // file name has been received incompletely
        //    else {
        //        Buffer.BlockCopy(
        //            receiveEA.Buffer,
        //            token.DataOffset,
        //            token.TextData,
        //            token.TextBytesReceived,
        //            bytesToProcess);
        //        token.DataOffset -= token.PrefixBytesProcessed;
        //        token.TextBytesReceived += bytesToProcess;
        //        return false;
        //    }


        //    // create writer
        //    if (token.DataBytesReceived == 0) {
        //        Console.WriteLine("Receive> Create Writer.");
        //        // append or create
        //        if (!File.Exists(token.FilePath)) {
        //            token.Writer = new BinaryWriter(File.Open(token.FilePath, FileMode.Create));
        //        } else {
        //            token.Writer = new BinaryWriter(File.Open(token.FilePath, FileMode.Append));
        //        }
        //    }
        //    // file has been received completely.
        //    if (bytesToProcess + token.DataBytesReceived == token.MessageLength) {
        //        Console.WriteLine("Receive> File complete: Write " + bytesToProcess + " bytes.");
        //        token.Writer.Write(receiveEA.Buffer, token.DataOffset, bytesToProcess);
        //        return true;
        //    }
        //    // file has been received incompletely
        //    else {
        //        Console.WriteLine("Receive> File incomplete: Write " + bytesToProcess + " bytes.");
        //        token.Writer.Write(receiveEA.Buffer, token.DataOffset, bytesToProcess);
        //        token.DataOffset -= token.PrefixBytesProcessed;
        //        token.DataBytesReceived += bytesToProcess;
        //        return false;
        //    }
        //}

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
