using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HOPLGrammar;
using System.Reflection;
using HomeControlInterpreterInterface.Attributes;
using HomeControlInterpreterInterface;
using HOPLInterpreter.NamespaceTypes.Values;

namespace HOPLInterpreter.NamespaceTypes
{
	public class SuppliedGlobalEntity : IGlobalEntity
	{
		public bool Constant { get { return true; } }
		public bool Required { get { return false; } }
		public bool Supplied { get { return true; } }
		public bool HasDefaultValue { get { return false; } }

		public HOPLGrammarParser.ExprContext DefaultValue { get { return null; } }

		public string Name { get; protected set; }
		public InterpreterType Type { get; protected set; }
		public object Supplier { get; protected set; }
		public PropertyInfo Property { get; protected set; }
		private SuppliedFunction function;
		public InterpreterValue Value
		{
			get
			{
				if (function != null)
					return new InterpreterFunction(function);
				object suppVal = Property.GetValue(Supplier);
				return InterpreterValue.FromNative(suppVal);
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public SuppliedGlobalEntity(PropertyInfo pi, object suppNamespace, InterpreterGlobalVariableAttribute attr)
		{
			Name = attr.Name ?? pi.Name;
			Supplier = suppNamespace;
			Property = pi;
			Type = InterpreterType.FromNative(pi.PropertyType);
		}

		public SuppliedGlobalEntity(SuppliedFunction suppFunc)
		{
			Name = suppFunc.Name;
			Supplier = suppFunc.Supplier;
			Property = null;
			Type = new InterpreterType(suppFunc.Signature);
			function = suppFunc;
		}
	}
}
