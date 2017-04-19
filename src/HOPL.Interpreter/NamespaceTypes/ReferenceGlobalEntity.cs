using System;
using System.Collections.Generic;
using System.Text;
using HOPL.Grammar;
using HOPL.Interpreter.NamespaceTypes.Values;
using HOPL.Interpreter.Api;

namespace HOPL.Interpreter.NamespaceTypes
{
    class ReferenceGlobalEntity : IGlobalEntity
    {
        public string Name { get; protected set; }
        public InterpreterType Type { get; protected set; }
        public bool Required { get { return false; } }
        public bool Constant { get { return true; } }
        public bool Supplied { get { return true; } }
        public HOPLGrammarParser.ExprContext DefaultValue { get { return null; } }
        public bool HasDefaultValue { get { return false; } }
        public InterpreterValue Value
        {
            get { return InterpreterValue.FromNative(ReferencedObject.Value); }
            set { throw new InvalidOperationException(); }
        }

        public ReferenceObject ReferencedObject { get; set; } 

        public ReferenceGlobalEntity(string name, ReferenceObject obj)
        {
            Name = name;
            Type = InterpreterType.FromNative(obj.Value.GetType());
        }

        public void LockRead() { }
        public void LockWrite() { }
        public void ReleaseRead() { }
        public void ReleaseWrite() { }
    }
}
