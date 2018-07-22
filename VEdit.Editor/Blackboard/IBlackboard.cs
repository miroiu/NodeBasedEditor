using System.Collections.ObjectModel;
using VEdit.Common;

namespace VEdit.Editor
{
    public interface IBlackboard
    {
        ISelectionService<IElement> SelectionService { get; }
        ReadOnlyObservableCollection<IElement> Elements { get; }
        IServiceProvider ServiceProvider { get; }

        double X { get; set; }
        double Y { get; set; }

        double Width { get; set; }
        double Height { get; set; }

        void AddElement(IElement element);
        void RemoveElement(IElement element);

        ICommand DeleteSelectionCommand { get; }
    }
}
