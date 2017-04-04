using HOPL.Interpreter.NamespaceTypes.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Parser = HOPL.Grammar.HOPLGrammarParser;

namespace HOPL.Interpreter.NamespaceTypes
{
	public interface IGlobalEntity
	{
		string Name { get; }
		InterpreterType Type { get; }
		InterpreterValue Value { get; set; }
		bool Required { get; }
		bool Constant { get; }
		bool Supplied { get; }
		Parser.ExprContext DefaultValue { get; }
		bool HasDefaultValue { get; }
	}
}
