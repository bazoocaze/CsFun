using Grpc.Core.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.Packet
{
    public interface IPacketTransport : IDisposable
    {
        void SetReceiveAction(Action<IPacketTransport, InternalPacket> receiveAction);
        void SetDisconnectAction(Action<IPacketTransport, DisconnectReason> disconnecteAction);

        void Start();
        void Close();

        void Send(InternalPacket packet);
        bool TrySend(InternalPacket packet);
    }
}
