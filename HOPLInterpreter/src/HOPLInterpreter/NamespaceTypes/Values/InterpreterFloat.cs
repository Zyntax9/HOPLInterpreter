using System;

namespace HOPLInterpreter.NamespaceTypes.Values
{
	public class InterpreterFloat : InterpreterValue<float>
	{
		public InterpreterFloat() : base(0.0f) { }

		public InterpreterFloat(float value) : base(value) { }

		protected override InterpreterValue Add(InterpreterValue other)
		{
			return new InterpreterFloat(value + (float)other.Value);
		}

		protected override InterpreterValue Subtract(InterpreterValue other)
		{
			return new InterpreterFloat(value - (float)other.Value);
		}

		protected override InterpreterValue Multiply(InterpreterValue other)
		{
			return new InterpreterFloat(value * (float)other.Value);
		}

		protected override InterpreterValue Divide(InterpreterValue other)
		{
			return new InterpreterFloat(value / (float)other.Value);
		}

		protected override InterpreterValue Negate()
		{
			return new InterpreterFloat(-value);
		}

		protected override InterpreterValue Equal(InterpreterValue other)
		{
			return new InterpreterBool(value == (float)other.Value);
		}

		protected override InterpreterValue NEqual(InterpreterValue other)
		{
			return new InterpreterBool(value != (float)other.Value);
		}

		protected override InterpreterValue GEqual(InterpreterValue other)
		{
			return new InterpreterBool(value >= (float)other.Value);
		}

		protected override InterpreterValue LEqual(InterpreterValue other)
		{
			return new InterpreterBool(value <= (float)other.Value);
		}

		protected override InterpreterValue Greater(InterpreterValue other)
		{
			return new InterpreterBool(value > (float)other.Value);
		}

		protected override InterpreterValue Less(InterpreterValue other)
		{
			return new InterpreterBool(value < (float)other.Value);
		}
	}
}
