using System;

namespace HOPLInterpreter.Exceptions
{
	public class RecursiveVariableDependencyException : Exception
	{
		public RecursiveVariableDependencyException(string msg) : base(msg) { }
	}
}
