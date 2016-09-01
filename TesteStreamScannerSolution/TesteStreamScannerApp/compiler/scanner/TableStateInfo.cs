using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteStreamScanner.compiler.scanner
{
	public class TableStateInfo
	{
		public const int MAX_INPUTS = 260;

		public int State;

		protected int[] Target;
		protected int[] TokenId;

		protected TableStateInfo()
		{
			Target = new int[MAX_INPUTS];
			TokenId = new int[MAX_INPUTS];

			for (int p = 0; p < MAX_INPUTS; p++)
			{
				Target[p] = GenericScanner.INVALID_STATE;
				TokenId[p] = GenericScanner.NO_TOKEN_ID;
			}
		}

		public TableStateInfo(int state)
			: this()
		{
			this.State = state;
		}

		public void SetState(int toState, int input, eStateModifiers modifiers, int tokenId)
		{
			int dest = toState | (int)modifiers;
			Target[input] = dest;
			TokenId[input] = tokenId;
		}

		public void SetState(int toState, eStateModifiers modifiers, int tokenId)
		{
			int dest = toState | (int)modifiers;
			for (int p = 0; p < MAX_INPUTS; p++)
			{
				Target[p] = dest;
				TokenId[p] = tokenId;
			}
		}

		public void SetState(int toState, string chars, eStateModifiers modifiers, int tokenId)
		{
			int dest = toState | (int)modifiers;
			int lastChar = 0;
			for (int pos = 0; pos < chars.Length; pos++)
			{
				int c = (int)chars[pos];
				if ((c == '-') && (pos > 0) && (pos + 1 < chars.Length))
				{
					for (int range = lastChar; range < (int)chars[pos + 1]; range++)
					{
						Target[range] = dest;
						TokenId[range] = tokenId;
					}
				}
				else
				{
					Target[c] = dest;
					TokenId[c] = tokenId;
				}
				lastChar = c;
			}
		}

		public int GetTarget(int input)
		{
			if (input < 0 || input >= MAX_INPUTS)
				return GenericScanner.INVALID_INPUT;
			return Target[input];
		}

		public int GetTokenId(int input)
		{
			if (input < 0 || input >= MAX_INPUTS)
				return GenericScanner.NO_TOKEN_ID;
			return TokenId[input];
		}

		public override string ToString()
		{
			return String.Format("State {0}", State);
		}
	}
}
