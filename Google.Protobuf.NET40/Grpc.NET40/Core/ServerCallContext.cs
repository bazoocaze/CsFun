using Grpc.Extras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Grpc.Core
{
    public class ServerCallContext
    {
        private TimedCancellationTokenSource m_CancelSource;
        private CancellationToken m_CancelToken;

        public ServerCallContext()
        {
            // TODO: Default cancel timeout for ServerCallContext
            m_CancelSource = new TimedCancellationTokenSource(4000);
            m_CancelToken = m_CancelSource.Token;
        }

        /// <summary>Cancellation token signals when call is cancelled.</summary>
        public CancellationToken CancellationToken
        { get { return this.m_CancelToken; } }
    }
}
