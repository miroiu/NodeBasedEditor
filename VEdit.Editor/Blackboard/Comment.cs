using System;
using System.Collections.Generic;
using System.Linq;
using VEdit.Common;

namespace VEdit.Editor
{
    public class Comment : BlackboardElement, ISaveLoad
    {
        private Graph _graph;

        public event Action Loaded;

        public Comment(Graph graph) : base(graph, graph.ServiceProvider)
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

        private IEnumerable<IBlackboardElement> GetChildren()
        {
            return Parent.Elements.GetElementsInBounds(X, Y, Width, Height).Where(e => !(e is Comment));
        }

        public void Save(IArchive archive)
        {
            archive.Write(nameof(X), X);
            archive.Write(nameof(Y), Y);

            archive.Write(nameof(Width), Width);
            archive.Write(nameof(Height), Height);

            archive.Write(nameof(Name), Name);
            archive.Write(nameof(Description), Description);
        }

        public void Load(IArchive archive)
        {
            X = archive.Read<double>(nameof(X));
            Y = archive.Read<double>(nameof(Y));

            Width = archive.Read<double>(nameof(Width));
            Height = archive.Read<double>(nameof(Height));

            Name = archive.Read<string>(nameof(Name));
            Description = archive.Read<string>(nameof(Description));
        }
    }
}
