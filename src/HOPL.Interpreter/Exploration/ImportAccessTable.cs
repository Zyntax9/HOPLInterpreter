using HOPL.Interpreter.NamespaceTypes;
using System;
using System.Collections.Generic;

namespace HOPL.Interpreter.Exploration
{
	public class ImportAccessTable : Dictionary<string, HashSet<Import>>
	{
		public ImportAccessTable() : base() { }

		public bool TryGetImport(string file, string importName, out Import accessImport)
		{
			accessImport = null;
			HashSet<Import> access;
			if (!TryGetValue(file, out access))
				return false;

			foreach (Import import in access)
			{
				if (import.Alias == importName)
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
