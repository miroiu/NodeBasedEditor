using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VEdit.Common
{
    public interface IArchive
    {
        IEnumerable<T> ReadAll<T>();
        T Read<T>(string key);
        object Read(string key, Type type);
        void Write(string key, object value);

        bool HasKey(string key);

        void Save(string file);
        Task SaveAsync(string file);

        bool AllowOverwrite { get; set; }

        void Load(string file);
        Task LoadAsync(string file);
    }

    public class Archive : IArchive
    {
        public bool AllowOverwrite { get; set; }

        private readonly IServiceProvider _provider;
        private readonly ISerializer<string> _serializer;
        private Dictionary<string, object> _data = new Dictionary<string, object>();

        public Archive(IServiceProvider serviceProvider)
        {
            _provider = serviceProvider;
            _serializer = _provider.Get<ISerializer<string>>();
        }

        public bool HasKey(string key)
        {
            return _data.ContainsKey(key);
        }

        public virtual T Read<T>(string key)
        {
            var obj = Read(key, typeof(T));
            if (obj == null)
            {
                return default(T);
            }
            return (T)obj;
        }

        public IEnumerable<T> ReadAll<T>()
        {
            foreach (var kvp in _data)
            {
                var result = Read(kvp.Key, typeof(T));
                if (result != null)
                {
                    yield return (T)result;
                }
            }
        }

        public object Read(string key, Type type)
        {
            if (_data.TryGetValue(key, out object value))
            {
                if (value == null)
                {
                    return null;
                }

                var json = $"\"{value.ToString()}\"";

                if (type == typeof(Archive))
                {
                    json = value.ToString();

                    return new Archive(_provider)
                    {
                        _data = _serializer.Deserialize<Dictionary<string, object>>(json)
                    };
                }

                var result = _serializer.Deserialize(json, type);

                if (result != null)
                {
                    return result;
                }

                throw new ArchiveReadException($"Cannot cast {value.GetType()} to {type}.");
            }
            return null;
        }

        public virtual void Write(string key, object data)
        {
            if (data is Archive arch)
            {
                data = arch._data;
            }

            if (_data.ContainsKey(key))
            {
                if (!AllowOverwrite)
                {
                    throw new ArchiveWriteException("Key already exists. Set AllowOverwrite to true if you want to overwrite values.");
                }

                _data[key] = data;
            }
            else
            {
                _data.Add(key, data);
            }
        }

        public void Save(string file)
        {
            var io = _provider.Get<IFileIO>();
            var data = _serializer.Serialize(_data);
            io.Write(file, data);
        }

        public async Task SaveAsync(string file)
        {
            var io = _provider.Get<IFileIO>();
            var data = _serializer.Serialize(_data);
            await io.WriteAsync(file, data);
        }

        public void Load(string file)
        {
            var io = _provider.Get<IFileIO>();
            var json = io.Read(file);
            _data = _serializer.Deserialize<Dictionary<string, object>>(json);
        }

        public async Task LoadAsync(string file)
        {
            var io = _provider.Get<IFileIO>();
            var json = await io.ReadAsync(file);
            _data = _serializer.Deserialize<Dictionary<string, object>>(json);
        }
    }

    [Serializable]
    public class ArchiveReadException : Exception
    {
        public ArchiveReadException(string message = null) : base(message)
        {
        }

        public ArchiveReadException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    [Serializable]
    public class ArchiveWriteException : Exception
    {
        public ArchiveWriteException(string message = null) : base(message)
        {
        }

        public ArchiveWriteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
