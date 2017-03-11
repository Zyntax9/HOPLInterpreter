using HOPLInterpreter.Faults.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Exceptions
{
	public class RuntimeFaultException : Exception
	{
		public RuntimeFault Fault { get; protected set; }

		public RuntimeFaultException(RuntimeFault fault)
		{
			Fault = fault;
		}
	}
}
