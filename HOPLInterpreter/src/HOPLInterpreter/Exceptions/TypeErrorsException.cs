using HOPLInterpreter.TypeCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HOPLInterpreter.Errors.TypeCheck;
using HOPLInterpreter.Errors;

namespace HOPLInterpreter.Exceptions
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
