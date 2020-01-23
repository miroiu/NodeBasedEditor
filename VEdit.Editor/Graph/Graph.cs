using System;
using System.Collections.Generic;
using System.Linq;
using VEdit.Common;
using VEdit.Core;
using VEdit.Execution;

namespace VEdit.Editor
{
    public abstract class Graph : Blackboard, ISaveLoad
    {
        const string CopyPasteFormat = "VEdit.Editor.CopyPasteFormat";

        [Serializable]
        private class CopyPasteFiles
        {
            public CopyPasteFiles()
            {
                NodesFile = TempFile.Create();
                LinksFile = TempFile.Create();
            }

            public string NodesFile { get; }
            public string LinksFile { get; }
        }

        public EntryNode Entry { get; }
        public ExitNode Exit { get; }

        private List<Node> _nodes = new List<Node>();
        public IEnumerable<Node> Nodes => _nodes;

        private List<Link> _links = new List<Link>();
        public IEnumerable<Link> Links => _links;

        private List<Comment> _comments = new List<Comment>();

        public IEnumerable<Comment> Comments => _comments;

        public IEnumerable<Node> SelectedNodes => Nodes.Where(n => n.IsSelected).Cast<Node>();

        public ActionList NodeList { get; }
        // Instance id used for serialization
        public Guid Id { get; protected set; }
        public Guid ProjectId { get; set; }
        public Guid FileId { get; set; }

        private readonly IOutputManager _output;
        private readonly ICommandProvider _cmdProvider;

        public Graph(GraphDto graph, ISelectionService<BlackboardElement> selectionService, ICommandProvider commandProvider, IActionsDatabase actionsDatabase,
            IOutputManager outputManager) : base(selectionService)
        {
            Id = Guid.NewGuid();
            NodeList = new ActionList(this, commandProvider, actionsDatabase);

            _cmdProvider = commandProvider;
            _output = outputManager;
        }

        public Graph(Common.IServiceProvider serviceProvider) : base(serviceProvider)
        {
            Id = Guid.NewGuid();
            NodeList = new ActionList(this, serviceProvider);

            _output = serviceProvider.Get<IOutputManager>();
            _cmdProvider = serviceProvider.Get<ICommandProvider>();

            var entryBuilder = new NodeBuilder<EntryNode>(this, serviceProvider);
            Entry = BuildEntryNode(entryBuilder);
            Entry.Name = "Inputs";

            var exitBuilder = new NodeBuilder<ExitNode>(this, serviceProvider);
            Exit = BuildReturnNode(exitBuilder);
            Exit.Name = "Outputs";

            AddNode(Entry);
            AddNode(Exit);
        }

        #region Nodes API

        public abstract EntryNode BuildEntryNode(NodeBuilder<EntryNode> builder);
        public abstract ExitNode BuildReturnNode(NodeBuilder<ExitNode> builder);

        public void AddNode(Node node)
        {
            AddElement(node);
            _nodes.Add(node);
        }

        public void DeleteNode(Node node)
        {
            if (!node.IsEntryOrExit())
            {
                BreakLinks(node);

                RemoveElement(node);
                _nodes.Remove(node);
            }
        }

        #endregion

        #region Links API

        internal void RemoveLink(Link link)
        {
            RemoveElement(link);
            _links.Remove(link);

            if (!HasLinks(link.Input))
            {
                link.Input.IsConnected = false;
            }

            if (!HasLinks(link.Output))
            {
                link.Output.IsConnected = false;
            }
        }

        public bool HasLinks(Pin pin)
        {
            return Links.Any(l => l.Input == pin || l.Output == pin);
        }

        // Should only be used when deserializing
        internal void AddLink(Link link)
        {
            ValidateLinksBeforeNewLink(link.Input, link.Output);
            AddPinsActions(link);

            AddElement(link);
            _links.Add(link);

            link.Input.IsConnected = true;
            link.Output.IsConnected = true;

            void ValidateLinksBeforeNewLink(Pin input, Pin output)
            {
                if (output.IsExec && output.IsConnected)
                {
                    output.BreakLinks();
                }

                if (!input.IsExec && input.IsConnected)
                {
                    input.BreakLinks();
                }
            }
        }

        internal void BreakLinks(Node node)
        {
            var links = Links.Where(l => l.Input.Node == node || l.Output.Node == node).ToList();
            foreach (var link in links)
            {
                RemoveLink(link);
            }
        }

        private void AddPinsActions(Link link)
        {
            // TODO: Save actions in a dictionary with the link as the key and when the link is removed, remove the action from the pin

            //var commandProvider = ServiceProvider.Get<ICommandProvider>();

            //link.Input.Actions.Add($"Break link to {link.Output.Name} ({(link.Output.IsExec ? "execute" : "data")})",
            //    commandProvider.Create(() => RemoveLink(link)));
        }

