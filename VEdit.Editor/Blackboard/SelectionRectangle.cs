namespace VEdit.Editor
{
    public class SelectionRectangle : BlackboardElement
    {
        public SelectionRectangle(IBlackboard blackboard) : base(blackboard)
        {
        }

        public override bool IsSelected => false;
    }
}
