using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Networking.Server {
    internal class AcceptUserToken {
        #region Fields
        private string id;
        #endregion

        public AcceptUserToken(string id) {
            this.id = id;
        }

        #region Properties
        public string Id {
            get { return this.id; }
        }
        #endregion
    }
}
