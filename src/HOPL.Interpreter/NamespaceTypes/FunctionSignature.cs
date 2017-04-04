using HOPL.Interpreter.TypeCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Parser = HOPL.Grammar.HOPLGrammarParser;

namespace HOPL.Interpreter.NamespaceTypes
{
	public class FunctionSignature
	{
		public InterpreterType[] Domain { get; protected set; }
		public InterpreterType Range { get; private set; }

		public FunctionSignature(InterpreterType[] domain, InterpreterType range)
		{
			Domain = domain;
			Range = range;
		}

		public FunctionSignature(IEnumerable<InterpreterType> domain, InterpreterType range)
		{
			Domain = domain.ToArray();
			Range = range;
		}

		public FunctionSignature(Parser.FunctionDecContext functionContext)
		{
			Parser.TypeNameContext returnType = functionContext.typeName();
			Parser.ArgContext[] args = functionContext.args().arg();

			TypeChecker typeChecker = new TypeChecker();

			Domain = new InterpreterType[args.Length];
			for (int i = 0; i < args.Length; i++)
				Domain[i] = typeChecker.VisitArg(args[i]);

			Range = typeChecker.VisitTypeName(returnType);
		}

		public FunctionSignature(MethodInfo suppliedFunctionInfo)
		{
			Type returnType = suppliedFunctionInfo.ReturnType;
			ParameterInfo[] args = suppliedFunctionInfo.GetParameters();

			Domain = new InterpreterType[args.Length];
			for (int i = 0; i < args.Length; i++)
				Domain[i] = InterpreterType.FromNative(args[i].ParameterType);
			Range = InterpreterType.FromNative(returnType);
		}
	}
}
