using System;
using System.Threading.Tasks;

namespace VEdit.Common
{
    // TODO: Shared commands
    public interface ICommandProvider
    {
        ICommand Create(Action execute, Func<bool> canExecute = null);
        ICommand CreateAsync(Func<Task> execute, Func<bool> canExecute = null);
        ICommand<T> Create<T>(Action<T> execute, Func<T, bool> canExecute = null);
        ICommand<T> CreateAsync<T>(Func<T, Task> execute, Func<T, bool> canExecute = null);
    }

    public class CommandProvider : ICommandProvider
    {
        public ICommand Create(Action execute, Func<bool> canExecute = null) => new RelayCommand(execute, canExecute);
        public ICommand<T> Create<T>(Action<T> execute, Func<T, bool> canExecute = null) => new RelayCommand<T>(execute, canExecute);
        public ICommand CreateAsync(Func<Task> execute, Func<bool> canExecute = null) => new AsyncRelayCommand(execute, canExecute);
        public ICommand<T> CreateAsync<T>(Func<T, Task> execute, Func<T, bool> canExecute = null) => new AsyncRelayCommand<T>(execute, canExecute);
    }
}
