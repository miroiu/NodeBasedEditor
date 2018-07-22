using System;
using VEdit.Execution;

namespace VEdit.Editor
{
    public class SplitNode : Node
    {
        public SplitNode(Graph root) : base(root, Guid.Empty)
        {
        }

        public override IExecutionBlock Compile(IExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
