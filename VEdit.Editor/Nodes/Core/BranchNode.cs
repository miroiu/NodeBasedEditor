using System;
using VEdit.Execution;

namespace VEdit.Editor
{
    public class BranchNode : Node
    {
        public static Guid Template = Guid.Parse("12b0823f-5cd3-4517-979a-3349cccb6ded");

        public BranchNode(Graph root) : base(root, Template)
        {
            Name = "Branch";

            AddExecPin(Direction.Input);
            AddExecPin(Direction.Output, "True");
            AddExecPin(Direction.Output, "False");

            AddDataPin<bool>(Direction.Input, "Condition");
        }

        public override IExecutionBlock Compile(IExecutionContext context)
        {
            return new BranchBlock(context);
        }
    }
}
