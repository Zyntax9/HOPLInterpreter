﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.Exceptions
{
	public class MissingCallableException : Exception
	{
		public MissingCallableException(string callableName) : base(callableName) { }
	}
}