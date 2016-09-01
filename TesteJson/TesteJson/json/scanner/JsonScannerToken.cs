using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteObjetoDinamico.json.scanner
{
    public class JsonScannerToken
    {
        public eTokenType Type;
        public StringBuilder Conteudo;
        public string Mensagem;
        public int NumeroLinha;
        public int NumeroColuna;

        public JsonScannerToken()
        {
            Conteudo = new StringBuilder();
        }

        public override string ToString()
        {
            return String.Format("Tipo:{0}-{1} Conteúdo:[{2}]", (int)Type, Type, Conteudo);
        }

        public static JsonScannerToken New(eTokenType type)
        {
            JsonScannerToken ret = new JsonScannerToken();
            ret.Type = type;
            return ret;
        }
    }

}
