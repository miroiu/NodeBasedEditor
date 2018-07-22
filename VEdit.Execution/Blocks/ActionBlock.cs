using System;

namespace VEdit.Execution
{
    public class ActionBlock<T> : ExecutionBlock
    {
        private Action<T> _action;

        public ActionBlock(IExecutionContext context, Action<T> action) : base(context)
        {
            _action = action;
        }

        public override void Execute()
        {
            base.Execute();

            _action.Invoke((T)In[0].Value);

            ExecuteNext();
        }
    }
}
