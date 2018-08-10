using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using VEdit.Controls;
using VEdit.Editor;
using VEdit.Element;

namespace VEdit
{
    public class ElementBehaviour : Behavior<FrameworkElement>
    {
        public UIState<ElementBehaviour> State { get; set; }

        public BlackboardElement Element
        {
            get => (BlackboardElement)GetValue(ElementProperty);
            set => SetValue(ElementProperty, value);
        }
        
        public bool UpdateSize
        {
            get => (bool)GetValue(UpdateSizeProperty);
            set => SetValue(UpdateSizeProperty, value);
        }

        public static readonly DependencyProperty ElementProperty = DependencyProperty.Register(nameof(Element),
            typeof(BlackboardElement), typeof(ElementBehaviour), new PropertyMetadata(null));

        public static readonly DependencyProperty UpdateSizeProperty = DependencyProperty.Register(nameof(UpdateSize),
            typeof(bool), typeof(ElementBehaviour), new PropertyMetadata(true));

        private BlackboardView _parent;

        protected override void OnAttached()
        {
            State = new DefaultState(this);
            _parent = AssociatedObject.FindParentOfType<BlackboardView>();

            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.MouseRightButtonUp += OnMouseRightButtonUp;

            AssociatedObject.MouseMove += OnMouseMove;
            AssociatedObject.LayoutUpdated += OnLayoutUpdated;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            AssociatedObject.MouseRightButtonUp -= OnMouseRightButtonUp;

            AssociatedObject.MouseMove -= OnMouseMove;
            AssociatedObject.LayoutUpdated -= OnLayoutUpdated;
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            var position = GetMousePositionRelativeToBlackboard(e);
            State.OnRightMouseButtonUp(position);
        }

        private void OnLayoutUpdated(object sender, System.EventArgs e)
        {
            if (UpdateSize && Element != null)
            {
                Element.Width = AssociatedObject.ActualWidth;
                Element.Height = AssociatedObject.ActualHeight;
            }
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = GetMousePositionRelativeToBlackboard(e);
            State.OnLeftMouseButtonDown(position);

            ((FrameworkElement)sender).CaptureMouse();

            e.Handled = true;
        }

        private Point GetMousePositionRelativeToBlackboard(MouseEventArgs args) => args.GetPosition(_parent);

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var position = GetMousePositionRelativeToBlackboard(e);
            State.OnLeftMouseButtonUp(position);
            ((FrameworkElement)sender).ReleaseMouseCapture();
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var element = (UIElement)sender;
            if (!element.IsMouseCaptured) return;
            var position = GetMousePositionRelativeToBlackboard(e);
            State.OnMouseMove(position);
        }
    }
}
