using Grpc.Core.Internal;
using Grpc.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.Core
{
    public abstract class ClientBase<T> : ClientBase
    {
        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class that
        /// throws <c>NotImplementedException</c> upon invocation of any RPC.
        /// This constructor is only provided to allow creation of test doubles
        /// for client classes (e.g. mocking requires a parameterless constructor).
        /// </summary>
        public ClientBase()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of <c>ClientBase</c> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        protected ClientBase(ClientBaseConfiguration configuration) // : base(configuration)
        {
            throw new NotImplementedException();
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
        /// Creates a new instance of client from given <c>ClientBaseConfiguration</c>.
        /// </summary>
        protected abstract T NewInstance(ClientBaseConfiguration configuration);


        protected internal class ClientBaseConfiguration
        {
        }
    }

    public abstract class ClientBase
    {
        readonly CallInvoker callInvoker;

        protected ClientBase()
        { }

        protected ClientBase(Channel channel) : this(new DefaultCallInvoker(channel))
        { }

        protected ClientBase(CallInvoker callInvoker)
        {
            this.callInvoker = callInvoker;
        }

        /// <summary>
        /// Gets the call invoker.
        /// </summary>
        protected CallInvoker CallInvoker
        {
            get { return this.callInvoker; }
        }
    }
}
