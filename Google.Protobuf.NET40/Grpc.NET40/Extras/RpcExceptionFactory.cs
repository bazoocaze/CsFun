using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Grpc.NET40.InternalProtos;

namespace Grpc.Extras
{
    public static class RpcExceptionFactory
    {
        public static RpcException Create(StatusCode statusCode, string details)
        {
            return new RpcException(new Status(statusCode, details));
        }

        internal static RpcException CreateFrom(RpcExceptionProto exception)
        {
            int statusCode = exception.StatusCode;
            string detail = exception.Detail;
            string message = exception.Message;

            if (!Enum.IsDefined(typeof(StatusCode), statusCode))
                statusCode = (int)StatusCode.Unknown;

            Status status = new Status((StatusCode)statusCode, detail);

            if (String.IsNullOrWhiteSpace(message))
                return new RpcException(status);
            else
                return new RpcException(status, message);
        }

        public static RpcException CreateFrom(Exception ex)
        {
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
            }

            if (ex is RpcException) return ex as RpcException;

            StatusCode statusCode = StatusCode.Unknown;

            if (ex is ArgumentException)
                statusCode = StatusCode.Internal;
            else if (ex is IOException)
                statusCode = StatusCode.DataLoss;
            else if (ex is OperationCanceledException)
                statusCode = StatusCode.Cancelled;
            else if (ex is TimeoutException)
                statusCode = StatusCode.DeadlineExceeded;

            return new RpcException(new Status(statusCode, ex.Message));
        }

        // -------------------------------------------
        // ----- SERIALIZATION / DESERIALIZATION -----
        // -------------------------------------------

        public static RpcException Deserialize(byte[] data, int pos, int size)
        {
            if ((data?.Length ?? 0) == 0)
                return new RpcException(new Status(StatusCode.Unknown, "Unknow remote exception"));

            MemoryStream ms = new MemoryStream(data, pos, size);
            BinaryReader br = new BinaryReader(ms);

            int statusCode = br.ReadInt32();
            string statusDetail = br.ReadString();
            string exMessage = br.ReadString();

            if (!Enum.IsDefined(typeof(StatusCode), statusCode))
                statusCode = (int)StatusCode.Unknown;

            Status status = new Status((StatusCode)statusCode, statusDetail);

            return new RpcException(status, exMessage);
        }

        public static byte[] Serialize(RpcException data)
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);

            bw.Write((int)data.Status.StatusCode);
            bw.Write((string)(data.Status.Detail ?? ""));
            bw.Write((string)data.Message ?? "");

            return ms.ToArray();
        }
    }
}
