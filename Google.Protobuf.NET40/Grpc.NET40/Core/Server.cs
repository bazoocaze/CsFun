using Grpc.Core;
using Grpc.Core.Internal;
using Grpc.Extras;
using Grpc.Packet;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Grpc.Core
{
    /// <summary>
    /// Representa um servidor de rede para requisições de serviços RPC.
    /// </summary>
    public class Server
    {
        protected List<ServerPort> m_Ports;
        protected List<ServerServiceDefinition> m_Services;
        protected ServiceDefinitionCollection m_ServiceCollection;

        protected List<SimpleTcpServer> m_tcpServers;
        internal List<ServerSession> m_Sessoes;

        public Server()
        {
            m_Ports = new List<ServerPort>();
            m_Services = new List<ServerServiceDefinition>();
            m_ServiceCollection = new ServiceDefinitionCollection(this);
        }

        public IList<ServerPort> Ports { get { return m_Ports; } }
        public ServiceDefinitionCollection Services { get { return m_ServiceCollection; } }

        /// <summary>
        /// Inicia o servidor de rede ouvindo nas portas registradas e atendendo os serviços registrados.
        /// Gera um IOException caso nenhum porta de rede possa ser configurada para recebimento de conexões.
        /// </summary>
        public virtual void Start()
        {
            if ((m_Ports?.Count ?? 0) == 0) throw new InvalidOperationException("Nenhuma porta TCP informada para o servidor");
            if ((m_Services?.Count ?? 0) == 0) throw new InvalidOperationException("Nenhum serviço informado para o servidor");

            if (m_Sessoes != null && m_tcpServers != null) return;
            m_Sessoes = new List<ServerSession>();
            m_tcpServers = new List<SimpleTcpServer>();

            Exception lastExpception = null;

            foreach (var endpoint in m_Ports)
            {
                try
                {
                    IPAddress address = null;
                    if (IPAddress.TryParse(endpoint.Address, out address))
                    {
                        SimpleTcpServer server = new SimpleTcpServer();
                        server.Start(address, endpoint.Port);
                        server.ClientConnectedAction = OnClientConnected;
                        m_tcpServers.Add(server);
                    }
                }
                catch (SocketException ex) { lastExpception = ex; }
            }

            // Gera uma exception caso nenhuma porta posssa ser configurada para escuta.
            if (m_tcpServers.Count == 0)
                throw lastExpception;
        }

        /// <summary>
        /// Finaliza a executação do servidor.
        /// Todas as RPCs pendentes são finalizas, bem como as conexões de rede são liberadas.
        /// </summary>
        public void Shutdown()
        {
            var servers = m_tcpServers;
            var sessoes = m_Sessoes;
            m_tcpServers = null;
            m_Sessoes = null;

            if (servers != null)
            {
                foreach (var server in servers)
                    server.Stop();
            }

            if (sessoes != null)
            {
                foreach (var sessao in sessoes)
                    sessao.Dispose();
            }
        }

        private void OnClientConnected(object sender, TcpClient newClient)
        {
            PacketTransport transport = TransportFactoryCreateTransport(newClient);
            ServerSession session = new ServerSession(transport);
            session.ProcessPacketAction = OnProcessPacket;
            session.Start();
        }

        private void OnProcessPacket(IPacketSession session, InternalPacket packet)
        {
            foreach (var service in m_Services)
            {
                IServerCallHandler methodHandler = null;
                if (service.CallHandlers.TryGetValue(packet.FullMethodName, out methodHandler))
                {
                    // TODO: Server: analise the need to store the call tasks.
                    methodHandler.HandleCall(session, packet);
                    return;
                }
            }

            // Discard unknow or unexpected packet
        }

        private PacketTransport TransportFactoryCreateTransport(TcpClient newClient)
        {
            return new PacketTransport(newClient);
        }

        internal void AddServiceDefinitionInternal(ServerServiceDefinition service)
        {
            m_Services.Add(service);
        }


        /// <summary>
        /// Collection of service definitions.
        /// </summary>
        public class ServiceDefinitionCollection : IEnumerable<ServerServiceDefinition>
        {
            readonly Server server;

            internal ServiceDefinitionCollection(Server server)
            {
                this.server = server;
            }

            /// <summary>
            /// Adds a service definition to the server. This is how you register
            /// handlers for a service with the server. Only call this before Start().
            /// </summary>
            public void Add(ServerServiceDefinition serviceDefinition)
            {
                server.AddServiceDefinitionInternal(serviceDefinition);
            }

            /// <summary>
            /// Gets enumerator for this collection.
            /// </summary>
            public IEnumerator<ServerServiceDefinition> GetEnumerator()
            {
                return server.m_Services.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return server.m_Services.GetEnumerator();
            }
        }

    }
}
