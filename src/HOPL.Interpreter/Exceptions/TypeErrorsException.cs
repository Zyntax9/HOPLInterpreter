using System.Collections.Generic;
using HOPL.Interpreter.Errors.TypeCheck;
using HOPL.Interpreter.Errors;

namespace HOPL.Interpreter.Exceptions
{
    public class TypeErrorsException : ErrorException
	{
		private TypeErrorCollection errors;
		public override IEnumerable<IError> Errors { get { return errors; } }

		public override string ErrorName { get { return "Type Errors"; } }

		public TypeErrorsException(TypeErrorCollection errors) : base()
		{
			this.errors = errors;
		}
	}
}
