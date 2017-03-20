using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace HOPLInterpreter.Errors.TypeCheck
{
	public class TypeErrorCollection : List<TypeError>
	{
		public TypeErrorCollection() : base() { }

		public void Add(TypeErrorMessage msg, ParserRuleContext context, string file)
		{
			Add(new TypeError(msg, context, file));
		}
	}
}
