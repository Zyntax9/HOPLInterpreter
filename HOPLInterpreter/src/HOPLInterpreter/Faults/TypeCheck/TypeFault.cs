using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parser = HOPLGrammar.HOPLGrammarParser;

namespace HomeControlInterpreter.Faults.TypeCheck
{
	public class TypeFault
	{
		public TypeFaultMessage Message { get; protected set; }
		public int LineNumber { get; protected set; }
		public int ColumnNumber { get; protected set; }
		public string File { get; protected set; }

		public TypeFault(TypeFaultMessage message, int lineNumber, int columnNumber, string file)
		{
			Message = message;
			LineNumber = lineNumber;
			ColumnNumber = columnNumber;
			File = file;
		}

		public TypeFault(TypeFaultMessage message, ParserRuleContext context, string file)
		{
			Message = message;
			LineNumber = context.start.Line;
			ColumnNumber = context.start.Column;
			File = file;
		}
	}
}
