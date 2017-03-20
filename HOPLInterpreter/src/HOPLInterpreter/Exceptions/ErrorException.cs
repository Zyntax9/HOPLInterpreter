using HOPLInterpreter.Errors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Exceptions
{
    public abstract class ErrorException : Exception
    {
		public virtual IEnumerable<IError> Errors { get { return new List<IError>(); } }
		public virtual string ErrorName { get { return "Error"; } }
    }
}
