using Antlr4.Runtime;
using System.Collections.Generic;

namespace HOPLInterpreter.Errors.Parsing
{
	public class ParsingErrorListener : IAntlrErrorListener<IToken>
	{
		public List<ParsingError> Errors { get; protected set; } = new List<ParsingError>();
		private string file;

		public ParsingErrorListener(string file)
		{
			this.file = file;
		}

		public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
		{
			Errors.Add(new ParsingError(msg, offendingSymbol, line, charPositionInLine, file));
		}
	}
}
