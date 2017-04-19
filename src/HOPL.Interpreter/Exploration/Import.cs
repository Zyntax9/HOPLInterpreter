namespace HOPL.Interpreter.Exploration
{
	public class Import
	{
		public string NamespaceName;
		private string alias;
		public string Alias { get { return alias ?? NamespaceName; } }

		public Import(string @namespace, string alias = null)
		{
			NamespaceName = @namespace;
			this.alias = alias;
		}

		public override int GetHashCode()
		{
			return Alias.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			if (obj is Import)
				return ((Import)obj).Alias.Equals(Alias);
			else
				return base.Equals(obj);
		}
	}
}
