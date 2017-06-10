using System;

namespace HOPL.Interpreter.Exceptions
{
    public class InternalExecutorException : Exception
	{
		public InternalExecutorException(string message) : base(message) { }
	}
}
