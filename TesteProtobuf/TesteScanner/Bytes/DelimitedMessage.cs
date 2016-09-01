using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteScanner.Bytes;

namespace TesteScanner.Bytes
{
    public class DelimitedMessage
    {
        public static bool TryGetPacket(ByteBuilder input, out ByteStream output)
        {
            output = null;
            return input.TryReadStreamDelimited(ref output);
        }

        public static void WritePacket(IMessage message, Stream output)
        {
            ByteBuilder buffer = new ByteBuilder();
            var outStream = new ByteStream(buffer);
            buffer.AddDelimited(message);
            buffer.CopyTo(output);
        }
    }
}
