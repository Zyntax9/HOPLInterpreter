using System;

namespace HOPL.Interpreter.NamespaceTypes.Values
{
	public class InterpreterFloat : InterpreterValue<float>
	{
		public InterpreterFloat() : base(0.0f) { }

		public InterpreterFloat(float value) : base(value) { }

		protected override InterpreterValue Add(InterpreterValue other)
		{
			if (other.Value.GetType() == typeof(int))
				return new InterpreterFloat(value + Convert.ToSingle((int)other.Value));
			return new InterpreterFloat(value + (float)other.Value);
		}

		protected override InterpreterValue Subtract(InterpreterValue other)
		{
			if (other.Value.GetType() == typeof(int))
				return new InterpreterFloat(value - Convert.ToSingle((int)other.Value));
			return new InterpreterFloat(value - (float)other.Value);
		}

		protected override InterpreterValue Multiply(InterpreterValue other)
		{
			if (other.Value.GetType() == typeof(int))
				return new InterpreterFloat(value * Convert.ToSingle((int)other.Value));
			return new InterpreterFloat(value * (float)other.Value);
		}

		protected override InterpreterValue Divide(InterpreterValue other)
		{
			if (other.Value.GetType() == typeof(int))
				return new InterpreterFloat(value / Convert.ToSingle((int)other.Value));
			return new InterpreterFloat(value / (float)other.Value);
		}

		protected override InterpreterValue Negate()
		{
			return new InterpreterFloat(-value);
		}

		protected override InterpreterValue Equal(InterpreterValue other)
		{
			if (other.Value.GetType() == typeof(int))
				return new InterpreterBool(value == Convert.ToSingle((int)other.Value));
			return new InterpreterBool(value == (float)other.Value);
		}

		protected override InterpreterValue NEqual(InterpreterValue other)
		{
			if (other.Value.GetType() == typeof(int))
				return new InterpreterBool(value != Convert.ToSingle((int)other.Value));
			return new InterpreterBool(value != (float)other.Value);
		}

		protected override InterpreterValue GEqual(InterpreterValue other)
		{
			if (other.Value.GetType() == typeof(int))
				return new InterpreterBool(value >= Convert.ToSingle((int)other.Value));
			return new InterpreterBool(value >= (float)other.Value);
		}

		protected override InterpreterValue LEqual(InterpreterValue other)
		{
			if (other.Value.GetType() == typeof(int))
				return new InterpreterBool(value <= Convert.ToSingle((int)other.Value));
			return new InterpreterBool(value <= (float)other.Value);
		}

		protected override InterpreterValue Greater(InterpreterValue other)
		{
			if (other.Value.GetType() == typeof(int))
				return new InterpreterBool(value > Convert.ToSingle((int)other.Value));
			return new InterpreterBool(value > (float)other.Value);
		}

		protected override InterpreterValue Less(InterpreterValue other)
		{
			if (other.Value.GetType() == typeof(int))
				return new InterpreterBool(value < Convert.ToSingle((int)other.Value));
			return new InterpreterBool(value < (float)other.Value);
		}
	}
}
