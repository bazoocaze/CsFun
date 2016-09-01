using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesteStreamScanner.compiler.scanner;

namespace TesteStreamScanner.compiler
{
    public interface ITokenizer
    {
        void Init();
        TokenInfo GetToken();
    }
}
