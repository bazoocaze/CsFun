using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesteStreamScanner.compiler.scanner;

namespace TesteStreamScanner.compiler
{
    public class ManualParser
    {
        public ITokenizer Scanner;


        protected TokenInfo CurrentToken;
        protected TokenInfo LastToken;

        public void SetSource(ITokenizer tokenizer)
        {
            this.Scanner = tokenizer;
        }

        public void Init()
        {
            OnInit();
        }

        protected virtual void OnInit()
        {
            Scanner.Init();
            ReadToken();
        }

        protected void ReadToken()
        {
            LastToken = CurrentToken;
            CurrentToken = Scanner.GetToken();
        }

        protected bool Accept(int tokenId)
        {
            if (CurrentToken.Id == tokenId)
            {
                ReadToken();
                return true;
            }
            return false;
        }

        protected void Expect(int tokenId)
        {
            if (Accept(tokenId)) return;
            throw new Exception(String.Format("ERRO: token incorreto. Esperado {0}, encontrado {1}",
                tokenId, CurrentToken.Id));
        }

        public bool Parse()
        {
            return OnParse();
        }

        protected virtual bool OnParse()
        {
            return false;
        }
    }
}
