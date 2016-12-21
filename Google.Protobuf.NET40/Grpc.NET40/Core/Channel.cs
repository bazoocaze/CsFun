using Grpc.Core;
using Grpc.Core.Internal;
using Grpc.Extras;
using Grpc.Packet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Grpc.Core
{
    /// <summary>
    /// Representa um canal de comunicação RPC cliente.
    /// </summary>
    public class Channel : IDisposable
    {
        private object m_lastMsgIdLock = new object();
        private object m_transportInitializeLock = new object();

        private string m_remoteHostName;
        private int m_remotePort;

        private int m_lastMsgId;
        private ChannelState m_State;
        private IPacketSession m_Session;

        protected IPacketTransport m_transport;
        protected Dictionary<int, IRpc> m_rpcs;


        protected Channel()
        {
            this.m_State = ChannelState.NotConnected;
            this.m_rpcs = new Dictionary<int, IRpc>();
        }


        /// <summary>
        /// Construtor padrão para um RPC conectado no host:port informados.
        /// </summary>
        /// <param name="host">Endereço do servidor para conectar o RPC</param>
        /// <param name="port">Número da porta TCP para conectar no servidor</param>
        public Channel(string host, int port) : this()
        {
            this.m_remoteHostName = host;
            this.m_remotePort = port;
        }


        /// <summary>
        /// Retorna o estado atual da conexão ao servidor.
        /// </summary>
        public ChannelState State { get { return m_State; } }


        /// <summary>
        /// Tenta conectar no servidor.
        /// Se já estiver conectado, nenhum comando é executado.
        /// Gera uma IOException em caso de erro de conexão, ou
        /// uma exception geral em caso de erro na inicialização do transporte.
        /// </summary>
        public void Connect()
        {
            InternalGetTransport(true);
        }


        /// <summary>
        /// Retorna a sessão interna correspondente ao canal atual.
        /// A sessão é usada para registrar/desregistrar RPC, e enviar/receber
        /// mensagens para o servidor.
        /// </summary>
        /// <returns></returns>
        internal IPacketSession GetSession()
        {
            if (m_Session == null) m_Session = new ChannelSessionWrapper(this);
            return m_Session;
        }


        /// <summary>
        /// Finaliza o canal atual, cancelando todas as RPCs pendentes, desconectando
        /// do servidor e liberando os recursos alocados.
        /// </summary>
        public void Dispose()
        {
            CancelRpcs();
            m_transport?.Close();
            m_transport?.Dispose();
        }


        /// <summary>
        /// Retorna o próximo número de mensagem livre não usado na conexão atual.
        /// </summary>
        /// <returns>O próximo número de mensagem livre não usado na conexão atual.</returns>
        protected int GetNewMsgId()
        { lock (m_lastMsgIdLock) return ++m_lastMsgId; }


        /// <summary>
        /// Retorna o RPC registrado para o número de mensagem informado, ou null caso
        /// não haja um RPC registrado para o número de mensagem.
        /// </summary>
        /// <param name="msgId">Número da mensagem</param>
        /// <returns>RPC registrado para o número da mensagem, ou null caso nenhum registrado.</returns>
        protected IRpc GetRpc(int msgId)
        {
            IRpc rpc = null;
            m_rpcs.TryGetValue(msgId, out rpc);
            return rpc;
        }


        /// <summary>
        /// Retorna o transporte para a conexão atual.
        /// Se ainda não estiver conectado, tenta conectar no servidor.
        /// O estado desconctado é um estado terminal.
        /// Retorna uma excessão caso não consiga conectar no servidor.
        /// </summary>
        /// <param name="forceConnect">Força uma reconexão mesmo que esteja desconectado.</param>
        /// <returns>O transporte atualmente conectado.</returns>
        protected IPacketTransport InternalGetTransport(bool forceConnect)
        {
            if (m_State == ChannelState.Connected) return m_transport;

            lock (m_transportInitializeLock)
            {
                if (m_State == ChannelState.Connected) return m_transport;

                if (!forceConnect)
                {
                    if (m_State != ChannelState.NotConnected)
                    {
                        SocketException sex = new SocketException((int)SocketError.NotConnected);
                        throw sex;
                    }
                }

                m_State = ChannelState.Connecting;

                TcpClient client = new TcpClient();
                IPacketTransport transport = null;

                try
                {
                    client.Connect(m_remoteHostName, m_remotePort);

                    // TODO: Channel: implement multiple transport options
                    transport = new PacketTransport(client);
                    transport.SetReceiveAction(OnPacketReceived);
                    transport.SetDisconnectAction(OnDisconnected);
                    transport.Start();

                    m_transport = transport;
                    m_State = ChannelState.Connected;

                    return m_transport;
                }
                catch (Exception)
                {
                    m_State = ChannelState.NotConnected;

                    client?.Close();
                    transport?.Dispose();

                    throw;
                }
            }
        }


        /// <summary>
        /// Cancela todas as RPCs pendentes para a conexão atual.
        /// </summary>
        protected void CancelRpcs()
        {
            foreach (var rpc in m_rpcs.Values.ToArray())
            {
                rpc.Cancel();
                rpc.Dispose();
            }
            m_rpcs.Clear();
        }


        /// <summary>
        /// Callback de notificação de desconexão do transporte.
        /// Notifica e depois finaliza todas as RPCs pendentes.
        /// </summary>
        /// <param name="sender">Instância geradora da notificação</param>
        /// <param name="reason">Motivo da desconexão</param>
        protected virtual void OnDisconnected(object sender, DisconnectReason reason)
        {
            m_State = ChannelState.Disconnected;

            foreach (var rpc in m_rpcs.Values.ToArray())
            {
                rpc.OnDisconnected(GetSession(), reason);
                rpc.Dispose();
            }
            m_rpcs.Clear();
        }


        /// <summary>
        /// Callback de notificação de pacote recebido.
        /// Localiza o RPC a partir do número da mensagem e transmite o pacote
        /// para o RPC.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="packet"></param>
        protected void OnPacketReceived(object sender, InternalPacket packet)
        {
            int msgId = packet.MsgId;

            IRpc rpc = GetRpc(msgId);
            if (rpc != null)
            {
                rpc.OnPacketReceived(GetSession(), packet);
                return;
            }

            MyDebug.LogDebug("Channel.OnPacketReceived: msgid {0} desconhecido ou inesperado", packet.MsgId);
        }


        /// <summary>
        /// Representa um Wrapper de IPacketSession para o Channel
        /// </summary>
        private class ChannelSessionWrapper : IPacketSession
        {
            private readonly Channel m_channel;

            public ChannelSessionWrapper(Channel channel)
            { m_channel = channel; }

            public int GetNewMsgId()
            { return m_channel.GetNewMsgId(); }

            public void AddRpc(IRpc rpc)
            { m_channel.m_rpcs.Add(rpc.GetMsgId(), rpc); }

            public void RemoveRpc(int msgId)
            { m_channel.m_rpcs.Remove(msgId); }

            public void RemoveRpc(IRpc rpc)
            { m_channel.m_rpcs.Remove(rpc.GetMsgId()); }

            public void Send(InternalPacket packet)
            { m_channel.InternalGetTransport(false).Send(packet); }

            public bool TrySend(InternalPacket packet)
            { return m_channel.InternalGetTransport(false).TrySend(packet); }
        }
    }

    /// <summary>
    /// Estado da conexão ao servidor.
    /// </summary>
    public enum ChannelState
    {
        /// <summary>
        /// Não conectado ao servidor.
        /// </summary>
        NotConnected,

        /// <summary>
        /// Conectando no servidor.
        /// </summary>
        Connecting,

        /// <summary>
        /// Conectado no servidor.
        /// </summary>
        Connected,

        /// <summary>
        /// Conexão finalizada/perdida com o servidor.
        /// </summary>
        Disconnected
    }
}
