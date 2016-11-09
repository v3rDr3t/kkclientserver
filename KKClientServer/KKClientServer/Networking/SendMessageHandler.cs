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
        internal int HandlePrefixAndFileName(SocketAsyncEventArgs sendEA, SendToken token) {
            int bytesLeft = NetworkController.checkedConversion(token.RemainingBytesToSend);
            int bytesSent = NetworkController.checkedConversion(token.BytesSent);

            // prefix and file name were already sent
            if (token.PrefixAndFileNameBytesToSend == 0) {
                return (token.RemainingBytesToSend < Constants.BUFFER_SIZE)
                    ? bytesLeft
                    : Constants.BUFFER_SIZE;
            }

            // prefix and file name fit into buffer
            if (token.PrefixAndFileNameBytesToSend < Constants.BUFFER_SIZE) {
                Buffer.BlockCopy(token.TextData,
                    bytesSent,
                    sendEA.Buffer,
                    token.SendBufferOffset,
                    token.PrefixAndFileNameBytesToSend);

                return (token.RemainingBytesToSend < Constants.BUFFER_SIZE)
                    ? bytesLeft - token.PrefixAndFileNameBytesToSend
                    : Constants.BUFFER_SIZE - token.PrefixAndFileNameBytesToSend;
            }
            // prefix and file name do not fit into buffer
            else {
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
        internal bool HandleFile(SocketAsyncEventArgs sendEA, SendToken token, int fileDataToProcess) {
            if (fileDataToProcess > 0) {
                try {
                    Console.WriteLine("Send> Processing " + fileDataToProcess + " bytes of file data...");
                    // read part of file into buffer
                    int bytesToRead = fileDataToProcess;
                    int offset = (token.PrefixAndFileNameBytesToSend > 0)
                        ? token.SendBufferOffset + token.PrefixAndFileNameBytesToSend
                        : token.SendBufferOffset;
                    while (bytesToRead > 0) {
                        int bytesRead = token.FileStream.Read(sendEA.Buffer, offset, bytesToRead);
                        Console.WriteLine("Send> Read " + bytesRead + " bytes. Offset = " + token.FileStream.Position);
                        if (bytesRead == 0)
                            break;
                        offset += bytesRead;
                        bytesToRead -= bytesRead;
                    }
                } catch (Exception) {
                    return false;
                }
            }
            return true;
        }
    }
}
