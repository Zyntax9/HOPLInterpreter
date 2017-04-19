using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using Parser = HOPL.Grammar.HOPLGrammarParser;
using HOPL.Grammar;

namespace HOPL.Interpreter.Exploration
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
				if (stat is Parser.ReturnStatContext)
					return true;

				if (stat is Parser.IfStatContext && VisitIfStat((Parser.IfStatContext)stat))
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
