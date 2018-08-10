namespace VEdit.Editor
{
    public class SelectionRectangle : BlackboardElement
    {
        public SelectionRectangle(Blackboard blackboard) : base(blackboard, blackboard.ServiceProvider)
        {
        }

        public override bool IsSelected => false;
    }
}
