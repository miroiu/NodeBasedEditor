using System.Windows;
using System.Windows.Interactivity;

namespace VEdit
{
    public class StopEventBehaviour : Behavior<FrameworkElement>
    {
        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.MouseDown += OnMouseDown;
        }

        private void OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            AssociatedObject.MouseDown -= OnMouseDown;
        }
    }
}
