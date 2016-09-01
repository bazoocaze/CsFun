using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Dynamic;
using TesteObjetoDinamico.json.scanner;

namespace TesteObjetoDinamico.json
{
    public class JsonDynReader
    {
        public List<JsonScannerToken> Tokens;
        public int TokenIndex;
        public JsonScannerToken LastToken;
        protected JsonScannerToken m_pushToken;

        public JsonDynReader()
        {
            TokenIndex = 0;
            m_pushToken = null;
        }

        public JsonScannerToken GetToken()
        {
            if (m_pushToken != null)
            {
                JsonScannerToken ret = m_pushToken;
                m_pushToken = null;
                return ret;
            }
            LastToken = Tokens[TokenIndex++];
            return LastToken;
        }

        protected void PushToken(JsonScannerToken token)
        {
            m_pushToken = token;
        }

        public object ReadValor()
        {
            JsonScannerToken token = GetToken();

            if (token.Type == eTokenType.Numeric)
                return Convert.ToDecimal(token.Conteudo.ToString());

            if (token.Type == eTokenType.String)
                return token.Conteudo.ToString();

            if (token.Type == eTokenType.AbreChaves)
                return ReadObjeto();

            if (token.Type == eTokenType.AbreColchetes)
                return ReadVetor();

            if (token.Type == eTokenType.Ident)
            {
                string nomeIdent = token.Conteudo.ToString();
                if (nomeIdent == "null") return null;
                if (nomeIdent == "true") return true;
                if (nomeIdent == "false") return false;
            }

            return ReturnError("Valor inválido");
        }

        private object ReadVetor()
        {
            List<object> ret = new List<object>();
            while (true)
            {
                JsonScannerToken token = GetToken();
                if (token.Type == eTokenType.FechaColchetes) return ret.ToArray();
                PushToken(token);
                ret.Add(ReadValor());
                token = GetToken();
                if (token.Type == eTokenType.FechaColchetes) return ret.ToArray();
                if (token.Type == eTokenType.Virgula) continue;
                return ReturnError("Item de vetor inválido");
            }
        }

        private object ReadObjeto()
        {
            dynamic ret = new ExpandoObject();
            IDictionary<string, object> objeto = ((IDictionary<String, Object>)ret);

            while (true)
            {
                JsonScannerToken token = GetToken();
                if (token.Type == eTokenType.FechaChaves) return ret;
                if (token.Type == eTokenType.String)
                {
                    string nome = token.Conteudo.ToString();
                    token = GetToken();
                    if (token.Type != eTokenType.DoisPontos)
                        return ReturnError("Esperado :");
                    object valor = ReadValor();
                    objeto.Add(nome, valor);

                    token = GetToken();
                    if (token.Type == eTokenType.FechaChaves) return ret;
                    if (token.Type == eTokenType.Virgula) continue;
                }
                ReturnError("Par nome/valor inválido");
            }
        }

        private object ReturnError(string msg)
        {
            string mensagem = string.Format("Token:[{0}] Erro:{1}", LastToken.ToString(), msg);
            return new Exception(mensagem);
        }

    }

}
