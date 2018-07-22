using System;

namespace VEdit.Editor
{
    public enum PinType
    {
        Data,
        Exec
    }

    public class PinData
    {
        public PinData(string name = null, Type dataType = null)
        {
            Name = name;
            Type = dataType == null ? PinType.Exec : PinType.Data;
            DataType = dataType;
        }

        public bool IsFactory { get; set; }
        public string Name { get; }
        public PinType Type { get; }
        public Type DataType { get; }
    }
}
