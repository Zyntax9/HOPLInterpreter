using System;

namespace HOPL.Interpreter.Exceptions
{
    public class InternalTypeCheckerException : Exception
	{
		public InternalTypeCheckerException(string message) : base(message) { }
	}
}
