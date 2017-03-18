using HOPLInterpreter.Faults;
using HOPLInterpreter.Faults.Parsing;
using System;
using System.Collections.Generic;

namespace HOPLInterpreter.Exceptions
{
	public class ParsingFaultsException : FaultException
	{
		private IEnumerable<ParsingFault> faults;
		public override IEnumerable<IFault> Faults { get { return faults; } }

		public override string FaultName { get { return "Parsing Faults"; } }

		public ParsingFaultsException(IEnumerable<ParsingFault> faults) : base()
		{
			this.faults = faults;
		}
	}
}
