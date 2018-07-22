using System.Collections.ObjectModel;
using VEdit.Common;

namespace VEdit.Editor
{
    public class Breakpoint : NamedAction
    {
        public Node Node { get; }

        public Breakpoint(Node node, ICommand command) : base($"{node.FullName} -> Project: {node.Graph.ProjectId}", command)
        {
            Node = node;
        }
    }

    public class Breakpoints : ObservableCollection<Breakpoint>
    {
        public void Add(Node node, ICommand command)
        {
            Add(new Breakpoint(node, command));
        }

        public void RequeryCanExecute()
        {
            foreach (var action in this)
            {
                action.RaiseCanExecuteChanged();
            }
        }
    }
}
