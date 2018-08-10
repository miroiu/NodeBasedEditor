using System.Collections.Generic;

namespace VEdit.Editor
{
    public interface ISelectionService<T> where T : ISelectable
    {
        void Select(T selectable);
        void Unselect(T selectable);
        void UnselectAll();

        IEnumerable<T> Selection { get; }
    }

    public static class SelectionServiceExtensions
    {
        public static void SelectRange<T>(this ISelectionService<T> service, IEnumerable<ISelectable> selectables) where T: ISelectable
        {
            foreach(var selectable in selectables)
            {
                service.Select((T)selectable);
            }
        }

        public static IEnumerable<T> GetElementsInBounds<T>(this IEnumerable<T> elements, double x, double y, double width, double height) where T: BlackboardElement
        {
            foreach (BlackboardElement elem in elements)
            {
                if (elem.X < x + width &&
                    elem.X + elem.Width > x &&
                    elem.Y < y + height &&
                    elem.Y + elem.Height > y)
                {
                    yield return (T)elem;
                }
            }
        }
    }

    public class ElementSelectionService : ISelectionService<BlackboardElement>
    {
        private List<BlackboardElement> _selected = new List<BlackboardElement>();

        public IEnumerable<BlackboardElement> Selection => _selected;

        public void Select(BlackboardElement selectable)
        {
            selectable.IsSelected = true;

            if (!_selected.Contains(selectable))
            {
                _selected.Add(selectable);
            }
        }

        public void Unselect(BlackboardElement selectable)
        {
            if (_selected.Contains(selectable))
            {
                selectable.IsSelected = false;
                _selected.Remove(selectable);
            }
        }

        public void UnselectAll()
        {
            _selected.ForEach(s => s.IsSelected = false);
            _selected.Clear();
        }
    }
}
