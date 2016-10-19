using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Receiver {

    /// <summary>
    /// This class is a large reusable buffer for socket I/O operations.
    /// </summary>
    public class ManagedBuffer {
        #region Fields (ManagedBuffer)
        // total number of bytes
        private Int32 bytesTotal;

        // byte array.
        private byte[] buffer;

        private Int32 bytesAllocated;
        private Stack<int> freeIndexPool;
        private Int32 curIndex;
        #endregion

        /// <summary>
        /// Constructs a <code>ManagedBuffer</code> object.
        /// </summary>
        /// <param name="tbytes">The total number of bytes managed.</param>
        /// <param name="abytes">The total number of bytes allocated for each operation.</param>
        public ManagedBuffer(Int32 tbytes, Int32 abytes) {
            this.bytesTotal = tbytes;
            this.curIndex = 0;
            this.bytesAllocated = abytes;
            this.freeIndexPool = new Stack<int>();
        }

        /// <summary>
        /// Initializes the managed buffer.
        /// </summary>
        public void Init() {
            this.buffer = new byte[bytesTotal];
        }

        /// <summary>
        /// Split the buffer among all I/O operations.
        /// </summary>
        /// <param name="so">The socket operation.</param>
        /// <returns>
        /// <code>True</code> if the buffer was successfully set, <code>false</code> otherwise.
        /// </returns>
        internal bool Set(SocketAsyncEventArgs so) {
            if (this.freeIndexPool.Count > 0) {
                so.SetBuffer(this.buffer, this.freeIndexPool.Pop(), this.bytesAllocated);
            } else {
                if ((bytesTotal - this.bytesAllocated) < this.curIndex) {
                    return false;
                }
                so.SetBuffer(this.buffer, this.curIndex, this.bytesAllocated);
                this.curIndex += this.bytesAllocated;
            }
            return true;
        }

        /// <summary>
        /// Removes the buffer from an I/O operation and frees the buffer.
        /// </summary>
        /// <param name="so">The given socket operation.</param>
        internal void Free(SocketAsyncEventArgs so) {
            this.freeIndexPool.Push(so.Offset);
            so.SetBuffer(null, 0, 0);
        }
    }
}
