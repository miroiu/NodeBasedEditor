namespace VEdit.Execution
{
    public class EntryExitBlock : ExecutionBlock
    {
        public IExecutionBlock Entry { get; }
        public IExecutionBlock Exit { get; }

        public EntryExitBlock(IExecutionBlock entry, IExecutionBlock exit, IExecutionContext context) : base(context)
        {
            Entry = entry;
            Exit = exit;

            foreach(var param in entry.Out)
            {
                In.Add(param);
            }

            foreach(var param in exit.In)
            {
                Out.Add(param);
            }
        }

        public override void Execute()
        {
            if (Entry.Dependency.Count == 0)
            {
                Entry.Next[0]?.Dependency.AddRange(Dependency);
            }

            Entry.Execute();
        }
    }
}
