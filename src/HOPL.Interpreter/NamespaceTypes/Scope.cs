using HOPL.Interpreter.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPL.Interpreter.NamespaceTypes
{
	public abstract class Scope<T>
	{
		public int Depth { get; protected set; } = 0;
		public Namespace TopNamespace { get; protected set; }

		protected Dictionary<string, T> variables = new Dictionary<string, T>();
		protected List<List<string>> scopeLayout = new List<List<string>>();

		public Scope(Namespace topNamespace)
		{
			TopNamespace = topNamespace;
			scopeLayout.Add(new List<string>());
		}

		public virtual void AddVariable(string name, T value)
		{
			if (variables.ContainsKey(name))
				throw new DuplicateVariableException(name);
			variables.Add(name, value);
			scopeLayout[Depth].Add(name);
		}

		public virtual bool TryAddVariable(string name, T value)
		{
			if (variables.ContainsKey(name))
				return false;
			variables.Add(name, value);
			scopeLayout[Depth].Add(name);
			return true;
		}

		public virtual T GetVariable(string name)
		{
			T t;
			if (!TryGetVariable(name, out t))
				throw new MissingVariableException(name);
			return t;
		}

		public virtual bool TryGetVariable(string name, out T type)
		{
			return variables.TryGetValue(name, out type);
		}

		public virtual bool IsConstant(string name)
		{
			IGlobalEntity ge;
			if (!TopNamespace.TryGetGlobalEntity(name, out ge))
				return false;
			return ge.Constant;
		}

		public virtual void PushDepth()
		{
			Depth++;
			scopeLayout.Add(new List<string>());
		}

		public virtual void PopDepth()
		{
			if (Depth <= 0)
				throw new DepthTooLowException();

			foreach (string name in scopeLayout[Depth])
				variables.Remove(name);

			scopeLayout.RemoveAt(Depth);
			Depth--;
		}
	}
}
