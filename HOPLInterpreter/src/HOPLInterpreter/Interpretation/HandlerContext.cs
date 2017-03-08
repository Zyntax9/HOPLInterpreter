using HomeControlInterpreter.Exploration;
using HomeControlInterpreter.NamespaceTypes;
using Parser = HOPLGrammar.HOPLGrammarParser;

namespace HomeControlInterpreter.Interpretation
{
	public class HandlerContext
	{
		public Handler Handler { get; protected set; }
		public object[] Arguments { get; set; }

		public HandlerContext(Namespace @namespace, NamespaceSet namespaces, Parser.HandlerDecContext context, 
			string file, ImportAccessTable accessTable, object[] triggerArguments)
		{
			Handler = new Handler(context, @namespace, file, namespaces, accessTable);
			Arguments = triggerArguments;
		}

		public HandlerContext(Handler handler, object[] triggerArguments)
		{
			Handler = handler;
			Arguments = triggerArguments;
		}
	}
}
