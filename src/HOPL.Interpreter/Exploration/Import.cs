using HOPL.Interpreter.NamespaceTypes;

namespace HOPL.Interpreter.Exploration
{
	public class Import
	{
		public NamespaceString NamespaceName;
		private NamespaceString alias;
		public NamespaceString Alias { get { return alias ?? NamespaceName; } }

		public Import(string @namespace, string alias = null)
		{
			NamespaceName = new NamespaceString(@namespace);

            if(alias != null)
			    this.alias = new NamespaceString(alias);
        }

        public Import(NamespaceString @namespace, NamespaceString alias = null)
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
