using System;
using System.Collections.Generic;

namespace VEdit.Core
{
    [Serializable]
    public abstract class Graph
    {
        private readonly HashSet<Node> _nodes = new HashSet<Node>();
        public IEnumerable<Node> Nodes => _nodes;

        public void AddNode(Node node)
        {
            if(node is null)
            {
                throw new ArgumentNullException(nameof(node));
            }

            if(!_nodes.Add(node))
            {
                throw new ArgumentException("Node is already present in the collection.");
            }
        }

        public bool TryAddNode(Node node)
        {
            return !(node is null) && _nodes.Add(node); 
        }
    }
}
