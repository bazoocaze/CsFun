using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core
{
    public class AsyncUnaryCall<TResponse>
    {
        private Task<TResponse> m_taskResponse;

        public AsyncUnaryCall(Task<TResponse> taskResponse)
        { m_taskResponse = taskResponse; }

        /// <summary>
        /// Asynchronous call result.
        /// </summary>
        public Task<TResponse> ResponseAsync
        { get { return m_taskResponse; } }
    }
}
