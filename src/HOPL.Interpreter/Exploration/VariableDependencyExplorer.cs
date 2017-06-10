using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using Parser = HOPL.Grammar.HOPLGrammarParser;
using HOPL.Interpreter.NamespaceTypes;
using Antlr4.Runtime;

namespace HOPL.Interpreter.Exploration
{
    public class VariableDependencyExplorer : DependencyExplorer
	{
		public VariableDependencyExplorer(ImportAccessTable accessTable, string filename, NamespaceString @namespace)
			: base(accessTable, filename, @namespace)
		{ }

		protected void AddDependency(string id, NamespaceString @namespace, ParserRuleContext context)
		{
			NamespaceString actNs = @namespace;

			Import import;
            NamespaceString remaining;
			if (access.TryGetImport(file, @namespace, out import, out remaining))
				actNs = import.NamespaceName + remaining;

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
				AddDependency(id, new NamespaceString(ns.GetText()), context);
			else
				AddDependency(id, @namespace, context);
		}
	}
}
