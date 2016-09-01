using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteObjetoDinamico.json.dom
{
    public class JsonValue : JsonNode, IJsonValue
    {
        public object Value;

        public JsonValue()
        {
            this.Type = eNodeType.Value;
        }

        public JsonValue(object value)
        {
            this.Type = eNodeType.Value;
            this.Value = value;
        }

        public override string ToString()
        {
            return this.SerializeValue(Value);
        }
    }
}
