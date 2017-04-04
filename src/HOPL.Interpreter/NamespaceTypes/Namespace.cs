using Antlr4.Runtime.Tree;
using HOPL.Interpreter.Exceptions;
using HOPL.Interpreter.NamespaceTypes.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Parser = HOPL.Grammar.HOPLGrammarParser;

namespace HOPL.Interpreter.NamespaceTypes
{
	public class Namespace
	{
		private const char SEPERATOR = '.';

		public string Name { get; protected set; }
		public string CompleteName { get { return GetCompleteNamespaceName(); } }
		public Namespace Parent { get; protected set; }
		public bool ContainsRequired
		{
			get
			{
				return (from IGlobalEntity ge in globalEntities.Values
						where ge.Required
						select ge).Any();
			}
		}

		private IDictionary<string, Namespace> childNamespaces = new Dictionary<string, Namespace>();
		private IDictionary<string, IGlobalEntity> globalEntities = new Dictionary<string, IGlobalEntity>();
		private IDictionary<string, IFunction> functions = new Dictionary<string, IFunction>();

		public IEnumerable<Namespace> ChildNamespaces { get { return childNamespaces.Values.AsEnumerable(); } }
		public IEnumerable<IGlobalEntity> GlobalEntities { get { return globalEntities.Values.AsEnumerable(); } }
		public IEnumerable<IFunction> Functions { get { return functions.Values.AsEnumerable(); } }

		public Namespace(IEnumerable<string> namespaceParts, Namespace parent = null)
		{
			Name = namespaceParts.ElementAt(0);
			Parent = parent;

			IEnumerable<string> subNamespace = namespaceParts.Skip(1);
			AddChildNamespace(subNamespace);
		}

		public Namespace(string namespaceID, Namespace parent = null)
			: this(SplitNamespaceParts(namespaceID), parent) { }

		public Namespace(Parser.NamespaceDecContext namespaceDecContext, Namespace parent = null)
			: this(SplitNamespaceParts(namespaceDecContext), parent) { }

		public Namespace AddChildNamespace(IEnumerable<string> namespaceParts)
		{
			if (namespaceParts.Count() == 0)
				return this;

			int partCount = namespaceParts.Count();
			string topNamespace = namespaceParts.ElementAt(0);

			Namespace subNamespace;
			if (childNamespaces.TryGetValue(topNamespace, out subNamespace))
			{
				return subNamespace.AddChildNamespace(namespaceParts.Skip(1));
			}
			else
			{
				subNamespace = new Namespace(namespaceParts, this);
				childNamespaces.Add(topNamespace, subNamespace);
				return subNamespace.GetNamespace(namespaceParts);
			}
		}

		public Namespace AddChildNamespace(string namespaceID)
		{
			return AddChildNamespace(SplitNamespaceParts(namespaceID));
		}

		public Namespace AddChildNamespace(Parser.NamespaceDecContext namespaceDecContext)
		{
			return AddChildNamespace(SplitNamespaceParts(namespaceDecContext));
		}

		public bool TryGetNamespace(IEnumerable<string> namespaceParts, out Namespace childNamespace)
		{
			childNamespace = null;
			if (namespaceParts.ElementAt(0) != Name)
				return false;

			IEnumerable<string> subParts = namespaceParts.Skip(1);
			if (subParts.Count() == 0)
			{
				childNamespace = this;
				return true;
			}

			Namespace subNamespace;
			if (!childNamespaces.TryGetValue(subParts.ElementAt(0), out subNamespace))
				return false;

			return subNamespace.TryGetNamespace(subParts, out childNamespace);
		}

		public bool TryGetNamespace(string namespaceID, out Namespace childNamespace)
		{
			return TryGetNamespace(SplitNamespaceParts(namespaceID), out childNamespace);
		}

		public bool TryGetNamespace(Parser.NamespaceDecContext namespaceDecContext, out Namespace childNamespace)
		{
			return TryGetNamespace(SplitNamespaceParts(namespaceDecContext), out childNamespace);
		}

		public Namespace GetNamespace(IEnumerable<string> namespaceParts)
		{
			Namespace n;
			if (!TryGetNamespace(namespaceParts, out n))
				throw new MissingNamespaceException(JoinNamespaceParts(namespaceParts));
			return n;
		}

		public Namespace GetNamespace(string namespaceID)
		{
			return GetNamespace(SplitNamespaceParts(namespaceID));
		}

		public Namespace GetNamespace(Parser.NamespaceDecContext namespaceDecContext)
		{
			return GetNamespace(SplitNamespaceParts(namespaceDecContext));
		}

		public void AddGlobalEntity(Parser.GlobalDecContext globalDecContext)
		{
			GlobalEntity gv = new GlobalEntity(globalDecContext);
			AddGlobalEntity(gv);
		}

