using System;
using System.Collections.Generic;

namespace VEdit.Editor
{
    // Used by the database as a template when creating nodes
    public class NodeEntry
    {
        public NodeEntry(Type nodeType, Guid templateId)
        {
            NodeType = nodeType;
            TemplateId = templateId;

            Input = new List<PinData>();
            Output = new List<PinData>();
        }

        // Used to determine what type of node to create
        public Type NodeType { get; }
        // Used as a key for the database service
        public Guid TemplateId { get; }

        public string DisplayName { get; set; }
        public string CompactName { get; set; }
        public string Tooltip { get; set; }
        public string Category { get; set; }
        public string Keywords { get; set; }

        public IList<PinData> Input { get; }
        public IList<PinData> Output { get; }
    }
}
