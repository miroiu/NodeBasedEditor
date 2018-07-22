using VEdit.Common;

namespace VEdit.Editor
{
    public class FunctionGraph : Graph
    {
        public FunctionGraph(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "New Function Graph";
        }

        public override EntryNode BuildEntryNode(NodeBuilder<EntryNode> builder)
        {
            var node = builder.Build() as EntryNode;
            node.IsEditable = true;
            node.X = 100;
            node.Y = 300;

            node.AddExecPin(Direction.Output, canBeRenamed: true);

            return node;
        }

        public override ExitNode BuildReturnNode(NodeBuilder<ExitNode> builder)
        {
            var node = builder.Build() as ExitNode;
            node.IsEditable = true;
            node.X = 750;
            node.Y = 300;

            node.AddExecPin(Direction.Input, canBeRenamed: true);

            return node;
        }
    }
}
