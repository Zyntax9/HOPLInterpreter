using HomeControlInterpreter.Exploration;
using HomeControlInterpreter.NamespaceTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Interpretation
{
    public class InterpretationContext
    {
		public NamespaceSet Namespaces { get; protected set; }
		public ImportAccessTable AccessTable { get; protected set; }
		public List<Handler> Handlers { get; protected set; }
		public Queue<Dependency> EvaluationOrder { get; protected set; }

		public InterpretationContext(Explorer explorer, Queue<Dependency> evalOrder)
		{
			Namespaces = explorer.Namespaces;
			AccessTable = explorer.ImportTable;
			Handlers = explorer.Handlers;
			EvaluationOrder = evalOrder;
		}
	}
}
