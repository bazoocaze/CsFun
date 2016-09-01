using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TesteStreamScanner.compiler.scanner;

namespace TesteStreamScanner.teste
{
	public class TesteCScanner : Tokenizer
	{
		public const int STATE_IDENT = 10;
		public const int STATE_NUMERO = 11;
		public const int STATE_STRING = 12;
		public const int STATE_STRING1 = 13;

		public const int TOKEN_IDENT = 1;
		public const int TOKEN_NUMERO = 2;
		public const int TOKEN_IGUAL = 10;
		public const int TOKEN_PONTO_VIRGULA = 11;
		public const int TOKEN_ABRE_PAR = 12;
		public const int TOKEN_FECHAR_PAR = 13;
		public const int TOKEN_STRING = 14;

		protected override void OnCreateTable(ITableConstructor def)
		{
			def.SetStateFail(INITIAL_STATE);
			def.SetStateSuccessEOF(INITIAL_STATE);
			def.SetState(INITIAL_STATE, INITIAL_STATE, " \t\r\n");

			def.SetState(INITIAL_STATE, STATE_IDENT, "a-zA-Z", eStateModifiers.e02ClearToken | eStateModifiers.e03InsertToken);
			def.SetState(STATE_IDENT, INITIAL_STATE, eStateModifiers.e01ReturnToken | eStateModifiers.e10PushBack, TOKEN_IDENT);
			def.SetState(STATE_IDENT, STATE_IDENT, "a-zA-Z0-9", eStateModifiers.e03InsertToken);

			def.SetState(INITIAL_STATE, STATE_NUMERO, "0-9", eStateModifiers.e02ClearToken | eStateModifiers.e03InsertToken);
			def.SetState(STATE_NUMERO, INITIAL_STATE, eStateModifiers.e01ReturnToken | eStateModifiers.e10PushBack, TOKEN_NUMERO);
			def.SetState(STATE_NUMERO, STATE_NUMERO, "0-9", eStateModifiers.e03InsertToken);

			def.SetState(INITIAL_STATE, INITIAL_STATE, "=", eStateModifiers.e02ClearToken | eStateModifiers.e03InsertToken | eStateModifiers.e04ReturnToken | eStateModifiers.e05ClearToken, TOKEN_IGUAL);
			def.SetState(INITIAL_STATE, INITIAL_STATE, ";", eStateModifiers.e02ClearToken | eStateModifiers.e03InsertToken | eStateModifiers.e04ReturnToken | eStateModifiers.e05ClearToken, TOKEN_PONTO_VIRGULA);
			def.SetState(INITIAL_STATE, INITIAL_STATE, "(", eStateModifiers.e02ClearToken | eStateModifiers.e03InsertToken | eStateModifiers.e04ReturnToken | eStateModifiers.e05ClearToken, TOKEN_ABRE_PAR);
			def.SetState(INITIAL_STATE, INITIAL_STATE, ")", eStateModifiers.e02ClearToken | eStateModifiers.e03InsertToken | eStateModifiers.e04ReturnToken | eStateModifiers.e05ClearToken, TOKEN_FECHAR_PAR);

			def.SetState(INITIAL_STATE, STATE_STRING, "\"", eStateModifiers.e02ClearToken | eStateModifiers.e03InsertToken);
			def.SetState(STATE_STRING, STATE_STRING, eStateModifiers.e03InsertToken);
			def.SetStateFailEOF(STATE_STRING);
			def.SetState(STATE_STRING, INITIAL_STATE, "\"", eStateModifiers.e03InsertToken | eStateModifiers.e04ReturnToken | eStateModifiers.e05ClearToken, TOKEN_STRING);
			def.SetState(STATE_STRING, STATE_STRING1, "\\", eStateModifiers.e03InsertToken);
			def.SetState(STATE_STRING1, STATE_STRING, eStateModifiers.e03InsertToken);
			def.SetStateFailEOF(STATE_STRING1);
		}
	}
}
