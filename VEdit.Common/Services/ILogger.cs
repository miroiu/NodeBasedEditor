using System;

namespace VEdit.Common
{
    public enum LogType
    {
        Info,
        Warning,
        Error
    }

    public interface ILogger
    {
        void Log(LogType type, string message);
    }

    public static class LoggerExtensions
    {
        public static void LogWarning(this ILogger logger, string message)
        {
            logger.Log(LogType.Warning, message);
        }

        public static void LogError(this ILogger logger, string message)
        {
            logger.Log(LogType.Error, message);
        }

        public static void Log(this ILogger logger, string message)
        {
            logger.Log(LogType.Info, message);
        }
    }

    public class Logger : ILogger
    {
        private string _filePath;
        private IFileIO _io;

        public Logger(string filePath, IFileIO io)
        {
            _filePath = filePath;
            _io = io;
        }

        public void Log(LogType type, string message)
        {
            _io.Append(_filePath, $"{(DateTime.Now)} [{type}]: {message}");
        }
    }
}
