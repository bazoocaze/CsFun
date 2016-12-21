using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Grpc.Extras
{
    internal class AsyncQueue<TData> : IDisposable
    {
        private object m_dataLock = new object();
        private Queue<TData> m_data;
        private Queue<TaskCompletionSource<TData>> m_readMessageTcs;

        public AsyncQueue()
        {
            m_data = new Queue<TData>();
            m_readMessageTcs = new Queue<TaskCompletionSource<TData>>();
        }

        public void Add(TData data)
        {
            lock (m_dataLock)
            {
                if (m_data == null || m_readMessageTcs == null) throw new ObjectDisposedException("AsyncQueue");

                if (m_readMessageTcs?.Count > 0)
                {
                    var readMessageTcs = m_readMessageTcs.Dequeue();
                    readMessageTcs.SetResult(data);
                    return;
                }
                if (m_data == null) m_data = new Queue<TData>();
                m_data.Enqueue(data);
            }
        }

        public Task<TData> ReadMessageAsync()
        {
            lock (m_dataLock)
            {
                if (m_data == null || m_readMessageTcs == null) throw new ObjectDisposedException("AsyncQueue");

                TaskCompletionSource<TData> tcs = new TaskCompletionSource<TData>();
                if (m_data?.Count > 0)
                    tcs.SetResult(m_data.Dequeue());
                else
                    m_readMessageTcs.Enqueue(tcs);
                return tcs.Task;
            }
        }

        public TData ReadMessage()
        {
            var data = ReadMessageAsync();
            return data.Result;
        }

        public TData ReadMessage(int millisecondsTimeout)
        {
            var data = ReadMessageAsync();
            if (!data.Wait(millisecondsTimeout)) throw new TimeoutException();
            return data.Result;
        }

        public TData ReadMessage(CancellationToken cancellationToken)
        {
            var data = ReadMessageAsync();
            data.Wait(cancellationToken);
            return data.Result;
        }

        public TData ReadMessage(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            var data = ReadMessageAsync();
            if (!data.Wait(millisecondsTimeout, cancellationToken)) throw new TimeoutException();
            return data.Result;
        }

        public void Dispose()
        {
            lock (m_dataLock)
            {
                CancelAll();
                m_readMessageTcs = null;
                m_data?.Clear();
                m_data = null;
            }
        }

        public void CancelAll()
        {
            var tcs = m_readMessageTcs;
            if (tcs != null)
            {
                foreach (var item in tcs) item.TrySetCanceled();
                tcs.Clear();
            }
        }

        public void ReleaseAll(TData data)
        {
            var tcs = m_readMessageTcs;
            if (tcs != null)
            {
                foreach (var item in tcs.ToArray()) item.TrySetResult(data);
                tcs.Clear();
            }
        }

        public void AbortAll(Exception exception)
        {
            var tcs = m_readMessageTcs;
            if (tcs != null)
            {
                foreach (var item in tcs.ToArray()) item.TrySetException(exception);
                tcs.Clear();
            }
        }

    }
}
