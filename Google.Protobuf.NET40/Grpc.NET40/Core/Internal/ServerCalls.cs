using Grpc.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public static IServerCallHandler ClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, ClientStreamingServerMethod<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            return new ClientStreamingServerCallHandler<TRequest, TResponse>(method, handler);
        }

        public static IServerCallHandler ServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, ServerStreamingServerMethod<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            return new ServerStreamingServerCallHandler<TRequest, TResponse>(method, handler);
        }

        public static IServerCallHandler DuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, DuplexStreamingServerMethod<TRequest, TResponse> handler)
            where TRequest : class
            where TResponse : class
        {
            return new DuplexStreamingServerCallHandler<TRequest, TResponse>(method, handler);
        }
    }
}
