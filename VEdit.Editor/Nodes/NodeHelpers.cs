using System.Linq;
using VEdit.Common;

namespace VEdit.Editor
{
    public static class NodeHelpers
    {
        public static bool IsEntryOrExit(this Node node)
        {
            return node is EntryNode || node is ExitNode;
        }

        public static bool IsGetOrSet(this Node node)
        {
            return node is GetNode || node is SetNode;
        }

        public static bool Expand(this GraphNode node)
        {
            var output = node.ServiceProvider.Get<IOutputManager>();

            var entryGraph = node.LinkedGraph.Entry.Output;
            var exitGraph = node.LinkedGraph.Exit.Input;

            if (entryGraph.Count != node.Input.Count || exitGraph.Count != node.Output.Count)
            {
                output.Write($"Graph node is outdated: {node.FullName}. Pin count not matching.");
                return false;
            }

            for (int i = 0; i < entryGraph.Count; i++)
            {
                var entryPin = entryGraph[i];
                var nodePin = node.Input[i];

                if (entryPin.IsExec == nodePin.IsExec == true)
                {
                    if (nodePin.IsConnected)
                    {
                        if (entryPin.IsConnected)
                        {
                            var conGraphPin = entryPin.Links.Select(l => l.Input).First();
                            var conNodePins = nodePin.Links.Select(l => l.Output).ToArray();

                            nodePin.BreakLinks();
                            entryPin.BreakLinks();

                            foreach (var conNodePin in conNodePins)
                            {
                                node.Graph.AddLink(new Link(node.Graph, conGraphPin, conNodePin));
                            }
                        }
                    }
                }
                else if (entryPin.IsExec == nodePin.IsExec == false)
                {
                    if (entryPin.Type != nodePin.Type)
                    {
                        output.Write($"Graph node is outdated: {node.FullName}. Pin types not matching.");
                        return false;
                    }

                    if (nodePin.IsConnected)
                    {
                        if (entryPin.IsConnected)
                        {
                            var conGraphPins = entryPin.Links.Select(l => l.Input).ToArray();
                            var conNodePin = nodePin.Links.Select(l => l.Output).First();

                            nodePin.BreakLinks();
                            entryPin.BreakLinks();

                            foreach (var conGraphPin in conGraphPins)
                            {
                                node.Graph.AddLink(new Link(node.Graph, conGraphPin, conNodePin));
                            }
                        }
                    }
                    else
                    {
                        var conGraphPins = entryPin.Links.Select(l => l.Output).ToArray();
                        foreach (var pin in conGraphPins)
                        {
                            pin.DefaultValue = nodePin.DefaultValue;
                        }
                    }
                }
                else
                {
                    output.Write($"Graph node is outdated: {node.FullName}. Pin types not matching.");
                    return false;
                }
            }

            for (int i = 0; i < exitGraph.Count; i++)
            {
                var exitPin = exitGraph[i];
                var nodePin = node.Output[i];

                if (exitPin.IsExec == nodePin.IsExec == true)
                {
                    if (nodePin.IsConnected)
                    {
                        if (exitPin.IsConnected)
                        {
                            var conGraphPins = exitPin.Links.Select(l => l.Output).ToArray();
                            var conNodePin = nodePin.Links.Select(l => l.Input).First();

                            nodePin.BreakLinks();
                            exitPin.BreakLinks();

                            foreach (var conGraphPin in conGraphPins)
                            {
                                node.Graph.AddLink(new Link(node.Graph, conGraphPin, conNodePin));
                            }
                        }
                    }
                }
                else if (exitPin.IsExec == nodePin.IsExec == false)
                {
                    if (exitPin.Type != nodePin.Type)
                    {
                        output.Write($"Graph node is outdated: {node.FullName}. Pin types not matching.");
                        return false;
                    }

                    if (nodePin.IsConnected)
                    {
                        if (exitPin.IsConnected)
                        {
                            var conGraphPin = exitPin.Links.Select(l => l.Output).First();
                            var conNodePins = nodePin.Links.Select(l => l.Input).ToArray();

                            nodePin.BreakLinks();
                            exitPin.BreakLinks();

                            foreach (var conNodePin in conNodePins)
                            {
                                node.Graph.AddLink(new Link(node.Graph, conGraphPin, conNodePin));
                            }
                        }
                        else
                        {
                            var conNodePins = nodePin.Links.Select(l => l.Input).ToArray();
                            foreach (var pin in conNodePins)
                            {
                                pin.DefaultValue = nodePin.DefaultValue;
                            }
                        }
                    }
                }
                else
                {
                    output.Write($"Graph node is outdated: {node.FullName}. Pin types not matching.");
                    return false;
                }
            }

            return node.LinkedGraph.Expand();
        }
    }
}
