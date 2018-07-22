namespace VEdit.Editor
{
    public class TempLink : BlackboardElement
    {
        public TempLink(IBlackboard blackboard, string color, double x = 0, double y = 0) : base(blackboard)
        {
            Color = color;
            StartX = x;
            StartY = y;
            EndX = x;
            EndY = y;
        }

        public string Color { get; }

        private double _startX;
        public double StartX
        {
            get => _startX;
            set => SetProperty(ref _startX, value);
        }

        private double _startY;
        public double StartY
        {
            get => _startY;
            set => SetProperty(ref _startY, value);
        }

        private double _endX;
        public double EndX
        {
            get => _endX;
            set => SetProperty(ref _endX, value);
        }

        private double _endY;
        public double EndY
        {
            get => _endY;
            set => SetProperty(ref _endY, value);
        }
    }
}
