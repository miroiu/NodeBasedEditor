using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using VEdit.Common;
using VEdit.Editor;

namespace VEdit.Converters
{
    public class LinkDataToPathConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            var startX = (double)value[0];
            var startY = (double)value[1];

            var endX = (double)value[2];
            var endY = (double)value[3];

            //var splits = (IReadOnlyList<Split>)value[4];

            var half = ((new Point(startX, startY) - new Point(endX, endY)) / 2).Length;
            Point anchorStart = new Point(startX - half, startY);
            Point anchorEnd = new Point(endX + half, endY);

            //if (splits.Count > 0)
            //{
            //    var split = splits[0];

            //    half = ((new Point(startX, startY) - new Point(split.X, split.Y)) / 2).Length;
            //    anchorStart = new Point(startX - half, startY);
            //    anchorEnd = new Point(split.X + half, split.Y);
            //}
            
            Point start = new Point(startX, startY);
            Point end = new Point(endX, endY);

            var bezier = new BezierSegment(anchorStart, anchorEnd, end, true);

            PathFigure figure = new PathFigure(start, new PathSegmentCollection(bezier.ToOneItemArray()), false); //true if closed
            PathGeometry geometry = new PathGeometry(figure.ToOneItemArray());

            return geometry;
        }

        public object[] ConvertBack(object value, Type[] targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
