using System;
using System.Collections.Generic;

namespace VEdit.Core
{
    public enum SocketType
    {
        Input,
        Output
    }

    [Serializable]
    public abstract class Socket
    {
        public Node Node { get; }

        public string Name { get; set; }
        public SocketType Type { get; }

        public Socket(Node node, SocketType type)
        {
            Type = type;
        }
    }

    [Serializable]
    public class ExecSocket : Socket
    {
        public ExecSocket(Node node, SocketType type) : base(node, type)
        {
        }
    }

    [Serializable]
    public class DataSocket : Socket
    {
        public DataSocket Parent { get; private set; }
        public Type DataType { get; }

        public DataSocket(Node node, SocketType type, Type dataType) : base(node, type)
        {
            DataType = dataType;
            _socketExtractor = new Lazy<SocketExtractor>(() => new SocketExtractor(this, socket => Parent = socket));
        }

        private Lazy<SocketExtractor> _socketExtractor;

        private readonly List<DataSocket> _children;
        public IEnumerable<DataSocket> Children => _socketExtractor.Value.Sockets;
    }

    [Serializable]
    public class DataSocket<T> : DataSocket
    {
        public DataSocket(Node node, SocketType type) : base(node, type, typeof(T))
        {
        }
    }
}
