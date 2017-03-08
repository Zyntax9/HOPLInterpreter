﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Exceptions
{
	public class MissingNamespaceException : Exception
	{
		public MissingNamespaceException(string namespaceName) : base(namespaceName) { }
	}
}