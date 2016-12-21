using Grpc.Core;
using Grpc.Core.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Grpc.Packet;
using Grpc.Extras;

namespace Grpc.Core.Internal
{
    /// <summary>
    /// Representa um RPC sobre uma sessão cliente/servidor.
    /// </summary>
    public interface IRpc : IDisposable
    {
        /// <summary>
        /// Retorna o número da mensagem para comunicação do RPC.
        /// </summary>
        /// <returns>Retorna o número da mensagem para comunicação do RPC.</returns>
        int GetMsgId();

        /// <summary>
        /// Cancela o RPC.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Callback de notificação de pacote recebido.
        /// </summary>
        /// <param name="session">Sessão de origem da comunicação.</param>
        /// <param name="packet">Pacote contendo a requisição/resposta.</param>
        void OnPacketReceived(IPacketSession session, InternalPacket packet);

        /// <summary>
        /// Callback de notificação de desconexão do transporte de rede.
        /// </summary>
        /// <param name="session">Sessão de origem da comunicação.</param>
        /// <param name="reason">Razão da desconexão.</param>
        void OnDisconnected(IPacketSession session, DisconnectReason reason);
    }


    /// <summary>
    /// Representa um tunel genérico de mensagens (requisições/respostas) sobre uma sessão RPC
    /// cliente/servidor.
    /// </summary>
    /// <typeparam name="TSend">Tipo de dados das mensagens a serem enviadas para o host remoto.</typeparam>
    /// <typeparam name="TReceive">Tipo de dados das mensagens a serem recebidas do host remoto.</typeparam>
    public class RpcTunnel<TSend, TReceive> : IRpc
    {
        /// <summary>
        /// Representa um pacote de dados sobre o stream RPC.
        /// </summary>
        public const int P_OPTIONS_STREAM_DATA = 100;

        /// <summary>
        /// Representa um pacote de finalização do stream RPC.
        /// </summary>
        public const int P_OPTIONS_STREAM_CLOSE = 101;

        /// <summary>
        /// Representa o pacote de resultado final do processamento da RPC.
        /// </summary>
        public const int P_OPTIONS_RPC_RETURN = 102;

        /// <summary>
        /// Representa um pacote de encerramento da conexão RPC (sem valor de resultado final).
        /// </summary>
        public const int P_OPTIONS_RPC_CLOSE = 103;

        /// <summary>
        /// Representa um pacote de encerramento de RPC com condição de erro remoto.
        /// </summary>
        public const int P_OPTIONS_RPC_ERROR = 104;


        /// <summary>
        /// Estado de conexão do stream RPC.
        /// </summary>
        public enum StreamState
        {
            /// <summary>
            /// RPC não conectado.
            /// </summary>
            NotConnected,

            /// <summary>
            /// RPC aberto.
            /// </summary>
            Open,

            /// <summary>
            /// RPC fechado.
            /// </summary>
            Closed,

            /// <summary>
            /// RPC cancelado.
            /// </summary>
            Canceled,

            /// <summary>
            /// RPC desconectado por erro.
            /// </summary>
            Error,

            /// <summary>
            /// RPC finalizado.
            /// </summary>
            Disposed
        }

        private int m_MsgId;
        private TaskCompletionSource<TReceive> m_ReturnValueTcs;
        private StreamState m_State;
        private IPacketSession m_Session;
        private Method<TSend, TReceive> m_Method;

        private RpcStreamReader m_RpcStreamReader;
        private RpcStreamWriter m_RpcStreamWriter;


        /// <summary>
        /// Construtor padrão de um RPC sobre a sessão informada.
        /// O tipo de dados dos pacotes de requisição e responsta é dado pelo Method.
        /// </summary>
        /// <param name="session">Sessão de comunicação/transporte.</param>
        /// <param name="method">Informação sobre o tipo de dados da requisição/resposta.</param>
        /// <param name="msgId">Número da mensagem (opcional).</param>
        public RpcTunnel(IPacketSession session, Method<TSend, TReceive> method, int msgId = -1)
        {
            this.m_Session = session;
            this.m_Method = method;
            this.m_ReturnValueTcs = new TaskCompletionSource<TReceive>();

            if (msgId >= 0)
                this.m_MsgId = msgId;
            else
                this.m_MsgId = session.GetNewMsgId();

            m_RpcStreamReader = new RpcStreamReader(this);
            m_RpcStreamWriter = new RpcStreamWriter(this);

            this.m_State = StreamState.NotConnected;
        }

