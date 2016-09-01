using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRpc.Packet
{
    public class ByteStream : Stream
    {
        private ByteBuilder m_Builder;

        private bool m_canRead;
        private bool m_canWrite;

        public ByteStream(ByteBuilder baseBuilder)
        {
            m_Builder = baseBuilder;
            m_canRead = true;
            m_canWrite = true;
        }

        public ByteStream(ByteBuilder baseBuilder, bool canRead, bool canWrite)
        {
            m_Builder = baseBuilder;
            m_canRead = canRead;
            m_canWrite = canWrite;
        }

        public override bool CanRead { get { return m_canRead; } }

        public override bool CanSeek { get { return m_canRead != m_canWrite; } }

        public override bool CanWrite { get { return m_canWrite; } }

        public override long Length { get { return m_Builder.Length; } }

        public override long Position
        {
            get { return InternalPosition; }
            set { ValidateCanSeek(); InternalPosition = (int)value; }
        }

        public override void Flush() { }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return m_Builder.Read(buffer, offset, count);
        }

        public override int ReadByte()
        {
            return m_Builder.ReadByte();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            ValidateCanSeek();
            return InternalSeek((int)offset, origin);
        }

        private void ValidateCanSeek()
        {
            if (!CanSeek) throw new InvalidOperationException("CanSeek não disponível");
        }

        private int InternalSeek(int offset, SeekOrigin origin)
        {
            int posBegin = (CanRead ? 0 : m_Builder.ReadPos);
            int posEnd = (CanRead ? m_Builder.WritePos : m_Builder.Capacity);
            int curPos = InternalPosition;

            int desloc;

            switch (origin)
            {
                case SeekOrigin.Begin: desloc = posBegin; break;
                case SeekOrigin.Current: desloc = InternalPosition; break;
                case SeekOrigin.End: desloc = posEnd; break;
                default: throw new ArgumentException("Parâmetro inválido", nameof(origin));
            }

            int targetPos = desloc + offset;

            InternalPosition = Math.Max(posBegin, Math.Min(posEnd, targetPos));

            return InternalPosition;
        }

        private int InternalPosition
        {
            get
            {
                if (CanRead) return m_Builder.ReadPos;
                else return m_Builder.WritePos;
            }

            set
            {
                if (CanRead) m_Builder.ReadPos = (int)value;
                else m_Builder.WritePos = (int)value;
            }
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            m_Builder.Add(buffer, offset, count);
        }

        public override void WriteByte(byte value)
        {
            m_Builder.Add((byte)value);
        }
    }
}
