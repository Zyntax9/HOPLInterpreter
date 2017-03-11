using HomeControlInterpreterInterface.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HOPLInterpreter.NamespaceTypes
{
	public class SuppliedFunction : IFunction
	{
		public string Name { get; protected set; }
		public string File { get { return "External"; } }
		public FunctionSignature Signature { get; protected set; }
		public Argument[] Arguments { get; protected set; }

		public MethodInfo Method { get; protected set; }
		public object Supplier { get; protected set; }

		public SuppliedFunction(MethodInfo mi, object suppNamespace, InterpreterFunctionAttribute attr)
		{
			Name = attr.Name ?? mi.Name;
			Signature = new FunctionSignature(mi);
			Method = mi;
			Supplier = suppNamespace;

			ParameterInfo[] parameters = mi.GetParameters();
			Arguments = new Argument[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				Arguments[i] = new Argument(parameters[i]);
			}
		}

		public SuppliedFunction(MethodInfo mi, object suppNamespace)
		{
			Name = mi.Name;
			Signature = new FunctionSignature(mi);
			Method = mi;
			Supplier = suppNamespace;

			ParameterInfo[] parameters = mi.GetParameters();
			Arguments = new Argument[parameters.Length];
			for (int i = 0; i < parameters.Length; i++)
			{
				Arguments[i] = new Argument(parameters[i]);
			}
		}
	}
}
