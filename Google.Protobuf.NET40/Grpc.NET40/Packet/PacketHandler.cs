using Grpc.Core;
using Grpc.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.Packet
{
    public static class PacketHandler
    {
        public static TReceived GetData<TReceived>(InternalPacket packet, Marshaller<TReceived> marshaller)
        {
            return marshaller.Deserializer(packet.Payload);
        }

        public static RpcException GetException(InternalPacket packet)
        {
            return packet.Exception;
        }
    }
}
