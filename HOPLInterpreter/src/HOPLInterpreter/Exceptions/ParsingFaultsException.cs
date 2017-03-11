using HOPLInterpreter.Faults.Parsing;
using System;
using System.Collections.Generic;

namespace HOPLInterpreter.Exceptions
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
