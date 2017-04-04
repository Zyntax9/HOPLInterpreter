using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPL.Interpreter.Errors
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
