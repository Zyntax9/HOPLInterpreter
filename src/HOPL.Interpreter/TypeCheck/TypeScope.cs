using HOPL.Interpreter.Exceptions;
using HOPL.Interpreter.NamespaceTypes;
using System.Collections.Generic;

namespace HOPL.Interpreter.TypeCheck
{
	public class TypeScope : Scope<InterpreterType>
	{
		public TypeScope(Namespace topNamespace) : base(topNamespace) { }

		public override bool TryGetVariable(string name, out InterpreterType type)
		{
			return variables.TryGetValue(name, out type) || // First look for variable in bodies
				TopNamespace.TryGetGlobalEntityType(name, out type);    // Then in top-namespace
		}
	}
}
