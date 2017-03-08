using System;
using HomeControlInterpreter.Faults.Preparation;

namespace HomeControlInterpreter.Exceptions
{
	public class PrepareFaultException : Exception
	{
		public PrepareFault Fault { get; protected set; }

		public PrepareFaultException(PrepareFault fault) : base()
		{
			Fault = fault;
		}

		public PrepareFaultException(PrepareFaultMessage msg, string info) : base()
		{
			Fault = new PrepareFault(msg, info);
		}
	}
}
