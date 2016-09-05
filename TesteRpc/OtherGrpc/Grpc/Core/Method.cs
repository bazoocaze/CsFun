using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core
{
    /// <summary>
    /// Method types supported by gRPC.
    /// </summary>
    public enum MethodType
    {
        /// <summary>Single request sent from client, single response received from server.</summary>
        Unary,

        /// <summary>Stream of request sent from client, single response received from server.</summary>
        ClientStreaming,

        /// <summary>Single request sent from client, stream of responses received from server.</summary>
        ServerStreaming,

        /// <summary>Both server and client can stream arbitrary number of requests and responses simultaneously.</summary>
        DuplexStreaming
    }

    public class Method<TRequest, TResponse>
    {

        /// <summary>
        /// Initializes a new instance of the <c>Method</c> class.
        /// </summary>
        /// <param name="type">Type of method.</param>
        /// <param name="serviceName">Name of service this method belongs to.</param>
        /// <param name="name">Unqualified name of the method.</param>
        /// <param name="requestMarshaller">Marshaller used for request messages.</param>
        /// <param name="responseMarshaller">Marshaller used for response messages.</param>
        public Method(MethodType type, string serviceName, string name, Marshaller<TRequest> requestMarshaller, Marshaller<TResponse> responseMarshaller)
        {
            // this.type = type;
            // this.serviceName = GrpcPreconditions.CheckNotNull(serviceName, "serviceName");
            // this.name = GrpcPreconditions.CheckNotNull(name, "name");
            // this.requestMarshaller = GrpcPreconditions.CheckNotNull(requestMarshaller, "requestMarshaller");
            // this.responseMarshaller = GrpcPreconditions.CheckNotNull(responseMarshaller, "responseMarshaller");
            // this.fullName = GetFullName(serviceName, name);
        }
    }
}
