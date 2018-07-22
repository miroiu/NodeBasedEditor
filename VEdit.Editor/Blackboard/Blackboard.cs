using System;
using System.Collections.ObjectModel;
using System.Linq;
using VEdit.Common;

namespace VEdit.Editor
{
    public abstract class Blackboard : BaseViewModel, IBlackboard, ISaveLoad
    {
        public ReadOnlyObservableCollection<IElement> Elements { get; }
        public ISelectionService<IElement> SelectionService { get; }

        private ObservableCollection<IElement> _elements;

        public Blackboard(Common.IServiceProvider serviceProvider) : base(serviceProvider)
        {
            SelectionService = serviceProvider.Get<ISelectionService<IElement>>();
            var cmdProvider = serviceProvider.Get<ICommandProvider>();

            DeleteSelectionCommand = cmdProvider.Create(DeleteSelection, () => SelectionService.Selection.Any());

            _elements = new ObservableCollection<IElement>();
            Elements = new ReadOnlyObservableCollection<IElement>(_elements);
        }

        public ICommand DeleteSelectionCommand { get; }

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

        private double _heigth;
        public double Height
        {
            get => _heigth;
            set => SetProperty(ref _heigth, value);
        }

        private double _x;
        public double X
        {
            get => _x;
            set
            {
                var newX = value - _x;
                if (SetProperty(ref _x, value))
                {
                    foreach(var element in Elements)
                    {
                        element.Drag(newX, 0);
                    }
                }
            }
        }

        private double _y;

        public event Action Loaded;

        public double Y
        {
            get => _y;
            set
            {
                var newY = value - _y;
                if (SetProperty(ref _y, value))
                {
                    foreach (var element in Elements)
                    {
                        element.Drag(0, newY);
                    }
                }
            }
        }

        public void AddElement(IElement element)
        {
            _elements.Add(element);
        }

        public void RemoveElement(IElement element)
        {
            _elements.Remove(element);
        }

        internal abstract void DeleteSelection();

        public virtual void Save(IArchive archive)
        {
            archive.Write(nameof(Name), Name);
            archive.Write(nameof(Description), Description);
        }

        public virtual void Load(IArchive archive)
        {
            Name = archive.Read<string>(nameof(Name));
            Description = archive.Read<string>(nameof(Description));
            OnLoaded();
        }

        private void OnLoaded()
        {
            Loaded?.Invoke();
        }
    }
}
