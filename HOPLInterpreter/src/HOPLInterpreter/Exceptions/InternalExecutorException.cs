using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Exceptions
{
	public class InternalTypeCheckerException : Exception
	{
		public InternalTypeCheckerException(string message) : base(message) { }
	}
}
