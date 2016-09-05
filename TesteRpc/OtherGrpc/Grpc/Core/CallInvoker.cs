using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core
{
    public class CallInvoker
    {
        /// <summary>
        /// Invokes a simple remote call in a blocking fashion.
        /// </summary>
        public TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
            where TRequest : class
            where TResponse : class
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Invokes a simple remote call asynchronously.
        /// </summary>
        public AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
            where TRequest : class
            where TResponse : class
        {
            throw new NotImplementedException();
        }

    }
}
