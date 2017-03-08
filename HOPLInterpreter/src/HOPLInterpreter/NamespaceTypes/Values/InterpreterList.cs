﻿using Api = HomeControlInterpreterInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace HomeControlInterpreter.NamespaceTypes.Values
{
	public class InterpreterList : InterpreterValue<List<InterpreterValue>>, IInterpreterTriggerable
	{
		public Api.SuppliedTrigger collectiveTrigger { get; protected set; } = new Api.SuppliedTrigger();

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
			collectiveTrigger.Fire(sender, arguments);
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

		public override object ToNative()
		{
			List<object> nativeList = new List<object>();
			foreach (InterpreterValue value in value)
				nativeList.Add(value.ToNative());
			return nativeList;
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
			collectiveTrigger.Subscribe(handler);
		}

		public void Unsubscribe(Api.TriggerEventHandler handler)
		{
			collectiveTrigger.Unsubscribe(handler);
		}
	}
}