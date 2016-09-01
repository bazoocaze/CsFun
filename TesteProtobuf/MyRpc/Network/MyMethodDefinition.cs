using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

namespace MyRpc.Network
{
    public class MyMethodDefinition
    {
        public string MethodId { get; protected set; }

        public virtual IMessage OnRequest(ByteString payloadData)
        {
            throw new NotImplementedException();
        }
    }

    public class MyMethodDefinition<RI, RO> : MyMethodDefinition
        where RI : IMessage
        where RO : IMessage
    {

        public static MyMethodDefinition<RI, RO> Create(string methodId)
        {
            MyMethodDefinition<RI, RO> ret = new MyMethodDefinition<RI, RO>(methodId);
            return ret;
        }

        public static MyMethodDefinition<RI, RO> Create(string methodId, Func<RI, RO> methodDelegate)
        {
            MyMethodDefinition<RI, RO> ret = new MyMethodDefinition<RI, RO>(methodId, methodDelegate);
            return ret;
        }

        protected Func<RI, RO> m_Delegate;
        protected Type m_TypeRI;
        protected Type m_TypeRO;

        public MyMethodDefinition(string methodId)
        {
            this.MethodId = methodId;
            this.m_TypeRI = typeof(RI);
            this.m_TypeRO = typeof(RO);
        }

        public MyMethodDefinition(string methodId, Func<RI, RO> methodDelegate)
        {
            this.MethodId = methodId;
            this.m_Delegate = methodDelegate;
            this.m_TypeRI = typeof(RI);
            this.m_TypeRO = typeof(RO);
        }

        public override IMessage OnRequest(ByteString payloadData)
        {
            RI request = (RI)Activator.CreateInstance(m_TypeRI);
            request.MergeFrom(payloadData);
            return m_Delegate.Invoke(request);
        }
    }
}
