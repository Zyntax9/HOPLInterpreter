using HOPLInterpreter.Interpretation;
using System;
using Api = HOPLInterpreterInterface;

namespace HOPLInterpreter.NamespaceTypes.Values
{
	public class InterpreterTrigger : InterpreterValue<Api.SuppliedTrigger>, IInterpreterTriggerable
	{
		private Lock fireLock = new Lock();
		private event Api.TriggerEventHandler onFire;

		private Lock refLock = new Lock();
		public delegate void ReferenceChangedHandler(object sender, InterpreterTrigger newTrigger);
		private event ReferenceChangedHandler referenceChanged;

		private InterpreterTrigger reference;

		public InterpreterTrigger() : base(new Api.SuppliedTrigger())
		{
			value.Subscribe(Fire);
		}

		public InterpreterTrigger(Api.SuppliedTrigger value) : base(value)
		{
			value.Subscribe(Fire);
		}

		public InterpreterTrigger(InterpreterTrigger trigger) : this()
		{
			SubscribeToReference(trigger);
		}

		public override InterpreterValue Clone()
		{
			return new InterpreterTrigger(this);
		}

		private void Fire(object sender, object[] arguments)
		{
			onFire?.Invoke(sender, arguments);
		}

		private void SubscribeToReference(InterpreterTrigger trigger)
		{
			value = trigger.value;
			reference = trigger;
			reference.Subscribe(Fire);
			reference.SubscribeReference(OnReferenceChange);
		}

		private void UnsubscribeReference()
		{
			reference.Unsubscribe(Fire);
			reference.UnsubscribeReference(OnReferenceChange);
			reference = null;
			value = null;
		}

		private void OnReferenceChange(object sender, InterpreterTrigger newTrigger)
		{
			UnsubscribeReference();
			value = newTrigger.value;
			SubscribeToReference(newTrigger);
		}

		public void ReferenceChanging(InterpreterTrigger newTrigger)
		{
			referenceChanged?.Invoke(this, newTrigger);
		}

		public void DropReference()
		{
			if (!ReferenceEquals(reference, null))
				UnsubscribeReference();
		}

		public void SubscribeReference(ReferenceChangedHandler handler)
		{
			lock (refLock)
				referenceChanged += handler;
		}

		public void UnsubscribeReference(ReferenceChangedHandler handler)
		{
			lock (refLock)
				referenceChanged -= handler;
		}

		public void Subscribe(Api.TriggerEventHandler handler)
		{
			lock (fireLock)
				onFire += handler;
		}

		public void Unsubscribe(Api.TriggerEventHandler handler)
		{
			lock (fireLock)
				onFire -= handler;
		}

		public void TransferSubscribers(IInterpreterTriggerable triggerable)
		{
			if (ReferenceEquals(triggerable, this))
				return; // Otherwise we will deadlock

			lock (fireLock)
			{
				foreach (Delegate del in onFire.GetInvocationList())
				{
					Api.TriggerEventHandler handler = (Api.TriggerEventHandler)del;
					onFire -= handler;
					triggerable.Subscribe(handler);
				}
			}
		}
	}
}
