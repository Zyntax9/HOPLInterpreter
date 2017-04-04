using Api = HOPL.Interpreter.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HOPL.Interpreter.Interpretation.ThreadPool
{
	public delegate void ReadyEventHandler(AwaitingThread waiter);

    public class AwaitingThread
    {
		public Thread Thread { get; protected set; }
		public LinkedListNode<AwaitingThread> LinkNode { get; protected set; }
		public event ReadyEventHandler Ready;

		private object[] arguments = null;
		private Lock argumentLock = new Lock();
		public object[] Arguments
		{
			get
			{
				object[] args;
				lock (argumentLock)
					args = arguments;
				return args;
			}
		}

		private Api.SuppliedTrigger trigger;
		private Semaphore waitsem = new Semaphore(0, 1);

		public AwaitingThread(Thread thread, Api.SuppliedTrigger trigger)
		{
			Thread = thread;
			this.trigger = trigger;
			trigger.Subscribe(TriggerFired);
		}

		public void Wait()
		{
			waitsem.WaitOne();
		}

		public void Resume()
		{
			waitsem.Release();
		}

		public void AddToList(LinkedList<AwaitingThread> list)
		{
			LinkNode = list.AddLast(this);
		}

		private void TriggerFired(object sender, object[] arguments)
		{
			lock(argumentLock)
			{
				if (this.arguments != null)
					return; // Trigger has already been called once, and we weren't unsubscribed yet.
				this.arguments = arguments;
			}
			trigger.Unsubscribe(TriggerFired);
			Ready(this);
		}
    }
}
