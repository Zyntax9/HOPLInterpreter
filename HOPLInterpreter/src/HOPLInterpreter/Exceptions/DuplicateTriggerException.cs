using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Exceptions
{
	public class DuplicateTriggerException : Exception
	{
		public DuplicateTriggerException(string triggerName) : base(triggerName) { }
	}
}
