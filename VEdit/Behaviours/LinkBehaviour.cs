using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interactivity;
using VEdit.Editor;

namespace VEdit
{
    public class LinkBehaviour : Behavior<FrameworkElement>
    {
        public static DependencyProperty ElementProperty = DependencyProperty.Register(nameof(Element),
            typeof(Link), typeof(LinkBehaviour), new PropertyMetadata(null));

        public UIState<LinkBehaviour> State { get; set; }

        public Link Element
        {
            get => (Link)GetValue(ElementProperty);
            set => SetValue(ElementProperty, value);
        }

        protected override void OnAttached()
        {
            AssociatedObject.MouseRightButtonDown += MouseRightButtonDown;
        }

        private void MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseRightButtonDown -= MouseRightButtonDown;
        }
    }
}
