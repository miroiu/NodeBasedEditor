namespace VEdit.Common
{
    public enum OutputType
    {
        Info,
        Warning,
        Error,
        Message
    }

    public interface IOutputManager
    {
        OutputLogs Logs { get; }

        void Write(string output, ICommand action, OutputType type = OutputType.Message);
        void Write(string output, OutputType type = OutputType.Message);
        void Write(string output);

        void Clear();
    }
}
