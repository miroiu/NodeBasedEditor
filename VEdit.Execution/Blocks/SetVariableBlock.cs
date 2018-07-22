using System;

namespace VEdit.Execution
{
    public class SetVariableBlock : ExecutionBlock
    {
        private Guid _id;

        public SetVariableBlock(Guid variableId, IExecutionContext context) : base(context)
        {
            _id = variableId;
        }

        public override void Execute()
        {
            base.Execute();

            var variable = Context.GetVariable(_id);
            variable.Value = In[0].Value;
            Out[0].Value = In[0].Value;

            ExecuteNext();
        }
    }
}
