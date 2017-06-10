using HOPL.Interpreter.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Parser = HOPL.Grammar.HOPLGrammarParser;
using HOPL.Interpreter.Api.Attributes;
using HOPL.Interpreter.Api;
using System.Collections;

namespace HOPL.Interpreter.NamespaceTypes
{
    public class NamespaceSet : IEnumerable<KeyValuePair<string, Namespace>>
	{
		private Dictionary<string, Namespace> namespaces = new Dictionary<string, Namespace>();
		public Namespace this[string namespaceID] { get { return Get(namespaceID); } }

		public NamespaceSet() { }

		public NamespaceSet(IEnumerable<Namespace> namespaces)
		{
			foreach (Namespace ns in namespaces)
				Add(ns);
		}

		public NamespaceSet(IEnumerable<ISuppliedNamespace> suppliedNamespaces)
		{
            foreach (ISuppliedNamespace sn in suppliedNamespaces)
                Add(sn);
		}

        public Namespace Add(ISuppliedNamespace suppliedNamespace)
        {
            TypeInfo t = suppliedNamespace.GetType().GetTypeInfo();

            string nsname = suppliedNamespace.Name ?? t.FullName;

            Namespace ns = Add(nsname);

            foreach (PropertyInfo pi in t.GetProperties())
                TryAddToNamespace(pi, suppliedNamespace, ns);

            foreach (MethodInfo mi in t.GetMethods())
                TryAddToNamespace(mi, suppliedNamespace, ns);

            return ns;
        }

        private bool TryAddToNamespace(PropertyInfo property, ISuppliedNamespace suppliedNamespace, Namespace ns)
        {
            if (property.PropertyType == typeof(ReferenceCollection))
                return TryAddToNamespace((ReferenceCollection)property.GetValue(suppliedNamespace), ns);

            InterpreterGlobalVariableAttribute ge =
                property.GetCustomAttribute<InterpreterGlobalVariableAttribute>();

            if (ge != null)
            {
                SuppliedGlobalEntity sge = new SuppliedGlobalEntity(property, suppliedNamespace, ge);
                ns.AddGlobalEntity(sge);
                return true;
            }
            return false;
        }

        private bool TryAddToNamespace(ReferenceCollection referenceCollection, Namespace ns)
        {
            foreach(KeyValuePair<string, ReferenceObject> trigger in referenceCollection)
            {
                IGlobalEntity ge = new ReferenceGlobalEntity(trigger.Key, trigger.Value);
                ns.AddGlobalEntity(ge);
            }
            return false;
        }

        private bool TryAddToNamespace(MethodInfo method, ISuppliedNamespace suppliedNamespace, Namespace ns)
        {
            InterpreterFunctionAttribute fun = method.GetCustomAttribute<InterpreterFunctionAttribute>();

            if (fun != null)
            {
                SuppliedFunction sfun = new SuppliedFunction(method, suppliedNamespace, fun);
                SuppliedGlobalEntity sfge = new SuppliedGlobalEntity(sfun);
                ns.AddFunction(sfun);
                ns.AddGlobalEntity(sfge);
                return true;
            }
            return false;
        }

        public Namespace Add(IEnumerable<string> namespaceParts)
		{
			string topNamespace = namespaceParts.ElementAt(0);

			Namespace n;
			if (namespaces.TryGetValue(topNamespace, out n))
			{
				n = n.AddChildNamespace(namespaceParts.Skip(1));
				return n;
			}
			else
			{
				n = new Namespace(namespaceParts);
				namespaces.Add(topNamespace, n);
				return n.GetNamespace(namespaceParts);
			}
		}

		public Namespace Add(string namespaceID)
		{
			return Add(Namespace.SplitNamespaceParts(namespaceID));
		}

		public Namespace Add(Parser.NamespaceDecContext namespaceDecContext)
		{
			return Add(Namespace.SplitNamespaceParts(namespaceDecContext));
		}

		public Namespace Add(Namespace @namespace)
		{
			Namespace existingNamespace;
			if (namespaces.TryGetValue(@namespace.Name, out existingNamespace))
			{
				existingNamespace.MergeInto(@namespace);
				return existingNamespace;
			}
			else
			{
				namespaces.Add(@namespace.Name, @namespace);
				return @namespace;
			}
		}

		public Namespace Get(IEnumerable<string> namespaceParts)
		{
			Namespace topNamespace;
			if (!namespaces.TryGetValue(namespaceParts.ElementAt(0), out topNamespace))
				throw new MissingNamespaceException(Namespace.JoinNamespaceParts(namespaceParts));

			return topNamespace.GetNamespace(namespaceParts);
		}

		public Namespace Get(string namespaceID)
		{
			IEnumerable<string> split = Namespace.SplitNamespaceParts(namespaceID);
			return Get(split);
		}

		public Namespace Get(Parser.NamespaceContext namespaceContext)
		{
			return Get(Namespace.SplitNamespaceParts(namespaceContext));
		}

		public Namespace Get(Parser.NamespaceDecContext namespaceDecContext)
		{
			return Get(Namespace.SplitNamespaceParts(namespaceDecContext));
        }

        public Namespace Get(NamespaceString namespaceString)
        {
            return Get(namespaceString.Name);
        }

        public bool TryGet(IEnumerable<string> namespaceParts, out Namespace namespaced)
		{
			Namespace topNamespace;
			if (!namespaces.TryGetValue(namespaceParts.ElementAt(0), out topNamespace))
				throw new MissingNamespaceException(Namespace.JoinNamespaceParts(namespaceParts));

			return topNamespace.TryGetNamespace(namespaceParts, out namespaced);
		}

		public bool TryGet(string namespaceID, out Namespace namespaced)
		{
			return TryGet(Namespace.SplitNamespaceParts(namespaceID), out namespaced);
		}

		public bool TryGet(Parser.NamespaceContext namespaceContext, out Namespace namespaced)
		{
			return TryGet(Namespace.SplitNamespaceParts(namespaceContext), out namespaced);
        }

        public bool TryGet(NamespaceString namespaceString, out Namespace namespaced)
        {
            return TryGet(namespaceString.Parts, out namespaced);
        }

        public bool TryGet(Parser.NamespaceDecContext namespaceDecContext, out Namespace namespaced)
		{
			return TryGet(Namespace.SplitNamespaceParts(namespaceDecContext), out namespaced);
		}

		public bool ContainsNamespace(IEnumerable<string> namespaceParts)
		{
			return namespaces.ContainsKey(namespaceParts.ElementAt(0));
		}

		public bool ContainsNamespace(string namespaceID)
		{
			return ContainsNamespace(Namespace.SplitNamespaceParts(namespaceID));
		}

		public bool ContainsNamespace(Parser.NamespaceContext namespaceContext)
		{
			return ContainsNamespace(Namespace.SplitNamespaceParts(namespaceContext));
		}

		public bool ContainsNamespace(Parser.NamespaceDecContext namespaceDecContext)
		{
			return ContainsNamespace(Namespace.SplitNamespaceParts(namespaceDecContext));
        }

        public bool ContainsNamespace(NamespaceString namespaceString)
        {
            return ContainsNamespace(namespaceString.Parts);
        }

        public IEnumerator<KeyValuePair<string, Namespace>> GetEnumerator()
        {
            return namespaces.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return namespaces.GetEnumerator();
        }
    }
}
