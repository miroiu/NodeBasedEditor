using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace VEdit
{
    public class MouseBehaviour : Behavior<FrameworkElement>
    {
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register(
            nameof(CommandParameter), typeof(object), typeof(MouseBehaviour), new PropertyMetadata(null));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty MouseMoveCommandProperty = DependencyProperty.Register(
            nameof(MouseMoveCommand), typeof(ICommand), typeof(MouseBehaviour), new PropertyMetadata(null));

        public ICommand MouseMoveCommand
        {
            get => (ICommand)GetValue(MouseMoveCommandProperty);
            set => SetValue(MouseMoveCommandProperty, value);
        }

        public static readonly DependencyProperty LeftMouseDownCommandProperty = DependencyProperty.Register(
            nameof(LeftMouseDownCommand), typeof(ICommand), typeof(MouseBehaviour), new PropertyMetadata(null));

        public ICommand LeftMouseDownCommand
        {
            get => (ICommand)GetValue(LeftMouseDownCommandProperty);
            set => SetValue(LeftMouseDownCommandProperty, value);
        }

        public static readonly DependencyProperty LeftMouseUpCommandProperty = DependencyProperty.Register(
            nameof(LeftMouseUpCommand), typeof(ICommand), typeof(MouseBehaviour), new PropertyMetadata(null));

        public ICommand LeftMouseUpCommand
        {
            get => (ICommand)GetValue(LeftMouseUpCommandProperty);
            set => SetValue(LeftMouseUpCommandProperty, value);
        }

        public static readonly DependencyProperty RightMouseUpCommandProperty = DependencyProperty.Register(
            nameof(RightMouseUpCommand), typeof(ICommand), typeof(MouseBehaviour), new PropertyMetadata(null));

        public ICommand RightMouseUpCommand
        {
            get => (ICommand)GetValue(RightMouseUpCommandProperty);
            set => SetValue(RightMouseUpCommandProperty, value);
        }

        public static readonly DependencyProperty RightMouseDownCommandProperty = DependencyProperty.Register(
            nameof(RightMouseDownCommand), typeof(ICommand), typeof(MouseBehaviour), new PropertyMetadata(null));

        public ICommand RightMouseDownCommand
        {
            get => (ICommand)GetValue(RightMouseDownCommandProperty);
            set => SetValue(RightMouseDownCommandProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
            AssociatedObject.MouseLeftButtonDown += AssociatedObjectOnMouseDown;
            AssociatedObject.MouseLeftButtonUp += AssociatedObjectOnMouseLeftButtonUp;
            AssociatedObject.MouseRightButtonDown += AssociatedObjectOnMouseRightButtonDown;
            AssociatedObject.MouseRightButtonUp += AssociatedObjectOnMouseRightButtonUp;
        }
        
        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= AssociatedObjectOnMouseMove;
            AssociatedObject.MouseLeftButtonDown -= AssociatedObjectOnMouseDown;
            AssociatedObject.MouseLeftButtonUp -= AssociatedObjectOnMouseLeftButtonUp;
            AssociatedObject.MouseRightButtonDown -= AssociatedObjectOnMouseRightButtonDown;
            AssociatedObject.MouseRightButtonUp -= AssociatedObjectOnMouseRightButtonUp;
        }

        private void AssociatedObjectOnMouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (RightMouseUpCommand != null)
            {
                if (CommandParameter != null)
                {
                    RightMouseUpCommand.Execute(CommandParameter);
                }
                else
                {
                    var point = e.GetPosition(AssociatedObject);
                    RightMouseUpCommand.Execute(new Point(point.X, point.Y));
                }

                //e.Handled = true;
            }
        }

        private void AssociatedObjectOnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (RightMouseDownCommand != null)
            {
                if (CommandParameter != null)
                {
                    RightMouseDownCommand.Execute(CommandParameter);
                }
                else
                {
                    var point = e.GetPosition(AssociatedObject);
                    RightMouseDownCommand.Execute(new Point(point.X, point.Y));
                }

                //e.Handled = true;
            }
        }

        private void AssociatedObjectOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (LeftMouseUpCommand != null)
            {
                if (CommandParameter != null)
                {
                    LeftMouseUpCommand.Execute(CommandParameter);
                }
                else
                {
                    var point = e.GetPosition(AssociatedObject);
                    LeftMouseUpCommand.Execute(new Point(point.X, point.Y));
                }

                //e.Handled = true;
            }
        }

        private void AssociatedObjectOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (LeftMouseDownCommand != null)
            {
                if (CommandParameter != null)
                {
                    LeftMouseDownCommand.Execute(CommandParameter);
                }
                else
                {
                    var point = e.GetPosition(AssociatedObject);
                    LeftMouseDownCommand.Execute(new Point(point.X, point.Y));
                }

                //e.Handled = true;
            }
        }

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            if (MouseMoveCommand != null)
            {
                var pos = mouseEventArgs.GetPosition(AssociatedObject);
                MouseMoveCommand.Execute(new Point(pos.X, pos.Y));
            }
        }
    }
}
