using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ClientServer.Networking {

    public class SocketOperationPool {
        #region Fields (SocketOperationPool)
        // The data structure to hold all event args     
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
        /// Adds event args to the pool. 
        /// </summary>
        /// <param name="saea">The socket operation to add.</param>
        internal void Push(SocketAsyncEventArgs saea) {
            if (saea == null) {
                throw new ArgumentNullException("Socket operation added to the pool must not be null!");
            }
            lock (this.pool) {
                this.pool.Push(saea);
            }
        }

        /// <summary>
        /// Removes event args from the pool.
        /// </summary>
        /// <returns>
        /// The removed event args.
        /// </returns>   
        internal SocketAsyncEventArgs Pop() {
            lock (this.pool) {
                if (this.pool.Count > 0) {
                    return this.pool.Pop();
                } else
                    return null;
            }
        }

        /// <summary>
        /// Removes event args from the pool.
        /// </summary>
        /// <returns>
        /// The removed event args.
        /// </returns>   
        internal void Clear() {
            int i = 0;
            lock (this.pool) {
                while (this.pool.Count > 0) {
                    SocketAsyncEventArgs seae = this.pool.Pop();
                    if (seae.AcceptSocket != null) {
                        Console.WriteLine(++i + ": accept socket");
                        seae.AcceptSocket.Close();
                        seae.AcceptSocket = null;
                    }
                    if (seae.ConnectSocket != null) {
                        Console.WriteLine(++i + ": connect socket");
                        seae.ConnectSocket.Close();
                    }
                }
            }
        }
    }
}
