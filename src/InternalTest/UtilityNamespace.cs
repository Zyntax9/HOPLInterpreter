using HOPL.Interpreter.Api;
using HOPL.Interpreter.Api.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InternalTest
{
    class UtilityNamespace : ISuppliedNamespace
    {
        public string Name => "Utility";

        [InterpreterFunction]
        public int Sleep(int milliseconds)
        {
            var t = Task.Run(async delegate
            {
                await Task.Delay(TimeSpan.FromMilliseconds(milliseconds));
                return 0;
            });
            t.Wait();
            return 1;
        }
    }
}
