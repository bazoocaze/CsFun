using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TesteObjetoDinamico.json.scanner
{
    public class JsonScanner : IDisposable
    {
        public const int P_EOF = -1;
        public const int P_NO_CHAR = -2;
        public const int P_CR = 13;
        public const int P_LF = 10;

        protected JsonScannerToken CurrentToken;
        protected int NumeroLinha;
        protected int NumeroColuna;

        protected int m_nextChar;
        private TextReader m_Input;

        protected List<JsonScannerToken> m_tokens;
        protected int m_tokensIndex;

        public JsonScanner()
        {
            Reset();
        }

        public JsonScanner(TextReader input)
        {
            Reset();
            this.m_Input = input;
        }

        public void SetInput(string input)
        {
            MemoryStream ms = new MemoryStream();
            StreamWriter sw = new StreamWriter(ms);
            sw.Write(input);
            sw.Flush();
            ms.Position = 0;
            SetInput(ms);
        }


        public void SetInput(Stream input)
        {
            SetInput(new StreamReader(input));
        }


        public void SetInput(TextReader input)
        {
            this.m_Input = input;
            Reset();
        }


        public void Reset()
        {
            NumeroLinha = 1;
            NumeroColuna = 0;
            m_nextChar = P_NO_CHAR;
            m_tokens = new List<JsonScannerToken>();
            m_tokensIndex = 0;
            NewCurrentToken();
        }


        protected void NewCurrentToken()
        {
            CurrentToken = new JsonScannerToken();
            CurrentToken.NumeroLinha = this.NumeroLinha;
            CurrentToken.NumeroColuna = this.NumeroColuna;
        }


        protected bool IsSeparador(char c)
        {
            if (c == 32 || c == 8 || c == 13 || c == 10) return true;
            return false;
        }


        static int lastC;

        protected int InputReadChar()
        {
            int c = m_Input.Read();
            if (c == 13 || c == 10)
            {
                if (!(c == 10 && lastC == 13))
                {
                    NumeroLinha++;
                    NumeroColuna = 0;
                }
            }
            else
            {
                NumeroColuna++;
            }
            lastC = c;
            return c;
        }


        protected int ReadChar()
        {
            int c;
            if (m_nextChar != P_NO_CHAR)
            {
                c = m_nextChar;
                m_nextChar = P_NO_CHAR;
            }
            else
            {
                c = InputReadChar();
            }
            return c;
        }


        protected int NextChar
        {
            get
            {
                if (m_nextChar != P_NO_CHAR) return m_nextChar;
                m_nextChar = InputReadChar();
                return m_nextChar;
            }
        }

        public void Clear()
        {
            m_tokensIndex = 0;
            m_tokens.Clear();
            if (m_Input != null) m_Input.Dispose();
            m_Input = null;
        }

        public void Rewind()
        {
            m_tokensIndex = 0;
        }

        public JsonScannerToken GetToken()
        {
            if (m_Input != null)
            {
                JsonScannerToken ret = InternalGetToken();
                if (ret.Type == eTokenType.Eof || ret.Type == eTokenType.Erro)
                {
                    m_Input.Dispose();
                    m_Input = null;
                }
                m_tokens.Add(ret);
                return ret;
            }
            if (m_tokensIndex < m_tokens.Count)
                return m_tokens[m_tokensIndex++];
            return JsonScannerToken.New(eTokenType.Eof);
        }

        protected JsonScannerToken InternalGetToken()
        {
            while (true)
            {
                int lido = ReadChar();
                if (lido == P_EOF) return ReturnToken(eTokenType.Eof);
                char c = (char)lido;
                if (IsSeparador(c)) continue;
                if (c == '\"') return GetTokenString();

                CurrentTokenAppend(c);
                if (c == ':') return ReturnToken(eTokenType.DoisPontos);
                if (c == '{') return ReturnToken(eTokenType.AbreChaves);
                if (c == '}') return ReturnToken(eTokenType.FechaChaves);
                if (c == '[') return ReturnToken(eTokenType.AbreColchetes);
                if (c == ']') return ReturnToken(eTokenType.FechaColchetes);
                if (c == ',') return ReturnToken(eTokenType.Virgula);
                if (Char.IsDigit(c) || c == '-') return GetTokenNumerico();
                if (Char.IsLetter(c)) return GetTokenIdent();
                if (lido < 32) return ReturnErro("caracter inválido encontrado");
                return ReturnErro(String.Format("caracter inesperado encontrado: {0}", c));
            }
        }


        private JsonScannerToken GetTokenIdent()
        {
            while (true)
            {
                int lido = ReadChar();
                if (lido == P_EOF) break;

                char c = (char)lido;

                if (Char.IsLetter(c))
                {
                    CurrentTokenAppend(c);
                    continue;
                }

                PushChar(c);
                break;
            }
            return ReturnToken(eTokenType.Ident);
        }


        private JsonScannerToken GetTokenNumerico()
        {
            while (true)
            {
                int lido = ReadChar();
                if (lido == P_EOF) break;

                char c = (char)lido;
                if (IsSeparador(c)) break;

                if (Char.IsDigit(c) || c == '.' || c == 'e' || c == 'E' || c == '+' || c == '-')
                {
                    CurrentTokenAppend(c);
                    continue;
                }

                PushChar(c);
                break;
            }
            return ReturnToken(eTokenType.Numeric);
        }


        private void PushChar(char c)
        {
            m_nextChar = c;
        }


        protected JsonScannerToken ReturnToken(eTokenType tipo)
        {
            return ReturnToken(tipo, null);
        }


        protected JsonScannerToken ReturnToken(eTokenType tipo, object conteudo)
        {
            CurrentToken.Type = tipo;
            if (conteudo != null) CurrentTokenAppend(conteudo);
            JsonScannerToken ret = CurrentToken;
            NewCurrentToken();
            return ret;
        }



        protected JsonScannerToken GetTokenString()
        {
            bool escape = false;
            while (true)
            {
                int lido = ReadChar();
                if (lido == P_EOF) return ReturnErro("esperado fim do string (\") mas encontrado fim do arquivo");
                if (lido < 32)
                {
                    if (lido == P_CR || lido == P_LF) return ReturnErro("string não finalizado - faltando o caracter \"");
                    return ReturnErro("caracter inválido encontrado dentro do string");
                }
                if (!escape)
                {
                    if (lido == '\\')
                    {
                        escape = true;
                        continue;
                    }
                    if (lido == '\"')
                    {
                        return ReturnToken(eTokenType.String);
                    }
                }
                else
                {
                    escape = false;
                    int novoChar = -1;
                    switch (lido)
                    {
                        case '\"':
                        case '\\':
                        case '/':
                            novoChar = lido;
                            break;
                        case 'b': novoChar = 8; break;
                        case 'f': novoChar = 12; break;
                        case 'n': novoChar = 10; break;
                        case 'r': novoChar = 13; break;
                        case 't': novoChar = 9; break;
                        case 'u':
                            StringBuilder strHex = new StringBuilder();
                            while (strHex.Length < 4)
                            {
                                int hexDigit = ReadChar();
                                if (!((hexDigit >= 'a' && hexDigit <= 'z') || (hexDigit >= 'A' && hexDigit <= 'Z'))) break;
                                strHex.Append(hexDigit);
                            }
                            if (strHex.Length == 4)
                                novoChar = Convert.ToInt32(strHex.ToString(), 16);
                            break;
                    }
                    if (novoChar == -1) return ReturnErro("sequência de escape inválida");
                    lido = novoChar;
                }
                CurrentTokenAppend((char)lido);
            }
        }


        protected void CurrentTokenAppend(object valor)
        {
            if(CurrentToken.Conteudo.Length == 0)
            {
                CurrentToken.NumeroLinha = this.NumeroLinha;
                CurrentToken.NumeroColuna = this.NumeroColuna;
            }
            CurrentToken.Conteudo.Append(valor);
        }


        protected JsonScannerToken ReturnErro(string msg)
        {
            CurrentToken.NumeroLinha = this.NumeroLinha;
            CurrentToken.NumeroColuna = this.NumeroColuna - 1;
            string mensagem = String.Format("linha {0} coluna {1}: {2}", CurrentToken.NumeroLinha, CurrentToken.NumeroColuna, msg);
            CurrentToken.Mensagem = mensagem;
            return ReturnToken(eTokenType.Erro);
        }


        public void Dispose()
        {
            if (m_Input != null)
            {
                m_Input.Dispose();
                m_Input = null;
            }
        }
    }


    public enum eTokenType
    {
        Ident,
        String,
        Numeric,
        AbreColchetes,
        FechaColchetes,
        AbreChaves,
        FechaChaves,
        DoisPontos,
        Virgula,
        Erro,
        Eof
    }
}
