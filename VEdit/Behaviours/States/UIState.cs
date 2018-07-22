namespace VEdit
{
    public abstract class UIState<T>
    {
        public T Behaviour { get; }

        public UIState(T element) => Behaviour = element;

        public virtual void OnMouseMove(System.Windows.Point position)
        {
        }

        public virtual void OnLeftMouseButtonDown(System.Windows.Point position)
        {
        }

        public virtual void OnLeftMouseButtonUp(System.Windows.Point position)
        {
        }

        public virtual void OnRightMouseButtonDown(System.Windows.Point position)
        {
        }

        public virtual void OnRightMouseButtonUp(System.Windows.Point position)
        {
        }
    }

    public abstract class NodeState : UIState<NodeBehaviour>
    {
        public NodeState(NodeBehaviour element) : base(element) { }
    }

    public abstract class BlackboardState : UIState<BlackboardBehaviour>
    {
        public BlackboardState(BlackboardBehaviour blackboard) : base(blackboard) { }
    }

    public abstract class PinState : UIState<PinBehaviour>
    {
        public PinState(PinBehaviour element) : base(element) { }
    }

    public abstract class ElementState : UIState<ElementBehaviour>
    {
        public ElementState(ElementBehaviour element) : base(element) { }
    }
}
