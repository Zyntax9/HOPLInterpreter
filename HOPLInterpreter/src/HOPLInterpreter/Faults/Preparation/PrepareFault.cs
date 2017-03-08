using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Faults.Preparation
{
	public class PrepareFault
	{
		public PrepareFaultMessage Message { get; protected set; }
		public string Info { get; protected set; }

		public PrepareFault(PrepareFaultMessage message, string info)
		{
			Message = message;
			Info = info;
		}
	}
}
