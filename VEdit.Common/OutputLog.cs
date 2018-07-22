using System;
using System.Collections.ObjectModel;

namespace VEdit.Common
{
    public class OutputLog : NamedAction
    {
        public OutputLog(string name, ICommand command, OutputType type = OutputType.Message) : base(type != OutputType.Message ? $"{DateTime.Now.ToString("HH:mm:ss")} [{type}]: {name}" : name, command)
        {
            Type = type;
        }

        public OutputType Type { get; }
    }

    public class OutputLogs : ObservableCollection<OutputLog>
    {
        public void Add(string name, ICommand command, OutputType type = OutputType.Message)
        {
            if (command == null)
            {
                command = new RelayCommand(() => { }, () => false);
            }
            Add(new OutputLog(name, command, type));
        }

        public void RequeryCanExecute()
        {
            foreach (var action in this)
            {
                action.RaiseCanExecuteChanged();
            }
        }

        public void Stack(string name, ICommand command, OutputType type)
        {
            if (command == null)
            {
                command = new RelayCommand(() => { }, () => false);
            }
            Insert(0, new OutputLog(name, command, type));
        }
    }
}
