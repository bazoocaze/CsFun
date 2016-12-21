using Grpc.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    /// <summary>
    /// A non-generic representation of a remote method.
    /// </summary>
    public interface IMethod
    {
        /// <summary>
        /// Gets the type of the method.
        /// </summary>
        MethodType Type { get; }

        /// <summary>
        /// Gets the name of the service to which this method belongs.
        /// </summary>
        string ServiceName { get; }

        /// <summary>
        /// Gets the unqualified name of the method.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the fully qualified name of the method. On the server side, methods are dispatched
        /// based on this name.
        /// </summary>
        string FullName { get; }
    }

    /// <summary>
    /// A description of a remote method.
    /// </summary>
    /// <typeparam name="TRequest">Request message type for this method.</typeparam>
    /// <typeparam name="TResponse">Response message type for this method.</typeparam>
    public class Method<TRequest, TResponse> : IMethod
    {
        readonly MethodType m_type;
        readonly string m_serviceName;
        readonly string m_name;
        readonly Marshaller<TRequest> m_requestMarshaller;
        readonly Marshaller<TResponse> m_responseMarshaller;
        readonly string m_fullName;

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
            this.m_type = type;
            this.m_serviceName = GrpcPreconditions.CheckNotNull(serviceName, nameof(serviceName));
            this.m_name = GrpcPreconditions.CheckNotNull(name, nameof(name));
            this.m_requestMarshaller = GrpcPreconditions.CheckNotNull(requestMarshaller, nameof(requestMarshaller));
            this.m_responseMarshaller = GrpcPreconditions.CheckNotNull(responseMarshaller, nameof(responseMarshaller));
            this.m_fullName = GetFullName(serviceName, name);
        }

        /// <summary>
        /// Gets the type of the method.
        /// </summary>
        public MethodType Type
        { get { return this.m_type; } }

        /// <summary>
        /// Gets the name of the service to which this method belongs.
        /// </summary>
        public string ServiceName
        { get { return this.m_serviceName; } }

        /// <summary>
        /// Gets the unqualified name of the method.
        /// </summary>
        public string Name
        { get { return this.m_name; } }

        /// <summary>
        /// Gets the marshaller used for request messages.
        /// </summary>
        public Marshaller<TRequest> RequestMarshaller
        { get { return this.m_requestMarshaller; } }

        /// <summary>
        /// Gets the marshaller used for response messages.
        /// </summary>
        public Marshaller<TResponse> ResponseMarshaller
        { get { return this.m_responseMarshaller; } }

        /// <summary>
        /// Gets the fully qualified name of the method. On the server side, methods are dispatched
        /// based on this name.
        /// </summary>
        public string FullName
        { get { return this.m_fullName; } }

        /// <summary>
        /// Gets full name of the method including the service name.
        /// </summary>
        internal static string GetFullName(string serviceName, string methodName)
        { return "/" + serviceName + "/" + methodName; }
    }
}
