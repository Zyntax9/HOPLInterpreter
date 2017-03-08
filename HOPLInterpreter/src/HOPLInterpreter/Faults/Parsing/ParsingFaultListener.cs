using Antlr4.Runtime;
using System.Collections.Generic;

namespace HomeControlInterpreter.Faults.Parsing
{
	public class ParsingFaultListener : IAntlrErrorListener<IToken>
	{
		public List<ParsingFault> Faults { get; protected set; } = new List<ParsingFault>();
		private string file;

		public ParsingFaultListener(string file)
		{
			this.file = file;
		}

		public void SyntaxError(IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
		{
			Faults.Add(new ParsingFault(msg, offendingSymbol, line, charPositionInLine, file));
		}
	}
}
