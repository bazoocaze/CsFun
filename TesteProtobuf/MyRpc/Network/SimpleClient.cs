using Google.Protobuf;
using MyRpc.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MyRpc.Network
{
    public class SimpleClient
    {
        protected NetworkStream m_Stream;
        protected TcpClient m_Client;
        protected ByteBuilder m_ReceiveBuffer;

        public Action<SimpleClient, ByteStream> OnPacketReceived;

        public SimpleClient(TcpClient client)
        {
            m_Client = client;
            m_Stream = client.GetStream();
            m_ReceiveBuffer = new ByteBuilder();
        }

        public static Task<SimpleClient> ConnectAsync(string host, int port)
        {
            return new Task<SimpleClient>(() => Connect(host, port));
        }

        public static SimpleClient Connect(string host, int port)
        {
            TcpClient cliente = new TcpClient();
            cliente.Connect(Dns.GetHostAddresses(host), port);
            return new SimpleClient(cliente);
        }

        protected void RegisterWaitData()
        {
            var stream = m_Stream;
            if (stream != null)
            {
                var ptr = m_ReceiveBuffer.LockWrite(1024);
                m_Stream?.BeginRead(ptr.Ptr, ptr.Offset, ptr.Size, DataReceivedCallback, m_Stream);
            }
        }

        private void DataReceivedCallback(IAsyncResult ar)
        {
            NetworkStream stream = (NetworkStream)ar.AsyncState;
            int lidos;
            try
            {
                lidos = stream.EndRead(ar);
            }
            catch (ObjectDisposedException) { return; }

            if (lidos == 0) return;

            m_ReceiveBuffer.ConfirmWrite(lidos);
            ByteStream packetStream;
            while (DelimitedMessage.TryGetPacket(m_ReceiveBuffer, out packetStream))
            {
                OnPacketReceived?.Invoke(this, packetStream);
            }

            m_ReceiveBuffer.Compact();

            RegisterWaitData();
        }

        public void Close()
        {
            m_Stream?.Close();
            m_Stream?.Dispose();
            m_Client?.Close();
            m_Stream = null;
            m_Client = null;
        }

        public void Send(IMessage message)
        {
            var stream = m_Stream;
            if (stream != null)
            {
                DelimitedMessage.WritePacket(message, stream);
                stream.Flush();
            }
        }

        public void Start()
        {
            RegisterWaitData();
        }
    }
}
