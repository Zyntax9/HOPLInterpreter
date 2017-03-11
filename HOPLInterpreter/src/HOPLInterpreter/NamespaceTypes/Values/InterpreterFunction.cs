using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HOPLInterpreter.NamespaceTypes.Values
{
    public class InterpreterFunction : InterpreterValue<IFunction>
	{
		public InterpreterFunction() : base(null) { }

		public InterpreterFunction(Delegate function) : base(ToFunction(function)) { }

		public InterpreterFunction(IFunction function) : base(function) { }

		private static SuppliedFunction ToFunction(Delegate del)
		{
			return new SuppliedFunction(del.GetMethodInfo(), del.Target);
		}
	}
}
