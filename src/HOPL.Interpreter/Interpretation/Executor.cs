using System;
using System.Collections.Generic;
using System.Linq;
using HOPL.Interpreter.NamespaceTypes.Values;
using HOPL.Grammar;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using Parser = HOPL.Grammar.HOPLGrammarParser;
using HOPL.Interpreter.NamespaceTypes;
using HOPL.Interpreter.Exploration;
using HOPL.Interpreter.Exceptions;
using HOPL.Interpreter.Errors.Runtime;
using Antlr4.Runtime;
using HOPL.Interpreter.Interpretation.ThreadPool;
using Api = HOPL.Interpreter.Api;
using System.Globalization;

namespace HOPL.Interpreter.Interpretation
{
	public class Executor : IHOPLGrammarVisitor<InterpreterValue>
	{
		private struct StackEntry
		{
			public ValueScope scope;
			public string file;
		}
		
		NamespaceSet namespaces;
		ImportAccessTable accessTable;
		IThreadPool pool;

		Stack<StackEntry> scopeStack = new Stack<StackEntry>();
		ValueScope currentScope { get { return scopeStack.Peek().scope; } }
		string currentFile { get { return scopeStack.Peek().file; } }
		BooleanRef running;

		public Executor(HandlerContext handler, IThreadPool pool = null, BooleanRef running = null)
		{
			this.pool = pool;
			this.accessTable = handler.Handler.AccessTable;
			this.running = running ?? new BooleanRef();

			namespaces = handler.Handler.Namespaces;

			StackEntry se = new StackEntry()
			{
				scope = new ValueScope(handler.Handler.Namespace),
				file = handler.Handler.File
			};
			scopeStack.Push(se);

			ITerminalNode[] ids = handler.Handler.Context.ID();
			for (int i = 0; i < ids.Length; i++)
			{
				InterpreterValue argument = InterpreterValue.FromNative(handler.Arguments[i]);
				currentScope.AddVariable(ids[i].GetText(), argument);
			}
		}

		public Executor(Namespace @namespace, NamespaceSet namespaces, string file, 
			ImportAccessTable accessTable, IThreadPool pool = null, BooleanRef running = null)
		{
			this.pool = pool;
			this.namespaces = namespaces;
			this.accessTable = accessTable;
			this.running = running ?? new BooleanRef();

			StackEntry se = new StackEntry()
			{
				scope = new ValueScope(@namespace),
				file = file
			};
			scopeStack.Push(se);
		}

		public void ExecuteHandler(Parser.HandlerDecContext handler)
		{
			VisitBody(handler.body());
		}

		#region Auxiliary
		private Namespace ResolveNamespace(Parser.NamespaceContext context)
		{
			Import import;
			if (!accessTable.TryGetImport(currentFile, context.GetText(), out import))
				return null;
			Namespace ns = currentScope.TopNamespace;
			if (context != null && !namespaces.TryGet(import.NamespaceName, out ns))
				return null;
			return ns;
		}

		private InterpreterValue ResolveIdentifier(Parser.IdentifierContext context)
		{
			Parser.NamespaceContext nscontext = context.@namespace();
			string idName = context.ID().GetText();

			InterpreterValue val;
			if (nscontext != null)
			{
				Namespace ns = ResolveNamespace(nscontext);
				val = ns.GetGlobalEntity(idName).Value;
			}
			else
			{
				val = currentScope.GetVariable(idName);
			}

			foreach (Parser.ExprContext expr in context.expr())
			{
				InterpreterValue index = VisitExpr(expr);
				val = ResolveIndex(val, index, context);
			}

			return val;
		}

		private InterpreterValue ResolveIndex(InterpreterValue val, InterpreterValue index, 
			ParserRuleContext context)
		{
			int indexVal = (int)index.Value;

			if (indexVal < 0 || indexVal > val.Count)
			{
				RuntimeError error = new RuntimeError(RuntimeErrorMessage.INDEX_OUT, context, currentFile);
				throw new RuntimeErrorException(error);
			}

			return val[indexVal];
		}

