using KKClientServer.Networking;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace KKClientServer {

    internal enum MessageType : byte {
        Text = 131,
        File = 135,
    }

    internal class TcpMessageBuilder {

        /// <summary>
        /// Builds a tcp message from text and stores the resulting byte array in the user token of the given event args.
        /// </summary>
        /// <param name="text">The text message.</param>
        /// <param name="saea">The event args.</param>
        internal void BuildTextData(string text, SocketAsyncEventArgs saea) {
            TransferData token = (TransferData)saea.UserToken;

            // set type
            token.Type = MessageType.Text;
            int textLength = text.Length;
            // convert text message to byte array
            byte[] textAsBytes = Encoding.Default.GetBytes(text);
            // convert message length to byte array
            byte[] lengthAsBytes = BitConverter.GetBytes(textLength);

            // serialize
            token.TextData = new byte[Constants.PREFIX_SIZE + textLength];
            // (type)
            Buffer.SetByte(token.TextData, 0, (byte)token.Type);
            // (text length)
            Buffer.BlockCopy(
                lengthAsBytes, 0,
                token.TextData, Constants.TEXT_LENGTH_PREFIX_OFFSET,
                Constants.LENGTH_PREFIX_SIZE);
            // (text)
            Buffer.BlockCopy(
                textAsBytes, 0,
                token.TextData, Constants.PREFIX_SIZE,
                textLength);

            // set byte counters
            token.RemainingBytesToSend = Convert.ToInt64(Constants.PREFIX_SIZE + textLength);
            token.BytesSent = 0;
        }

        /// <summary>
        /// Builds a tcp message from file data.
        /// </summary>
        /// <param name="fileInfo">The file information.</param>
        /// <param name="saea">The event args.</param>
        internal void BuildFileInfoData(FileInfo fileInfo, SocketAsyncEventArgs saea) {
            TransferData token = (TransferData)saea.UserToken;

            // set type
            token.Type = MessageType.File;
            // get file information
            int fileNameLength = fileInfo.Name.Length;
            long fileLength = fileInfo.Length;
            // convert to byte array
            byte[] fileNameLengthAsBytes = BitConverter.GetBytes(fileNameLength);
            byte[] fileLengthAsBytes = BitConverter.GetBytes(fileLength);
            byte[] fileNameAsBytes = Encoding.Default.GetBytes(fileInfo.Name);

            // serialize
            token.TextData = new byte[Constants.PREFIX_SIZE + fileNameLength];
            // (type)
            Buffer.SetByte(token.TextData, 0, (byte)token.Type);
            // (file name length)
            Buffer.BlockCopy(
                fileNameLengthAsBytes, 0,
                token.TextData, Constants.TEXT_LENGTH_PREFIX_OFFSET,
                Constants.LENGTH_PREFIX_SIZE);
            // (file length)
            Buffer.BlockCopy(
                fileLengthAsBytes, 0,
                token.TextData, Constants.FILE_LENGTH_PREFIX_OFFSET,
                Constants.LENGTH_PREFIX_SIZE * 2);
            // (file name)
            Buffer.BlockCopy(
                fileNameAsBytes, 0,
                token.TextData, Constants.PREFIX_SIZE,
                fileNameLength);

            // set file stream
            token.FileStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);

            // set byte counters
            token.PrefixAndFileNameBytesToSend = Constants.PREFIX_SIZE + fileNameLength;
            token.RemainingBytesToSend = Convert.ToInt64(Constants.PREFIX_SIZE + fileNameLength)
                + fileLength;
            token.BytesSent = 0;
        }
    }
}