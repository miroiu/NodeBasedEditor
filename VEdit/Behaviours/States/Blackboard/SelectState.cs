using System.Linq;
using System.Windows;
using VEdit.Editor;

namespace VEdit.Blackboard
{
    public class SelectState : BlackboardState
    {
        private SelectionRectangle _selectionRectangle;
        private Point _clickOrigin;

        public SelectState(BlackboardBehaviour blackboard) : base(blackboard)
        {
            _selectionRectangle = new SelectionRectangle((Editor.Blackboard)blackboard.Blackboard);
        }

        public override void OnLeftMouseButtonDown(Point position)
        {
            _clickOrigin = position;

            var blackboard = Behaviour.Blackboard;
            blackboard.SelectionService.UnselectAll();
            blackboard.RemoveElement(_selectionRectangle);

            _selectionRectangle.X = _clickOrigin.X;
            _selectionRectangle.Y = _clickOrigin.Y;

            _selectionRectangle.Width = 0;
            _selectionRectangle.Height = 0;

            blackboard.AddElement(_selectionRectangle);
        }

        public override void OnLeftMouseButtonUp(Point position)
        {
            var blackboard = Behaviour.Blackboard;
            blackboard.RemoveElement(_selectionRectangle);

            var selection = blackboard.Elements.GetElementsInBounds(_selectionRectangle.X, _selectionRectangle.Y, _selectionRectangle.Width, _selectionRectangle.Height)
                .Where(e => !(e is Comment));

            blackboard.SelectionService.SelectRange(selection);

            Behaviour.State = new DefaultState(Behaviour);
        }

        public override void OnMouseMove(Point position)
        {
            var deltaX = position.X - _clickOrigin.X;
            var deltaY = position.Y - _clickOrigin.Y;

            if (deltaX < 0)
            {
                _selectionRectangle.X = position.X;
                _selectionRectangle.Width = -deltaX;
            }
            else
            {
                _selectionRectangle.Width = deltaX;
            }

            if (deltaY < 0)
            {
                _selectionRectangle.Y = position.Y;
                _selectionRectangle.Height = -deltaY;
            }
            else
            {
                _selectionRectangle.Height = deltaY;
            }
        }
    }
}
