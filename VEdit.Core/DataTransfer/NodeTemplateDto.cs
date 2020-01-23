using System.Collections.Generic;

namespace VEdit.Core
{
    public class NodeTemplateDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IList<SocketDto> Sockets { get; set; } = new List<SocketDto>();
    }
}
