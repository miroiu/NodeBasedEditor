using System;
using System.Linq;
using VEdit.Common;
using VEdit.Execution;

namespace VEdit.Editor
{
    public class GraphNode : Node
    {
        public Guid LinkedGraphId { get; }

        private IGraphProvider _provider;

        private Graph _linkedGraph;
        public Graph LinkedGraph
        {
            get
            {
                if (_linkedGraph == null)
                {
                    _linkedGraph = _provider.Get(LinkedGraphId);
                }
                return _linkedGraph;
            }
        }

        public GraphNode(Graph root, Guid templateId) : base(root, templateId)
        {
            Id = templateId;

            var cmdProvider = ServiceProvider.Get<ICommandProvider>();
            _provider = ServiceProvider.Get<IGraphProvider>();

            Actions.Add("Go to Graph", cmdProvider.Create(GoToGraph));

            LinkedGraphId = templateId;
        }

        public void GoToGraph()
        {
            if (LinkedGraphId != Graph.Id)
            {
                _provider.OpenInEditor(LinkedGraphId);
            }
        }

        public override IExecutionBlock Compile(IExecutionContext context)
        {
            var newContext = new ExecutionContext(context);
            var result = LinkedGraph.Compile(newContext);
            return new EntryExitBlock(result[0], result[1], newContext);
        }
    }
}
