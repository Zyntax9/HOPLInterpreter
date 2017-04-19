using System.Collections.Generic;
using Parser = HOPL.Grammar.HOPLGrammarParser;
using HOPL.Grammar;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime;

namespace HOPL.Interpreter.Exploration
{
	public enum DependencyType
	{
		VARIABLE,
		FUNCTION
	}

	public class Dependency
	{
		public string ID { get; set; }
		public string Namespace { get; set; }
		public string File { get; set; }
		public DependencyType Type { get; set; }
		public ParserRuleContext Context { get; set; }
		public bool ContainsAwait { get; set; }

		public Dependency(string id, string @namespace, string file, DependencyType type,
			ParserRuleContext context, bool containsAwait = false)
		{
			ID = id;
			Namespace = @namespace;
			File = file;
			Type = type;
			Context = context;
			ContainsAwait = containsAwait;
		}

		public override bool Equals(object obj)
		{
			if (obj is Dependency)
			{
				Dependency d = (Dependency)obj;
				return ID == d.ID && Namespace == d.Namespace;
			}
			return base.Equals(obj);
		}

		public override string ToString()
		{
			return Namespace + "." + ID;
		}

		public override int GetHashCode()
		{
			return ToString().GetHashCode();
		}

		public static bool operator ==(Dependency d1, Dependency d2)
		{
			return d1.Equals(d2);
		}

		public static bool operator !=(Dependency d1, Dependency d2)
		{
			return !d1.Equals(d2);
		}
	}

	public class DependencyExplorer : HOPLGrammarBaseListener
	{
		protected ImportAccessTable access;
		protected string file;
		protected string @namespace;

		public bool ContainsAwait { get; set; } = false;

		public HashSet<Dependency> Dependencies { get; protected set; } = new HashSet<Dependency>();

		public DependencyExplorer(ImportAccessTable accessTable, string filename, string namespaceName)
		{
			access = accessTable;
			file = filename;
			@namespace = namespaceName;
		}

		public override void EnterParanExpr([NotNull] Parser.ParanExprContext context)
		{
			context.expr().EnterRule(this);
		}

		public override void EnterMultExpr([NotNull] Parser.MultExprContext context)
		{
			foreach (Parser.ExprContext expr in context.expr())
				expr.EnterRule(this);
		}

		public override void EnterAddiExpr([NotNull] Parser.AddiExprContext context)
		{
			foreach (Parser.ExprContext expr in context.expr())
				expr.EnterRule(this);
		}

		public override void EnterAndExpr([NotNull] Parser.AndExprContext context)
		{
			foreach (Parser.ExprContext expr in context.expr())
				expr.EnterRule(this);
		}

		public override void EnterOrExpr([NotNull] Parser.OrExprContext context)
		{
			foreach (Parser.ExprContext expr in context.expr())
				expr.EnterRule(this);
		}

		public override void EnterCompExpr([NotNull] Parser.CompExprContext context)
		{
			foreach (Parser.ExprContext expr in context.expr())
				expr.EnterRule(this);
		}

		public override void EnterNegExpr([NotNull] Parser.NegExprContext context)
		{
			context.expr().EnterRule(this);
		}

		public override void EnterNotExpr([NotNull] Parser.NotExprContext context)
		{
			context.expr().EnterRule(this);
		}

		public override void EnterIndexExpr([NotNull] Parser.IndexExprContext context)
		{
			foreach (Parser.ExprContext expr in context.expr())
				expr.EnterRule(this);
		}

		public override void EnterVarExpr([NotNull] Parser.VarExprContext context)
		{
			context.identifier().EnterRule(this);
		}

		public override void EnterCallExpr([NotNull] Parser.CallExprContext context)
		{
			context.call().EnterRule(this);
		}

		public override void EnterCall([NotNull] Parser.CallContext context)
		{
			context.identifier().EnterRule(this);

			foreach (Parser.ExprContext expr in context.expr())
				expr.EnterRule(this);
		}

		public override void EnterListExpr([NotNull] Parser.ListExprContext context)
		{
			foreach (Parser.ExprContext expr in context.expr())
				expr.EnterRule(this);
		}

		public override void EnterTupleExpr([NotNull] Parser.TupleExprContext context)
		{
			foreach (Parser.ExprContext expr in context.expr())
				expr.EnterRule(this);
		}

		public override void EnterConcatExpr([NotNull] Parser.ConcatExprContext context)
		{
			foreach (Parser.ExprContext expr in context.expr())
				expr.EnterRule(this);
		}

		public override void EnterAwaitExpr([NotNull] Parser.AwaitExprContext context)
		{
			context.await().EnterRule(this);
		}

		public override void EnterAwait([NotNull] Parser.AwaitContext context)
		{
			ContainsAwait = true;
			context.expr().EnterRule(this);
		}
	}
}
