using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesteStreamScanner.compiler;
using TesteStreamScanner.compiler.scanner;

namespace TesteStreamScanner.teste
{
    public class TesteScanner2 : Tokenizer
    {
        public const int TOKEN_STRING1 = 10;
        public const int TOKEN_STRING2 = 11;
        public const int TOKEN_STRING_ASPAS = 20;

		protected override void OnCreateTable(ITableConstructor def)
        {
            def.SetStateFail(INITIAL_STATE);
            def.SetStateSuccessEOF(INITIAL_STATE);

            def.SetState(INITIAL_STATE, INITIAL_STATE, " \t\r\n");
            def.SetState(INITIAL_STATE, 10, "a-zA-Z", eStateModifiers.e02ClearToken | eStateModifiers.e03InsertToken);
            def.SetState(INITIAL_STATE, 11, "z", eStateModifiers.e02ClearToken | eStateModifiers.e03InsertToken);
            def.SetState(INITIAL_STATE, 20, "\"", eStateModifiers.e02ClearToken);

            def.SetState(10, INITIAL_STATE, eStateModifiers.e01ReturnToken | eStateModifiers.e10PushBack, TOKEN_STRING1);
            def.SetState(10, 10, "a-zA-Z", eStateModifiers.e03InsertToken);
            def.SetState(10, 11, "z", eStateModifiers.e03InsertToken);

            def.SetState(11, INITIAL_STATE, eStateModifiers.e01ReturnToken | eStateModifiers.e10PushBack, TOKEN_STRING2);
            def.SetState(11, 10, "a-zA-Z", eStateModifiers.e03InsertToken);
            def.SetState(11, 11, "z", eStateModifiers.e03InsertToken);

            // def.SetState(20, INITIAL_STATE, eScannerStateModifiers.e01ReturnToken | eScannerStateModifiers.e10PushBack, TOKEN_STRING_ASPAS);
            def.SetState(20, 20, eStateModifiers.e03InsertToken);
            def.SetStateFailEOF(20);
            def.SetState(20, INITIAL_STATE, "\"", eStateModifiers.e04ReturnToken, TOKEN_STRING_ASPAS);
        }
    }
}
