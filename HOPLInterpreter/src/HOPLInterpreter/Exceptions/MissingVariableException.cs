using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Exceptions
{
	public class MissingVariableException : Exception
	{
		public MissingVariableException(string variableName) : base(variableName) { }
	}
}
