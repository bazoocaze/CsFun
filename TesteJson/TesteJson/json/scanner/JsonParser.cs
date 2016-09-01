using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteObjetoDinamico.json.scanner
{
    public class JsonParser
    {
        private JsonScanner m_Scanner;

        private string m_ParseErrorMessage;
        private JsonScannerToken m_ParseErrorToken;

        protected JsonScannerToken LastToken;

        public bool ThrowError { get; set; }

        public string ParseErrorMessage { get { return m_ParseErrorMessage; } }
        public JsonScannerToken ParseErrorToken { get { return m_ParseErrorToken; } }

        public JsonParser(JsonScanner scanner)
        {
            Reset();
            this.m_Scanner = scanner;
        }


        public JsonScanner Scanner
        { get { return m_Scanner; } }


        private void Reset()
        {
            this.ThrowError = true;
            this.m_Scanner = null;
            this.m_ParseErrorMessage = null;
            this.m_ParseErrorToken = null;
        }


        public void SetScanner(JsonScanner scanner)
        {
            this.m_Scanner = scanner;
        }




        private JsonScannerToken InputGetToken()
        {
            JsonScannerToken ret = m_Scanner.GetToken();
            LastToken = ret;
            return ret;
        }


        public void Begin()
        {
        }


        public bool ParseObject()
        {
            return ParseObject(true);
        }


        public bool ParseObject(bool eof)
        {
            if (!InternalParseObject()) return ReturnError("Valor JSON esperado");
            if (eof)
            {
                JsonScannerToken token = InputGetToken();
                if (token.Type != eTokenType.Eof) return ReturnError("EOF esperado");
            }
            return true;
        }


        public bool ParseValue()
        {
            return ParseValue(true);
        }


        public bool ParseValue(bool eof)
        {
            while (true)
            {
                JsonScannerToken token = InputGetToken();

                if (token.Type == eTokenType.Numeric) break;
                if (token.Type == eTokenType.String) break;

                if (token.Type == eTokenType.AbreChaves) return InternalParseObjectProperties();
                if (token.Type == eTokenType.AbreColchetes) return InternalParseVector();

                if (token.Type == eTokenType.Ident) return InternalParseIdent(token);

                return ReturnError("Esperado: número, string, '{' ou '['");
            }

            if (eof)
            {
                JsonScannerToken token = InputGetToken();
                if (token.Type != eTokenType.Eof) return ReturnError("EOF esperado");
            }

            return true;
        }

        private bool InternalParseIdent(JsonScannerToken token)
        {
            string cont = token.Conteudo.ToString();
            switch (cont)
            {
                case "true":
                case "false":
                case "null":
                    return true;
            }
            return ReturnError("Esperado true, false ou null");
        }


        public bool ParseVector()
        {
            return ParseVector(true);
        }


        public bool ParseVector(bool eof)
        {
            JsonScannerToken token = InputGetToken();
            if (token.Type != eTokenType.AbreColchetes) return ReturnError("Esperado '['");
            if (!InternalParseVector()) return false;
            if (eof)
            {
                token = InputGetToken();
                if (token.Type != eTokenType.Eof) return ReturnError("EOF esperado");
            }
            return true;
        }


        private bool InternalParseObject()
        {
            JsonScannerToken token = InputGetToken();
            if (token.Type != eTokenType.AbreChaves) return ReturnError("Esperado '{'");
            return InternalParseObjectProperties();
        }


        private bool InternalParseVector()
        {
            while (true)
            {
                if (!ParseValue(false)) return ReturnError("Esperado valor JSON");
                JsonScannerToken token = InputGetToken();
                if (token.Type == eTokenType.FechaColchetes) return true;
                if (token.Type == eTokenType.Virgula) continue;
                return ReturnError("Esperado: ']' ou ','");
            }
        }


        private bool InternalParseObjectProperties()
        {
            while (true)
            {
                JsonScannerToken token = InputGetToken();
                if (token.Type == eTokenType.FechaChaves) return true;
                if (token.Type == eTokenType.String)
                {
                    token = InputGetToken();
                    if (token.Type != eTokenType.DoisPontos) return ReturnError("Esperado ':'");
                    if (!ParseValue(false)) return false;
                    token = InputGetToken();
                    if (token.Type == eTokenType.FechaChaves) return true;
                    if (token.Type == eTokenType.Virgula) continue;
                    return ReturnError("Esperado '}' ou ','");
                }
                return ReturnError("Esperado string ou '}'");
            }
        }

        public bool ReturnError(string mensagem)
        {
            return ReturnError(mensagem, null);
        }

        public bool ReturnError(string mensagem, JsonScannerToken token)
        {
            if (token == null) token = LastToken;

            mensagem += String.Format(" (linha {0}, coluna {1})", token.NumeroLinha, token.NumeroColuna);
            if (ThrowError) throw new Exception(mensagem);
            if (String.IsNullOrWhiteSpace(this.m_ParseErrorMessage))
            {
                this.m_ParseErrorMessage = mensagem;
                this.m_ParseErrorToken = token;
            }
            return false;
        }
    }
}
