﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Core
{
    public class CallOptions
    {
        /// <summary>
        /// Creates a new instance of <c>CallOptions</c> struct.
        /// </summary>
        /// <param name="headers">Headers to be sent with the call.</param>
        /// <param name="deadline">Deadline for the call to finish. null means no deadline.</param>
        /// <param name="cancellationToken">Can be used to request cancellation of the call.</param>
        public CallOptions(Metadata headers = null, DateTime? deadline = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            // this.headers = headers;
            // this.deadline = deadline;
            // this.cancellationToken = cancellationToken;
        }
    }
}
