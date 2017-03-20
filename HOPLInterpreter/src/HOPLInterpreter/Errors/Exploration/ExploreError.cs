using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Errors.Exploration
{
	public class ExploreError : IError
	{
		public ExploreErrorMessage Message { get; protected set; }
		public int LineNumber { get; protected set; }
		public int ColumnNumber { get; protected set; }
		public string File { get; protected set; }

		string IError.Message { get { return Message.message; } }
		public int ID { get { return Message.id; } }

		public string ErrorTypeName { get { return "Explore Error"; } }

		public ExploreError(ExploreErrorMessage message, int lineNumber, int columnNumber, string file)
		{
			Message = message;
			LineNumber = lineNumber;
			ColumnNumber = columnNumber;
			File = file;
		}

		public ExploreError(ExploreErrorMessage message, ParserRuleContext context, string file)
		{
			Message = message;
			LineNumber = context.start.Line;
			ColumnNumber = context.start.Column;
			File = file;
		}
	}
}
