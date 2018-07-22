namespace VEdit.Execution
{
    public class BranchBlock : ExecutionBlock
    {
        public BranchBlock(IExecutionContext context) : base(context)
        {
        }

        public override void Execute()
        {
            base.Execute();

            var condition = (bool)In[0].Value;

            if (condition)
            {
                Next[0]?.Execute();
            }
            else
            {
                Next[1]?.Execute();
            }
        }
    }
}