		private void SetVariable(Parser.IdentifierContext idContext, InterpreterValue value, 
			ParserRuleContext context)
		{
			InterpreterValue newValue = value;

			Parser.NamespaceContext nsctx = idContext.@namespace();
			Parser.ExprContext[] indecies = idContext.expr();

			string varName = idContext.ID().GetText();
			if (nsctx != null)
			{
				Namespace ns = ResolveNamespace(nsctx);
				IGlobalEntity ge = ns.GetGlobalEntity(varName);

				if (indecies != null && indecies.Length > 0)
				{
					int[] intIndecies = new int[indecies.Length];
					for (int i = 0; i < indecies.Length; i++)
						intIndecies[i] = (int)VisitExpr(indecies[i]).Value;
					newValue = SetIndex(ge.Value, intIndecies, value, context);
				}

				ge.Value = newValue;
			}
			else
			{
				if (indecies != null && indecies.Length > 0)
				{
					InterpreterValue currentValue = currentScope.GetVariable(varName);
					int[] intIndecies = new int[indecies.Length];
					for (int i = 0; i < indecies.Length; i++)
						intIndecies[i] = (int)VisitExpr(indecies[i]).Value;
					newValue = SetIndex(currentValue, intIndecies, value, context);
				}

				currentScope.SetVariable(varName, newValue);
			}
		}

		private InterpreterValue SetIndex(InterpreterValue value, int[] indecies, InterpreterValue newValue,
			ParserRuleContext context, int start = 0)
		{
			if (start >= indecies.Length)
				return newValue;

			int index = indecies[start];
			if (index < 0 || index > value.Count)
			{
				RuntimeError error = new RuntimeError(RuntimeErrorMessage.INDEX_OUT, context, currentFile);
				throw new RuntimeErrorException(error);
			}

			InterpreterValue retValue = value.Clone();
			retValue[index] = SetIndex(value[index], indecies, newValue, context, start+1);
			return retValue;
		}

		private InterpreterValue CallFunction(IFunction ifunction, InterpreterValue[] arguments)
		{
			if (ifunction.GetType() == typeof(Function))
			{
				Function function = (Function)ifunction;
				ValueScope fscope = new ValueScope(function.Namespace);

				for(int i = 0; i < arguments.Length; i++)
				{
					Argument fargument = function.Arguments[i];
					InterpreterValue arg = arguments[i];
					fscope.AddVariable(fargument.Name, arg);
				}

				StackEntry fentry = new StackEntry() { scope = fscope, file = function.File };
				scopeStack.Push(fentry);
				InterpreterValue retval = VisitBody(function.Body);
				scopeStack.Pop();

				return retval;
			}
			if(ifunction.GetType() == typeof(SuppliedFunction))
			{
				SuppliedFunction function = (SuppliedFunction)ifunction;
				Argument[] fArgs = ifunction.Arguments;

				object[] nativeArguments = new object[arguments.Length];
				for (int i = 0; i < arguments.Length; i++)
					nativeArguments[i] = arguments[i].ToNative(fArgs[i].Type);

				object retval = function.Method.Invoke(function.Supplier, nativeArguments);
				return InterpreterValue.FromNative(retval);
			}

			throw new InternalExecutorException("Function not recognized.");
		}
		#endregion

		public InterpreterValue Visit(IParseTree tree)
		{
			throw new NotImplementedException();
		}

		public InterpreterValue VisitErrorNode(IErrorNode node)
		{
			throw new NotImplementedException();
		}

		public InterpreterValue VisitChildren(IRuleNode node)
		{
			throw new NotImplementedException();
		}

		public InterpreterValue VisitTerminal(ITerminalNode node)
		{
			throw new NotImplementedException();
		}

