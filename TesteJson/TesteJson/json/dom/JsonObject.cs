using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteObjetoDinamico.json.dom
{
    public class JsonObject : JsonNode, IJsonValue
    {
        protected Dictionary<string, JsonNode> m_items;

        public JsonObject()
        {
            this.Type = eNodeType.Object;
            this.m_items = new Dictionary<string, JsonNode>();
        }

        public JsonElement Element(string name)
        {
            return Item(name) as JsonElement;
        }

        public JsonNode Item(string name)
        {
            JsonNode ret = null;
            if(m_items.TryGetValue(name, out ret)) return ret;
            return null;
        }

        public void Add(string name, object value)
        {
            if (!(value is IJsonElement))
                value = new JsonElement(name, value);
            else
                (value as JsonElement).Name = name;
            Childs.Add(value as JsonNode);
            m_items.Add(name, value as JsonNode);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            sb.Append("{ ");
            foreach (var item in Childs)
            {
                if (!first) sb.Append(", \n");
                else first = false;
                sb.Append(item.ToString());
            }
            sb.Append(" }");
            return sb.ToString();
        }
    }
}
