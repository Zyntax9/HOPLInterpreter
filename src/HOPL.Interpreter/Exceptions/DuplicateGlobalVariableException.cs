using System;

namespace HOPL.Interpreter.Exceptions
{
    public class DuplicateGlobalEntityException : Exception
	{
		public DuplicateGlobalEntityException(string globalEntityName) : base(globalEntityName) { }
	}
}
