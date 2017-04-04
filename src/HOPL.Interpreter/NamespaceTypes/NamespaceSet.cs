using HOPL.Interpreter.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using Parser = HOPL.Grammar.HOPLGrammarParser;
using HOPL.Interpreter.Api.Attributes;
using HOPL.Interpreter.Api;

namespace HOPL.Interpreter.NamespaceTypes
{
	public class NamespaceSet
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
			{
				TypeInfo t = sn.GetType().GetTypeInfo();

				string nsname = sn.Name ?? t.FullName;

				Namespace ns = Add(nsname);

				foreach (PropertyInfo pi in t.GetProperties())
				{
					InterpreterGlobalVariableAttribute ge =
						pi.GetCustomAttribute<InterpreterGlobalVariableAttribute>();

					if (ge != null)
					{
						SuppliedGlobalEntity sge = new SuppliedGlobalEntity(pi, sn, ge);
						ns.AddGlobalEntity(sge);
					}
				}

				foreach (MethodInfo mi in t.GetMethods())
				{
					InterpreterFunctionAttribute fun = mi.GetCustomAttribute<InterpreterFunctionAttribute>();

					if (fun != null)
					{
						SuppliedFunction sfun = new SuppliedFunction(mi, sn, fun);
						SuppliedGlobalEntity sfge = new SuppliedGlobalEntity(sfun);
						ns.AddFunction(sfun);
						ns.AddGlobalEntity(sfge);
					}
				}
			}
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
	}
}
