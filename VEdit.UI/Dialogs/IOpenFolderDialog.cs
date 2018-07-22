namespace VEdit.UI
{
    public interface IOpenFolderDialog : IDialog
    {
        string FolderPath { get; set; }
        string FolderName { get; }
    }
}