		public InterpreterValue VisitAddiExpr([NotNull] Parser.AddiExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();
			InterpreterValue left = VisitExpr(exprs[0]);
			InterpreterValue right = VisitExpr(exprs[1]);

			switch (context.op.Type)
			{
				case Parser.PLUS:
					return left + right;
				case Parser.MINUS:
					return left - right;
				default:
					throw new InvalidOperationException();
			}
		}

		public InterpreterValue VisitAndExpr([NotNull] Parser.AndExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();
			InterpreterValue left = VisitExpr(exprs[0]);

			if (!(bool)left.Value)
				return left;

			InterpreterValue right = VisitExpr(exprs[1]);
			return right;
		}

		public InterpreterValue VisitArg([NotNull] Parser.ArgContext context)
		{
			throw new InternalExecutorException("Arguments should not be accessed.");
		}

		public InterpreterValue VisitArgs([NotNull] Parser.ArgsContext context)
		{
			throw new InternalExecutorException("Arguments should not be accessed.");
		}

		public InterpreterValue VisitAssign([NotNull] Parser.AssignContext context)
		{
			Parser.IdentifierContext idctx = context.identifier();
			InterpreterValue exprValue = VisitExpr(context.expr());
			SetVariable(idctx, exprValue, context);
			return exprValue;
		}

		public InterpreterValue VisitAssignStat([NotNull] Parser.AssignStatContext context)
		{
			return VisitAssign(context.assign());
		}

		public InterpreterValue VisitAwait([NotNull] Parser.AwaitContext context)
		{
			if (!running.Value)
				throw new ExecutorInterruptException();

			InterpreterValue trigger = VisitExpr(context.expr());
			object[] values = pool.Await((Api.SuppliedTrigger)trigger.Value);
			return new InterpreterTuple(values);
		}

		public InterpreterValue VisitAwaitExpr([NotNull] Parser.AwaitExprContext context)
		{
			return VisitAwait(context.await());
		}

		public InterpreterValue VisitBody([NotNull] Parser.BodyContext context)
		{
			currentScope.PushDepth();
			foreach (Parser.StatContext stat in context.stat())
			{
				InterpreterValue value = VisitStat(stat);
				if (!ReferenceEquals(value, null))
				{
					currentScope.PopDepth();
					return value;
				}
			}
			currentScope.PopDepth();
			return null;
		}

		public InterpreterValue VisitBoolType([NotNull] Parser.BoolTypeContext context)
		{
			return null;
		}

		public InterpreterValue VisitBoolVal([NotNull] Parser.BoolValContext context)
		{
			return new InterpreterBool(context.BOOLEAN_VAL().GetText() == "true");
		}

		public InterpreterValue VisitCall([NotNull] Parser.CallContext context)
		{
			if (!running.Value)
				throw new ExecutorInterruptException();

			Parser.ExprContext[] exprs = context.expr();
			InterpreterValue[] arguments = new InterpreterValue[exprs.Length];
			for (int i = 0; i < exprs.Length; i++)
				arguments[i] = VisitExpr(exprs[i]);

			InterpreterValue callable = VisitIdentifier(context.identifier());
			if(callable.GetType() == typeof(InterpreterFunction))
			{
				IFunction function = (IFunction)callable.Value;

				if (function == null)
				{
					RuntimeError error = new RuntimeError(RuntimeErrorMessage.FUNC_UNINIT, context, function.File);
					throw new RuntimeErrorException(error);
				}

				return CallFunction(function, arguments);
			}
			if(callable.GetType() == typeof(InterpreterTrigger))
			{
				Api.SuppliedTrigger trigger = (Api.SuppliedTrigger)callable.Value;
				trigger.Fire(arguments);
				return null;
			}
			throw new InternalExecutorException("Function not recognized.");
		}

		public InterpreterValue VisitCallExpr([NotNull] Parser.CallExprContext context)
		{
			return VisitCall(context.call());
		}

