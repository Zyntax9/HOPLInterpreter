using HOPLInterpreter.Errors.Exploration;
using System.Collections.Generic;
using HOPLInterpreter.Errors;

namespace HOPLInterpreter.Exceptions
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
