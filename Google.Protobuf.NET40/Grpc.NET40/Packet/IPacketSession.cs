using Grpc.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.Packet
{
    public interface IPacketSession
    {
        int GetNewMsgId();

        void AddRpc(IRpc rpc);
        void RemoveRpc(int msgId);
        void RemoveRpc(IRpc rpc);

        void Send(InternalPacket packet);
        bool TrySend(InternalPacket packet);
    }
}
