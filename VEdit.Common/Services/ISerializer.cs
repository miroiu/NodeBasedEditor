using System;

namespace VEdit.Common
{
    public interface ISerializer<T>
    {
        T Serialize<U>(U content);
        U Deserialize<U>(T content);
        object Deserialize(string content, Type type);
    }
}
