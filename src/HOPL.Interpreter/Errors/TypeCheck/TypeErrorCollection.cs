using System.Collections.Generic;
using Antlr4.Runtime;

namespace HOPL.Interpreter.Errors.TypeCheck
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
