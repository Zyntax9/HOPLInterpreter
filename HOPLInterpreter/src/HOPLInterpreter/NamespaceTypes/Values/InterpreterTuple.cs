using HomeControlInterpreterInterface;

namespace HOPLInterpreter.NamespaceTypes.Values
{
	public class InterpreterTuple : InterpreterValue<InterpreterValue[]>
	{
		public InterpreterTuple(object[] value) : base(FromNativeArray(value)) { }

		public InterpreterTuple(InterpreterValue[] value) : base(value) { }

		public InterpreterTuple(ISuppliedTuple tuple) : base(FromNativeArray(tuple.GetObjectArray())) { }

		public override int Count { get { return value.Length; } }

		public override bool TypeEqual(InterpreterValue other)
		{
			if (GetType() != other.GetType())
				return false;
			InterpreterTuple otherArray = (InterpreterTuple)other;
			InterpreterValue[] oarray = (InterpreterValue[])otherArray.Value;

			if (value.Length != oarray.Length)
				return false;

			for (int i = 0; i <= value.Length; i++)
			{
				if (!value[i].TypeEqual(oarray[i]))
					return false;
			}

			return true;
		}

		protected override InterpreterValue Equal(InterpreterValue other)
		{
			if (other.GetType() != typeof(InterpreterTuple))
				return new InterpreterBool(false);

			InterpreterTuple otherArray = (InterpreterTuple)other;
			InterpreterValue[] oarray = (InterpreterValue[])otherArray.Value;

			if (oarray.Length != value.Length)
				return new InterpreterBool(false);

			for (int i = 0; i < value.Length; i++)
			{
				InterpreterValue equal = value[i] != oarray[i];
				if ((bool)equal.Value)
					return new InterpreterBool(false);
			}

			return new InterpreterBool(true);
		}

		public override object ToNative()
		{
			object[] nativeArray = new object[value.Length];
			for (int i = 0; i < value.Length; i++)
				nativeArray[i] = value[i].ToNative();
			return nativeArray;
		}

		public override InterpreterValue Clone()
		{
			InterpreterValue[] vals = new InterpreterValue[value.Length];
			for (int i = 0; i < value.Length; i++)
				vals[i] = value[i].Clone();
			return new InterpreterTuple(vals);
		}

		public static InterpreterValue[] FromNativeArray(object[] value)
		{
			InterpreterValue[] vals = new InterpreterValue[value.Length];
			for (int i = 0; i < value.Length; i++)
				vals[i] = FromNative(value[i]);
			return vals;
		}

		public override InterpreterValue this[int key]
		{
			get
			{
				return FromNative(value[key]);
			}
			set
			{
				this.value[key] = value.Clone();
			}
		}
	}
}
