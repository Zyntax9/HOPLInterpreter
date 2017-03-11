using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Antlr4.Runtime.Misc;
using HOPLGrammar;
using Parser = HOPLGrammar.HOPLGrammarParser;
using HOPLInterpreter.NamespaceTypes;
using Antlr4.Runtime;

namespace HOPLInterpreter.Exploration
{
	public class VariableDependencyExplorer : DependencyExplorer
	{
		public VariableDependencyExplorer(ImportAccessTable accessTable, string filename, string namespaceName)
			: base(accessTable, filename, namespaceName)
		{ }

		protected void AddDependency(string id, string @namespace, ParserRuleContext context)
		{
			string actNs = @namespace;

			Import import;
			if (access.TryGetImport(file, @namespace, out import))
				actNs = import.NamespaceName;

			Dependency dep = new Dependency(id, actNs, file, DependencyType.VARIABLE, context);
			if (!Dependencies.Contains(dep))
				Dependencies.Add(dep);
		}

		public HashSet<Dependency> EvaluateDependencies(Parser.GlobalDecContext gDec)
		{
			EnterGlobalDec(gDec);
			return Dependencies;
		}

		public override void EnterGlobalDec([NotNull] Parser.GlobalDecContext context)
		{
			context.varDec().EnterRule(this);
		}

		public override void EnterVarDec([NotNull] Parser.VarDecContext context)
		{
			Parser.ExprContext expr = context.expr();
			if (expr != null)
				expr.EnterRule(this);
		}

		public override void EnterIdentifier([NotNull] Parser.IdentifierContext context)
		{
			Parser.NamespaceContext ns = context.@namespace();
			string id = context.ID().GetText();
			if (ns != null)
				AddDependency(id, ns.GetText(), context);
			else
				AddDependency(id, @namespace, context);
		}
	}
}
