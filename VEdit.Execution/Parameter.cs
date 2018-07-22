namespace VEdit.Execution
{
    public interface IParameter
    {
        IParameter Dependency { get; set; }
        object Value { get; set; }
    }

    public class Parameter : IParameter
    {
        public IParameter Dependency { get; set; }

        private object _value;
        public object Value
        {
            get
            {
                if (Dependency != null)
                {
                    return Dependency.Value;
                }
                return _value;
            }
            set => _value = value;
        }

        public Parameter(object defaultValue, IParameter dependency = null)
        {
            Dependency = dependency;
            Value = defaultValue;
        }
    }

    public class Parameter<T> : Parameter
    {
        public Parameter(T defaultValue = default(T), IParameter dependency = null) : base(defaultValue, dependency)
        {
        }

        public new T Value
        {
            get => (T)base.Value;
            set => base.Value = value;
        }
    }
}
