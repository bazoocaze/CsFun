using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteObjetoDinamico.json.dom
{
    public class JsonElement : JsonNode, IJsonElement
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public JsonElement(string name, object value)
        {
            this.Type = eNodeType.Element;
            this.Name = name;
            this.Value = value;
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", this.EncodeString(Name), this.SerializeValue(Value));
        }
    }
}
