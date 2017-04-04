using HOPL.Interpreter.Exploration;
using HOPL.Interpreter.Interpretation;
using Parser = HOPL.Grammar.HOPLGrammarParser;

namespace HOPL.Interpreter.NamespaceTypes
{
	public delegate void HandlerTriggeredEvent(HandlerContext context);

	public class Handler
    {
		public Parser.HandlerDecContext Context { get; protected set; }
		public Namespace Namespace { get; protected set; }
		public string File { get; protected set; }
		public NamespaceSet Namespaces { get; protected set; }
		public ImportAccessTable AccessTable { get; set; }

		public event HandlerTriggeredEvent HandlerTriggered;

		public Handler(Parser.HandlerDecContext context, Namespace @namespace, string file, 
			NamespaceSet namespaces, ImportAccessTable accessTable)
		{
			Context = context;
			Namespace = @namespace;
			File = file;
			Namespaces = namespaces;
			AccessTable = accessTable;
		}

		public void OnTrigger(object sender, object[] arguments)
		{
			HandlerContext context = new HandlerContext(this, arguments);
			HandlerTriggered(context);
		}
	}
}
