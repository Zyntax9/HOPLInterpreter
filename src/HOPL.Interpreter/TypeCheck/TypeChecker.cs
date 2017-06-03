using System;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using HOPL.Grammar;
using Parser = HOPL.Grammar.HOPLGrammarParser;
using System.Collections.Generic;
using HOPL.Interpreter.NamespaceTypes;
using HOPL.Interpreter.Exceptions;
using HOPL.Interpreter.Errors.TypeCheck;
using Antlr4.Runtime;
using HOPL.Interpreter.Exploration;

namespace HOPL.Interpreter.TypeCheck
{
	public class TypeChecker : IHOPLGrammarVisitor<InterpreterType>
	{
		public TypeErrorCollection Errors { get; private set; } = new TypeErrorCollection();

		private string filename;
		private NamespaceSet namespaceSet = new NamespaceSet();
		private ImportAccessTable accessTable = new ImportAccessTable();

		private Namespace currentNamespace;
		private TypeScope currentScope;
		private InterpreterType currentCallable;

		public TypeChecker() { }

		public TypeChecker(string filename, NamespaceSet namespaces, ImportAccessTable accessTable)
		{
			this.filename = filename;
			namespaceSet = namespaces;
			this.accessTable = accessTable;
		}

		#region Auxiliary methods
		private bool AssignmentAllowed(InterpreterType assignTo, InterpreterType assignFrom)
		{
			if (assignTo == assignFrom ||
				assignTo == InterpreterType.FLOAT && assignFrom == InterpreterType.INT)
				return true;

			if (assignTo.TypeOf == InterpreterType.Types.LIST &&
			   assignFrom.TypeOf == InterpreterType.Types.LIST)
				return AssignmentAllowed(assignTo.TypeArray[0], assignFrom.TypeArray[0]);

			if (assignTo.TypeOf == InterpreterType.Types.TUPLE &&
				assignFrom.TypeOf == InterpreterType.Types.TUPLE &&
				assignTo.TypeArray.Length == assignFrom.TypeArray.Length)
			{
				for (int i = 0; i < assignTo.TypeArray.Length; i++)
				{
					if (!AssignmentAllowed(assignTo.TypeArray[i], assignFrom.TypeArray[i]))
						return false;
				}
				return true;
			}

			return false;
		}

		private Namespace ResolveNamespace(Parser.NamespaceContext context)
		{
            NamespaceString @namespace = new NamespaceString(context.GetText());

			Import import;
            NamespaceString remaining;
			if (!accessTable.TryGetImport(filename, @namespace, out import, out remaining))
				return null;
			Namespace ns = currentNamespace;
			if (context != null && !namespaceSet.TryGet(import.NamespaceName, out ns))
				return null;
			return ns;
		}

		private InterpreterType ResolveIdentifier(Parser.IdentifierContext context, bool toAssign = false)
		{
			IGlobalEntity ge;
			InterpreterType varType = InterpreterType.NONE;
			Parser.NamespaceContext nscontext = context.@namespace();
			string idName = context.ID().GetText();
			if (nscontext != null)
			{
				Namespace ns = ResolveNamespace(nscontext);
				if (ns == null)
					return RaiseError(TypeErrorMessage.NS_MISSING, context);

				if (!ns.TryGetGlobalEntity(idName, out ge))
					return RaiseError(TypeErrorMessage.VAR_NOTDEF, context);

				if (toAssign && ge.Constant)
					return RaiseError(TypeErrorMessage.ASSIGN_CONST, context);

				varType = ge.Type;
			}
			else
			{
				if (!currentScope.TryGetVariable(idName, out varType))
					return RaiseError(TypeErrorMessage.VAR_NOTDEF, context);

				if (toAssign && currentScope.IsConstant(idName))
					return RaiseError(TypeErrorMessage.ASSIGN_CONST, context);
			}

			foreach(Parser.ExprContext expr in context.expr())
			{
				if (varType.TypeOf == InterpreterType.Types.LIST)
					varType = ResolveListIndex(varType, expr, context);
				else if (varType.TypeOf == InterpreterType.Types.TUPLE)
					varType = ResolveTupleIndex(varType, expr, context);
				else
					return RaiseError(TypeErrorMessage.INDEX_LORT, context);
				
				if (varType == InterpreterType.ERROR)
					return InterpreterType.ERROR;
			}

			return varType;
		}

