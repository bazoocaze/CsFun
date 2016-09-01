using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace TesteStreamScanner.compiler.scanner
{
	public abstract class ScanEngine : Scanner
	{
		public const int TARGET_STATE_MASK = 0xFFFF;

		protected List<TableStateInfo> m_tabela;

		protected StringBuilder CurrentToken;
		protected StringBuilder LastToken;
		public int LastState;
		public int CurrentState;
		public int NextState;
		protected int PushbackInput;

		protected int LineNumber;
		protected int ColumnNumber;
		protected int TokenLineNumber;
		protected int TokenColumnNumber;
		protected int LastLineNumber;
		protected int LastColumnNumber;


		public List<TokenInfo> Tokens;
		public int FinalState;


		public ScanEngine()
		{
			m_tabela = new List<TableStateInfo>();
			InitTable(new TableConstructor(m_tabela));
		}

		protected abstract int OnGetInput();

		protected abstract void InitTable(ITableConstructor def);

		public void Reset()
		{
			CurrentToken = new StringBuilder();
			LastToken = null;
			LastState = Scanner.INVALID_STATE;
			CurrentState = Scanner.INITIAL_STATE;
			NextState = Scanner.INVALID_STATE;
			PushbackInput = Scanner.INVALID_INPUT;
			LineNumber = 1;
			ColumnNumber = 0;
			Tokens = new List<TokenInfo>();
		}

		protected int GetInput()
		{
			int ret;
			if (PushbackInput != Scanner.INVALID_INPUT)
			{
				ret = PushbackInput;
				PushbackInput = Scanner.INVALID_INPUT;
			}
			else
			{
				this.LastLineNumber = this.LineNumber;
				this.LastColumnNumber = this.ColumnNumber + 1;
				ret = OnGetInput();
				if (ret == Scanner.STREAM_EOF) ret = Scanner.INPUT_EOF;
				if (ret == 10)
				{
					LineNumber++;
					ColumnNumber = 0;
				}
				else
				{
					ColumnNumber++;
				}
			}
			return ret;
		}

		public TokenInfo GetToken()
		{
			if (Tokens.Count == 0)
			{
				InternalProccessInput(true);
			}

			TokenInfo ret = null;
			if (Tokens.Count > 0)
			{
				ret = Tokens[0];
				Tokens.RemoveAt(0);
			}
			return ret;
		}

		public bool ProccessInput()
		{
			return ProccessInput(Scanner.INITIAL_STATE);
		}

		public bool ProccessInput(int initialState)
		{
			Reset();
			CurrentState = initialState;
			OnBeginProccess();
			bool ret = InternalProccessInput(false);
			OnEndProccess();
			return ret;
		}

		protected virtual void OnError(string msg)
		{
			Debug.WriteLine(String.Format("ERRO: {0}", msg));
		}

		public bool InternalProccessInput(bool returnOneToken)
		{
			int input = Scanner.INPUT_EOF;
			bool retorno = false;

			while (true)
			{
				if ((CurrentState < 0 || CurrentState >= m_tabela.Count) || (m_tabela[CurrentState] == null))
				{
					string msg;					
					if (input == Scanner.INPUT_EOF)
						msg = "Fim de arquivo encontrado.";
					else
						msg = "Estado inválido encontrado.";
					FinalState = Scanner.INVALID_STATE;
					OnError(msg);
					break;
				}

				input = GetInput();

				TableStateInfo estadoAtual = m_tabela[CurrentState];
				int target = estadoAtual.GetTarget(input);
				if (target <= 0)
				{
					string msg;
					if (input == Scanner.INPUT_EOF)
						msg = "Fim de arquivo inesperado encontrado.";
					else
					{
						msg = String.Format("Caracter inválido encontrado: {0}", (char)input);
					}
					FinalState = Scanner.INVALID_STATE;
					OnError(msg);
					break;
				}

				NextState = target & TARGET_STATE_MASK;

				char dc = '?';
				if (input >= 32 && input < 127) dc = (char)input;

				if (NextState == Scanner.SUCCESS_STATE)
				{
					FinalState = Scanner.SUCCESS_STATE;
					retorno = true;
					break;
				}
				else if (NextState == Scanner.FAILED_STATE)
				{
					FinalState = Scanner.FAILED_STATE;
					OnError("Falha no reconhecimento da entrada");
					break;
				}

				// ----------------------------

				if ((target & (int)eStateModifiers.e01ReturnToken) != 0)
					ReturnToken(estadoAtual.GetTokenId(input));

				if ((target & (int)eStateModifiers.e02ClearToken) != 0)
					ClearToken();

				if ((target & (int)eStateModifiers.e03InsertToken) != 0)
					InsertToken(input);

				// ----------------------------

				if ((target & (int)eStateModifiers.e04ReturnToken) != 0)
					ReturnToken(estadoAtual.GetTokenId(input));

				if ((target & (int)eStateModifiers.e05ClearToken) != 0)
					ClearToken();

				if ((target & (int)eStateModifiers.e06InsertToken) != 0)
					InsertToken(input);

				// ----------------------------

				if ((target & (int)eStateModifiers.e07ReturnToken) != 0)
					ReturnToken(estadoAtual.GetTokenId(input));

				if ((target & (int)eStateModifiers.e08ClearToken) != 0)
					ClearToken();

				if ((target & (int)eStateModifiers.e09InsertToken) != 0)
					InsertToken(input);

				// ----------------------------

				if ((target & (int)eStateModifiers.e10PushBack) != 0)
					PushInput(input);

				// ----------------------------

				LastState = CurrentState;
				CurrentState = NextState;

				if (returnOneToken && Tokens.Count > 0) break;
			}
			return retorno;
		}

		protected virtual void OnEndProccess()
		{
		}

		protected virtual void OnBeginProccess()
		{
		}

		private void PushInput(int input)
		{
			PushbackInput = input;
		}

		protected void ReturnToken(int id) { OnReturntoken(id); }
		protected void InsertToken(int input) { OnInsertToken(input); }
		protected void ClearToken() { OnClearToken(); }

		protected virtual void OnInsertToken(int input)
		{
			if (this.TokenLineNumber < 0) this.TokenLineNumber = this.LastLineNumber;
			if (this.TokenColumnNumber < 0) this.TokenColumnNumber = this.LastColumnNumber;
			CurrentToken.Append((char)input);
		}

		protected virtual void OnClearToken()
		{
			this.TokenLineNumber = -1;
			this.TokenColumnNumber = -1;
			CurrentToken.Clear();
		}

		protected virtual void OnReturntoken(int tokenId)
		{
			string token = CurrentToken.ToString();
			OnReturnToken(tokenId, token);
		}

		protected virtual void OnReturnToken(int tokenId, string token)
		{
			TokenInfo ret = new TokenInfo();
			ret.Id = tokenId;
			ret.Token = token;
			ret.LineNumber = this.TokenLineNumber;
			ret.CollumnNumber = this.TokenColumnNumber;
			ret.FileName = OnGetFileName();
			Tokens.Add(ret);
		}

		protected virtual string OnGetFileName()
		{
			return "-";
		}

		public ITableConstructor GetConstructor()
		{
			if (m_tabela == null) m_tabela = new List<TableStateInfo>();
			return new TableConstructor(m_tabela);
		}

		// public int FinalState { get { return CurrentState; } }
	}

}
