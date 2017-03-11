using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace HOPLInterpreter.Faults.TypeCheck
{
	public class TypeFaultCollection : List<TypeFault>
	{
		public TypeFaultCollection() : base() { }

		public void Add(TypeFaultMessage msg, ParserRuleContext context, string file)
		{
			Add(new TypeFault(msg, context, file));
		}
	}
}
