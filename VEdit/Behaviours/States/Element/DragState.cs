using System.Windows;

namespace VEdit.Element
{
    public class DragState : ElementState
    {
        private Point _clickOrigin;

        public DragState(ElementBehaviour element) : base(element)
        {

        }

        public override void OnLeftMouseButtonDown(Point position)
        {
            _clickOrigin = position;
        }

        public override void OnLeftMouseButtonUp(Point position)
        {
            Behaviour.State = new DefaultState(Behaviour);
            Behaviour.State.OnLeftMouseButtonUp(position);
        }

        public override void OnMouseMove(Point position)
        {
            var element = Behaviour.Element;
            if (element.IsSelected)
            {
                var delta = position - _clickOrigin;

                if (delta.Length > 0)
                {
                    var selection = element.Parent.SelectionService;
                    foreach (var selected in selection.Selection)
                    {
                        selected.Drag(delta.X, delta.Y);
                    }
                    _clickOrigin = position;
                }
            }
        }
    }
}
