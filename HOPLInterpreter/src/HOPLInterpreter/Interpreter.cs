using System.Collections.Generic;
using Antlr4.Runtime;
using System.IO;
using Lexer = HOPLGrammar.HOPLGrammarLexer;
using Parser = HOPLGrammar.HOPLGrammarParser;
using HOPLInterpreter.NamespaceTypes;
using HOPLInterpreter.NamespaceMapping;
using HOPLInterpreter.Exploration;
using HOPLInterpreter.TypeCheck;
using HOPLInterpreter.Exceptions;
using HOPLInterpreter.Faults.Parsing;
using HOPLInterpreter.Faults.Preparation;
using HOPLInterpreter.Interpretation.ThreadPool;
using HOPLInterpreter.Interpretation;
using HOPLInterpreter.NamespaceTypes.Values;
using Api = HomeControlInterpreterInterface;

namespace HOPLInterpreter
{
	public class Interpreter
	{
		public Interpreter()
		{ }

		public static IThreadPool Run(InterpretationContext context, 
			Dictionary<string, object> setupValues, int? threads = null)
		{
			Setup(context.Namespaces, context.AccessTable, context.EvaluationOrder, setupValues);

			IThreadPool pool;
			if(threads.HasValue)
				pool = new StaticThreadPool(threads.Value);
			else
				pool = new DynamicThreadPool();

			InitializeHandlers(context.Handlers, pool);

			return pool;
		}

		public static IThreadPool Run(InterpretationContext context,
			Dictionary<string, object> setupValues, IThreadPool pool)
		{
			Setup(context.Namespaces, context.AccessTable, context.EvaluationOrder, setupValues);
			InitializeHandlers(context.Handlers, pool);
			return pool;
		}

		private static void Setup(NamespaceSet namespaces, ImportAccessTable accessTable,
			Queue<Dependency> evalOrder, Dictionary<string, object> setupValues)
		{
			foreach(Dependency dep in evalOrder)
			{
				Namespace ns = namespaces.Get(dep.Namespace);
				Parser.GlobalDecContext context = (Parser.GlobalDecContext)dep.Context;
				if (context.REQUIRED_KW() != null)
				{
					string varName = context.varDec().ID().GetText();
					string fullName = dep.Namespace + "." + varName;
					IGlobalEntity ge = ns.GetGlobalEntity(varName);
					ge.Value = InterpreterValue.FromNative(setupValues[fullName]);
				}
				else
				{
					Executor executor = new Executor(ns, namespaces, dep.File, accessTable);
					executor.VisitGlobalDec(context);
				}
			}
		}

		private static void InitializeHandlers(List<Handler> handlers, IThreadPool pool)
		{
			foreach(Handler handler in handlers)
			{
				Executor executor = new Executor(handler.Namespace, handler.Namespaces, 
					handler.File, handler.AccessTable);
				InterpreterValue expr = executor.VisitExpr(handler.Context.expr());
				Api.SuppliedTrigger trigger = (Api.SuppliedTrigger)expr.Value;
				trigger.Subscribe(handler.OnTrigger);
				handler.HandlerTriggered += pool.QueueHandler;
			}
		}
		
