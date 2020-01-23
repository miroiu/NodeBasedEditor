namespace VEdit.Data
{
    public class Socket
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SocketType Type { get; set; }
        public DataFlow Flow { get; set; }
        public string CustomData { get; set; }
    }
}
