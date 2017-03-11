namespace HOPLInterpreter.Faults.TypeCheck
{
	public class TypeFaultMessage
	{
		public readonly int id;
		public readonly string message;

		// General messages
		public static readonly TypeFaultMessage NUMERICAL_MISMATCH = new TypeFaultMessage(1, "Both sides must be numerical.");
		public static readonly TypeFaultMessage BOOLEAN_MISMATCH = new TypeFaultMessage(2, "Both sides must be boolean.");
		public static readonly TypeFaultMessage NONBOOLEAN_MISMATCH = new TypeFaultMessage(3, "Both sides must be non-boolean.");
		public static readonly TypeFaultMessage ARGCOUNT_MISMATCH = new TypeFaultMessage(4, "Incorrect number of arguments given.");
		public static readonly TypeFaultMessage VAR_NOTDEF = new TypeFaultMessage(5, "Variable used but not defined.");
		public static readonly TypeFaultMessage STAT_BOOLEAN = new TypeFaultMessage(6, "Expression in statement must evaluate to boolean.");
		public static readonly TypeFaultMessage NS_MISSING = new TypeFaultMessage(14, "Namespace was not found.");

		// Context-specific messages
		public static readonly TypeFaultMessage ASSIGN_MISMATCH = new TypeFaultMessage(7, "Variable type does not match the assigned value type.");
		public static readonly TypeFaultMessage CALL_ARGMISMATCH = new TypeFaultMessage(8, "Argument type does not match the correct type.");
		public static readonly TypeFaultMessage CALLABLE_NOTFOUND = new TypeFaultMessage(9, "Callable does not exists.");
		public static readonly TypeFaultMessage CALLABLE_NOTCALLABLE = new TypeFaultMessage(10, "Token is not a callable entity.");
		public static readonly TypeFaultMessage COMPEXPR_MISMATCH = new TypeFaultMessage(11, "Both sides must either be of same type or be a combination of int and float.");
		public static readonly TypeFaultMessage HANDLERDEC_NOTTRIGGER = new TypeFaultMessage(12, "Token is not a trigger.");
		public static readonly TypeFaultMessage HANDLERDEC_ARGMISMATCH = new TypeFaultMessage(13, "Handler variables must have the same type as those defined by the trigger.");
		public static readonly TypeFaultMessage VAREXPR_GLOBALMISSING = new TypeFaultMessage(15, "Global entity used without having been defined.");
		public static readonly TypeFaultMessage NEG_NUMMISMATCH = new TypeFaultMessage(16, "Negated expression must be numerical.");
		public static readonly TypeFaultMessage NOT_BOOLMISMATCH = new TypeFaultMessage(17, "\"not\"-statements can only be applied to boolean values.");
		public static readonly TypeFaultMessage RETURN_TRIGGERVAL = new TypeFaultMessage(18, "Return statement in a trigger must not contain return value.");
		public static readonly TypeFaultMessage RETURN_MISMATCH = new TypeFaultMessage(19, "Return value does not match the return type.");
		public static readonly TypeFaultMessage VARDEC_REDEF = new TypeFaultMessage(20, "Variable has already been defined.");
		public static readonly TypeFaultMessage VARDEC_MISMATCH = new TypeFaultMessage(21, "Assigned value does not match the type of the variable.");
		public static readonly TypeFaultMessage AWAIT_NOTTRIGGER = new TypeFaultMessage(22, "Token is not trigger.");
		public static readonly TypeFaultMessage LISTEXPR_ALL = new TypeFaultMessage(23, "All values in a list must be of the same type.");
		public static readonly TypeFaultMessage FOREACH_LIST = new TypeFaultMessage(24, "Expression must evaluate to list.");
		public static readonly TypeFaultMessage FOREACH_ITERMISMATCH = new TypeFaultMessage(25, "Iterating value must have the same type as contained in list.");
		public static readonly TypeFaultMessage INDEX_LORT = new TypeFaultMessage(26, "Indexing must be applied to either a list or a tuple.");
		public static readonly TypeFaultMessage INDEX_EMPTY = new TypeFaultMessage(27, "Cannot index into empty lists.");
		public static readonly TypeFaultMessage INDEX_LINT = new TypeFaultMessage(28, "Index must be of integer value.");
		public static readonly TypeFaultMessage INDEX_TCINT = new TypeFaultMessage(29, "Index of tuple must be of constant integer value.");
		public static readonly TypeFaultMessage INDEX_TOOR = new TypeFaultMessage(30, "Index out of tuple range.");
		public static readonly TypeFaultMessage ARG_SHADOW = new TypeFaultMessage(31, "Argument cannot have the same name as a global entity in the same namespace.");
		public static readonly TypeFaultMessage ADDI_MISMATCH = new TypeFaultMessage(32, "Both sides must be either both be numerical or both be string literals.");
		public static readonly TypeFaultMessage CONCAT_RIGHTM = new TypeFaultMessage(33, "Right side of append must be the same type as the inner type of the left-side list.");
		public static readonly TypeFaultMessage CONCAT_LEFTM = new TypeFaultMessage(34, "Left side of append must be the same type as the inner type of the right-side list.");
		public static readonly TypeFaultMessage CONCAT_LIST = new TypeFaultMessage(35, "Both lists in concatination must be same type.");
		public static readonly TypeFaultMessage CONCAT_MISMATCH = new TypeFaultMessage(36, "At least one of the sides must evaluate to list.");
		public static readonly TypeFaultMessage UNPACK_NOTTUPLE = new TypeFaultMessage(37, "Unpacked expression must be a tuple.");
		public static readonly TypeFaultMessage UNPACK_TOOFEW = new TypeFaultMessage(38, "Too few values to unpack.");
		public static readonly TypeFaultMessage UNPACK_TOOMANY = new TypeFaultMessage(39, "Too many values to unpack.");
		public static readonly TypeFaultMessage ASSIGN_CONST = new TypeFaultMessage(40, "Cannot assign new value to a constant variable or declared function.");

		private TypeFaultMessage(int id, string message)
		{
			this.id = id;
			this.message = message;
		}

		public override string ToString()
		{
			return message;
		}
	}
}
