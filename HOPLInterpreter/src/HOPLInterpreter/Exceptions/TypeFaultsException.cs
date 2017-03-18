using HOPLInterpreter.TypeCheck;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HOPLInterpreter.Faults.TypeCheck;
using HOPLInterpreter.Faults;

namespace HOPLInterpreter.Exceptions
{
	public class TypeFaultsException : FaultException
	{
		private TypeFaultCollection faults;
		public override IEnumerable<IFault> Faults { get { return faults; } }

		public override string FaultName { get { return "Type Faults"; } }

		public TypeFaultsException(TypeFaultCollection faults) : base()
		{
			this.faults = faults;
		}
	}
}
