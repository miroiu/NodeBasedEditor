using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using VEdit.Controls;
using VEdit.Editor;
using VEdit.EditorNode;

namespace VEdit
{
    public class NodeBehaviour : Behavior<NodeView>
    {
        public UIState<NodeBehaviour> State { get; set; }

        public Node Node
        {
            get => (Node)GetValue(NodeProperty);
            set => SetValue(NodeProperty, value);
        }

        public static readonly DependencyProperty NodeProperty = DependencyProperty.Register(nameof(Node),
            typeof(Node), typeof(NodeBehaviour), new PropertyMetadata(null));

        public BlackboardBehaviour ParentBehaviour { get; private set; }

        protected override void OnAttached()
        {
            var blackboardView = AssociatedObject.FindParentOfType<BlackboardView>();

            var behaviors = Interaction.GetBehaviors(blackboardView);
            ParentBehaviour = behaviors[0] as BlackboardBehaviour;

            State = new DefaultState(this);

            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
        }

        private Point GetMousePositionFromMainWindow(MouseEventArgs args)
        {
            var parent = Application.Current.MainWindow;
            return args.GetPosition(parent);
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if ((e.OriginalSource as FrameworkElement).FindParentOfType<PinView>() == null)
            {
                var position = GetMousePositionFromMainWindow(e);
                State.OnLeftMouseButtonUp(position);
            }
        }
    }
}