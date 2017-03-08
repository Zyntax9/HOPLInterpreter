using HomeControlInterpreter.Exploration;
using HomeControlInterpreter.Faults.Exploration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Exceptions
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
