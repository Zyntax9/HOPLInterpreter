namespace HOPL.Interpreter.NamespaceTypes
{
    public interface IFunction
	{
		string Name { get; }
		string File { get; }
		FunctionSignature Signature { get; }
		Argument[] Arguments { get; }
	}
}
