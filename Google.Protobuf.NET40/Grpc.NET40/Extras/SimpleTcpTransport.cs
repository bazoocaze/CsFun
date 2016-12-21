using Grpc.Core.Internal;
using Grpc.Packet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace Grpc.Extras
{
    /// <summary>
    /// Transporte TCP simples para envio e recebimento de pacotes de bytes.
    /// </summary>
    public class SimpleTcpTransport : IDisposable
    {
        private const int P_MAX_PACKET_SIZE = 256 * 1024;
        private const int P_NETWORK_READ_SIZE = 128;


        /// <summary>
        /// Notificação gerada quando o ponto remoto for desconectado.
        /// </summary>
        public Action<SimpleTcpTransport, DisconnectReason> DisconnectedAction;


        /// <summary>
        /// Conexão TCP corrente.
        /// </summary>
        protected TcpClient m_client;


        /// <summary>
        /// Stream de dados corrente.
        /// </summary>
        protected NetworkStream m_dataStream;


        /// <summary>
        /// Buffer de recebimento parcial dos pacotes.
        /// </summary>
        protected ByteBuffer m_receiveBuffer;


        /// <summary>
        /// Bloqueio para envio atômico de pacotes.
        /// </summary>
        protected object m_lockSend = new object();


        /// <summary>
        /// Indica se a instância foi finalizada (Dispose).
        /// </summary>
        protected bool m_disposed;


        /// <summary>
        /// Indica se o transporte está desconectado.
        /// </summary>
        private bool m_disconnected;


        /// <summary>
        /// Indica se o transporte está configurado para receber dados.
        /// </summary>
        private bool m_started;


        /// <summary>
        /// Cria um transporte usando a conexão inforamda.
        /// </summary>
        /// <param name="client">Conexão de rede do transporte.</param>
        public SimpleTcpTransport(TcpClient client)
        {
            this.m_client = client;
            this.m_dataStream = client.GetStream();
            this.m_receiveBuffer = new ByteBuffer();
        }


        /// <summary>
        /// Retorna true/false se o transporte está conectado.
        /// </summary>
        public bool IsConnected
        { get { return (!m_disconnected) && (m_client?.Connected ?? false); } }


        /// <summary>
        /// Retorna true/false se o transporte está aguardando por dados remotos.
        /// </summary>
        public bool IsReceiving
        { get { return m_started && IsConnected; } }


        /// <summary>
        /// Prepara o transporte para receber dados remotos.
        /// </summary>
        public void Start()
        {
            if (m_disposed || m_disconnected) throw new ObjectDisposedException(nameof(PacketTransport));

            if (m_started) return;
            m_started = true;
            RegisterWaitData();
        }


        /// <summary>
        /// Encerra a conexão do transporte.
        /// </summary>
        public void Close()
        {
            m_disconnected = true;
            m_dataStream?.Close();
            m_dataStream?.Dispose();
            m_client?.Close();
        }

        /// <summary>
        /// Finaliza a instância, liberando todos os recursos alocados.
        /// </summary>
        public void Dispose()
        {
            if (m_disposed) return;
            m_disposed = true;
            Close();
            m_client = null;
            m_dataStream = null;
            m_receiveBuffer = null;
        }


        /// <summary>
        /// Prepara para o recebimento assíncrono de dados remotos.
        /// </summary>
        private void RegisterWaitData()
        {
            if (!IsConnected) return;

            var stream = m_dataStream;
            try
            {
                var tmp = m_receiveBuffer.LockWrite(P_NETWORK_READ_SIZE);
                stream?.BeginRead(tmp.Ptr, tmp.Pos, tmp.Size, DataReceivedCallback, stream);
            }
            catch (ObjectDisposedException) { }
            catch (IOException ex) { OnDisconnected(new DisconnectReason(ex)); return; }
        }


        /// <summary>
        /// Callback de recebimento assíncrono de dados remotos.
        /// </summary>
        /// <param name="ar">AsyncResult com os dados da chamada assíncrona</param>
        private void DataReceivedCallback(IAsyncResult ar)
        {
            NetworkStream stream = (NetworkStream)ar.AsyncState;
            int lidos = 0;
            try
            {
                lidos = stream.EndRead(ar);
            }
            catch (ObjectDisposedException) { return; }
            catch (IOException ex) { OnDisconnected(new DisconnectReason(ex)); return; }

            // Verifica se recebeu EOS (fim do stream/desconectado)
            if (lidos == 0)
            {
                OnDisconnected(new DisconnectReason());
                return;
            }

            // Confirma a gravação dos dados lidos
            m_receiveBuffer.ConfirmWrite(lidos);

            // Pode haver mais de um pacote no buffer -> processa todos os pacotes
            while (OnDataReceived(m_receiveBuffer)) { }

            // Possivelmente limpa o buffer
            m_receiveBuffer.Compact();

            // Prepara para receber novo pacote
            RegisterWaitData();
        }


        /// <summary>
        /// Callback de processamento dos dados assíncronos recebidos.
        /// </summary>
        /// <param name="receiveBuffer">Buffer contendo os dados recebidos até agora.</param>
        /// <returns>True/false caso tenha recebido um pacote completo com sucesso.</returns>
        private bool OnDataReceived(ByteBuffer receiveBuffer)
        {
            // Cabeçalho formado por um int32 (4 bytes) indicando o tamanho dos dados do pacote
            int packetHeaderSize = 4;

            // Verifica se possui o cabeçalho completo no buffer
            if (receiveBuffer.Length <= packetHeaderSize) return false;

            // Lê o cabeçalho -> tamanho dos dados do pacote
            int packetDataSize = BitConverter.ToInt32(receiveBuffer.Ptr, receiveBuffer.ReadPos);

            // Valida o tamanho dos dados do pacote
            if (packetDataSize <= 0 || packetDataSize > P_MAX_PACKET_SIZE)
            {
                // Tamanho inválido
                OnInvalidDataReceived();
                receiveBuffer.Clear();
                return false;
            }

            // Verifica se possui o pacote completo no buffer
            if (receiveBuffer.Length < packetHeaderSize + packetDataSize) return false;

            // Descarta o cabeçalho já lido e lê o bloco dos dados
            receiveBuffer.DiscardBytes(packetHeaderSize);
            var packetData = receiveBuffer.ReadBlock(packetDataSize);
            OnDataReceived(packetData);

            // Notifica que recebeu um pacote completo com sucesso
            return true;
        }


        protected virtual void OnDataReceived(BytePtr packetData)
        {
        }


        /// <summary>
        /// Callback de processamento de dados inválidos.
        /// </summary>
        protected void OnInvalidDataReceived()
        {
        }


        /// <summary>
        /// Callback de processamento de desconexão.
        /// </summary>
        private void OnDisconnected(DisconnectReason reason)
        {
            if (m_disconnected || m_disposed) return;
            Close();

            try
            {
                DisconnectedAction?.Invoke(this, reason);
            }
            catch (Exception ex)
            {
                MyDebug.LogError("SimpleTcpTransport.DisconnectedAction", ex);
            }
        }

    }


    /// <summary>
    /// Transporte TCP simples para envio e recebimento de pacotes complexos.
    /// Os pacotes request e response são serializados e deserializados por demanda usando os
    /// serializers e deserializers informados.
    /// </summary>
    public class SimpleTcpTransport<TRequest, TResponse> : SimpleTcpTransport
        where TRequest : class
        where TResponse : class
    {
        public delegate TRequest RequestDeserializerDelegate(byte[] data, int pos, int size);
        public delegate byte[] ResponseSerializerDelegate(TResponse data);


        /// <summary>
        /// Callback de deserialização de requests.
        /// </summary>
        public RequestDeserializerDelegate RequestDeserializer;


        /// <summary>
        /// Callback de serialização de responses.
        /// </summary>
        public ResponseSerializerDelegate ResponseSerializer;


        /// <summary>
        /// Notificação gerada quando o transporte receber um pacote.
        /// </summary>
        public Action<object, TRequest> PacketReceivedAction;


        /// <summary>
        /// Construtor padrão para o transporte simples.
        /// </summary>
        /// <param name="client">Conexão TCP para o transporte</param>
        public SimpleTcpTransport(TcpClient client) : base(client) { }


        /// <summary>
        /// Envia o pacote para o host remoto.
        /// Gera uma Exception em caso de erro:
        /// System.IOException: em caso de erro de comunicação.
        /// System.ObjectDisposedException: caso a instância já tenha sido finalizada (Dispose).
        /// </summary>
        /// <param name="packet">Pacote a ser enviado</param>
        public void Send(TResponse packet)
        {
            var stream = m_dataStream;

            if (stream == null || m_disposed) throw new ObjectDisposedException(nameof(PacketTransport));

            var data = ResponseSerializer(packet);
            lock (m_lockSend)
            {
                // Debug.WriteLine(String.Format("Network.Send {0} bytes", data.Length));

                BinaryWriter bw = new BinaryWriter(stream);
                bw.Write((int)data.Length);
                bw.Write(data);
                bw.Flush();
            }
        }


        /// <summary>
        /// Envia o pacote para o host remoto, ignorando erros silenciosamente.
        /// Em caso de erro, o pacote é descartado silenciosamente.
        /// Retorna true/false caso tenha conseguido despachar o pacote sem erros.
        /// </summary>
        /// <param name="packet">Pacote a ser enviado</param>
        /// <returns>True/false caso tenha conseguido despachar o pacote sem erros.</returns>
        public bool TrySend(TResponse packet)
        {
            var stream = m_dataStream;

            if (stream == null || !IsConnected) return false;

            try
            {
                var data = ResponseSerializer(packet);

                // Debug.WriteLine(String.Format("Network.Send {0} bytes", data.Length));

                lock (m_lockSend)
                {
                    BinaryWriter bw = new BinaryWriter(stream);
                    bw.Write((int)data.Length);
                    bw.Write(data);
                    bw.Flush();

                    return true;
                }
            }
            catch (Exception ex)
            {
                MyDebug.LogError("SimpleTcpTransport.TrySend", ex);
                return false;
            }
        }


        /// <summary>
        /// Callback de processamento do pacote de bytes recebido.
        /// A implementação padrão deserializa o pacote de dados em um objeto request
        /// e então dispara a notificação PacketReceivedAction passando o objeto request.
        /// </summary>
        /// <param name="packetData">Pacote de bytes</param>
        protected override void OnDataReceived(BytePtr packetData)
        {
            TRequest packet = null;

            try
            {
                // Deserializa os dados do pacote
                packet = RequestDeserializer(packetData.Ptr, packetData.Pos, packetData.Size);

                PacketReceivedAction?.Invoke(this, packet);
            }
            catch (Exception ex)
            {
                MyDebug.LogError("SimpleTcpTransport.OnDataReceived", ex);
            }
        }

    }
}
