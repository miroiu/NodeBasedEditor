using System;

namespace VEdit.Execution
{
    public class Variable
    {
        public Variable(IExecutionContext context, Guid id, Type type)
        {
            Context = context;
            Id = id;
            Type = type;
        }

        public Guid Id { get; }
        public Type Type { get; }
        public object Value { get; set; }
        public IExecutionContext Context { get; }
    }
}
