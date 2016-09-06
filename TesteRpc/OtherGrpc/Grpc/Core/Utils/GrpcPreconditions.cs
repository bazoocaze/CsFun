using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OtherGrpc.Grpc.Core.Utils
{
    public static class GrpcPreconditions
    {
        public static T CheckNotNull<T>(T reference, string paramName)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(paramName);
            }
            return reference;
        }
    }
}
