using System.Collections.Generic;
using System.Linq;

namespace VEdit.UI
{
    public class FileDialogFilter
    {
        public FileDialogFilter(string header, params string[] extensions)
        {
            Header = header;
            Extensions = extensions;
        }

        public string Header { get; }
        public IEnumerable<string> Extensions { get; }

        public override string ToString() => $"{Header}|{string.Join(";", Extensions.Select(ext => "*." + ext))}";
    }
}
