using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Errors
{
    public interface IError
    {
		string ErrorTypeName { get; }
		string Message { get; }
		int ID { get; }
		int LineNumber { get; }
		int ColumnNumber { get; }
    }
}
