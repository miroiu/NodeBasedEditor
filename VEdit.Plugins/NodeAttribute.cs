using System;

namespace VEdit.Plugins
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class NodeAttribute : Attribute
    {
        public string Category { get; set; }

        public string DisplayName { get; set; }

        public string Tooltip { get; set; }

        public string Keywords { get; set; }

        public string ReturnName { get; set; }
    }
}
