using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteStreamScanner.compiler.scanner
{
	public class TableConstructor : ITableConstructor
	{
		protected List<TableStateInfo> m_tabela;

		public TableConstructor(List<TableStateInfo> tabela)
		{
			m_tabela = tabela;
		}

		private void Clear()
		{
			if (m_tabela != null) m_tabela.Clear();
		}

		protected TableStateInfo GetState(int state)
		{
			while (m_tabela.Count <= state) m_tabela.Add(null);
			if (m_tabela[state] == null)
				m_tabela[state] = new TableStateInfo(state);
			return m_tabela[state];
		}

		#region IScannerConstructor Members

		void ITableConstructor.SetState(int fromState, int toState, eStateModifiers modifiers)
		{
			this.SetState(fromState, toState, modifiers, Scanner.NO_TOKEN_ID);
		}

		void ITableConstructor.SetState(int fromState, int toState, eStateModifiers modifiers, int tokenId)
		{
			this.SetState(fromState, toState, modifiers, tokenId);
		}

		void ITableConstructor.SetState(int fromState, int toState, string chars)
		{
			this.SetState(fromState, toState, chars, eStateModifiers.eNenhum, Scanner.NO_TOKEN_ID);
		}

		void ITableConstructor.SetState(int fromState, int toState, string chars, eStateModifiers modifiers)
		{
			this.SetState(fromState, toState, chars, modifiers, Scanner.NO_TOKEN_ID);
		}

		void ITableConstructor.SetState(int fromState, int toState, string chars, eStateModifiers modifiers, int tokenId)
		{
			this.SetState(fromState, toState, chars, modifiers, tokenId);
		}

		void ITableConstructor.SetStateSuccess(int stateIndex)
		{
			this.SetStateSuccess(stateIndex);
		}

		void ITableConstructor.SetStateFail(int stateIndex)
		{
			this.SetStateFail(stateIndex);
		}

		void ITableConstructor.SetStateFailEOF(int stateIndex)
		{
			this.SetStateFailEOF(stateIndex);
		}

		void ITableConstructor.SetStateSuccessEOF(int stateIndex)
		{
			this.SetStateSuccessEOF(stateIndex);
		}

		void ITableConstructor.Clear()
		{
			this.Clear();
		}

		private void SetState(int fromState, int toState, eStateModifiers modifiers, int tokenId)
		{
			TableStateInfo state = GetState(fromState);
			state.SetState(toState, modifiers, tokenId);
		}

		private void SetState(int fromState, int toState, string chars, eStateModifiers modifiers, int tokenId)
		{
			TableStateInfo state = GetState(fromState);
			state.SetState(toState, chars, modifiers, tokenId);
		}

		private void SetStateSuccess(int stateIndex)
		{
			TableStateInfo state = GetState(stateIndex);
			state.SetState(Scanner.SUCCESS_STATE, eStateModifiers.eNenhum, Scanner.NO_TOKEN_ID);
		}

		private void SetStateFail(int stateIndex)
		{
			TableStateInfo state = GetState(stateIndex);
			state.SetState(Scanner.FAILED_STATE, eStateModifiers.eNenhum, Scanner.NO_TOKEN_ID);
		}

		private void SetStateFailEOF(int stateIndex)
		{
			TableStateInfo state = GetState(stateIndex);
			state.SetState(Scanner.FAILED_STATE, Scanner.INPUT_EOF, eStateModifiers.eNenhum, Scanner.NO_TOKEN_ID);
		}

		private void SetStateSuccessEOF(int stateIndex)
		{
			TableStateInfo state = GetState(stateIndex);
			state.SetState(Scanner.SUCCESS_STATE, Scanner.INPUT_EOF, eStateModifiers.eNenhum, Scanner.NO_TOKEN_ID);
		}

		#endregion IScannerConstructor Members

	}

}
