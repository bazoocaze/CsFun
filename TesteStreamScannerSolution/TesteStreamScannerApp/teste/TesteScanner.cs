using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesteStreamScanner.compiler;
using TesteStreamScanner.compiler.scanner;

namespace TesteStreamScanner.teste
{
    public class TesteScanner : Tokenizer
    {
        public const int STATE_STRING = 10;
        public const int TOKEN_STRING = 10;

        public const int STATE_NUMBER = 20;
        public const int TOKEN_NUMBER = 20;

        public const int STATE_DATE = 30;
        public const int TOKEN_DATE = 30;

		protected override void OnCreateTable(ITableConstructor def)
        {
            def.SetStateFail(INITIAL_STATE);
			def.SetStateSuccessEOF(Scanner.INITIAL_STATE);
			def.SetState(Scanner.INITIAL_STATE, Scanner.INITIAL_STATE, " \t\r\n");

			def.SetState(Scanner.INITIAL_STATE, STATE_STRING, "a-zA-Z", eStateModifiers.e02ClearToken | eStateModifiers.e03InsertToken);
			def.SetState(STATE_STRING, Scanner.INITIAL_STATE, eStateModifiers.e01ReturnToken | eStateModifiers.e10PushBack, TOKEN_STRING);
			def.SetState(STATE_STRING, STATE_STRING, "a-zA-Z", eStateModifiers.e03InsertToken);

			def.SetState(Scanner.INITIAL_STATE, STATE_NUMBER, "0-9", eStateModifiers.e02ClearToken | eStateModifiers.e03InsertToken);
			def.SetState(STATE_NUMBER, Scanner.INITIAL_STATE, eStateModifiers.e01ReturnToken | eStateModifiers.e10PushBack, TOKEN_NUMBER);
            def.SetState(STATE_NUMBER, STATE_NUMBER, "0-9", eStateModifiers.e03InsertToken);

            def.SetState(STATE_NUMBER, STATE_DATE, "-/", eStateModifiers.e03InsertToken);
			def.SetState(STATE_DATE, Scanner.INITIAL_STATE, eStateModifiers.e01ReturnToken | eStateModifiers.e10PushBack, TOKEN_DATE);
            def.SetState(STATE_DATE, STATE_DATE, "-/Tt:0-9", eStateModifiers.e03InsertToken);
        }
    }
}
