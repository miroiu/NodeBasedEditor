using System.Collections.ObjectModel;

namespace VEdit.UI
{
    public class ProjectDirectory : ProjectEntry
    {
        public ObservableCollection<ProjectEntry> Children { get; }
        private bool _isExpanded;

        public ProjectDirectory(Project project, ProjectDirectory root, string name) : base(project, root, name)
        {
            Children = new ObservableCollection<ProjectEntry>();
        }

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public override string RelativePath => Root == null ? string.Empty : base.RelativePath;

        public override void Delete() => Root?.Children.Remove(this);
    }
}