        /// <summary>
        /// Inicia o RPC sem enviar uma requisição (início passivo/servidor).
        /// </summary>
        public void Start()
        {
            this.m_State = StreamState.Open;
            m_Session.AddRpc(this);            
        }

        /// <summary>
        /// Inicia o RPC enviando uma requisição vazia para o ponto remoto.
        /// </summary>
        public void StartRequest()
        {
            var packet = PacketFactory.NewPacket(m_MsgId, m_Method.FullName);
            StartRequest(packet);
        }

        /// <summary>
        /// Inicia o RPC enviando a requisição para o ponto remoto.
        /// </summary>
        /// <param name="request"></param>
        public void StartRequest(TSend request)
        {
            var packet = PacketFactory.NewPacket(m_MsgId, m_Method.FullName, m_Method.RequestMarshaller, request);
            StartRequest(packet);
        }

        /// <summary>
        /// Inicia o RPC enviando o pacote de dados para o ponto remoto.
        /// </summary>
        /// <param name="packet"></param>
        private void StartRequest(InternalPacket packet)
        {
            m_Session.AddRpc(this);
            this.m_State = StreamState.Open;
            try
            {
                m_Session.Send(packet);
            }
            catch (Exception)
            {
                this.m_State = StreamState.Closed;
                m_Session.RemoveRpc(this);
                throw;
            }
        }

        /// <summary>
        /// Promessa do valor de retorno do RPC.
        /// </summary>
        /// <returns>A promessa do valor de retorno do RPC.</returns>
        public Task<TReceive> ReturnValueAsync()
        {
            if (m_ReturnValueTcs == null)
                m_ReturnValueTcs = new TaskCompletionSource<TReceive>();

            return m_ReturnValueTcs.Task;
        }

        /// <summary>
        /// Encerra o RPC.
        /// As chamadas pendentes locais são canceladas.
        /// As chamadas pendentes remotas são desconectadas.
        /// </summary>
        public void Close()
        {
            if (m_State != StreamState.Open) return;

            m_State = StreamState.Closed;
            var packet = PacketFactory.NewPacket(m_MsgId, P_OPTIONS_RPC_CLOSE);
            m_Session.TrySend(packet);
            m_ReturnValueTcs.TrySetCanceled();

            Finish();
        }

        /// <summary>
        /// Encerra o RPC, enviando o valor de retorno para o ponto remoto.
        /// As chamadas pendentes locais são canceladas.
        /// As chamadas pendentes remotas são concluídas com o valor de retorno informado.
        /// </summary>
        /// <param name="request">Valor de return para encerramento do RPC.</param>
        public void Close(TSend request)
        {
            if (m_State != StreamState.Open) return;

            m_State = StreamState.Closed;
            var packet = PacketFactory.NewPacket(m_MsgId, P_OPTIONS_RPC_RETURN, m_Method.RequestMarshaller, request);
            m_Session.TrySend(packet);
            m_ReturnValueTcs.TrySetCanceled();

            Finish();
        }

        /// <summary>
        /// Encerra o RPC conforme o resultado da finalização da task de entrada.
        /// Caso a task tenha sido encerrada com sucesso, o RPC é desconectado sem o envio de
        /// um valor de retorno, encerrando as chamadas remotas por desconexão (close).
        /// Caso a task tenha sido encerrada com erro, um erro RpcException é enviado
        /// para o ponto remoto, encerrando as chamadas remotas com erro.
        /// Em qualquer caso, as chamadas locais são canceladas/encerradas por desconexão.
        /// </summary>
        /// <param name="taskSend">Task de processamento do retorno do RPC</param>
        public void Close(Task taskSend)
        {
            if (m_State != StreamState.Open) return;
            m_State = StreamState.Closed;

            InternalPacket packet;

            Exception ex = null;
            if (taskSend.IsFaulted)
                ex = taskSend.Exception;
            else if (taskSend.IsCanceled)
                ex = new OperationCanceledException();

            if (taskSend.IsFaulted || taskSend.IsCanceled)
            {
                // Failed return
                var rpcException = RpcExceptionFactory.CreateFrom(ex);
                packet = PacketFactory.NewPacket(m_MsgId, P_OPTIONS_RPC_ERROR, rpcException);
            }
            else
            {
                // Normal return
                packet = PacketFactory.NewPacket(m_MsgId, P_OPTIONS_RPC_CLOSE);
            }

            m_Session.TrySend(packet);
            m_ReturnValueTcs.TrySetCanceled();
            Finish();
        }

