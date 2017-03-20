using HOPLInterpreter.NamespaceTypes.Values;
using HOPLInterpreter.TypeCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Parser = HOPLGrammar.HOPLGrammarParser;

namespace HOPLInterpreter.NamespaceTypes
{
	public class GlobalEntity : IGlobalEntity
	{
		public string Name { get; protected set; }
		public InterpreterType Type { get; protected set; }
		public bool Required { get; protected set; }
		public bool Constant { get; protected set; }
		public bool Supplied { get { return false; } }
		public Parser.ExprContext DefaultValue { get; protected set; }
		public bool HasDefaultValue { get { return DefaultValue != null; } }

		private InterpreterValue value;
		private ReadWriteLock rwlock = new ReadWriteLock();
		public InterpreterValue Value
		{
			get
			{
				rwlock.Read();
				InterpreterValue retval = value;
				rwlock.ReadRelease();
				return retval;
			}
			set
			{
				rwlock.Write();
				if (!ReferenceEquals(this.value, null) && 
					this.value.GetType() == typeof(InterpreterTrigger))
				{
					// Change reference of all referencing triggers
					InterpreterTrigger trigger = (InterpreterTrigger)this.value;
					trigger.ReferenceChanging((InterpreterTrigger)value);
				}
				this.value = value;
				rwlock.WriteRelease();
			}
		}

		public GlobalEntity(Parser.GlobalDecContext globalDecContext)
		{
			DefaultValue = globalDecContext.varDec().expr();

			Parser.VarDecContext varDecContext = globalDecContext.varDec();

			Name = varDecContext.ID().GetText();

			TypeChecker typeChecker = new TypeChecker();
			Type = typeChecker.VisitTypeName(varDecContext.typeName());

			Required = globalDecContext.REQUIRED_KW() != null;
			Constant = globalDecContext.CONSTANT_KW() != null;
		}

		public GlobalEntity(Parser.FunctionDecContext funcDecContext, Namespace @namespace, string file)
		{
			Function f = new Function(funcDecContext, @namespace, file);

			Name = f.Name;
			Type = new InterpreterType(f.Signature);
			Value = new InterpreterFunction(f);

			Constant = true;
		}

		public GlobalEntity(Function function)
		{
			Name = function.Name;
			Type = new InterpreterType(function.Signature);
			Value = new InterpreterFunction(function);

			Constant = true;
		}
	}
}
