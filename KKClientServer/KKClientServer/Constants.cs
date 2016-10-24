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
        public const int DEFAULT_PORT = 51010;

        // maximum number of connections for send/receive
        public const int MAX_NUM_CONNECTIONS = 100;

        // maximum number of connections for send/receive
        public const int MAX_NUM_SEND_REC = 500;

        // size of the data buffer.
        public const int BUFFER_SIZE = 512;

        // maximum number of asynchronous accept operations that can be
        // posted simultaneously
        public const int MAX_ASYNC_ACCEPT_OPS = 10;

        // maximum number of asynchronous connect operations that can be
        // posted simultaneously
        public const int MAX_ASYNC_CONNECT_OPS = 10;

        // for the managed buffer preallocation (1 for receive, 1 for send)
        public const int PREALLOCATION_OPS = 2;

        // size of the queue for incoming connections
        public const int BACKLOG = 100;

        // size of the message prefixes
        public const int MSG_PREFIX_LENGTH = 5;

        // logging
        public const string LOG_DATETIME_FORMAT = "HH:mm:ss (dd-MM-yyyy)";
    }
}
