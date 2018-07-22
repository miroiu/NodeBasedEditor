using System.Collections.Generic;
using System.Linq;
using VEdit.Common;
using VEdit.Editor.ActionsList;

namespace VEdit.Editor
{
    public class ActionList : BlackboardElement, IActionList
    {
        private string _filterString;

        private Category _root;
        public Category Root
        {
            get => _root;
            set => SetProperty(ref _root, value);
        }

        private IServiceProvider _serviceProvider;
        private IActionsDatabase _actionsDatabase;
        private Graph _graph;

        public ActionList(Graph graph, IServiceProvider serviceProvider) : base(graph)
        {
            _serviceProvider = serviceProvider;
            _graph = graph;
            _graph.Loaded += OnGraphLoaded;
            Name = "Available Actions";

            var cmdProvider = serviceProvider.Get<ICommandProvider>();
            _actionsDatabase = serviceProvider.Get<IActionsDatabase>();

            SpawnNodeCommand = cmdProvider.Create<System.Guid>(SpawnNode);
            CloseCommand = cmdProvider.Create(Close);

            _actionsDatabase.DatabaseChanged += OnDatabaseChanged;
            Root = CreateActionList(_actionsDatabase.GetAvailable(_graph.ProjectId));
        }

        private void OnGraphLoaded()
        {
            OnDatabaseChanged();
        }

        private void OnDatabaseChanged()
        {
            Root = CreateActionList(_actionsDatabase.GetAvailable(_graph.ProjectId));
        }

        private Category CreateActionList(IEnumerable<ActionEntry> entries)
        {
            var root = new Category("Nodes");
            var categories = new Dictionary<string, Category>();

            foreach (var entry in entries)
            {
                Category parent = root;
                if (!string.IsNullOrWhiteSpace(entry.Category))
                {
                    var catSplit = entry.Category.Split(new[] { '|', '/' }).Select(c => c.Trim());

                    foreach (var split in catSplit)
                    {
                        if (!categories.TryGetValue(split, out Category value))
                        {
                            value = new Category(split);
                            parent?.Children.Add(value);
                            categories.Add(split, value);
                        }

                        parent = value;
                    }
                }
                parent.Children.Add(new Action(entry.Name, entry.TemplateId));
            }

            return root;
        }

        public ICommand SpawnNodeCommand { get; }
        public ICommand CloseCommand { get; }

        public string FilterString
        {
            get => _filterString;
            set
            {
                if (SetProperty(ref _filterString, value))
                {
                    var filter = _actionsDatabase.FilterByText(_graph.ProjectId, value);
                    Root = CreateActionList(filter);

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        Root.ExpandAll();
                    }
                    else
                    {
                        Root.CloseAll();
                    }
                }
            }
        }

        public event CloseEventHandler CloseEvent;

        public void Close() => CloseEvent?.Invoke();

        public void SpawnNode(System.Guid templateId)
        {
            var node = new NodeBuilder(_graph, new NodeBuilder.Configuration(ServiceProvider, templateId))
                .Build();

            node.X = X;
            node.Y = Y;

            _graph.AddNode(node);
            Close();
        }
    }
}
