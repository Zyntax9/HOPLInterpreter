using System;
using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using Parser = HOPL.Grammar.HOPLGrammarParser;
using HOPL.Interpreter.NamespaceTypes;
using Antlr4.Runtime;

namespace HOPL.Interpreter.Exploration
{
	public class FunctionDependencyExplorer : DependencyExplorer
	{
		private Stack<HashSet<string>> scope = new Stack<HashSet<string>>();

		public string FunctionName { get; protected set; }

		public FunctionDependencyExplorer(ImportAccessTable accessTable, string filename, string namespaceName)
			: base(accessTable, filename, namespaceName)
		{ }
		
		protected void AddDependency(string id, string @namespace, ParserRuleContext context)
		{
			string actNs = @namespace;

			Import import;
			if (access.TryGetImport(file, @namespace, out import))
				actNs = import.NamespaceName;

			Dependency dep = new Dependency(id, actNs, file, DependencyType.FUNCTION, context);
			if (!Dependencies.Contains(dep))
				Dependencies.Add(dep);
		}

		public HashSet<Dependency> EvaluateDependencies(Parser.FunctionDecContext funcDec)
		{
			EnterFunctionDec(funcDec);
			return Dependencies;
		}

		private bool IsInScope(string id)
		{
			foreach (HashSet<string> set in scope)
				if (set.Contains(id))
					return true;
			return false;
		}

		public override void EnterFunctionDec([NotNull] Parser.FunctionDecContext context)
		{
			FunctionName = context.ID().GetText();

			HashSet<string> args = new HashSet<string>();
			foreach (Parser.ArgContext arg in context.args().arg())
				args.Add(arg.ID().GetText());

			EnterBody(context.body(), args);
		}

		public override void EnterIdentifier([NotNull] Parser.IdentifierContext context)
		{
			Parser.NamespaceContext ns = context.@namespace();
			string id = context.ID().GetText();
			if (ns != null)
				AddDependency(id, ns.GetText(), context);
			else if (!IsInScope(id))
				AddDependency(id, @namespace, context);
		}

		public override void EnterBody([NotNull] Parser.BodyContext context)
		{
			EnterBody(context, new HashSet<string>());
		}

		public void EnterBody([NotNull] Parser.BodyContext context, HashSet<string> initScope)
		{
			scope.Push(new HashSet<string>());
			foreach (Parser.StatContext stat in context.stat())
				stat.EnterRule(this);
			scope.Pop();
		}

		public override void EnterAssignStat([NotNull] Parser.AssignStatContext context)
		{
			context.assign().EnterRule(this);
		}

		public override void EnterAssign([NotNull] Parser.AssignContext context)
		{
			context.identifier().EnterRule(this);
			context.expr().EnterRule(this);
		}

		public override void EnterDecStat([NotNull] Parser.DecStatContext context)
		{
			context.varDec().EnterRule(this);
		}

		public override void EnterVarDec([NotNull] Parser.VarDecContext context)
		{
			Parser.ExprContext expr = context.expr();
			if (expr != null)
				expr.EnterRule(this);

			string id = context.ID().GetText();
			scope.Peek().Add(id);
		}

		public override void EnterExprStat([NotNull] Parser.ExprStatContext context)
		{
			context.expr().EnterRule(this);
		}

		public override void EnterReturnStat([NotNull] Parser.ReturnStatContext context)
		{
			context.@return().EnterRule(this);
		}

		public override void EnterReturn([NotNull] Parser.ReturnContext context)
		{
			Parser.ExprContext expr = context.expr();
			if (expr != null)
				expr.EnterRule(this);
		}

		public override void EnterUnpackStat([NotNull] Parser.UnpackStatContext context)
		{
			context.unpack().EnterRule(this);
		}

		public override void EnterUnpack([NotNull] Parser.UnpackContext context)
		{
			context.expr().EnterRule(this);

			foreach (Parser.UnpackedContext unpackable in context.unpacked())
				unpackable.EnterRule(this);
		}

		public override void EnterIdUnpacked([NotNull] Parser.IdUnpackedContext context)
		{
			context.identifier().EnterRule(this);
		}

		public override void EnterDecUnpacked([NotNull] Parser.DecUnpackedContext context)
		{
			scope.Peek().Add(context.ID().GetText());
		}

		public override void EnterIfStat([NotNull] Parser.IfStatContext context)
		{
			context.@if().EnterRule(this);

			foreach (Parser.ElseIfContext elif in context.elseIf())
				elif.EnterRule(this);

			Parser.ElseContext el = context.@else();
			if (el != null)
				el.EnterRule(this);
		}

		public override void EnterIf([NotNull] Parser.IfContext context)
		{
			context.expr().EnterRule(this);
			context.body().EnterRule(this);
		}

		public override void EnterElseIf([NotNull] Parser.ElseIfContext context)
		{
			context.expr().EnterRule(this);
			context.body().EnterRule(this);
		}

		public override void EnterElse([NotNull] Parser.ElseContext context)
		{
			context.body().EnterRule(this);
		}

		public override void EnterWhileStat([NotNull] Parser.WhileStatContext context)
		{
			context.@while().EnterRule(this);
		}

		public override void EnterWhile([NotNull] Parser.WhileContext context)
		{
			context.expr().EnterRule(this);
			context.body().EnterRule(this);
		}

		public override void EnterForStat([NotNull] Parser.ForStatContext context)
		{
			context.@for().EnterRule(this);
		}

		public override void EnterFor([NotNull] Parser.ForContext context)
		{
			scope.Push(new HashSet<string>());

			if (context.declare != null)
				context.declare.EnterRule(this);

			context.predicate.EnterRule(this);

			if (context.reeval != null)
				context.reeval.EnterRule(this);

			context.body().EnterRule(this);

			scope.Pop();
		}

		public override void EnterForeachStat([NotNull] Parser.ForeachStatContext context)
		{
			context.@foreach().EnterRule(this);
		}

		public override void EnterForeach([NotNull] Parser.ForeachContext context)
		{
			context.expr().EnterRule(this);

			HashSet<string> set = new HashSet<string>();
			set.Add(context.ID().GetText());

			EnterBody(context.body(), set);
		}
	}
}
