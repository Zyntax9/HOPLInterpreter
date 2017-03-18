using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Faults
{
    public interface IFault
    {
		string FaultTypeName { get; }
		string Message { get; }
		int ID { get; }
		int LineNumber { get; }
		int ColumnNumber { get; }
    }
}
