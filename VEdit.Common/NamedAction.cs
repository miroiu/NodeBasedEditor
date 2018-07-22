using System;
using System.Collections.ObjectModel;

namespace VEdit.Common
{
    public class NamedAction : ICommand
    {
        private ICommand _command;

        public NamedAction(string name, ICommand command)
        {
            Name = name;
            _command = command;
        }

        public string Name { get; }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return _command.CanExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _command.Execute(parameter);
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }

    public class NamedActions : ObservableCollection<NamedAction>
    {
        public void Add(string name, ICommand command)
        {
            Add(new NamedAction(name, command));
        }

        public void RequeryCanExecute()
        {
            foreach(var action in this)
            {
                action.RaiseCanExecuteChanged();
            }
        }
    }
}
