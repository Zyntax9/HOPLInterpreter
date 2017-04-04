using System;

namespace HOPL.Interpreter.Exceptions
{
	public class RecursiveVariableDependencyException : Exception
	{
		public RecursiveVariableDependencyException(string msg) : base(msg) { }
	}
}