		public InterpreterValue VisitCompExpr([NotNull] Parser.CompExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();
			InterpreterValue left = VisitExpr(exprs[0]);
			InterpreterValue right = VisitExpr(exprs[1]);

			switch (context.op.Type)
			{
				case Parser.EQ:
					return left == right;
				case Parser.NEQ:
					return left != right;
				case Parser.LEQ:
					return left <= right;
				case Parser.GEQ:
					return left >= right;
				case Parser.LESS:
					return left < right;
				case Parser.GRT:
					return left > right;
				default:
					throw new InvalidOperationException();
			}
		}

		public InterpreterValue VisitCompileUnit([NotNull] Parser.CompileUnitContext context)
		{
			throw new InternalExecutorException("Execution should not start from compile unit.");
		}

		public InterpreterValue VisitConcatExpr([NotNull] Parser.ConcatExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();
			InterpreterValue left = VisitExpr(exprs[0]);
			InterpreterValue right = VisitExpr(exprs[1]);

			// Concat
			if (left.TypeEqual(right))
			{
				List<InterpreterValue> leftValues = (List<InterpreterValue>)left.Value;
				List<InterpreterValue> rightValues = (List<InterpreterValue>)right.Value;
				List<InterpreterValue> newValues = leftValues.Concat(rightValues).ToList();
				return new InterpreterList(newValues);
			}

			// Append
			if (left.GetType() == typeof(InterpreterList))
			{
				List<InterpreterValue> rightBuffer = new List<InterpreterValue>();
				rightBuffer.Add(right);
				InterpreterList rightList = new InterpreterList(rightBuffer);

				if (left.TypeEqual(rightList))
				{
					List<InterpreterValue> leftValues = (List<InterpreterValue>)left.Value;
					List<InterpreterValue> newValues = leftValues.Append(right).ToList();
					return new InterpreterList(newValues);
				}
			}

			// Prepend
			if (right.GetType() == typeof(InterpreterList))
			{
				List<InterpreterValue> leftBuffer = new List<InterpreterValue>();
				leftBuffer.Add(left);
				InterpreterList leftList = new InterpreterList(leftBuffer);

				if (left.TypeEqual(leftList))
				{
					List<InterpreterValue> rightValues = (List<InterpreterValue>)right.Value;
					List<InterpreterValue> newValues = rightValues.Prepend(left).ToList();
					return new InterpreterList(newValues);
				}
			}

			throw new InvalidOperationException();
		}

		public InterpreterValue VisitDecStat([NotNull] Parser.DecStatContext context)
		{
			return VisitVarDec(context.varDec());
		}

		public InterpreterValue VisitDecUnpacked([NotNull] Parser.DecUnpackedContext context)
		{
			return null;
		}

		public InterpreterValue VisitElse([NotNull] Parser.ElseContext context)
		{
			return VisitBody(context.body());
		}

		public InterpreterValue VisitElseIf([NotNull] Parser.ElseIfContext context)
		{
			bool pred;
			return VisitElseIf(context, out pred);
		}

		public InterpreterValue VisitElseIf([NotNull] Parser.ElseIfContext context, out bool predicate)
		{
			predicate = (bool)VisitExpr(context.expr()).Value;
			
			if (predicate)
				return VisitBody(context.body());
			return null;
		}

