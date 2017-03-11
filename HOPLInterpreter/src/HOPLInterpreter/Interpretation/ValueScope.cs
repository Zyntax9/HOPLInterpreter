using HOPLInterpreter.NamespaceTypes;
using HOPLInterpreter.NamespaceTypes.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Interpretation
{
	public class ValueScope : Scope<InterpreterValue>
	{
		public ValueScope(Namespace topNamespace) : base(topNamespace) { }

		public override bool TryGetVariable(string name, out InterpreterValue value)
		{
			return variables.TryGetValue(name, out value) || // First look for variable in bodies
				TopNamespace.TryGetGlobalEntityValue(name, out value); // Then in top-namespace
		}

		public void SetVariable(string name, InterpreterValue value)
		{
			if (variables.ContainsKey(name))
			{
				variables[name] = value;
				return;
			}

			IGlobalEntity ge;
			if (TopNamespace.TryGetGlobalEntity(name, out ge) && !ge.Constant)
			{
				ge.Value = value;
				return;
			}

			variables[name] = value;
		}
	}
}
