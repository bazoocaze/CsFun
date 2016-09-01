using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TesteScanner.Bytes;

namespace TesteScanner.Network
{
    public class Client
    {
        private const int P_NETWORK_READ_SIZE = 256;

        public static Client Conectar(IPEndPoint remoteEP)
        {
            TcpClient tcpClient = new TcpClient();
            tcpClient.Connect(remoteEP);
            return new Client(tcpClient);
        }

        public static Task<Client> ConectarAsync(IPEndPoint remoteEP)
        {
            return new Task<Client>(() => Conectar(remoteEP));
        }

        protected TcpClient m_Client;
        protected NetworkStream m_Stream;
        protected Action<Client, ByteStream> m_PacketReceived;
        protected ByteBuilder m_ReceiveBuffer;

        public Client(TcpClient client)
        {
            m_Client = client;
        }

        public void Start(Action<Client, ByteStream> packetReceived)
        {
            m_ReceiveBuffer = new ByteBuilder();
            m_PacketReceived = packetReceived;
            m_Stream = m_Client.GetStream();
            RegisterReceiveData();
        }

        private void RegisterReceiveData()
        {
            if (m_Stream != null)
            {
                var ptr = m_ReceiveBuffer.LockWrite(P_NETWORK_READ_SIZE);
                m_Stream.BeginRead(ptr.Ptr, ptr.Offset, ptr.Size, DataReceivedCallback, m_Stream);
            }
        }

        private void DataReceivedCallback(IAsyncResult ar)
        {
            NetworkStream ns = (NetworkStream)ar.AsyncState;
            int lidos = ns.EndRead(ar);
            m_ReceiveBuffer.ConfirmWrite(lidos);
            if (lidos == 0)
            {
                m_PacketReceived?.Invoke(this, null);
                EndConnection();
            }

            ByteStream packetStream = null;
            while (DelimitedMessage.TryGetPacket(m_ReceiveBuffer, out packetStream))
            {
                m_PacketReceived?.Invoke(this, packetStream);
            }

            if (packetStream != null) m_ReceiveBuffer.Compact();
        }

        private void EndConnection()
        {

        }

        public void Send(IMessage message)
        {
            if (m_Stream != null) DelimitedMessage.WritePacket(message, m_Stream);
        }

        public void SendRequestResponse()
        {
        }
    }
}
