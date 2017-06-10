using System;

namespace HOPL.Interpreter.Exceptions
{
    public class DuplicateFunctionException : Exception
	{
		public DuplicateFunctionException(string functionName) : base(functionName) { }
	}
}