		public InterpreterValue VisitExpr([NotNull] Parser.ExprContext context)
		{
			Type t = context.GetType();
			if (t == typeof(Parser.ParanExprContext))
				return VisitParanExpr((Parser.ParanExprContext)context);
			if (t == typeof(Parser.MultExprContext))
				return VisitMultExpr((Parser.MultExprContext)context);
			if (t == typeof(Parser.AddiExprContext))
				return VisitAddiExpr((Parser.AddiExprContext)context);
			if (t == typeof(Parser.AndExprContext))
				return VisitAndExpr((Parser.AndExprContext)context);
			if (t == typeof(Parser.OrExprContext))
				return VisitOrExpr((Parser.OrExprContext)context);
			if (t == typeof(Parser.CompExprContext))
				return VisitCompExpr((Parser.CompExprContext)context);
			if (t == typeof(Parser.NegExprContext))
				return VisitNegExpr((Parser.NegExprContext)context);
			if (t == typeof(Parser.NotExprContext))
				return VisitNotExpr((Parser.NotExprContext)context);
			if (t == typeof(Parser.ValExprContext))
				return VisitValExpr((Parser.ValExprContext)context);
			if (t == typeof(Parser.VarExprContext))
				return VisitVarExpr((Parser.VarExprContext)context);
			if (t == typeof(Parser.CallExprContext))
				return VisitCallExpr((Parser.CallExprContext)context);
			if (t == typeof(Parser.AwaitExprContext))
				return VisitAwaitExpr((Parser.AwaitExprContext)context);
			if (t == typeof(Parser.ListExprContext))
				return VisitListExpr((Parser.ListExprContext)context);
			if (t == typeof(Parser.TupleExprContext))
				return VisitTupleExpr((Parser.TupleExprContext)context);
			if (t == typeof(Parser.IndexExprContext))
				return VisitIndexExpr((Parser.IndexExprContext)context);
			throw new InternalExecutorException("expr alias not recognized.");
		}

		public InterpreterValue VisitExprStat([NotNull] Parser.ExprStatContext context)
		{
			return VisitExpr(context.expr());
		}

		public InterpreterValue VisitFloatType([NotNull] Parser.FloatTypeContext context)
		{
			return null;
		}

		public InterpreterValue VisitFloatVal([NotNull] Parser.FloatValContext context)
		{
			string valString = context.FLOAT_VAL().GetText();
			float val = float.Parse(valString, CultureInfo.InvariantCulture.NumberFormat);
			return new InterpreterFloat(val);
		}

		public InterpreterValue VisitFor([NotNull] Parser.ForContext context)
		{
			currentScope.PushDepth();

			VisitVarDec(context.declare);
			while ((bool)VisitExpr(context.predicate).Value)
			{
				if (!running.Value)
					throw new ExecutorInterruptException();

				InterpreterValue bodyValue = VisitBody(context.body());

				if(!ReferenceEquals(bodyValue, null))
				{
					currentScope.PopDepth();
					return bodyValue;
				}

				VisitAssign(context.reeval);
			}

			currentScope.PopDepth();
			return null;
		}

		public InterpreterValue VisitForeach([NotNull] Parser.ForeachContext context)
		{
			InterpreterValue exprList = VisitExpr(context.expr());
			List<InterpreterValue> list = (List<InterpreterValue>)exprList.Value;
			
			foreach (InterpreterValue value in list)
			{
				if (!running.Value)
					throw new ExecutorInterruptException();

				currentScope.PushDepth();
				currentScope.AddVariable(context.ID().GetText(), value);
				InterpreterValue bodyValue = VisitBody(context.body());
				currentScope.PopDepth();

				if (!ReferenceEquals(bodyValue, null))
					return bodyValue;
			}

			return null;
		}

		public InterpreterValue VisitForeachStat([NotNull] Parser.ForeachStatContext context)
		{
			return VisitForeach(context.@foreach());
		}

		public InterpreterValue VisitForStat([NotNull] Parser.ForStatContext context)
		{
			return VisitFor(context.@for());
		}

		public InterpreterValue VisitFunctionDec([NotNull] Parser.FunctionDecContext context)
		{
			throw new InternalExecutorException("Function declaration code should not be accessed.");
		}

		public InterpreterValue VisitFunctionDecNamespace([NotNull] Parser.FunctionDecNamespaceContext context)
		{
			throw new InternalExecutorException("Function declaration code should not be accessed.");
		}

		public InterpreterValue VisitFunctionType([NotNull] Parser.FunctionTypeContext context)
		{
			return null;
		}

