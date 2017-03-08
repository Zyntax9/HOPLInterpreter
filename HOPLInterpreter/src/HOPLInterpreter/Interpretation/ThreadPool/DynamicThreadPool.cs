using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api = HomeControlInterpreterInterface;

namespace HomeControlInterpreter.Interpretation.ThreadPool
{
	public class DynamicThreadPool : IThreadPool
	{
		public bool Running { get; protected set; } = true;

		private LinkedList<Thread> pool = new LinkedList<Thread>();

		public DynamicThreadPool() { }

		public void ThreadRuntime(object context)
		{
			ThreadContext tcontext = (ThreadContext)context;
			
			Executor executor = new Executor(tcontext.Handler, this);
			executor.ExecuteHandler(tcontext.Handler.Handler.Context);

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

		public void Stop()
		{
			Running = false;
		}

		public void StopAndJoin()
		{
			Running = false;
			lock (pool)
			{
				foreach (Thread t in pool)
				{
					t.Join();
				}
			}
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
