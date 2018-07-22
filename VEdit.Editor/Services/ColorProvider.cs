using System;
using System.Collections.Generic;

namespace VEdit.Editor
{
    public interface IColorProvider
    {
        string Get(Type type);
    }

    public class ColorProvider : IColorProvider
    {
        const string DefaultColor = "#FFFFFF";
        private Dictionary<Guid, string> _colors;

        public ColorProvider()
        {
            _colors = new Dictionary<Guid, string>();

            // TODO: Load from config
            _colors.Add(typeof(bool).GUID, "#FF0000");
            _colors.Add(typeof(int).GUID, "#00FFFF");
            _colors.Add(typeof(double).GUID, "#7CFC00");
            _colors.Add(typeof(float).GUID, "#7CFC00");
            _colors.Add(typeof(byte).GUID, "#FDFF00");
            _colors.Add(typeof(string).GUID, "#FF00FF");
        }

        public string Get(Type type)
        {
            string value = DefaultColor;
            if (type != null)
            {
                if (!_colors.TryGetValue(type.GUID, out value))
                {
                    return DefaultColor;
                }
            }
            return value;
        }
    }
}
