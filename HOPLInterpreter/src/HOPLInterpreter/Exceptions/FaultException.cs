using HOPLInterpreter.Faults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Exceptions
{
    public abstract class FaultException : Exception
    {
		public virtual IEnumerable<IFault> Faults { get { return new List<IFault>(); } }
		public virtual string FaultName { get { return "Fault"; } }
    }
}
