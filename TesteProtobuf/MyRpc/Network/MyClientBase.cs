using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teste.Protos;
using MyRpc.Packet;

namespace MyRpc.Network
{
    public class MyClientBase
    {
        public string ServiceId { get; protected set; }

        protected SimpleClient m_Canal;

        protected Dictionary<string, MyMethodDefinition> m_Methods;
        protected Dictionary<int, MyRequestHandler> m_PendingRequests;

        protected Dictionary<string, MyMethodDefinition> Methods
        {
            get { return m_Methods; }
        }

        protected MyClientBase(string serviceId, SimpleClient canal)
        {
            this.ServiceId = serviceId;
            this.m_Canal = canal;
            this.m_Methods = new Dictionary<string, MyMethodDefinition>();
            this.m_PendingRequests = new Dictionary<int, MyRequestHandler>();
        }

        public void Start()
        {
            this.m_Canal.OnPacketReceived = OnPacketReceived;
            this.m_Canal.Start();
        }

        protected void AddPendingRequest(MyRequestHandler handler)
        {
            m_PendingRequests.Add(handler.ClientId, handler);
        }

        private void OnPacketReceived(SimpleClient sender, ByteStream packetStream)
        {
            MyRpcRequest req = new MyRpcRequest();
            req.MergeFrom(packetStream);
            MyRequestHandler pending;
            if (m_PendingRequests.TryGetValue(req.RequestId, out pending))
            {
                pending.OnResponse(req);
                if (pending.Concluido) m_PendingRequests.Remove(req.RequestId);
            }
        }

        protected void AddMethod(MyMethodDefinition method)
        {
            m_Methods.Add(method.MethodId, method);
        }

        protected void Send(string methodId, IMessage message)
        {
            Send(Methods[methodId], message);
        }

        protected void Send(MyMethodDefinition method, IMessage message)
        {
            MyRpcRequest req = new MyRpcRequest();
            req.RequestId = 0;
            req.MethodId = method.MethodId;
            req.ServiceId = this.ServiceId;
            req.PayloadData = message.ToByteString();
            m_Canal.Send(req);
        }
    }
}
