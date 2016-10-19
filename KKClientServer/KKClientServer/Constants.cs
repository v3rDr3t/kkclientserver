using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer {
    public class Settings {

    }

    public class Constants {
        // default network settings
        public const Int32 DEFAULT_PORT = 51010;

        // maximum number of connections for send/receive
        public const Int32 MAX_NUM_CONNECTIONS = 100;

        // maximum number of connections for send/receive
        public const Int32 MAX_NUM_SEND_REC = 500;

        // size of the data buffer.
        public const Int32 BUFFER_SIZE = 500;

        // maximum number of asynchronous accept operations that can be
        // posted simultaneously
        public const Int32 MAX_ASYNC_ACCEPT_OPS = 10;

        // size of the queue for incoming connections
        public const Int32 CONNECTIONS_QUEUE_SIZE = 100;

        // size of the message prefixes
        public const Int32 MSG_PREFIX_LENGTH = 4;

        // logging
        public const string LOG_DATETIME_FORMAT = "HH:mm:ss (dd-MM-yyyy)";
    }
}
