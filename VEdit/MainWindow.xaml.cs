using MahApps.Metro.Controls;
using Ninject;
using System;
using System.ComponentModel;
using System.Windows;
using VEdit.Common;
using VEdit.UI;

namespace VEdit
{
    public partial class MainWindow : MetroWindow
    {
        private EditorViewModel _viewModel;
        private ILogger _logger;
        private bool _waitForSave = true;

        public MainWindow()
        {
            Loaded += OnLoaded;

            IKernel serviceProvider = new StandardKernel();
            _logger = serviceProvider.Get<ILogger>();
            _viewModel = serviceProvider.Get<EditorViewModel>();

            DataContext = _viewModel;
            InitializeComponent();

            AppDomain.CurrentDomain.FirstChanceException += (s, e)
                => Application.Current.Dispatcher.BeginInvoke(new Action(() => MessageBox.Show($"Object {s} {Environment.NewLine}Error Occurred {Environment.NewLine} {e.Exception.Message} {Environment.NewLine} {e.Exception.StackTrace}")), null);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;

            _viewModel.Initialize();
        }

        protected override async void OnClosing(CancelEventArgs e)
        {
            e.Cancel = _waitForSave;

            if (_waitForSave)
            {
                try
                {
                    await _viewModel.Shutdown();
                }
                catch(Exception ex)
                {
                    _logger.Log(ex.ToString());
                }
                _waitForSave = false;
            }

            Application.Current.Shutdown();
        }
    }
}
