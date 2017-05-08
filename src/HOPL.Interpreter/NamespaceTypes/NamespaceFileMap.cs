using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HOPL.Interpreter.NamespaceTypes
{
    public class NamespaceFileMap
    {
        Dictionary<string, MapNode> map = new Dictionary<string, MapNode>();

        public NamespaceFileMap() { }

        public void AddFile(NamespaceString @namespace, string file)
        {
            string topName = @namespace.Parts[0];
            if (string.IsNullOrEmpty(topName))
                return;

            if (!map.ContainsKey(topName))
                map.Add(topName, new MapNode(topName));

            map[topName].AddFile(@namespace.Tail, file);
        }

        public ISet<string> GetFilesUnderNamespace(NamespaceString @namespace)
        {
            string topName = @namespace.Parts[0];

            if (!map.ContainsKey(topName))
                return new HashSet<string>();

            MapNode node;
            if (!map[topName].TryGetSubNamespace(@namespace.Tail, out node))
                return new HashSet<string>();
            return node.Flatten();
        }

        private class MapNode
        {
            public string Name { get; set; }
            public ISet<string> Files { get; set; } = new HashSet<string>();
            public Dictionary<string, MapNode> SubNodes { get; set; } = new Dictionary<string, MapNode>();

            public MapNode(string name)
            {
                Name = name;
            }

            public void AddFile(NamespaceString subNamespace, string file)
            {
                if(string.IsNullOrEmpty(subNamespace.Name))
                {
                    Files.Add(file);
                    return;
                }

                string topSub = subNamespace.Parts[0];
                if (!SubNodes.ContainsKey(topSub))
                    SubNodes.Add(topSub, new MapNode(topSub));
                
                SubNodes[topSub].AddFile(subNamespace.Tail, file);
            }

            public bool TryGetSubNamespace(NamespaceString subNamespace, out MapNode node)
            {
                node = null;

                if (string.IsNullOrEmpty(subNamespace.Name))
                {
                    node = this;
                    return true;
                }

                string topSub = subNamespace.Parts[0];
                if (!SubNodes.ContainsKey(topSub))
                    return false;
                return SubNodes[topSub].TryGetSubNamespace(subNamespace.Tail, out node);
            }

            public MapNode GetSubNamespace(NamespaceString subNamespace)
            {
                if (string.IsNullOrEmpty(subNamespace.Name))
                    return this;

                string topSub = subNamespace.Parts[0];
                return SubNodes[topSub].GetSubNamespace(subNamespace.Tail);
            }

            public ISet<string> Flatten()
            {
                ISet<string> flattenedSet = Files;
                foreach(KeyValuePair<string, MapNode> pair in SubNodes)
                {
                    ISet<string> flatNode = pair.Value.Flatten();
                    flattenedSet = new HashSet<string>(flattenedSet.Union(flatNode));
                }
                return flattenedSet;
            }
        }
    }
}
