using System;
using VEdit.Common;
using VEdit.Execution;

namespace VEdit.Editor
{
    public class PrintNode : Node
    {
        public static Guid Template = Guid.Parse("9708e3f2-32ca-4629-ad92-ca1127bcdb5a");

        public PrintNode(Graph root) :base(root, Template)
        {
            Name = "Print";

            AddExecPin(Direction.Input);
            AddExecPin(Direction.Output);

            AddDataPin<string>(Direction.Input);
        }

        public override IExecutionBlock Compile(IExecutionContext context)
        {
            var output = ServiceProvider.Get<IOutputManager>();

            return new ActionBlock<string>(context, output.Write);
        }
    }
}
