using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core
{
    public struct Status
    {
        /// <summary>
        /// Creates a new instance of <c>Status</c>.
        /// </summary>
        /// <param name="statusCode">Status code.</param>
        /// <param name="detail">Detail.</param>
        public Status(StatusCode statusCode, string detail)
        {
            // this.statusCode = statusCode;
            // this.detail = detail;
        }
    }
}
