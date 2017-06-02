using System.Collections.Generic;
using Antlr4.Runtime.Misc;
using HOPL.Interpreter.Exceptions;
using HOPL.Grammar;
using Parser = HOPL.Grammar.HOPLGrammarParser;
using HOPL.Interpreter.NamespaceTypes;
using HOPL.Interpreter.Errors.Exploration;
using System;

namespace HOPL.Interpreter.Exploration
{
	public class Explorer : HOPLGrammarBaseListener
	{
		public NamespaceSet Namespaces { get; protected set; } = new NamespaceSet();
		// Filename -many-> Import namespace access
		public ImportAccessTable ImportTable { get; protected set; } = new ImportAccessTable();
		public ExploreErrorCollection Errors { get; protected set; } = new ExploreErrorCollection();
		public List<Handler> Handlers { get; protected set; } = new List<Handler>();
        public Dictionary<string, InterpreterType> Required { get; protected set; } = new Dictionary<string, InterpreterType>();

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
					Errors.Add(ExploreErrorMessage.ALIAS_OVERLAP, context, currentFile);
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
			NamespaceString completeNamespace = new NamespaceString(currentNamespace.CompleteName);
			FunctionDependencyExplorer fde = new FunctionDependencyExplorer(ImportTable, currentFile, 
				completeNamespace);
			HashSet<Dependency> dependencies = fde.EvaluateDependencies(functionDec);
			Dependency f = new Dependency(fName, completeNamespace, currentFile, DependencyType.FUNCTION, 
				functionDec, fde.ContainsAwait);

			if (!dependencyMap.ContainsKey(f))
				dependencyMap.Add(f, dependencies);

			ReturnChecker returns = new ReturnChecker();
			if (!returns.Check(functionDec))
				Errors.Add(ExploreErrorMessage.FUNC_RETURN, context, currentFile);

			try
			{
				Function function = new Function(functionDec, currentNamespace, currentFile);
				GlobalEntity functionge = new GlobalEntity(function);
				currentNamespace.AddFunction(function);
				currentNamespace.AddGlobalEntity(functionge);
			}
			catch (DuplicateGlobalEntityException)
			{
				Errors.Add(ExploreErrorMessage.GV_DUPL, context, currentFile);
			}
			catch (DuplicateFunctionException)
			{
				Errors.Add(ExploreErrorMessage.FUNC_DUPL, context, currentFile);
			}
		}

		public override void EnterGlobalDecNamespace([NotNull] Parser.GlobalDecNamespaceContext context)
		{
			Parser.GlobalDecContext globalVarDec = context.globalDec();
			if (globalVarDec.REQUIRED_KW() != null)
				ContainedRequired = true;

            NamespaceString completeNamespace = new NamespaceString(currentNamespace.CompleteName);
			string gvName = globalVarDec.varDec().ID().GetText();
			VariableDependencyExplorer vde = new VariableDependencyExplorer(ImportTable, currentFile, completeNamespace);
			HashSet<Dependency> dependencies = vde.EvaluateDependencies(globalVarDec);
			Dependency dep = new Dependency(gvName, completeNamespace, currentFile, DependencyType.VARIABLE, 
				globalVarDec, vde.ContainsAwait);

			if (!dependencyMap.ContainsKey(dep))
				dependencyMap.Add(dep, dependencies);

            IGlobalEntity gv = null;
            try
			{
				gv = currentNamespace.AddGlobalEntity(globalVarDec);
			}
			catch (DuplicateGlobalEntityException)
			{
				Errors.Add(ExploreErrorMessage.GV_DUPL, context, currentFile);
                return;
			}

            if (gv.Required)
                Required.Add(completeNamespace + "." + gvName, gv.Type);
		}
	}
}
