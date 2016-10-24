using KKClientServer.Networking.Client;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace KKClientServer.Client {

    internal enum MessageType : byte {
        Text = 1,
        File = 2,
    }

    internal class TcpMessageBuilder {

        /// <summary>
        /// Builds a tcp message from text and stores the resulting byte array in the user token of the given event args.
        /// </summary>
        /// <param name="text">The text message.</param>
        /// <param name="saea">The event args.</param>
        internal void BuildTextData(string text, SocketAsyncEventArgs saea) {
            DataUserToken token = (DataUserToken)saea.UserToken;

            int textLength = text.Length;
            // convert text message to byte array
            byte[] textAsBytes = Encoding.Default.GetBytes(text);
            // convert message length to byte array
            byte[] lengthAsBytes = BitConverter.GetBytes(textLength);
            // serialize
            token.SendData = new byte[Constants.MSG_PREFIX_LENGTH + textLength];
            Buffer.SetByte(token.SendData, 0, (byte)MessageType.Text);
            Buffer.BlockCopy(lengthAsBytes, 0, token.SendData, 1, Constants.MSG_PREFIX_LENGTH - 1);
            Buffer.BlockCopy(textAsBytes, 0, token.SendData, Constants.MSG_PREFIX_LENGTH, textLength);

            // set byte counters
            token.RemainingBytesToSend = Constants.MSG_PREFIX_LENGTH + textLength;
            token.BytesSent = 0;
        }

        /// <summary>
        /// Builds a tcp message from file data.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        /// <param name="saea">The event args.</param>
        internal void BuildFileInfoData(FileInfo fileInfo, SocketAsyncEventArgs saea) {
            DataUserToken token = (DataUserToken)saea.UserToken;

            // get file information
            int fileLength = (int)fileInfo.Length;
            // convert file length to byte array
            byte[] lengthAsBytes = BitConverter.GetBytes(fileLength);
            // serialize prefix
            token.SendData = new byte[Constants.MSG_PREFIX_LENGTH];
            Buffer.SetByte(token.SendData, 0, (byte)MessageType.File);
            Buffer.BlockCopy(lengthAsBytes, 0, token.SendData, 1, Constants.MSG_PREFIX_LENGTH - 1);

            // set file stream
            token.FileStream = new FileStream(fileInfo.Name, FileMode.Open);

            // set byte counters for prefix
            token.PrefixBytesToSend = Constants.MSG_PREFIX_LENGTH;
            token.RemainingBytesToSend = Constants.MSG_PREFIX_LENGTH + fileLength;
            token.BytesSent = 0;
        }
    }
}