using System;
using Api = HOPLInterpreterInterface;

namespace HOPLInterpreter.Interpretation.ThreadPool
{
	public interface IThreadPool
	{
		BooleanRef Running { get; }

		void QueueHandler(HandlerContext context);
		int GetQueuedCount();
		void Stop();
		void StopAndJoin();
		object[] Await(Api.SuppliedTrigger trigger);
	}
}
