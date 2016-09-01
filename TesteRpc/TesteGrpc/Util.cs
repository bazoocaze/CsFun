using Grpc.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteGrpc
{
    public class Util
    {
        public static Task<T> RpcReturnAsync<T, R>(Func<R, ServerCallContext, T> func, R request, ServerCallContext context) where T : class, new()
        {
            return Task.Run(() => RpcReturn(func, request, context));
        }

        public static T RpcReturn<T, R>(Func<R, ServerCallContext, T> func, R request, ServerCallContext context) where T : class, new()
        {
            StatusCode statusCode;
            Exception retEx;

            try
            {
                return func(request, context);
            }
            catch (UnauthorizedAccessException ex)
            {
                retEx = ex;
                statusCode = StatusCode.PermissionDenied;
            }
            catch (DirectoryNotFoundException ex)
            {
                retEx = ex;
                statusCode = StatusCode.NotFound;
            }
            catch (IOException ex)
            {
                retEx = ex;
                statusCode = StatusCode.NotFound;
            }
            context.Status = new Status(statusCode, retEx.Message);
            context.ResponseTrailers.Add("ExceptionClassName", retEx.GetType().Name);
            context.ResponseTrailers.Add("ExceptionMessage", retEx.Message);
            return new T();
        }


    }
}
