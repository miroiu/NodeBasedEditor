using System.Windows;
using VEdit.Editor;

namespace VEdit.Blackboard
{
    public class ShowNodeListState : BlackboardState
    {
        private IActionList _nodeList;

        public ShowNodeListState(BlackboardBehaviour blackboard) : base(blackboard)
        {
            _nodeList = ((Graph)blackboard.Blackboard).NodeList;
            _nodeList.CloseEvent += Close;
        }

        public override void OnRightMouseButtonUp(Point position)
        {
            _nodeList.X = position.X;
            _nodeList.Y = position.Y;
            Behaviour.Blackboard.AddElement(_nodeList);
        }

        public override void OnRightMouseButtonDown(Point position)
        {
            Close();
            Behaviour.State.OnRightMouseButtonDown(position);
        }

        public override void OnLeftMouseButtonDown(Point position)
        {
            Close();
            Behaviour.State.OnLeftMouseButtonDown(position);
        }

        private void Close()
        {
            Behaviour.Blackboard.RemoveElement(_nodeList);
            Behaviour.State = new DefaultState(Behaviour);

            _nodeList.CloseEvent -= Close;
        }
    }
}
