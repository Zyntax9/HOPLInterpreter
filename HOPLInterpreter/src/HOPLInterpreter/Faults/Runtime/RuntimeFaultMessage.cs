﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Faults.Runtime
{
	public class RuntimeFaultMessage
	{
		public readonly int id;
		public readonly string message;

		public static readonly RuntimeFaultMessage FUNC_UNINIT = new RuntimeFaultMessage(1, "Function types must be set to a value before call.");
		public static readonly RuntimeFaultMessage INDEX_OUT = new RuntimeFaultMessage(2, "Index is out of range.");

		private RuntimeFaultMessage(int id, string message)
		{
			this.id = id;
			this.message = message;
		}

		// Custom runtime fault (id: 0)
		public RuntimeFaultMessage(string message)
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