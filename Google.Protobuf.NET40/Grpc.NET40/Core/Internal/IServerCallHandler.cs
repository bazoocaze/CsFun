using Grpc.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core.Internal
{
    /// <summary>
    /// Interface padrão para um handler de chamadas no servidor.
    /// </summary>
    internal interface IServerCallHandler
    {
        /// <summary>
        /// Manipulador de chamada no servidor.
        /// Retorna a task de execução do manipulador.
        /// </summary>
        /// <param name="session">Sessão de origem do pacote.</param>
        /// <param name="packet">Pacote de dados contendo a requisição.</param>
        /// <returns>Retorna a task de execução do manipulador.</returns>
        Task HandleCall(IPacketSession session, InternalPacket packet);
    }
}
