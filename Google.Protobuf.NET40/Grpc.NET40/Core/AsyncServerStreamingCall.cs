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
    /// Representa uma chamada de cliente assíncrona, onde o cliente recebe
    /// um fluxo stream de TResponse em resposta.
    /// </summary>
    /// <typeparam name="TResponse">Tipo de dados das respostas do servidor</typeparam>
    public abstract class AsyncServerStreamingCall<TResponse> : IDisposable
    {
        /// <summary>
        /// Async stream to read streaming responses.
        /// </summary>
        public abstract IAsyncStreamReader<TResponse> ResponseStream { get; }

        /// <summary>
        /// Finaliza a chamada assíncrona, liberando os recursos alocados.
        /// As chamadas pendentes locais e remotas são canceladas.
        /// </summary>
        public abstract void Dispose();
    }


    /// <summary>
    /// Implementação da chamada de cliente assíncrona, onde o cliente envia uma
    /// requisição TRequest para o servidor e recebe um fluxo stream de TResponse
    /// em resposta.
    /// </summary>
    /// <typeparam name="TRequest">Tipo de dados da requisição do cliente</typeparam>
    /// <typeparam name="TResponse">Tipo de dados das respostas do servidor</typeparam>
    public class AsyncServerStreamingCallImpl<TRequest, TResponse>
        : AsyncServerStreamingCall<TResponse>
    {
        private RpcTunnel<TRequest, TResponse> m_Rpc;

        internal AsyncServerStreamingCallImpl(RpcTunnel<TRequest, TResponse> rpc, IPacketSession session, Method<TRequest, TResponse> method)
        {
            this.m_Rpc = rpc;
        }

        /// <summary>
        /// Finaliza a chamada assíncrona, liberando os recursos alocados.
        /// As chamadas pendentes locais e remotas são canceladas.
        /// </summary>
        public override void Dispose()
        {
            m_Rpc?.Dispose();
            m_Rpc = null;
        }

        /// <summary>
        /// Async stream to read streaming responses.
        /// </summary>
        public override IAsyncStreamReader<TResponse> ResponseStream
        { get { return m_Rpc.GetReceiver(); } }
    }
}
