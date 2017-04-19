using System;
using System.Collections.Generic;
using System.Linq;
using HOPL.Interpreter;
using HOPL.Interpreter.Interpretation.ThreadPool;
using HOPL.Interpreter.Interpretation;
using HOPL.Interpreter.NamespaceTypes;
using System.Threading;
using HOPL.Interpreter.Exceptions;
using HOPL.Interpreter.Errors;
using HOPL.Interpreter.Api;
using Xunit;

namespace InternalTest
{
	public class HoplScriptTests
	{
		private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(50);
		
		[Theory]
		[InlineData("./InternalTests/ArithmeticOperators.hopl")]
		[InlineData("./InternalTests/ComparisonOperators.hopl")]
		[InlineData("./InternalTests/LogicalOperators.hopl")]
		[InlineData("./InternalTests/Precendece.hopl")]
		[InlineData("./InternalTests/TriggerReference.hopl")]
		[InlineData("./InternalTests/Triggers.hopl")]
		private void RunFile(string file)
		{
			UnitTestNamespace unitTestNamespace = new UnitTestNamespace(file);
			if (!Prepare(file, unitTestNamespace, out InterpretationContext context))
				return;

			IThreadPool pool = Interpreter.Run(context, null);
			unitTestNamespace.Run();

			DateTime startTime = DateTime.Now;
			while (!unitTestNamespace.IsComplete() && DateTime.Now - startTime < Timeout)
				Thread.Sleep(100);
			pool.StopAndJoin();

			if (DateTime.Now - startTime >= Timeout)
				Assert.True(false, $"Timeout on {file}");
			
			if (unitTestNamespace.TestCount != unitTestNamespace.SuccessCount)
			{
				Assert.True(false);
			}
		}

		private static bool Prepare(string file, ISuppliedNamespace unitTestNamespace, out InterpretationContext context)
		{
			context = null;

			HashSet<string> importPaths = new HashSet<string>();
			NamespaceSet namespaces = new NamespaceSet(new[] { unitTestNamespace });

			try
			{
				context = Interpreter.PrepareFile(file, importPaths, namespaces);
			}
			catch (ErrorException e)
			{
				string errors = e.Errors
					.Select(error => $"\"{error.Message}\" ({error.ID}) at {error.LineNumber}:{error.ColumnNumber}")
					.Aggregate((current, next) => current + "\n" + next);

				Assert.True(false, errors);
				return false;
			}
			catch (Exception e)
			{
				Assert.True(false, "Failed due to exception: " + e.Message);
				return false;
			}
			return true;
		}
	}
}
