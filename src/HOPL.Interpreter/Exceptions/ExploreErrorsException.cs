using HOPL.Interpreter.Errors.Exploration;
using System.Collections.Generic;
using HOPL.Interpreter.Errors;

namespace HOPL.Interpreter.Exceptions
{
	public class ExploreErrorsException : ErrorException
	{
		private ExploreErrorCollection errors;
		public override IEnumerable<IError> Errors { get { return errors; } }

		public override string ErrorName { get { return "Explore Errors"; } }

		public ExploreErrorsException(ExploreErrorCollection errors) : base()
		{
			this.errors = errors;
		}
	}
}
