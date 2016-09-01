using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Reflection;
using static Google.Protobuf.WireFormat;

namespace TesteProtobuf
{
    public class DuckerTeste : IMessage
    {
        public int Id;
        public string Nome;

        public MessageDescriptor Descriptor { get { throw new NotImplementedException(); } }

        public int CalculateSize()
        {
            int ret = 0;
            ret += MyCalculateSize(1, FieldType.Int32, this.Id);
            ret += MyCalculateSize(2, FieldType.String, this.Nome);
            return ret;
        }

        public void MergeFrom(CodedInputStream input)
        {
            uint tag;
            while ((tag = input.ReadTag()) != 0)
            {
                if (tag == WireFormat.MakeTag(1, WireFormat.WireType.Varint))
                {
                    this.Id = input.ReadInt32();
                }
                else if (tag == WireFormat.MakeTag(2, WireFormat.WireType.LengthDelimited))
                {
                    this.Nome = input.ReadString();
                }
                else
                {
                    input.SkipLastField();
                }
            }
        }

        public void WriteTo(CodedOutputStream output)
        {
            MyWriteField(output, 1, FieldType.Int32, WireType.Varint, this.Id);
            MyWriteField(output, 2, FieldType.String, WireType.LengthDelimited, this.Nome);
        }

        private static int MyCalculateSize(int fieldNumber, FieldType type, object value)
        {
            int ret = CodedOutputStream.ComputeTagSize(fieldNumber);
            switch (type)
            {
                case FieldType.Int32: ret += CodedOutputStream.ComputeInt32Size((int)value); break;
                case FieldType.String: ret += CodedOutputStream.ComputeStringSize((string)value); break;
                default: throw new NotImplementedException();
            }
            return ret;
        }

        private static void MyWriteField(CodedOutputStream output, int fieldNumber, FieldType fType, WireType wType, object valor)
        {
            output.WriteTag(fieldNumber, wType);
            switch(fType)
            {
                case FieldType.Int32: output.WriteInt32((int)valor); break;
                case FieldType.String: output.WriteString((string)valor); break;
                default: throw new NotImplementedException();
            }
        }

    }
}