        public IEnumerable<Link> GetLinks(Pin pin)
        {
            return Links.Where(l => l.Input == pin || l.Output == pin);
        }

        public IEnumerable<Link> GetLinks(Node node)
        {
            return Links.Where(l => l.Input.Node == node || l.Output.Node == node);
        }

        public IEnumerable<Link> GetLinks(IEnumerable<Node> nodes)
        {
            var links = new List<Link>();
            foreach (var node in nodes)
            {
                links.AddRange(Links.Where(l => l.Input.Node == node || l.Output.Node == node));
            }
            return links;
        }

        internal void BreakLinks(Pin pin)
        {
            var links = Links.Where(l => l.Input == pin || l.Output == pin).ToList();
            foreach (var link in links)
            {
                RemoveLink(link);
            }
        }

        public bool ConnectionHasErrors(Pin input, Pin output, out string error)
        {
            error = null;

            if (input.Node == output.Node)
            {
                error = "Pins are on the same node";
                return true;
            }

            if (input.IsInput == output.IsInput)
            {
                error = $"Both pins are {(input.IsInput ? "input" : "output")}";
                return true;
            }

            if (input.IsExec != output.IsExec)
            {
                error = "Cannot connect data to exec";
                return true;
            }

            if (Links.Any(l => l.Input == input && l.Output == output))
            {
                error = "Existing connection";
                return true;
            }

            if (!input.IsExec)
            {
                // TODO: Check if a cast node is defined and spawn it
                if (!output.Type.HasCastDefined(input.Type))
                {
                    error = "No cast defined";
                    return true;
                }
            }

            if (input.Graph != output.Graph)
            {
                return true;
            }

            return false;
        }

        public bool TryToConnectPins(Pin pin1, Pin pin2)
        {
            var input = pin1.IsInput ? pin1 : pin2;
            var output = pin2.IsInput ? pin1 : pin2;

            if (ConnectionHasErrors(input, output, out string error))
            {
                return false;
            }

            AddLink(new Link(this, input, output));

            return true;
        }

        #endregion

        #region Commands

        public void AddComment(Comment comment)
        {
            AddElement(comment);
            _comments.Add(comment);
        }

        public void DeleteComment(Comment comment)
        {
            RemoveElement(comment);
            _comments.Remove(comment);
        }

        public void CommentSelection()
        {
            var offset = 30;
            var bounding = SelectionService.Selection.GetBoundingBox(offset);

            var comment = new Comment(this);
            comment.Rename();
            AddComment(comment);

            comment.X = bounding.X;
            comment.Y = bounding.Y - offset / 2;
            comment.Width = bounding.Width;
            comment.Height = bounding.Height;
        }

        public void CopySelectionToClipboard(bool cut = false)
        {
            var copyPaste = new CopyPasteFiles();

            var clip = ServiceProvider.Get<IClipboardManager>();
            clip.SetData(CopyPasteFormat, copyPaste);

            var links = GetLinks(SelectedNodes);

            var nodes = SaveNodes(SelectedNodes.Where(n => n.CanCopy));
            var linksArc = SaveLinks(links);

            nodes.AllowOverwrite = true;
            nodes.Save(copyPaste.NodesFile);

            linksArc.AllowOverwrite = true;
            linksArc.Save(copyPaste.LinksFile);

            if (cut)
            {
                DeleteSelection();
            }
        }

        public void PasteFromClipboard()
        {
            var clip = ServiceProvider.Get<IClipboardManager>();
            var files = clip.GetData<CopyPasteFiles>(CopyPasteFormat);

            if (files != null)
            {
                var nodesArc = ServiceProvider.Get<IArchive>();
                nodesArc.Load(files.NodesFile);

                var linksArc = ServiceProvider.Get<IArchive>();
                linksArc.Load(files.LinksFile);

                var nodes = LoadNodes(nodesArc);
                LoadLinks(linksArc);

                foreach (var node in nodes)
                {
                    node.Id = Guid.NewGuid();
                    node.X += 50;
                    node.Y += 50;
                }
            }
        }

        internal void CollapseSelectionToFunction(bool toMacro = false)
        {
            //var nodes = SaveNodes(SelectedNodes);
            //var links = GetLinks(SelectedNodes);
        }

        internal void DeleteSelection()
        {
            var selection = SelectionService.Selection;
            var nodes = SelectedNodes.ToList();

            foreach (var node in nodes)
            {
                DeleteNode(node);
            }
        }

        #endregion

        #region Serialization

