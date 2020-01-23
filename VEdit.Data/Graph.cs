using System.Collections.Generic;

namespace VEdit.Data
{
    public class Graph
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<int> Nodes { get; set; } = new List<int>();
        public IList<int> Connections { get; set; } = new List<int>();
    }
}
