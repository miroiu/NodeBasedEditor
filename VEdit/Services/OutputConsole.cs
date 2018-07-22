using System.Windows;
using System.Windows.Threading;
using VEdit.Common;
using VEdit.UI;

namespace VEdit
{
    public class OutputConsole : OutputManager
    {
        public OutputConsole(IServiceProvider provider) : base(provider)
        {
            Application.Current.DispatcherUnhandledException += OnUnhandledDispatcherException;
        }

        private void OnUnhandledDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            HandleException(args.Exception);
            args.Handled = true;
        }

        public override void Write(string output, ICommand action, OutputType type = OutputType.Message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                base.Write(output, action, type);
            });
        }
    }
}
