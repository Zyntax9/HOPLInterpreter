using Api = HOPLInterpreterInterface;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using HOPLInterpreter.Interpretation;
using System;

namespace HOPLInterpreter.NamespaceTypes.Values
{
	public class InterpreterList : InterpreterValue<List<InterpreterValue>>, IInterpreterTriggerable
	{
		private Lock eventLock = new Lock();
		private event Api.TriggerEventHandler triggerFire;
		
		public InterpreterList() : base(new List<InterpreterValue>()) { }

		public InterpreterList(IList value) : base(FromNativeList(value))
		{
			foreach(InterpreterValue val in value)
			{
				if(typeof(IInterpreterTriggerable).IsAssignableFrom(val.GetType()))
				{
					IInterpreterTriggerable triggerable = (IInterpreterTriggerable)val;
					triggerable.Subscribe(AnyFired);
				}
			}
		}

		public override int Count { get { return value.Count; } }

		private void AnyFired(object sender, object[] arguments)
		{
			triggerFire?.Invoke(sender, arguments);
		}

		private static List<InterpreterValue> FromNativeList(IList native)
		{
			List<InterpreterValue> interpreterList = new List<InterpreterValue>();
			foreach (object obj in native)
				interpreterList.Add(FromNative(obj));
			return interpreterList;
		}

		public override bool TypeEqual(InterpreterValue other)
		{
			if (GetType() != other.GetType())
				return false;
			InterpreterList otherList = (InterpreterList)other;
			List<InterpreterValue> olist = (List<InterpreterValue>)otherList.Value;

			int tcount = value.Count;
			int ocount = olist.Count;
			if (tcount == 0 || ocount == 0)
				return true; // One is empty, so it could be any list

			InterpreterValue tentry = value[0];
			InterpreterValue oentry = olist[0];

			return tentry.TypeEqual(oentry);
		}

		public override object ToNative(InterpreterType expected = null)
		{
			InterpreterType subexpect = expected?.TypeArray[0];

			List<object> nativeList = new List<object>();
			foreach (InterpreterValue value in value)
				nativeList.Add(value.ToNative(subexpect));

			Type generator;
			if(value.Count <= 0)
			{
				generator = expected.ToNative();
			}
			else
			{
				Type listGenericType = typeof(List<>);
				generator = listGenericType.MakeGenericType(nativeList.First().GetType());
			}

			ConstructorInfo ci = generator.GetConstructor(new Type[] { });

			IList result = (IList)ci.Invoke(new object[] { });

			foreach (object native in nativeList)
				result.Add(native);

			return result;
		}

		public override InterpreterValue Clone()
		{
			return new InterpreterList(value);
		}

		public override InterpreterValue this[int key]
		{
			get
			{
				return FromNative(value[key]);
			}
			set
			{
				this.value[key] = value.Clone();
			}
		}

		protected override InterpreterValue Equal(InterpreterValue other)
		{
			if (other.GetType() != typeof(InterpreterList))
				return new InterpreterBool(false);

			InterpreterList otherList = (InterpreterList)other;
			List<InterpreterValue> olist = (List<InterpreterValue>)otherList.Value;

			if (olist.Count != value.Count)
				return new InterpreterBool(false);

			for (int i = 0; i < value.Count; i++)
			{
				InterpreterValue equal = value[i] != olist[i];
				if ((bool)equal.Value)
					return new InterpreterBool(false);
			}

			return new InterpreterBool(true);
		}

		public void Subscribe(Api.TriggerEventHandler handler)
		{
			lock(eventLock)
				triggerFire += handler;
		}

		public void Unsubscribe(Api.TriggerEventHandler handler)
		{
			lock (eventLock)
				triggerFire -= handler;
		}

		public void TransferSubscribers(IInterpreterTriggerable triggerable)
		{
			if (ReferenceEquals(triggerable, this))
				return; // Otherwise we will deadlock

			lock (eventLock)
			{
				foreach (Delegate del in triggerFire.GetInvocationList())
				{
					Api.TriggerEventHandler handler = (Api.TriggerEventHandler)del;
					triggerFire -= handler;
					triggerable.Subscribe(handler);
				}
			}
		}
	}
}
