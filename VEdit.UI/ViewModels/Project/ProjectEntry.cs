using System;
using System.IO;
using System.Text.RegularExpressions;
using VEdit.Common;

namespace VEdit.UI
{
    public abstract class ProjectEntry : ObservableObject
    {
        private ProjectDirectory _root;

        public ProjectDirectory Root
        {
            get => _root;
            set
            {
                var prevRoot = _root;
                if(SetProperty(ref _root, value))
                {
                    prevRoot?.Children.Remove(this);
                    _root?.Children.Add(this);
                }
            }
        }

        public ProjectEntry(Project project, ProjectDirectory root, string name)
        {
            Root = root;
            Project = project;
            ProjectPath = project?.DirectoryPath;
            Name = Path.GetFileNameWithoutExtension(name);
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (SetProperty(ref _name, value))
                {
                    OnRename();
                }
            }
        }

        public string RelativeDirectoryPath => Root?.RelativePath;
        public string FullDirectoryPath => Project != null ? Path.Combine(ProjectPath, Root?.RelativePath ?? string.Empty) : null;
        public string DirectoryName => Root?.Name;

        public event Action Renamed;

        public Project Project { get; }

        public virtual string FullName => Name;
        public string ProjectPath { get; }

        public string AbsolutePath => Project != null ? Path.Combine(ProjectPath, RelativePath) : null;
        public virtual string RelativePath => Path.Combine(Root?.RelativePath ?? string.Empty, FullName);

        public abstract void Delete();

        public virtual bool Rename(string newName)
        {
            if(Regex.IsMatch(newName, @"[aA-zZ0-9]|\s"))
            {
                Name = newName;
                return true;
            }
            return false;
        }

        protected void OnRename()
        {
            Renamed?.Invoke();
        }
    }
}
