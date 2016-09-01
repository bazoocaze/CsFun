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
    public class MyServer
    {
        protected SimpleServer m_SimpleServer;

        protected Dictionary<string, MyServiceDefinition> m_Services;

        public MyServer()
        {
            m_SimpleServer = new SimpleServer();
            m_SimpleServer.OnPacketReceived = OnPacketReceived;

            m_Services = new Dictionary<string, MyServiceDefinition>();
        }

        public void AddService(MyServiceDefinition service)
        {
            m_Services.Add(service.ServiceId, service);
        }

        public void Start()
        {
            m_SimpleServer.Start(12345);
        }

        public void Stop()
        {
            m_SimpleServer.Stop();
        }

        private void OnPacketReceived(SimpleClient sender, ByteStream packetStream)
        {
            MyRpcRequest req = new MyRpcRequest();
            req.MergeFrom(new CodedInputStream(packetStream));

            MyServiceDefinition service;
            if(m_Services.TryGetValue(req.ServiceId, out service))
            {
                service.OnRequestReceived(sender, req);
            }
        }

    }
}
