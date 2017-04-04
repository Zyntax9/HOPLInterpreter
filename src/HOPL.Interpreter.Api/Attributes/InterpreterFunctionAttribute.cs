using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPL.Interpreter.Api.Attributes
{
	[AttributeUsage(AttributeTargets.Method)]
	public class InterpreterFunctionAttribute : Attribute
	{
		public string Name { get; set; }
	}
}
