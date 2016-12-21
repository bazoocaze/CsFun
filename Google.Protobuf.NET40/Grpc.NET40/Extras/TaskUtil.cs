using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using System.Threading;

namespace Grpc.Extras
{
    public static class TaskUtil
    {
        public static Task CompletedTask()
        {
            var ret = Task.Factory.StartNew(() => { });
            ret.Wait();
            return ret;
        }

        public static Task<TResult> CompletedTask<TResult>(TResult result)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        public static Task<TResult> FailedTask<TResult>(Exception ex)
        {
            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            tcs.SetException(ex);
            return tcs.Task;
        }

        public static TResult GetResult<TResult>(Task<TResult> task, CancellationToken cancelToken)
        {
            task.Wait(cancelToken);
            return task.Result;
        }

        public static TResult GetResult<TResult>(Task<TResult> task, int milliseconds)
        {
            if (!task.Wait(milliseconds))
                throw new OperationCanceledException();
            return task.Result;
        }
    }
}
