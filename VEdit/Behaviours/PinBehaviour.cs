using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using VEdit.Controls;
using VEdit.Editor;
using VEdit.EditorPin;

namespace VEdit
{
    public class PinBehaviour : Behavior<PinView>
    {
        public UIState<PinBehaviour> State { get; set; }

        public Pin Pin
        {
            get => (Pin)GetValue(PinProperty);
            set => SetValue(PinProperty, value);
        }

        public static readonly DependencyProperty PinProperty = DependencyProperty.Register(nameof(Pin),
            typeof(Pin), typeof(PinBehaviour), new PropertyMetadata(null));

        private Point GetRelativePosition(MouseEventArgs args) => args.GetPosition(AssociatedObject);
        private BlackboardBehaviour _blackboardBehaviour;

        protected override void OnAttached()
        {
            var blackboardView = AssociatedObject.FindParentOfType<BlackboardView>();

            var behaviors = Interaction.GetBehaviors(blackboardView);
            _blackboardBehaviour = behaviors[0] as BlackboardBehaviour;

            State = new DefaultState(this, _blackboardBehaviour);

            AssociatedObject.MouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;

            AssociatedObject.MouseEnter += OnMouseEnter;
            AssociatedObject.MouseLeave += OnMouseLeave;
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            if (_blackboardBehaviour.State is Blackboard.DrawTempLinkState temp)
            {
                var input = Pin.IsInput ? Pin : temp.Pin;
                var output = temp.Pin.IsInput ? Pin : temp.Pin;

                if (Pin.Graph.ConnectionHasErrors(input, output, out string error))
                {
                    AssociatedObject.Cursor = Cursors.No;
                    Pin.Error = error;
                }
            }
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            AssociatedObject.Cursor = Cursors.Arrow;
            Pin.Error = null;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var position = GetRelativePosition(e);
            State.OnLeftMouseButtonUp(position);
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = GetRelativePosition(e);
            State.OnLeftMouseButtonDown(position);

            e.Handled = true;
        }
    }
}
