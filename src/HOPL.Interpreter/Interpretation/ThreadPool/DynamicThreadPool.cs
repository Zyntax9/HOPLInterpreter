using HOPL.Interpreter.Errors.Runtime;
using HOPL.Interpreter.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Api = HOPL.Interpreter.Api;

namespace HOPL.Interpreter.Interpretation.ThreadPool
{
	public class DynamicThreadPool : IThreadPool
    {
        private CancellationTokenSource cancelSource;
        private CancellationToken cancelToken;
        public bool Running { get { return !cancelToken.IsCancellationRequested; } }

        private LinkedList<Task> pool = new LinkedList<Task>();

		public event RuntimeErrorEventHandler RuntimeErrorEvent;

		public DynamicThreadPool()
        {
            cancelSource = new CancellationTokenSource();
            cancelToken = cancelSource.Token;
        }

		public void ThreadRuntime(object context)
		{
			ThreadContext tcontext = (ThreadContext)context;
			
			Executor executor = new Executor(tcontext.Handler, cancelToken, this);
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
            ThreadContext tcontext = new ThreadContext();
			tcontext.Handler = context;

            Task t = new Task(() => ThreadRuntime(tcontext));

            lock (pool)
				tcontext.ThreadNode = pool.AddLast(t);

            t.Start();
		}

		public int GetQueuedCount()
		{
			return 0;
		}

		public void Stop()
		{
            cancelSource.Cancel();
		}

		public void StopAndJoin()
		{
			Stop();
			lock (pool)
            {
                foreach (Task t in pool)
                {
                    try
                    {
                        t.Wait();
                    }
                    catch (AggregateException) { }
                }
            }
		}

		public object[] Await(Api.SuppliedTrigger trigger)
		{
			int currentId = Task.CurrentId.Value;
            Task current = null;
            foreach (Task t in pool)
                if (t.Id == currentId)
                    current = t;
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
			public LinkedListNode<Task> ThreadNode { get; set; }
			public HandlerContext Handler { get; set; }
		}
	}
}
