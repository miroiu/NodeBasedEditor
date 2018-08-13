using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VEdit.Core
{
    internal class SocketExtractor
    {
        private readonly PropertyExtractor _propertyExtractor;
        private readonly Action<DataSocket> _applyOnNewSocket;
        private readonly Node _node;
        private readonly SocketType _socket;

        public SocketExtractor(DataSocket socket, Action<DataSocket> applyOnNewSocket) : this(socket.DataType, socket.Node, socket.Type)
        {
            if (socket is null)
            {
                throw new ArgumentNullException(nameof(socket));
            }

            _applyOnNewSocket = applyOnNewSocket;
        }

        public SocketExtractor(Type type, Node node, SocketType socket)
        {
            if(type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _propertyExtractor = new PropertyExtractor(type);
            _node = node;
            _socket = socket;
        }

        public IEnumerable<DataSocket> Sockets
        {
            get
            {
                foreach (var property in _propertyExtractor.Properties)
                {
                    yield return ExtractProperty(property);
                }
            }
        }

        private DataSocket ExtractProperty(PropertyInfo info)
        {
            return new DataSocket(_node, _socket, info.PropertyType);
        }
    }

    internal class PropertyExtractor
    {
        private readonly Type _type;
        private readonly BindingFlags _flags = BindingFlags.Public | BindingFlags.Instance;

        public PropertyExtractor(Type type)
        {
            _type = type;
        }

        public IEnumerable<PropertyInfo> Properties => _type.GetProperties(_flags).Where(p => p.CanWrite);
    }
}
