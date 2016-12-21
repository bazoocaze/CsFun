using Grpc.Core.Internal;
using Grpc.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core
{
    /// <summary>
    /// Representa uma chamada de cliente assíncrona, onde o cliente envia requisições
    /// TRequest para o servidor em um fluxo stream.
    /// O servidor responde com uma única resposta TResponse.
    /// </summary>
    /// <typeparam name="TRequest">Tipo de dados das requisições do cliente</typeparam>
    /// <typeparam name="TResponse">Tipo de dados das responstas do servidor</typeparam>
    public class AsyncClientStreamingCall<TRequest, TResponse> : IDisposable
    {
        private RpcTunnel<TRequest, TResponse> m_Rpc;

        internal AsyncClientStreamingCall(RpcTunnel<TRequest, TResponse> rpc, IPacketSession session, Method<TRequest, TResponse> method)
        {
            this.m_Rpc = rpc;
        }

        /// <summary>
        /// Finaliza a chamada assíncrona, liberando os recursos alocados.
        /// As chamadas pendentes são canceladas.
        /// </summary>
        public void Dispose()
        {
            m_Rpc?.Dispose();
            m_Rpc = null;
        }

        /// <summary>
        /// Asynchronous call result.
        /// </summary>
        public Task<TResponse> ResponseAsync
        { get { return this.m_Rpc.ReturnValueAsync(); } }

        /// <summary>
        /// Async stream to send streaming requests.
        /// </summary>
        public IClientStreamWriter<TRequest> RequestStream
        { get { return m_Rpc.GetSender(); } }
    }
}
