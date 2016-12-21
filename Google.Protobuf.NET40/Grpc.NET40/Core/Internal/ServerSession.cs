using Grpc.Packet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Grpc.Core.Internal;

namespace Grpc.Core.Internal
{
    /// <summary>
    /// Representa uma sessão (de um cliente) no servidor.
    /// </summary>
    internal class ServerSession : IPacketSession
    {
        /// <summary>
        /// Delegate para callback de processamento de novas requisições.
        /// </summary>
        /// <param name="session">Sessão de origem do pacote</param>
        /// <param name="packet">Pacote contendo a requisição</param>
        public delegate void ProcessPacketDelegate(IPacketSession session, InternalPacket packet);

        /// <summary>
        /// Callback a ser executada em caso de recebimento de um pacote que não faça parte
        /// de nehum RPC registrado - tipicamente o caso de novas requisiões.
        /// </summary>
        public ProcessPacketDelegate ProcessPacketAction;


        private int m_lastMsgId;
        private object m_lastMsgIdLock = new object();
        private PacketTransport m_Transport;
        private Dictionary<int, IRpc> m_Rpcs;
        private PacketQueue m_PacketQueue;


        /// <summary>
        /// Construtor padrão para uma nova sessão sobre o transporte informado.
        /// </summary>
        /// <param name="transport">Transporte de comunicação de pacotes.</param>
        public ServerSession(PacketTransport transport)
        {
            this.m_Transport = transport;
            this.m_Rpcs = new Dictionary<int, IRpc>();
            this.m_PacketQueue = new PacketQueue();

            m_PacketQueue.DisconnectedAction = OnDisconnected;
            m_PacketQueue.PacketReceivedAction = OnPacketReceived;
        }


        /// <summary>
        /// Inicia o recebimento/tratamento de requisições.
        /// </summary>
        public void Start()
        {
            this.m_Transport.SetReceiveAction((s, p) => m_PacketQueue.Add(p));
            this.m_Transport.SetDisconnectAction((s, r) => m_PacketQueue.SetDisconnected(r));
            this.m_Transport.Start();
        }


        /// <summary>
        /// Finaliza a sessão, encerrando todas as RPCs pendentes e liberando os recursos alocados.
        /// </summary>
        public void Dispose()
        {
            InternalClose();
        }


        /// <summary>
        /// Finaliza a sessão, encerrando todas as RPCs pendentes e liberando os recursos alocados.
        /// </summary>
        private void InternalClose()
        {
            m_Transport?.Dispose();
            m_Transport = null;

            var rpcs = m_Rpcs;
            m_Rpcs = null;
            if (rpcs != null)
            {
                foreach (var rpc in rpcs.Values.ToArray())
                {
                    rpc.Cancel();
                    rpc.Dispose();
                }
                rpcs.Clear();
            }
        }


        /// <summary>
        /// Retorna o RPC para o número de mensagem, ou null em caso de nenhum RPC registrado
        /// para o número informado (p.ex. nova requisição).
        /// </summary>
        /// <param name="msgId">Número de mensagem</param>
        /// <returns>RPC ativo, ou null caso nenhum ativo para o número de mensagem.</returns>
        private IRpc GetRpc(int msgId)
        {
            IRpc ret = null;
            if (m_Rpcs.TryGetValue(msgId, out ret)) return ret;
            return null;
        }


        /// <summary>
        /// Callback de notificação de desconexão do transporte de rede.
        /// Notifica todos os RPCs pendentes de que houve uma desconexão,
        /// encerrando em seguidas os RPCs e liberando todos os recursos alocados.
        /// </summary>
        /// <param name="reason">Razão da desconexão.</param>
        private void OnDisconnected(DisconnectReason reason)
        {
            var rpcs = m_Rpcs;
            m_Rpcs = null;

            if (rpcs == null) return;

            foreach (var rpc in rpcs.Values.ToArray())
            {
                rpc.OnDisconnected(this, reason);
                rpc.Dispose();
            }

            rpcs.Clear();
        }


        /// <summary>
        /// Callback de notificação de um pacote de rede recebido.
        /// Determina o RPC ativo para o número de mensagem e notifica o 
        /// RPC do recebimento.
        /// Caso não haja um RPC ativo (p.ex. uma nova requisição), repassa
        /// o pacote recebido para o callback de recebimento registrado em
        /// ProcessPacketAction.
        /// </summary>
        /// <param name="packet">Pacote de dados contendo a requisição</param>
        private void OnPacketReceived(InternalPacket packet)
        {
            IRpc rpc = GetRpc(packet.MsgId);
            if (rpc != null)
            {
                rpc.OnPacketReceived(this, packet);
                return;
            }

            if (ProcessPacketAction != null)
            {
                ProcessPacketAction.Invoke(this, packet);
                return;
            }

            // Discard unknow or unexpected packet
        }


        #region IPacketSession Interface


        int IPacketSession.GetNewMsgId()
        {
            lock (m_lastMsgIdLock) { return ++m_lastMsgId; }
        }


        void IPacketSession.Send(InternalPacket packet)
        {
            m_Transport.Send(packet);
        }


        bool IPacketSession.TrySend(InternalPacket packet)
        {
            return m_Transport?.TrySend(packet) ?? false;
        }


        void IPacketSession.AddRpc(IRpc rpc)
        {
            m_Rpcs.Add(rpc.GetMsgId(), rpc);
        }


        void IPacketSession.RemoveRpc(int msgId)
        {
            m_Rpcs?.Remove(msgId);
        }


        void IPacketSession.RemoveRpc(IRpc rpc)
        {
            m_Rpcs?.Remove(rpc.GetMsgId());
        }

        #endregion /* IPacketSession Interface */

    }
}
