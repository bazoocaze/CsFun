using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteStreamScanner.compiler.scanner
{
    public class TokenInfo
    {
        public int Id;
        public string Token;
        public int LineNumber;
        public int CollumnNumber;
        public string FileName;
        public object Tag;

        public override string ToString()
        {
            return String.Format("Id:{0} Token:'{1}' Linha:{2} Coluna:{3} Arquivo:{4}", Id, Token, LineNumber, CollumnNumber, FileName);
        }
    }
}
