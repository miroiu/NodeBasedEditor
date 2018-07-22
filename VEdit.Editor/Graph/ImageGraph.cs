using VEdit.Common;

namespace VEdit.Editor
{
    public class ImageGraph : Graph
    {
        public ImageGraph(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Name = "New Image Graph";
        }

        public override EntryNode BuildEntryNode(NodeBuilder<EntryNode> builder)
        {
            var node = builder.Build() as EntryNode;
            node.IsEditable = false;
            node.X = 100;
            node.Y = 300;

            node.AddExecPin(Direction.Output);
            node.AddDataPin<double[]>(Direction.Output, "Red");
            node.AddDataPin<double[]>(Direction.Output, "Green");
            node.AddDataPin<double[]>(Direction.Output, "Blue");
            node.AddDataPin<int>(Direction.Output, "Width");
            node.AddDataPin<int>(Direction.Output, "Height");

            return node;
        }

        public override ExitNode BuildReturnNode(NodeBuilder<ExitNode> builder)
        {
            var node = builder.Build() as ExitNode;
            node.IsEditable = false;
            node.X = 750;
            node.Y = 300;

            node.AddExecPin(Direction.Input);
            node.AddDataPin<double[]>(Direction.Input, "Red");
            node.AddDataPin<double[]>(Direction.Input, "Green");
            node.AddDataPin<double[]>(Direction.Input, "Blue");
            node.AddDataPin<int>(Direction.Input, "Width");
            node.AddDataPin<int>(Direction.Input, "Height");

            return node;
        }
    }
}
