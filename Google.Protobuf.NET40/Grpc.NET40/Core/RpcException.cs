using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.Core
{
    /// <summary>
    /// Representa uma excessão remota/RPC.
    /// </summary>
    public class RpcException : Exception
    {
        /// <summary>
        /// Status da excessão remota contendo o tipo de erro.
        /// </summary>
        public Status Status { get; protected set; }

        /// <summary>
        /// Creates a new <c>RpcException</c> associated with given status.
        /// </summary>
        /// <param name="status">Resulting status of a call.</param>
        public RpcException(Status status) : this(status, status.ToString())
        {
        }

        /// <summary>
        /// Creates a new <c>RpcException</c> associated with given status.
        /// </summary>
        /// <param name="status">Resulting status of a call.</param>
        /// <param name="message">Resulting status of a call.</param>
        public RpcException(Status status, string message) : base(message)
        {
            this.Status = status;
        }
    }
}
