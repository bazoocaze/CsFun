using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteStreamScanner.compiler.scanner
{
    public interface ITableConstructor
    {
        void Clear();

        void SetState(int fromState, int toState, eStateModifiers modifiers);
        void SetState(int fromState, int toState, eStateModifiers modifiers, int tokenId);
        void SetState(int fromState, int toState, string chars);
        void SetState(int fromState, int toState, string chars, eStateModifiers modifiers);
        void SetState(int fromState, int toState, string chars, eStateModifiers modifiers, int tokenId);
        void SetStateSuccess(int stateIndex);
        void SetStateSuccessEOF(int stateIndex);
        void SetStateFail(int stateIndex);
        void SetStateFailEOF(int stateIndex);
    }

	[Flags]
	public enum eStateModifiers
	{
		eNenhum = 0,
		e01ReturnToken = (1 << 30),
		e02ClearToken = (1 << 29),
		e03InsertToken = (1 << 28),
		e04ReturnToken = (1 << 27),
		e05ClearToken = (1 << 26),
		e06InsertToken = (1 << 25),
		e07ReturnToken = (1 << 24),
		e08ClearToken = (1 << 23),
		e09InsertToken = (1 << 22),
		e10PushBack = (1 << 21),
	}
}
