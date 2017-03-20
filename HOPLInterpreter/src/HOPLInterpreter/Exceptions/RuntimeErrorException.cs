using HOPLInterpreter.Errors;
using HOPLInterpreter.Errors.Runtime;
using System.Collections.Generic;

namespace HOPLInterpreter.Exceptions
{
	public class RuntimeErrorException : ErrorException
	{
		private RuntimeError error;
		public override IEnumerable<IError> Errors { get { return new RuntimeError[] { error }; } }

		public override string ErrorName { get { return "Runtime Errors"; } }

		public RuntimeErrorException(RuntimeError error)
		{
			this.error = error;
		}
	}
}
