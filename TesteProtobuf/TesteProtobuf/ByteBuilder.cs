using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesteProtobuf
{
    public class ByteBuilder
    {
        private byte[] m_Buffer;
        private int m_ReadPos;
        private int m_WritePos;

        public ByteBuilder() { }

        public ByteBuilder(ByteBuilder src)
        {
            this.m_Buffer = src.m_Buffer;
            this.m_ReadPos = src.m_ReadPos;
            this.m_WritePos = src.m_WritePos;
        }

        public ByteBuilder(byte[] buffer, int readPos, int writePos)
        {
            this.m_Buffer = buffer;
            this.m_ReadPos = readPos;
            this.m_WritePos = writePos;
        }

        public int Length { get { return m_WritePos - m_ReadPos; } }
        public int Capacity { get { return m_Buffer?.Length ?? 0; } }
        public byte[] Ptr { get { return m_Buffer; } }

        public int ReadPos
        {
            get { return m_ReadPos; }
            set { m_ReadPos = value; }
        }

        public int WritePos
        {
            get { return m_WritePos; }
            set { m_WritePos = value; }
        }

        protected void Resize(int minCapacity)
        {
            // TODO: otimizar Resize()
            if (Capacity >= minCapacity) return;

            int startCapacity = Math.Max(16, Capacity * 2);
            int newCapacity = Math.Max(startCapacity, minCapacity);

            Array.Resize<byte>(ref m_Buffer, newCapacity);
        }

        public void Add(byte[] src, int offset, int size)
        {
            Resize(m_WritePos + size);
            Array.Copy(src, offset, m_Buffer, m_WritePos, size);
            m_WritePos += size;
        }

        public void Add(byte valor)
        {
            Resize(m_WritePos + 1);
            m_Buffer[m_WritePos++] = valor;
        }

        private void Add(int valor)
        {
            Resize(m_WritePos + 4);
            m_Buffer[m_WritePos + 0] = (byte)((valor >> 0) & 0xff);
            m_Buffer[m_WritePos + 1] = (byte)((valor >> 8) & 0xff);
            m_Buffer[m_WritePos + 2] = (byte)((valor >> 16) & 0xff);
            m_Buffer[m_WritePos + 3] = (byte)((valor >> 24) & 0xff);
            m_WritePos += 4;
        }

        public void AddDelimited(IMessage message)
        {
            int messageSize = message.CalculateSize();
            Resize(m_WritePos + 5 + messageSize);
            int varIntSize = ByteUtil.WriteVarInt(m_Buffer, m_WritePos, messageSize);
            m_WritePos += varIntSize;
            message.WriteTo(new ByteStream(this, false, true));
        }

        public int Read(byte[] dst, int offset, int size)
        {
            int adjSize = Math.Min(Length, size);
            if (adjSize > 0)
            {
                Array.Copy(m_Buffer, m_ReadPos, dst, offset, adjSize);
                m_ReadPos += adjSize;
            }
            return adjSize;
        }

        public int ReadByte()
        {
            if (Length == 0) return -1;
            return m_Buffer[m_ReadPos++];
        }

        public ByteStream ReadStream(int size)
        {
            ByteBuilder innerBuilder = new ByteBuilder(m_Buffer, m_ReadPos, m_ReadPos + size);
            ByteStream ret = new ByteStream(innerBuilder, true, false);
            m_ReadPos += size;
            return ret;
        }

        public ByteBuilderLockRange LockWrite(int size)
        {
            Resize(m_WritePos + size);
            ByteBuilderLockRange ret = new ByteBuilderLockRange(m_Buffer, m_WritePos, size);
            return ret;
        }

        public void ConfirmWrite(int size)
        {
            m_WritePos += size;
        }

        public bool TryReadStreamDelimited(ref ByteStream readStream)
        {
            int streamSize, readSize;

            if (Length < 1) return false;

            if (!ByteUtil.TryReadVarint(m_Buffer, m_ReadPos, Length, out readSize, out streamSize)) return false;

            if (Length < (readSize + streamSize)) return false;
            m_ReadPos += readSize;
            readStream = ReadStream(streamSize);
            return true;
        }

        protected bool TryPeekInt32(ref int valor)
        {
            if (Length < 4) return false;
            valor
                = (m_Buffer[m_ReadPos + 0] << 0)
                + (m_Buffer[m_ReadPos + 1] << 8)
                + (m_Buffer[m_ReadPos + 2] << 16)
                + (m_Buffer[m_ReadPos + 3] << 24);
            return true;
        }

        public void Compact()
        {
            if (Length == 0)
            {
                m_Buffer = null;
                m_ReadPos = 0;
                m_WritePos = 0;
            }
        }

        public void CopyTo(Stream dst)
        {
            if (Length > 0)
            {
                dst.Write(m_Buffer, m_ReadPos, Length);
                m_ReadPos = 0;
                m_WritePos = 0;
                m_Buffer = null;
            }
        }
    }



    public class ByteBuilderLockRange
    {
        public byte[] Ptr;
        public int Offset;
        public int Size;

        public ByteBuilderLockRange(byte[] ptr, int offset, int size)
        {
            this.Ptr = ptr;
            this.Offset = offset;
            this.Size = size;
        }
    }

}
