using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TesteStreamScanner.compiler.scanner
{
    public abstract class Tokenizer : GenericScanner
    {
        public Tokenizer()
			:base(null)
        {

        }

		protected override void InitTable(ITableConstructor def)
		{
			OnCreateTable(def);
		}

		protected override abstract void OnCreateTable(ITableConstructor def);
    }
}
