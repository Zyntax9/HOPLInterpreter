using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Exceptions
{
	public class MissingTriggerException : MissingCallableException
	{
		public MissingTriggerException(string triggerName) : base(triggerName) { }
	}
}
