using System.Collections.Specialized;
using System.Windows.Controls;
using Microsoft.Xaml.Behaviors;

namespace VEdit
{
    public class AutoScrollBehaviour : Behavior<ListBox>
    {
        protected override void OnAttached()
        {
            ListBox listBox = AssociatedObject;
            ((INotifyCollectionChanged)listBox.Items).CollectionChanged += OnListBox_CollectionChanged;
        }

        protected override void OnDetaching()
        {
            ListBox listBox = AssociatedObject;
            ((INotifyCollectionChanged)listBox.Items).CollectionChanged -= OnListBox_CollectionChanged;
        }

        private void OnListBox_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ListBox listBox = AssociatedObject;
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                listBox.ScrollIntoView(e.NewItems[0]);
            }
        }
    }
}
