using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Errors.Runtime
{
	public class RuntimeErrorMessage
	{
		public readonly int id;
		public readonly string message;

		public static readonly RuntimeErrorMessage FUNC_UNINIT = new RuntimeErrorMessage(1, "Function types must be set to a value before call.");
		public static readonly RuntimeErrorMessage INDEX_OUT = new RuntimeErrorMessage(2, "Index is out of range.");

		private RuntimeErrorMessage(int id, string message)
		{
			this.id = id;
			this.message = message;
		}

		// Custom runtime error (id: 0)
		public RuntimeErrorMessage(string message)
		{
			this.id = 0;
			this.message = message;
		}

		public override string ToString()
		{
			return message;
		}
	}
}
