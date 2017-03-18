using System;
using HOPLInterpreter.Faults.Preparation;
using System.Collections.Generic;
using HOPLInterpreter.Faults;

namespace HOPLInterpreter.Exceptions
{
	public class PrepareFaultException : FaultException
	{
		private PrepareFault fault;
		public override IEnumerable<IFault> Faults { get { return new PrepareFault[] { fault }; } }

		public override string FaultName { get { return "Prepare Faults"; } }

		public PrepareFaultException(PrepareFault fault) : base()
		{
			this.fault = fault;
		}

		public PrepareFaultException(PrepareFaultMessage msg, string info) : base()
		{
			this.fault = new PrepareFault(msg, info);
		}
	}
}
