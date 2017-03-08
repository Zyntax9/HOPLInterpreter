using System;

namespace HomeControlInterpreter.Exceptions
{
	public class RecursiveVariableDependencyException : Exception
	{
		public RecursiveVariableDependencyException(string msg) : base(msg) { }
	}
}
