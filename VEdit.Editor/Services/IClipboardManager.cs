namespace VEdit.Editor
{
    public interface IClipboardManager
    {
        void SetData(string format, object data);
        T GetData<T>(string format);
    }
}
