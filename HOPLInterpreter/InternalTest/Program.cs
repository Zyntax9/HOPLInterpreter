using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using HOPLInterpreter;
using HOPLInterpreter.Interpretation.ThreadPool;
using HOPLInterpreter.Interpretation;
using HOPLInterpreter.NamespaceTypes;
using System.Threading;
using HOPLInterpreter.Exceptions;
using HOPLInterpreter.Faults.TypeCheck;
using HOPLInterpreter.Faults.Parsing;
using HOPLInterpreter.Faults;

namespace InternalTest
{
	public class Program
	{
		public static void Main(string[] args)
		{
			string[] files = Directory.GetFiles("./InternalTests", "*.hopl");
			foreach (string file in files)
			{
				UnitTestNamespace unitTestNamespace = new UnitTestNamespace(file);
				InterpretationContext context;
				if (!Prepare(file, unitTestNamespace, out context))
					continue;

				IThreadPool pool = Interpreter.Run(context, null);
				unitTestNamespace.Run();
				while(!unitTestNamespace.IsComplete())
					Thread.Sleep(100);
				pool.StopAndJoin();

				WriteStat(unitTestNamespace.TestCount, unitTestNamespace.SuccessCount,
					Path.GetFileName(file));
			}
			Console.ReadLine();
		}

		private static void WriteStat(int tests, int success, string file)
		{
			Console.WriteLine("Test for {0} complete: {1}/{2} successful.", file, success, tests);
		}

		private static bool Prepare(string file, UnitTestNamespace unitTestNamespace, 
			out InterpretationContext context)
		{
			context = null;

			HashSet<string> importPaths = new HashSet<string>();
			NamespaceSet namespaces = new NamespaceSet(new object[] { unitTestNamespace });

			try
			{
				context = Interpreter.PrepareFile(file, importPaths, namespaces);
			}
			catch (FaultException e)
			{
				Console.WriteLine("Test of {0} failed due to {1}:", file, e.FaultName);
				foreach (IFault fault in e.Faults)
				{
					Console.WriteLine("\"{0}\" ({1}) at {2}:{3}", fault.Message,
						fault.ID, fault.LineNumber, fault.ColumnNumber);
				}
				return false;
			}
			catch (Exception e)
			{
				Console.WriteLine("Test of {0} failed due to exception: {1}", file, e.Message);
				return false;
			}
			return true;
		}
	}
}
