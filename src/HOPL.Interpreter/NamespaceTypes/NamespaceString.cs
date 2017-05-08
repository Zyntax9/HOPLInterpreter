using System;
using System.Linq;

namespace HOPL.Interpreter.NamespaceTypes
{
    public class NamespaceString
    {
        public string Name { get; protected set; }
        public string[] Parts { get; protected set; }

        public NamespaceString Tail { get { return GetTail(); } }

        public NamespaceString(string namespaceName)
        {
            Name = namespaceName;
            Parts = SplitNamespace(namespaceName);
        }

        public NamespaceString(string[] namespaceParts)
        {
            Name = JoinNamespace(namespaceParts);
            Parts = namespaceParts;
        }

        private NamespaceString GetTail()
        {
            return new NamespaceString(Parts.Skip(1).ToArray());
        }

        public bool HasSub(NamespaceString ns, out NamespaceString remainingSub)
        {
            remainingSub = null;

            NamespaceString longest = Parts.Length < ns.Parts.Length ? ns : this;
            int overlap = Math.Min(ns.Parts.Length, Parts.Length);
            int outer = longest.Parts.Length - overlap;

            for(int i = 0; i < overlap; i++)
                if (ns.Parts[i] != Parts[i])
                    return false;

            string[] remaining = new string[outer];
            Array.Copy(longest.Parts, overlap, remaining, 0, outer);
            remainingSub = new NamespaceString(remaining);

            return true;
        }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is NamespaceString)
                return Name == ((NamespaceString)obj).Name;
            else if (obj is string)
                return Name == (string)obj;
            else
                return false;
        }

        public static NamespaceString operator +(NamespaceString a, NamespaceString b)
        {
            return Combine(a, b);
        }

        public static bool operator ==(NamespaceString a, NamespaceString b)
        {
            return a.Name == b.Name;
        }

        public static bool operator !=(NamespaceString a, NamespaceString b)
        {
            return a.Name != b.Name;
        }

        private static string[] SplitNamespace(string namespaceName)
        {
            return namespaceName.Split('.');
        }

        private static string JoinNamespace(string[] namespaceParts)
        {
            return string.Join(".", namespaceParts);
        }

        private static NamespaceString Combine(NamespaceString a, NamespaceString b)
        {
            if (string.IsNullOrEmpty(a.Name))
                return b;
            if (string.IsNullOrEmpty(b.Name))
                return a;
            return new NamespaceString(a.Name + "." + b.Name);
        }
    }
}
