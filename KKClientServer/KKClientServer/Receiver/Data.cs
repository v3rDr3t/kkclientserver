using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Receiver {

    public class Data {
        #region Fields (Data)
        private Int32 receivedTransMissionId;
        private Int32 sessionId;
        private Byte[] dataReceived;
        #endregion

        #region Properties (Data)
        public Int32 ReceivedTransMissionId {
            get {
                return receivedTransMissionId;
            }
            set {
                this.receivedTransMissionId = value;
            }
        }

        public Int32 SessionId {
            get {
                return sessionId;
            }
            set {
                this.sessionId = value;
            }
        }

        public Byte[] DataReceived {
            get {
                return dataReceived;
            }
            set {
                this.dataReceived = value;
            }
        }
        #endregion
    }
}
