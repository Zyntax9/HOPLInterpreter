using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPL.Interpreter.Exceptions
{
	public class NamespaceNotFoundException : Exception
	{
		public NamespaceNotFoundException(string @namespace) : base(@namespace) { }
	}
}
