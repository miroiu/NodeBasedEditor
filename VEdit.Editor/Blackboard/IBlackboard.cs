using System.Collections.ObjectModel;

namespace VEdit.Editor
{
    public enum ZoomDirection
    {
        In,
        Out
    }

    public interface IBlackboard
    {
        ISelectionService<IBlackboardElement> SelectionService { get; }
        ReadOnlyObservableCollection<IBlackboardElement> Elements { get; }

        // Centers the view at the element's coordinates
        void Focus(IBlackboardElement element);
        void Pan(double deltaX, double deltaY);
        void Zoom(ZoomDirection direction, double centerX = 0, double centerY = 0);
        
        double ZoomFactor { get; }

        double Width { get; set; }
        double Height { get; set; }

        void AddElement(IBlackboardElement element);
        void RemoveElement(IBlackboardElement element);
    }
}
