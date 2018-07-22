using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace VEdit.Element
{
    public class DefaultState : ElementState
    {
        public DefaultState(ElementBehaviour element) : base(element)
        {
        }

        public override void OnLeftMouseButtonDown(Point position)
        {
            var element = Behaviour.Element;
            var service = element.Parent.SelectionService;

            if (!element.IsSelected)
            {
                if (service.Selection.Any() && !(Keyboard.Modifiers == ModifierKeys.Control))
                {
                    service.UnselectAll();
                }

                service.Select(element);
            }
            else if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                service.Unselect(element);
            }

            Behaviour.State = new DragState(Behaviour);
            Behaviour.State.OnLeftMouseButtonDown(position);
        }

        public override void OnRightMouseButtonUp(Point position)
        {
            var element = Behaviour.Element;
            var service = element.Parent.SelectionService;

            if (!element.IsSelected)
            {
                service.UnselectAll();
                service.Select(element);
            }
        }
    }
}
