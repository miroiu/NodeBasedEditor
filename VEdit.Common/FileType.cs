using System;

namespace VEdit.Common
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = false)]
    public class ExtensionAttribute : Attribute
    {
        public string[] Extensions { get; }

        public ExtensionAttribute(params string[] extensions) => Extensions = extensions;
    }

    public enum FileType
    {
        [Extension(Extension.Graph)]
        Graph,
        [Extension(Extension.Plugin)]
        Plugin,
        [Extension("bmp", "png", "jpeg", "jpg")]
        Image,
        [Extension("txt")]
        Text,
        Unknown,
    }
}
