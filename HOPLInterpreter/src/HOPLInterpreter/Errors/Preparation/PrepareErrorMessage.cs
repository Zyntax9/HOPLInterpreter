namespace HOPLInterpreter.Errors.Preparation
{
	public class PrepareErrorMessage
	{
		public readonly int id;
		public readonly string message;

		public static readonly PrepareErrorMessage NS_NOTFOUND = new PrepareErrorMessage(1, "Could not find imported namespace.");
		public static readonly PrepareErrorMessage NS_REQ = new PrepareErrorMessage(2, "Imported namespaces must not include required global entities.");
		public static readonly PrepareErrorMessage DEP_REC = new PrepareErrorMessage(3, "Recursive dependencies found in global entities.");
		public static readonly PrepareErrorMessage DEP_AWAIT = new PrepareErrorMessage(4, "Await statements must not be used in global entities or function calls from global entities.");

		private PrepareErrorMessage(int id, string message)
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
