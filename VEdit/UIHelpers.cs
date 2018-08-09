using System.Windows;
using System.Windows.Media;

namespace VEdit
{
    public static class UIHelpers
    {
        public static T FindParentOfType<T>(this FrameworkElement child) where T : FrameworkElement
        {
            DependencyObject node = child;

            while (node != null && !(node is T))
            {
                node = VisualTreeHelper.GetParent(node);
            }

            return node as T;
        }
        
        public static Point GetCenterLocationInsideParent(this FrameworkElement child, FrameworkElement parent)
        {
            var mid = new Point(child.ActualWidth / 2, child.ActualHeight / 2);
            return child.TranslatePoint(mid, parent);
        }
    }
}