		public InterpreterValue VisitGlobalDec([NotNull] Parser.GlobalDecContext context)
		{
			Parser.VarDecContext varDec = context.varDec();
			Parser.ExprContext expr = varDec.expr();

			InterpreterValue val;
			if(expr != null)
			{
				val = VisitExpr(expr);
			}
			else
			{
				Parser.TypeNameContext type = varDec.typeName();
				val = InterpreterValue.GetDefault(type);
			}
			string varName = varDec.ID().GetText();

			IGlobalEntity ge = currentScope.TopNamespace.GetGlobalEntity(varName);
			ge.Value = val;
			return val;
		}

		public InterpreterValue VisitGlobalDecNamespace([NotNull] Parser.GlobalDecNamespaceContext context)
		{
			throw new InternalExecutorException("Global declaration code should not be accessed.");
		}

		public InterpreterValue VisitHandlerDec([NotNull] Parser.HandlerDecContext context)
		{
			throw new InternalExecutorException("Handler declaration code should not be accessed.");
		}

		public InterpreterValue VisitHandlerDecNamespace([NotNull] Parser.HandlerDecNamespaceContext context)
		{
			throw new InternalExecutorException("Handler declaration code should not be accessed.");
		}

		public InterpreterValue VisitIdentifier([NotNull] Parser.IdentifierContext context)
		{
			return ResolveIdentifier(context).Clone();
		}

		public InterpreterValue VisitIdUnpacked([NotNull] Parser.IdUnpackedContext context)
		{
			return null;
		}

		public InterpreterValue VisitIf([NotNull] Parser.IfContext context)
		{
			bool pred;
			return VisitIf(context, out pred);
		}

		public InterpreterValue VisitIf([NotNull] Parser.IfContext context, out bool predicate)
		{
			predicate = (bool)VisitExpr(context.expr()).Value;

			if (predicate)
				return VisitBody(context.body());
			return null;
		}

		public InterpreterValue VisitIfStat([NotNull] Parser.IfStatContext context)
		{
			bool ifpred;
			InterpreterValue ifBodyValue = VisitIf(context.@if(), out ifpred);
			if (ifpred)
				return ifBodyValue;

			foreach (Parser.ElseIfContext elif in context.elseIf())
			{
				bool elifpred;
				InterpreterValue elifBodyValue = VisitElseIf(elif, out elifpred);
				if (elifpred)
					return elifBodyValue;
			}
			
			return VisitElse(context.@else());
		}

		public InterpreterValue VisitIgnoreUnpacked([NotNull] Parser.IgnoreUnpackedContext context)
		{
			throw null;
		}

		public InterpreterValue VisitImportNamespace([NotNull] Parser.ImportNamespaceContext context)
		{
			throw new InternalExecutorException("Import statements should not be accessed.");
		}

		public InterpreterValue VisitIndexExpr([NotNull] Parser.IndexExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();
			InterpreterValue index = VisitExpr(exprs[1]);
			InterpreterValue list = VisitExpr(exprs[0]);
			return ResolveIndex(list, index, context);
		}

		public InterpreterValue VisitIntType([NotNull] Parser.IntTypeContext context)
		{
			return null;
		}

		public InterpreterValue VisitIntVal([NotNull] Parser.IntValContext context)
		{
			return new InterpreterInt(int.Parse(context.INTEGER_VAL().GetText()));
		}

		public InterpreterValue VisitListExpr([NotNull] Parser.ListExprContext context)
		{
			List<InterpreterValue> values = new List<InterpreterValue>();

			foreach (Parser.ExprContext expr in context.expr())
				values.Add(VisitExpr(expr));

			return new InterpreterList(values);
		}

		public InterpreterValue VisitListType([NotNull] Parser.ListTypeContext context)
		{
			return null;
		}

