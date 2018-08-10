using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using VEdit.Editor;

namespace VEdit
{
    public class ResizeGripBehaviour : Behavior<Thumb>
    {
        public IBlackboardElement Element
        {
            get => (IBlackboardElement)GetValue(ElementProperty);
            set => SetValue(ElementProperty, value);
        }

        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(nameof(Element),
            typeof(IBlackboardElement), typeof(ResizeGripBehaviour), new PropertyMetadata(null));

        private Cursor _cursor;

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.DragStarted += OnDragStarted;
            AssociatedObject.DragCompleted += OnDragCompleted;
            AssociatedObject.DragDelta += OnDragDelta;
            AssociatedObject.Loaded += OnLoaded;
        }

        private void OnDragCompleted(object sender, DragCompletedEventArgs e)
        {
            AssociatedObject.Cursor = _cursor;
        }

        private void OnDragStarted(object sender, DragStartedEventArgs e)
        {
            _cursor = AssociatedObject.Cursor;
            AssociatedObject.Cursor = Cursors.SizeNWSE;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            if (Element == null)
            {
                var parent = AssociatedObject.FindParentOfType<UserControl>();
                Element = parent.DataContext as IBlackboardElement;
            }
        }

        private void OnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var deltaX = Element.Width + e.HorizontalChange > 0 ? e.HorizontalChange : 0;
            var deltaY = Element.Height + e.VerticalChange > 0 ? e.VerticalChange : 0;

            Element.Width += deltaX;
            Element.Height += deltaY;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            
            AssociatedObject.DragDelta -= OnDragDelta;
            AssociatedObject.Loaded -= OnLoaded;
        }
    }
}
