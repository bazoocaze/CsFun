using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teste.Protos;

namespace MyRpc.Network
{
    public class MyServiceDefinition
    {
        public string ServiceId { get; protected set; }
        private Dictionary<string, MyMethodDefinition> m_Methods;

        public MyServiceDefinition(string serviceId)
        {
            this.ServiceId = serviceId;
            this.m_Methods = new Dictionary<string, MyMethodDefinition>();
        }

        public void AddMethod(MyMethodDefinition method)
        {
            m_Methods.Add(method.MethodId, method);
        }

        public void OnRequestReceived(SimpleClient sender, MyRpcRequest req)
        {
            MyMethodDefinition method;
            if (m_Methods.TryGetValue(req.MethodId, out method))
            {
                IMessage result = method.OnRequest(req.PayloadData);
                if(result != null)
                {
                    MyRpcRequest response = new MyRpcRequest();
                    response.ServiceId = req.ServiceId;
                    response.MethodId = req.MethodId;
                    response.RequestId = req.RequestId;
                    response.PayloadData = result.ToByteString();

                    sender.Send(response);
                }
            }
        }
    }
}
