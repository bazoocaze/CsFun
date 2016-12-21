using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Grpc.Extras
{
    internal static class MyDebug
    {
        public const bool BreakOnTodo = false;
        private static Stopwatch m_Inicio = null;


        private static Stopwatch GetStopWatch()
        {
            if (m_Inicio == null)
            {
                m_Inicio = new Stopwatch();
                m_Inicio.Start();
            }
            return m_Inicio;
        }

        private static int GetElapsed()
        {
            var timer = GetStopWatch();
            return (int)(timer.ElapsedMilliseconds);
        }

        public static void LogTodo(string mensagem)
        {
            System.Diagnostics.Debug.WriteLine(String.Format("TODO: {0}", mensagem));

            if (BreakOnTodo && System.Diagnostics.Debugger.IsAttached)
                System.Diagnostics.Debugger.Break();
        }

        public static void LogError(string message, Exception ex)
        {
            string userMsg = String.Format("ERROR: {0} - {1}-{2}", message, ex.GetType().Name, ex.Message);
            string timedMsg = String.Format("[{0}] {1}", GetElapsed(), userMsg);
            System.Diagnostics.Debug.WriteLine(timedMsg);
        }

        public static void LogDebug(string message, params object[] lista)
        {
            string msg = String.Format(message, lista);
            string timedMsg = String.Format("[{0}] DEBUG: {1}", GetElapsed(), msg);
            System.Diagnostics.Debug.WriteLine(timedMsg);
        }
    }
}
