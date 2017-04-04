using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPL.Interpreter.Exceptions
{
	public class DuplicateFunctionException : Exception
	{
		public DuplicateFunctionException(string functionName) : base(functionName) { }
	}
}
