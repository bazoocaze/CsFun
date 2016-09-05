using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core
{
    /// <summary>
    /// Flags for write operations.
    /// </summary>
    [Flags]
    public enum WriteFlags
    {
        /// <summary>
        /// Hint that the write may be buffered and need not go out on the wire immediately.
        /// gRPC is free to buffer the message until the next non-buffered
        /// write, or until write stream completion, but it need not buffer completely or at all.
        /// </summary>
        BufferHint = 0x1,

        /// <summary>
        /// Force compression to be disabled for a particular write.
        /// </summary>
        NoCompress = 0x2
    }

    /// <summary>
    /// Options for write operations.
    /// </summary>
    public class WriteOptions
    {
        /// <summary>
        /// Default write options.
        /// </summary>
        public static readonly WriteOptions Default = new WriteOptions();

        private readonly WriteFlags flags;

        /// <summary>
        /// Initializes a new instance of <c>WriteOptions</c> class.
        /// </summary>
        /// <param name="flags">The write flags.</param>
        public WriteOptions(WriteFlags flags = default(WriteFlags))
        {
            this.flags = flags;
        }

        /// <summary>
        /// Gets the write flags.
        /// </summary>
        public WriteFlags Flags
        {
            get
            {
                return this.flags;
            }
        }
    }
}
