using HOPLInterpreter.Exploration;
using HOPLInterpreter.Faults.Exploration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HOPLInterpreter.Faults;

namespace HOPLInterpreter.Exceptions
{
	public class ExploreFaultsException : FaultException
	{
		private ExploreFaultCollection faults;
		public override IEnumerable<IFault> Faults { get { return faults; } }

		public override string FaultName { get { return "Explore Faults"; } }

		public ExploreFaultsException(ExploreFaultCollection faults) : base()
		{
			this.faults = faults;
		}
	}
}
