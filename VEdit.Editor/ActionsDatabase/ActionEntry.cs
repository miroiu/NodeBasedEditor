using System;

namespace VEdit.Editor
{
    public class ActionEntry
    {
        public ActionEntry(Guid templateId, string name, string category = null)
        {
            TemplateId = templateId;
            Name = name;
            Category = category;
        }

        public Guid ProjectId { get; set; }

        public Guid TemplateId { get; }

        public string Name { get; }

        public string Category { get; }

        public string Keywords { get; set; }
    }
}
