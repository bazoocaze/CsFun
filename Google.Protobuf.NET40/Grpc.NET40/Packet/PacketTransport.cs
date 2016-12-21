using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Grpc.Core.Internal;
using Grpc.Extras;

namespace Grpc.Packet
{
    /// <summary>
    /// Transporte de pacotes TCP usando requests e responses do tipo InternalPacket.
    /// </summary>
    public class PacketTransport : IPacketTransport
    {
        SimpleTcpTransport<InternalPacket, InternalPacket> m_transport;

        public PacketTransport(TcpClient client)
        {
            m_transport = new SimpleTcpTransport<InternalPacket, InternalPacket>(client);
            m_transport.ResponseSerializer = PacketFactory.Serialize;
            m_transport.RequestDeserializer = PacketFactory.Deserialize;
        }

        public void Close()
        {
            m_transport.Close();
        }

        public void Dispose()
        {
            m_transport.Dispose();
        }

        public void Send(InternalPacket packet)
        {
            m_transport.Send(packet);
        }

        public bool TrySend(InternalPacket packet)
        {
            return m_transport.TrySend(packet);
        }

        public void SetDisconnectAction(Action<IPacketTransport, DisconnectReason> disconnecteAction)
        {
            m_transport.DisconnectedAction = (t, r) => disconnecteAction(this, r);
        }

        public void SetReceiveAction(Action<IPacketTransport, InternalPacket> receiveAction)
        {
            m_transport.PacketReceivedAction = (t, p) => receiveAction(this, p);
        }

        public void Start()
        {
            m_transport.Start();
        }
    }
}
