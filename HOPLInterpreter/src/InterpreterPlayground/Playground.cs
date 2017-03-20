using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HOPLInterpreterInterface.Attributes;
using HOPLInterpreterInterface;
using System.Diagnostics.Tracing;

namespace InterpreterPlayground
{
	public class Playground : ISuppliedNamespace
	{
		public string Name { get { return "InterpreterPlayground.Playground"; } }

		[InterpreterGlobalVariable]
		public int gv_int { get; set; } = 10;

		[InterpreterGlobalVariable]
		public bool gv_bool { get; set; } = true;

		[InterpreterGlobalVariable]
		public string gv_string { get; set; } = "This is a supplied global string";

		[InterpreterGlobalVariable]
		public float gv_float { get; set; } = 0.00001f;

		[InterpreterGlobalVariable]
		public List<int> gv_list { get; set; } = new List<int>();

		[InterpreterGlobalVariable]
		public SuppliedTuple<string, float> gv_tuple { get; set; } = new SuppliedTuple<string, float>("Tuple", 3.14f);

		[InterpreterGlobalVariable]
		public SuppliedTrigger<int, string> trigger_test1 { get; set; } = new SuppliedTrigger<int, string>();

		[InterpreterGlobalVariable]
		public SuppliedTrigger trigger_test2 { get; set; } = new SuppliedTrigger();

		[InterpreterGlobalVariable]
		public SuppliedTrigger<string> console_input { get; set; } = new SuppliedTrigger<string>();

		public delegate int func(string f);

		[InterpreterFunction]
		public func ReturnsFunction()
		{
			return new func(Print);
		}

		[InterpreterFunction]
		public int PlaygroundFunction(float f)
		{
			return (int)Math.Round(f);
		}

		[InterpreterFunction]
		public int Print(string toPrint)
		{
			Console.WriteLine("> " + toPrint);
			return 0;
		}

		[InterpreterFunction]
		public string IntToString(int i)
		{
			return i.ToString();
		}
	}
}
