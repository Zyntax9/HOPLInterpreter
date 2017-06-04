using System;
using System.Collections.Generic;
using System.Linq;
using HOPL.Interpreter.Exceptions;
using HOPL.Interpreter.Errors.Runtime;
using System.Threading.Tasks;
using System.Threading;

namespace HOPL.Interpreter.Interpretation.ThreadPool
{
    public class StaticThreadPool : IThreadPool
	{
		private Semaphore queueSemaphore = new Semaphore(0, int.MaxValue);
		private Queue<HandlerContext> queue = new Queue<HandlerContext>();
        private CancellationTokenSource cancelSource;
        private CancellationToken cancelToken;
		public bool Running { get { return !cancelToken.IsCancellationRequested; } }

		private Task[] pool;
		private LinkedList<AwaitingThread> awaiting = new LinkedList<AwaitingThread>();
		private Queue<AwaitingThread> ready = new Queue<AwaitingThread>();

		public event RuntimeErrorEventHandler RuntimeErrorEvent;

		public StaticThreadPool(int poolSize)
		{
			pool = new Task[poolSize];
            cancelSource = new CancellationTokenSource();
            cancelToken = cancelSource.Token;

			lock (pool)
				for (int i = 0; i < poolSize; i++)
                    pool[i] = Task.Run((Action)ThreadRuntime);
		}

		private void ThreadRuntime()
		{
			while (Running)
			{
				queueSemaphore.WaitOne();

				if (!Running)
					break; // Finish if we stopped after wait

				AwaitingThread readyThread = null;
				lock (ready)
					if(ready.Count > 0)
						readyThread = ready.Dequeue();

				if(readyThread != null)
				{
                    int current = Task.CurrentId.Value;
					lock (pool)
					{
						int index = Array.FindIndex(pool, x => x.Id == current);
						pool[index] = readyThread.Thread;
						readyThread.Resume();
						return;
					}
				}

				HandlerContext handler;
				lock (queue)
					handler = queue.Dequeue();

				Executor executor = new Executor(handler, cancelToken, this);
				try
				{
					executor.ExecuteHandler(handler.Handler.Context);
				}
				catch (RuntimeErrorException e)
				{
					RuntimeErrorEvent?.Invoke(this, (RuntimeError)e.Errors.First());
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

		public int GetQueuedCount()
		{
			int c = 0;
			lock (queue)
				c = queue.Count;
			return c;
		}

		public void Stop()
		{
            cancelSource.Cancel();
		}

		public void StopAndJoin()
		{
			Stop();

			for (int i = 0; i < pool.Length; i++)
				queueSemaphore.Release(); // Wake up all waiting threads

			foreach (Task t in pool)
            {
                try
                {
                    t.Wait();
                }
                catch (AggregateException) { }
            }
		}

		public object[] Await(Api.SuppliedTrigger trigger)
		{
            int index = Array.FindIndex(pool, x => x.Id == Task.CurrentId.Value);
            Task current = pool[index];
			AwaitingThread currentAwait = new AwaitingThread(current, trigger);

			lock (awaiting)
				currentAwait.AddToList(awaiting);

			currentAwait.Ready += SetReady;

			lock (pool)
                pool[index] = Task.Run((Action)ThreadRuntime);

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
