using Api = HomeControlInterpreterInterface;

namespace HomeControlInterpreter.Interpretation.ThreadPool
{
	public interface IThreadPool
	{
		bool Running { get; }

		void QueueHandler(HandlerContext context);
		void Stop();
		void StopAndJoin();
		object[] Await(Api.SuppliedTrigger trigger);
	}
}
