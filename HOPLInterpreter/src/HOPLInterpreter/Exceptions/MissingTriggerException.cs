using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Exceptions
{
	public class MissingTriggerException : MissingCallableException
	{
		public MissingTriggerException(string triggerName) : base(triggerName) { }
	}
}
