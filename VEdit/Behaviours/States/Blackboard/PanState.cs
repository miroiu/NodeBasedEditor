using VEdit.Editor;

namespace VEdit.Blackboard
{
    public class PanState : BlackboardState
    {
        private System.Windows.Point _clickOrigin;
        private System.Windows.Point _firstClick;

        public PanState(BlackboardBehaviour element) : base(element)
        {
        }

        public override void OnRightMouseButtonDown(System.Windows.Point position)
        {
            _clickOrigin = position;
            _firstClick = position;
        }

        public override void OnRightMouseButtonUp(System.Windows.Point position)
        {
            if(_firstClick == position && Behaviour.Blackboard is Graph graph)
            {
                Behaviour.State = new ShowNodeListState(Behaviour);
                Behaviour.State.OnRightMouseButtonUp(position);
            }
            else
            {
                Behaviour.State = new DefaultState(Behaviour);
            }
        }

        public override void OnMouseMove(System.Windows.Point position)
        {
            var delta = position - _clickOrigin;

            if (delta.Length > 0)
            {
                var blackboard = Behaviour.Blackboard;

                blackboard.Pan(delta.X, delta.Y);

                _clickOrigin = position;
            }
        }
    }
}
