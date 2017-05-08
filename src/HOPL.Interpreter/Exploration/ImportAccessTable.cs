using HOPL.Interpreter.NamespaceTypes;
using System;
using System.Collections.Generic;

namespace HOPL.Interpreter.Exploration
{
	public class ImportAccessTable : Dictionary<string, HashSet<Import>>
	{
		public ImportAccessTable() : base() { }

		public bool TryGetImport(string file, NamespaceString importName, out Import accessImport, 
            out NamespaceString remainingSub)
		{
			accessImport = null;
            remainingSub = null;

			HashSet<Import> access;
			if (!TryGetValue(file, out access))
				return false;

			foreach (Import import in access)
			{
				if (importName.HasSub(import.Alias, out remainingSub))
				{
					accessImport = import;
					return true;
				}
			}
			return false;
		}

		public void Add(string key)
		{
			Add(key, new HashSet<Import>());
		}
	}
}
