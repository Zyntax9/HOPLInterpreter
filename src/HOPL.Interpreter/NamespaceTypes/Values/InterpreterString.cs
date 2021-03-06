﻿namespace HOPL.Interpreter.NamespaceTypes.Values
{
    public class InterpreterString : InterpreterValue<string>
	{
		public InterpreterString() : base("") { }

		public InterpreterString(string value) : base(value) { }

		protected override InterpreterValue Add(InterpreterValue other)
		{
			return new InterpreterString(value + (string)other.Value);
		}
	}
}
