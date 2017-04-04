using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parser = HOPL.Grammar.HOPLGrammarParser;

namespace HOPL.Interpreter.Errors.TypeCheck
{
	public class TypeError : IError
	{
		public TypeErrorMessage Message { get; protected set; }
		public int LineNumber { get; protected set; }
		public int ColumnNumber { get; protected set; }
		public string File { get; protected set; }

		string IError.Message { get { return Message.message; } }
		public int ID { get { return Message.id; } }

		public string ErrorTypeName { get { return "Type Error"; } }

		public TypeError(TypeErrorMessage message, int lineNumber, int columnNumber, string file)
		{
			Message = message;
			LineNumber = lineNumber;
			ColumnNumber = columnNumber;
			File = file;
		}

		public TypeError(TypeErrorMessage message, ParserRuleContext context, string file)
		{
			Message = message;
			LineNumber = context.start.Line;
			ColumnNumber = context.start.Column;
			File = file;
		}
	}
}
