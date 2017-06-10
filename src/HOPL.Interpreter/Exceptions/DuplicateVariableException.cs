using System;

namespace HOPL.Interpreter.Exceptions
{
    public class DuplicateVariableException : Exception
	{
		public DuplicateVariableException(string variableName) : base(variableName) { }
	}
}