        /// <summary>
        /// Encerra o RPC conforme o resultado da finalização da task de entrada.
        /// Caso a task tenha sido encerrada com sucesso, o RPC é desconectado com o envio do
        /// valor de retorno da task, encerrando as chamadas remotas com sucesso usando o valor
        /// de retorno em questão.
        /// Caso a task tenha sido encerrada com erro, um erro RpcException é enviado
        /// para o ponto remoto, encerrando as chamadas remotas com erro.
        /// Em qualquer caso, as chamadas locais são canceladas/encerradas por desconexão.
        /// </summary>
        /// <param name="taskSend">Task de processamento do retorno do RPC</param>
        public void Close(Task<TSend> taskSend)
        {
            if (m_State != StreamState.Open) return;
            m_State = StreamState.Closed;

            InternalPacket packet;

            if (taskSend.IsFaulted || taskSend.IsCanceled)
            {
                var rpcException = RpcExceptionFactory.CreateFrom(taskSend.Exception);
                packet = PacketFactory.NewPacket(m_MsgId, P_OPTIONS_RPC_ERROR, rpcException);
            }
            else
            {
                var data = taskSend.Result;
                packet = PacketFactory.NewPacket(m_MsgId, P_OPTIONS_RPC_RETURN, m_Method.RequestMarshaller, data);
            }

            m_Session.TrySend(packet);
            m_ReturnValueTcs.TrySetCanceled();
            Finish();
        }

        /// <summary>
        /// Cancela o RPC atual.
        /// As chamadas locais pendentes são canceladas/encerradas por desconexão.
        /// As chamadas remotas pendentes são canceladas caso ainda conectado.
        /// </summary>
        public void Cancel()
        {
            if (m_State == StreamState.Open)
            {
                m_State = StreamState.Canceled;
                var rpcException = RpcExceptionFactory.Create(StatusCode.Cancelled, "A operação foi cancelada.");
                var packet = PacketFactory.NewPacket(m_MsgId, P_OPTIONS_RPC_ERROR, rpcException);
                m_Session.Send(packet);
                m_ReturnValueTcs.TrySetCanceled();

                Finish();
            }
        }

        /// <summary>
        /// Finaliza o RPC, liberando os recursos alocados e finalizando todas as requisições.
        /// Todas as chamadas pendentes (locais e remotas) são canceladas.
        /// </summary>
        public void Dispose()
        {
            Cancel();
            Finish();
            m_State = StreamState.Disposed;
        }

        /// <summary>
        /// Finalização comum do RPC.
        /// Desregistra da sessão para recebimentos de mais mensagens.
        /// </summary>
        private void Finish()
        {
            m_Session?.RemoveRpc(this);
        }

        /// <summary>
        /// Callback de notificação de cancelamento do RPC.
        /// </summary>
        void IRpc.Cancel()
        {
            Cancel();
        }

        /// <summary>
        /// Callback para Dispose do objeto.
        /// </summary>
        void IDisposable.Dispose()
        {
            Dispose();
        }

        /// <summary>
        /// Retorna o número de mensagem para o RPC atual.
        /// </summary>
        /// <returns>Retorna o número de mensagem para o RPC atual.</returns>
        int IRpc.GetMsgId()
        { return m_MsgId; }

        /// <summary>
        /// Callback de notificação de desconexão do transporte.
        /// </summary>
        /// <param name="session">Sessão de origem da desconexão.</param>
        /// <param name="reason">Razão de desconexão.</param>
        void IRpc.OnDisconnected(IPacketSession session, DisconnectReason reason)
        {
            if (m_State != StreamState.Open) return;

            m_State = StreamState.Error;
            m_ReturnValueTcs.TrySetException(reason.GetException());

            Finish();
        }

        /// <summary>
        /// Desempacota o payload de dentro do pacote recebido.
        /// </summary>
        /// <param name="packet">Pacote contendo os dados recebidos.</param>
        /// <returns>Dados desempacotados.</returns>
        TReceive GetData(InternalPacket packet)
        {
            return PacketHandler.GetData(packet, m_Method.ResponseMarshaller);
        }

