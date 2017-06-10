namespace HOPL.Interpreter.Exceptions
{
    public class MissingFunctionException : MissingCallableException
	{
		public MissingFunctionException(string functionName) : base(functionName) { }
	}
}
