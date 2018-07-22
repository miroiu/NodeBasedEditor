using System.Windows;

namespace VEdit.Blackboard
{
    public class DefaultState : BlackboardState
    {
        public DefaultState(BlackboardBehaviour blackboard) : base(blackboard)
        {
        }

        public override void OnLeftMouseButtonDown(Point position)
        {
            Behaviour.State = new SelectState(Behaviour);
            Behaviour.State.OnLeftMouseButtonDown(position);
        }

        public override void OnRightMouseButtonDown(Point position)
        {
            Behaviour.State = new PanState(Behaviour);
            Behaviour.State.OnRightMouseButtonDown(position);
        }
    }
}
