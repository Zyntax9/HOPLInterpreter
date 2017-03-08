using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.NamespaceTypes.Values
{
    public class InterpreterFunction : InterpreterValue<IFunction>
	{
		public InterpreterFunction() : base(null) { }

		public InterpreterFunction(IFunction function) : base(function) { }
	}
}
