using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace HomeControlInterpreter.Faults.Exploration
{
	public class ExploreFaultCollection : List<ExploreFault>
	{
		public ExploreFaultCollection() : base() { }

		public void Add(ExploreFaultMessage msg, ParserRuleContext context, string file)
		{
			Add(new ExploreFault(msg, context, file));
		}
	}
}
