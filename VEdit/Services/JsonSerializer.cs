using Newtonsoft.Json;
using System;
using VEdit.Common;

namespace VEdit
{
    public class JsonSerializer : ISerializer<string>
    {
        public Formatting Formatting { get; set; } = Formatting.Indented;

        public T Deserialize<T>(string content) => JsonConvert.DeserializeObject<T>(content);

        public object Deserialize(string content, Type type) => JsonConvert.DeserializeObject(content, type);

        public string Serialize<U>(U content) => JsonConvert.SerializeObject(content, Formatting);
    }
}
