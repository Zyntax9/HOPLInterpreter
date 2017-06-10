using HOPL.Interpreter.Exceptions;
using System;
using System.Collections;
using System.Reflection;
using Parser = HOPL.Grammar.HOPLGrammarParser;

namespace HOPL.Interpreter.NamespaceTypes.Values
{
    public abstract class InterpreterValue
	{
		public virtual object Value { get; }

		public virtual int Count { get { throw new InvalidOperationException(); } }

		public virtual InterpreterValue Clone()
		{
			return this; // There is generally no reason to clone primitives
		}

		public virtual InterpreterValue this[int key]
		{
			get
			{
				throw new InvalidOperationException();
			}
			set
			{
				throw new InvalidOperationException();
			}
		}

		public virtual bool TypeEqual(InterpreterValue other)
		{
			return GetType() == other.GetType();
		}

        public virtual bool IsAssignableFrom(InterpreterValue assignTo)
        {
            return TypeEqual(assignTo);
        }

		public virtual object ToNative(InterpreterType expected = null)
		{
			return Value;
		}

		protected virtual InterpreterValue Add(InterpreterValue other)
		{
			throw new InvalidOperationException();
		}

		protected virtual InterpreterValue Subtract(InterpreterValue other)
		{
			throw new InvalidOperationException();
		}

		protected virtual InterpreterValue Negate()
		{
			throw new InvalidOperationException();
		}

		protected virtual InterpreterValue Multiply(InterpreterValue other)
		{
			throw new InvalidOperationException();
		}

		protected virtual InterpreterValue Divide(InterpreterValue other)
		{
			throw new InvalidOperationException();
		}

		protected virtual InterpreterValue Equal(InterpreterValue other)
		{
			return new InterpreterBool(Value == other.Value);
		}

		protected virtual InterpreterValue NEqual(InterpreterValue other)
		{
			throw new InvalidOperationException();
		}

		protected virtual InterpreterValue GEqual(InterpreterValue other)
		{
			throw new InvalidOperationException();
		}

		protected virtual InterpreterValue LEqual(InterpreterValue other)
		{
			throw new InvalidOperationException();
		}

		protected virtual InterpreterValue Greater(InterpreterValue other)
		{
			throw new InvalidOperationException();
		}

		protected virtual InterpreterValue Less(InterpreterValue other)
		{
			throw new InvalidOperationException();
		}

		protected virtual InterpreterValue Not()
		{
			throw new InvalidOperationException();
		}

		protected virtual InterpreterValue And(InterpreterValue other)
		{
			throw new InvalidOperationException();
		}

		protected virtual InterpreterValue Or(InterpreterValue other)
		{
			throw new InvalidOperationException();
		}

		public static InterpreterValue operator +(InterpreterValue i1, InterpreterValue i2)
		{
			return i1.Add(i2);
		}

		public static InterpreterValue operator -(InterpreterValue i1, InterpreterValue i2)
		{
			return i1.Subtract(i2);
		}

		public static InterpreterValue operator -(InterpreterValue i1)
		{
			return i1.Negate();
		}

		public static InterpreterValue operator *(InterpreterValue i1, InterpreterValue i2)
		{
			return i1.Multiply(i2);
		}

		public static InterpreterValue operator /(InterpreterValue i1, InterpreterValue i2)
		{
			return i1.Divide(i2);
		}

		public static InterpreterValue operator ==(InterpreterValue i1, InterpreterValue i2)
		{
			if (ReferenceEquals(i1, null) || ReferenceEquals(i2, null))
				return new InterpreterBool(false);
			return i1.Equal(i2);
		}

		public static InterpreterValue operator !=(InterpreterValue i1, InterpreterValue i2)
		{
			return i1.NEqual(i2);
		}

		public static InterpreterValue operator >=(InterpreterValue i1, InterpreterValue i2)
		{
			return i1.GEqual(i2);
		}

		public static InterpreterValue operator <=(InterpreterValue i1, InterpreterValue i2)
		{
			return i1.LEqual(i2);
		}

		public static InterpreterValue operator >(InterpreterValue i1, InterpreterValue i2)
		{
			return i1.Greater(i2);
		}

		public static InterpreterValue operator <(InterpreterValue i1, InterpreterValue i2)
		{
			return i1.Less(i2);
		}

		public static InterpreterValue operator !(InterpreterValue i1)
		{
			return i1.Not();
		}

		public static InterpreterValue FromNative(object obj)
		{
            Type t = obj.GetType();

			if (typeof(InterpreterValue).GetTypeInfo().IsAssignableFrom(t))
			{
				return ((InterpreterValue)obj).Clone();
			}

			if (obj is int)
			{
				return new InterpreterInt((int)obj);
			}
			if (obj is float)
			{
				return new InterpreterFloat((float)obj);
			}
			if (obj is string)
			{
				return new InterpreterString((string)obj);
			}
			if (obj is bool)
			{
				return new InterpreterBool((bool)obj);
			}
			if (typeof(IList).GetTypeInfo().IsAssignableFrom(t))
			{
				return new InterpreterList((IList)obj);
			}
			if (typeof(Api.ISuppliedTuple).GetTypeInfo().IsAssignableFrom(t))
			{
				return new InterpreterTuple((Api.ISuppliedTuple)obj);
			}
			if (typeof(Api.SuppliedTrigger).GetTypeInfo().IsAssignableFrom(t))
			{
				return new InterpreterTrigger((Api.SuppliedTrigger)obj);
			}
			if (typeof(Delegate).GetTypeInfo().IsAssignableFrom(t))
			{
				return new InterpreterFunction((Delegate)obj);
			}

			throw new InvalidNativeTypeException(t);
		}

		public static InterpreterValue GetDefault(Parser.TypeNameContext context)
		{
			if (context is Parser.IntTypeContext)
				return new InterpreterInt();
			if (context is Parser.FloatTypeContext)
				return new InterpreterFloat();
			if (context is Parser.BoolTypeContext)
				return new InterpreterBool();
			if (context is Parser.StringTypeContext)
				return new InterpreterString();
			if (context is Parser.ListTypeContext)
				return new InterpreterList();
			if (context is Parser.TupleTypeContext)
				return GetDefault((Parser.TupleTypeContext)context);
			if (context is Parser.TriggerTypeContext)
				return new InterpreterTrigger();
			if (context is Parser.FunctionTypeContext)
				return new InterpreterFunction();
			throw new InvalidOperationException();
		}

		public static InterpreterValue GetDefault(Parser.TupleTypeContext context)
		{
			Parser.TypeNameContext[] types = context.typeName();
			InterpreterValue[] values = new InterpreterValue[types.Length];

			for (int i = 0; i < types.Length; i++)
				values[i] = GetDefault(types[i]);

			return new InterpreterTuple(values);
		}
	}

	public abstract class InterpreterValue<T> : InterpreterValue
	{
		protected T value;
		public override object Value { get { return value; } }

		protected InterpreterValue(T value)
		{
			this.value = value;
		}
	}
}
