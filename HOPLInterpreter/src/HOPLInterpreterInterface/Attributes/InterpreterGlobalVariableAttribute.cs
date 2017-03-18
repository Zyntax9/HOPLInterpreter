using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreterInterface.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class InterpreterGlobalVariableAttribute : Attribute
	{
		public string Name { get; set; }
	}
}
