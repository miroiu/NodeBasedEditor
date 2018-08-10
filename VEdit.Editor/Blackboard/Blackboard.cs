using System.Collections.ObjectModel;
using VEdit.Common;

namespace VEdit.Editor
{
    public enum ZoomType
    {
        In,
        Out
    }

    public abstract class Blackboard : BaseViewModel
    {
        public ReadOnlyObservableCollection<BlackboardElement> Elements { get; }
        public ISelectionService<BlackboardElement> SelectionService { get; }

        private ObservableCollection<BlackboardElement> _elements;

        public Blackboard(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            SelectionService = serviceProvider.Get<ISelectionService<BlackboardElement>>();
            var cmdProvider = serviceProvider.Get<ICommandProvider>();

            _elements = new ObservableCollection<BlackboardElement>();
            Elements = new ReadOnlyObservableCollection<BlackboardElement>(_elements);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private double _width;
        public double Width
        {
            get => _width;
            set => SetProperty(ref _width, value);
        }

        private double _height;
        public double Height
        {
            get => _height;
            set => SetProperty(ref _height, value);
        }

        private readonly double _maxZoom = 2;
        private readonly double _minZoom = 0.2;
        private readonly double _zoomStep = 0.2;

        private double _zoom = 1;
        public double ZoomFactor
        {
            get => _zoom;
            private set
            {
                if (value <= _maxZoom && value >= _minZoom)
                {
                    SetProperty(ref _zoom, value);
                }
            }
        }

        public void Pan(double deltaX, double deltaY)
        {
            foreach (var element in Elements)
            {
                element.Drag(deltaX, deltaY);
            }
        }

        public void Focus(BlackboardElement element)
        {
            Focus(element.X + element.Width / 2, element.Y + element.Height / 2);
        }

        public void Zoom(ZoomType dir, double centerX, double centerY)
        {
            ZoomFactor += dir == ZoomType.In ? _zoomStep : -_zoomStep;
        }

        private void Focus(double x, double y)
        {
            double centerX = Width / 2;
            double centerY = Height / 2;

            Pan(centerX - x, centerY - y);
        }
        
        public void AddElement(BlackboardElement element)
        {
            _elements.Add(element);
        }

        public void RemoveElement(BlackboardElement element)
        {
            _elements.Remove(element);
        }
    }
}
