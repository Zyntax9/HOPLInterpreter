using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.NamespaceTypes.Values
{
	public class InterpreterBool : InterpreterValue<bool>
	{
		public InterpreterBool() : base(true) { }

		public InterpreterBool(bool value) : base(value) { }
		
		protected override InterpreterValue Not()
		{
			return new InterpreterBool(!value);
		}

		protected override InterpreterValue And(InterpreterValue other)
		{
			return new InterpreterBool(value && (bool)other.Value);
		}

		protected override InterpreterValue Or(InterpreterValue other)
		{
			return new InterpreterBool(value || (bool)other.Value);
		}

		protected override InterpreterValue Equal(InterpreterValue other)
		{
			return new InterpreterBool(value == (bool)other.Value);
		}
	}
}
