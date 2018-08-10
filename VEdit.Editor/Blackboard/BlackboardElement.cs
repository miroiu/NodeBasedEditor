using System;
using VEdit.Common;

namespace VEdit.Editor
{
    public abstract class BlackboardElement : BaseViewModel, IBlackboardElement
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

        public BlackboardElement(IBlackboard blackboard, Common.IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Parent = blackboard;
        }

        #region Drag Handlers

        public bool IsDragging { get; set; }
        public event Action Dragged;

        private readonly int _gridSize = 20;

        public virtual void Drag(double x, double y)
        {
            X += x;
            Y += y;
        }

        private double SnapToGrid(double value)
        {
            return Math.Round(value / _gridSize) * _gridSize;
        }

        public void StartDrag(double x, double y)
        {
            IsDragging = true;
        }

        public void EndDrag(double x, double y)
        {
            X = SnapToGrid(X);
            Y = SnapToGrid(Y);

            IsDragging = false;

            // OnDragged()
        }

        protected void OnDragged() => Dragged?.Invoke();

        #endregion
    }
}
