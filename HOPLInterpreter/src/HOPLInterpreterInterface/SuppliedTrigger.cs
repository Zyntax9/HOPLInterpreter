﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace HOPLInterpreterInterface
{
	public delegate void TriggerEventHandler(object sender, object[] arguments);

	public class SuppliedTrigger
	{
		private class Lock { }
		
		private event TriggerEventHandler OnFire;
		private Lock eventLock = new Lock();

		public void Fire(object[] arguments)
		{
			OnFire?.Invoke(this, arguments);
		}

		public void Fire(object sender, object[] arguments)
		{
			OnFire?.Invoke(sender, arguments);
		}

		public void Subscribe(TriggerEventHandler handler)
		{
			lock (eventLock)
				OnFire += handler;
		}

		public void Unsubscribe(TriggerEventHandler handler)
		{
			lock (eventLock)
				OnFire -= handler;
		}
	}

	public class SuppliedTrigger<T0> : SuppliedTrigger { }
	public class SuppliedTrigger<T0, T1> : SuppliedTrigger { }
	public class SuppliedTrigger<T0, T1, T2> : SuppliedTrigger { }
	public class SuppliedTrigger<T0, T1, T2, T3> : SuppliedTrigger { }
	public class SuppliedTrigger<T0, T1, T2, T3, T4> : SuppliedTrigger { }
	public class SuppliedTrigger<T0, T1, T2, T3, T4, T5> : SuppliedTrigger { }
	public class SuppliedTrigger<T0, T1, T2, T3, T4, T5, T6> : SuppliedTrigger { }
	public class SuppliedTrigger<T0, T1, T2, T3, T4, T5, T6, T7> : SuppliedTrigger { }
	public class SuppliedTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8> : SuppliedTrigger { }
	public class SuppliedTrigger<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : SuppliedTrigger { }
}
