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
            public Builder AddMethod<TRequest, TResponse>(Method<TRequest, TResponse> method, Func<TRequest, ServerCallContext, Task<TResponse>> func)
            {
                throw new NotImplementedException();
            }

            public ServerServiceDefinition Build()
            {
                throw new NotImplementedException();
            }
        }
    }
}
