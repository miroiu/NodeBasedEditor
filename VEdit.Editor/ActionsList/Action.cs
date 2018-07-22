using System;

namespace VEdit.Editor.ActionsList
{
    public class Action : ListEntry
    {
        public Guid TemplateId { get; }

        public Action(string name, Guid templateId) : base(name)
        {
            TemplateId = templateId;
        }
    }
}
