using System.Collections.Generic;

namespace VEdit.Data
{
    public class Node
    {
        public int Id { get; set; }
        public int TemplateId { get; set; }
        public string NameOverride { get; set; }
        public IList<int> Sockets { get; set; } = new List<int>();
    }
}
