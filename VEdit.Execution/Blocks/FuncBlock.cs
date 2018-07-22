using System;

namespace VEdit.Execution
{
    public class FuncBlock<T, R> : ExecutionBlock
    {
        private Func<T, R> _func;

        public FuncBlock(IExecutionContext context, Func<T, R> func) : base(context)
        {
            _func = func;
        }

        public override void Execute()
        {
            base.Execute();

            Out[0].Value = _func.Invoke((T)In[0].Value);

            ExecuteNext();
        }
    }
}
