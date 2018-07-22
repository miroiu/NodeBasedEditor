using VEdit.Common;
using VEdit.Editor;

namespace VEdit.UI
{
    public class BreakpointWindow : BaseViewModel
    {
        public IBreakpointManager Breakpoint { get; }

        private bool _hasBreakpoints;
        public bool HasBreakpoints
        {
            get => _hasBreakpoints;
            set => SetProperty(ref _hasBreakpoints, value);
        }

        public BreakpointWindow(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Breakpoint = serviceProvider.Get<IBreakpointManager>();
            Breakpoint.Breakpoints.CollectionChanged += OnBreakpointsChanged;

            var cmdProvider = serviceProvider.Get<ICommandProvider>();
            ClearCommand = cmdProvider.Create(async () =>
            {
                var dialog = serviceProvider.Get<IDialogManager>();
                var result = await dialog.ShowMessageAsync("Delete breakpoints", "Are you sure you want to delete all breakpoints?", DialogStyle.Affirmative);
                if (result == DialogResult.Affirmative)
                {
                    Breakpoint.DeleteAll();
                }
            });

            ClearBreakpointCommand = cmdProvider.Create<Breakpoint>(brk =>
            {
                Breakpoint.Delete(brk);
            });
        }

        private void OnBreakpointsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (Breakpoint.Breakpoints.Count == 0)
            {
                HasBreakpoints = false;
            }
            else
            {
                HasBreakpoints = true;
            }
        }

        public ICommand ClearCommand { get; }
        public ICommand ClearBreakpointCommand { get; }
    }
}
