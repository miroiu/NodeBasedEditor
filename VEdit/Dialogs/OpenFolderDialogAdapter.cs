using Ookii.Dialogs.Wpf;
using System.IO;
using VEdit.UI;

namespace VEdit
{
    public class OpenFolderDialogAdapter : IOpenFolderDialog
    {
        private readonly VistaFolderBrowserDialog _dialog;

        public OpenFolderDialogAdapter()
        {
            _dialog = new VistaFolderBrowserDialog()
            {
                ShowNewFolderButton = true
            };
        }

        public string FolderPath
        {
            get => _dialog.SelectedPath;
            set => _dialog.SelectedPath = value;
        }

        public string FolderName => Path.GetFileName(_dialog.SelectedPath);

        public bool Show() => _dialog.ShowDialog() ?? false;
    }
}
