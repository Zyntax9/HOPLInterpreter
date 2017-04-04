using System;
using HOPL.Interpreter.Errors.Preparation;
using System.Collections.Generic;
using HOPL.Interpreter.Errors;

namespace HOPL.Interpreter.Exceptions
{
	public class PrepareErrorException : ErrorException
	{
		private PrepareError error;
		public override IEnumerable<IError> Errors { get { return new PrepareError[] { error }; } }

		public override string ErrorName { get { return "Prepare Errors"; } }

		public PrepareErrorException(PrepareError error) : base()
		{
			this.error = error;
		}

		public PrepareErrorException(PrepareErrorMessage msg, string info) : base()
		{
			this.error = new PrepareError(msg, info);
		}
	}
}
