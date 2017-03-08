using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Exceptions
{
	public class DuplicateGlobalEntityException : Exception
	{
		public DuplicateGlobalEntityException(string globalEntityName) : base(globalEntityName) { }
	}
}
