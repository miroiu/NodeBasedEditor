using System;
using System.Collections.Generic;

namespace VEdit.Editor
{
    public interface INodeBuilder
    {
        Node Build();
    }

    public class NodeBuilder : INodeBuilder
    {
        public class Configuration
        {
            private Configuration(Common.IServiceProvider provider)
            {
                Factory = provider.Get<INodeFactory>();
            }

            public Configuration(Common.IServiceProvider serviceProvider, Guid templateId) : this(serviceProvider)
            {
                TemplateData = serviceProvider.Get<INodeDatabase>().Get(templateId);
                NodeType = TemplateData?.NodeType;
            }

            protected Configuration(Common.IServiceProvider serviceProvider, Type type) : this(serviceProvider)
            {
                NodeType = type;
            }

            public Type NodeType { get; }
            public NodeEntry TemplateData { get; }
            public INodeFactory Factory { get; }
            public bool IsValid => NodeType != null;
        }

        public class Configuration<T> : Configuration where T : Node
        {
            public Configuration(Common.IServiceProvider serviceProvider) : base(serviceProvider, typeof(T))
            {
            }
        }

        private IList<PinData> _input = new List<PinData>();
        private IList<PinData> _output = new List<PinData>();

        public Graph Graph { get; }
        public Configuration Config { get; }
        public bool IsTemplate => Config.TemplateData != null;

        public NodeBuilder(Graph root, Configuration config)
        {
            Graph = root;
            Config = config;
        }

        public virtual Node Build()
        {
            Node node = null;

            if (Config.IsValid)
            {
                if (IsTemplate)
                {
                    if (Config.TemplateData is Plugin plugin)
                    {
                        node = CreatePlugin(plugin);
                    }
                    else
                    {
                        node = Config.Factory.Create(Config.NodeType, Graph, Config.TemplateData.TemplateId);
                        node.Name = Config.TemplateData.DisplayName;
                    }

                    _input = Config.TemplateData.Input;
                    _output = Config.TemplateData.Output;
                }
                else
                {
                    node = Config.Factory.Create(Config.NodeType, Graph, Guid.Empty);
                }

                BuildPins(node);
            }

            return node;
        }

        private Node CreatePlugin(Plugin plugin)
        {
            var isCompact = !string.IsNullOrWhiteSpace(plugin.CompactName);

            return new PluginNode(Graph, plugin.TemplateId, isCompact, plugin.Method)
            {
                Name = isCompact ? plugin.CompactName : plugin.DisplayName,
                Description = Config.TemplateData.Tooltip,
            };
        }

        private void BuildPins(Node node)
        {
            foreach (var pin in _input)
            {
                AddPin(Direction.Input, pin);
            }

            foreach (var pin in _output)
            {
                AddPin(Direction.Output, pin);
            }

            void AddPin(Direction dir, PinData pin)
            {
                Pin result = pin.Type == PinType.Data ? node.AddDataPin(dir, pin.DataType, pin.Name) : node.AddExecPin(dir, pin.Name);

                if (pin.IsFactory)
                {
                    node.FactoryPin = result;
                }
            }
        }
    }

    public class NodeBuilder<T> : NodeBuilder where T : Node
    {
        public NodeBuilder(Graph root, Common.IServiceProvider serviceProvider) : base(root, new Configuration<T>(serviceProvider))
        {
        }
    }
}
