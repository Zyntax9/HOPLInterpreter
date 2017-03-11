using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using HOPLInterpreter.Exceptions;
using Api = HomeControlInterpreterInterface;

namespace HOPLInterpreter.NamespaceTypes
{

	public class InterpreterType
	{
		public enum Types { NONE, INT, FLOAT, STRING, BOOL, LIST, TUPLE, TRIGGER, FUNCTION, IGNORE, FAULT };

		public static readonly Types[] callableTypes = { Types.FUNCTION, Types.TRIGGER };

		public static InterpreterType NONE { get; } = new InterpreterType(Types.NONE);
		public static InterpreterType IGNORE { get; } = new InterpreterType(Types.IGNORE);
		public static InterpreterType FAULT { get; } = new InterpreterType(Types.FAULT);

		public static InterpreterType INT { get; } = new InterpreterType(Types.INT);
		public static InterpreterType FLOAT { get; } = new InterpreterType(Types.FLOAT);
		public static InterpreterType STRING { get; } = new InterpreterType(Types.STRING);
		public static InterpreterType BOOL { get; } = new InterpreterType(Types.BOOL);

		public static InterpreterType INT_LIST { get; } = new InterpreterType(Types.LIST, INT);
		public static InterpreterType FLOAT_LIST { get; } = new InterpreterType(Types.LIST, FLOAT);
		public static InterpreterType STRING_LIST { get; } = new InterpreterType(Types.LIST, STRING);
		public static InterpreterType BOOL_LIST { get; } = new InterpreterType(Types.LIST, BOOL);

		public static InterpreterType EMPTY_LIST { get; } = new InterpreterType(Types.LIST);

		public InterpreterType[] TypeArray { get; protected set; }
		public Types TypeOf { get; protected set; }

		public bool IsCallable { get { return callableTypes.Contains(TypeOf); } }
		public bool IsEmptyList { get { return TypeOf == Types.LIST && TypeArray.Length == 0; } }
		public bool IsTriggerable
		{
			get
			{
				return TypeOf == Types.TRIGGER ||
				TypeOf == Types.LIST && TypeArray[0].IsTriggerable;
			}
		}

		public InterpreterType(Types type)
		{
			TypeArray = new InterpreterType[] { };
			TypeOf = type;
		}

		public InterpreterType(Types type, InterpreterType t)
		{
			TypeArray = new InterpreterType[] { t };
			TypeOf = type;
		}

		public InterpreterType(Types t, InterpreterType[] ta)
		{
			TypeArray = ta;
			TypeOf = t;
		}

		// NOTE: Function is special case. First element in array is range, rest is parameters
		public InterpreterType(FunctionSignature signature) : this(signature.Range, signature.Domain) { }
		public InterpreterType(InterpreterType ret, InterpreterType[] ta)
		{
			TypeArray = new InterpreterType[] { ret };
			TypeArray = TypeArray.Concat(ta).ToArray();
			TypeOf = Types.FUNCTION;
		}

		public InterpreterType[] GetArgumentTypes()
		{
			if (TypeOf == Types.FUNCTION || TypeOf == Types.TRIGGER)
				return TypeArray;
			else if (TypeOf == Types.LIST)
				return TypeArray[0].GetArgumentTypes();
			return new InterpreterType[0];
		}

		public InterpreterType GetCallableRange()
		{
			if (TypeOf != Types.FUNCTION)
				return NONE;
			return TypeArray[0];
		}

		public InterpreterType[] GetCallableDomain()
		{
			if (!IsCallable)
				return new InterpreterType[0];
			if (TypeOf == Types.TRIGGER)
				return TypeArray;
			return TypeArray.Skip(1).ToArray();
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;

			if (obj.GetType() != typeof(InterpreterType))
				return false;

			InterpreterType t = (InterpreterType)obj;

			if (this.TypeOf == Types.IGNORE)
				return t.TypeOf != Types.FAULT;
			if (t.TypeOf == Types.IGNORE)
				return this.TypeOf != Types.FAULT;

			if (t.IsEmptyList)
				return this.TypeOf == Types.LIST;
			if (this.IsEmptyList)
				return t.TypeOf == Types.LIST;

			return t.TypeOf == this.TypeOf && Enumerable.SequenceEqual(t.TypeArray, this.TypeArray);
		}

		public static bool operator ==(InterpreterType a, InterpreterType b)
		{
			a = a ?? NONE;
			b = b ?? NONE;

			return a.Equals(b);
		}

		public static bool operator !=(InterpreterType a, InterpreterType b)
		{
			a = a ?? NONE;
			b = b ?? NONE;

			return !a.Equals(b);
		}

		public static InterpreterType FromNative(Type t)
		{
			if (t == typeof(int))
			{
				return INT;
			}
			if (t == typeof(float))
			{
				return FLOAT;
			}
			if (t == typeof(string))
			{
				return STRING;
			}
			if (t == typeof(bool))
			{
				return BOOL;
			}

			TypeInfo ti = t.GetTypeInfo();
			if (typeof(IList).IsAssignableFrom(t))
			{
				Type[] args = ti.GetGenericArguments();
				if (args.Count() > 1)
					throw new InvalidNativeTypeException(t);
				return new InterpreterType(Types.LIST, new InterpreterType[] { FromNative(args[0]) });
			}
			if (typeof(Api.ISuppliedTuple).IsAssignableFrom(t))
			{
				Type[] args = ti.GetGenericArguments();
				InterpreterType[] its = new InterpreterType[args.Length];
				for (int i = 0; i < args.Length; i++)
					its[i] = FromNative(args[i]);
				return new InterpreterType(Types.TUPLE, its);
			}
			if (typeof(Api.SuppliedTrigger).IsAssignableFrom(t))
			{
				Type[] args = ti.GetGenericArguments();
				InterpreterType[] its = new InterpreterType[args.Length];
				for (int i = 0; i < args.Length; i++)
					its[i] = FromNative(args[i]);
				return new InterpreterType(Types.TRIGGER, its);
			}
			if (typeof(Delegate).IsAssignableFrom(t))
			{
				MethodInfo mi = t.GetMethod("Invoke");
				InterpreterType ret = FromNative(mi.ReturnType);
				ParameterInfo[] args = mi.GetParameters();
				InterpreterType[] its = new InterpreterType[args.Length];
				for (int i = 0; i < args.Length; i++)
					its[i] = FromNative(args[i].ParameterType);
				return new InterpreterType(ret, its);
			}

			throw new InvalidNativeTypeException(t);
		}
	}
}
