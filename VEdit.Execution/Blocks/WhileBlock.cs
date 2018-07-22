namespace VEdit.Execution
{
    public class WhileBlock : ExecutionBlock
    {
        public WhileBlock(IExecutionContext context) : base(context)
        {
        }

        public override void Execute()
        {
            base.Execute();
            var condition = (bool)In[0].Value;

            while (condition)
            {
                base.Execute();
                condition = (bool)In[0].Value;

                Next[0]?.Execute();
            }

            Next[1]?.Execute();
        }
    }
}
