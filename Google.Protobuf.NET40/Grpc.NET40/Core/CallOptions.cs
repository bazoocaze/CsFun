using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Grpc.Core
{
    public struct CallOptions
    {
        DateTime? deadline;
        CancellationToken cancellationToken;

        /// <summary>
        /// Creates a new instance of <c>CallOptions</c> struct.
        /// </summary>
        /// <param name="headers">Headers to be sent with the call.</param>
        /// <param name="deadline">Deadline for the call to finish. null means no deadline.</param>
        /// <param name="cancellationToken">Can be used to request cancellation of the call.</param>
        /// <param name="writeOptions">Write options that will be used for this call.</param>
        /// <param name="propagationToken">Context propagation token obtained from <see cref="ServerCallContext"/>.</param>
        /// <param name="credentials">Credentials to use for this call.</param>
        public CallOptions(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.deadline = deadline;
            this.cancellationToken = cancellationToken;
        }


        /// <summary>
        /// Call deadline.
        /// </summary>
        public DateTime? Deadline
        {
            get { return deadline; }
        }

        /// <summary>
        /// Token that can be used for cancelling the call on the client side.
        /// Cancelling the token will request cancellation
        /// of the remote call. Best effort will be made to deliver the cancellation
        /// notification to the server and interaction of the call with the server side
        /// will be terminated. Unless the call finishes before the cancellation could
        /// happen (there is an inherent race),
        /// the call will finish with <c>StatusCode.Cancelled</c> status.
        /// </summary>
        public CancellationToken CancellationToken
        {
            get { return cancellationToken; }
        }

    }
}
