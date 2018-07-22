using System;
using System.Collections.Generic;
using VEdit.Editor;

namespace VEdit.UI
{
    public class ProjectData
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<FileEntry> Files { get; } = new List<FileEntry>();
        public List<GraphNodeEntry> Nodes { get; } = new List<GraphNodeEntry>();
    }
}
