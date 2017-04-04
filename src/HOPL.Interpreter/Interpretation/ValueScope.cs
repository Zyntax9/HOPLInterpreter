using HOPL.Interpreter.NamespaceTypes;
using HOPL.Interpreter.NamespaceTypes.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPL.Interpreter.Interpretation
{
	public class ValueScope : Scope<InterpreterValue>
	{
		public ValueScope(Namespace topNamespace) : base(topNamespace) { }

		public override bool TryGetVariable(string name, out InterpreterValue value)
		{
			return variables.TryGetValue(name, out value) || // First look for variable in bodies
				TopNamespace.TryGetGlobalEntityValue(name, out value); // Then in top-namespace
		}

		public override void PopDepth()
		{
			foreach(string varName in scopeLayout[Depth])
			{
				InterpreterValue val = variables[varName];
				if(val.GetType() == typeof(InterpreterTrigger))
					((InterpreterTrigger)val).DropReference(); // Drop any references
			}
			base.PopDepth();
		}

		public void SetVariable(string name, InterpreterValue value)
		{
			if (variables.ContainsKey(name))
			{
				if (variables[name].GetType() == typeof(InterpreterTrigger))
				{
					// Change reference of all referencing triggers
					InterpreterTrigger trigger = (InterpreterTrigger)variables[name];
					trigger.ReferenceChanging((InterpreterTrigger)value);
				}

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
