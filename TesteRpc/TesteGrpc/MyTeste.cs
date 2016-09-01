using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TesteGrpc
{
    public class MyTeste : ISerializable
    {
        public int Id;
        public string Nome;

        public MyTeste()
        {

        }

        public MyTeste(SerializationInfo info, StreamingContext context)
        {
            this.Id = info.GetInt32("Id");
            this.Nome = info.GetString("Nome");
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", this.Id);
            info.AddValue("Nome", this.Nome);
        }

    }
}
