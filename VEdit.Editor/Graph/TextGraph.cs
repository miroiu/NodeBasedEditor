using VEdit.Common;

namespace VEdit.Editor
{
    public class TextGraph : Graph
    {
        public TextGraph(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "New Text Graph";
        }

        public override EntryNode BuildEntryNode(NodeBuilder<EntryNode> builder)
        {
            var node = builder.Build() as EntryNode;
            node.IsEditable = false;
            node.X = 100;
            node.Y = 300;

            node.AddExecPin(Direction.Output);
            node.AddDataPin<string>(Direction.Output, "Text");

            return node;
        }

        public override ExitNode BuildReturnNode(NodeBuilder<ExitNode> builder)
        {
            var node = builder.Build() as ExitNode;
            node.IsEditable = false;
            node.X = 750;
            node.Y = 300;

            node.AddExecPin(Direction.Input);
            node.AddDataPin<string>(Direction.Input, "Text");

            return node;
        }
    }
}
