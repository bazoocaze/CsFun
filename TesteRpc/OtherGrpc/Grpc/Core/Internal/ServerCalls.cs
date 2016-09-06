using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core.Internal
{
    internal static class ServerCalls
    {
        public static IServerCallHandler UnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, UnaryServerMethod<TRequest, TResponse> handler)
           where TRequest : class
           where TResponse : class
        {
            return new UnaryServerCallHandler<TRequest, TResponse>(method, handler);
        }
    }
}
