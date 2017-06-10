namespace HOPL.Interpreter.NamespaceTypes.Values
{
    public class InterpreterInt : InterpreterValue<int>
	{
		public InterpreterInt() : base(0) { }

		public InterpreterInt(int value) : base(value) { }

		protected override InterpreterValue Add(InterpreterValue other)
		{
			if(other is InterpreterFloat)
				return new InterpreterFloat(value + (float)other.Value);
			return new InterpreterInt(value + (int)other.Value);
		}

		protected override InterpreterValue Subtract(InterpreterValue other)
		{
			if (other is InterpreterFloat)
				return new InterpreterFloat(value - (float)other.Value);
			return new InterpreterInt(value - (int)other.Value);
		}

		protected override InterpreterValue Multiply(InterpreterValue other)
		{
			if (other is InterpreterFloat)
				return new InterpreterFloat(value * (float)other.Value);
			return new InterpreterInt(value * (int)other.Value);
		}

		protected override InterpreterValue Divide(InterpreterValue other)
		{
			if (other is InterpreterFloat)
				return new InterpreterFloat(value / (float)other.Value);
			return new InterpreterInt(value / (int)other.Value);
		}

		protected override InterpreterValue Negate()
		{
			return new InterpreterInt(-value);
		}

		protected override InterpreterValue Equal(InterpreterValue other)
		{
			if (other is InterpreterFloat)
				return new InterpreterBool(value == (float)other.Value);
			return new InterpreterBool(value == (int)other.Value);
		}

		protected override InterpreterValue NEqual(InterpreterValue other)
		{
			if (other is InterpreterFloat)
				return new InterpreterBool(value != (float)other.Value);
			return new InterpreterBool(value != (int)other.Value);
		}

		protected override InterpreterValue GEqual(InterpreterValue other)
		{
			if (other is InterpreterFloat)
				return new InterpreterBool(value >= (float)other.Value);
			return new InterpreterBool(value >= (int)other.Value);
		}

		protected override InterpreterValue LEqual(InterpreterValue other)
		{
			if (other is InterpreterFloat)
				return new InterpreterBool(value <= (float)other.Value);
			return new InterpreterBool(value <= (int)other.Value);
		}

		protected override InterpreterValue Greater(InterpreterValue other)
		{
			if (other is InterpreterFloat)
				return new InterpreterBool(value > (float)other.Value);
			return new InterpreterBool(value > (int)other.Value);
		}

		protected override InterpreterValue Less(InterpreterValue other)
		{
			if (other is InterpreterFloat)
				return new InterpreterBool(value < (float)other.Value);
			return new InterpreterBool(value < (int)other.Value);
		}
	}
}
