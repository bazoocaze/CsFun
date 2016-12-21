using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.Extras
{
    internal static class MethodUtil
    {
        public static Method<T2, T1> Invert<T1, T2>(Method<T1, T2> method)
            where T1 : class
            where T2 : class
        {
            Method<T2, T1> ret = new Method<T2, T1>(method.Type, method.ServiceName, method.Name, method.ResponseMarshaller, method.RequestMarshaller);
            return ret;
        }
    }
}
