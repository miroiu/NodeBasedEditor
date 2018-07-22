using System;
using System.Collections.Generic;

namespace VEdit.Execution
{
    public interface IExecutionContext
    {
        IExecutionContext Parent { get; }

        Variable AddVariable(Guid id, Type type);
        Variable GetVariable(Guid id);
    }

    public class ExecutionContext : IExecutionContext
    {
        private Dictionary<Guid, Variable> _variables = new Dictionary<Guid, Variable>();

        public ExecutionContext(IExecutionContext parent)
        {
            Parent = parent;
        }

        public IExecutionContext Parent { get; }

        public Variable AddVariable(Guid id, Type type)
        {
            if (_variables.TryGetValue(id, out Variable value))
            {
                return value;
            }

            value = new Variable(this, id, type);
            _variables.Add(id, value);
            return value;
        }

        public Variable GetVariable(Guid id)
        {
            if(_variables.TryGetValue(id, out Variable value))
            {
                return value;
            }
            return default(Variable);
        }
    }
}
