using System;
using System.Collections.Generic;
using System.Linq;
using HOPLInterpreter;
using HOPLInterpreter.Faults.Parsing;
using HOPLInterpreter.Faults.TypeCheck;
using HOPLInterpreter.Faults.Exploration;
using HOPLInterpreter.Exceptions;
using HOPLInterpreter.NamespaceTypes;
using System.Threading;
using HOPLInterpreter.Interpretation;
using HOPLInterpreter.Interpretation.ThreadPool;

namespace InterpreterPlayground
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// Supplied namespace
			Playground pg = new Playground();

			InterpretationContext interpreterContext = null;
			try
			{
				HashSet<string> importPaths = new HashSet<string>();
				importPaths.Add("./TestLibraries");
				NamespaceSet ns = new NamespaceSet(new object[] { pg });
				interpreterContext = Interpreter.PrepareFile("testscript.txt", importPaths, ns);
			}
			catch (ParsingFaultsException pfe)
			{
				Console.WriteLine("Parsing encountered {0} faults.", pfe.Faults.Count());
				foreach (ParsingFault pf in pfe.Faults)
				{
					Console.WriteLine("Message: {0}", pf.Message);
					Console.WriteLine("File: {0}", pf.File);
					Console.WriteLine("Position: {0}:{1}", pf.LineNumber, pf.ColumnNumber);
				}
				Console.ReadKey();
				return;
			}
			catch (ExploreFaultsException efe)
			{
				Console.WriteLine("Explorer encountered {0} faults.", efe.Faults.Count);
				foreach (ExploreFault ef in efe.Faults)
				{
					Console.WriteLine("Message: {0}", ef.Message);
					Console.WriteLine("File: {0}", ef.File);
					Console.WriteLine("Position: {0}:{1}", ef.LineNumber, ef.ColumnNumber);
				}
				Console.ReadKey();
				return;
			}
			catch (TypeFaultsException tfe)
			{
				Console.WriteLine("Type checker encountered {0} faults.", tfe.Faults.Count);
				foreach (TypeFault tf in tfe.Faults)
				{
					Console.WriteLine("Message: {0}", tf.Message);
					Console.WriteLine("File: {0}", tf.File);
					Console.WriteLine("Position: {0}:{1}", tf.LineNumber, tf.ColumnNumber);
				}
				Console.ReadKey();
				return;
			}
			catch (PrepareFaultException pfe)
			{
				Console.WriteLine("Preparation encountered fault.");
				Console.WriteLine("Message: {0}", pfe.Fault.Message);
				Console.WriteLine("Additional info: {0}", pfe.Fault.Info);
				Console.ReadKey();
				return;
			}
			Console.WriteLine("Preparation completed without error!");

			Console.WriteLine("Interpreter:");

			Dictionary<string, object> setupValues = new Dictionary<string, object>();
			setupValues.Add("someNamespace.sayHello", "Hello!");

			IThreadPool pool = Interpreter.Run(interpreterContext, setupValues, 4);

			//ReadWriteLockTest();
			while (pool.Running.Value)
			{
				string s = Console.ReadLine();
				switch (s.ToLower())
				{
					case "a":
						pg.trigger_test1.Fire(new object[] { 3, "Test" });
						break;
					case "b":
						pg.trigger_test2.Fire(null);
						break;
					case "e":
						pool.StopAndJoin();
						break;
					default:
						pg.console_input.Fire(new object[] { s });
						break;
				}
			}
		}

		private static void ReadWriteLockTest()
		{
			Console.WriteLine("Doing ReadWrite lock test.");

			ReadWriteLock rwlock = new ReadWriteLock();
			Lock consoleLock = new Lock();

			WriterContext wctx1 = new WriterContext() { rwlock = rwlock, id = 1 };
			Thread Writer1 = new Thread(new ParameterizedThreadStart(WriterTest));
			Writer1.Start(wctx1);

			WriterContext wctx2 = new WriterContext() { rwlock = rwlock, id = 2 };
			Thread Writer2 = new Thread(new ParameterizedThreadStart(WriterTest));
			Writer2.Start(wctx2);

			Thread[] readers1 = new Thread[10];
			for(int i = 0; i < 10; i++)
			{
				ReaderContext rctx = new ReaderContext() { rwlock = rwlock, id = i + 1, consoleLock = consoleLock };
				Thread reader = new Thread(new ParameterizedThreadStart(ReaderTest));
				reader.Start(rctx);
				readers1[i] = reader;
			}

			WriterContext wctx3 = new WriterContext() { rwlock = rwlock, id = 3 };
			Thread Writer3 = new Thread(new ParameterizedThreadStart(WriterTest));
			Writer3.Start(wctx3);

			Thread[] readers2 = new Thread[10];
			for (int i = 0; i < 10; i++)
			{
				ReaderContext rctx = new ReaderContext() { rwlock = rwlock, id = i + 11, consoleLock = consoleLock };
				Thread reader = new Thread(new ParameterizedThreadStart(ReaderTest));
				reader.Start(rctx);
				readers2[i] = reader;
			}

			WriterContext wctx4 = new WriterContext() { rwlock = rwlock, id = 4 };
			Thread Writer4 = new Thread(new ParameterizedThreadStart(WriterTest));
			Writer4.Start(wctx4);

			if (Writer4.IsAlive)
				Writer4.Join();

			Console.WriteLine("Success?");
		}

		private struct WriterContext { public ReadWriteLock rwlock; public int id; }
		private static void WriterTest(object context)
		{
			WriterContext ctx = (WriterContext)context;
			ctx.rwlock.Write();
			Console.WriteLine("I am a writer {0}!", ctx.id);
			Thread.Sleep(1000);
			Console.WriteLine("Writer {0} releases lock!", ctx.id);
			ctx.rwlock.WriteRelease();
		}

		private struct ReaderContext { public ReadWriteLock rwlock; public int id; public Lock consoleLock; }
		private static void ReaderTest(object context)
		{
			ReaderContext ctx = (ReaderContext)context;
			ctx.rwlock.Read();
			lock(ctx.consoleLock)
				Console.WriteLine("I am a reader {0}!", ctx.id);
			lock (ctx.consoleLock)
				Console.WriteLine("Reader {0} releases lock!", ctx.id);
			ctx.rwlock.ReadRelease();
		}
	}
}