		public InterpreterValue VisitMultExpr([NotNull] Parser.MultExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();
			InterpreterValue left = VisitExpr(exprs[0]);
			InterpreterValue right = VisitExpr(exprs[1]);

			switch (context.op.Type)
			{
				case Parser.MULT:
					return left * right;
				case Parser.DIV:
					return left / right;
				default:
					throw new InvalidOperationException();
			}
		}

		public InterpreterValue VisitNamespace([NotNull] Parser.NamespaceContext context)
		{
			return null;
		}

		public InterpreterValue VisitNamespaceBody([NotNull] Parser.NamespaceBodyContext context)
		{
			throw new InternalExecutorException("Outer namespace should not be accessed.");
		}

		public InterpreterValue VisitNamespaceDec([NotNull] Parser.NamespaceDecContext context)
		{
			throw new InternalExecutorException("Outer namespace should not be accessed.");
		}

		public InterpreterValue VisitNamespacePart([NotNull] Parser.NamespacePartContext context)
		{
			return null;
		}

		public InterpreterValue VisitNegExpr([NotNull] Parser.NegExprContext context)
		{
			InterpreterValue value = VisitExpr(context.expr());
			return -value;
		}

		public InterpreterValue VisitNotExpr([NotNull] Parser.NotExprContext context)
		{
			InterpreterValue value = VisitExpr(context.expr());
			return !value;
		}

		public InterpreterValue VisitOrExpr([NotNull] Parser.OrExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();
			InterpreterValue left = VisitExpr(exprs[0]);

			if ((bool)left.Value)
				return left;

			InterpreterValue right = VisitExpr(exprs[1]);
			return right;
		}

		public InterpreterValue VisitParanExpr([NotNull] Parser.ParanExprContext context)
		{
			return VisitExpr(context.expr());
		}

		public InterpreterValue VisitReturn([NotNull] Parser.ReturnContext context)
		{
			Parser.ExprContext expr = context.expr();
			if(expr != null)
				return VisitExpr(expr);
			return new InterpreterEmptyReturn();
		}

		public InterpreterValue VisitReturnStat([NotNull] Parser.ReturnStatContext context)
		{
			return VisitReturn(context.@return());
		}

		public InterpreterValue VisitStat([NotNull] Parser.StatContext context)
		{
			if (context.GetType() == typeof(Parser.AssignStatContext))
				VisitAssignStat((Parser.AssignStatContext)context);
			else if (context.GetType() == typeof(Parser.DecStatContext))
				VisitDecStat((Parser.DecStatContext)context);
			else if (context.GetType() == typeof(Parser.ExprStatContext))
				VisitExprStat((Parser.ExprStatContext)context);
			else if (context.GetType() == typeof(Parser.UnpackStatContext))
				VisitUnpackStat((Parser.UnpackStatContext)context);
			else if (context.GetType() == typeof(Parser.ReturnStatContext))
				return VisitReturnStat((Parser.ReturnStatContext)context);
			else if (context.GetType() == typeof(Parser.IfStatContext))
				return VisitIfStat((Parser.IfStatContext)context);
			else if (context.GetType() == typeof(Parser.WhileStatContext))
				return VisitWhileStat((Parser.WhileStatContext)context);
			else if (context.GetType() == typeof(Parser.ForStatContext))
				return VisitForStat((Parser.ForStatContext)context);
			else if (context.GetType() == typeof(Parser.ForeachStatContext))
				return VisitForeachStat((Parser.ForeachStatContext)context);
			else
				throw new InternalExecutorException("stat not recognized.");
			return null;
		}

		public InterpreterValue VisitStringType([NotNull] Parser.StringTypeContext context)
		{
			return null;
		}

		public InterpreterValue VisitStringVal([NotNull] Parser.StringValContext context)
		{
			string fullString = context.STRING_VAL().GetText();
			return new InterpreterString(fullString.Substring(1, fullString.Length - 2));
		}

