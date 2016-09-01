using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace TesteGrpc
{
    public class DebugFormatter : IFormatter, IFormatterConverter
    {
        public SerializationBinder Binder
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public StreamingContext Context { get; set; }

        public ISurrogateSelector SurrogateSelector
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public object Convert(object value, TypeCode typeCode)
        {
            throw new NotImplementedException();
        }

        public object Convert(object value, Type type)
        {
            throw new NotImplementedException();
        }

        public object Deserialize(Stream serializationStream)
        {
            return null;
        }

        // [SecurityCritical]
        // [SecuritySafeCritical]
        // [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public void Serialize(Stream serializationStream, object graph)
        {
            if (graph == null) throw new ArgumentNullException(nameof(graph));
            Type t = graph.GetType();
            var metodo = t.GetMethod("GetObjectData");
            var serInfo = new SerializationInfo(t, this);
            Context = new StreamingContext(StreamingContextStates.All);
            var ctx = Context;
            metodo.Invoke(graph, new object[] { serInfo, ctx });

            foreach (var x in serInfo)
            {
                Debug.WriteLine(String.Format("{0,-25} | {1,-15} | {2,-20}", x.Name, x.ObjectType.Name, x.Value?.ToString() ?? "<null>"));
            }

        }

        public bool ToBoolean(object value)
        {
            throw new NotImplementedException();
        }

        public byte ToByte(object value)
        {
            throw new NotImplementedException();
        }

        public char ToChar(object value)
        {
            throw new NotImplementedException();
        }

        public DateTime ToDateTime(object value)
        {
            throw new NotImplementedException();
        }

        public decimal ToDecimal(object value)
        {
            throw new NotImplementedException();
        }

        public double ToDouble(object value)
        {
            throw new NotImplementedException();
        }

        public short ToInt16(object value)
        {
            throw new NotImplementedException();
        }

        public int ToInt32(object value)
        {
            throw new NotImplementedException();
        }

        public long ToInt64(object value)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(object value)
        {
            throw new NotImplementedException();
        }

        public float ToSingle(object value)
        {
            throw new NotImplementedException();
        }

        public string ToString(object value)
        {
            throw new NotImplementedException();
        }

        public ushort ToUInt16(object value)
        {
            throw new NotImplementedException();
        }

        public uint ToUInt32(object value)
        {
            throw new NotImplementedException();
        }

        public ulong ToUInt64(object value)
        {
            throw new NotImplementedException();
        }
    }
}
