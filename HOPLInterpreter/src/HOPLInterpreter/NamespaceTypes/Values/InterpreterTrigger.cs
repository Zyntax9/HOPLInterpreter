using Api = HOPLInterpreterInterface;

namespace HOPLInterpreter.NamespaceTypes.Values
{
	public class InterpreterTrigger : InterpreterValue<Api.SuppliedTrigger>, IInterpreterTriggerable
	{
		public InterpreterTrigger() : base(new Api.SuppliedTrigger()) { }

		public InterpreterTrigger(Api.SuppliedTrigger value) : base(value) { }

		public void Subscribe(Api.TriggerEventHandler handler)
		{
			value.Subscribe(handler);
		}

		public void Unsubscribe(Api.TriggerEventHandler handler)
		{
			value.Unsubscribe(handler);
		}
	}
}
