using System;

namespace VEdit.Execution
{
    public class GetVariableBlock : ExecutionBlock
    {
        private Guid _id;

        public GetVariableBlock(Guid id, IExecutionContext context) : base(context)
        {
            _id = id;
        }

        public override void Execute()
        {
            base.Execute();

            var variable = Context.GetVariable(_id);
            Out[0].Value = variable.Value;
        }
    }
}
