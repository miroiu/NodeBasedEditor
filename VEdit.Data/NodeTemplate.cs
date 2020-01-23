using System.Collections.Generic;

namespace VEdit.Data
{
    public class NodeTemplate
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<int> Sockets { get; set; } = new List<int>();
    }
}
