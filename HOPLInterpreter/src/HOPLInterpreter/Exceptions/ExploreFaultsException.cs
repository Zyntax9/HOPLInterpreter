using HOPLInterpreter.Exploration;
using HOPLInterpreter.Faults.Exploration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Exceptions
{
	public class ExploreFaultsException : Exception
	{
		public ExploreFaultCollection Faults { get; protected set; }

		public ExploreFaultsException(ExploreFaultCollection faults) : base()
		{
			Faults = faults;
		}
	}
}
