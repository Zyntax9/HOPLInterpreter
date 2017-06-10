using System;

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
