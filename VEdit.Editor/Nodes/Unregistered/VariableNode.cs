using System;
using System.Collections.Generic;
using VEdit.Common;
using VEdit.Execution;

namespace VEdit.Editor
{
    public class VariableNode : Node
    {
        public static Guid Template = Guid.Parse("02b0facf-e33c-4597-b61d-d2697c495dd4");

        private List<Node> _nodes = new List<Node>();

        public Type Type { get; }
        public object DefaultValue => Input[0].DefaultValue;

        public VariableNode(Graph root, Type type, Guid templateId = default(Guid)) : base(root, templateId)
        {
            CanCopy = false;
            Type = type;

            AddDataPin(Direction.Output, Type, "Value");
            AddDataPin(Direction.Input, Type);

            var cmdProvider = ServiceProvider.Get<ICommandProvider>();
            Actions.Add("Get", cmdProvider.Create(CreateGetNodeCommand));
            Actions.Add("Set", cmdProvider.Create(CreateSetNodeCommand));
            Actions.Add("Rename", cmdProvider.Create(RenameNode));
        }

        private async void RenameNode()
        {
            var dialog = ServiceProvider.Get<IDialogManager>();
            Name = await dialog.ShowInputAsync("Rename", "Enter new name");
            OnRename();
        }

        public GetNode CreateGetNode()
        {
            var getNode = new GetNode(this);
            Graph.AddNode(getNode);
            _nodes.Add(getNode);

            return getNode;
        }

        public SetNode CreateSetNode()
        {
            var setNode = new SetNode(this);
            Graph.AddNode(setNode);
            _nodes.Add(setNode);

            return setNode;
        }

        private void CreateSetNodeCommand()
        {
            CreateSetNode();
        }

        private void CreateGetNodeCommand()
        {
            CreateGetNode();
        }

        public override void Delete()
        {
            base.Delete();

            foreach (var node in _nodes)
            {
                node.Delete();
            }
        }

        public event Action Renamed;

        protected void OnRename()
        {
            Renamed?.Invoke();
        }

        public override void Save(IArchive archive)
        {
            base.Save(archive);
            archive.Write(nameof(Type), Type);

            var nodesArc = ServiceProvider.Get<IArchive>();
            archive.Write(nameof(Graph.Nodes), nodesArc);

            foreach (var node in _nodes)
            {
                var nodeArc = ServiceProvider.Get<IArchive>();
                node.Save(nodeArc);
                nodeArc.Write(nameof(Type), node.GetType());
                nodesArc.Write(node.GetHashCode().ToString(), nodeArc);
            }
        }

        public override void Load(IArchive archive)
        {
            base.Load(archive);

            var nodesArc = archive.Read<Archive>(nameof(Graph.Nodes));

            foreach (var arc in nodesArc.ReadAll<Archive>())
            {
                var type = arc.Read<Type>(nameof(Type));
                Node result = null;
                if (type == typeof(GetNode))
                {
                    result = CreateGetNode();
                }
                else
                {
                    result = CreateSetNode();
                }
                result.Load(arc);
            }
        }

        public override IExecutionBlock Compile(IExecutionContext context)
        {
            var variable = context.AddVariable(Id, Type);
            variable.Value = DefaultValue;
            return new GetVariableBlock(Id, context);
        }
    }

    // TODO:
    public class VariableNode<T> : VariableNode
    {
        public VariableNode(Graph root, Guid templateId) : base(root, typeof(T), templateId)
        {
        }
    }
}
