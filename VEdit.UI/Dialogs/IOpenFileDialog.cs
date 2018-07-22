using System.Collections.Generic;

namespace VEdit.UI
{
    public interface IOpenFileDialog : IDialog
    {
        bool AllowMultiple { get; set; }
        FileDialogFilter Filter { set; }
        string FilePath { get; set; }
        string FileName { get; }
        IEnumerable<string> FilePaths { get; }
        IEnumerable<string> FileNames { get; }
    }
}
