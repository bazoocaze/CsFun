using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grpc.Core
{
    public class ClientBase
    {
        public ClientBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="channel">The channel to use for remote call invocation.</param>
        public ClientBase(Channel channel)
        {
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="callInvoker">The <c>CallInvoker</c> for remote call invocation.</param>
        public ClientBase(CallInvoker callInvoker)
        {
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected ClientBase(ClientBaseConfiguration configuration)
        {
        }

        protected internal class ClientBaseConfiguration
        {
        }

    }

    public abstract class ClientBase<T> : ClientBase
    {
        readonly CallInvoker callInvoker;

        public ClientBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="channel">The channel to use for remote call invocation.</param>
        public ClientBase(Channel channel) : base(channel)
        {
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="callInvoker">The <c>CallInvoker</c> for remote call invocation.</param>
        public ClientBase(CallInvoker callInvoker) : base(callInvoker)
        {
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected ClientBase(ClientBaseConfiguration configuration) : base(configuration)
        {
        }

        /// <summary>
        /// Gets the call invoker.
        /// </summary>
        protected CallInvoker CallInvoker
        {
            get { return this.callInvoker; }
        }

        /// <summary>
        /// Creates a new instance of client from given <c>ClientBaseConfiguration</c>.
        /// </summary>
        protected abstract T NewInstance(ClientBaseConfiguration configuration);

    }
}
