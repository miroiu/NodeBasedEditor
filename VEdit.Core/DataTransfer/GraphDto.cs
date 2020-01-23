using System.Collections.Generic;

namespace VEdit.Core
{
    public class GraphDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<NodeDto> Nodes { get; set; } = new List<NodeDto>();
        public IList<SocketBindingDto> Connections { get; set; } = new List<SocketBindingDto>();
    }
}
