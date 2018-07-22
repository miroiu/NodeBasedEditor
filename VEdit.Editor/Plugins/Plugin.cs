using System;
using System.Reflection;

namespace VEdit.Editor
{
    public class Plugin : NodeEntry
    {
        public Plugin(Guid instanceId, MethodInfo method) : base(typeof(PluginNode), instanceId)
        {
            Method = method;
        }

        public MethodInfo Method { get; }
    }
}