		public InterpreterValue VisitTriggerType([NotNull] Parser.TriggerTypeContext context)
		{
			return null;
		}

		public InterpreterValue VisitTupleExpr([NotNull] Parser.TupleExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();
			InterpreterValue[] values = new InterpreterValue[exprs.Length];
			for (int i = 0; i < exprs.Length; i++)
				values[i] = VisitExpr(exprs[i]);
			return new InterpreterTuple(values);
		}

		public InterpreterValue VisitTupleType([NotNull] Parser.TupleTypeContext context)
		{
			return null;
		}

		public InterpreterValue VisitTypeName([NotNull] Parser.TypeNameContext context)
		{
			return null;
		}

		public InterpreterValue VisitTypeVal([NotNull] Parser.TypeValContext context)
		{
			if (context.GetType() == typeof(Parser.IntValContext))
				return VisitIntVal((Parser.IntValContext)context);
			if (context.GetType() == typeof(Parser.FloatValContext))
				return VisitFloatVal((Parser.FloatValContext)context);
			if (context.GetType() == typeof(Parser.BoolValContext))
				return VisitBoolVal((Parser.BoolValContext)context);
			if (context.GetType() == typeof(Parser.StringValContext))
				return VisitStringVal((Parser.StringValContext)context);
			throw new InternalExecutorException("typeval not recognized.");
		}

		public InterpreterValue VisitUnpack([NotNull] Parser.UnpackContext context)
		{
			InterpreterValue expr = VisitExpr(context.expr());
			InterpreterValue[] tupleValues = (InterpreterValue[])expr.Value;

			Parser.UnpackedContext[] upkd = context.unpacked();
			for(int i = 0; i < upkd.Length; i++)
			{
				if (upkd[i].GetType() == typeof(Parser.IgnoreUnpackedContext))
					continue;
				
				if (upkd[i].GetType() == typeof(Parser.DecUnpackedContext))
				{
					Parser.DecUnpackedContext dec = (Parser.DecUnpackedContext)upkd[i];
					string varName = dec.ID().GetText();
					currentScope.AddVariable(varName, tupleValues[i]);
				}
				else
				{
					Parser.IdUnpackedContext idu = (Parser.IdUnpackedContext)upkd[i];
					Parser.IdentifierContext identifier = idu.identifier();
					SetVariable(identifier, tupleValues[i], context);
				}
			}

			return null;
		}

		public InterpreterValue VisitUnpacked([NotNull] Parser.UnpackedContext context)
		{
			return null;
		}

		public InterpreterValue VisitUnpackStat([NotNull] Parser.UnpackStatContext context)
		{
			return VisitUnpack(context.unpack());
		}

		public InterpreterValue VisitValExpr([NotNull] Parser.ValExprContext context)
		{
			return VisitTypeVal(context.typeVal());
		}

		public InterpreterValue VisitVarDec([NotNull] Parser.VarDecContext context)
		{
			Parser.ExprContext expr = context.expr();
			InterpreterValue value;
			if (expr != null)
				value = VisitExpr(expr);
			else
				value = InterpreterValue.GetDefault(context.typeName());

			currentScope.AddVariable(context.ID().GetText(), value);
			return value;
		}

		public InterpreterValue VisitVarExpr([NotNull] Parser.VarExprContext context)
		{
			return VisitIdentifier(context.identifier());
		}

		public InterpreterValue VisitWhile([NotNull] Parser.WhileContext context)
		{
			Parser.ExprContext expr = context.expr();
			while ((bool)VisitExpr(expr).Value)
			{
				if (!running.Value)
					throw new ExecutorInterruptException();

				InterpreterValue bodyValue = VisitBody(context.body());

				if (!ReferenceEquals(bodyValue, null))
					return bodyValue;
			}
			return null;
		}

		public InterpreterValue VisitWhileStat([NotNull] Parser.WhileStatContext context)
		{
			return VisitWhile(context.@while());
		}
	}
}
