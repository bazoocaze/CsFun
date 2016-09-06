using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Teste.Protos;

namespace Grpc.Core.Internal
{
    public class ChannelCallInvoker : CallInvoker
    {
        private int m_LastRequestId;
        private object m_LastRequestIdLock = new object();

        protected Channel m_Channel;

        protected Dictionary<int, PendingRequest> m_RequestsPending;

        public ChannelCallInvoker(Channel channel)
        {
            m_Channel = channel;
        }

        public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            throw new NotImplementedException();
        }

        public override TResponse BlockingUnaryCall<TRequest, TResponse>(Method<TRequest, TResponse> method, string host, CallOptions options, TRequest request)
        {
            byte[] msg = method.RequestMarshaller.Serializer(request);
            RpcEnvelope envelope = new RpcEnvelope();
            envelope.ServiceName = method.ServiceName;
            envelope.MethodName = method.Name;
            envelope.Payload = Google.Protobuf.ByteString.CopyFrom(msg);
            envelope.RequestId = GetNewRequestId();

            PendingRequest<TRequest, TResponse> pending = new PendingRequest<TRequest, TResponse>(envelope.RequestId, method);

            AddPendingRequest(pending);

            m_Channel.Send((IMessage)envelope);

            return pending.WaitReponse();
        }

        private void AddPendingRequest(PendingRequest pending)
        {
            m_RequestsPending.Add(pending.RequestId, pending);
        }

        private void PacketReceived(RpcEnvelope envelope)
        {
            PendingRequest pending;
            if(m_RequestsPending.TryGetValue(envelope.RequestId, out pending))
            {
                pending.PacketReceived(envelope);
            }
        }

        private int GetNewRequestId()
        {
            lock (m_LastRequestIdLock) return ++m_LastRequestId;
        }

        protected abstract class PendingRequest
        {
            public int RequestId;

            protected PendingRequest() { }

            public abstract void PacketReceived(RpcEnvelope envelope);
        }

        protected class PendingRequest<TRequest, TResponse> : PendingRequest
        {            
            public Method<TRequest, TResponse> Method;
            public TResponse Result;            
            private bool m_Received;

            public PendingRequest(int requestId, Method<TRequest, TResponse> method)
            {
                this.RequestId = requestId;
                this.Method = method;
            }

            public override void PacketReceived(RpcEnvelope envelope)
            {
                Result = Method.ResponseMarshaller.Deserializer(envelope.Payload.ToArray());
                m_Received = true;
            }

            public TResponse WaitReponse()
            {
                int counter = 60;
                while(counter-- > 0)
                {
                    if (m_Received) return Result;
                    Thread.Sleep(1000);
                }
                throw new TimeoutException();
            }
        }
    }
}
