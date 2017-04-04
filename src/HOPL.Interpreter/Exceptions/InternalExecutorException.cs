using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPL.Interpreter.Exceptions
{
	public class InternalTypeCheckerException : Exception
	{
		public InternalTypeCheckerException(string message) : base(message) { }
	}
}
