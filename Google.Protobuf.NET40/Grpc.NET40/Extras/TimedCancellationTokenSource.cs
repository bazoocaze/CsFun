using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Grpc.Extras
{
    public class TimedCancellationTokenSource : IDisposable
    {
        protected CancellationTokenSource m_Cts;
        protected Timer m_Timer;

        public TimedCancellationTokenSource()
        {
            m_Cts = new CancellationTokenSource();
        }

        public TimedCancellationTokenSource(int milliseconds) : this()
        {
            CancelAfter(milliseconds);
        }

        public CancellationToken Token
        {
            get { return m_Cts?.Token ?? CancellationToken.None; }
        }

        public void Cancel(bool throwOnFirstException = false)
        {
            try
            {
                m_Cts?.Cancel(throwOnFirstException);
            }
            finally
            {
                Finish();
            }
        }

        private void Finish()
        {
            m_Timer?.Dispose();
            m_Timer = null;
            m_Cts?.Dispose();
            m_Cts = null;
        }

        public void CancelAfter(TimeSpan span)
        {
            TimeSpan disablePeriodic = TimeSpan.FromMilliseconds(-1);

            if (m_Timer == null)
                m_Timer = new Timer(TimedCancelCallback, null, span, disablePeriodic);
            else
                m_Timer.Change(span, disablePeriodic);
        }

        public void CancelAfter(int milliseconds)
        {
            if (milliseconds < 1) milliseconds = 1;
            CancelAfter(TimeSpan.FromMilliseconds(milliseconds));
        }

        private void TimedCancelCallback(object state)
        {
            try
            {
                Cancel();
            }
            catch (Exception) { }
        }

        public void Dispose()
        {
            Finish();
        }
    }
}
