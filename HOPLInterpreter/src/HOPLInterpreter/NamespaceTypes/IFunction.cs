﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parser = HOPLGrammar.HOPLGrammarParser;

namespace HomeControlInterpreter.NamespaceTypes
{
	public interface IFunction
	{
		string Name { get; }
		string File { get; }
		FunctionSignature Signature { get; }
		Argument[] Arguments { get; }
	}
}