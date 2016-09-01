using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using TesteSoftGl.gl;
using TesteSoftGl.util;
using System.Threading;

namespace TesteSoftGl
{
    public class GameLoop
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct NativeMessage
        {
            public IntPtr Handle;
            public uint Message;
            public IntPtr WParameter;
            public IntPtr LParameter;
            public uint Time;
            public Point Location;
        }

        [DllImport("user32.dll")]
        public static extern int PeekMessage(out NativeMessage message, IntPtr window, uint filterMin, uint filterMax, uint remove);



        public Action GameRender;
        public Action GameUpdate;
        public Action GameInfo;
        public bool Enabled;
        public DateTime LastInfo;
        public int LastInfoMilis;
        public int FrameCount;
        public int WinEvents;
        public int FpsTarget = 30;

        public void Init()
        {
            LastInfo = DateTime.Now;
            Application.Idle += new EventHandler(Application_Idle);
        }

        protected bool IsApplicationIdle()
        {
            NativeMessage result;
            return PeekMessage(out result, IntPtr.Zero, (uint)0, (uint)0, (uint)0) == 0;
        }

        public void Application_Idle(object sender, EventArgs e)
        {
            WinEvents++;
            while (Enabled && IsApplicationIdle())
            {
                DateTime agora = DateTime.Now;
                LastInfoMilis = (int)(agora - LastInfo).TotalMilliseconds;
                if (LastInfoMilis > 1000f)
                {
                    if (GameInfo != null) GameInfo();
                    LastInfo = agora;
                    LastInfoMilis = 0;
                    FrameCount = 0;
                    WinEvents = 0;
                }

                FrameCount++;
                if (GameUpdate != null) GameUpdate();
                if (GameRender != null) GameRender();

                if (FpsTarget > 0)
                {
                    // float frameTime = 1000f / FpsTarget;
                    int ndif = (int)((FrameCount * 1000 / FpsTarget) - LastInfoMilis);
                    if (ndif > 1)
                    {
                        if (ndif > 1) ndif--;
                        if (ndif > 100) ndif = 100;
                        // Thread.Sleep((int)ndif);
                        SpinWait.SpinUntil(() => { return (DateTime.Now - agora).TotalMilliseconds >= ndif; }, ndif);
                    }
                }


            }
        }
    }
}
