using VEdit.Common;

namespace VEdit.Editor.ActionsList
{
    public abstract class ListEntry : ObservableObject
    {
        public string Name { get; set; }

        public ListEntry(string name) => Name = name;
    }
}
