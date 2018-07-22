using System.Windows;
using VEdit.Blackboard;

namespace VEdit.EditorNode
{
    public class DefaultState : NodeState
    {
        public DefaultState(NodeBehaviour element) : base(element)
        {
        }

        public override void OnLeftMouseButtonUp(Point position)
        {
            if (Behaviour.ParentBehaviour.State is DrawTempLinkState temp)
            {
                var pin = temp.Pin;
                var node = Behaviour.Node;
                temp.Done();

                node.TryToConnectPin(pin);
            }
        }
    }
}
