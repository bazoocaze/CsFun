using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Grpc.Core
{
    public class Server
    {
        private List<ServerServiceDefinition> m_Services;

        public Server()
        {
            m_Services = new List<ServerServiceDefinition>();
        }

        public void AddPort(ServerPort serverPort)
        {
        }

        public void Start()
        {

        }

        public Task ShutdownAsync()
        {
            return Task.FromResult(true);
        }

        public IList<ServerServiceDefinition> Services { get { return m_Services; } }
    }
}