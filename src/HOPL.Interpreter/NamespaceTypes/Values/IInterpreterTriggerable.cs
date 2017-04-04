using Api = HOPL.Interpreter.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPL.Interpreter.NamespaceTypes.Values
{
    public interface IInterpreterTriggerable
    {
		void Subscribe(Api.TriggerEventHandler handler);
		void Unsubscribe(Api.TriggerEventHandler handler);
		void TransferSubscribers(IInterpreterTriggerable to);
	}
}