		private InterpreterType ResolveListIndex(InterpreterType type, Parser.ExprContext index, 
			ParserRuleContext context)
		{
			if (type.IsEmptyList)
				return RaiseError(TypeErrorMessage.INDEX_EMPTY, context);

			InterpreterType itype = VisitExpr(index);
			if (itype == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (itype.TypeOf != InterpreterType.Types.INT)
				return RaiseError(TypeErrorMessage.INDEX_LINT, context);

			return type.TypeArray[0];
		}

		private InterpreterType ResolveTupleIndex(InterpreterType type, Parser.ExprContext index,
			ParserRuleContext context)
		{
			int i;
			if (!int.TryParse(index.GetText(), out i))
				return RaiseError(TypeErrorMessage.INDEX_TCINT, context);

			if (i >= type.TypeArray.Length || i < 0)
				return RaiseError(TypeErrorMessage.INDEX_TOOR, context);

			return type.TypeArray[i];
		}

		private InterpreterType RaiseError(TypeErrorMessage msg, ParserRuleContext context)
		{
			Errors.Add(msg, context, filename);
			return InterpreterType.ERROR;
		}
		#endregion

		#region Visitor implementation
		public InterpreterType Visit(IParseTree tree)
		{
			throw new NotImplementedException();
		}

		public InterpreterType VisitChildren(IRuleNode node)
		{
			throw new NotImplementedException();
		}

		public InterpreterType VisitErrorNode(IErrorNode node)
		{
			throw new NotImplementedException();
		}

		public InterpreterType VisitAddiExpr([NotNull] Parser.AddiExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();

			InterpreterType left = VisitExpr(exprs[0]);
			InterpreterType right = VisitExpr(exprs[1]);

			if (left == InterpreterType.ERROR || right == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (context.op.Type == Parser.PLUS && left == InterpreterType.STRING &&
				right == InterpreterType.STRING)
				return InterpreterType.STRING;

			if (left != InterpreterType.INT && left != InterpreterType.FLOAT ||
				right != InterpreterType.INT && right != InterpreterType.FLOAT)
			{
				if (context.op.Type == Parser.PLUS)
					return RaiseError(TypeErrorMessage.ADDI_MISMATCH, context);
				else
					return RaiseError(TypeErrorMessage.NUMERICAL_MISMATCH, context);
			}

			return left == InterpreterType.FLOAT || right == InterpreterType.FLOAT ?
				InterpreterType.FLOAT : InterpreterType.INT;
		}

		public InterpreterType VisitAndExpr([NotNull] Parser.AndExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();

			InterpreterType left = VisitExpr(exprs[0]);
			InterpreterType right = VisitExpr(exprs[1]);

			if (left == InterpreterType.ERROR || right == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (left != InterpreterType.BOOL || right != InterpreterType.BOOL)
				return RaiseError(TypeErrorMessage.BOOLEAN_MISMATCH, context);

			return InterpreterType.BOOL;
		}

		public InterpreterType VisitArg([NotNull] Parser.ArgContext context)
		{
			return VisitTypeName(context.typeName());
		}

		public InterpreterType VisitArgs([NotNull] Parser.ArgsContext context)
		{
			Parser.ArgContext[] args = context.arg();
			InterpreterType[] domain = currentCallable.GetCallableDomain();

			if (args.Length != domain.Length)
				return RaiseError(TypeErrorMessage.ARGCOUNT_MISMATCH, context);

			foreach (Parser.ArgContext arg in args)
				VisitArg(arg);

			return InterpreterType.NONE;
		}

		public InterpreterType VisitAssign([NotNull] Parser.AssignContext context)
		{
			InterpreterType right = VisitExpr(context.expr());
			if (right == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			InterpreterType varType = ResolveIdentifier(context.identifier(), true);
			if (varType == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (AssignmentAllowed(varType, right))
				return varType;

			return RaiseError(TypeErrorMessage.ASSIGN_MISMATCH, context);
		}

		public InterpreterType VisitAssignStat([NotNull] Parser.AssignStatContext context)
		{
			return VisitAssign(context.assign());
		}

		public InterpreterType VisitBody([NotNull] Parser.BodyContext context)
		{
			return VisitBody(context, null);
		}

		public InterpreterType VisitBody([NotNull] Parser.BodyContext context, IEnumerable<Argument> bodyArgs)
		{
			currentScope.PushDepth();

			if (bodyArgs != null)
				foreach (Argument arg in bodyArgs)
					if (!currentScope.TryAddVariable(arg.Name, arg.Type))
						return RaiseError(TypeErrorMessage.ARG_SHADOW, context);

			foreach (Parser.StatContext stat in context.stat())
				VisitStat(stat);

			currentScope.PopDepth();
			return InterpreterType.NONE;
		}

		public InterpreterType VisitBoolType([NotNull] Parser.BoolTypeContext context)
		{
			return InterpreterType.BOOL;
		}

		public InterpreterType VisitBoolVal([NotNull] Parser.BoolValContext context)
		{
			return InterpreterType.BOOL;
		}

		public InterpreterType VisitCall([NotNull] Parser.CallContext context)
		{
			InterpreterType upperCallable = currentCallable;

			InterpreterType callableType = VisitIdentifier(context.identifier());

			if (!callableType.IsCallable)
				return RaiseError(TypeErrorMessage.CALLABLE_NOTCALLABLE, context);

			if (callableType == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			currentCallable = callableType;

			Parser.ExprContext[] exprs = context.expr();
			InterpreterType[] domain = currentCallable.GetCallableDomain();

			if (exprs.Length != domain.Length)
			{
				currentCallable = upperCallable;
				return RaiseError(TypeErrorMessage.ARGCOUNT_MISMATCH, context);
			}

			for (int i = 0; i < exprs.Length; i++)
			{
				Parser.ExprContext expr = exprs[i];
				InterpreterType exprType = VisitExpr(expr);

				if (exprType == InterpreterType.ERROR)
				{
					currentCallable = upperCallable;
					return InterpreterType.ERROR;
				}

				if (!AssignmentAllowed(domain[i], exprType))
				{
					currentCallable = upperCallable;
					return RaiseError(TypeErrorMessage.CALL_ARGMISMATCH, context);
				}
			}

			InterpreterType returnType = currentCallable.GetCallableRange();
			currentCallable = upperCallable;
			return returnType;
		}

		public InterpreterType VisitIdentifier([NotNull] Parser.IdentifierContext context)
		{
			return VisitIdentifier(context, false);
		}

		public InterpreterType VisitIdentifier([NotNull] Parser.IdentifierContext context, bool toAssign)
		{
			return ResolveIdentifier(context, toAssign);
		}

		public InterpreterType VisitCallExpr([NotNull] Parser.CallExprContext context)
		{
			return VisitCall(context.call());
		}

		public InterpreterType VisitCompExpr([NotNull] Parser.CompExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();

			InterpreterType left = VisitExpr(exprs[0]);
			InterpreterType right = VisitExpr(exprs[1]);

			if (left == InterpreterType.ERROR || right == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if ((left == InterpreterType.BOOL || right == InterpreterType.BOOL) &&
                context.op.Type != Parser.EQ && context.op.Type != Parser.NEQ)
				return RaiseError(TypeErrorMessage.NONBOOLEAN_MISMATCH, context);

			if (!(left == InterpreterType.FLOAT && right == InterpreterType.INT ||
				 left == InterpreterType.INT && right == InterpreterType.FLOAT ||
				 left == right))
				return RaiseError(TypeErrorMessage.COMPEXPR_MISMATCH, context);

			return InterpreterType.BOOL;
		}

		public InterpreterType VisitCompileUnit([NotNull] Parser.CompileUnitContext context)
		{
			foreach (Parser.ImportNamespaceContext import in context.importNamespace())
				VisitImportNamespace(import);

			foreach (Parser.NamespaceDecContext namespaceDec in context.namespaceDec())
				VisitNamespaceDec(namespaceDec);

			return InterpreterType.NONE;
		}

		public InterpreterType VisitDecStat([NotNull] Parser.DecStatContext context)
		{
			return VisitVarDec(context.varDec());
		}

		public InterpreterType VisitExpr([NotNull] Parser.ExprContext context)
		{
			Type t = context.GetType();
			if (context is Parser.ParanExprContext)
				return VisitParanExpr((Parser.ParanExprContext)context);
			if (context is Parser.MultExprContext)
				return VisitMultExpr((Parser.MultExprContext)context);
			if (context is Parser.AddiExprContext)
				return VisitAddiExpr((Parser.AddiExprContext)context);
			if (context is Parser.AndExprContext)
				return VisitAndExpr((Parser.AndExprContext)context);
			if (context is Parser.OrExprContext)
				return VisitOrExpr((Parser.OrExprContext)context);
			if (context is Parser.CompExprContext)
				return VisitCompExpr((Parser.CompExprContext)context);
			if (context is Parser.NegExprContext)
				return VisitNegExpr((Parser.NegExprContext)context);
			if (context is Parser.NotExprContext)
				return VisitNotExpr((Parser.NotExprContext)context);
			if (context is Parser.ValExprContext)
				return VisitValExpr((Parser.ValExprContext)context);
			if (context is Parser.VarExprContext)
				return VisitVarExpr((Parser.VarExprContext)context);
			if (context is Parser.CallExprContext)
				return VisitCallExpr((Parser.CallExprContext)context);
			if (context is Parser.AwaitExprContext)
				return VisitAwaitExpr((Parser.AwaitExprContext)context);
			if (context is Parser.ListExprContext)
				return VisitListExpr((Parser.ListExprContext)context);
			if (context is Parser.TupleExprContext)
				return VisitTupleExpr((Parser.TupleExprContext)context);
			if (context is Parser.IndexExprContext)
				return VisitIndexExpr((Parser.IndexExprContext)context);
            if (context is Parser.ConcatExprContext)
                return VisitConcatExpr((Parser.ConcatExprContext)context);
			throw new InternalTypeCheckerException("expr alias not recognized.");
		}

		public InterpreterType VisitFloatType([NotNull] Parser.FloatTypeContext context)
		{
			return InterpreterType.FLOAT;
		}

		public InterpreterType VisitFloatVal([NotNull] Parser.FloatValContext context)
		{
			return InterpreterType.FLOAT;
		}

		public InterpreterType VisitFunctionDec([NotNull] Parser.FunctionDecContext context)
		{
			IFunction f = currentNamespace.GetFunction(context.ID().GetText());
			currentCallable = new InterpreterType(f.Signature);

			VisitArgs(context.args());
			VisitBody(context.body(), f.Arguments);

			currentCallable = null;
			return VisitTypeName(context.typeName());
		}

		public InterpreterType VisitFunctionDecNamespace([NotNull] Parser.FunctionDecNamespaceContext context)
		{
			return VisitFunctionDec(context.functionDec());
		}

		public InterpreterType VisitGlobalDec([NotNull] Parser.GlobalDecContext context)
		{
			return VisitVarDec(context.varDec(), true);
		}

		public InterpreterType VisitGlobalDecNamespace([NotNull] Parser.GlobalDecNamespaceContext context)
		{
			return VisitGlobalDec(context.globalDec());
		}

		public InterpreterType VisitHandlerDec([NotNull] Parser.HandlerDecContext context)
		{
			InterpreterType callableType = VisitExpr(context.expr());

			if (callableType == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (!callableType.IsTriggerable)
			{
				return RaiseError(TypeErrorMessage.HANDLERDEC_NOTTRIGGER, context);
			}
			
			currentCallable = callableType;
            
			Parser.ArgContext[] args = context.args().arg();
			InterpreterType[] domain = currentCallable.GetTriggerableDomain();
			if (args.Length != domain.Length)
			{
				currentCallable = null;
				return RaiseError(TypeErrorMessage.ARGCOUNT_MISMATCH, context);
			}

			InterpreterType[] typeNames = new InterpreterType[args.Length];
			for (int i = 0; i < args.Length; i++)
			{
				typeNames[i] = VisitTypeName(args[i].typeName());

				if (typeNames[i] == InterpreterType.ERROR)
				{
					currentCallable = null;
					return InterpreterType.ERROR;
				}

				if (typeNames[i] != domain[i])
				{
					currentCallable = null;
					return RaiseError(TypeErrorMessage.HANDLERDEC_ARGMISMATCH, context);
				}
			}

			Argument[] handlerArgs = new Argument[args.Length];
			for (int i = 0; i < args.Length; i++)
				handlerArgs[i] = new Argument(args[i].ID().GetText(), domain[i]);

			VisitBody(context.body(), handlerArgs);

			currentCallable = null;
			return InterpreterType.NONE;
		}

		public InterpreterType VisitVarExpr([NotNull] Parser.VarExprContext context)
		{
			return ResolveIdentifier(context.identifier());
		}

		public InterpreterType VisitIntType([NotNull] Parser.IntTypeContext context)
		{
			return InterpreterType.INT;
		}

		public InterpreterType VisitIntVal([NotNull] Parser.IntValContext context)
		{
			return InterpreterType.INT;
		}

		public InterpreterType VisitMultExpr([NotNull] Parser.MultExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();

			InterpreterType left = VisitExpr(exprs[0]);
			InterpreterType right = VisitExpr(exprs[1]);

			if (left == InterpreterType.ERROR || right == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (left != InterpreterType.INT && left != InterpreterType.FLOAT ||
				right != InterpreterType.INT && right != InterpreterType.FLOAT)
				return RaiseError(TypeErrorMessage.NUMERICAL_MISMATCH, context);

			return left == InterpreterType.FLOAT || right == InterpreterType.FLOAT ?
				InterpreterType.FLOAT : InterpreterType.INT;
		}

		public InterpreterType VisitNamespace([NotNull] Parser.NamespaceContext context)
		{
			return InterpreterType.NONE;
		}

		public InterpreterType VisitNamespaceBody([NotNull] Parser.NamespaceBodyContext context)
		{
			currentScope = new TypeScope(currentNamespace);

			foreach (Parser.NamespacePartContext part in context.namespacePart())
				VisitNamespacePart(part);

			currentScope = null;
			return InterpreterType.NONE;
		}

		public InterpreterType VisitNamespaceDec([NotNull] Parser.NamespaceDecContext context)
		{
			currentNamespace = namespaceSet.Get(context.@namespace().GetText());
			VisitNamespace(context.@namespace());
			VisitNamespaceBody(context.namespaceBody());
			currentNamespace = null;
			return InterpreterType.NONE;
		}

		public InterpreterType VisitNegExpr([NotNull] Parser.NegExprContext context)
		{
			InterpreterType exprType = VisitExpr(context.expr());

			if (exprType == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (exprType != InterpreterType.INT || exprType != InterpreterType.FLOAT)
				return RaiseError(TypeErrorMessage.NEG_NUMMISMATCH, context);

			return exprType;
		}

		public InterpreterType VisitNotExpr([NotNull] Parser.NotExprContext context)
		{
			InterpreterType exprType = VisitExpr(context.expr());

			if (exprType == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (exprType != InterpreterType.BOOL)
				return RaiseError(TypeErrorMessage.NOT_BOOLMISMATCH, context);

			return exprType;
		}

		public InterpreterType VisitOrExpr([NotNull] Parser.OrExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();

			InterpreterType left = VisitExpr(exprs[0]);
			InterpreterType right = VisitExpr(exprs[1]);

			if (left == InterpreterType.ERROR || right == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (left != InterpreterType.BOOL || right != InterpreterType.BOOL)
				return RaiseError(TypeErrorMessage.BOOLEAN_MISMATCH, context);

			return left;
		}

		public InterpreterType VisitParanExpr([NotNull] Parser.ParanExprContext context)
		{
			return VisitExpr(context.expr());
		}

		public InterpreterType VisitReturn([NotNull] Parser.ReturnContext context)
		{
			Parser.ExprContext expr = context.expr();
			InterpreterType returnType = expr == null ? InterpreterType.NONE : VisitExpr(expr);

			if (returnType == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (currentCallable.TypeOf == InterpreterType.Types.TRIGGER && returnType != InterpreterType.NONE)
				return RaiseError(TypeErrorMessage.RETURN_TRIGGERVAL, context);

			if (returnType != currentCallable.GetCallableRange())
				return RaiseError(TypeErrorMessage.RETURN_MISMATCH, context);

			return returnType;
		}

		public InterpreterType VisitReturnStat([NotNull] Parser.ReturnStatContext context)
		{
			return VisitReturn(context.@return());
		}

		public InterpreterType VisitStat([NotNull] Parser.StatContext context)
		{
			if (context is Parser.AssignStatContext)
				return VisitAssignStat((Parser.AssignStatContext)context);
			if (context is Parser.DecStatContext)
				return VisitDecStat((Parser.DecStatContext)context);
			if (context is Parser.ReturnStatContext)
				return VisitReturnStat((Parser.ReturnStatContext)context);
			if (context is Parser.ExprStatContext)
				return VisitExprStat((Parser.ExprStatContext)context);
			if (context is Parser.UnpackStatContext)
				return VisitUnpackStat((Parser.UnpackStatContext)context);
            if (context is Parser.LockStatContext)
                return VisitLockStat((Parser.LockStatContext)context);
			if (context is Parser.IfStatContext)
				return VisitIfStat((Parser.IfStatContext)context);
			if (context is Parser.WhileStatContext)
				return VisitWhileStat((Parser.WhileStatContext)context);
			if (context is Parser.ForStatContext)
				return VisitForStat((Parser.ForStatContext)context);
			if (context is Parser.ForeachStatContext)
				return VisitForeachStat((Parser.ForeachStatContext)context);
			throw new InternalTypeCheckerException("stat not recognized.");
		}

		public InterpreterType VisitStringType([NotNull] Parser.StringTypeContext context)
		{
			return InterpreterType.STRING;
		}

		public InterpreterType VisitStringVal([NotNull] Parser.StringValContext context)
		{
			return InterpreterType.STRING;
		}

		public InterpreterType VisitTerminal(ITerminalNode node)
		{
			return InterpreterType.NONE;
		}

		public InterpreterType VisitTypeName([NotNull] Parser.TypeNameContext context)
		{
			if (context is Parser.IntTypeContext)
				return VisitIntType((Parser.IntTypeContext)context);
			if (context is Parser.FloatTypeContext)
				return VisitFloatType((Parser.FloatTypeContext)context);
			if (context is Parser.BoolTypeContext)
				return VisitBoolType((Parser.BoolTypeContext)context);
			if (context is Parser.StringTypeContext)
				return VisitStringType((Parser.StringTypeContext)context);
			if (context is Parser.ListTypeContext)
				return VisitListType((Parser.ListTypeContext)context);
			if (context is Parser.TupleTypeContext)
				return VisitTupleType((Parser.TupleTypeContext)context);
			if (context is Parser.TriggerTypeContext)
				return VisitTriggerType((Parser.TriggerTypeContext)context);
			if (context is Parser.FunctionTypeContext)
				return VisitFunctionType((Parser.FunctionTypeContext)context);
			throw new InternalTypeCheckerException("typename not recognized.");
		}

		public InterpreterType VisitTypeVal([NotNull] Parser.TypeValContext context)
		{
			if (context is Parser.IntValContext)
				return VisitIntVal((Parser.IntValContext)context);
			if (context is Parser.FloatValContext)
				return VisitFloatVal((Parser.FloatValContext)context);
			if (context is Parser.BoolValContext)
				return VisitBoolVal((Parser.BoolValContext)context);
			if (context is Parser.StringValContext)
				return VisitStringVal((Parser.StringValContext)context);
			throw new InternalTypeCheckerException("typeval not recognized.");
		}

		public InterpreterType VisitValExpr([NotNull] Parser.ValExprContext context)
		{
			return VisitTypeVal(context.typeVal());
		}

		public InterpreterType VisitVarDec([NotNull] Parser.VarDecContext context)
		{
			return VisitVarDec(context, false);
		}

		public InterpreterType VisitVarDec([NotNull] Parser.VarDecContext context, bool global)
		{
			string varName = context.ID().GetText();
			InterpreterType varType = VisitTypeName(context.typeName());

			Parser.ExprContext expr = context.expr();
			if (expr != null)
			{
				InterpreterType exprType = VisitExpr(expr);

				if (exprType == InterpreterType.ERROR)
					return InterpreterType.ERROR;

				if (!AssignmentAllowed(varType, exprType))
					return RaiseError(TypeErrorMessage.VARDEC_MISMATCH, context);
			}

			if (!global && !currentScope.TryAddVariable(varName, varType))
				return RaiseError(TypeErrorMessage.VARDEC_REDEF, context);

			return varType;
		}

		public InterpreterType VisitNamespacePart([NotNull] Parser.NamespacePartContext context)
		{
			if (context is Parser.FunctionDecNamespaceContext)
				return VisitFunctionDecNamespace((Parser.FunctionDecNamespaceContext)context);
			if (context is Parser.GlobalDecNamespaceContext)
				return VisitGlobalDecNamespace((Parser.GlobalDecNamespaceContext)context);
			if (context is Parser.HandlerDecNamespaceContext)
				return VisitHandlerDecNamespace((Parser.HandlerDecNamespaceContext)context);
			throw new InternalTypeCheckerException("namespacepart not recognized.");
		}

		public InterpreterType VisitHandlerDecNamespace([NotNull] Parser.HandlerDecNamespaceContext context)
		{
			return VisitHandlerDec(context.handlerDec());
		}

		public InterpreterType VisitAwait([NotNull] Parser.AwaitContext context)
		{
			Parser.ExprContext exprContext = context.expr();
			InterpreterType etype = VisitExpr(exprContext);
			if (etype == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (!etype.IsTriggerable)
				return RaiseError(TypeErrorMessage.AWAIT_NOTTRIGGER, context);

			return new InterpreterType(InterpreterType.Types.TUPLE, etype.GetArgumentTypes());
		}

		public InterpreterType VisitTupleType([NotNull] Parser.TupleTypeContext context)
		{
			Parser.TypeNameContext[] typeContexts = context.typeName();
			InterpreterType[] types = new InterpreterType[typeContexts.Length];

			for (int i = 0; i < typeContexts.Length; i++)
				types[i] = VisitTypeName(typeContexts[i]);

			foreach (InterpreterType type in types)
				if (type == InterpreterType.ERROR)
					return InterpreterType.ERROR;

			return new InterpreterType(InterpreterType.Types.TUPLE, types);
		}

		public InterpreterType VisitListType([NotNull] Parser.ListTypeContext context)
		{
			Parser.TypeNameContext typeContext = context.typeName();
			InterpreterType type = VisitTypeName(typeContext);

			if (type == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			return new InterpreterType(InterpreterType.Types.LIST, type);
		}

		public InterpreterType VisitTupleExpr([NotNull] Parser.TupleExprContext context)
		{
			Parser.ExprContext[] exprContexts = context.expr();
			InterpreterType[] types = new InterpreterType[exprContexts.Length];

			for (int i = 0; i < exprContexts.Length; i++)
				types[i] = VisitExpr(exprContexts[i]);

			foreach (InterpreterType type in types)
				if (type == InterpreterType.ERROR)
					return InterpreterType.ERROR;

			return new InterpreterType(InterpreterType.Types.TUPLE, types);
		}

		public InterpreterType VisitListExpr([NotNull] Parser.ListExprContext context)
		{
			Parser.ExprContext[] exprContexts = context.expr();

			if (exprContexts.Length == 0)
				return InterpreterType.EMPTY_LIST;

			InterpreterType type = VisitExpr(exprContexts[0]);

			for (int i = 1; i < exprContexts.Length; i++)
            {
                InterpreterType nextType = VisitExpr(exprContexts[i]);
                if (!AssignmentAllowed(type, nextType))
                {
                    if (AssignmentAllowed(nextType, type))
                        type = nextType; // nextType is dominant
                    else
                        return RaiseError(TypeErrorMessage.LISTEXPR_ALL, context);
                }
            }

			return new InterpreterType(InterpreterType.Types.LIST, type);
		}

		public InterpreterType VisitIfStat([NotNull] Parser.IfStatContext context)
		{
			if (VisitIf(context.@if()) == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			foreach (Parser.ElseIfContext eifcontext in context.elseIf())
				if (VisitElseIf(eifcontext) == InterpreterType.ERROR)
					return InterpreterType.ERROR;

			Parser.ElseContext elseContext = context.@else();
			if (elseContext != null)
				if (VisitElse(elseContext) == InterpreterType.ERROR)
					return InterpreterType.ERROR;

			return InterpreterType.NONE;
		}

		public InterpreterType VisitForeachStat([NotNull] Parser.ForeachStatContext context)
		{
			return VisitForeach(context.@foreach());
		}

		public InterpreterType VisitForStat([NotNull] Parser.ForStatContext context)
		{
			return VisitFor(context.@for());
		}

		public InterpreterType VisitWhileStat([NotNull] Parser.WhileStatContext context)
		{
			return VisitWhile(context.@while());
		}

		public InterpreterType VisitIf([NotNull] Parser.IfContext context)
		{
			InterpreterType exprType = VisitExpr(context.expr());
			if (exprType == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (exprType != InterpreterType.BOOL)
				return RaiseError(TypeErrorMessage.STAT_BOOLEAN, context);

			return VisitBody(context.body());
		}

		public InterpreterType VisitElseIf([NotNull] Parser.ElseIfContext context)
		{
			InterpreterType exprType = VisitExpr(context.expr());
			if (exprType == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (exprType != InterpreterType.BOOL)
				return RaiseError(TypeErrorMessage.STAT_BOOLEAN, context);

			return VisitBody(context.body());
		}

		public InterpreterType VisitElse([NotNull] Parser.ElseContext context)
		{
			return VisitBody(context.body());
		}

		public InterpreterType VisitWhile([NotNull] Parser.WhileContext context)
		{
			InterpreterType exprType = VisitExpr(context.expr());
			if (exprType == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (exprType != InterpreterType.BOOL)
				return RaiseError(TypeErrorMessage.STAT_BOOLEAN, context);

			return VisitBody(context.body());
		}

		public InterpreterType VisitFor([NotNull] Parser.ForContext context)
		{
			currentScope.PushDepth();

			if (context.declare != null)
			{
				InterpreterType dec = VisitVarDec(context.declare);
				if (dec == InterpreterType.ERROR)
				{
					currentScope.PopDepth();
					return InterpreterType.ERROR;
				}
			}

			InterpreterType pred = VisitExpr(context.predicate);
			if (pred == InterpreterType.ERROR)
			{
				currentScope.PopDepth();
				return InterpreterType.ERROR;
			}
			if (pred != InterpreterType.BOOL)
			{
				currentScope.PopDepth();
				return RaiseError(TypeErrorMessage.STAT_BOOLEAN, context);
			}

			if (context.reeval != null)
			{
				InterpreterType reeval = VisitAssign(context.reeval);
				if (reeval == InterpreterType.ERROR)
				{
					currentScope.PopDepth();
					return InterpreterType.ERROR;
				}
			}

			InterpreterType body = VisitBody(context.body());

			currentScope.PopDepth();
			return body;
		}

		public InterpreterType VisitForeach([NotNull] Parser.ForeachContext context)
		{
			string iteratorName = context.ID().GetText();
			InterpreterType iteratorType = VisitTypeName(context.typeName());

			InterpreterType exprType = VisitExpr(context.expr());
			if (exprType == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (exprType.TypeOf != InterpreterType.Types.LIST)
				return RaiseError(TypeErrorMessage.FOREACH_LIST, context);

			if (!exprType.IsEmptyList && !AssignmentAllowed(iteratorType, exprType.TypeArray[0]))
				return RaiseError(TypeErrorMessage.FOREACH_ITERMISMATCH, context);

			Argument iteratorVar = new Argument(iteratorName, iteratorType);

			return VisitBody(context.body(), new Argument[] { iteratorVar });
		}

		public InterpreterType VisitIndexExpr([NotNull] Parser.IndexExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();

			InterpreterType etype = VisitExpr(exprs[0]);
			if (etype == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (etype.TypeOf == InterpreterType.Types.LIST)
				return ResolveListIndex(etype, exprs[1], context);
			else if (etype.TypeOf == InterpreterType.Types.TUPLE)
				return ResolveTupleIndex(etype, exprs[1], context);

			return RaiseError(TypeErrorMessage.INDEX_LORT, context);
		}

		public InterpreterType VisitTriggerType([NotNull] Parser.TriggerTypeContext context)
		{
			Parser.TypeNameContext[] typeNames = context.typeName();
			InterpreterType[] enclosedTypes = new InterpreterType[typeNames.Length];
			for (int i = 0; i < typeNames.Length; i++)
				enclosedTypes[i] = VisitTypeName(typeNames[i]);
			return new InterpreterType(InterpreterType.Types.TRIGGER, enclosedTypes);
		}

		public InterpreterType VisitFunctionType([NotNull] Parser.FunctionTypeContext context)
		{
			Parser.TypeNameContext[] typeContexts = context.typeName();

			InterpreterType[] types = new InterpreterType[typeContexts.Length];
			for (int i = 0; i < typeContexts.Length; i++)
				types[i] = VisitTypeName(typeContexts[i]);

			return new InterpreterType(InterpreterType.Types.FUNCTION, types);
		}

		public InterpreterType VisitImportNamespace([NotNull] Parser.ImportNamespaceContext context)
		{
            Parser.NamespaceContext[] namespaceContexts = context.@namespace();

            // Check if namespace really exists by the alias
            Namespace ns = ResolveNamespace(namespaceContexts[namespaceContexts.Length - 1]);
            if (ns == null)
                return RaiseError(TypeErrorMessage.NS_MISSING, context);

			return InterpreterType.NONE;
		}

		public InterpreterType VisitConcatExpr([NotNull] Parser.ConcatExprContext context)
		{
			Parser.ExprContext[] exprs = context.expr();
			InterpreterType left = VisitExpr(exprs[0]);
			InterpreterType right = VisitExpr(exprs[1]);

			if (left == InterpreterType.ERROR || right == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (left.TypeOf == InterpreterType.Types.LIST && right.TypeOf == InterpreterType.Types.LIST)
			{
                if (left.IsEmptyList)
                    return right;
                if (right.IsEmptyList)
                    return left;

				if (left != right)
				{
                    if (left == right.TypeArray[0])
                        return right;
                    else if (right == left.TypeArray[0])
                        return left;
                    else if (AssignmentAllowed(right, left))
                        return right;
                    else if (AssignmentAllowed(left, right))
                        return left;
                    else
                        return RaiseError(TypeErrorMessage.CONCAT_LIST, context);
				}
				return left;
			}
			else if (left.TypeOf == InterpreterType.Types.LIST)
			{
				if (!left.IsEmptyList && !AssignmentAllowed(left.TypeArray[0], right))
					return RaiseError(TypeErrorMessage.CONCAT_RIGHTM, context);
                if (left.IsEmptyList)
                    return new InterpreterType(InterpreterType.Types.LIST, right);
				return left;
			}
			else if (right.TypeOf == InterpreterType.Types.LIST)
			{
				if (!right.IsEmptyList && !AssignmentAllowed(right.TypeArray[0], left))
					return RaiseError(TypeErrorMessage.CONCAT_LEFTM, context);
                if (right.IsEmptyList)
                    return new InterpreterType(InterpreterType.Types.LIST, left);
                return right;
			}
			else
			{
				return RaiseError(TypeErrorMessage.CONCAT_MISMATCH, context);
			}
		}

		public InterpreterType VisitAwaitExpr([NotNull] Parser.AwaitExprContext context)
		{
			return VisitAwait(context.await());
		}

		public InterpreterType VisitExprStat([NotNull] Parser.ExprStatContext context)
		{
			return VisitExpr(context.expr());
		}

		public InterpreterType VisitUnpackStat([NotNull] Parser.UnpackStatContext context)
		{
			return VisitUnpack(context.unpack());
		}

		public InterpreterType VisitUnpack([NotNull] Parser.UnpackContext context)
		{
			InterpreterType exprType = VisitExpr(context.expr());
			if (exprType == InterpreterType.ERROR)
				return InterpreterType.ERROR;

			if (exprType.TypeOf != InterpreterType.Types.TUPLE)
				return RaiseError(TypeErrorMessage.UNPACK_NOTTUPLE, context);

			Parser.UnpackedContext[] unpackedArray = context.unpacked();
			InterpreterType[] unpackedTypes = new InterpreterType[unpackedArray.Length];
			for (int i = 0; i < unpackedArray.Length; i++)
			{
				unpackedTypes[i] = VisitUnpacked(unpackedArray[i]);
				if (unpackedTypes[i] == InterpreterType.ERROR)
					return InterpreterType.ERROR;
			}

			if (unpackedTypes.Length > exprType.TypeArray.Length)
				return RaiseError(TypeErrorMessage.UNPACK_TOOFEW, context);

			if (unpackedTypes.Length < exprType.TypeArray.Length)
				return RaiseError(TypeErrorMessage.UNPACK_TOOMANY, context);

			InterpreterType unpackedTuple = new InterpreterType(InterpreterType.Types.TUPLE, unpackedTypes);
			if (!AssignmentAllowed(unpackedTuple, exprType))
				return RaiseError(TypeErrorMessage.VARDEC_MISMATCH, context);

			return InterpreterType.NONE;
		}

		public InterpreterType VisitUnpacked([NotNull] Parser.UnpackedContext context)
		{
			Type t = context.GetType();
			if (context is Parser.IdUnpackedContext)
				return VisitIdUnpacked((Parser.IdUnpackedContext)context);
			if (context is Parser.DecUnpackedContext)
				return VisitDecUnpacked((Parser.DecUnpackedContext)context);
			if (context is Parser.IgnoreUnpackedContext)
				return VisitIgnoreUnpacked((Parser.IgnoreUnpackedContext)context);
			throw new InternalTypeCheckerException("unpacked alias not recognized.");
		}

		public InterpreterType VisitIdUnpacked([NotNull] Parser.IdUnpackedContext context)
		{
			return VisitIdentifier(context.identifier(), true);
		}

		public InterpreterType VisitDecUnpacked([NotNull] Parser.DecUnpackedContext context)
		{
			string varName = context.ID().GetText();
			InterpreterType varType = VisitTypeName(context.typeName());

			if (!currentScope.TryAddVariable(varName, varType))
				return RaiseError(TypeErrorMessage.VARDEC_REDEF, context);

			return varType;
		}

		public InterpreterType VisitIgnoreUnpacked([NotNull] Parser.IgnoreUnpackedContext context)
		{
			return InterpreterType.IGNORE; // Ignore assignment
		}

        public InterpreterType VisitLockStat([NotNull] Parser.LockStatContext context)
        {
            return VisitLock(context.@lock());
        }

        public InterpreterType VisitLock([NotNull] Parser.LockContext context)
        {
            InterpreterType type = ResolveIdentifier(context.identifier());
            if (type == InterpreterType.ERROR)
                return InterpreterType.ERROR;
            return VisitBody(context.body());
        }

        #endregion
    }
}
