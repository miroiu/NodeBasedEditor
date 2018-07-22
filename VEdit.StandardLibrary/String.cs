using VEdit.Plugins;

namespace VEdit.StandardLibrary
{
    [ContainsNodes]
    public static class String
    {
        [ExecutableNode(Category = "String", Keywords = "string trim")]
        public static string Trim(string value)
        {
            return value.Trim();
        }

        [ExecutableNode(Category = "String", Keywords = "string replace")]
        public static string Replace(string value, string replace, string with)
        {
            return value.Replace(replace, with);
        }

        [ExecutableNode(Category = "String", Keywords = "string contains")]
        public static bool Contains(string value, string Value)
        {
            return value.Contains(Value);
        }

        [ExecutableNode(Category = "String", Keywords = "string letter")]
        public static string Letter(string value, int index)
        {
            return value[index].ToString();
        }
    }
}
