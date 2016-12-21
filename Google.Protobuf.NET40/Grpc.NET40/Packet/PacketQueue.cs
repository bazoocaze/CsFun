using Grpc.Core.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Packet
{
    internal class PacketQueue
    {
        private bool m_Eof;
        private bool m_Running;
        private Queue<PacketItem> m_Packets;
        private object m_RunReaderLock = new object();


        public Action<InternalPacket> PacketReceivedAction;
        public Action<DisconnectReason> DisconnectedAction;


        public PacketQueue()
        {
            m_Packets = new Queue<PacketItem>();
        }


        public void Add(InternalPacket packet)
        {
            if (m_Eof) return;

            m_Packets.Enqueue(new PacketItem(packet));
            StartReader();
        }


        public void SetDisconnected(DisconnectReason reason)
        {
            if (m_Eof) return;

            m_Packets.Enqueue(new PacketItem(reason));
            StartReader();
        }


        public void Clear()
        {
            m_Packets.Clear();
        }


        protected void StartReader()
        {
            if (!m_Running) Task.Factory.StartNew(ReaderTask);
        }


        protected void ReaderTask()
        {
            lock (m_RunReaderLock)
            {                
                while (true)
                {
                    m_Running = true;

                    while (m_Packets.Count > 0)
                    {
                        var item = m_Packets.Dequeue();

                        if (m_Eof) continue;

                        if (item.Disconnected)
                        {
                            m_Eof = true;
                            DisconnectedAction?.Invoke(item.Reason);
                        }
                        else
                        {
                            PacketReceivedAction?.Invoke(item.Packet);
                        }
                    }

                    m_Running = false;

                    if (m_Packets.Count == 0) break;                    
                }
            }
        }


        private class PacketItem
        {
            public InternalPacket Packet;
            public DisconnectReason Reason;

            public PacketItem(InternalPacket packet)
            { this.Packet = packet; }

            public PacketItem(DisconnectReason reason)
            { this.Reason = reason; }

            public bool Disconnected { get { return Reason != null; } }
        }
    }
}
