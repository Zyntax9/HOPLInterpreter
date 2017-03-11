namespace HOPLInterpreter.Faults.Exploration
{
	public class ExploreFaultMessage
	{
		public readonly int id;
		public readonly string message;

		public static readonly ExploreFaultMessage GV_DUPL = new ExploreFaultMessage(1, "Global variable name is already in use.");
		public static readonly ExploreFaultMessage FUNC_DUPL = new ExploreFaultMessage(2, "Function name is already in use.");
		public static readonly ExploreFaultMessage FUNC_RETURN = new ExploreFaultMessage(3, "All paths of the function must return a value.");
		public static readonly ExploreFaultMessage ALIAS_OVERLAP = new ExploreFaultMessage(4, "Namespace import names must not overlap.");

		private ExploreFaultMessage(int id, string message)
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
