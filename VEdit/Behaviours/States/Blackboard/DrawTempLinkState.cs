using System.Windows;
using VEdit.Editor;

namespace VEdit.Blackboard
{
    public class DrawTempLinkState : BlackboardState
    {
        private TempLink _link;

        public Pin Pin { get; }
        //private bool _shouldDelete;
        private bool _isDone;

        public DrawTempLinkState(BlackboardBehaviour blackboard, Pin pin) : base(blackboard)
        {
            Pin = pin;
            _link = new TempLink(blackboard.Blackboard, pin.Color, pin.X, pin.Y);
            blackboard.Blackboard.AddElement(_link);
        }

        public override void OnMouseMove(Point position)
        {
            if (!_isDone)
            {
                _link.EndX = position.X;
                _link.EndY = position.Y;
            }
        }

        public override void OnLeftMouseButtonUp(Point position)
        {
            //if (_shouldDelete)
            {
                Behaviour.Blackboard.RemoveElement(_link);
                Behaviour.State = new DefaultState(Behaviour);
            }
            //else
            {
                _isDone = true;
                // TODO: Spawn actions menu and filter it by pin type
            }
        }

        public void Done()
        {
            //_shouldDelete = true;
            _isDone = true;
        }
    }
}
