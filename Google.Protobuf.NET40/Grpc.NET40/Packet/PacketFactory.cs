using Grpc.Core;
using Grpc.Core.Utils;
using Grpc.Extras;
using Grpc.NET40.InternalProtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Packet
{
    public static class PacketFactory
    {
        private const int P_DEFAULT_OPTIONS = 0;


        // ----------------------------------------
        // ----- EMPTY PAYLOAD / EVENT PACKET -----
        // ----------------------------------------

        public static InternalPacket NewPacket(int msgId, int options = P_DEFAULT_OPTIONS)
        {
            return NewInternalPacket(msgId, options);
        }

        public static InternalPacket NewPacket(int msgId, string fullName)
        {
            GrpcPreconditions.CheckNotNull(fullName, nameof(fullName));

            return NewInternalPacket(msgId, P_DEFAULT_OPTIONS, fullName);
        }


        // ---------------------------
        // ----- PAYLOAD IS DATA -----
        // --------------------------- 

        public static InternalPacket NewPacket<TSend>(int msgId, Marshaller<TSend> marshaller, TSend data)
        {
            GrpcPreconditions.CheckNotNull(marshaller, nameof(marshaller));
            GrpcPreconditions.CheckNotNull(data, nameof(data));

            return NewInternalPacket(msgId, P_DEFAULT_OPTIONS, null, marshaller, data);
        }

        public static InternalPacket NewPacket<TSend>(int msgId, int options, Marshaller<TSend> marshaller, TSend data)
        {
            GrpcPreconditions.CheckNotNull(marshaller, nameof(marshaller));
            GrpcPreconditions.CheckNotNull(data, nameof(data));

            return NewInternalPacket(msgId, options, null, marshaller, data);
        }

        public static InternalPacket NewPacket<TSend>(int msgId, string fullName, Marshaller<TSend> marshaller, TSend data)
        {
            GrpcPreconditions.CheckNotNull(fullName, nameof(fullName));
            GrpcPreconditions.CheckNotNull(marshaller, nameof(marshaller));
            GrpcPreconditions.CheckNotNull(data, nameof(data));

            return NewInternalPacket(msgId, P_DEFAULT_OPTIONS, fullName, marshaller, data);
        }

        public static InternalPacket NewPacket<TSend>(int msgId, int options, string fullName, Marshaller<TSend> marshaller, TSend data)
        {
            GrpcPreconditions.CheckNotNull(fullName, nameof(fullName));
            GrpcPreconditions.CheckNotNull(marshaller, nameof(marshaller));
            GrpcPreconditions.CheckNotNull(data, nameof(data));

            return NewInternalPacket(msgId, options, fullName, marshaller, data);
        }


        // ---------------------------------
        // ----- PAYLOAD IS TASKRESULT -----
        // ---------------------------------

        public static Task<InternalPacket> NewPacket<TSend>(int msgId, Marshaller<TSend> marshaller, Task<TSend> taskResult)
        {
            GrpcPreconditions.CheckNotNull(marshaller, nameof(marshaller));
            GrpcPreconditions.CheckNotNull(taskResult, nameof(taskResult));

            return taskResult.ContinueWith((tResult) =>
            {
                if (taskResult.IsFaulted)
                {
                    var rpcException = RpcExceptionFactory.CreateFrom(taskResult.Exception);
                    return NewInternalPacket(msgId, P_DEFAULT_OPTIONS, null, rpcException);
                }

                if (taskResult.IsCanceled)
                    return null;

                return NewInternalPacket(msgId, P_DEFAULT_OPTIONS, null, marshaller, taskResult.Result);
            });
        }


        // --------------------------------
        // ----- PAYLOAD IS EXCEPTION -----
        // -------------------------------- 

        public static InternalPacket NewPacket(int msgId, RpcException exception)
        {
            GrpcPreconditions.CheckNotNull(exception, nameof(exception));

            return NewInternalPacket(msgId, P_DEFAULT_OPTIONS, null, exception);
        }

        public static InternalPacket NewPacket(int msgId, int options, RpcException exception)
        {
            GrpcPreconditions.CheckNotNull(exception, nameof(exception));

            return NewInternalPacket(msgId, options, null, exception);
        }

        public static InternalPacket NewPacket(int msgId, int options, string fullName, RpcException exception)
        {
            GrpcPreconditions.CheckNotNull(fullName, nameof(fullName));
            GrpcPreconditions.CheckNotNull(exception, nameof(exception));

            return NewInternalPacket(msgId, options, fullName, exception);
        }


        // -------------------------------------------
        // ----- SERIALIZATION / DESERIALIZATION -----
        // -------------------------------------------

        public static InternalPacket Deserialize(byte[] data, int pos, int size)
        {
            return InternalPacket.InternalFactory.Deserialize(data, pos, size);
        }

        public static byte[] Serialize(InternalPacket data)
        {
            return InternalPacket.InternalFactory.Serialize(data);
        }



        // ---------------------
        // ----- INTERNALS -----
        // ---------------------

        private static InternalPacket NewInternalPacket<TSend>(int msgId, int options, string fullName, Marshaller<TSend> marshaller, TSend request)
        {
            InternalPacket packet = InternalPacket.Create();
            packet.MsgId = msgId;
            packet.FullMethodName = fullName ?? "";
            packet.Options = options;
            packet.Payload = marshaller.Serializer(request);
            return packet;
        }

        private static InternalPacket NewInternalPacket(int msgId, int options, string fullName = null, RpcException exception = null)
        {
            InternalPacket packet = InternalPacket.Create();
            packet.MsgId = msgId;
            packet.FullMethodName = fullName ?? "";
            packet.Options = options;

            if (exception != null)
                packet.Exception = exception;

            return packet;
        }

    }

}
