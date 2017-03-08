using HomeControlInterpreter.TypeCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HomeControlInterpreter.Faults.TypeCheck;

namespace HomeControlInterpreter.Exceptions
{
	public class TypeFaultsException : Exception
	{
		public TypeFaultCollection Faults { get; protected set; }

		public TypeFaultsException(TypeFaultCollection faults) : base()
		{
			Faults = faults;
		}
	}
}
