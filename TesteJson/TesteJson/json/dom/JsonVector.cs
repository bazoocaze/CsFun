using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteObjetoDinamico.json.dom
{
    public class JsonVector : JsonNode, IJsonValue
    {
        public JsonVector()
        {
            this.Type = eNodeType.Vector;
        }

        public void Add(object value)
        {
            if (value is IJsonValue)
            {
                Childs.Add(value as JsonNode);
                return;
            }
            Childs.Add(new JsonValue(value));
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            sb.Append("[ ");
            foreach (var item in Childs)
            {
                if (!first) sb.Append(", ");
                else first = false;
                sb.Append(this.SerializeValue(item));
            }
            sb.Append(" ]");
            return sb.ToString();
        }
    }
}
