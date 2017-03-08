using HomeControlInterpreter.Exploration;
using HomeControlInterpreter.TypeCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Parser = HOPLGrammar.HOPLGrammarParser;

namespace HomeControlInterpreter.NamespaceTypes
{
	public class Argument
	{
		public string Name { get; protected set; }
		public InterpreterType Type { get; protected set; }

		public Argument(string name, InterpreterType type)
		{
			Name = name;
			Type = type;
		}

		public Argument(Parser.ArgContext argContext)
		{
			Name = argContext.ID().GetText();

			Parser.TypeNameContext type = argContext.typeName();
			TypeChecker typeChecker = new TypeChecker();
			Type = typeChecker.VisitTypeName(type);
		}

		public Argument(Parser.TypeNameContext typeContext)
		{
			Name = ""; // We don't care what name it takes

			TypeChecker typeChecker = new TypeChecker();
			Type = typeChecker.VisitTypeName(typeContext);
		}

		public Argument(ParameterInfo pi)
		{
			Name = pi.Name;
			Type = InterpreterType.FromNative(pi.ParameterType);
		}

		public override bool Equals(object obj)
		{
			if (obj.GetType() != typeof(Argument))
				return false;

			Argument a = (Argument)obj;

			return Type == a.Type && (a.Name == "" || Name == "" || a.Name == Name);
		}

		public static bool operator ==(Argument a, Argument b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Argument a, Argument b)
		{
			return !a.Equals(b);
		}
	}
}
