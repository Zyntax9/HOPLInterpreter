using HOPL.Interpreter.Exploration;
using HOPL.Interpreter.NamespaceTypes;
using System.Collections.Generic;

namespace HOPL.Interpreter.Interpretation
{
    public class InterpretationContext
    {
        public string MainFile { get; protected set; }
        public NamespaceSet Namespaces { get; protected set; }
		public ImportAccessTable AccessTable { get; protected set; }
		public List<Handler> Handlers { get; protected set; }
        public Dictionary<string, InterpreterType> Required { get; set; }
        public Queue<Dependency> EvaluationOrder { get; protected set; }

        public InterpretationContext(string mainFile, Explorer explorer, Queue<Dependency> evalOrder)
		{
            MainFile = mainFile;
			Namespaces = explorer.Namespaces;
			AccessTable = explorer.ImportTable;
			Handlers = explorer.Handlers;
            Required = explorer.Required;
			EvaluationOrder = evalOrder;
		}
	}
}
