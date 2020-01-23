using System.Collections.Generic;

namespace VEdit.Core
{
    public class NodeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public NodeTemplateDto Template { get; set; }
        public IList<SocketDto> Sockets { get; set; } = new List<SocketDto>();
    }
}
