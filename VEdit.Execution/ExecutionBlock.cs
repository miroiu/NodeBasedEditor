using System.Collections.Generic;

namespace VEdit.Execution
{
    public interface IExecutionBlock
    {
        IExecutionContext Context { get; }

        List<IExecutionBlock> Next { get; }
        List<IExecutionBlock> Dependency { get; }

        List<IParameter> In { get; }
        List<IParameter> Out { get; }

        void Execute();
    }

    public class ExecutionBlock : IExecutionBlock
    {
        public IExecutionContext Context { get; }

        public List<IExecutionBlock> Next { get; } = new List<IExecutionBlock>();
        public List<IExecutionBlock> Dependency { get; } = new List<IExecutionBlock>();

        public List<IParameter> In { get; } = new List<IParameter>();
        public List<IParameter> Out { get; } = new List<IParameter>();

        public ExecutionBlock(IExecutionContext context)
        {
            Context = context;
        }

        public virtual void Execute()
        {
            foreach (var dep in Dependency)
            {
                dep.Execute();
            }
        }

        protected void ExecuteNext()
        {
            if (Next.Count > 0)
            {
                Next[0]?.Execute();
            }
        }
    }
}