        public void Save(IArchive archive)
        {
            archive.Write(nameof(Name), Name);
            archive.Write(nameof(Description), Description);

            archive.Write(nameof(Id), Id);

            archive.Write(nameof(Nodes), SaveNodes(Nodes.Where(n => !n.IsGetOrSet())));
            archive.Write(nameof(Links), SaveLinks(Links));
            archive.Write(nameof(Comments), SaveComments(Comments));
        }

        public event Action Loaded;

        private void OnLoaded()
        {
            Loaded?.Invoke();
        }

        public void Load(IArchive archive)
        {
            Name = archive.Read<string>(nameof(Name));
            Description = archive.Read<string>(nameof(Description));

            Id = archive.Read<Guid>(nameof(Id));

            LoadNodes(archive.Read<Archive>(nameof(Nodes))).Run();
            LoadLinks(archive.Read<Archive>(nameof(Links)));
            LoadComments(archive.Read<Archive>(nameof(Comments)));

            OnLoaded();
        }

        private IArchive SaveComments(IEnumerable<Comment> comments)
        {
            var commentsArchive = ServiceProvider.Get<IArchive>();

            foreach (var comment in comments)
            {
                var commentArchive = ServiceProvider.Get<IArchive>();
                commentsArchive.Write(comment.GetHashCode().ToString(), commentArchive);
                comment.Save(commentArchive);
            }

            return commentsArchive;
        }

        public IArchive SaveLinks(IEnumerable<Link> links)
        {
            var linksArchive = ServiceProvider.Get<IArchive>();
            linksArchive.AllowOverwrite = true;

            foreach (var link in links)
            {
                var linkArchive = ServiceProvider.Get<IArchive>();
                linksArchive.Write(link.GetHashCode().ToString(), linkArchive);
                link.Save(linkArchive);
            }

            return linksArchive;
        }

        public IArchive SaveNodes(IEnumerable<Node> nodes)
        {
            var nodesArchive = ServiceProvider.Get<IArchive>();
            foreach (var node in nodes)
            {
                var nodeArchive = ServiceProvider.Get<IArchive>();
                nodesArchive.Write(node.Id.ToString(), nodeArchive);
                node.Save(nodeArchive);
            }

            return nodesArchive;
        }

        private void LoadComments(IArchive commentsArchive)
        {
            var comments = commentsArchive.ReadAll<Archive>();
            foreach (var comment in comments)
            {
                var com = new Comment(this);
                AddComment(com);
                com.Load(comment);
            }
        }

        public void LoadLinks(IArchive linksArchive)
        {
            var links = linksArchive.ReadAll<Archive>();

            foreach (var link in links)
            {
                var input = link.Read<Archive>(nameof(Link.Input));
                var inNode = input.Read<Guid>(nameof(Node));
                var inIndex = input.Read<int>("Index");

                var output = link.Read<Archive>(nameof(Link.Output));
                var outNode = output.Read<Guid>(nameof(Node));
                var outIndex = output.Read<int>("Index");

                var firstNode = Nodes.FirstOrDefault(n => n.Id == inNode);
                var secondNode = Nodes.FirstOrDefault(n => n.Id == outNode);

                if (firstNode != null && secondNode != null)
                {
                    if (firstNode.Input.Count > inIndex && secondNode.Output.Count > outIndex)
                    {
                        var inputPin = firstNode.Input[inIndex];
                        var outputPin = secondNode.Output[outIndex];

                        TryToConnectPins(inputPin, outputPin);
                    }
                    else
                    {
                        _output.Write($"Could not create link in node {outNode}: Input.Index={inIndex} and Output.Index={outIndex}",
                            _cmdProvider.Create(() => Focus(firstNode)),
                            OutputType.Warning);
                    }
                }
            }
        }

        public IEnumerable<Node> LoadNodes(IArchive nodesArchive)
        {
            var nodes = nodesArchive.ReadAll<Archive>();

            foreach (var node in nodes)
            {
                var templateId = node.Read<Guid>(nameof(Node.TemplateId));
                if (templateId != Guid.Empty)
                {
                    var builder = new NodeBuilder(this, new NodeBuilder.Configuration(ServiceProvider, templateId));
                    var result = builder.Build();
                    if (result != null)
                    {
                        result.Load(node);
                        AddNode(result);
                        yield return result;
                    }
                    else
                    {
                        var name = node.Read<string>(nameof(Node.Name));
                        _output.Write($"Could not load node: {name} -> {templateId}", OutputType.Warning);
                    }
                }
                else // it's a default node spawned by the graph or a variable
                {
                    var instanceId = node.Read<Guid>(nameof(Node.Id));
                    if (instanceId == EntryNode.InstanceId)
                    {
                        Entry.Load(node);
                    }
                    else if (instanceId == ExitNode.InstanceId)
                    {
                        Exit.Load(node);
                    }
                    else // it's a variable
                    {
                        var type = node.Read<Type>(nameof(VariableNode.Type));
                        var result = this.AddVariable(type);
                        result.Load(node);
                    }
                }
            }
        }

