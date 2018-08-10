using System.Windows;
using VEdit.Editor;

namespace VEdit.Element
{
    public class DragState : ElementState
    {
        private Point _clickOrigin;
        private readonly ISelectionService<BlackboardElement> _selection;

        public DragState(ElementBehaviour element) : base(element)
        {
            _selection = element.Element.Parent.SelectionService;
        }

        public override void OnLeftMouseButtonDown(Point position)
        {
            _clickOrigin = position;

            foreach (var selected in _selection.Selection)
            {
                selected.StartDrag(selected.X, selected.Y);
            }
        }

        public override void OnLeftMouseButtonUp(Point position)
        {
            foreach (var selected in _selection.Selection)
            {
                selected.EndDrag(selected.X, selected.Y);
            }

            Behaviour.State = new DefaultState(Behaviour);
            Behaviour.State.OnLeftMouseButtonUp(position);
        }

        public override void OnMouseMove(Point position)
        {
            if (Behaviour.Element.IsSelected)
            {
                var delta = position - _clickOrigin;

                if (delta.Length > 0)
                {
                    foreach (var selected in _selection.Selection)
                    {
                        selected.Drag(delta.X, delta.Y);
                    }
                    _clickOrigin = position;
                }
            }
        }
    }
}
