using System;
using System.Reflection;
using VEdit.Execution;
using System.Linq;

namespace VEdit.Editor
{
    public class PluginNode : Node
    {
        public MethodInfo Method { get; }

        public PluginNode(Graph root, Guid templatedId, bool isCompact, MethodInfo method) : base(root, templatedId)
        {
            IsCompact = isCompact;
            Method = method;
        }

        public override IExecutionBlock Compile(IExecutionContext context)
        {
            return new MethodBlock(context, Method, Output.Count(o => !o.IsExec));
        }
    }
}
