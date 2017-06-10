using System;

namespace HOPL.Interpreter.Api.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
	public class InterpreterGlobalVariableAttribute : Attribute
	{
		public string Name { get; set; }
	}
}
