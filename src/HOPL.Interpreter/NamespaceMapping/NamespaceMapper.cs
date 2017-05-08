using System.IO;
using System.Collections.Generic;
using Parser = HOPL.Grammar.HOPLGrammarParser;
using Lexer = HOPL.Grammar.HOPLGrammarLexer;
using Antlr4.Runtime;
using HOPL.Interpreter.NamespaceTypes;

namespace HOPL.Interpreter.NamespaceMapping
{
	public class NamespaceMapper
	{
		const string extension = ".txt";

		public static NamespaceFileMap MapNamespaces(ISet<string> importPaths)
		{
			NamespaceFileMap map = new NamespaceFileMap();

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

				foreach (NamespaceString namespaceName in ExtractAllNamespaces(lexer))
				{
                    map.AddFile(namespaceName, file);
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

		private static List<NamespaceString> ExtractAllNamespaces(Lexer lexer)
		{
			List<NamespaceString> namespaces = new List<NamespaceString>();

			for (IToken token = lexer.NextToken(); token.Type != Parser.Eof; token = lexer.NextToken())
				if (token.Type == Parser.NAMESPACE_KW)
					namespaces.Add(ExtractNextNamespace(lexer));

			return namespaces;
		}

		private static NamespaceString ExtractNextNamespace(Lexer lexer)
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

			return new NamespaceString(namespaceName);
		}
	}
}
