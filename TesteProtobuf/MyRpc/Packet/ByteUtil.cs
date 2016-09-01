using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyRpc.Packet
{
    public class ByteUtil
    {
        public static int ReadInt32(byte[] src, int offset)
        {
            return
                (src[offset + 0] << 0) +
                (src[offset + 1] << 8) +
                (src[offset + 2] << 16) +
                (src[offset + 3] << 24);
        }

        public static bool TryReadVarint(byte[] src, int offset, int maxSize, out int varIntSize, out int retVal)
        {
            retVal = 0;
            varIntSize = 0;
            maxSize = Math.Min(5, maxSize);

            for (int max = 0; max < maxSize; max++)
            {
                byte lido = src[offset + varIntSize];
                retVal |= (lido & 0x7F) << (7 * varIntSize);
                varIntSize++;
                bool bigger = ((lido & 0x80) == 0x80);
                if (!bigger) return true;
            }

            return (maxSize == 5);
        }

        public static int ReadVarint(byte[] src, int offset, out int size)
        {
            int ret = 0;
            size = 0;

            for (int max = 0; max < 5; max++)
            {
                byte lido = src[offset + size];
                ret |= (lido & 0x7F) << (7 * size);
                size++;
                bool bigger = ((lido & 0x80) == 0x80);
                if (!bigger) break;
            }

            return ret;
        }

        public static int WriteVarInt(byte[] dst, int offset, int invalor)
        {
            int desloc = 0;
            int bigger;
            uint valor = (uint)invalor;

            do
            {
                bigger = (((valor & 0xFFFFFF80) == 0) ? 0 : 1);
                if (dst != null) dst[offset + desloc] = (byte)((valor & 0x7F) | (bigger << 7));
                valor = valor >> 7;
                desloc++;
            } while (bigger != 0);

            return desloc;
        }
    }
}
