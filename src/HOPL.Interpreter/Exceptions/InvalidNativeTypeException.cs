using System;

namespace HOPL.Interpreter.Exceptions
{
    public class InvalidNativeTypeException : Exception
	{
		public Type NativeType { get; protected set; }
		public InvalidNativeTypeException(Type nativeType)
		{
			NativeType = nativeType;
		}
	}
}
