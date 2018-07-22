using VEdit.Common;

namespace VEdit.UI
{
    public class OutputWindow : BaseViewModel
    {
        public IOutputManager Output { get; }

        public OutputWindow(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Output = serviceProvider.Get<IOutputManager>();
            Output.Logs.CollectionChanged += OnLogsChanged;

            var cmd = serviceProvider.Get<ICommandProvider>();
            ClearCommand = cmd.Create(() =>
            {
                Output.Clear();
            });
        }

        private void OnLogsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(Output.Logs.Count == 0)
            {
                HasLogs = false;
            }
            else
            {
                HasLogs = true;
            }
        }

        private bool _hasLogs;
        public bool HasLogs
        {
            get => _hasLogs;
            set => SetProperty(ref _hasLogs, value);
        }

        public ICommand ClearCommand { get; }
    }
}
