using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPL.Interpreter.Exceptions
{
	public class InternalExecutorException : Exception
	{
		public InternalExecutorException(string message) : base(message) { }
	}
}
