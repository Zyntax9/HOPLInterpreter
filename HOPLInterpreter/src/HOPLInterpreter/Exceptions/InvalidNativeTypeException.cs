using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Exceptions
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
