using System;
using System.Collections.Generic;
using System.Linq;
using VEdit.Common;

namespace VEdit.Editor
{
    public interface INodeDatabase
    {
        NodeEntry Get(Guid id);
        IEnumerable<GraphNodeEntry> GetByProject(Guid projectId);
        GraphNodeEntry GetByFile(Guid fileId);

        void Register(NodeEntry data);
        void Replace(NodeEntry data);
        void Remove(NodeEntry data);
        void Update(NodeEntry data);

        void Load(IEnumerable<GraphNodeEntry> nodes);
        void Load();

        IEnumerable<GraphNodeEntry> GraphNodes { get; }

        event Action<NodeEntry> DataUpdated;
    }

    public class NodeDatabase : INodeDatabase
    {
        private readonly IActionsDatabase _actionsDatabase;
        private readonly IPluginProvider _plugins;
        private readonly IOutputManager _output;

        public IEnumerable<GraphNodeEntry> GraphNodes => _data.Values.Where(v => v is GraphNodeEntry).Cast<GraphNodeEntry>();

        private Dictionary<Guid, NodeEntry> _data = new Dictionary<Guid, NodeEntry>();

        public event Action<NodeEntry> DataUpdated;

        public NodeDatabase(IActionsDatabase actionsDatabase, IPluginProvider pluginProvider, IOutputManager output)
        {
            _actionsDatabase = actionsDatabase;
            _plugins = pluginProvider;
            _output = output;

            Register(new NodeEntry(typeof(VariableNode<bool>), Guid.Parse("91bf6391-aa44-46d2-9aa6-280ffd102dc3"))
            {
                DisplayName = "New bool",
                Keywords = "variable bool"
            });

            Register(new NodeEntry(typeof(VariableNode<int>), Guid.Parse("3ba992a0-137f-47a4-8d48-00ca82565f7b"))
            {
                DisplayName = "New int",
                Keywords = "variable int"
            });

            Register(new NodeEntry(typeof(VariableNode<double>), Guid.Parse("d5c82a03-3b03-47d6-b6c3-93dd2ac53209"))
            {
                DisplayName = "New double",
                Keywords = "variable double"
            });

            Register(new NodeEntry(typeof(VariableNode<string>), Guid.Parse("cd475c21-6439-423e-a728-398d3bc7dfd4"))
            {
                DisplayName = "New string",
                Keywords = "variable string"
            });

            Register(new NodeEntry(typeof(BranchNode), BranchNode.Template)
            {
                DisplayName = "Branch",
                Category = "Flow",
                Keywords = "branch if bool flow"
            });

            Register(new NodeEntry(typeof(SequenceNode), SequenceNode.Template)
            {
                DisplayName = "Sequence",
                Category = "Flow",
                Keywords = "sequence execute flow"
            });

            Register(new NodeEntry(typeof(WhileNode), WhileNode.Template)
            {
                DisplayName = "While",
                Category = "Flow",
                Keywords = "while loop flow"
            });

            Register(new NodeEntry(typeof(ForLoopNode), ForLoopNode.Template)
            {
                DisplayName = "For",
                Category = "Flow",
                Keywords = "for loop flow"
            });

            Register(new NodeEntry(typeof(PrintNode), PrintNode.Template)
            {
                DisplayName = "Print",
                Category = "Utils",
                Keywords = "print string"
            });

            Register(new NodeEntry(typeof(ToStringNode), ToStringNode.Template)
            {
                DisplayName = "To String",
                Category = "Utils",
                Keywords = "to string"
            });
        }

        public NodeEntry Get(Guid templateId)
        {
            if(!_data.TryGetValue(templateId, out NodeEntry value))
            {
                _output.Write($"Node {templateId} could not be found.", OutputType.Warning);
            }
            return value;
        }

        public void Register(NodeEntry data)
        {
            _data.Add(data.TemplateId, data);
            SendToDatabase(data);
        }

        private void SendToDatabase(NodeEntry data)
        {
            _actionsDatabase.Add(new ActionEntry(data.TemplateId, data.DisplayName, data.Category)
            {
                ProjectId = data is GraphNodeEntry gr ? gr.ProjectId : Guid.Empty,
                Keywords = data.Keywords
            });
        }

        public void Load()
        {
            var plugins = _plugins.Load();
            foreach (var node in plugins)
            {
                Register(node);
            }
        }

        public void Replace(NodeEntry data)
        {
            if(!_data.ContainsKey(data.TemplateId))
            {
                Register(data);
            }
            else
            {
                Update(data);
            }
        }

        public void Load(IEnumerable<GraphNodeEntry> nodes)
        {
            foreach (var node in nodes)
            {
                Replace(node);
            }
        }

        public IEnumerable<GraphNodeEntry> GetByProject(Guid projectId)
        {
            return GraphNodes.Where(n => n.ProjectId == projectId);
        }

        public GraphNodeEntry GetByFile(Guid fileId)
        {
            return _data.Values.FirstOrDefault(n => n is GraphNodeEntry gr && gr.FileId == fileId) as GraphNodeEntry;
        }

        public void Remove(NodeEntry data)
        {
            _data.Remove(data.TemplateId);
            _actionsDatabase.Remove(data.TemplateId);
        }

        public void Update(NodeEntry data)
        {
            _data[data.TemplateId] = data;
            _actionsDatabase.Remove(data.TemplateId);
            SendToDatabase(data);
            OnDataUpdated(data);
        }

        private void OnDataUpdated(NodeEntry data)
        {
            DataUpdated?.Invoke(data);
        }
    }
}
