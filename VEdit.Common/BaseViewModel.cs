using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VEdit.Common
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(property, value))
            {
                property = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class BaseViewModel : ObservableObject
    {
        public BaseViewModel()
        {

        }

        public IServiceProvider ServiceProvider { get; }

        public BaseViewModel(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            Actions = new NamedActions();
        }

        public NamedActions Actions { get; }
    }
}
