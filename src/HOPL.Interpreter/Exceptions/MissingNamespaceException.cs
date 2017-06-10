using System;

namespace HOPL.Interpreter.Exceptions
{
    public class MissingNamespaceException : Exception
	{
		public MissingNamespaceException(string namespaceName) : base(namespaceName) { }
	}
}