		public static InterpretationContext PrepareFile(string file, ISet<string> importPaths, 
			NamespaceSet namespaces)
		{
			List<ParsingFault> parserFaults;
			Parser.CompileUnitContext cUnit = GetParseTree(file, out parserFaults);

			if (parserFaults.Count > 0)
				throw new ParsingFaultsException(parserFaults);

			HashSet<string> exploredFiles = new HashSet<string>();
			Explorer explorer = new Explorer(ref namespaces);
			explorer.CurrentFile = file;
			Dictionary<string, ISet<string>> namespaceFileMap = NamespaceMapper.MapNamespaces(importPaths);

			explorer.EnterCompileUnit(cUnit);
			exploredFiles.Add(file);

			if (explorer.Faults.Count > 0)
				throw new ExploreFaultsException(explorer.Faults);

			HashSet<Import> topImports = explorer.ImportTable[file];
			foreach (Import import in topImports)
			{
				ISet<string> importFiles;
				if (namespaceFileMap.TryGetValue(import.NamespaceName, out importFiles))
				{
					foreach (string importFile in importFiles)
					{
						if (!exploredFiles.Contains(importFile))
						{
							PrepareImport(importFile, importPaths, namespaces,
								exploredFiles, explorer, namespaceFileMap);
						}
					}
				}

				if (!namespaces.ContainsNamespace(import.NamespaceName))
					throw new PrepareFaultException(PrepareFaultMessage.NS_NOTFOUND, import.NamespaceName);
			}

			TypeChecker typeCheck = new TypeChecker(file, namespaces, explorer.ImportTable);
			typeCheck.VisitCompileUnit(cUnit);

			if (typeCheck.Faults.Count > 0)
				throw new TypeFaultsException(typeCheck.Faults);

			Queue<Dependency> evalOrder = null;
			try
			{
				evalOrder = explorer.DepencencyEvaluationOrder();
			}
			catch (RecursiveVariableDependencyException e)
			{
				throw new PrepareFaultException(PrepareFaultMessage.DEP_REC, e.Message);
			}
			catch (DependencyContainsAwaitException e)
			{
				throw new PrepareFaultException(PrepareFaultMessage.DEP_AWAIT, e.Message);
			}

			return new InterpretationContext(explorer, evalOrder);
		}

		private static void PrepareImport(string file,
		   ISet<string> importPaths,
		   NamespaceSet namespaces,
		   HashSet<string> exploredFiles,
		   Explorer explorer,
		   Dictionary<string, ISet<string>> namespaceFileMap)
		{
			List<ParsingFault> parserFaults;
			Parser.CompileUnitContext cUnit = GetParseTree(file, out parserFaults);

			if (parserFaults.Count > 0)
				throw new ParsingFaultsException(parserFaults);

			explorer.CurrentFile = file;
			explorer.EnterCompileUnit(cUnit);
			exploredFiles.Add(file);

			if (explorer.Faults.Count > 0)
				throw new ExploreFaultsException(explorer.Faults);

			if (explorer.ContainedRequired)
				throw new PrepareFaultException(PrepareFaultMessage.NS_REQ, file);

			HashSet<Import> topImports = explorer.ImportTable[file];
			foreach (Import import in topImports)
			{
				ISet<string> importFiles;
				if (namespaceFileMap.TryGetValue(import.NamespaceName, out importFiles))
				{
					foreach (string importFile in importFiles)
					{
						if (!exploredFiles.Contains(importFile))
						{
							PrepareImport(importFile, importPaths, namespaces,
								exploredFiles, explorer, namespaceFileMap);
						}
					}
				}

				if (!namespaces.ContainsNamespace(import.NamespaceName))
					throw new PrepareFaultException(PrepareFaultMessage.NS_NOTFOUND, import.NamespaceName);
			}

			TypeChecker typeCheck = new TypeChecker(file, namespaces, explorer.ImportTable);
			typeCheck.VisitCompileUnit(cUnit);

			if (typeCheck.Faults.Count > 0)
				throw new TypeFaultsException(typeCheck.Faults);
		}

		public static CommonTokenStream GetTokenStream(string file)
		{
			CommonTokenStream tokenStream;
			using (FileStream fs = File.Open(file, FileMode.Open))
			{
				AntlrInputStream ais = new AntlrInputStream(fs);

				Lexer lexer = new Lexer(ais);

				tokenStream = new CommonTokenStream(lexer);
			}
			return tokenStream;
		}

		public static Parser.CompileUnitContext GetParseTree(string file, out List<ParsingFault> parserFaults)
		{
			parserFaults = new List<ParsingFault>();

			using (FileStream fs = File.Open(file, FileMode.Open))
			{
				AntlrInputStream ais = new AntlrInputStream(fs);

				Lexer lexer = new Lexer(ais);

				CommonTokenStream tokenStream = new CommonTokenStream(lexer);

				Parser parser = new Parser(tokenStream);

				ParsingFaultListener faultListener = new ParsingFaultListener(file);
				parser.AddErrorListener(faultListener);

				Parser.CompileUnitContext compileUnit = parser.compileUnit();

				parserFaults = faultListener.Faults;

				return compileUnit;
			}
		}
	}
}
