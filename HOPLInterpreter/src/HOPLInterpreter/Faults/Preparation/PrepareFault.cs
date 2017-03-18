using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Faults.Preparation
{
	public class PrepareFault : IFault
	{
		public PrepareFaultMessage Message { get; protected set; }
		public string Info { get; protected set; }

		string IFault.Message { get { return Message.message; } }
		public int ID { get { return Message.id; } }
		public int LineNumber { get { return 0; } }
		public int ColumnNumber { get { return 0; } }

		public string FaultTypeName { get { return "Preparation Fault"; } }

		public PrepareFault(PrepareFaultMessage message, string info)
		{
			Message = message;
			Info = info;
		}
	}
}
