using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Exceptions
{
	public class MissingFunctionException : MissingCallableException
	{
		public MissingFunctionException(string functionName) : base(functionName) { }
	}
}
