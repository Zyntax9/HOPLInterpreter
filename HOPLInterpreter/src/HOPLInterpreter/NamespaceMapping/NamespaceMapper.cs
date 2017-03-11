using HOPLInterpreter.Faults.Parsing;
using System.IO;
using System.Collections.Generic;
using Parser = HOPLGrammar.HOPLGrammarParser;
using Lexer = HOPLGrammar.HOPLGrammarLexer;
using Antlr4.Runtime;

namespace HOPLInterpreter.NamespaceMapping
{
	public class NamespaceMapper
	{
		const string extension = ".txt";

		public static Dictionary<string, ISet<string>> MapNamespaces(ISet<string> importPaths)
		{
			Dictionary<string, ISet<string>> map = new Dictionary<string, ISet<string>>();

			List<string> files = new List<string>();
			foreach (string path in importPaths)
			{
				string[] pFiles = Directory.GetFiles(path, "*" + extension);
				files.AddRange(pFiles);
			}

			foreach (string file in files)
			{
				Lexer lexer;
				try
				{
					lexer = GenerateLexer(file);
				}
				catch
				{
					continue; // Ignore if read failed.
				}

				foreach (string namespaceName in ExtractAllNamespaces(lexer))
				{
					if (!map.ContainsKey(namespaceName))
						map.Add(namespaceName, new HashSet<string>());
					map[namespaceName].Add(file);
				}
			}

			return map;
		}

		private static Lexer GenerateLexer(string file)
		{
			AntlrInputStream ais;
			using (FileStream fs = File.Open(file, FileMode.Open))
				ais = new AntlrInputStream(fs);
			return new Lexer(ais);
		}

		private static List<string> ExtractAllNamespaces(Lexer lexer)
		{
			List<string> namespaces = new List<string>();

			for (IToken token = lexer.NextToken(); token.Type != Parser.Eof; token = lexer.NextToken())
				if (token.Type == Parser.NAMESPACE_KW)
					namespaces.Add(ExtractNextNamespace(lexer));

			return namespaces;
		}

		private static string ExtractNextNamespace(Lexer lexer)
		{
			string namespaceName = "";

			for (IToken token = lexer.NextToken();
				token.Type == Parser.ID || token.Type == Parser.DOT || token.Type == Parser.WS;
				token = lexer.NextToken())
			{
				if (token.Type == Parser.WS)
					continue;
				namespaceName += token.Text;
			}

			return namespaceName;
		}
	}
}
