using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreterInterface.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class InterpreterTriggerAttribute : Attribute
    {
		public string Name { get; set; }
    }
}
