namespace VEdit.Core
{
    public enum SocketType
    {
        Exec,
        Data,
        Custom
    }

    public enum SocketFlow
    {
        Input,
        Output
    }

    public class SocketDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SocketType Type { get; set; }
        public SocketFlow Flow { get; set; }
    }
}
