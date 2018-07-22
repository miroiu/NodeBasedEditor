using System;
using VEdit.Execution;

namespace VEdit.Editor
{
    public class EntryNode : EditableNode
    {
        public static Guid InstanceId = Guid.Parse("b926fe9a-8ff7-4542-9744-069265d3a0ed");

        public EntryNode(Graph root) : base(root)
        {
            CanCopy = false;
            Id = InstanceId;
        }

        public override Pin AddDataPin(Type dataType, string name = null)
        {
            return AddDataPin(Direction.Output, dataType, name, IsEditable, IsEditable);
        }

        public override Pin AddExecPin(string name = null)
        {
            return AddExecPin(Direction.Output, name, IsEditable, IsEditable);
        }

        public override IExecutionBlock Compile(IExecutionContext context)
        {
            return new PassThroughBlock(context);
        }
    }
}
