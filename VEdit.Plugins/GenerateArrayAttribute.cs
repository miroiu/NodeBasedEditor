using System;

namespace VEdit.Plugins
{
    public enum Pattern
    {
        Empty,
        Letter,
        Number
    }

    /// <summary>
    /// Can be placed near params parameters.
    /// Represents the minimum number of pins spawned.
    /// </summary>
    [AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public class GenerateArrayAttribute : Attribute
    {
        public int Value { get; }
        public Pattern Name { get; set; }

        public GenerateArrayAttribute(int value)
        {
            Value = value;
        }
    }
}
