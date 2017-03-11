using HOPLInterpreter.NamespaceTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPLInterpreter.Interpretation.ThreadPool
{
    public class BooleanRef
    {
		private ReadWriteLock rwlock = new ReadWriteLock();
		private bool value;
		public bool Value {
			get
			{
				rwlock.Read();
				bool val = value;
				rwlock.ReadRelease();
				return val;
			}
			set
			{
				rwlock.Write();
				this.value = value;
				rwlock.WriteRelease();
			}
		}

		public BooleanRef(bool value = true) { this.value = value; }
    }
}
