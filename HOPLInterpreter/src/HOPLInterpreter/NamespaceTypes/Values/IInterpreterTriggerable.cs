using Api = HomeControlInterpreterInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeControlInterpreter.NamespaceTypes.Values
{
    public interface IInterpreterTriggerable
    {
		void Subscribe(Api.TriggerEventHandler handler);
		void Unsubscribe(Api.TriggerEventHandler handler);
	}
}
