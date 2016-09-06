using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using System.IO;

namespace Grpc.Core
{
    public class Channel
    {
        private string m_HostName;
        private int m_Port;

        private Stream m_OutputStream;
        private Stream m_InputStream;
        private BinaryWriter m_OutputWriter;
        private BinaryReader m_InputReader;

        public Channel(string hostName, int port)
        {
            this.m_HostName = hostName;
            this.m_Port = port;
        }

        internal void Send(IMessage envelope)
        {
            m_OutputWriter.Write((int)envelope.CalculateSize());
            m_OutputWriter.Write(envelope.ToByteArray());
            m_OutputWriter.Flush();
        }

        protected void RegisterWaitData()
        {
        }

    }
}
