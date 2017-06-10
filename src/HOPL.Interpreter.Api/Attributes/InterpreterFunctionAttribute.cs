using System;

namespace HOPL.Interpreter.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
	public class InterpreterFunctionAttribute : Attribute
	{
		public string Name { get; set; }
	}
}
