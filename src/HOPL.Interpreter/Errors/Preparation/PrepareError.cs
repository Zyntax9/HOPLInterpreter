using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HOPL.Interpreter.Errors.Preparation
{
	public class PrepareError : IError
	{
		public PrepareErrorMessage Message { get; protected set; }
		public string Info { get; protected set; }

		string IError.Message { get { return Message.message; } }
		public int ID { get { return Message.id; } }
		public int LineNumber { get { return 0; } }
		public int ColumnNumber { get { return 0; } }

        public static string ErrorType = "Preparation Error";
        public string ErrorTypeName { get { return "Preparation Error"; } }

		public PrepareError(PrepareErrorMessage message, string info)
		{
			Message = message;
			Info = info;
		}
	}
}
