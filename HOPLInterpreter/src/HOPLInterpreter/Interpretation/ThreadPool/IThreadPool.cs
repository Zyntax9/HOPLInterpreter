using System;
using Api = HomeControlInterpreterInterface;

namespace HOPLInterpreter.Interpretation.ThreadPool
{
	public interface IThreadPool
	{
		BooleanRef Running { get; }

		void QueueHandler(HandlerContext context);
		void Stop();
		void StopAndJoin();
		object[] Await(Api.SuppliedTrigger trigger);
	}
}
