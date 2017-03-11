using Api = HomeControlInterpreterInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HOPLInterpreter.Exceptions;

namespace HOPLInterpreter.Interpretation.ThreadPool
{
	public class StaticThreadPool : IThreadPool
	{
		private Semaphore queueSemaphore = new Semaphore(0, int.MaxValue);
		private Queue<HandlerContext> queue = new Queue<HandlerContext>();
		public BooleanRef Running { get; protected set; } = new BooleanRef(true);

		private Thread[] pool;
		private LinkedList<AwaitingThread> awaiting = new LinkedList<AwaitingThread>();
		private Queue<AwaitingThread> ready = new Queue<AwaitingThread>();

		public StaticThreadPool(int poolSize)
		{
			pool = new Thread[poolSize];

			lock (pool)
			{
				for (int i = 0; i < poolSize; i++)
				{
					pool[i] = new Thread(new ThreadStart(ThreadRuntime));
					pool[i].Start();
				}
			}
		}

		private void ThreadRuntime()
		{
			while (Running.Value)
			{
				queueSemaphore.WaitOne();

				if (!Running.Value)
					break; // Finish if we stopped after wait

				AwaitingThread readyThread = null;
				lock (ready)
					if(ready.Count > 0)
						readyThread = ready.Dequeue();

				if(readyThread != null)
				{
					Thread current = Thread.CurrentThread;
					lock (pool)
					{
						int index = Array.FindIndex(pool, x => ReferenceEquals(x, current));
						pool[index] = readyThread.Thread;
						readyThread.Resume();
						return;
					}
				}

				HandlerContext handler;
				lock (queue)
					handler = queue.Dequeue();

				Executor executor = new Executor(handler, this, Running);
				try
				{
					executor.ExecuteHandler(handler.Handler.Context);
				}
				catch (ExecutorInterruptException) { }
			}
		}

		public void QueueHandler(HandlerContext context)
		{
			lock (queue)
				queue.Enqueue(context);
			queueSemaphore.Release();
		}

		public void Stop()
		{
			Running.Value = false;
		}

		public void StopAndJoin()
		{
			Running.Value = false;
			for (int i = 0; i < pool.Length; i++)
				queueSemaphore.Release(); // Wake up all waiting threads
			foreach (Thread t in pool)
				t.Join();
		}

		public object[] Await(Api.SuppliedTrigger trigger)
		{
			Thread current = Thread.CurrentThread;
			AwaitingThread currentAwait = new AwaitingThread(current, trigger);

			lock (awaiting)
				currentAwait.AddToList(awaiting);

			currentAwait.Ready += SetReady;

			lock (pool)
			{
				int index = Array.FindIndex(pool, x => ReferenceEquals(x, current));
				pool[index] = new Thread(new ThreadStart(ThreadRuntime));
				pool[index].Start();
			}

			currentAwait.Wait();

			return currentAwait.Arguments;
		}

		private void SetReady(AwaitingThread waitingThread)
		{
			lock (awaiting)
				awaiting.Remove(waitingThread.LinkNode);

			lock (ready)
				ready.Enqueue(waitingThread);

			queueSemaphore.Release();
		}
	}
}
