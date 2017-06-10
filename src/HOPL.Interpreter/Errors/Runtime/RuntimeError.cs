using Antlr4.Runtime;

namespace HOPL.Interpreter.Errors.Runtime
{
    public class RuntimeError : IError
	{
		public RuntimeErrorMessage Message { get; protected set; }
		public int LineNumber { get; protected set; }
		public int ColumnNumber { get; protected set; }
		public string File { get; protected set; }

		string IError.Message { get { return Message.message; } }
		public int ID { get { return Message.id; } }

        public static string ErrorType = "Runtime Error";
        public string ErrorTypeName { get { return ErrorType; } }

		public RuntimeError(RuntimeErrorMessage message, int lineNumber, int columnNumber, string file)
		{
			Message = message;
			LineNumber = lineNumber;
			ColumnNumber = columnNumber;
			File = file;
		}

		public RuntimeError(RuntimeErrorMessage message, ParserRuleContext context, string file)
		{
			Message = message;
			LineNumber = context.start.Line;
			ColumnNumber = context.start.Column;
			File = file;
		}
	}
}
