using System.Collections.Generic;
using System.Linq;
using VEdit.Common;

namespace VEdit.Editor
{
    public class Comment : BlackboardElement
    {
        private Graph _graph;

        public Comment(Graph graph) : base(graph)
        {
            _graph = graph;
            var cmdProvider = ServiceProvider.Get<ICommandProvider>();

            Actions.Add("Select nodes", cmdProvider.Create(SelectNodes));
            Actions.Add("Rename comment", cmdProvider.Create(Rename));
            Actions.Add("Delete comment", cmdProvider.Create(Delete));
            Actions.Add("Delete selection", cmdProvider.Create(DeleteSelection));
        }

        private void DeleteSelection()
        {
            Delete();
            _graph.DeleteSelection();
        }

        public async void Rename()
        {
            var dialog = ServiceProvider.Get<IDialogManager>();
            Name = await dialog.ShowInputAsync("Comment", "Enter comment");
        }

        public void SelectNodes()
        {
            Parent.SelectionService.SelectRange(GetChildren());
        }

        public void Delete()
        {
            _graph.DeleteComment(this);
        }

        public override bool IsSelected
        {
            get => base.IsSelected;
            set
            {
                if(value && value != base.IsSelected)
                {
                    SelectNodes();
                }

                base.IsSelected = value;
            }
        }

        private IEnumerable<IElement> GetChildren()
        {
            return Parent.Elements.GetElementsInBounds(X, Y, Width, Height).Where(e => !(e is Comment));
        }
    }
}
