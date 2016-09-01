using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteObjetoDinamico.json.dom
{
    public abstract class JsonNode
    {
        public eNodeType Type { get; protected set; }

        public List<JsonNode> Childs { get; protected set; }

        public JsonNode()
        {
            Childs = new List<JsonNode>();
        }

        protected string SerializeValue(object value)
        {
            if (value == null) return "null";
            if (value is bool) return ((bool)value) ? "true" : "false";
            if (value is string) return EncodeString((string)value);
            if (value is JsonNode) return value.ToString();
            // numeric
            return value.ToString();
        }

        protected string EncodeString(string value)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("\"");
            int size = value.Length;
            for (int i = 0; i < size; i++)
            {
                char c = value[i];
                int n = c;
                string s;
                switch (c)
                {
                    case '\n': s = "\\n"; break;
                    case '\r': s = "\\r"; break;
                    case '\f': s = "\\f"; break;
                    case '\t': s = "\\t"; break;
                    case '\b': s = "\\b"; break;
                    case '\"': s = "\\\""; break;
                    case '\\': s = "\\\\"; break;
                    case '/': s = "\\/"; break;
                    default:
                        if (c < 32)
                        {
                            s = String.Format("{0:X4}", n);
                        }
                        else
                        {
                            sb.Append(c);
                            continue;
                        }
                        break;
                }
                sb.Append(s);
            }
            sb.Append("\"");
            return sb.ToString();
        }
    }

    public enum eNodeType
    {
        Object,
        Element,
        Vector,
        Value
    }
}
