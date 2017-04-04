using HOPL.Interpreter.Errors.Runtime;
using HOPL.Interpreter.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Api = HOPL.Interpreter.Api;

namespace HOPL.Interpreter.Interpretation.ThreadPool
{
	public class DynamicThreadPool : IThreadPool
	{
		public BooleanRef Running { get; protected set; } = new BooleanRef();

		private LinkedList<Thread> pool = new LinkedList<Thread>();

		public event RuntimeErrorEventHandler RuntimeErrorEvent;

		public DynamicThreadPool() { }

		public void ThreadRuntime(object context)
		{
			ThreadContext tcontext = (ThreadContext)context;
			
			Executor executor = new Executor(tcontext.Handler, this, Running);
			try
			{
				executor.ExecuteHandler(tcontext.Handler.Handler.Context);
			}
			catch (RuntimeErrorException e)
			{
				RuntimeErrorEvent?.Invoke(this, (RuntimeError)e.Errors.First());
			}
			catch (ExecutorInterruptException) { }

			lock (pool)
				pool.Remove(tcontext.ThreadNode);
		}

		public void QueueHandler(HandlerContext context)
		{
			Thread t = new Thread(new ParameterizedThreadStart(ThreadRuntime));

			ThreadContext tcontext = new ThreadContext();
			tcontext.Handler = context;

			lock (pool)
				tcontext.ThreadNode = pool.AddLast(t);

			t.Start(tcontext);
		}

		public int GetQueuedCount()
		{
			return 0;
		}

		public void Stop()
		{
			Running.Value = false;
		}

		public void StopAndJoin()
		{
			Stop();
			lock (pool)
				foreach (Thread t in pool)
					t.Join();
		}

		public object[] Await(Api.SuppliedTrigger trigger)
		{
			Thread current = Thread.CurrentThread;
			AwaitingThread currentAwait = new AwaitingThread(current, trigger);
			currentAwait.Ready += ResumeAwaiting;
			currentAwait.Wait();
			return currentAwait.Arguments;
		}

		public void ResumeAwaiting(AwaitingThread awaiting)
		{
			awaiting.Resume();
		}

		private struct ThreadContext
		{
			public LinkedListNode<Thread> ThreadNode { get; set; }
			public HandlerContext Handler { get; set; }
		}
	}
}
