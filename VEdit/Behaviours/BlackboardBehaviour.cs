﻿using Microsoft.Xaml.Behaviors;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VEdit.Blackboard;
using VEdit.Editor;

namespace VEdit
{
    public class BlackboardBehaviour : Behavior<FrameworkElement>
    {
        public static DependencyProperty BlackboardProperty = DependencyProperty.Register(nameof(Blackboard),
            typeof(Editor.Blackboard), typeof(BlackboardBehaviour), new PropertyMetadata(null));

        public UIState<BlackboardBehaviour> State { get; set; }

        public Editor.Blackboard Blackboard
        {
            get => (Editor.Blackboard)GetValue(BlackboardProperty);
            set => SetValue(BlackboardProperty, value);
        }

        protected override void OnAttached()
        {
            State = new DefaultState(this);

            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.MouseWheel += OnMouseWheel;

            AssociatedObject.MouseRightButtonDown += OnMouseRightButtonDown;
            AssociatedObject.MouseRightButtonUp += OnMouseRightButtonUp; ;

            AssociatedObject.MouseMove += OnMouseMove;

            AssociatedObject.LayoutUpdated += OnLayoutUpdated;
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var center = GetRelativePosition(e);

            Blackboard.Zoom(e.Delta > 0 ? ZoomType.In : ZoomType.Out, center.X, center.Y);
        }

        private void OnLayoutUpdated(object sender, System.EventArgs e)
        {
            if (Blackboard != null)
            {
                Blackboard.Width = AssociatedObject.ActualWidth;
                Blackboard.Height = AssociatedObject.ActualHeight;
            }
        }

        private void OnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Canvas)
            {
                var position = GetRelativePosition(e);
                State.OnRightMouseButtonUp(position);
            }
        }

        private void OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource is Canvas)
            {
                var position = GetRelativePosition(e);
                State.OnRightMouseButtonDown(position);
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            var position = GetRelativePosition(e);
            State.OnMouseMove(position);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = GetRelativePosition(e);
            State.OnLeftMouseButtonDown(position);

            ((FrameworkElement)sender).CaptureMouse();
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var position = GetRelativePosition(e);
            State.OnLeftMouseButtonUp(position);
            ((FrameworkElement)sender).ReleaseMouseCapture();
        }

        private Point GetRelativePosition(MouseEventArgs args) => args.GetPosition(AssociatedObject);

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            AssociatedObject.MouseMove -= OnMouseMove;

            AssociatedObject.LayoutUpdated -= OnLayoutUpdated;
        }

        public void DrawTempLink(Pin pin)
        {
            State = new DrawTempLinkState(this, pin);
        }
    }
}
