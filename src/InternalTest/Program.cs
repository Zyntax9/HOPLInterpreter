using System;
using System.Collections.Generic;
using System.IO;
using HOPL.Interpreter;
using HOPL.Interpreter.Interpretation.ThreadPool;
using HOPL.Interpreter.Interpretation;
using HOPL.Interpreter.NamespaceTypes;
using System.Threading;
using HOPL.Interpreter.Exceptions;
using HOPL.Interpreter.Errors;
using HOPL.Interpreter.Api;

namespace InternalTest
{
	public class Program
	{
		private static TimeSpan TIMEOUT = new TimeSpan(0, 0, 50);

		public static void Main(string[] args)
		{
			int success = 0;

			string[] files = Directory.GetFiles("./InternalTests", "*.hopl");
			foreach (string file in files)
			{
				UnitTestNamespace unitTestNamespace = new UnitTestNamespace(file);
				InterpretationContext context;
				if (!Prepare(file, unitTestNamespace, out context))
					continue;

				IThreadPool pool = Interpreter.Run(context, null);
				unitTestNamespace.Run();

				DateTime startTime = DateTime.Now;
				while(!unitTestNamespace.IsComplete() && DateTime.Now - startTime < TIMEOUT)
					Thread.Sleep(100);
				pool.StopAndJoin();

				if (DateTime.Now - startTime >= TIMEOUT)
					Console.WriteLine("Timeout on {0}", file);

				WriteStat(unitTestNamespace.TestCount, unitTestNamespace.SuccessCount,
					Path.GetFileName(file));

				if (unitTestNamespace.TestCount == unitTestNamespace.SuccessCount)
					success++;
			}

			Console.WriteLine("Completed internal tests! Successful tests {0}/{1}.", success, files.Length);

			Console.ReadKey();
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
			NamespaceSet namespaces = new NamespaceSet(new ISuppliedNamespace[] { unitTestNamespace });

			try
			{
				context = Interpreter.PrepareFile(file, importPaths, namespaces);
			}
			catch (ErrorException e)
			{
				Console.WriteLine("Test of {0} failed due to {1}:", file, e.ErrorName);
				foreach (IError error in e.Errors)
				{
					Console.WriteLine("\"{0}\" ({1}) at {2}:{3}", error.Message,
						error.ID, error.LineNumber, error.ColumnNumber);
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
