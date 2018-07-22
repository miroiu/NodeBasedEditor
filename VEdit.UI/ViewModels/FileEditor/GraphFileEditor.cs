using System;
using System.Linq;
using VEdit.Common;
using VEdit.Editor;

namespace VEdit.UI
{
    public class GraphFileEditor : FileEditor
    {
        private IOutputManager _output;
        private INodeDatabase _database;
        private ICommandProvider _cmdProvider;

        private Graph _graph;
        public Graph Graph
        {
            get => _graph;
            set => SetProperty(ref _graph, value);
        }

        public GraphFileEditor(ProjectFile file, Common.IServiceProvider serviceProvider) : base(file, serviceProvider)
        {
            _output = ServiceProvider.Get<IOutputManager>();
            _database = ServiceProvider.Get<INodeDatabase>();
            _database.DataUpdated += OnDataUpdated;

            _cmdProvider = ServiceProvider.Get<ICommandProvider>();
            CompileGraphCommand = _cmdProvider.Create(CompileGraph);
            PasteCommand = _cmdProvider.Create(Paste);
        }

        public ICommand CompileGraphCommand { get; }
        public ICommand PasteCommand { get; }

        private ICommand _jumpToNode;
        public ICommand JumpToInputCommand
        {
            get => _jumpToNode;
            set => SetProperty(ref _jumpToNode, value);
        }

        private ICommand _jumpToOuputNode;
        public ICommand JumpToOutputCommand
        {
            get => _jumpToOuputNode;
            set => SetProperty(ref _jumpToOuputNode, value);
        }
        
        public void CompileGraph()
        {
            var nodeData = Graph.ToNodeData();
            _database.Replace(nodeData);
        }

        private void Paste()
        {
            Graph.PasteFromClipboard();
        }

        public override bool LoadContent()
        {
            var archive = ServiceProvider.Get<IArchive>();
            var factory = ServiceProvider.Get<IGraphFactory>();

            archive.Load(Path);
            var type = archive.Read<Type>(nameof(Type));

            Graph = factory.Create(type);
            Graph.ProjectId = File.Project.Id;
            Graph.FileId = File.Id;
            Graph.Load(archive);

            if (Graph == null)
            {
                _output.Write($"Graph type: {type} is not registered.", OutputType.Error);
                return false;
            }

            JumpToInputCommand = _cmdProvider.Create(() => Graph.JumpToNode(Graph.Entry));
            JumpToOutputCommand = _cmdProvider.Create(() => Graph.JumpToNode(Graph.Exit));

            return true;
        }

        public override void SaveContent()
        {
            var archive = ServiceProvider.Get<IArchive>();

            archive.Write(nameof(Type), Graph.GetType());
            Graph.Name = Name;

            Graph.Save(archive);
            archive.Save(Path);
        }

        private void OnDataUpdated(NodeEntry obj)
        {
            var old = Graph.Nodes.FirstOrDefault(n => n.TemplateId == obj.TemplateId);
            if (old != null)
            {
                var builder = new NodeBuilder(Graph, new NodeBuilder.Configuration(ServiceProvider, obj.TemplateId));
                var newNode = builder.Build();
                if (newNode != null)
                {
                    var links = Graph.GetLinks(old).ToList();
                    var arc = Graph.SaveLinks(links);

                    var tempFile = System.IO.Path.GetTempFileName();
                    arc.Save(tempFile);

                    Graph.DeleteNode(old);
                    Graph.AddNode(newNode);

                    arc.Load(tempFile);
                    Graph.LoadLinks(arc);

                    System.IO.File.Delete(tempFile);

                    newNode.X = old.X;
                    newNode.Y = old.Y;
                }
            }
        }
    }
}
