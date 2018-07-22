using System;
using System.Collections.Generic;

namespace VEdit.Editor
{
    public interface INodeFactory
    {
        void Register<T>(Func<Graph, Guid, Node> factory) where T: Node;
        Node Create<T>(Graph root, Guid templateId);
        Node Create(Type type, Graph root, Guid templateId);
    }

    public class NodeFactory : INodeFactory
    {
        private Dictionary<Type, Func<Graph, Guid, Node>> _factory = new Dictionary<Type, Func<Graph, Guid, Node>>();

        public NodeFactory(Common.IServiceProvider serviceProvider)
        {
            // Mixed
            Register<EntryNode>((root, templateID) => new EntryNode(root));
            Register<ExitNode>((root, templateID) => new ExitNode(root));
            Register<GraphNode>((root, templateID) => new GraphNode(root, templateID));
            Register<SplitNode>((root, templateID) => new SplitNode(root));
            Register<PrintNode>((root, templateID) => new PrintNode(root));
            Register<ToStringNode>((root, templateID) => new ToStringNode(root));

            // Flow
            Register<SequenceNode>((root, templateID) => new SequenceNode(root));
            Register<ForLoopNode>((root, templateID) => new ForLoopNode(root));
            Register<BranchNode>((root, templateID) => new BranchNode(root));
            Register<WhileNode>((root, templateID) => new WhileNode(root));

            // Variables
            Register<VariableNode<int>>((root, templateID) =>  new VariableNode<int>(root, templateID));
            Register<VariableNode<bool>>((root, templateID) =>  new VariableNode<bool>(root, templateID));
            Register<VariableNode<float>>((root, templateID) =>  new VariableNode<float>(root, templateID));
            Register<VariableNode<string>>((root, templateID) =>  new VariableNode<string>(root, templateID));
        }

        public Node Create<T>(Graph root, Guid templateId)
        {
            return Create(typeof(T), root, templateId);
        }

        public Node Create(Type type, Graph root, Guid templateId)
        {
            if (_factory.TryGetValue(type, out Func<Graph, Guid, Node> creator))
            {
                return creator(root, templateId);
            }
            return null;
        }

        public void Register<T>(Func<Graph, Guid, Node> factory) where T: Node
        {
            _factory.Add(typeof(T), factory);
        }
    }
}
