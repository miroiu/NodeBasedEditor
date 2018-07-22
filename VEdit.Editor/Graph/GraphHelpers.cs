using System.Linq;
using VEdit.Common;

namespace VEdit.Editor
{
    public static class GraphHelpers
    {
        #region Extensions

        // TODO: Check if there is a path from entry to exit
        public static bool IsEntryConnectedToExit(this Graph graph)
        {
            var entry = graph.Entry;
            var exit = graph.Exit;

            return true;
        }

        public static GraphNodeEntry ToNodeData(this Graph graph)
        {
            var entry = graph.Entry;
            var exit = graph.Exit;

            var node = new GraphNodeEntry(graph.Id, graph.ProjectId, graph.FileId, graph.Id)
            {
                DisplayName = graph.Name,
                Tooltip = graph.Description
            };

            foreach (var pin in entry.Output)
            {
                node.Input.Add(new PinData(pin.Name, pin.Type));
            }

            foreach (var pin in exit.Input)
            {
                node.Output.Add(new PinData(pin.Name, pin.Type));
            }

            return node;
        }

        public static VariableNode AddVariable(this Graph graph, System.Type type)
        {
            var var = new VariableNode(graph, type);
            graph.AddNode(var);
            return var;
        }

        public static bool Expand(this Graph graph)
        {
            var output = graph.ServiceProvider.Get<IOutputManager>();

            if (!graph.IsEntryConnectedToExit())
            {
                output.Write($"Graph {graph.Name} is not returning.");
                return false;
            }

            foreach (var node in graph.Nodes)
            {
                if(node is GraphNode gr)
                {
                    if(!gr.Expand())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion
    }
}