		public void AddGlobalEntity(IGlobalEntity globalEntity)
		{
			if (globalEntities.ContainsKey(globalEntity.Name))
				throw new DuplicateGlobalEntityException(globalEntity.Name);

			globalEntities.Add(globalEntity.Name, globalEntity);
		}

		public bool TryAddGlobalEntity(Parser.GlobalDecContext globalDecContext)
		{
			GlobalEntity gv = new GlobalEntity(globalDecContext);

			return TryAddGlobalEntity(gv);
		}

		public bool TryAddGlobalEntity(IGlobalEntity globalEntity)
		{
			if (globalEntities.ContainsKey(globalEntity.Name))
				return false;

			globalEntities.Add(globalEntity.Name, globalEntity);
			return true;
		}

		public IGlobalEntity GetGlobalEntity(string name)
		{
			IGlobalEntity gv;
			if (!globalEntities.TryGetValue(name, out gv))
				throw new MissingGlobalEntityException(name);

			return gv;
		}

		public bool TryGetGlobalEntity(string name, out IGlobalEntity globalEntity)
		{
			if (!globalEntities.TryGetValue(name, out globalEntity))
				return false;

			return true;
		}

		public bool TryGetGlobalEntityType(string name, out InterpreterType globalEntityType)
		{
			globalEntityType = null;
			IGlobalEntity globalEntity;
			if (!globalEntities.TryGetValue(name, out globalEntity))
				return false;

			globalEntityType = globalEntity.Type;
			return true;
		}

		public bool TryGetGlobalEntityValue(string name, out InterpreterValue globalEntityValue)
		{
			globalEntityValue = null;
			IGlobalEntity globalEntity;
			if (!globalEntities.TryGetValue(name, out globalEntity))
				return false;

			globalEntityValue = globalEntity.Value;
			return true;
		}

		public void AddFunction(IFunction function)
		{
			if (functions.ContainsKey(function.Name))
				throw new DuplicateFunctionException(function.Name);

			functions.Add(function.Name, function);
		}

		public bool TryAddFunction(Parser.FunctionDecContext functionDecContext, Namespace @namespace, string file)
		{
			Function f = new Function(functionDecContext, @namespace, file);
			GlobalEntity functionGlobalVar = new GlobalEntity(f);

			if (globalEntities.ContainsKey(functionGlobalVar.Name))
				return false;

			if (functions.ContainsKey(f.Name))
				return false;

			functions.Add(f.Name, f);
			return true;
		}

		public IFunction GetFunction(string name)
		{
			IFunction function;
			if (!TryGetFunction(name, out function))
				throw new MissingFunctionException(name);

			if (function.GetType() != typeof(Function))
				throw new MissingFunctionException(name);

			return function;
		}

		public bool TryGetFunction(string name, out IFunction trigger)
		{
			trigger = null;

			IFunction function;
			if (!functions.TryGetValue(name, out function))
				return false;

			trigger = function;
			return true;
		}

		public string GetCompleteNamespaceName()
		{
			if (Parent != null)
				return Parent.GetCompleteNamespaceName() + SEPERATOR + Name;
			return Name;
		}

		public void MergeInto(Namespace @namespace)
		{
			foreach (IFunction func in @namespace.Functions)
				AddFunction(func);

			foreach (IGlobalEntity gv in @namespace.GlobalEntities)
				AddGlobalEntity(gv);

			foreach (Namespace child in @namespace.ChildNamespaces)
			{
				Namespace mchild;
				if (TryGetNamespace(child.Name, out mchild))
				{
					mchild.MergeInto(child);
				}
				else
				{
					childNamespaces.Add(child.Name, child);
				}
			}
		}

		public static IEnumerable<string> SplitNamespaceParts(string namespaceID)
		{
			if (namespaceID == null)
				throw new ArgumentNullException("namespaceID");

			return namespaceID.Split('.');
		}

		public static IEnumerable<string> SplitNamespaceParts(Parser.NamespaceContext namespaceContext)
		{
			ITerminalNode[] ids = namespaceContext.ID();
			return from id in ids select id.GetText();
		}

		public static IEnumerable<string> SplitNamespaceParts(Parser.NamespaceDecContext namespaceDecContext)
		{
			return SplitNamespaceParts(namespaceDecContext.@namespace());
		}

		public static string JoinNamespaceParts(IEnumerable<string> namespaceParts)
		{
			return string.Join(SEPERATOR.ToString(), namespaceParts);
		}

		public static string JoinNamespaceParts(Parser.NamespaceDecContext namespaceDecContext)
		{
			return string.Join(SEPERATOR.ToString(), SplitNamespaceParts(namespaceDecContext));
		}
	}
}
