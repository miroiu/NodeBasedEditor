using System.Windows;
using VEdit.Editor;

namespace VEdit
{
    public class ClipboardManager : IClipboardManager
    {
        public void SetData(string format, object data)
        {
            Clipboard.SetData(format, data);
        }

        public T GetData<T>(string format)
        {
            if (Clipboard.ContainsData(format))
            {
                var obj = Clipboard.GetData(format);
                if (obj is T)
                {
                    return (T)obj;
                }
            }
            return default(T);
        }
    }
}
