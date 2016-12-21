using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Extras
{
    /// <summary>
    /// Representa um servidor de rede TCP simples.
    /// O servidor aguarda conexões assincronamente, e executa uma ação (delegate) para cada
    /// conexão cliente recebida.
    /// </summary>
    public class SimpleTcpServer : IDisposable
    {
        /// <summary>
        /// Template para a ação a ser executada quando for recebida uma nova conexão.
        /// </summary>
        /// <param name="sender">Servidor TCP origem da conexão.</param>
        /// <param name="newClient"></param>
        public delegate void ClientConnectedDelegate(SimpleTcpServer sender, TcpClient newClient);

        /// <summary>
        /// Ação a ser executada quando for recebida uma nova conexão.
        /// </summary>
        public ClientConnectedDelegate ClientConnectedAction;

        /// <summary>
        /// Endpoint local corrente do servidor TCP.
        /// </summary>
        public IPEndPoint LocalEndpoint { get; protected set; }

        /// <summary>
        /// Listener interno do servidor TCP.
        /// </summary>
        protected TcpListener m_listener;

        /// <summary>
        /// Indica se o objeto foi finalizado via Dispose.
        /// </summary>
        private bool m_disposed;

        /// <summary>
        /// Construtor padrão do servidor TCP.
        /// </summary>
        public SimpleTcpServer()
        {
        }

        /// <summary>
        /// Retorna true/false se o servidor está aguardando conexões.
        /// </summary>
        public bool IsListening
        { get { return m_listener?.Server?.IsBound ?? false; } }

        /// <summary>
        /// Inicia o servidor e aguarda conexões no IP e porta informados.
        /// </summary>
        /// <param name="address">IP onde aguardar conexões.</param>
        /// <param name="port">Porta TCP de escuta das conexões.</param>
        public void Start(IPAddress address, int port)
        {
            if (m_disposed) throw new ObjectDisposedException(nameof(SimpleTcpServer));

            InternalStart(new IPEndPoint(address, port));
        }

        /// <summary>
        /// Inicia o servidor e aguarda conexões no IP e porta informados.
        /// </summary>
        /// <param name="localEP">IP e porta TCP de escuta das conexões.</param>
        public void Start(IPEndPoint localEP)
        {
            if (m_disposed) throw new ObjectDisposedException(nameof(SimpleTcpServer));
            if (localEP == null) throw new ArgumentNullException(nameof(localEP));

            InternalStart(localEP);
        }

        /// <summary>
        /// Finaliza o servidor, parando de aguardar conexões de rede.
        /// </summary>
        public void Stop()
        {
            if (m_disposed) throw new ObjectDisposedException(nameof(SimpleTcpServer));

            // Finaliza o listener/servidor
            m_listener?.Stop();
        }

        /// <summary>
        /// Finaliza o servidor, liberando todos os recursos alocados.
        /// </summary>
        public void Dispose()
        {
            if (m_disposed) return;
            m_disposed = true;

            // Finaliza o listener/servidor
            m_listener?.Stop();
            m_listener = null;
        }

        /// <summary>
        /// Iniciar o servidor TCP no endpoint informado.
        /// </summary>
        /// <param name="localEP">Endpoint de escuta.</param>
        private void InternalStart(IPEndPoint localEP)
        {
            // Finaliza o servidor atual
            m_listener?.Stop();

            this.LocalEndpoint = localEP;

            // Cria e iniciar o novo servidor
            TcpListener listener = new TcpListener(localEP);
            listener.Start();
            m_listener = listener;

            // Registra para recebimento de novos clientes
            RegisterWaitClient();
        }

        /// <summary>
        /// Registra e prepara para recebimento assíncrono de conexões.
        /// </summary>
        private void RegisterWaitClient()
        {
            var listener = m_listener;
            if (m_disposed || listener == null || (!IsListening)) return;

            try
            {
                // Registra para recebimento assíncrono de clientes
                listener?.BeginAcceptTcpClient(ClientConnectedCallback, listener);
            }
            catch (ObjectDisposedException) { } /* caso o listener tenha sido finalizado */
            catch (IOException ex) { OnIOException(ex); }
        }

        /// <summary>
        /// Callback de recebimento assíncrono de conexão de rede.
        /// </summary>
        /// <param name="ar">AsyncResult da callback</param>
        private void ClientConnectedCallback(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient newClient = null;
            try
            {
                // Recebe o novo cliente (assíncrono)
                newClient = listener.EndAcceptTcpClient(ar);
            }
            catch (ObjectDisposedException) { return; } /* caso o listener tenha sido finalizado */
            catch (IOException) { RegisterWaitClient(); return; }

            // Notifica o recebimento da nova conexão
            OnClientConnectedCallback(newClient);

            // Registra para receber próxima conexão
            RegisterWaitClient();
        }

        /// <summary>
        /// Callback de conexão recebida.
        /// A implementação padrão executa a ação ClientConnectedAction.
        /// </summary>
        /// <param name="newClient">Novo cliente conectado.</param>
        protected virtual void OnClientConnectedCallback(TcpClient newClient)
        {
            var action = ClientConnectedAction;
            if (action != null)
            {
                // Dispara uma task para não bloquear o recebimento de mais clientes
                Task.Factory.StartNew(() => action.Invoke(this, newClient));
            }
        }

        /// <summary>
        /// Callback de recebimento assíncrono de IOException.
        /// </summary>
        /// <param name="ex">Exceção assíncrona recebida.</param>
        protected virtual void OnIOException(Exception ex)
        {
            MyDebug.LogError("SimpleTcpServer.OnIOException", ex);
        }

    }
}
