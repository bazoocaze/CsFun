using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core
{
    public class Marshaller<T>
    {
        /// <summary>
        /// Initializes a new marshaller.
        /// </summary>
        /// <param name="serializer">Function that will be used to serialize messages.</param>
        /// <param name="deserializer">Function that will be used to deserialize messages.</param>
        public Marshaller(Func<T, byte[]> serializer, Func<byte[], T> deserializer)
        {
            // this.serializer = GrpcPreconditions.CheckNotNull(serializer, "serializer");
            // this.deserializer = GrpcPreconditions.CheckNotNull(deserializer, "deserializer");
        }

    }

    /// <summary>
    /// Utilities for creating marshallers.
    /// </summary>
    public static class Marshallers
    {
        /// <summary>
        /// Creates a marshaller from specified serializer and deserializer.
        /// </summary>
        public static Marshaller<T> Create<T>(Func<T, byte[]> serializer, Func<byte[], T> deserializer)
        {
            return new Marshaller<T>(serializer, deserializer);
        }

        /// <summary>
        /// Returns a marshaller for <c>string</c> type. This is useful for testing.
        /// </summary>
        public static Marshaller<string> StringMarshaller
        {
            get
            {
                return new Marshaller<string>(System.Text.Encoding.UTF8.GetBytes,
                                              System.Text.Encoding.UTF8.GetString);
            }
        }
    }
}
