using System;

namespace VEdit.UI
{
    public class FileEntry
    {
        public Guid Id { get; set; }
        public string RelativePath { get; set; }
        public bool IsDirectory { get; set; }
    }
}
