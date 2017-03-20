using HOPLInterpreter.Errors;
using HOPLInterpreter.Errors.Parsing;
using System;
using System.Collections.Generic;

namespace HOPLInterpreter.Exceptions
{
	public class ParsingErrorsException : ErrorException
	{
		private IEnumerable<ParsingError> errors;
		public override IEnumerable<IError> Errors { get { return errors; } }

		public override string ErrorName { get { return "Parsing Errors"; } }

		public ParsingErrorsException(IEnumerable<ParsingError> errors) : base()
		{
			this.errors = errors;
		}
	}
}
