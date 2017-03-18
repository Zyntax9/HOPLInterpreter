using HOPLInterpreter.Faults;
using HOPLInterpreter.Faults.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Exceptions
{
	public class RuntimeFaultException : FaultException
	{
		private RuntimeFault fault;
		public override IEnumerable<IFault> Faults { get { return new RuntimeFault[] { fault }; } }

		public override string FaultName { get { return "Runtime Faults"; } }

		public RuntimeFaultException(RuntimeFault fault)
		{
			this.fault = fault;
		}
	}
}
