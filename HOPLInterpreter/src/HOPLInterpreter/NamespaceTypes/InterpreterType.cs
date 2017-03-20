using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections;
using HOPLInterpreter.Exceptions;
using Api = HOPLInterpreterInterface;

namespace HOPLInterpreter.NamespaceTypes
{

	public class InterpreterType
	{
		public enum Types { NONE, INT, FLOAT, STRING, BOOL, LIST, TUPLE, TRIGGER, FUNCTION, IGNORE, ERROR };

		public static readonly Types[] callableTypes = { Types.FUNCTION, Types.TRIGGER };

		public static InterpreterType NONE { get; } = new InterpreterType(Types.NONE);
		public static InterpreterType IGNORE { get; } = new InterpreterType(Types.IGNORE);
		public static InterpreterType ERROR { get; } = new InterpreterType(Types.ERROR);

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

		public InterpreterType[] GetTriggerableDomain()
		{
			if (!IsTriggerable)
				return new InterpreterType[0];
			if (TypeOf == Types.TRIGGER)
				return TypeArray;
			return TypeArray[0].GetTriggerableDomain();
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
				return t.TypeOf != Types.ERROR;
			if (t.TypeOf == Types.IGNORE)
				return this.TypeOf != Types.ERROR;

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

		public Type ToNative()
		{
			switch(TypeOf)
			{
				case Types.BOOL:
					return typeof(bool);
				case Types.FLOAT:
					return typeof(float);
				case Types.INT:
					return typeof(int);
				case Types.STRING:
					return typeof(string);
				case Types.LIST:
					Type listGenericType = typeof(List<>);
					Type generic = TypeArray[0].ToNative();
					return listGenericType.MakeGenericType(generic);
				case Types.TRIGGER:
					switch (TypeArray.Length)
					{
						case 1:
							Type tuple1GenericType = typeof(Api.SuppliedTuple<>);
							Type generic11 = TypeArray[0].ToNative();
							return tuple1GenericType.MakeGenericType(generic11);
						case 2:
							Type tuple2GenericType = typeof(Api.SuppliedTuple<,>);
							Type generic12 = TypeArray[0].ToNative();
							Type generic22 = TypeArray[1].ToNative();
							return tuple2GenericType.MakeGenericType(generic12, generic22);
						case 3:
							Type tuple3GenericType = typeof(Api.SuppliedTuple<,,>);
							Type generic13 = TypeArray[0].ToNative();
							Type generic23 = TypeArray[1].ToNative();
							Type generic33 = TypeArray[2].ToNative();
							return tuple3GenericType.MakeGenericType(generic13, generic23, generic33);
						case 4:
							Type tuple4GenericType = typeof(Api.SuppliedTuple<,,,>);
							Type generic14 = TypeArray[0].ToNative();
							Type generic24 = TypeArray[1].ToNative();
							Type generic34 = TypeArray[2].ToNative();
							Type generic44 = TypeArray[3].ToNative();
							return tuple4GenericType.MakeGenericType(generic14, generic24, generic34, 
								generic44);
						case 5:
							Type tuple5GenericType = typeof(Api.SuppliedTuple<,,,,>);
							Type generic15 = TypeArray[0].ToNative();
							Type generic25 = TypeArray[1].ToNative();
							Type generic35 = TypeArray[2].ToNative();
							Type generic45 = TypeArray[3].ToNative();
							Type generic55 = TypeArray[4].ToNative();
							return tuple5GenericType.MakeGenericType(generic15, generic25, generic35,
								generic45, generic55);
						case 6:
							Type tuple6GenericType = typeof(Api.SuppliedTuple<,,,,,>);
							Type generic16 = TypeArray[0].ToNative();
							Type generic26 = TypeArray[1].ToNative();
							Type generic36 = TypeArray[2].ToNative();
							Type generic46 = TypeArray[3].ToNative();
							Type generic56 = TypeArray[4].ToNative();
							Type generic66 = TypeArray[5].ToNative();
							return tuple6GenericType.MakeGenericType(generic16, generic26, generic36,
								generic46, generic56, generic66);
						case 7:
							Type tuple7GenericType = typeof(Api.SuppliedTuple<,,,,,,>);
							Type generic17 = TypeArray[0].ToNative();
							Type generic27 = TypeArray[1].ToNative();
							Type generic37 = TypeArray[2].ToNative();
							Type generic47 = TypeArray[3].ToNative();
							Type generic57 = TypeArray[4].ToNative();
							Type generic67 = TypeArray[5].ToNative();
							Type generic77 = TypeArray[6].ToNative();
							return tuple7GenericType.MakeGenericType(generic17, generic27, generic37,
								generic47, generic57, generic67, generic77);
						case 8:
							Type tuple8GenericType = typeof(Api.SuppliedTuple<,,,,,,,>);
							Type generic18 = TypeArray[0].ToNative();
							Type generic28 = TypeArray[1].ToNative();
							Type generic38 = TypeArray[2].ToNative();
							Type generic48 = TypeArray[3].ToNative();
							Type generic58 = TypeArray[4].ToNative();
							Type generic68 = TypeArray[5].ToNative();
							Type generic78 = TypeArray[6].ToNative();
							Type generic88 = TypeArray[7].ToNative();
							return tuple8GenericType.MakeGenericType(generic18, generic28, generic38,
								generic48, generic58, generic68, generic78, generic88);
						case 9:
							Type tuple9GenericType = typeof(Api.SuppliedTuple<,,,,,,,,>);
							Type generic19 = TypeArray[0].ToNative();
							Type generic29 = TypeArray[1].ToNative();
							Type generic39 = TypeArray[2].ToNative();
							Type generic49 = TypeArray[3].ToNative();
							Type generic59 = TypeArray[4].ToNative();
							Type generic69 = TypeArray[5].ToNative();
							Type generic79 = TypeArray[6].ToNative();
							Type generic89 = TypeArray[7].ToNative();
							Type generic99 = TypeArray[8].ToNative();
							return tuple9GenericType.MakeGenericType(generic19, generic29, generic39,
								generic49, generic59, generic69, generic79, generic89, generic99);
						case 10:
							Type tuple10GenericType = typeof(Api.SuppliedTuple<,,,,,,,,,>);
							Type generic110 = TypeArray[0].ToNative();
							Type generic210 = TypeArray[1].ToNative();
							Type generic310 = TypeArray[2].ToNative();
							Type generic410 = TypeArray[3].ToNative();
							Type generic510 = TypeArray[4].ToNative();
							Type generic610 = TypeArray[5].ToNative();
							Type generic710 = TypeArray[6].ToNative();
							Type generic810 = TypeArray[7].ToNative();
							Type generic910 = TypeArray[8].ToNative();
							Type generic1010 = TypeArray[9].ToNative();
							return tuple10GenericType.MakeGenericType(generic110, generic210, generic310,
								generic410, generic510, generic610, generic710, generic810, generic910,
								generic1010);
						default:
							return null;
					}
				default:
					return null;
			}
		}
	}
}
