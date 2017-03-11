using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Exceptions
{
	public class NamespaceNotFoundException : Exception
	{
		public NamespaceNotFoundException(string @namespace) : base(@namespace) { }
	}
}
