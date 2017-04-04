using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime;

namespace HOPL.Interpreter.Errors.Exploration
{
	public class ExploreErrorCollection : List<ExploreError>
	{
		public ExploreErrorCollection() : base() { }

		public void Add(ExploreErrorMessage msg, ParserRuleContext context, string file)
		{
			Add(new ExploreError(msg, context, file));
		}
	}
}
