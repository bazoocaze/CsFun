using Grpc.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core.Internal
{
    /// <summary>
    /// Ponte entre a interface de chamada CallInvoker e o canal de comunicação Channel.
    /// A implementação tunela as chamadas CallInvoker dentro do Channel via túnel RPC.
    /// </summary>
    internal class DefaultCallInvoker : CallInvoker
    {
        /// <summary>
        /// Canal de comunicação.
        /// </summary>
        protected Channel m_channel;

        /// <summary>
        /// Construtor padrão para comunicação via Channel informado.
        /// </summary>
        /// <param name="channel">Canal de comunicação.</param>
        public DefaultCallInvoker(Channel channel)
        {
            m_channel = channel;
        }

        public override AsyncClientStreamingCall<TRequest, TResponse> AsyncClientStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            var session = m_channel.GetSession();
            var rpc = new RpcTunnel<TRequest, TResponse>(session, method);
            var ret = new AsyncClientStreamingCall<TRequest, TResponse>(rpc, session, method);

            try
            {
                // Send request
                rpc.StartRequest();

                // Wait response
                return ret;
            }
            catch (Exception)
            {
                // Failed to request: unregister rpc
                ret.Dispose();
                throw;
            }
        }

        public override AsyncDuplexStreamingCall<TRequest, TResponse> AsyncDuplexStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options)
        {
            var session = m_channel.GetSession();
            var rpc = new RpcTunnel<TRequest, TResponse>(session, method);
            var ret = new AsyncDuplexStreamingCall<TRequest, TResponse>(rpc);

            try
            {
                // Send request
                rpc.StartRequest();

                // Wait response
                return ret;
            }
            catch (Exception)
            {
                // Failed to request: unregister rpc
                ret.Dispose();
                throw;
            }
        }

        public override AsyncServerStreamingCall<TResponse> AsyncServerStreamingCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var session = m_channel.GetSession();
            var rpc = new RpcTunnel<TRequest, TResponse>(session, method);
            var ret = new AsyncServerStreamingCallImpl<TRequest, TResponse>(rpc, session, method);

            try
            {
                // Send request
                rpc.StartRequest(request);

                // Wait response
                return ret;
            }
            catch (Exception)
            {
                // Failed to request: unregister rpc
                ret.Dispose();
                throw;
            }
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var session = m_channel.GetSession();

            var call = Task.Factory.StartNew(() =>
            {
                using (var rpc = new RpcTunnel<TRequest, TResponse>(session, method))
                {
                    rpc.StartRequest(request);
                    return TaskUtil.GetResult(rpc.ReturnValueAsync(), options.CancellationToken);
                }
            });

            return new AsyncUnaryCall<TResponse>(call);
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            var session = m_channel.GetSession();

            using (var rpc = new RpcTunnel<TRequest, TResponse>(session, method))
            {
                rpc.StartRequest(request);
                return TaskUtil.GetResult(rpc.ReturnValueAsync(), options.CancellationToken);
            }
        }

    }
}
