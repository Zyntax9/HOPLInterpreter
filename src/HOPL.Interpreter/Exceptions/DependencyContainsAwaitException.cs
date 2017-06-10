using System;

namespace HOPL.Interpreter.Exceptions
{
    public class DependencyContainsAwaitException : Exception
    {
		public DependencyContainsAwaitException(string msg) : base(msg) { }
    }
}
