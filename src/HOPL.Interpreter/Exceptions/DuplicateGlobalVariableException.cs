using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPL.Interpreter.Exceptions
{
	public class DuplicateGlobalEntityException : Exception
	{
		public DuplicateGlobalEntityException(string globalEntityName) : base(globalEntityName) { }
	}
}
