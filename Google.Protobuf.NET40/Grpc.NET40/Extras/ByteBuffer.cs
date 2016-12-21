using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Grpc.Extras
{
    public class ByteBuffer
    {
        private const int P_MIN_COMPACT_SIZE = 1024;
        private const int P_MAX_BUFFER_SIZE = 1024 * 1024 * 256;
        private const int P_MIN_START_SIZE = 16;

        public byte[] Ptr;
        public int ReadPos;
        public int WritePos;

        public int Length { get { return WritePos - ReadPos; } }
        public int Capacity { get { return Ptr?.Length ?? 0; } }

        public ByteBuffer()
        { }

        private void Resize(int minSize)
        {
            if (WritePos + minSize <= Capacity) return;

            int newSize = Capacity;
            if (newSize < P_MIN_START_SIZE) newSize = P_MIN_START_SIZE;
            
            while (newSize < minSize)
            {
                newSize *= 2;
                if (newSize > P_MAX_BUFFER_SIZE)
                    throw new InvalidOperationException("Tamanho de buffer muito grande.");
            }
            Array.Resize(ref Ptr, newSize);
        }

        public void Compact()
        {
            if (WritePos > P_MIN_COMPACT_SIZE && (WritePos == ReadPos))
            {
                WritePos = 0;
                ReadPos = 0;
                Ptr = null;
            }
        }

        public BytePtr LockWrite(int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));
            
            Resize(WritePos + size);
            return new BytePtr(Ptr, WritePos, size);
        }

        public void ConfirmWrite(int size)
        {
            if (size < 0) throw new ArgumentOutOfRangeException(nameof(size));

            WritePos += size;
        }

        public BytePtr ReadBlock(int size)
        {
            if (Length < size) throw new InvalidOperationException(String.Format("Quantidade insuficiente de dados no buffer ({0}). Esperado {1}, atual {2}.", nameof(ReadBlock), size, Length));
                        
            BytePtr ret = new BytePtr(Ptr, ReadPos, size);
            ReadPos += size;
            return ret;
        }

        public void Clear()
        {
            WritePos = 0;
            ReadPos = 0;
            Ptr = null;
        }

        public void DiscardBytes(int size)
        {
            if(size > Length) throw new InvalidOperationException(String.Format("Quantidade insuficiente de dados no buffer ({0}). Esperado {1}, atual {2}.", nameof(DiscardBytes), size, Length));

            ReadPos += size;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("{0}: ", nameof(ByteBuffer));
            sb.AppendFormat(" Length={0}", this.Length);

            if (ReadPos != 0) sb.AppendFormat(" ReadPos={0}", this.ReadPos);

            if (WritePos != 0) sb.AppendFormat(" WritePos={0}", this.WritePos);

            if (Capacity != 0) sb.AppendFormat(" Capacity={0}", this.Capacity);

            return sb.ToString();
        }
    }

    public class BytePtr
    {
        public byte[] Ptr;
        public int Pos;
        public int Size;

        public BytePtr()
        { }

        public BytePtr(byte[] ptr, int pos, int size)
        {
            this.Ptr = ptr;
            this.Pos = pos;
            this.Size = size;
        }

        public override string ToString()
        {
            return String.Format("{0}: Pos={1} Size={2}", nameof(BytePtr), Pos, Size);
        }
    }
}
