using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Extras;
using Grpc.Packet;

namespace Grpc.Core.Internal
{
    internal class UnaryServerCallHandler<TRequest, TResponse> : IServerCallHandler
        where TRequest : class
        where TResponse : class
    {
        private Method<TRequest, TResponse> m_Method;
        private Method<TResponse, TRequest> m_MethodInv;
        private UnaryServerMethod<TRequest, TResponse> m_Handler;

        public UnaryServerCallHandler(Method<TRequest, TResponse> method, UnaryServerMethod<TRequest, TResponse> handler)
        {
            this.m_Method = method;
            this.m_Handler = handler;
            this.m_MethodInv = MethodUtil.Invert(method);
        }

        public Task HandleCall(IPacketSession session, InternalPacket packet)
        {
            var rpc = new RpcTunnel<TResponse, TRequest>(session, m_MethodInv, packet.MsgId);
            rpc.Start();

            return Task.Factory.StartNew(() =>
            {
                using (rpc)
                {
                    TRequest request = PacketHandler.GetData(packet, m_Method.RequestMarshaller);

                    m_Handler
                        .Invoke(request, new ServerCallContext())
                        .ContinueWith((taskResult) => rpc.Close(taskResult))
                        .Wait();
                }
            });
        }
    }


    internal class ClientStreamingServerCallHandler<TRequest, TResponse> : IServerCallHandler
        where TRequest : class
        where TResponse : class
    {
        private Method<TRequest, TResponse> m_Method;
        private ClientStreamingServerMethod<TRequest, TResponse> m_Handler;
        private Method<TResponse, TRequest> m_MethodInv;

        public ClientStreamingServerCallHandler(Method<TRequest, TResponse> method, ClientStreamingServerMethod<TRequest, TResponse> handler)
        {
            this.m_Method = method;
            this.m_Handler = handler;
            this.m_MethodInv = MethodUtil.Invert(method);
        }

        public Task HandleCall(IPacketSession session, InternalPacket packet)
        {
            var rpc = new RpcTunnel<TResponse, TRequest>(session, m_MethodInv, packet.MsgId);
            rpc.Start();

            return Task.Factory.StartNew(() =>
            {
                using (rpc)
                {
                    m_Handler
                        .Invoke(rpc.GetReceiver(), new ServerCallContext())
                        .ContinueWith((taskResult) => rpc.Close(taskResult))
                        .Wait();
                }
            });
        }
    }


    internal class ServerStreamingServerCallHandler<TRequest, TResponse> : IServerCallHandler
        where TRequest : class
        where TResponse : class
    {
        private ServerStreamingServerMethod<TRequest, TResponse> m_Handler;
        private Method<TRequest, TResponse> m_Method;
        private Method<TResponse, TRequest> m_MethodInv;

        public ServerStreamingServerCallHandler(Method<TRequest, TResponse> method, ServerStreamingServerMethod<TRequest, TResponse> handler)
        {
            this.m_Method = method;
            this.m_Handler = handler;
            this.m_MethodInv = MethodUtil.Invert(method);
        }

        public Task HandleCall(IPacketSession session, InternalPacket packet)
        {
            var rpc = new RpcTunnel<TResponse, TRequest>(session, m_MethodInv, packet.MsgId);
            rpc.Start();

            return Task.Factory.StartNew(() =>
            {
                using (rpc)
                {
                    TRequest request = PacketHandler.GetData(packet, m_Method.RequestMarshaller);

                    m_Handler
                        .Invoke(request, rpc.GetSender(), new ServerCallContext())
                        .ContinueWith((taskResult) => rpc.Close(taskResult))
                        .Wait();
                }
            });
        }
    }


    internal class DuplexStreamingServerCallHandler<TRequest, TResponse> : IServerCallHandler
        where TRequest : class
        where TResponse : class
    {
        private DuplexStreamingServerMethod<TRequest, TResponse> m_Handler;
        private Method<TRequest, TResponse> m_Method;
        private Method<TResponse, TRequest> m_MethodInv;

        public DuplexStreamingServerCallHandler(Method<TRequest, TResponse> method, DuplexStreamingServerMethod<TRequest, TResponse> handler)
        {
            this.m_Method = method;
            this.m_Handler = handler;
            this.m_MethodInv = MethodUtil.Invert(method);
        }

        public Task HandleCall(IPacketSession session, InternalPacket packet)
        {
            var rpc = new RpcTunnel<TResponse, TRequest>(session, m_MethodInv, packet.MsgId);
            rpc.Start();

            return Task.Factory.StartNew(() =>
            {
                using (rpc)
                {
                    m_Handler
                        .Invoke(rpc.GetReceiver(), rpc.GetSender(), new ServerCallContext())
                        .ContinueWith((taskResult) => rpc.Close(taskResult))
                        .Wait();
                }
            });
        }
    }
}
