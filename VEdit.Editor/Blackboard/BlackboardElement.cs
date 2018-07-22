using System;
using VEdit.Common;

namespace VEdit.Editor
{
    public abstract class BlackboardElement : BaseViewModel, IElement, ISaveLoad
    {
        public IBlackboard Parent { get; }

        private string _name;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
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
        public virtual double X
        {
            get => _x;
            set => SetProperty(ref _x, value);
        }

        private double _y;
        public virtual double Y
        {
            get => _y;
            set => SetProperty(ref _y, value);
        }

        private bool _isSelected;
        public virtual bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        private string _description;
        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public BlackboardElement(IBlackboard blackboard) : base(blackboard.ServiceProvider)
        {
            Parent = blackboard;
        }

        public virtual void Drag(double x, double y)
        {
            X += x;
            Y += y;

            OnDragged();
        }

        public event Action Dragged;
        public event Action Loaded;

        protected void OnDragged() => Dragged?.Invoke();

        public virtual void Save(IArchive archive)
        {
            archive.Write(nameof(X), X);
            archive.Write(nameof(Y), Y);

            archive.Write(nameof(Width), Width);
            archive.Write(nameof(Height), Height);

            archive.Write(nameof(Name), Name);
            archive.Write(nameof(Description), Description);
        }

        public virtual void Load(IArchive archive)
        {
            X = archive.Read<double>(nameof(X));
            Y = archive.Read<double>(nameof(Y));

            Width = archive.Read<double>(nameof(Width));
            Height = archive.Read<double>(nameof(Height));

            Name = archive.Read<string>(nameof(Name));
            Description = archive.Read<string>(nameof(Description));
        }

        protected void OnLoaded()
        {
            Loaded?.Invoke();
        }
    }
}
