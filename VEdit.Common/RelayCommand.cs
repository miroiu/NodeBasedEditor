using System;
using System.Threading.Tasks;

namespace VEdit.Common
{
    public interface ICommand : System.Windows.Input.ICommand
    {
        void RaiseCanExecuteChanged();
    }

    public interface ICommand<T> : ICommand
    {
        void Execute(T param);
        bool CanExecute(T param);
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _action;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action action, Func<bool> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
        public virtual void Execute(object parameter) => _action?.Invoke();
        public void RaiseCanExecuteChanged() =>  CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class AsyncRelayCommand : RelayCommand
    {
        private readonly Func<Task> _asyncAction;

        public AsyncRelayCommand(Func<Task> asyncAction, Func<bool> canExecute = null) : base(null, canExecute)
        {
            _asyncAction = asyncAction;
        }

        public override async void Execute(object parameter) => await _asyncAction().ConfigureAwait(false);
    }

    public class RelayCommand<T> : ICommand<T>
    {
        private readonly Action<T> _action;
        private readonly Func<T, bool> _canExecute;
        public event EventHandler CanExecuteChanged;

        public RelayCommand(Action<T> action, Func<T, bool> canExecute = null)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public virtual bool CanExecute(T param) => _canExecute == null || _canExecute(param);
        public bool CanExecute(object parameter) => CanExecute((T)parameter);

        public virtual void Execute(T param) => _action(param);
        public virtual void Execute(object parameter) => Execute((T)parameter);

        public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    public class AsyncRelayCommand<T> : RelayCommand<T>
    {
        private readonly Func<T, Task> _asyncAction;

        public AsyncRelayCommand(Func<T, Task> asyncAction, Func<T, bool> canExecute = null) : base(null, canExecute)
        {
            _asyncAction = asyncAction;
        }

        public override async void Execute(object param)
        {
            RaiseCanExecuteChanged();
            await _asyncAction((T)param);
            RaiseCanExecuteChanged();
        }
    }
}
