namespace VEdit.Execution
{
    public class SequenceBlock : ExecutionBlock
    {
        public SequenceBlock(IExecutionContext context) : base(context)
        {
        }

        public override void Execute()
        {
            base.Execute();

            foreach (var block in Next)
            {
                block.Execute();
            }
        }
    }

}
