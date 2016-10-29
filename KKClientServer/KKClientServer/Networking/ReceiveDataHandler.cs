using KKClientServer.Client;
using KKClientServer.Networking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Networking {
    internal class ReceiveDataHandler {
        
        internal string HandleData(SocketAsyncEventArgs saea) {
            TransferData token = (TransferData)saea.UserToken;
            if (token.Type == MessageType.File)
                return token.FilePath;
            else
                return Encoding.Default.GetString(token.ReceiveData);
        }
    }
}
