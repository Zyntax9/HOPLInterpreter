using HOPL.Interpreter.Errors;
using System;
using System.Collections.Generic;

namespace HOPL.Interpreter.Exceptions
{
    public abstract class ErrorException : Exception
    {
		public virtual IEnumerable<IError> Errors { get { return new List<IError>(); } }
		public virtual string ErrorName { get { return "Error"; } }
    }
}
