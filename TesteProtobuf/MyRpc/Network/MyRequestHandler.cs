using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Teste.Protos;

namespace MyRpc.Network
{
    public class MyRequestHandler
    {
        private static object m_GetNextIdLock = new object();
        private static int m_NextId;

        public static int GetNextId()
        {
            lock (m_GetNextIdLock) return ++m_NextId;
        }

        public bool Concluido;
        public int ClientId;
        public int PacketId;

        public MyRequestHandler()
        {
            PacketId = GetNextId();
        }

        public virtual void OnResponse(MyRpcRequest request)
        {
            throw new NotImplementedException();
        }
    }


    public class MyRequestHandler<RI, RO> : MyRequestHandler where RI : IMessage where RO : IMessage, new()
    {
        protected RO m_ReturnValue;

        public MyRequestHandler()
        {
        }

        public override void OnResponse(MyRpcRequest req)
        {
            var ret = new RO();
            ret.MergeFrom(req.PayloadData);
            m_ReturnValue = ret;
            this.Concluido = true;
        }

        public RO Send(SimpleClient canal, string serviceId, string methodId, RI input)
        {
            MyRpcRequest req = new MyRpcRequest();
            req.ServiceId = serviceId;
            req.MethodId = methodId;
            req.RequestId = this.ClientId;

            canal.Send(req);

            int timeout = 60;
            while (!Concluido)
            {
                Thread.Sleep(1000);
                if (timeout-- < 1) throw new TimeoutException();
            }

            return m_ReturnValue;
        }
    }
}
