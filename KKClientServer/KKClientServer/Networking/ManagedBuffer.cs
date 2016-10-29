using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Networking {

    /// <summary>
    /// This class is a large reusable buffer for socket I/O operations.
    /// </summary>
    public class ManagedBuffer {
        #region Fields
        // The total number of bytes
        private int bytesTotal;
        // The global byte array.
        private byte[] buffer;

        private int bytesAllocated;
        private Stack<int> freeIndexPool;
        private int curIndex;
        #endregion

        /// <summary>
        /// Constructs a <code>ManagedBuffer</code> object.
        /// </summary>
        /// <param name="tBytes">The total number of bytes managed.</param>
        /// <param name="aBytes">The total number of bytes allocated for each event args.</param>
        public ManagedBuffer(int tBytes, int aBytes) {
            this.bytesTotal = tBytes;
            this.curIndex = 0;
            this.bytesAllocated = aBytes;
            this.freeIndexPool = new Stack<int>();
        }

        /// <summary>
        /// Initializes the managed buffer.
        /// </summary>
        public void Initialize() {
            this.buffer = new byte[bytesTotal];
        }

        /// <summary>
        /// Split the buffer among all I/O operations.
        /// </summary>
        /// <param name="saea">The event args.</param>
        /// <returns>
        /// <code>True</code> if the buffer was successfully set, <code>false</code> otherwise.
        /// </returns>
        internal bool Set(SocketAsyncEventArgs saea) {
            if (this.freeIndexPool.Count > 0) {
                saea.SetBuffer(this.buffer, this.freeIndexPool.Pop(), this.bytesAllocated);
            } else {
                if ((bytesTotal - this.bytesAllocated) < this.curIndex) {
                    return false;
                }
                saea.SetBuffer(this.buffer, this.curIndex, this.bytesAllocated);
                this.curIndex += this.bytesAllocated;
            }
            return true;
        }

        /// <summary>
        /// Removes the buffer from given event args and frees the buffer.
        /// </summary>
        /// <param name="saea">The event args.</param>
        internal void Free(SocketAsyncEventArgs saea) {
            this.freeIndexPool.Push(saea.Offset);
            saea.SetBuffer(null, 0, 0);
        }
    }
}
