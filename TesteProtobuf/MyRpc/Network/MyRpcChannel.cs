using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyRpc.Packet;
using Teste.Protos;
using Google.Protobuf;

namespace MyRpc.Network
{
    public class MyRpcChannel
    {
        protected SimpleClient m_Cliente;

        protected MyRpcChannel(SimpleClient simpleClient)
        {
            m_Cliente = simpleClient;
            m_Cliente.OnPacketReceived = OnPacketReceived;
        }

        public void Start()
        {
            m_Cliente.Start();
        }

        protected virtual void OnPacketReceived(SimpleClient sender, ByteStream message)
        {
            throw new NotImplementedException();
        }

        public virtual void Send(IMessage request)
        {
            m_Cliente.Send(request);
        }
    }

    public class MyRpcChannel<RI, RO> : MyRpcChannel where RI : IMessage, new() where RO : IMessage, new()
    {
        public Action<MyRpcChannel, RI> OnReceived;

        public MyRpcChannel(SimpleClient simpleClient) : base(simpleClient)
        {
        }

        protected virtual new void OnPacketReceived(SimpleClient sender, ByteStream message)
        {
            RI req = new RI();
            req.MergeFrom(message);
            OnReceived.Invoke(this, req);
        }

        public void Send(RI request)
        {
            m_Cliente.Send(request);
        }

        public override void Send(IMessage request)
        {
            Send((RI)request);
        }

    }
}
