using System.Collections.Generic;
using Antlr4.Runtime;
using System.IO;
using Lexer = HOPL.Grammar.HOPLGrammarLexer;
using Parser = HOPL.Grammar.HOPLGrammarParser;
using HOPL.Interpreter.NamespaceTypes;
using HOPL.Interpreter.NamespaceMapping;
using HOPL.Interpreter.Exploration;
using HOPL.Interpreter.TypeCheck;
using HOPL.Interpreter.Exceptions;
using HOPL.Interpreter.Errors.Parsing;
using HOPL.Interpreter.Errors.Preparation;
using HOPL.Interpreter.Interpretation.ThreadPool;
using HOPL.Interpreter.Interpretation;
using HOPL.Interpreter.NamespaceTypes.Values;
using Api = HOPL.Interpreter.Api;

namespace HOPL.Interpreter
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
				IInterpreterTriggerable trigger = (IInterpreterTriggerable)expr;
				trigger.Subscribe(handler.OnTrigger);
				handler.HandlerTriggered += pool.QueueHandler;
			}
		}
		
		public static InterpretationContext PrepareFile(string file, ISet<string> importPaths, 
			NamespaceSet namespaces)
		{
			List<ParsingError> parserErrors;
			Parser.CompileUnitContext cUnit = GetParseTree(file, out parserErrors);

			if (parserErrors.Count > 0)
				throw new ParsingErrorsException(parserErrors);

			HashSet<string> exploredFiles = new HashSet<string>();
			Explorer explorer = new Explorer(ref namespaces);
			explorer.CurrentFile = file;
			Dictionary<string, ISet<string>> namespaceFileMap = NamespaceMapper.MapNamespaces(importPaths);

			explorer.EnterCompileUnit(cUnit);
			exploredFiles.Add(file);

			if (explorer.Errors.Count > 0)
				throw new ExploreErrorsException(explorer.Errors);

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
					throw new PrepareErrorException(PrepareErrorMessage.NS_NOTFOUND, import.NamespaceName);
			}

			TypeChecker typeCheck = new TypeChecker(file, namespaces, explorer.ImportTable);
			typeCheck.VisitCompileUnit(cUnit);

			if (typeCheck.Errors.Count > 0)
				throw new TypeErrorsException(typeCheck.Errors);

			Queue<Dependency> evalOrder = null;
			try
			{
				evalOrder = explorer.DepencencyEvaluationOrder();
			}
			catch (RecursiveVariableDependencyException e)
			{
				throw new PrepareErrorException(PrepareErrorMessage.DEP_REC, e.Message);
			}
			catch (DependencyContainsAwaitException e)
			{
				throw new PrepareErrorException(PrepareErrorMessage.DEP_AWAIT, e.Message);
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
			List<ParsingError> parserErrors;
			Parser.CompileUnitContext cUnit = GetParseTree(file, out parserErrors);

			if (parserErrors.Count > 0)
				throw new ParsingErrorsException(parserErrors);

			explorer.CurrentFile = file;
			explorer.EnterCompileUnit(cUnit);
			exploredFiles.Add(file);

			if (explorer.Errors.Count > 0)
				throw new ExploreErrorsException(explorer.Errors);

			if (explorer.ContainedRequired)
				throw new PrepareErrorException(PrepareErrorMessage.NS_REQ, file);

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
					throw new PrepareErrorException(PrepareErrorMessage.NS_NOTFOUND, import.NamespaceName);
			}

			TypeChecker typeCheck = new TypeChecker(file, namespaces, explorer.ImportTable);
			typeCheck.VisitCompileUnit(cUnit);

			if (typeCheck.Errors.Count > 0)
				throw new TypeErrorsException(typeCheck.Errors);
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

		public static Parser.CompileUnitContext GetParseTree(string file, out List<ParsingError> parserErrors)
		{
			parserErrors = new List<ParsingError>();

			using (FileStream fs = File.Open(file, FileMode.Open))
			{
				AntlrInputStream ais = new AntlrInputStream(fs);

				Lexer lexer = new Lexer(ais);

				CommonTokenStream tokenStream = new CommonTokenStream(lexer);

				Parser parser = new Parser(tokenStream);

				ParsingErrorListener errorListener = new ParsingErrorListener(file);
				parser.AddErrorListener(errorListener);

				Parser.CompileUnitContext compileUnit = parser.compileUnit();

				parserErrors = errorListener.Errors;

				return compileUnit;
			}
		}
	}
}
