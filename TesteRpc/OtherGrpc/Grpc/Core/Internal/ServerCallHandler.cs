using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core.Internal
{
    internal interface IServerCallHandler
    {
    }

    internal class UnaryServerCallHandler<TRequest, TResponse> : IServerCallHandler
       where TRequest : class
       where TResponse : class
    {
        public UnaryServerCallHandler(Method<TRequest, TResponse> method, UnaryServerMethod<TRequest, TResponse> handler)
        {
            // this.method = method;
            // this.handler = handler;
        }
    }
}
