namespace VEdit.Execution
{
    public class ForLoopBlock : ExecutionBlock
    {
        public ForLoopBlock(IExecutionContext context) : base(context)
        {
        }

        public override void Execute()
        {
            base.Execute();

            var start = (int)In[0].Value;
            var end = (int)In[1].Value;

            var loop = Next[0];
            if (loop != null)
            {
                for (int i = start; i < end; i++)
                {
                    Out[0].Value = i;
                    loop.Execute();
                }
            }

            Next[1]?.Execute();
        }
    }
}
