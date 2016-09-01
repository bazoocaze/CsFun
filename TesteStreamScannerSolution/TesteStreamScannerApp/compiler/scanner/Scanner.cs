using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteStreamScanner.compiler.scanner
{
	public abstract class Scanner
	{
		public const int INVALID_INPUT = -1000;

		public const int FAILED_STATE = 0;
		public const int INVALID_STATE = 1;
		public const int SUCCESS_STATE = 2;
		public const int INITIAL_STATE = 3;

		public const int INPUT_EOF = 0;
		public const int STREAM_EOF = -1;
		public const int NO_TOKEN_ID = 0;
	}
}
