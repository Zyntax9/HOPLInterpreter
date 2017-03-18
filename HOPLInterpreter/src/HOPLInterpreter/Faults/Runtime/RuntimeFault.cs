using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace HOPLInterpreter.Faults.Runtime
{
	public class RuntimeFault : IFault
	{
		public RuntimeFaultMessage Message { get; protected set; }
		public int LineNumber { get; protected set; }
		public int ColumnNumber { get; protected set; }
		public string File { get; protected set; }

		string IFault.Message { get { return Message.message; } }
		public int ID { get { return Message.id; } }

		public string FaultTypeName { get { return "Runtime Fault"; } }

		public RuntimeFault(RuntimeFaultMessage message, int lineNumber, int columnNumber, string file)
		{
			Message = message;
			LineNumber = lineNumber;
			ColumnNumber = columnNumber;
			File = file;
		}

		public RuntimeFault(RuntimeFaultMessage message, ParserRuleContext context, string file)
		{
			Message = message;
			LineNumber = context.start.Line;
			ColumnNumber = context.start.Column;
			File = file;
		}
	}
}
