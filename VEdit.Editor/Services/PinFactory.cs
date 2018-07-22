using System;
using VEdit.Common;

namespace VEdit.Editor
{
    public enum Direction
    {
        Input,
        Output
    }

    public interface IPinFactory
    {
        Pin CreateData(Node parent, Direction direction, Type dataType, string name = null, bool isRemovable = false, bool canBeRenamed = false);
        Pin CreateData<T>(Node parent, Direction direction, string name = null, bool isRemovable = false, bool canBeRenamed = false);
        Pin CreateExec(Node parent, Direction direction, string name = null, bool isRemovable = false, bool canBeRenamed = false);
    }

    public class PinFactory : IPinFactory
    {
        private Common.IServiceProvider _serviceProvider;
        private IColorProvider _colorProvider;

        public PinFactory(Common.IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _colorProvider = _serviceProvider.Get<IColorProvider>();
        }

        public Pin CreateData<T>(Node parent, Direction direction, string name = null, bool isRemovable = false, bool canBeRenamed = false) => CreateData(parent, direction, typeof(T), name, isRemovable, canBeRenamed);

        public Pin CreateData(Node parent, Direction direction, Type dataType, string name = null, bool isRemovable = false, bool canBeRenamed = false)
        {
            var realType = dataType;

            if(dataType.IsByRef)
            {
                realType = dataType.GetElementType();
            }

            var color = realType.IsArray ? _colorProvider.Get(realType.GetElementType()) : _colorProvider.Get(realType);

            return new Pin(parent, direction == Direction.Input, false, name, realType, color, isRemovable, canBeRenamed)
            {
                DefaultValue = dataType.GetDefaultValue(),
            };
        }

        public Pin CreateExec(Node parent, Direction direction, string name = null, bool isRemovable = false, bool canBeRenamed = false) => new Pin(parent, direction == Direction.Input, true, name, isRemovable: isRemovable, canBeRenamed: canBeRenamed);
    }
}
