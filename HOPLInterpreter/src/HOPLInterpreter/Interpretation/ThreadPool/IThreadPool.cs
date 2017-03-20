using HOPLInterpreter.Errors.Runtime;
using Api = HOPLInterpreterInterface;

namespace HOPLInterpreter.Interpretation.ThreadPool
{
	public delegate void RuntimeErrorEventHandler(object sender, RuntimeError e);

	public interface IThreadPool
	{
		BooleanRef Running { get; }

		event RuntimeErrorEventHandler RuntimeErrorEvent;

		void QueueHandler(HandlerContext context);
		int GetQueuedCount();
		void Stop();
		void StopAndJoin();
		object[] Await(Api.SuppliedTrigger trigger);
	}
}
