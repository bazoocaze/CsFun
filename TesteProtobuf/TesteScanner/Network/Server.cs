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
    public class Server
    {
        private TcpListener m_Listener;
        protected List<Client> m_Conexoes;

        public int Porta;

        public Action<Client, ByteStream> OnPacketReceived;
        public Action<Client> OnClientAccepted;
        public bool AutoStartClients;
        public bool Listening { get { return m_Listener?.Server.IsBound ?? false; } }

        public Server()
        {
            this.Porta = -1;
            this.AutoStartClients = true;
        }

        public Server(int porta)
        {
            this.Porta = porta;
            this.AutoStartClients = true;
        }

        public void Start()
        {
            if (Listening) throw new InvalidProgramException();
            if (this.Porta < 0 || this.Porta > UInt16.MaxValue) throw new ArgumentOutOfRangeException(nameof(Porta));

            IPEndPoint localEP = new IPEndPoint(IPAddress.Any, this.Porta);
            m_Listener = new TcpListener(localEP);
            m_Listener.Start();
            RegisterAcceptClient();
        }

        public void Stop()
        {
            var listener = m_Listener;
            m_Listener = null;
            listener?.Stop();
        }

        private void RegisterAcceptClient()
        {
            m_Listener?.BeginAcceptTcpClient(ClientAcceptedCallback, m_Listener);
        }

        private void ClientAcceptedCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            TcpClient client;
            try
            {
                client = listener.EndAcceptTcpClient(ar);
            }
            catch (ObjectDisposedException) { return; }

            if (m_Conexoes == null) m_Conexoes = new List<Client>();
            var newClient = new Client(client);
            m_Conexoes.Add(newClient);
            if (AutoStartClients) newClient.Start(OnPacketReceived);
            OnClientAccepted?.Invoke(newClient);
            RegisterAcceptClient();
        }
    }
}
