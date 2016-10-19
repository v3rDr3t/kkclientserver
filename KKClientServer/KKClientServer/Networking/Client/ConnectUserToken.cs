using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Networking {
    internal class ConnectUserToken {
        #region Fields
        private string id;
        #endregion

        public ConnectUserToken(string id) {
            this.id = id;
        }

        #region Properties
        public string Id {
            get { return this.id; }
        }
        #endregion
    }
}
