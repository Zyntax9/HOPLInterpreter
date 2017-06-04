using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using HOPL.Interpreter.Api.Attributes;
using HOPL.Interpreter.Api;
using System.Threading;
using System.Diagnostics;
using Xunit.Abstractions;

namespace InternalTest
{
    public class UnitTestNamespace : ISuppliedNamespace
    {
		public string Name => "UTest"; 

		private class Lock { }
		private Lock consoleLock = new Lock();

        private Lock countLock = new Lock();
		public int TestCount { get; private set; } = 0;
		public int FailCount { get; private set; } = 0;
		public int SuccessCount { get; private set; } = 0;

        private ITestOutputHelper output;

        private string file;
		public string File
		{
			get
			{
				return file;
			}
			set
			{
				TestCount = 0;
				FailCount = 0;
				SuccessCount = 0;
				file = value;
			}
		}

		public UnitTestNamespace(string testFile, ITestOutputHelper output)
		{
			File = testFile;
            this.output = output;
		}

		private bool Assertion(bool assert, string location)
		{
			lock(countLock)
			{
				if (!assert)
				{
					FailCount++;
					AssertionFailed(location);
				}
				else
				{
					SuccessCount++;
                    lock (consoleLock)
                        output.WriteLine($"Completed {location}");
				}
			}
			return assert;
		}

		private void AssertionFailed(string location)
		{
			lock (consoleLock)
				output.WriteLine($"Assertion failed at: {location} ({Path.GetFileName(File)})");
		}

		public void Run()
		{
			BeforeTests.Fire(new object[0]);
			Thread.Sleep(100);
			RunTests.Fire(new object[0]);
		}

        public void Reset()
        {
            TestCount = 0;
		    FailCount = 0;
            SuccessCount = 0;
        }

		public bool IsComplete()
		{
			bool done = false;
			lock (countLock)
				done = (SuccessCount + FailCount) >= TestCount;
			return done;
		}

		[InterpreterGlobalVariable]
		public SuppliedTrigger BeforeTests { get; set; } = new SuppliedTrigger();

		[InterpreterGlobalVariable]
		public SuppliedTrigger RunTests { get; set; } = new SuppliedTrigger();

		[InterpreterFunction]
		public int SetTestCount(int tests)
		{
			lock(countLock)
				TestCount = tests;
			return tests;
		}

		#region Boolean assertions
		[InterpreterFunction]
		public bool AssertBoolTrue(bool b, string location)
		{
			return Assertion(b, location);
		}

		[InterpreterFunction]
		public bool AssertBoolFalse(bool b, string location)
		{
			return Assertion(!b, location);
        }

        [InterpreterFunction]
        public bool AssertBoolEqual(bool a, bool b, string location)
        {
            return Assertion(a == b, location);
        }
        #endregion

        #region Integer assertions
        [InterpreterFunction]
		public bool AssertIntEqual(int a, int b, string location)
		{
			return Assertion(a == b, location);
		}

		[InterpreterFunction]
		public bool AssertIntNotEqual(int a, int b, string location)
		{
			return Assertion(a != b, location);
		}

		[InterpreterFunction]
		public bool AssertIntLess(int a, int b, string location)
		{
			return Assertion(a < b, location);
		}

		[InterpreterFunction]
		public bool AssertIntLessOrEqual(int a, int b, string location)
		{
			return Assertion(a <= b, location);
		}

		[InterpreterFunction]
		public bool AssertIntGreater(int a, int b, string location)
		{
			return Assertion(a > b, location);
		}

		[InterpreterFunction]
		public bool AssertIntGreaterOrEqual(int a, int b, string location)
		{
			return Assertion(a >= b, location);
		}

		[InterpreterFunction]
		public bool AssertIntNegation(int negative, int positive, string location)
		{
			return Assertion(negative == -positive, location);
		}
		#endregion

		#region Float assertions
		const float floatCompPrecision = 0.000001f;

		[InterpreterFunction]
		public bool AssertFloatEqual(float a, float b, string location)
		{
			return Assertion(Math.Abs(a - b) < floatCompPrecision, location);
		}

		[InterpreterFunction]
		public bool AssertFloatNotEqual(float a, float b, string location)
		{
			return Assertion(Math.Abs(a - b) > floatCompPrecision, location);
		}

		[InterpreterFunction]
		public bool AssertFloatLess(float a, float b, string location)
		{
			return Assertion(a < b, location);
		}

		[InterpreterFunction]
		public bool AssertFloatLessOrEqual(float a, float b, string location)
		{
			return Assertion(a <= b, location);
		}

		[InterpreterFunction]
		public bool AssertFloatGreater(float a, float b, string location)
		{
			return Assertion(a > b, location);
		}

		[InterpreterFunction]
		public bool AssertFloatGreaterOrEqual(float a, float b, string location)
		{
			return Assertion(a >= b, location);
		}

		[InterpreterFunction]
		public bool AssertFloatNegation(float negative, float positive, string location)
		{
			return Assertion(negative == -positive, location);
		}
		#endregion

		#region String assertions
		[InterpreterFunction]
		public bool AssertStringEqual(string a, string b, string location)
		{
			return Assertion(a == b, location);
		}

		[InterpreterFunction]
		public bool AssertStringNotEqual(string a, string b, string location)
		{
			return Assertion(a != b, location);
		}
		#endregion

		#region List assertions
		public bool AssertListEqual(List<object> a, List<object> b, string location)
		{
			bool assert = true;
			if (a.Count != b.Count)
				assert = false;
			else
				for(int i = 0; i < a.Count; i++)
					if (!a[i].Equals(b[i]))
						assert = false;
			
			lock (countLock)
			{
				if (!assert)
				{
					FailCount++;
					AssertionFailed(location);
				}
				else
				{
					SuccessCount++;
				}
			}
			return assert;
		}

		[InterpreterFunction]
		public bool AssertIntListEqual(List<int> a, List<int> b, string location)
		{
			List<object> ao = a.Cast<object>().ToList();
			List<object> bo = b.Cast<object>().ToList();
			return AssertListEqual(ao, bo, location);
		}

		[InterpreterFunction]
		public bool AssertFloatListEqual(List<float> a, List<float> b, string location)
		{
			List<object> ao = a.Cast<object>().ToList();
			List<object> bo = b.Cast<object>().ToList();
			return AssertListEqual(ao, bo, location);
		}

		[InterpreterFunction]
		public bool AssertBoolListEqual(List<bool> a, List<bool> b, string location)
		{
			List<object> ao = a.Cast<object>().ToList();
			List<object> bo = b.Cast<object>().ToList();
			return AssertListEqual(ao, bo, location);
		}

		[InterpreterFunction]
		public bool AssertStringListEqual(List<string> a, List<string> b, string location)
		{
			List<object> ao = a.Cast<object>().ToList();
			List<object> bo = b.Cast<object>().ToList();
			return AssertListEqual(ao, bo, location);
		}
		#endregion
	}
}
