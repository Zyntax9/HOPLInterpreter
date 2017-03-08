using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using HOPLGrammar;
using HomeControlInterpreter.Exceptions;
using Parser = HOPLGrammar.HOPLGrammarParser;
using HomeControlInterpreter.NamespaceTypes;
using HomeControlInterpreter.Faults.Exploration;
using System;

namespace HomeControlInterpreter.Exploration
{
	public class Explorer : HOPLGrammarBaseListener
	{
		public NamespaceSet Namespaces { get; protected set; } = new NamespaceSet();
		// Filename -many-> Import namespace access
		public ImportAccessTable ImportTable { get; protected set; } = new ImportAccessTable();
		public ExploreFaultCollection Faults { get; protected set; } = new ExploreFaultCollection();
		public List<Handler> Handlers { get; protected set; } = new List<Handler>();

		public bool ContainedRequired { get; protected set; } = false;
		private string currentFile;
		public string CurrentFile
		{
			get
			{
				return currentFile;
			}
			set
			{
				currentFile = value;
				if (currentFile != null && !ImportTable.ContainsKey(currentFile))
					ImportTable.Add(currentFile);
				ContainedRequired = false;
			}
		}

		private Namespace currentNamespace;

		private Dictionary<Dependency, HashSet<Dependency>> dependencyMap =
			new Dictionary<Dependency, HashSet<Dependency>>();

		public Explorer(string currentFile = null)
		{
			CurrentFile = currentFile;
		}

		public Explorer(ref NamespaceSet namespaceSet, string currentFile = null) : this(currentFile)
		{
			Namespaces = namespaceSet;
		}

		public Queue<Dependency> DepencencyEvaluationOrder()
		{
			DependencyGraph graph = new DependencyGraph(dependencyMap);
			return graph.EvaluationOrder();
		}

		public override void EnterCompileUnit([NotNull] Parser.CompileUnitContext context)
		{
			Parser.ImportNamespaceContext[] importNs = context.importNamespace();
			foreach (Parser.ImportNamespaceContext importN in importNs)
			{
				importN.EnterRule(this);
			}

			Parser.NamespaceDecContext[] namespaceDecs = context.namespaceDec();
			foreach (Parser.NamespaceDecContext namespaceDec in namespaceDecs)
			{
				currentNamespace = Namespaces.Add(namespaceDec);

				if (currentFile != null)
				{
					// Implicit import of own namespace
					string ns = namespaceDec.@namespace().GetText();
					ImportTable[currentFile].Add(new Import(ns));
				}

				namespaceDec.namespaceBody().EnterRule(this);
			}
		}

		public override void EnterImportNamespace([NotNull] Parser.ImportNamespaceContext context)
		{
			Parser.NamespaceContext[] nsContexts = context.@namespace();
			string importNamespace = nsContexts[0].GetText();
			string importAlias = nsContexts.Length > 1 ? nsContexts[1].GetText() : null;
			Import importInfo = new Import(importNamespace, importAlias);
			if (currentFile != null)
				if (!ImportTable[currentFile].Add(importInfo))
					Faults.Add(ExploreFaultMessage.ALIAS_OVERLAP, context, currentFile);
		}

		public override void EnterNamespaceBody([NotNull] Parser.NamespaceBodyContext context)
		{
			foreach (Parser.NamespacePartContext part in context.namespacePart())
				part.EnterRule(this);
		}

		public override void EnterHandlerDecNamespace([NotNull] Parser.HandlerDecNamespaceContext context)
		{
			Parser.HandlerDecContext handlerDec = context.handlerDec();
			Handler handler = new Handler(handlerDec, currentNamespace, currentFile, Namespaces, ImportTable);
			Handlers.Add(handler);
		}

		public override void EnterFunctionDecNamespace([NotNull] Parser.FunctionDecNamespaceContext context)
		{
			Parser.FunctionDecContext functionDec = context.functionDec();

			string fName = functionDec.ID().GetText();
			string completeNamespace = currentNamespace.CompleteName;
			FunctionDependencyExplorer fde = new FunctionDependencyExplorer(ImportTable, currentFile, 
				completeNamespace);
			HashSet<Dependency> dependencies = fde.EvaluateDependencies(functionDec);
			Dependency f = new Dependency(fName, completeNamespace, currentFile, DependencyType.FUNCTION, 
				functionDec, fde.ContainsAwait);

			if (!dependencyMap.ContainsKey(f))
				dependencyMap.Add(f, dependencies);

			ReturnChecker returns = new ReturnChecker();
			if (!returns.Check(functionDec))
				Faults.Add(ExploreFaultMessage.FUNC_RETURN, context, currentFile);

			try
			{
				Function function = new Function(functionDec, currentNamespace, currentFile);
				GlobalEntity functionge = new GlobalEntity(function);
				currentNamespace.AddFunction(function);
				currentNamespace.AddGlobalEntity(functionge);
			}
			catch (DuplicateGlobalEntityException)
			{
				Faults.Add(ExploreFaultMessage.GV_DUPL, context, currentFile);
			}
			catch (DuplicateFunctionException)
			{
				Faults.Add(ExploreFaultMessage.FUNC_DUPL, context, currentFile);
			}
		}

		public override void EnterGlobalDecNamespace([NotNull] Parser.GlobalDecNamespaceContext context)
		{
			Parser.GlobalDecContext globalVarDec = context.globalDec();
			if (globalVarDec.REQUIRED_KW() != null)
				ContainedRequired = true;

			string completeNamespace = currentNamespace.CompleteName;
			string gvName = globalVarDec.varDec().ID().GetText();
			VariableDependencyExplorer vde = new VariableDependencyExplorer(ImportTable, currentFile, completeNamespace);
			HashSet<Dependency> dependencies = vde.EvaluateDependencies(globalVarDec);
			Dependency gv = new Dependency(gvName, completeNamespace, currentFile, DependencyType.VARIABLE, 
				globalVarDec, vde.ContainsAwait);

			if (!dependencyMap.ContainsKey(gv))
				dependencyMap.Add(gv, dependencies);

			try
			{
				currentNamespace.AddGlobalEntity(globalVarDec);
			}
			catch (DuplicateGlobalEntityException)
			{
				Faults.Add(ExploreFaultMessage.GV_DUPL, context, currentFile);
			}
		}
	}
}
