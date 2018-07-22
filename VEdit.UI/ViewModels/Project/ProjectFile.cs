using System;
using VEdit.Common;

namespace VEdit.UI
{
    public class ProjectFile : ProjectEntry
    {
        public FileType Type { get; }
        public Guid Id { get; internal set; }

        public string Extension { get; }

        public ProjectFile(Project project, ProjectDirectory root, string name) : base(project, root, name)
        {
            Id = Guid.NewGuid();
            Type = name.GetFileType();
            Extension = System.IO.Path.GetExtension(name);
        }

        public ProjectFile(string name) : this(null, null, name)
        {

        }

        public override string FullName => $"{Name}{Extension}";

        public override void Delete() => Root?.Children.Remove(this);

        public FileEditor Editor { get; internal set; }

        public bool IsOpen => Editor != null;
    }
}
