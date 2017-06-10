using HOPL.Interpreter.NamespaceTypes;
using HOPL.Interpreter.NamespaceTypes.Values;

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
				if(val is InterpreterTrigger)
					((InterpreterTrigger)val).DropReference(); // Drop any references
			}
			base.PopDepth();
		}

		public void SetVariable(string name, InterpreterValue value)
		{
			if (variables.ContainsKey(name))
			{
				if (variables[name] is InterpreterTrigger)
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

        public bool IsLocalEntity(string name)
        {
            return variables.ContainsKey(name);
        }

        public bool IsGlobalEntity(string name)
        {
            return !IsLocalEntity(name) && TopNamespace.ContainsGlobalEntity(name);
        }
	}
}
