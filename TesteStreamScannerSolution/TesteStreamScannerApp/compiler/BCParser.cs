using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteStreamScanner.compiler
{
    public class BCParser : ManualParser
    {
        protected void expr()
        {
            if (Accept(BCScanner.TOKEN_IDENT) || Accept(BCScanner.STATE_NUMBER))
            {
                while (Accept(BCScanner.TOKEN_PLUS))
                    expr();
                return;
            }
            throw new Exception(
                String.Format("ERRO: expressão inválida. Encontrado {0}, esperado {1} ou {2}",
                CurrentToken.Id, BCScanner.TOKEN_IDENT, BCScanner.TOKEN_NUMBER));
        }

        protected void prog()
        {
            expr();
            Expect(BCScanner.TOKEN_EOF);
        }

        protected override bool OnParse()
        {
            prog();
            return true;
        }
    }
}
