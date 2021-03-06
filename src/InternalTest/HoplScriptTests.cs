﻿using System;
using System.Collections.Generic;
using System.Linq;
using HOPL.Interpreter;
using HOPL.Interpreter.Interpretation.ThreadPool;
using HOPL.Interpreter.Interpretation;
using HOPL.Interpreter.NamespaceTypes;
using System.Threading;
using HOPL.Interpreter.Exceptions;
using HOPL.Interpreter.Api;
using Xunit;
using Xunit.Abstractions;
using HOPL.Interpreter.Errors;
using HOPL.Interpreter.Errors.Runtime;

namespace InternalTest
{
    public class HoplScriptTests
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
        private readonly ITestOutputHelper output;

        public static IEnumerable<object[]> GetPreprocessErrorFiles()
        {
            foreach(ErroneousFile file in ErrorMapping.PreprocessingErrorFiles)
            {
                yield return new object[] { file };
            }
        }

        public static IEnumerable<object[]> GetRuntimeErrorFiles()
        {
            foreach (ErroneousFile file in ErrorMapping.RuntimeErrorFiles)
            {
                yield return new object[] { file };
            }
        }

        public HoplScriptTests(ITestOutputHelper output)
        {
            this.output = output;
        }
		
		[Theory]
		[InlineData("./InternalTests/Execution/ArithmeticOperators.hopl")]
        [InlineData("./InternalTests/Execution/Assignment.hopl")]
        [InlineData("./InternalTests/Execution/Await.hopl")]
        [InlineData("./InternalTests/Execution/ComparisonOperators.hopl")]
        [InlineData("./InternalTests/Execution/Conditionals.hopl")]
        [InlineData("./InternalTests/Execution/Indexing.hopl")]
        [InlineData("./InternalTests/Execution/ListOperators.hopl")]
        [InlineData("./InternalTests/Execution/Locking.hopl")]
        [InlineData("./InternalTests/Execution/LogicalOperators.hopl")]
		[InlineData("./InternalTests/Execution/Precendece.hopl")]
		[InlineData("./InternalTests/Execution/TriggerReference.hopl")]
		[InlineData("./InternalTests/Execution/Triggers.hopl")]
        [InlineData("./InternalTests/Execution/Unpacking.hopl")]
        private void RunFile(string file)
		{
            RunInPool(file, null); // Run in dynamic pool
            RunInPool(file, 2); // Run in static pool (size=2)
        }

        private void RunInPool(string file, int? threadMax = null)
        {
            UnitTestNamespace unitTestNamespace = new UnitTestNamespace(file, output);
            UtilityNamespace utilityNamespace = new UtilityNamespace();

            ISuppliedNamespace[] namespaces = { unitTestNamespace, utilityNamespace };

            Prepare(file, namespaces, out InterpretationContext context);

            IThreadPool pool = Interpreter.Run(context, new Dictionary<string, object>(), threadMax);
            unitTestNamespace.Run();

            DateTime startTime = DateTime.Now;
            while (!unitTestNamespace.IsComplete() && DateTime.Now - startTime < Timeout)
                Thread.Sleep(100);
            pool.StopAndJoin();

            if (DateTime.Now - startTime >= Timeout)
                Assert.True(false, $"Timeout on {file} ({unitTestNamespace.SuccessCount}/{unitTestNamespace.TestCount})");

            if (unitTestNamespace.TestCount != unitTestNamespace.SuccessCount)
                Assert.True(false, $"Test/Success mismatch ({unitTestNamespace.SuccessCount}/{unitTestNamespace.TestCount})");
        }

		private static void Prepare(string file, ISuppliedNamespace[] suppliedNamespaces, out InterpretationContext context)
		{
			context = null;

			HashSet<string> importPaths = new HashSet<string>();
			NamespaceSet namespaces = new NamespaceSet(suppliedNamespaces);

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
			}
			catch (Exception e)
			{
				Assert.True(false, "Failed due to exception: " + e.Message + e.StackTrace);
			}
		}

        [Theory]
        [MemberData(nameof(GetPreprocessErrorFiles))]
        public void ExpectPreprocessError(ErroneousFile file)
        {
            HashSet<string> importPaths = new HashSet<string>();
            if (file.ImportPaths != null)
                importPaths = new HashSet<string>(file.ImportPaths);
            NamespaceSet namespaces = new NamespaceSet();

            try
            {
                Interpreter.PrepareFile(file.File, importPaths, namespaces);
                Assert.True(false, $"{file.File} did unexpectedly succeed.");
            }
            catch (ErrorException e)
            {
                Assert.Equal(file.ErrorType, e.ErrorName);

                foreach(ExpectedError expectedError in file.ExpectedErrors)
                {
                    Assert.True(ContainsError(e, expectedError), 
                        $"Preprocess errors from preparing {file.File} did not contain error code {expectedError.ErrorCode} at line {expectedError.Line}.");
                }
            }
            catch (Exception e)
            {
                Assert.True(false, "Failed due to exception: " + e.Message + e.StackTrace);
            }
        }

        private bool ContainsError(ErrorException e, ExpectedError expected)
        {
            IError errorFound = (from error in e.Errors
                                 where error.ID == expected.ErrorCode &&
                                       error.LineNumber == expected.Line
                                 select error).FirstOrDefault();
            return errorFound != null;
        }

        [Theory]
        [MemberData(nameof(GetRuntimeErrorFiles))]
        public void ExpectRuntimeError(ErroneousFile file)
        {
            HashSet<string> importPaths = new HashSet<string>();
            if (file.ImportPaths != null)
                importPaths = new HashSet<string>(file.ImportPaths);
            
            UnitTestNamespace unitTestNamespace = new UnitTestNamespace(file.File, output);
            NamespaceSet namespaces = new NamespaceSet(new ISuppliedNamespace[] { unitTestNamespace });

            try
            {
                InterpretationContext context = Interpreter.PrepareFile(file.File, importPaths, namespaces);
            
                IThreadPool pool = Interpreter.Run(context, new Dictionary<string, object>());

                RuntimeError error = null;
                pool.RuntimeErrorHandler += (obj, e) => error = e;

                unitTestNamespace.Run();

                DateTime startTime = DateTime.Now;
                while (error == null && DateTime.Now - startTime < Timeout)
                    Thread.Sleep(100);
                pool.StopAndJoin();

                if(error == null)
                    Assert.True(false, $"{file.File} did unexpectedly succeed or get stuck.");

                ExpectedError expectedError = file.ExpectedErrors.FirstOrDefault();

                Assert.True(error.ID == expectedError.ErrorCode && error.LineNumber == expectedError.Line,
                    $"Runtime error from running {file.File} did not contain error code {expectedError.ErrorCode} at line {expectedError.Line}.");
            }
            catch (Exception e)
            {
                Assert.True(false, "Failed due to exception: " + e.Message + e.StackTrace);
                return;
            }
        }
    }
}
