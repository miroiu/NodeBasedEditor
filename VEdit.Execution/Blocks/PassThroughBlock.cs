namespace VEdit.Execution
{
    public class PassThroughBlock : ExecutionBlock
    {
        public PassThroughBlock(IExecutionContext context) : base(context)
        {
            
        }

        public override void Execute()
        {
            base.Execute();

            ExecuteNext();
        }
    }
}
