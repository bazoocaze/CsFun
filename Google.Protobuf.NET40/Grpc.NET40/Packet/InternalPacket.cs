using Google.Protobuf;
using Grpc.Core;
using Grpc.Core.Utils;
using Grpc.Extras;
using Grpc.NET40.InternalProtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Packet
{
    public class InternalPacket
    {
        internal static InternalPacket Create()
        {
            return new InternalPacket();
        }


        public string FullMethodName;
        public int MsgId;
        public int Options;
        public byte[] Payload;
        public RpcException Exception;

        private InternalPacket()
        {
        }

        private InternalPacket(int msgId)
        {
            this.MsgId = msgId;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Id={0}", MsgId);

            if (Options != 0)
                sb.AppendFormat(" Options=0x{0:x}", Options);

            /* 
            if (PayloadSize != 0 && PayloadSize != Payload?.Length) 
                sb.AppendFormat(" PayloadSize={0}", PayloadSize); */

            if (Payload?.Length > 0)
                sb.AppendFormat(" Payload.Length={0}", Payload.Length);

            if (!String.IsNullOrWhiteSpace(FullMethodName))
                sb.AppendFormat(" Path=\"{0}\"", FullMethodName);

            return sb.ToString();
        }

        internal static class InternalFactory
        {
            public static InternalPacket Deserialize(byte[] data, int pos, int size)
            {                
                return ProtoDeserialize(data, pos, size);
            }

            public static byte[] Serialize(InternalPacket data)
            {             
                return ProtoSerialize(data);
            }

            public static InternalPacket ProtoDeserialize(byte[] data, int pos, int size)
            {
                InternalPacket ret = new InternalPacket();
                PacketProto protoPacket = new PacketProto();

                MemoryStream ms = new MemoryStream(data, pos, size);                
                protoPacket.MergeFrom(ms);

                ret.MsgId = protoPacket.MsgId;
                ret.FullMethodName = protoPacket.FullMethodName;
                ret.Options = protoPacket.Options;

                if(protoPacket.DataTypeCase == PacketProto.DataTypeOneofCase.Payload)
                {
                    ret.Payload = protoPacket.Payload.Data.ToByteArray();
                    int payloadSize = protoPacket.Payload.Size;

                    if (payloadSize < 0 || payloadSize > ms.Length || ret.Payload.Length != payloadSize)
                        throw new InvalidDataException("Pacote inválido: tamanho de payload inválido");
                }

                if(protoPacket.DataTypeCase == PacketProto.DataTypeOneofCase.Exception)
                {
                    ret.Exception = RpcExceptionFactory.CreateFrom(protoPacket.Exception);
                }

                return ret;
            }

            public static byte [] ProtoSerialize(InternalPacket data)
            {
                PacketProto packet = new PacketProto();
                packet.FullMethodName = data.FullMethodName ?? "";
                packet.MsgId = data.MsgId;
                packet.Options = data.Options;

                if (data.Payload != null)
                {
                    packet.Payload = new PayloadProto();
                    packet.Payload.Size = data.Payload.Length;
                    packet.Payload.Data = ByteString.CopyFrom(data.Payload);                
                }

                if(data.Exception != null)
                {
                    packet.Exception = new RpcExceptionProto();
                    packet.Exception.StatusCode = (int)data.Exception.Status.StatusCode;
                    packet.Exception.Detail = data.Exception.Status.Detail;

                    if (data.Exception.Message == data.Exception.Status.ToString())
                        packet.Exception.Message = "";
                    else
                        packet.Exception.Message = data.Exception.Message;
                }

                return packet.ToByteArray();
            }
        }
    }


}