        #endregion

        private Dictionary<Node, IExecutionBlock> _blocks = new Dictionary<Node, IExecutionBlock>();

        public virtual IExecutionBlock[] Compile(IExecutionContext context)
        {
            _blocks.Clear();

            foreach (var node in Nodes)
            {
                var block = node.Compile(context);
                _blocks.Add(node, block);

                if (node is GraphNode)
                {
                    var pins = node.Input.Where(p => !p.IsExec).ToList();
                    for (int i = 0; i < pins.Count; i++)
                    {
                        block.In[i].Value = pins[i].DefaultValue;
                    }
                }
                else
                {
                    foreach (var pin in node.Input.Where(p => !p.IsExec))
                    {
                        block.In.Add(new Parameter(pin.DefaultValue));
                    }

                    foreach (var pin in node.Output.Where(p => !p.IsExec))
                    {
                        block.Out.Add(new Parameter(pin.DefaultValue));
                    }
                }

                var execs = node.Output.Count(p => p.IsExec);
                for (int i = 0; i < execs; i++)
                {
                    block.Next.Add(null);
                };
            }

            foreach (var link in Links)
            {
                var input = link.Input;
                var output = link.Output;

                var inputNode = input.Node;
                var outputNode = output.Node;

                var inBlock = _blocks[inputNode];
                var outBlock = _blocks[outputNode];

                if (input.IsExec /*&& link.Output.IsExec*/)
                {
                    outBlock.Next[output.Index] = inBlock;
                }
                else
                {
                    if (!outputNode.IsExec && !outputNode.IsEntryOrExit())
                    {
                        inBlock.Dependency.Add(outBlock);
                    }

                    var inEx = input.Node.Input.Where(n => n.IsExec).Count();
                    var outEx = output.Node.Output.Where(n => n.IsExec).Count();

                    if (inputNode is GraphNode)
                    {
                        inBlock.In[input.Index - inEx].Dependency = outBlock.Out[output.Index - outEx];
                    }
                    else
                    {
                        inBlock.In[input.Index - inEx] = new Parameter(input.DefaultValue, outBlock.Out[output.Index - outEx]);
                    }
                }
            }

            foreach (GraphNode node in Nodes.Where(n => n is GraphNode))
            {
                var graph = node.LinkedGraph;
                var outBlock = graph._blocks[graph.Exit];

                var exitExecs = graph.Exit.Input.Where(i => i.IsExec).ToList();

                for (int i = 0; i < exitExecs.Count; i++)
                {
                    // Get connected node to graph node pin
                    var nodeExec = node.Output[i].Links.FirstOrDefault()?.Input.Node;
                    if (nodeExec != null)
                    {
                        var links = graph.GetLinks(exitExecs[i]);

                        var nodeExecBlock = _blocks[nodeExec];
                        foreach (var linkNode in links.Select(l => l.Output.Node))
                        {
                            var linkNodeblock = graph._blocks[linkNode];
                            var idx = linkNodeblock.Next.IndexOf(outBlock);
                            linkNodeblock.Next[idx] = nodeExecBlock;
                        }
                    }
                }

                var exitDataPins = graph.Exit.Input.Where(i => !i.IsExec).ToList();

                for (int i = 0; i < exitDataPins.Count; i++)
                {
                    var idx = exitExecs.Count + i;
                    var dataPin = exitDataPins[i];
                    var inLinks = node.Output[idx].Links;

                    var outNode = dataPin.Links.FirstOrDefault()?.Output.Node;
                    var outDataBlock = outNode != null ? graph._blocks[outNode] : null;
                    var outExecCount = node.Output.Where(p => p.IsExec).Count();

                    foreach (var inLink in inLinks)
                    {
                        var inPin = inLink.Input;
                        var inNode = inPin.Node;

                        var inBlock = _blocks[inNode];

                        var inExecCount = inNode.Input.Where(p => p.IsExec).Count();

                        if (outDataBlock != null)
                        {
                            inBlock.In[inPin.Index - inExecCount].Dependency = outDataBlock.Out[inLink.Output.Index - outExecCount];
                        }
                        else
                        {
                            inBlock.In[inPin.Index - exitExecs.Count].Value = dataPin.DefaultValue;
                        }

                        if (!inNode.IsEntryOrExit())
                        {
                            inBlock.Dependency.Add(outDataBlock);
                        }
                    }
                }
            }
            return new[] { _blocks[Entry], _blocks[Exit] };
        }
    }
}
