using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Exceptions
{
	public class DuplicateVariableException : Exception
	{
		public DuplicateVariableException(string variableName) : base(variableName) { }
	}
}
