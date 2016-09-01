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
    public class SimpleServer
    {
        protected TcpListener m_Listener;

        protected SimpleClient m_CurrentClient;

        public Action<object, SimpleClient> OnClientConnected;
        public Action<SimpleClient, ByteStream> OnPacketReceived;

        public SimpleServer()
        {
        }

        public void Start(int port)
        {
            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, port);
            m_Listener = new TcpListener(localEP);
            m_Listener.Start();

            RegisterWaitClient();
        }

        public void Stop()
        {
            m_Listener?.Stop();
            m_Listener = null;
        }

        private void RegisterWaitClient()
        {
            TcpListener listener = m_Listener;
            if (listener != null)
            {
                listener.BeginAcceptTcpClient(ClientReceivedCallback, listener);
            }
        }

        private void ClientReceivedCallback(IAsyncResult ar)
        {
            var listener = (TcpListener)ar.AsyncState;

            TcpClient client;

            try
            {
                client = listener.EndAcceptTcpClient(ar);
            }
            catch (ObjectDisposedException) { return; }

            ClientReceivedCallback(client);

            RegisterWaitClient();
        }

        private void OnPacketReceivedWrapper(SimpleClient sender, ByteStream packetStream)
        {
            OnPacketReceived?.Invoke(sender, packetStream);
        }

        private void ClientReceivedCallback(TcpClient client)
        {
            m_CurrentClient?.Close();
            m_CurrentClient = new SimpleClient(client);
            m_CurrentClient.OnPacketReceived = OnPacketReceivedWrapper;
            m_CurrentClient.Start();
        }
    }
}
