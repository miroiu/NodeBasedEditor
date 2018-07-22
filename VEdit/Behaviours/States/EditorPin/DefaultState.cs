using System.Windows;
using VEdit.Blackboard;
using VEdit.Editor;

namespace VEdit.EditorPin
{
    public class DefaultState : PinState
    {
        private BlackboardBehaviour _blackboardBehaviour;
        private Graph _graph;

        public DefaultState(PinBehaviour element, BlackboardBehaviour behaviour) : base(element)
        {
            _blackboardBehaviour = behaviour;
            _graph = element.Pin.Graph;
        }

        public override void OnLeftMouseButtonDown(Point position)
        {
            _blackboardBehaviour.DrawTempLink(Behaviour.Pin);
        }

        public override void OnLeftMouseButtonUp(Point position)
        {
            if (_blackboardBehaviour.State is DrawTempLinkState temp)
            {
                temp.Done();
                _graph.TryToConnectPins(temp.Pin, Behaviour.Pin);
            }
        }
    }
}