        /// <summary>
        /// Callback de notificação de recebimento de pacote.
        /// </summary>
        /// <param name="session">Sessão de origem do pacote.</param>
        /// <param name="packet">Pacote de dados.</param>
        void IRpc.OnPacketReceived(IPacketSession session, InternalPacket packet)
        {
            if (m_State != StreamState.Open)
            {
                Debug.WriteLine(String.Format("{0}.OnPacketReceived: Unexpected packet: {1}", nameof(RpcTunnel<TSend, TReceive>), packet));
                return;
            }

            var options = packet.Options;

            if (options == P_OPTIONS_RPC_ERROR)
            {
                m_State = StreamState.Error;

                var rpcException = PacketHandler.GetException(packet);
                m_ReturnValueTcs.TrySetException(rpcException);
                m_RpcStreamReader.OnError(rpcException);

                Finish();
                return;
            }

            if (options == P_OPTIONS_RPC_CLOSE)
            {
                m_State = StreamState.Closed;

                Exception ex = new EndOfStreamException();
                m_ReturnValueTcs.TrySetException(ex);
                m_RpcStreamReader.OnDataClosed();

                Finish();
                return;
            }

            if (options == P_OPTIONS_RPC_RETURN)
            {
                m_State = StreamState.Closed;

                var data = GetData(packet);
                m_ReturnValueTcs.TrySetResult(data);
                m_RpcStreamReader.OnDataClosed();

                Finish();
                return;
            }

            if (options == P_OPTIONS_STREAM_DATA)
            {
                var data = GetData(packet);
                m_RpcStreamReader.OnDataReceived(data);
                return;
            }

            if (options == P_OPTIONS_STREAM_CLOSE)
            {
                m_RpcStreamReader.OnDataClosed();
                return;
            }

            // Unknow or unexpected value -> discard packet

            MyDebug.LogDebug("{0}.OnPacketReceived: Unknow or unexpected packet: {1}", nameof(RpcTunnel<TSend, TReceive>), packet);
        }

        /// <summary>
        /// Retorna um receiver de stream de respostas.
        /// </summary>
        /// <returns>Retorna um receiver de stream de respostas.</returns>
        public RpcStreamReader GetReceiver()
        {
            return m_RpcStreamReader;
        }

        /// <summary>
        /// Retorna um writer de stream de requisições.
        /// </summary>
        /// <returns>Retorna um writer de stream de requisições.</returns>
        public RpcStreamWriter GetSender()
        {
            return m_RpcStreamWriter;
        }
        
        private Task Sender_WriteAsync(TSend data)
        {
            return Task.Factory.StartNew(() =>
            {
                if (m_State != StreamState.Open) return;

                var packet = PacketFactory.NewPacket(this.m_MsgId, P_OPTIONS_STREAM_DATA, m_Method.RequestMarshaller, data);
                m_Session?.Send(packet);
            });
        }

        private Task Sender_CompleteAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                if (m_State != StreamState.Open) return;

