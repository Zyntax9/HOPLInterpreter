using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPL.Interpreter.Api
{
    public class SupplierException : Exception
    {
		public string Supplier { get; protected set; }
		
		public SupplierException(string supplier, string message) : base(message)
		{
			Supplier = supplier;
		}
	}
}
