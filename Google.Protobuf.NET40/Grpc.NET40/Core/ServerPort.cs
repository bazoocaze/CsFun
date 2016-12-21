using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.Core
{
    /// <summary>
    /// Representa uma porta de rede para configuração do servidor.
    /// </summary>
    public class ServerPort
    {
        /// <summary>
        /// Endereço do servidor para escuta de conexões.
        /// </summary>
        public string Address { get; protected set; }

        /// <summary>
        /// Número da porta TCP.
        /// </summary>
        public int Port { get; protected set; }

        /// <summary>
        /// Construtor padrão para configuração do servidor no endereço
        /// e porta informados.
        /// </summary>
        /// <param name="address">Endereço do servidor para escuta de conexões.</param>
        /// <param name="port">Número da porta TCP.</param>
        public ServerPort(string address, int port)
        {
            this.Address = address;
            this.Port = port;
        }
    }
}
