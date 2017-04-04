namespace HOPL.Interpreter.Errors.Exploration
{
	public class ExploreErrorMessage
	{
		public readonly int id;
		public readonly string message;

		public static readonly ExploreErrorMessage GV_DUPL = new ExploreErrorMessage(1, "Global variable name is already in use.");
		public static readonly ExploreErrorMessage FUNC_DUPL = new ExploreErrorMessage(2, "Function name is already in use.");
		public static readonly ExploreErrorMessage FUNC_RETURN = new ExploreErrorMessage(3, "All paths of the function must return a value.");
		public static readonly ExploreErrorMessage ALIAS_OVERLAP = new ExploreErrorMessage(4, "Namespace import names must not overlap.");

		private ExploreErrorMessage(int id, string message)
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
