using System.Collections.Generic;
using System.ComponentModel;

namespace VEdit.Editor
{
    public interface IElement : ISelectable, IDraggable, INotifyPropertyChanged
    {
        IBlackboard Parent { get; }

        double X { get; set; }
        double Y { get; set; }

        double Width { get; set; }
        double Height { get; set; }

        string Name { get; set; }
        string Description { get; }
    }

    public static class ElementExtensions
    {
        public static (double X, double Y, double Width, double Height) GetBoundingBox<T>(this IEnumerable<T> elements, double offset = 0) where T : IElement
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;

            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach (IElement elem in elements)
            {
                if(elem.X < minX)
                {
                    minX = elem.X;
                }

                if (elem.Y < minY)
                {
                    minY = elem.Y;
                }

                var sizeX = elem.X + elem.Width;
                if (sizeX > maxX)
                {
                    maxX = sizeX;
                }

                var sizeY = elem.Y + elem.Height;
                if(sizeY > maxY)
                {
                    maxY = sizeY;
                }
            }

            return (minX - offset, minY - offset, maxX - minX + offset * 2, maxY - minY + offset * 2);
        }
    }
}
