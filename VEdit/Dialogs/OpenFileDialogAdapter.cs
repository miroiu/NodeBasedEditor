using Microsoft.Win32;
using System.Collections.Generic;
using VEdit.UI;

namespace VEdit
{
    public class OpenFileDialogAdapter : IOpenFileDialog
    {
        private OpenFileDialog _dialog = new OpenFileDialog();

        public bool AllowMultiple
        {
            get => _dialog.Multiselect;
            set => _dialog.Multiselect = value;
        }

        public FileDialogFilter Filter
        {
            set => _dialog.Filter = value.ToString();
        }

        public string FilePath
        {
            get => _dialog.FileName;
            set => _dialog.FileName = value;
        }

        public string FileName => _dialog.SafeFileName;

        public IEnumerable<string> FilePaths => _dialog.FileNames;
        public IEnumerable<string> FileNames => _dialog.SafeFileNames;
        
        public bool Show() => _dialog.ShowDialog() == true;
    }
}
