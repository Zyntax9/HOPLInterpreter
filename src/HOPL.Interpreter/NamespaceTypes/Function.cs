using Parser = HOPL.Grammar.HOPLGrammarParser;

namespace HOPL.Interpreter.NamespaceTypes
{
    public class Function : IFunction
	{
		public string Name { get; protected set; }
		public string File { get; protected set; }
		public FunctionSignature Signature { get; protected set; }
		public Argument[] Arguments { get; protected set; }
		public Parser.BodyContext Body { get; protected set; }
		public Namespace Namespace { get; protected set; }

		public Function(Parser.FunctionDecContext functionDecContext, Namespace @namespace, string file)
		{
			Namespace = @namespace;
			File = file;

			Name = functionDecContext.ID().GetText();
			Signature = new FunctionSignature(functionDecContext);

			Parser.ArgContext[] args = functionDecContext.args().arg();
			Arguments = new Argument[args.Length];
			for(int i = 0; i < args.Length; i++)
				Arguments[i] = new Argument(args[i]);

			Body = functionDecContext.body();
		}
	}
}
