using HomeControlInterpreter.Faults.Parsing;
using System;
using System.Collections.Generic;

namespace HomeControlInterpreter.Exceptions
{
	public class ParsingFaultsException : Exception
	{
		public IEnumerable<ParsingFault> Faults { get; protected set; }

		public ParsingFaultsException(IEnumerable<ParsingFault> faults) : base()
		{
			Faults = faults;
		}
	}
}
