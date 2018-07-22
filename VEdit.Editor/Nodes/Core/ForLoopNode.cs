using System;
using VEdit.Execution;

namespace VEdit.Editor
{
    public class ForLoopNode : Node
    {
        public static Guid Template = Guid.Parse("06e73288-b47c-4e57-933c-3265938a0584");

        public ForLoopNode(Graph root) : base(root, Template)
        {
            AddExecPin(Direction.Input);
            AddExecPin(Direction.Output, "Loop");
            AddExecPin(Direction.Output, "Completed");

            AddDataPin<int>(Direction.Input, "Start");
            AddDataPin<int>(Direction.Input, "End");
            AddDataPin<int>(Direction.Output, "Index");
        }

        public override IExecutionBlock Compile(IExecutionContext context)
        {
            return new ForLoopBlock(context);
        }
    }
}
