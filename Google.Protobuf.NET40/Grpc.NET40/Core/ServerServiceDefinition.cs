using Grpc.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.Core
{
    public class ServerServiceDefinition
    {
        readonly Dictionary<string, IServerCallHandler> callHandlers;

        private ServerServiceDefinition(Dictionary<string, IServerCallHandler> callHandlers)
        {
            this.callHandlers = new Dictionary<string, IServerCallHandler>(callHandlers);
        }

        internal IDictionary<string, IServerCallHandler> CallHandlers
        {
            get
            {
                return this.callHandlers;
            }
        }

        /// <summary>
        /// Creates a new builder object for <c>ServerServiceDefinition</c>.
        /// </summary>
        /// <returns>The builder object.</returns>
        public static Builder CreateBuilder()
        {
            return new Builder();
        }

        /// <summary>
        /// Builder class for <see cref="ServerServiceDefinition"/>.
        /// </summary>
        public class Builder
        {
            readonly Dictionary<string, IServerCallHandler> callHandlers;

            public Builder()
            {
                callHandlers = new Dictionary<string, IServerCallHandler>();
            }

            /// <summary>
            /// Adds a definitions for a single request - single response method.
            /// </summary>
            /// <typeparam name="TRequest">The request message class.</typeparam>
            /// <typeparam name="TResponse">The response message class.</typeparam>
            /// <param name="method">The method.</param>
            /// <param name="handler">The method handler.</param>
            /// <returns>This builder instance.</returns>
            public Builder AddMethod<TRequest, TResponse>(
                Method<TRequest, TResponse> method,
                UnaryServerMethod<TRequest, TResponse> handler)
                    where TRequest : class
                    where TResponse : class
            {
                callHandlers.Add(method.FullName, ServerCalls.UnaryCall(method, handler));
                return this;
            }

            /// <summary>
            /// Adds a definitions for a client streaming method.
            /// </summary>
            /// <typeparam name="TRequest">The request message class.</typeparam>
            /// <typeparam name="TResponse">The response message class.</typeparam>
            /// <param name="method">The method.</param>
            /// <param name="handler">The method handler.</param>
            /// <returns>This builder instance.</returns>
            public Builder AddMethod<TRequest, TResponse>(
                Method<TRequest, TResponse> method,
                ClientStreamingServerMethod<TRequest, TResponse> handler)
                    where TRequest : class
                    where TResponse : class
            {
                callHandlers.Add(method.FullName, ServerCalls.ClientStreamingCall(method, handler));
                return this;
            }

            /// <summary>
            /// Adds a definitions for a server streaming method.
            /// </summary>
            /// <typeparam name="TRequest">The request message class.</typeparam>
            /// <typeparam name="TResponse">The response message class.</typeparam>
            /// <param name="method">The method.</param>
            /// <param name="handler">The method handler.</param>
            /// <returns>This builder instance.</returns>
            public Builder AddMethod<TRequest, TResponse>(
                Method<TRequest, TResponse> method,
                ServerStreamingServerMethod<TRequest, TResponse> handler)
                    where TRequest : class
                    where TResponse : class
            {
                callHandlers.Add(method.FullName, ServerCalls.ServerStreamingCall(method, handler));
                return this;
            }

            /// <summary>
            /// Adds a definitions for a bidirectional streaming method.
            /// </summary>
            /// <typeparam name="TRequest">The request message class.</typeparam>
            /// <typeparam name="TResponse">The response message class.</typeparam>
            /// <param name="method">The method.</param>
            /// <param name="handler">The method handler.</param>
            /// <returns>This builder instance.</returns>
            public Builder AddMethod<TRequest, TResponse>(
                Method<TRequest, TResponse> method,
                DuplexStreamingServerMethod<TRequest, TResponse> handler)
                    where TRequest : class
                    where TResponse : class
            {
                callHandlers.Add(method.FullName, ServerCalls.DuplexStreamingCall(method, handler));
                return this;
            }

            /// <summary>
            /// Creates an immutable <c>ServerServiceDefinition</c> from this builder.
            /// </summary>
            /// <returns>The <c>ServerServiceDefinition</c> object.</returns>
            public ServerServiceDefinition Build()
            {
                return new ServerServiceDefinition(callHandlers);
            }

        }

    }
}