                var packet = PacketFactory.NewPacket(this.m_MsgId, P_OPTIONS_STREAM_CLOSE);
                m_Session?.Send(packet);
            });
        }
        
        /// <summary>
        /// Representa um writer de stream de requisições.
        /// </summary>
        public class RpcStreamWriter : IClientStreamWriter<TSend>, IServerStreamWriter<TSend>
        {
            RpcTunnel<TSend, TReceive> m_Rpc;
            WriteOptions m_WriteOptions = new WriteOptions();

            /// <summary>
            /// Construtor padrão para um stream de requisições sobre o RPC informado.
            /// </summary>
            /// <param name="rpc">RPC que serve de túnel para o stream.</param>
            public RpcStreamWriter(RpcTunnel<TSend, TReceive> rpc)
            {
                m_Rpc = rpc;
            }

            WriteOptions IAsyncStreamWriter<TSend>.WriteOptions
            {
                get { return m_WriteOptions; }
                set { m_WriteOptions = value; }
            }

            Task IClientStreamWriter<TSend>.CompleteAsync()
            {
                return m_Rpc.Sender_CompleteAsync();
            }

            Task IAsyncStreamWriter<TSend>.WriteAsync(TSend message)
            {
                return m_Rpc.Sender_WriteAsync(message);
            }
        }
        
        /// <summary>
        /// Representa um reader de stream de respostas.
        /// </summary>
        public class RpcStreamReader : IAsyncStreamReader<TReceive>
        {
            private RpcTunnel<TSend, TReceive> m_Rpc;
            private TReceive m_Current;
            private TaskCompletionSource<bool> m_MoveNext;
            private ReaderState m_State;
            private Exception m_StreamException;

            private object m_DataLock = new object();
            private Queue<TReceive> m_Data = new Queue<TReceive>();

            private enum ReaderState
            {
                Open,
                Closed,
                Error
            }

            /// <summary>
            /// Construtor padrão para o reader de stream de respostas sobre o RPC informado.
            /// </summary>
            /// <param name="rpc">RPC que serve de túnel para o stream.</param>
            public RpcStreamReader(RpcTunnel<TSend, TReceive> rpc)
            {
                m_Rpc = rpc;
                m_State = ReaderState.Open;
            }

            /// <summary>
            /// Callback de notificação de dados recebidos no stream.
            /// </summary>
            /// <param name="data">Dados recebidos já desempacotados.</param>
            public void OnDataReceived(TReceive data)
            {
                lock (m_DataLock)
                {
                    if (m_State != ReaderState.Open) return;

                    if (m_MoveNext != null)
                    {
                        m_Current = data;
                        m_MoveNext.TrySetResult(true);
                        m_MoveNext = null;
                        return;
                    }

                    m_Data.Enqueue(data);
                }
            }

            /// <summary>
            /// Callback de notificação de stream fechado.
            /// A leitura pendente, bem como as próximas leituras são finalizadas com EOF.
            /// </summary>
            public void OnDataClosed()
            {
                lock (m_DataLock)
                {
                    if (m_State != ReaderState.Open) return;

                    m_State = ReaderState.Closed;

                    if (m_MoveNext != null)
                    {
                        m_MoveNext.TrySetResult(false);
                        m_MoveNext = null;
                        return;
                    }
                }
            }

            /// <summary>
            /// Callback de notificação de desconexão por erro.
            /// A leitura pendente, bem como as próximas leituras resultam em um
            /// erro 'ex'.
            /// </summary>
            /// <param name="ex">Razão da desconexão.</param>
            public void OnError(Exception ex)
            {
                lock (m_DataLock)
                {
                    if (m_State != ReaderState.Open) return;

                    m_State = ReaderState.Error;
                    m_StreamException = ex;

                    if (m_MoveNext != null)
                    {
                        m_MoveNext.TrySetException(ex);
                        m_MoveNext = null;
                        return;
                    }
                }
            }

            /// <summary>
            /// Item corrente do reader.
            /// </summary>
            TReceive IAsyncEnumerator<TReceive>.Current
            {
                get { return m_Current; }
            }

            /// <summary>
            /// Finaliza o stream e libera os recursos alocados.
            /// As leituras pendentes são canceladas.
            /// </summary>
            void IDisposable.Dispose()
            {
                m_MoveNext?.TrySetCanceled();
                m_MoveNext = null;
            }

            /// <summary>
            /// Tenta mover para o próximo item do stream.
            /// Retorna true em caso de sucesso em mover para o próximo item, que fica
            /// disponível no membro Current.
            /// Retorna false caso atinja o fim do stream.
            /// </summary>
            /// <param name="cancellationToken">Token que permite o cancelamento da requisião assíncrona.</param>
            /// <returns>Returna true/false caso tenha conseguido mover para o próximo item.</returns>
            Task<bool> IAsyncEnumerator<TReceive>.MoveNext(CancellationToken cancellationToken)
            {
                lock (m_DataLock)
                {
                    TaskCompletionSource<bool> ret = new TaskCompletionSource<bool>();

                    if (m_Data.Count > 0)
                    {
                        var data = m_Data.Dequeue();
                        m_Current = data;
                        ret.TrySetResult(true);
                    }
                    else if (m_State == ReaderState.Closed)
                    {
                        ret.TrySetResult(false);
                    }
                    else if (m_State == ReaderState.Error)
                    {
                        ret.TrySetException(m_StreamException);
                    }
                    else
                    {
                        m_MoveNext = ret;
                    }
                    return ret.Task;
                }
            }
        }
        
    }
}
