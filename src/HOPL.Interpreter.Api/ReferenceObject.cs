using System;
using System.Collections.Generic;
using System.Text;

namespace HOPL.Interpreter.Api
{
    public class ReferenceObject
    {
        public object Value { get; set; }

        public ReferenceObject(object value)
        {
            Value = value;
        }
    }
}
