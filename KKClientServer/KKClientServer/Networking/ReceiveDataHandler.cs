using KKClientServer.Networking.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Networking {
    internal class ReceiveDataHandler {
        
        internal string HandleTextData(SocketAsyncEventArgs saea) {
            DataUserToken token = (DataUserToken)saea.UserToken;
            string text = Encoding.Default.GetString(token.ReceiveData);
            Console.WriteLine("Text: " + text);
            return text;
        }
    }
}
