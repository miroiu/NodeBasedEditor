using System;
using VEdit.Execution;

namespace VEdit.Editor
{
    public class SequenceNode : Node
    {
        public static Guid Template = Guid.Parse("324f995a-e677-4a9f-a859-2acfd0e8ab9e");

        public SequenceNode(Graph root) : base(root, Template)
        {
            Name = "Sequence";

            AddExecPin(Direction.Input);
            FactoryPin = AddExecPin(Direction.Output, "Then 0");
            AddExecPin(Direction.Output, "Then 1"); 
        }

        public override IExecutionBlock Compile(IExecutionContext context)
        {
            return new SequenceBlock(context);
        }
    }
}
