using KKClientServer.Networking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Networking {

    internal class SendMessageHandler {

        internal int HandlePrefix(SocketAsyncEventArgs sendEA, TransferData token) {
            // prefix was already sent
            if (token.PrefixBytesToSend == 0) {
                Console.WriteLine("Send> No prefix bytes to send.");
                return (token.RemainingBytesToSend < Constants.BUFFER_SIZE)
                    ? token.RemainingBytesToSend
                    : Constants.BUFFER_SIZE;
            }
            // prefix fits into buffer
            if (token.PrefixBytesToSend < Constants.BUFFER_SIZE) {
                Console.WriteLine("Send> Whole or remaining prefix fits into buffer.");
                Buffer.BlockCopy(token.SendData, token.BytesSent,
                    sendEA.Buffer, token.SendBufferOffset, token.PrefixBytesToSend);
                return (token.RemainingBytesToSend < Constants.BUFFER_SIZE)
                    ? token.RemainingBytesToSend - token.PrefixBytesToSend
                    : Constants.BUFFER_SIZE - token.PrefixBytesToSend;
            }
            // prefix does not fit into buffer
            else {
                Console.WriteLine("Send> Whole or remaining prefix does not fit into buffer.");
                Buffer.BlockCopy(token.SendData, token.BytesSent,
                    sendEA.Buffer, token.SendBufferOffset, Constants.BUFFER_SIZE);
                return 0;
            }
        }


        internal void HandleMessage(SocketAsyncEventArgs sendEA, TransferData token, int fileDataToProcess) {
            if (fileDataToProcess > 0) {
                try {
                    Console.WriteLine("Send> Reading " + fileDataToProcess + " bytes of file data...");
                    // read part of file into buffer
                    int bytesToRead = fileDataToProcess;
                    int offset = (token.PrefixBytesToSend > 0)
                        ? token.SendBufferOffset + token.PrefixBytesToSend
                        : token.SendBufferOffset;
                    while (fileDataToProcess > 0) {
                        int bytesRead = token.FileStream.Read(sendEA.Buffer, offset, bytesToRead);
                        Console.WriteLine("Send> Read " + bytesRead + " bytes. Offset = " + offset);
                        Console.WriteLine("Send> Position = " + token.FileStream.Position);
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
