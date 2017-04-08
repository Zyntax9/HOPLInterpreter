using HOPL.Interpreter.Errors.Runtime;
using Api = HOPL.Interpreter.Api;

namespace HOPL.Interpreter.Interpretation.ThreadPool
{
	public delegate void RuntimeErrorEventHandler(object sender, RuntimeError e);

	public interface IThreadPool
	{
		bool Running { get; }

		event RuntimeErrorEventHandler RuntimeErrorEvent;

		void QueueHandler(HandlerContext context);
		int GetQueuedCount();
		void Stop();
		void StopAndJoin();
		object[] Await(Api.SuppliedTrigger trigger);
	}
}
