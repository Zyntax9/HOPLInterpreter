using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreterInterface.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public class InterpreterNamespaceAttribute : Attribute
	{
		public string Name { get; set; }
	}
}
