using HOPL.Interpreter.NamespaceTypes.Values;
using Parser = HOPL.Grammar.HOPLGrammarParser;

namespace HOPL.Interpreter.NamespaceTypes
{
    public interface IGlobalEntity
	{
		string Name { get; }
		InterpreterType Type { get; }
		InterpreterValue Value { get; set; }
		bool Required { get; }
		bool Constant { get; }
		bool Supplied { get; }
		Parser.ExprContext DefaultValue { get; }
		bool HasDefaultValue { get; }
        
        void LockRead();
        void LockWrite();
        void ReleaseRead();
        void ReleaseWrite();
    }
}
