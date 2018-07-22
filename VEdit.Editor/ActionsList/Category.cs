using System.Collections.ObjectModel;

namespace VEdit.Editor.ActionsList
{
    public class Category : ListEntry
    {
        public Category(string name) : base(name)
        {
            Children = new ObservableCollection<ListEntry>();
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        public ObservableCollection<ListEntry> Children { get; }

        public void ExpandAll()
        {
            IsExpanded = true;
            foreach(var child in Children)
            {
                if(child is Category cat)
                {
                    cat.ExpandAll();
                }
            }
        }

        public void CloseAll()
        {
            IsExpanded = false;
            foreach (var child in Children)
            {
                if (child is Category cat)
                {
                    cat.CloseAll();
                }
            }
        }
    }
}
