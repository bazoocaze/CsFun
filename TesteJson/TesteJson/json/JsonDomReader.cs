using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using TesteObjetoDinamico.json.scanner;
using TesteObjetoDinamico.json.dom;
using System.IO;

namespace TesteObjetoDinamico.json
{
    public class JsonDomReader : IDisposable
    {
        protected JsonScannerToken LastToken;
        protected JsonScannerToken m_pushToken;

        protected JsonParser m_Parser;
        protected JsonScanner m_Scanner;

        public JsonDomReader()
        {
            Init();
        }


        public object ReadValue()
        {
            Prepare();
            m_Parser.ParseValue();
            m_Scanner.Rewind();
            return InternalReadValue();
        }

        public JsonObject ReadObject()
        {
            Prepare();
            m_Parser.ParseObject();
            m_Scanner.Rewind();
            return InternalReadObject();
        }


        private void Prepare()
        {
            if (m_Parser == null) m_Parser = new JsonParser(m_Scanner);
            m_Scanner.Rewind();
            m_Parser.ThrowError = true;
        }
        

        private void Init()
        {
            m_pushToken = null;
        }


        private void InitParsing()
        {
            m_Parser = new JsonParser(m_Scanner);
        }

        private JsonScannerToken GetToken()
        {
            if (m_pushToken != null)
            {
                JsonScannerToken ret = m_pushToken;
                m_pushToken = null;
                return ret;
            }
            return m_Scanner.GetToken();
        }

        protected void PushToken(JsonScannerToken token)
        {
            m_pushToken = token;
        }

        protected object InternalReadValue()
        {
            JsonScannerToken token = GetToken();

            if (token.Type == eTokenType.Numeric)
                return Convert.ToDecimal(token.Conteudo.ToString());

            if (token.Type == eTokenType.String)
                return token.Conteudo.ToString();

            if (token.Type == eTokenType.AbreChaves)
                return InternalReadObject();

            if (token.Type == eTokenType.AbreColchetes)
                return ReadVector();

            if (token.Type == eTokenType.Ident)
            {
                string identValue = token.Conteudo.ToString();
                if (identValue == "null") return null;
                if (identValue == "true") return true;
                if (identValue == "false") return false;
            }

            throw ReturnError("Valor inválido");
        }

        private JsonVector ReadVector()
        {
            JsonVector ret = new JsonVector();
            while (true)
            {
                JsonScannerToken token = GetToken();
                if (token.Type == eTokenType.FechaColchetes) return ret;
                PushToken(token);
                ret.Add(InternalReadValue());
                token = GetToken();
                if (token.Type == eTokenType.FechaColchetes) return ret;
                if (token.Type == eTokenType.Virgula) continue;
                throw ReturnError("Item de vetor inválido");
            }
        }

        protected JsonObject InternalReadObject()
        {
            JsonObject ret = new JsonObject();

            while (true)
            {
                JsonScannerToken token = GetToken();
                if (token.Type == eTokenType.FechaChaves) return ret;
                if (token.Type == eTokenType.String)
                {
                    string nome = token.Conteudo.ToString();
                    token = GetToken();
                    if (token.Type != eTokenType.DoisPontos)
                        throw ReturnError("Esperado :");
                    ret.Add(nome, InternalReadValue());

                    token = GetToken();
                    if (token.Type == eTokenType.FechaChaves) return ret;
                    if (token.Type == eTokenType.Virgula) continue;
                }
                throw ReturnError("Par nome/valor inválido");
            }
        }

        private Exception ReturnError(string msg)
        {
            string mensagem = string.Format("Token:[{0}] Erro:{1}", LastToken.ToString(), msg);
            throw new Exception(mensagem);
        }


        public void Dispose()
        {
            if (m_Scanner != null)
            {
                m_Scanner.Dispose();
                m_Scanner = null;
            }
        }
    }
}
