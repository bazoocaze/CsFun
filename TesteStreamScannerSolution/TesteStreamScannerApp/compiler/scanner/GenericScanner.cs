using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace TesteStreamScanner.compiler.scanner
{
    public abstract class GenericScanner : ScanEngine
    {
		protected TextReader InputReader;
		public string FileName;

        public GenericScanner()
        {
        }

        public GenericScanner(TextReader input)
        {
			InputReader = input;
        }

		public void SetInput(TextReader input)
		{
		}

		public void SetInput(string input)
		{
			InputReader = new StringReader(input);
		}

		public void SetInput(Stream input)
		{
			InputReader = new StreamReader(input);
		}

		protected override int OnGetInput()
		{
			return InputReader.Read();
		}

		protected override void InitTable(ITableConstructor def)
		{
			OnCreateTable(def);
		}

		protected abstract void OnCreateTable(ITableConstructor def);
	}
}
