using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace KKClientServer.Networking {

    public class SocketOperationPool {
        #region Fields (SocketOperationPool)       
        Stack<SocketAsyncEventArgs> pool;
        #endregion

        /// <summary>
        /// Constructs a <code>SocketOperationPool</code> object.
        /// </summary>
        /// <param name="cap">The maximum number of socket operations.</param>
        internal SocketOperationPool(int cap) {
            this.pool = new Stack<SocketAsyncEventArgs>(cap);
        }
        
        /// <summary>
        /// Returns the current amount of socket operations in the pool.
        /// </summary>
        /// <returns>
        /// The amount.
        /// </returns>       
        internal int Count() {
            return this.pool.Count;
        }

        /// <summary>
        /// Adds a socket operation to the pool. 
        /// </summary>
        /// <param name="so">The socket operation to add.</param>
        internal void Push(SocketAsyncEventArgs so) {
            if (so == null) {
                throw new ArgumentNullException("Socket operation added to the pool must not be null!");
            }
            lock (this.pool) {
                this.pool.Push(so);
            }
        }

        /// <summary>
        /// Removes a socket operation from the pool.
        /// </summary>
        /// <returns>
        /// The removed socket operation.
        /// </returns>   
        internal SocketAsyncEventArgs Pop() {
            lock (this.pool) {
                if (this.pool.Count > 0) {
                    return this.pool.Pop();
                } else
                    return null;
            }
        }
    }
}
