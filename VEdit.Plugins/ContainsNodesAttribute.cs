using System;

namespace VEdit.Plugins
{
    /// <summary>
    /// Placed on a static class containing methods marked with the node attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ContainsNodesAttribute : Attribute
    {
    }
}
