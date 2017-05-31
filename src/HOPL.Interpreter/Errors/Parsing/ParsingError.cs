using System;
using Antlr4.Runtime;

namespace HOPL.Interpreter.Errors.Parsing
{
	public class ParsingError : IError
	{
		public string Message { get; protected set; }
		public IToken Token { get; protected set; }
		public int LineNumber { get; protected set; }
		public int ColumnNumber { get; protected set; }
		public string File { get; protected set; }

		public int ID { get { return 0; } }

        public static string ErrorType = "Parsing Error";
        public string ErrorTypeName { get { return ErrorType; } }
		
		public ParsingError(string message, IToken token, int line, int column, string file)
		{
			Message = message;
			Token = token;
			LineNumber = line;
			ColumnNumber = column;
			File = file;
		}
	}
}
