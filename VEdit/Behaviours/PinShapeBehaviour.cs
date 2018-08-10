using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interactivity;
using System.Windows.Threading;
using VEdit.Controls;
using VEdit.Editor;

namespace VEdit
{
    public class PinShapeBehaviour : Behavior<FrameworkElement>
    {
        private NodeView _parent;
        private Point _relativeLocation;
        private IBlackboardElement _element;

        public double X
        {
            get => (double)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X),
            typeof(double), typeof(PinShapeBehaviour), new PropertyMetadata(default(double)));

        public double Y
        {
            get => (double)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y),
            typeof(double), typeof(PinShapeBehaviour), new PropertyMetadata(default(double)));

        protected async override void OnAttached()
        {
            _parent = AssociatedObject.FindParentOfType<NodeView>();

            _element = _parent.DataContext as IBlackboardElement;
            _element.PropertyChanged += ParentPositionChanged;
            AssociatedObject.LayoutUpdated += OnLayoutUpdated;

            await Dispatcher.BeginInvoke(DispatcherPriority.Loaded, new Action(() =>
            {
                _relativeLocation = AssociatedObject.GetCenterLocationInsideParent(_parent);
                CalculateRelativePosition();
            }));
        }

        protected override void OnDetaching()
        {
            _element.PropertyChanged -= ParentPositionChanged;
            AssociatedObject.LayoutUpdated -= OnLayoutUpdated;
        }

        private void OnLayoutUpdated(object sender, EventArgs e)
        {
            var newPos = AssociatedObject.GetCenterLocationInsideParent(_parent);
            if (newPos != _relativeLocation)
            {
                _relativeLocation = newPos;
                CalculateRelativePosition();
            }
        }

        #region Get Position

        private void ParentPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IBlackboardElement.X) || e.PropertyName == nameof(IBlackboardElement.Y))
            {
                CalculateRelativePosition();
            }
        }

        private void CalculateRelativePosition()
        {
            X = _relativeLocation.X + _element.X;
            Y = _relativeLocation.Y + _element.Y;
        }

        #endregion
    }
}
