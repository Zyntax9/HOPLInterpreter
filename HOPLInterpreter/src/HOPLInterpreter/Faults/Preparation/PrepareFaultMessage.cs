namespace HomeControlInterpreter.Faults.Preparation
{
	public class PrepareFaultMessage
	{
		public readonly int id;
		public readonly string message;

		public static readonly PrepareFaultMessage NS_NOTFOUND = new PrepareFaultMessage(1, "Could not find imported namespace.");
		public static readonly PrepareFaultMessage NS_REQ = new PrepareFaultMessage(2, "Imported namespaces must not include required global entities.");
		public static readonly PrepareFaultMessage DEP_REC = new PrepareFaultMessage(3, "Recursive dependencies found in global entities.");
		public static readonly PrepareFaultMessage DEP_AWAIT = new PrepareFaultMessage(4, "Await statements must not be used in global entities or function calls from global entities.");

		private PrepareFaultMessage(int id, string message)
		{
			this.id = id;
			this.message = message;
		}

		public override string ToString()
		{
			return message;
		}
	}
}
