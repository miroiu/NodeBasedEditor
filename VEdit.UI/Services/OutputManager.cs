using System;
using System.Threading.Tasks;
using VEdit.Common;

namespace VEdit.UI
{
    public abstract class OutputManager : IOutputManager
    {
        public ILogger Logger { get; }

        public OutputLogs Logs { get; }

        public OutputManager(Common.IServiceProvider provider)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;

            Logger = provider.Get<ILogger>();
            Logs = new OutputLogs();
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs args)
        {
            HandleException(args.Exception.Flatten().InnerException);
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            HandleException((Exception)args.ExceptionObject);
        }
        
        protected void HandleException(Exception ex)
        {
            if (ex is OperationCanceledException)
            {
                return;
            }
            Write("Oops.. something went wrong.", OutputType.Error);
            Logger.LogError(ex.ToString());
        }

        public virtual void Write(string output, ICommand action, OutputType type = OutputType.Message)
        {
            Logs.Add(output, action, type);
        }

        public void Write(string output, OutputType type = OutputType.Message)
        {
            Write(output, null, type);
        }

        public void Clear()
        {
            Logs.Clear();
        }

        public void Write(string output)
        {
            Write(output, null, OutputType.Message);
        }
    }
}
