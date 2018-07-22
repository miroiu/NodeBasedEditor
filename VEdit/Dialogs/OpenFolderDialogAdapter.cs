using System.IO;
using System.Windows.Forms;
using VEdit.UI;

namespace VEdit
{
    public class OpenFolderDialogAdapter : IOpenFolderDialog
    {
        private FolderBrowserDialog _dialog;

        public OpenFolderDialogAdapter()
        {
            _dialog = new FolderBrowserDialog()
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

        public bool Show() => _dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK;
    }
}
