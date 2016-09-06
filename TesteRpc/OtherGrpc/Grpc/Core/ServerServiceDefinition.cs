using Grpc.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teste.Protos;

namespace Grpc.Core
{
    public class ServerServiceDefinition
    {
        readonly Dictionary<string, IServerCallHandler> callHandlers;

        private ServerServiceDefinition(Dictionary<string, IServerCallHandler> callHandlers)
        {
            this.callHandlers = new Dictionary<string, IServerCallHandler>(callHandlers);
        }

        /// <summary>
        /// Creates a new builder object for <c>ServerServiceDefinition</c>.
        /// </summary>
        /// <returns>The builder object.</returns>
        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        public class Builder
        {
            readonly Dictionary<string, IServerCallHandler> callHandlers = new Dictionary<string, IServerCallHandler>();

            public Builder AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, UnaryServerMethod<TRequest, TResponse> handler)
                               where TRequest : class
                               where TResponse : class
            {
                callHandlers.Add(method.FullName, ServerCalls.UnaryCall(method, handler));
                return this;
            }

            public ServerServiceDefinition Build()
            {
                return new ServerServiceDefinition(callHandlers);
            }
        }
    }
}
