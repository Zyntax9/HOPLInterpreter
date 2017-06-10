using System;

namespace HOPL.Interpreter.Exceptions
{
    public class MissingCallableException : Exception
	{
		public MissingCallableException(string callableName) : base(callableName) { }
	}
}
