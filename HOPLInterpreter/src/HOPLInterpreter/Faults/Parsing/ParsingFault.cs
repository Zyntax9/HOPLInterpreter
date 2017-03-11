using Antlr4.Runtime;

namespace HOPLInterpreter.Faults.Parsing
{
	public class ParsingFault
	{
		public string Message { get; protected set; }
		public IToken Token { get; protected set; }
		public int LineNumber { get; protected set; }
		public int ColumnNumber { get; protected set; }
		public string File { get; protected set; }

		public ParsingFault(string message, IToken token, int line, int column, string file)
		{
			Message = message;
			Token = token;
			LineNumber = line;
			ColumnNumber = column;
			File = file;
		}
	}
}
