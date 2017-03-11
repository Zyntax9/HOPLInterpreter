using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace HOPLInterpreter.NamespaceTypes
{
	public class ReadWriteLock
	{
		private enum AccessorType { READ, WRITE };
		private class Accessor
		{
			public AccessorType Type;
			public Thread Thread;
			public SemaphoreSlim Sem = new SemaphoreSlim(0, 1);

			public Accessor(AccessorType type, Thread thread)
			{
				Type = type;
				Thread = thread;
			}
		}
		private class BasicLock { }

		private Queue<Accessor> accessQueue = new Queue<Accessor>();
		private int readerCount = 0;
		private AccessorType currentAccessor = AccessorType.READ;
		private BasicLock readerCountLock = new BasicLock();
		private BasicLock currentAccessorLock = new BasicLock();

		private void Lock(AccessorType type)
		{
			Accessor accessor = null;
			lock (accessQueue)
			{
				lock (currentAccessorLock)
				{
					if (accessQueue.Count > 0 || currentAccessor == AccessorType.WRITE)
					{
						accessor = new Accessor(type, Thread.CurrentThread);
						accessQueue.Enqueue(accessor);
					}
				}
			}

			if (accessor != null)
				accessor.Sem.Wait();
			else
				lock (currentAccessorLock)
					currentAccessor = type;
		}

		public void Read()
		{
			Lock(AccessorType.READ);
			lock (readerCountLock) readerCount++;
		}

		public void Write()
		{
			Lock(AccessorType.WRITE);
		}

		public void ReadRelease()
		{
			lock (readerCountLock)
			{
				readerCount--;
				if(readerCount <= 0)
					ReleaseNext();
			}
		}

		public void WriteRelease()
		{
			ReleaseNext();
		}

		private void ReleaseNext()
		{
			lock (accessQueue)
			{
				if (accessQueue.Count <= 0)
				{
					lock (currentAccessorLock)
						currentAccessor = AccessorType.READ;
					return;
				}

				Accessor accessor = accessQueue.Dequeue();

				lock (currentAccessorLock)
					currentAccessor = accessor.Type;

				accessor.Sem.Release();

				if (accessor.Type == AccessorType.WRITE)
					return;

				while (accessQueue.Count > 0 && accessQueue.Peek().Type != AccessorType.WRITE)
					accessQueue.Dequeue().Sem.Release();
			}
		}
	}
}
