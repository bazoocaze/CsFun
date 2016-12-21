using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core.Internal;

namespace Grpc.Core
{
    /// <summary>
    /// Representa uma chamada de cliente assíncrona, onde o cliente envia requisições
    /// TRequest para o servidor e recebe respostas TResponse.
    /// As requisições e responstas seguem um fluxo stream de múltiplos pacotes.
    /// </summary>
    /// <typeparam name="TRequest">Tipo de dados das requisições do cliente</typeparam>
    /// <typeparam name="TResponse">Tipo de dados das responstas do servidor</typeparam>
    public class AsyncDuplexStreamingCall<TRequest, TResponse>
    {
        private RpcTunnel<TRequest, TResponse> m_Rpc;

        internal AsyncDuplexStreamingCall(RpcTunnel<TRequest, TResponse> rpc)
        {
            this.m_Rpc = rpc;
        }

        /// <summary>
        /// Finaliza a chamada assíncrona, liberando os recursos alocados.
        /// As chamadas pendentes locais e remotas são canceladas.
        /// </summary>
        public void Dispose()
        {
            m_Rpc?.Dispose();
            m_Rpc = null;
        }

        /// <summary>
        /// Async stream to read streaming responses.
        /// </summary>
        public IAsyncStreamReader<TResponse> ResponseStream
        { get { return m_Rpc.GetReceiver(); } }

        /// <summary>
        /// Async stream to send streaming requests.
        /// </summary>
        public IClientStreamWriter<TRequest> RequestStream
        { get { return m_Rpc.GetSender(); } }

    }
}
