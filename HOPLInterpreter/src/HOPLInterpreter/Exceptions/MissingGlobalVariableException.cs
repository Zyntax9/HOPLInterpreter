using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Exceptions
{
	public class MissingGlobalEntityException : Exception
	{
		public MissingGlobalEntityException(string globalEntityName) : base(globalEntityName) { }
	}
}
