using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core
{
    public class RpcException : Exception
    {
        /// <summary>
        /// Creates a new <c>RpcException</c> associated with given status.
        /// </summary>
        /// <param name="status">Resulting status of a call.</param>
        public RpcException(Status status) : base(status.ToString())
        {
            // this.status = status;
        }
    }
}
