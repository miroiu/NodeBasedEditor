using System;
using VEdit.Execution;

namespace VEdit.Editor
{
    public class WhileNode : Node
    {
        public static Guid Template = Guid.Parse("0d7bca4b-aeb5-4e1d-9747-c4b0ad801ac7");

        public WhileNode(Graph root) : base(root, Template)
        {
            Name = "While";

            AddExecPin(Direction.Input);
            AddExecPin(Direction.Output, "Loop");
            AddExecPin(Direction.Output, "Completed");

            AddDataPin<bool>(Direction.Input, "Condition");
        }

        public override IExecutionBlock Compile(IExecutionContext context)
        {
            return new WhileBlock(context);
        }
    }
}
