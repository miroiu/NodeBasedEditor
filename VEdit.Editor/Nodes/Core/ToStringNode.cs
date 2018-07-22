using System;
using VEdit.Execution;

namespace VEdit.Editor
{
    public class ToStringNode : Node
    {
        public static Guid Template = Guid.Parse("be085066-3203-4d22-9c20-120cd6c5360b");

        public ToStringNode(Graph root) : base(root, Template)
        {
            Name = "To string";

            AddDataPin<object>(Direction.Input);
            AddDataPin<string>(Direction.Output);
        }

        public override IExecutionBlock Compile(IExecutionContext context)
        {
            return new FuncBlock<object, string>(context, p => p.ToString());
        }
    }
}
