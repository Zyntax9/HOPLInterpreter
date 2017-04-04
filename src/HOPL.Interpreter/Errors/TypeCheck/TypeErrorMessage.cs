namespace HOPL.Interpreter.Errors.TypeCheck
{
	public class TypeErrorMessage
	{
		public readonly int id;
		public readonly string message;

		// General messages
		public static readonly TypeErrorMessage NUMERICAL_MISMATCH = new TypeErrorMessage(1, "Both sides must be numerical.");
		public static readonly TypeErrorMessage BOOLEAN_MISMATCH = new TypeErrorMessage(2, "Both sides must be boolean.");
		public static readonly TypeErrorMessage NONBOOLEAN_MISMATCH = new TypeErrorMessage(3, "Both sides must be non-boolean.");
		public static readonly TypeErrorMessage ARGCOUNT_MISMATCH = new TypeErrorMessage(4, "Incorrect number of arguments given.");
		public static readonly TypeErrorMessage VAR_NOTDEF = new TypeErrorMessage(5, "Variable used but not defined.");
		public static readonly TypeErrorMessage STAT_BOOLEAN = new TypeErrorMessage(6, "Expression in statement must evaluate to boolean.");
		public static readonly TypeErrorMessage NS_MISSING = new TypeErrorMessage(14, "Namespace was not found.");

		// Context-specific messages
		public static readonly TypeErrorMessage ASSIGN_MISMATCH = new TypeErrorMessage(7, "Variable type does not match the assigned value type.");
		public static readonly TypeErrorMessage CALL_ARGMISMATCH = new TypeErrorMessage(8, "Argument type does not match the correct type.");
		public static readonly TypeErrorMessage CALLABLE_NOTFOUND = new TypeErrorMessage(9, "Callable does not exists.");
		public static readonly TypeErrorMessage CALLABLE_NOTCALLABLE = new TypeErrorMessage(10, "Token is not a callable entity.");
		public static readonly TypeErrorMessage COMPEXPR_MISMATCH = new TypeErrorMessage(11, "Both sides must either be of same type or be a combination of int and float.");
		public static readonly TypeErrorMessage HANDLERDEC_NOTTRIGGER = new TypeErrorMessage(12, "Token is not a trigger.");
		public static readonly TypeErrorMessage HANDLERDEC_ARGMISMATCH = new TypeErrorMessage(13, "Handler variables must have the same type as those defined by the trigger.");
		public static readonly TypeErrorMessage VAREXPR_GLOBALMISSING = new TypeErrorMessage(15, "Global entity used without having been defined.");
		public static readonly TypeErrorMessage NEG_NUMMISMATCH = new TypeErrorMessage(16, "Negated expression must be numerical.");
		public static readonly TypeErrorMessage NOT_BOOLMISMATCH = new TypeErrorMessage(17, "\"not\"-statements can only be applied to boolean values.");
		public static readonly TypeErrorMessage RETURN_TRIGGERVAL = new TypeErrorMessage(18, "Return statement in a trigger must not contain return value.");
		public static readonly TypeErrorMessage RETURN_MISMATCH = new TypeErrorMessage(19, "Return value does not match the return type.");
		public static readonly TypeErrorMessage VARDEC_REDEF = new TypeErrorMessage(20, "Variable has already been defined.");
		public static readonly TypeErrorMessage VARDEC_MISMATCH = new TypeErrorMessage(21, "Assigned value does not match the type of the variable.");
		public static readonly TypeErrorMessage AWAIT_NOTTRIGGER = new TypeErrorMessage(22, "Token is not trigger.");
		public static readonly TypeErrorMessage LISTEXPR_ALL = new TypeErrorMessage(23, "All values in a list must be of the same type.");
		public static readonly TypeErrorMessage FOREACH_LIST = new TypeErrorMessage(24, "Expression must evaluate to list.");
		public static readonly TypeErrorMessage FOREACH_ITERMISMATCH = new TypeErrorMessage(25, "Iterating value must have the same type as contained in list.");
		public static readonly TypeErrorMessage INDEX_LORT = new TypeErrorMessage(26, "Indexing must be applied to either a list or a tuple.");
		public static readonly TypeErrorMessage INDEX_EMPTY = new TypeErrorMessage(27, "Cannot index into empty lists.");
		public static readonly TypeErrorMessage INDEX_LINT = new TypeErrorMessage(28, "Index must be of integer value.");
		public static readonly TypeErrorMessage INDEX_TCINT = new TypeErrorMessage(29, "Index of tuple must be of constant integer value.");
		public static readonly TypeErrorMessage INDEX_TOOR = new TypeErrorMessage(30, "Index out of tuple range.");
		public static readonly TypeErrorMessage ARG_SHADOW = new TypeErrorMessage(31, "Argument cannot have the same name as a global entity in the same namespace.");
		public static readonly TypeErrorMessage ADDI_MISMATCH = new TypeErrorMessage(32, "Both sides must be either both be numerical or both be string literals.");
		public static readonly TypeErrorMessage CONCAT_RIGHTM = new TypeErrorMessage(33, "Right side of append must be the same type as the inner type of the left-side list.");
		public static readonly TypeErrorMessage CONCAT_LEFTM = new TypeErrorMessage(34, "Left side of append must be the same type as the inner type of the right-side list.");
		public static readonly TypeErrorMessage CONCAT_LIST = new TypeErrorMessage(35, "Both lists in concatination must be same type.");
		public static readonly TypeErrorMessage CONCAT_MISMATCH = new TypeErrorMessage(36, "At least one of the sides must evaluate to list.");
		public static readonly TypeErrorMessage UNPACK_NOTTUPLE = new TypeErrorMessage(37, "Unpacked expression must be a tuple.");
		public static readonly TypeErrorMessage UNPACK_TOOFEW = new TypeErrorMessage(38, "Too few values to unpack.");
		public static readonly TypeErrorMessage UNPACK_TOOMANY = new TypeErrorMessage(39, "Too many values to unpack.");
		public static readonly TypeErrorMessage ASSIGN_CONST = new TypeErrorMessage(40, "Cannot assign new value to a constant variable or declared function.");

		private TypeErrorMessage(int id, string message)
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
