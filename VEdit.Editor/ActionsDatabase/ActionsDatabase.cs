using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VEdit.Common;

namespace VEdit.Editor
{
    public interface IActionsDatabase
    {
        ReadOnlyObservableCollection<ActionEntry> Actions { get; }
        event Action DatabaseChanged;

        void Add(ActionEntry entry);
        void Remove(Guid entryId);

        IEnumerable<ActionEntry> FilterByText(Guid projectId, string value);
        IEnumerable<ActionEntry> ApplyFilter(Func<ActionEntry, bool> filter, Guid projectId);
        IEnumerable<ActionEntry> GetAvailable(Guid projectId);

        void NotifyChange();
    }

    public class ActionsDatabase : IActionsDatabase
    {
        public ReadOnlyObservableCollection<ActionEntry> Actions { get; }
        private ObservableCollection<ActionEntry> _actions;

        public event Action DatabaseChanged;

        public ActionsDatabase()
        {
            _actions = new ObservableCollection<ActionEntry>();
            Actions = new ReadOnlyObservableCollection<ActionEntry>(_actions);
        }

        public void Add(ActionEntry entry)
        {
            _actions.Add(entry);
            NotifyChange();
        }

        public void Remove(Guid entryId)
        {
            var action = _actions.First(a => a.TemplateId == entryId);
            _actions.Remove(action);
            NotifyChange();
        }

        public IEnumerable<ActionEntry> FilterByText(Guid projectId, string value)
        {
            return ApplyFilter(a => a.Name.ContainsIgnoreCase(value) || (a.Keywords?.ContainsIgnoreCase(value) ?? false), projectId);
        }

        public IEnumerable<ActionEntry> ApplyFilter(Func<ActionEntry, bool> filter, Guid projectId)
        {
            return _actions.Where(a => filter(a) && (a.ProjectId == projectId || a.ProjectId == Guid.Empty));
        }

        public IEnumerable<ActionEntry> GetAvailable(Guid projectId)
        {
            return _actions.Where(a => a.ProjectId == projectId || a.ProjectId == Guid.Empty);
        }

        public void NotifyChange()
        {
            DatabaseChanged?.Invoke();
        }
    }
}
