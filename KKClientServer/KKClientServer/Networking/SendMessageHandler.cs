using System;
using System.Net.Sockets;

namespace KKClientServer.Networking {

    internal class SendMessageHandler {

        /// <summary>
        /// Handles prefix and file name processing for outgoing messages.
        /// </summary>
        /// <param name="sendEA">The send event args.</param>
        /// <param name="token">The user token of the event args.</param>
        /// <returns>The remainder of file bytes to send.</returns>
        internal long HandlePrefixAndFileName(SocketAsyncEventArgs sendEA, TransferData token) {
            int bytesSent = NetworkController.checkedConversion(token.BytesSent, "HandlePrefix: ulong to int");

            // prefix and file name were already sent
            if (token.PrefixAndFileNameBytesToSend == 0) {
                //Console.WriteLine("Send> No more prefix or file name bytes to send: "
                //    + token.PrefixAndFileNameBytesToSend + " == 0");
                return (token.RemainingBytesToSend < Constants.BUFFER_SIZE)
                    ? token.RemainingBytesToSend
                    : Constants.BUFFER_SIZE;
            }

            // prefix and file name fit into buffer
            if (token.PrefixAndFileNameBytesToSend < Constants.BUFFER_SIZE) {
                //Console.WriteLine("Send> Remaining prefix and/or file name fit into buffer: " 
                //    + token.PrefixAndFileNameBytesToSend + " < " + Constants.BUFFER_SIZE);
                Buffer.BlockCopy(token.TextData,
                    bytesSent,
                    sendEA.Buffer,
                    token.SendBufferOffset,
                    token.PrefixAndFileNameBytesToSend);

                return (token.RemainingBytesToSend < Constants.BUFFER_SIZE)
                    ? token.RemainingBytesToSend - token.PrefixAndFileNameBytesToSend
                    : Constants.BUFFER_SIZE - token.PrefixAndFileNameBytesToSend;
            }
            // prefix and file name do not fit into buffer
            else {
                //Console.WriteLine("Send> Remaining prefix and/or file name do not fit into buffer: "
                //    + token.PrefixAndFileNameBytesToSend + " >= " + Constants.BUFFER_SIZE);
                Buffer.BlockCopy(token.TextData,
                    bytesSent,
                    sendEA.Buffer,
                    token.SendBufferOffset,
                    Constants.BUFFER_SIZE);
                return 0;
            }
        }

        /// <summary>
        /// Handles file processing for outgoing messages.
        /// </summary>
        /// <param name="sendEA">The send event args.</param>
        /// <param name="token">The user token of the event args.</param>
        internal void HandleFile(SocketAsyncEventArgs sendEA, TransferData token, long fileDataToProcess) {
            if (fileDataToProcess > 0) {
                try {
                    //Console.WriteLine("Send> Reading " + fileDataToProcess + " bytes of file data...");
                    // read part of file into buffer
                    int bytesToRead = NetworkController.checkedConversion(fileDataToProcess, "HandleFile: long to int");
                    int offset = (token.PrefixAndFileNameBytesToSend > 0)
                        ? token.SendBufferOffset + token.PrefixAndFileNameBytesToSend
                        : token.SendBufferOffset;
                    while (bytesToRead > 0) {
                        int bytesRead = token.FileStream.Read(sendEA.Buffer, offset, bytesToRead);

                        //Console.WriteLine("Send> Read " + bytesRead + " bytes. Offset = " + token.FileStream.Position);
                        //byte[] array = new byte[bytesToRead];
                        //Buffer.BlockCopy(sendEA.Buffer, offset,
                        //    array, 0,
                        //    bytesToRead);
                        //Console.WriteLine("Send> File bytes: " + BitConverter.ToString(array) + " = " + System.Text.Encoding.Default.GetString(array) + "\n");

                        if (bytesRead == 0)
                            break;
                        offset += bytesRead;
                        bytesToRead -= bytesRead;
                    }
                } catch (Exception e) {
                    // ...
                    Console.WriteLine("Error> Reading from file stream: " + e.Message);
                }
            }
        }
    }
}
