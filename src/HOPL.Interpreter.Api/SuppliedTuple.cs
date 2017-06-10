namespace HOPL.Interpreter.Api
{
    public interface ISuppliedTuple
	{
		int Length { get; }
		object[] GetObjectArray();
	}

	public class SuppliedTuple<T0> : ISuppliedTuple
	{
		public T0 Value0 { get; set; }

		public int Length { get { return 1; } }

		public SuppliedTuple() { }
		public SuppliedTuple(T0 val0) { Value0 = val0; }

		public object[] GetObjectArray()
		{
			return new object[] {
				Value0
			};
		}
	}

	public class SuppliedTuple<T0, T1> : ISuppliedTuple
	{
		public T0 Value0 { get; set; }
		public T1 Value1 { get; set; }

		public int Length { get { return 2; } }

		public SuppliedTuple() { }
		public SuppliedTuple(T0 val0, T1 val1)
		{
			Value0 = val0;
			Value1 = val1;
		}

		public object[] GetObjectArray()
		{
			return new object[] {
				Value0,
				Value1
			};
		}
	}

	public class SuppliedTuple<T0, T1, T2> : ISuppliedTuple
	{
		public T0 Value0 { get; set; }
		public T1 Value1 { get; set; }
		public T2 Value2 { get; set; }

		public int Length { get { return 3; } }

		public SuppliedTuple() { }
		public SuppliedTuple(T0 val0, T1 val1, T2 val2)
		{
			Value0 = val0;
			Value1 = val1;
			Value2 = val2;
		}

		public object[] GetObjectArray()
		{
			return new object[] {
				Value0,
				Value1,
				Value2
			};
		}
	}

	public class SuppliedTuple<T0, T1, T2, T3> : ISuppliedTuple
	{
		public T0 Value0 { get; set; }
		public T1 Value1 { get; set; }
		public T2 Value2 { get; set; }
		public T3 Value3 { get; set; }

		public int Length { get { return 4; } }

		public SuppliedTuple() { }
		public SuppliedTuple(T0 val0, T1 val1, T2 val2, T3 val3)
		{
			Value0 = val0;
			Value1 = val1;
			Value2 = val2;
			Value3 = val3;
		}

		public object[] GetObjectArray()
		{
			return new object[] {
				Value0,
				Value1,
				Value2,
				Value3
			};
		}
	}

	public class SuppliedTuple<T0, T1, T2, T3, T4> : ISuppliedTuple
	{
		public T0 Value0 { get; set; }
		public T1 Value1 { get; set; }
		public T2 Value2 { get; set; }
		public T3 Value3 { get; set; }
		public T4 Value4 { get; set; }

		public int Length { get { return 5; } }

		public SuppliedTuple() { }
		public SuppliedTuple(T0 val0, T1 val1, T2 val2, T3 val3, T4 val4)
		{
			Value0 = val0;
			Value1 = val1;
			Value2 = val2;
			Value3 = val3;
			Value4 = val4;
		}

		public object[] GetObjectArray()
		{
			return new object[] {
				Value0,
				Value1,
				Value2,
				Value3,
				Value4
			};
		}
	}

	public class SuppliedTuple<T0, T1, T2, T3, T4, T5> : ISuppliedTuple
	{
		public T0 Value0 { get; set; }
		public T1 Value1 { get; set; }
		public T2 Value2 { get; set; }
		public T3 Value3 { get; set; }
		public T4 Value4 { get; set; }
		public T5 Value5 { get; set; }

		public int Length { get { return 6; } }

		public SuppliedTuple() { }
		public SuppliedTuple(T0 val0, T1 val1, T2 val2, T3 val3, T4 val4, T5 val5)
		{
			Value0 = val0;
			Value1 = val1;
			Value2 = val2;
			Value3 = val3;
			Value4 = val4;
			Value5 = val5;
		}

		public object[] GetObjectArray()
		{
			return new object[] {
				Value0,
				Value1,
				Value2,
				Value3,
				Value4,
				Value5
			};
		}
	}

	public class SuppliedTuple<T0, T1, T2, T3, T4, T5, T6> : ISuppliedTuple
	{
		public T0 Value0 { get; set; }
		public T1 Value1 { get; set; }
		public T2 Value2 { get; set; }
		public T3 Value3 { get; set; }
		public T4 Value4 { get; set; }
		public T5 Value5 { get; set; }
		public T6 Value6 { get; set; }

		public int Length { get { return 7; } }

		public SuppliedTuple() { }
		public SuppliedTuple(T0 val0, T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6)
		{
			Value0 = val0;
			Value1 = val1;
			Value2 = val2;
			Value3 = val3;
			Value4 = val4;
			Value5 = val5;
			Value6 = val6;
		}

		public object[] GetObjectArray()
		{
			return new object[] {
				Value0,
				Value1,
				Value2,
				Value3,
				Value4,
				Value5,
				Value6
			};
		}
	}

	public class SuppliedTuple<T0, T1, T2, T3, T4, T5, T6, T7> : ISuppliedTuple
	{
		public T0 Value0 { get; set; }
		public T1 Value1 { get; set; }
		public T2 Value2 { get; set; }
		public T3 Value3 { get; set; }
		public T4 Value4 { get; set; }
		public T5 Value5 { get; set; }
		public T6 Value6 { get; set; }
		public T7 Value7 { get; set; }

		public int Length { get { return 8; } }

		public SuppliedTuple() { }
		public SuppliedTuple(T0 val0, T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6, T7 val7)
		{
			Value0 = val0;
			Value1 = val1;
			Value2 = val2;
			Value3 = val3;
			Value4 = val4;
			Value5 = val5;
			Value6 = val6;
			Value7 = val7;
		}

		public object[] GetObjectArray()
		{
			return new object[] {
				Value0,
				Value1,
				Value2,
				Value3,
				Value4,
				Value5,
				Value6,
				Value7
			};
		}
	}

	public class SuppliedTuple<T0, T1, T2, T3, T4, T5, T6, T7, T8> : ISuppliedTuple
	{
		public T0 Value0 { get; set; }
		public T1 Value1 { get; set; }
		public T2 Value2 { get; set; }
		public T3 Value3 { get; set; }
		public T4 Value4 { get; set; }
		public T5 Value5 { get; set; }
		public T6 Value6 { get; set; }
		public T7 Value7 { get; set; }
		public T8 Value8 { get; set; }

		public int Length { get { return 9; } }

		public SuppliedTuple() { }
		public SuppliedTuple(T0 val0, T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6, T7 val7, T8 val8)
		{
			Value0 = val0;
			Value1 = val1;
			Value2 = val2;
			Value3 = val3;
			Value4 = val4;
			Value5 = val5;
			Value6 = val6;
			Value7 = val7;
			Value8 = val8;
		}

		public object[] GetObjectArray()
		{
			return new object[] {
				Value0,
				Value1,
				Value2,
				Value3,
				Value4,
				Value5,
				Value6,
				Value7,
				Value8
			};
		}
	}

	public class SuppliedTuple<T0, T1, T2, T3, T4, T5, T6, T7, T8, T9> : ISuppliedTuple
	{
		public T0 Value0 { get; set; }
		public T1 Value1 { get; set; }
		public T2 Value2 { get; set; }
		public T3 Value3 { get; set; }
		public T4 Value4 { get; set; }
		public T5 Value5 { get; set; }
		public T6 Value6 { get; set; }
		public T7 Value7 { get; set; }
		public T8 Value8 { get; set; }
		public T9 Value9 { get; set; }

		public int Length { get { return 10; } }

		public SuppliedTuple() { }
		public SuppliedTuple(T0 val0, T1 val1, T2 val2, T3 val3, T4 val4, T5 val5, T6 val6, T7 val7, T8 val8, T9 val9)
		{
			Value0 = val0;
			Value1 = val1;
			Value2 = val2;
			Value3 = val3;
			Value4 = val4;
			Value5 = val5;
			Value6 = val6;
			Value7 = val7;
			Value8 = val8;
			Value9 = val9;
		}

		public object[] GetObjectArray()
		{
			return new object[] {
				Value0,
				Value1,
				Value2,
				Value3,
				Value4,
				Value5,
				Value6,
				Value7,
				Value8,
				Value9
			};
		}
	}
}
