namespace HOPL.Interpreter.NamespaceTypes.Values
{
    public interface IInterpreterTriggerable
    {
		void Subscribe(Api.TriggerEventHandler handler);
		void Unsubscribe(Api.TriggerEventHandler handler);
		void TransferSubscribers(IInterpreterTriggerable to);
	}
}
