using System;

namespace VEdit.Editor
{
    public class GraphNodeEntry : NodeEntry
    {
        public GraphNodeEntry(Guid templateId, Guid projectId, Guid fileId, Guid linkedGraphId) : base(typeof(GraphNode), templateId)
        {
            Category = "Project";
            ProjectId = projectId;
            FileId = fileId;
            LinkedGraphId = linkedGraphId;
        }

        public Guid LinkedGraphId { get; }

        // Used to tell which project owns this node
        public Guid ProjectId { get; }
        public Guid FileId { get; }
    }
}
