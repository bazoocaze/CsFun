using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using TesteProtobuf.Protos;
using static TesteProtobuf.Protos.PersonProtos.Types;
using Google.Protobuf;
using System.Text;
using System.Diagnostics;
using Google.Protobuf.WellKnownTypes;
using Google.Protobuf.Reflection;

namespace TesteProtobuf
{
    public static class Program
    {
        private const string P_ARQUIVO = "c:/temp/teste.dat";

        public static void Teste_Gravar_PB()
        {
            PersonProtos p = new PersonProtos();
            p.Id = 1;
            p.Name = "José";
            p.Email = null;
            p.Phones.Add(new PersonProtos.Types.PhoneNumber() { Number = "5134999107" });
            p.Phones.Add(new PersonProtos.Types.PhoneNumber() { Number = "5134999105" });

            using (var output = File.Create(P_ARQUIVO))
            {
                p.WriteTo(output);
            }

        }

        public static void Teste_Ler_PB()
        {
            PersonProtos ret;

            using (var input = File.OpenRead(P_ARQUIVO))
            {
                ret = PersonProtos.Parser.ParseFrom(input);
            }
        }

        public static void Teste_Gravar_Manual()
        {
            using (var output = File.Create(P_ARQUIVO))
            {
                CodedOutputStream cos = new CodedOutputStream(output);

                cos.WriteTag(1, WireFormat.WireType.LengthDelimited);
                cos.WriteString("Olvebra");

                cos.WriteTag(2, WireFormat.WireType.Varint);
                cos.WriteInt32(42);

                cos.WriteTag(3, WireFormat.WireType.LengthDelimited);
                cos.WriteString("informatica@olvebra.com.br");

                cos.Flush();
            }
        }

        public static uint CreateTag(int fieldNumer, WireFormat.WireType type)
        {
            return (uint)(fieldNumer << 3) | (uint)type;
        }

        public static void Teste_Ler_Manual()
        {
            using (var input = File.OpenRead(P_ARQUIVO))
            {
                CodedInputStream cis = new CodedInputStream(input);

                uint tag;

                while ((tag = cis.ReadTag()) != 0)
                {
                    if (tag == CreateTag(1, WireFormat.WireType.LengthDelimited))
                    {
                        Debug.WriteLine("I={0}  TAG={1}  VAL={2}", 1, tag, cis.ReadString());
                    }
                    else if (tag == CreateTag(2, WireFormat.WireType.Varint))
                    {
                        Debug.WriteLine("I={0}  TAG={1}  VAL={2}", 2, tag, cis.ReadInt32());
                    }
                    else if (tag == CreateTag(3, WireFormat.WireType.LengthDelimited))
                    {
                        Debug.WriteLine("I={0}  TAG={1}  VAL={2}", 3, tag, cis.ReadString());
                    }
                    else
                    {
                        cis.SkipLastField();
                    }
                }
            }
        }

        public static void Teste_Gravar_Manual(IMessage message)
        {
            Debug.WriteLine("Message size to write: {0} bytes", message.CalculateSize());

            using (var output = File.Create(P_ARQUIVO))
            {
                message.WriteTo(output);
            }
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            int[] lista = new int[] { -1000000, -10000, -1000, -1, 0, 1, 127, 128, 255, 256, 1023, 1024, 8191, 8192, 16383, 16384, 1073741824, 1073741823, Int32.MaxValue };

            byte[] varIntTmpbuffer = new byte[16];
            for (int i = 0; i < 4; i++) varIntTmpbuffer[i] = 0xff;
            int rrSize1, rrSize2;
            rrSize1 = ByteUtil.ReadVarint(varIntTmpbuffer, 0, out rrSize2);
            var bb = ByteUtil.TryReadVarint(varIntTmpbuffer, 0, 5, out rrSize1, out rrSize2);

            for (int i = 0; i < lista.Length; i++)
            {
                varIntTmpbuffer = new byte[16];

                int writeSize = ByteUtil.WriteVarInt(varIntTmpbuffer, 0, lista[i]);

                int readSize;
                int readResult = ByteUtil.ReadVarint(varIntTmpbuffer, 0, out readSize);

                int readSize2;
                int readResult2;
                bool boolResult2;

                boolResult2 = ByteUtil.TryReadVarint(varIntTmpbuffer, 0, 15, out readSize2, out readResult2);

                Debug.WriteLine(String.Format("IN={0}  WSIZE={1}   ||   RSIZE={2}  OUT={3}   ||   RSIZE={4}  OUT={5}  RET={6}",
                    lista[i], writeSize,
                    readSize, readResult,
                    readSize2, readResult2, boolResult2));
            }

            using (var output = File.Create(P_ARQUIVO))
            {
                var tmpBuffer = new ByteBuilder();

                AddPacote(tmpBuffer, 1, "Alfa");
                AddPacote(tmpBuffer, 2, "Bravo");
                AddPacote(tmpBuffer, 3, "Charlie");
                AddPacote(tmpBuffer, 4, "Delta");

                tmpBuffer.CopyTo(output);
            }

            using (var input = File.OpenRead(P_ARQUIVO))
            {
                int readSize = 32;
                ByteBuilder readBuffer = new ByteBuilder();
                // ByteStream readStream = new ByteStream(readBuffer);

                while (true)
                {
                    var tmp = readBuffer.LockWrite(readSize);
                    int lidos = input.Read(tmp.Ptr, tmp.Offset, tmp.Size);
                    if (lidos == 0) break;
                    readBuffer.ConfirmWrite(lidos);

                    ByteStream packetStream = null;
                    while (readBuffer.TryReadStreamDelimited(ref packetStream))
                    {
                        var pacote = MeuPacote.Parser.ParseFrom(packetStream);
                        Debug.WriteLine(String.Format("PACOTE: {0}", pacote));
                    }

                    if (packetStream != null) readBuffer.Compact();
                }
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        private static void AddPacote(ByteBuilder buffer, int packetId, string mensagem)
        {
            MeuPacote pacote = new MeuPacote();

            pacote.Header = new MeuCabecalho();
            pacote.Header.Id = packetId;
            pacote.Header.Tipo = 123;

            pacote.Data = new MeusDados();
            pacote.Data.Texto = mensagem;

            buffer.AddDelimited(pacote);
        }

        private static void ShowStringRes(string strId)
        {
            var s = Lang.Strings.ResourceManager.GetString(strId);
            Debug.WriteLine("String[{0}] = {1}", strId, s);
        }
    }
}
