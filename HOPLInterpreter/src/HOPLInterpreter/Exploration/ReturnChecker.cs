using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using HOPLGrammar;
using Parser = HOPLGrammar.HOPLGrammarParser;

namespace HOPLInterpreter.Exploration
{
	public class ReturnChecker : HOPLGrammarBaseVisitor<bool>
	{
		public bool Check(Parser.FunctionDecContext context)
		{
			return VisitBody(context.body());
		}

		public override bool VisitBody([NotNull] Parser.BodyContext context)
		{
			Parser.StatContext[] stats = context.stat();

			foreach (Parser.StatContext stat in stats)
			{
				if (stat.GetType() == typeof(Parser.ReturnStatContext))
					return true;

				if (stat.GetType() == typeof(Parser.IfStatContext) &&
					VisitIfStat((Parser.IfStatContext)stat))
					return true;
			}

			return false;
		}

		public override bool VisitIfStat([NotNull] Parser.IfStatContext context)
		{
			if (!VisitBody(context.@if().body()))
				return false;

			foreach (Parser.ElseIfContext elif in context.elseIf())
				if (!VisitBody(elif.body()))
					return false;

			Parser.ElseContext el = context.@else();
			if (el == null) // Must have else to ensure return
				return false;

			return VisitBody(el.body());
		}
	}
}
