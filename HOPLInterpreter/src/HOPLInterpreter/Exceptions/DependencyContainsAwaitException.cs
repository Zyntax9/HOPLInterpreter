﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Exceptions
{
    public class DependencyContainsAwaitException : Exception
    {
		public DependencyContainsAwaitException(string msg) : base(msg) { }
    }
}
